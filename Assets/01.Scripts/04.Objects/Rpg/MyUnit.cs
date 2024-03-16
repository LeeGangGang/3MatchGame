using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

public class MyUnit : Unit
{
    [SerializeField] SpriteRenderer mainSpr;
    
    public string Name;
    int currLevel;

    Dictionary<SkillData, int> curStack = new Dictionary<SkillData, int>();
    public List<SkillData> skillList = new List<SkillData>();

    string curUseSkillName;

    public int maxHp;
    public int critical_Per;
    public int defence;

    public override void Init(string name, int level)
    {
        Name = name;
        currLevel = level;

        anim = GetComponentInChildren<UnitAnim>();
        anim.Init(2);

        var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
        var data = udm.GetUnitData((eUnit)Enum.Parse(typeof(eUnit), name));

        maxHp = data.Hp[level];

        critical_Per = data.Critical_Per[level];
        defence = data.Defence[level];

        foreach (int skillKey in data.Skill)
        {
            var sdm = DataModelController.Inst.GetDataModel(eDataModel.SkillDataModel);
            SkillData sd = Util.DeepCopy(((SkillDataModel)sdm).GetData(skillKey));
            skillList.Add(sd);
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
                float amount = (float)curStack[skill] / (float)sdm.GetData(skill.key).Stack;
                UIManager.Inst.Game.UnitSlotUI.SetGaugeFillAmount(this, idx, amount);
            }
        }
    }

    public override void Die()
    {
        curStack = null;

        CurState = AnimState.Die;
    }

    public override void UseSkill(List<Unit> targets, Action onComplete)
    {
        var sdm = (SkillDataModel)DataModelController.Inst.GetDataModel(eDataModel.SkillDataModel);
        int idx = 0;
        foreach (var stack in curStack)
        {
            if (stack.Value >= sdm.GetData(stack.Key.key).Stack)
            {
                UIManager.Inst.Game.UnitSlotUI.SetGaugeFillAmount(this, idx, 0);
                this.targets = targets;
                this.onComplete = onComplete;
                curUseSkillName = stack.Key.Name;
                CurState = AnimState.Attack;
            }
            idx++;
        }
    }

    public override IEnumerator UseSkillCo()
    {
        GameObject go = Resources.Load(string.Format("SkillObject/{0}", curUseSkillName)) as GameObject;

        float angle = Mathf.Atan2(this.targets[0].transform.position.y, this.targets[0].transform.position.x) * Mathf.Rad2Deg;
        float a_angle = angle + UnityEngine.Random.Range(-10, 10);
        Quaternion rot = Quaternion.AngleAxis(a_angle, Vector3.forward);
        ASkill skill = Instantiate(go, transform.position, rot).GetComponent<ASkill>();

        skill.Enter(this.targets);
        yield return StartCoroutine(skill.During());

        onComplete?.Invoke();
    }

    void SetHpBar()
    {
        hpBarImg.fillAmount = (float)curHp / (float)maxHp;
    }
}
