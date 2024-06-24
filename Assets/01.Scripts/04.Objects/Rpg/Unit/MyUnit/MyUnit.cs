using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

public class MyUnit : Unit
{
    [SerializeField] SpriteRenderer mainSpr;
    
    int currLevel;
    Dictionary<SkillData, int> curStack = new Dictionary<SkillData, int>();
    public Dictionary<SkillData, ASkill> skillList = new Dictionary<SkillData, ASkill>();

    public override void Init(int idx, string name, int level)
    {
        Name = name;
        currLevel = level;

        anim = GetComponentInChildren<UnitAnim>();
        anim.Init();

        var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
        var data = udm.GetUnitData((eUnit)Enum.Parse(typeof(eUnit), name));

        maxHp = data.Hp[level];

        critical_Per = data.Critical_Per[level];
        defence = data.Defence[level];

        foreach (int skillKey in data.Skill)
        {
            var sdm = DataModelController.Inst.GetDataModel(eDataModel.SkillDataModel);
            SkillData sd = Util.DeepCopy(((SkillDataModel)sdm).GetData(skillKey));
            
            Type type = Type.GetType(sd.Name);
            ASkill skill = Activator.CreateInstance(type) as ASkill;
            skill.Init(sd, this);

            skillList.Add(sd, skill);
            curStack.Add(sd, 0);
        }
    }

    public override void Enter()
    {
        curHp = maxHp;

        SetHpBar();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            CurState = AnimState.Idle;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            CurState = AnimState.Walk;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            CurState = AnimState.Attack;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            CurState = AnimState.Skill;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            CurState = AnimState.Hit;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            CurState = AnimState.Die;
    }

    public override void Hit(float dmg, bool isCri)
    {
        if (curHp <= 0)
            return;

        int Dmg = (int)(dmg - defence);
        if (Dmg <= 0)
            Dmg = 1;

        Dmg = isCri ? Dmg * 2 : Dmg;
        curHp -= Dmg;
        if (curHp < 0)
            curHp = 0;

        // ----- DamgeText
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, 0f);
        GameManager.Inst.TextPool.EnterDamge(pos, Dmg.ToString(), isCri);
        // -----

        SetHpBar();

        if (curHp <= 0)
            Die();
        else
            CurState = AnimState.Hit;
    }

    public override bool IsFullStack()
    {
        foreach (var stack in curStack)
        {
            var sdm = (SkillDataModel)DataModelController.Inst.GetDataModel(eDataModel.SkillDataModel);
            int maxStack = sdm.GetData(stack.Key.key).Stack;
            if (stack.Value >= maxStack)
                return true;
        }

        return false;
    }

    public override bool IsDie()
    {
        if (CurState == AnimState.Die || curHp <= 0)
            return true;
        else
            return false;
    }

    public void AddStack(eColor color, int stack)
    {
        var sdm = (SkillDataModel)DataModelController.Inst.GetDataModel(eDataModel.SkillDataModel);
        List<SkillData> keys = new List<SkillData>(curStack.Keys);
        for (int idx = 0; idx < keys.Count; idx++)
        {
            SkillData skill = keys[idx];
            if (skill.Color == color)
            {
                curStack[skill] += stack;
                if (curStack[skill] > sdm.GetData(skill.key).Stack)
                    curStack[skill] = sdm.GetData(skill.key).Stack;

                float amount = (float)curStack[skill] / (float)sdm.GetData(skill.key).Stack;
                UIManager.Inst.Game.UnitSlotUI.SetGaugeFillAmount(this, idx, amount);
            }
        }
    }

    public override void Die()
    {
        curStack = null;

        CurState = AnimState.Die;

        UIManager.Inst.Game.UnitSlotUI.DieUnitSlot(this);

        GameManager.Inst.OnUnitDieEvent(Name);
    }

    public override void AttackStart(string skillName, int idx)
    {
        ASkill skill = skillList.Where(sk => sk.Key.Name == skillName).FirstOrDefault().Value;
        skill.Enter(targets);

        base.AttackStart(skillName, idx);
    }

    public override void CheckUseSkill(List<Unit> targets, Action<List<AttackEvent>> onAddAtkEvents)
    {
        List<AttackEvent> atkList = new List<AttackEvent>();

        var sdm = (SkillDataModel)DataModelController.Inst.GetDataModel(eDataModel.SkillDataModel);
        List<SkillData> keys = new List<SkillData>(curStack.Keys);
        for (int skillIdx = 0; skillIdx < keys.Count; skillIdx++)
        {
            SkillData skill = keys[skillIdx];
            int maxStack = sdm.GetData(skill.key).Stack;
            if (curStack[skill] >= maxStack)
            {
                UIManager.Inst.Game.UnitSlotUI.SetGaugeFillAmount(this, skillIdx, curStack[skill] - maxStack);

                this.targets = targets;
                curStack[skill] -= maxStack;

                AttackEvent atkEvent = new AttackEvent(0, Name, Index, skill.Name, skillIdx);
                atkList.Add(atkEvent);
            }
        }

        onAddAtkEvents?.Invoke(atkList);
    }

    public override IEnumerator UseSkillCo(string skillName)
    {
        ASkill skill = skillList.Where(sk => sk.Key.Name == skillName).FirstOrDefault().Value;
        
        yield return StartCoroutine(skill.During());

        IsAttacking = false;
    }

    void SetHpBar()
    {
        hpBarImg.fillAmount = (float)curHp / (float)maxHp;
    }
}
