using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using DG.Tweening;

public class Enemy : Unit
{
    [SerializeField] SpriteRenderer mainSpr;
    [SerializeField] Text levelTxt;

    int currLevel;

    float maxStack;
    float curStack;
    public int minAtk;
    public int maxAtk;

    int atkCnt = 1;

    Vector3 orgPos;

    public override void Init(int idx, string name, int level)
    {
        Name = name;
        Index = idx;
        currLevel = level;

        EnemyDataModel edm = (EnemyDataModel)DataModelController.Inst.GetDataModel(eDataModel.EnemyDataModel);
        var data = edm.GetData((eEnemy)Enum.Parse(typeof(eEnemy), name), level);

        anim = GetComponentInChildren<UnitAnim>();
        anim.Init();

        atkCnt = data.AttackMotion;

        curStack = 0f;
        maxStack = data.Stack;

        maxHp = data.Hp;

        critical_Per = data.Critical_Per;
        defence = data.Defence;

        minAtk = data.AttackRange[0];
        maxAtk = data.AttackRange[1];

        orgPos = transform.position;
    }

    public override void Enter()
    {
        curHp = maxHp;

        SetLevelText(currLevel);
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
        if (curStack >= maxStack)
            return true;
        else
            return false;
    }

    public override bool IsDie()
    {
        if (CurState == AnimState.Die || curHp <= 0)
            return true;
        else
            return false;
    }

    public void AddStack(int stack)
    {
        curStack += stack;
        if (curStack > maxStack)
            curStack = maxStack;
    }

    public override void Die()
    {
        curStack = 0f;

        CurState = AnimState.Die;

        GameManager.Inst.OnUnitDieEvent(Name);
    }

    public override void CheckUseSkill(List<Unit> targets, Action<List<AttackEvent>> onAddAtkEvents)
    {
        List<AttackEvent> atkList = new List<AttackEvent>();

        if (curStack >= maxStack)
        {
            this.targets = targets;
            curStack -= maxStack;
            int skillIdx = Random.Range(0, atkCnt);
            AttackEvent atkEvent = new AttackEvent(1, this.Name, Index, "Attack", skillIdx);
            atkList.Add(atkEvent);
        }

        onAddAtkEvents?.Invoke(atkList);
    }

    public override IEnumerator UseSkillCo(string skillName)
    {
        Vector3 movePos = targets[0].gameObject.transform.position;
        movePos.x += 0.7f;
        movePos.y += 0.5f;

        bool moveEnd = false;
        transform.DOMove(movePos, 0.35f).OnComplete(() =>
        {
            moveEnd = true;
        });

        yield return new WaitUntil(() => moveEnd == true);

        float dmg = Random.Range(minAtk, maxAtk);
        bool isCri = Random.Range(0, 100) <= critical_Per;
        targets[0].Hit(dmg, isCri);

        moveEnd = false;
        transform.DOMove(orgPos, 0.35f).OnComplete(() =>
        {
            moveEnd = true;
        });

        yield return new WaitUntil(() => moveEnd == true);

        IsAttacking = false;
    }

    void SetHpBar()
    {
        hpBarImg.fillAmount = (float)curHp / (float)maxHp;
    }

    void SetLevelText(int level)
    {
        levelTxt.text = string.Format("Lv.{0}", level);
    }
}
