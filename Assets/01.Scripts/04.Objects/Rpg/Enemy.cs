using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Enemy : Unit
{
    [SerializeField] SpriteRenderer mainSpr;
    [SerializeField] Text levelTxt;

    int currLevel;

    float maxStack;
    int curStack;

    public int maxHp;
    public int critical_Per;
    public int minAtk;
    public int maxAtk;
    public int defence;

    public override void Init(string name, int level)
    {
        currLevel = level;

        EnemyDataModel edm = (EnemyDataModel)DataModelController.Inst.GetDataModel(eDataModel.EnemyDataModel);
        var data = edm.GetData((eEnemy)Enum.Parse(typeof(eEnemy), name), level);

        anim = GetComponentInChildren<UnitAnim>();
        anim.Init(data.AttackMotion);

        curStack = 0;
        maxStack = data.Stack;

        maxHp = data.Hp;

        critical_Per = data.Critical_Per;
        defence = data.Defence;

        minAtk = data.AttackRange[0];
        maxAtk = data.AttackRange[1];
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
    }

    public override void Die()
    {
        curStack = 0;
        CurState = AnimState.Die;
    }

    public override void UseSkill(List<Unit> targets, Action onComplete)
    {
        if (curStack >= maxStack)
        {
            this.targets = targets;
            this.onComplete = onComplete;

            CurState = AnimState.Attack;
        }
    }

    public override IEnumerator UseSkillCo()
    {
        float dmg = Random.Range(minAtk, maxAtk);
        bool isCri = Random.Range(0, 100) <= critical_Per;
        
        this.targets[0].Hit(dmg, isCri);

        yield return null;
        onComplete?.Invoke();
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
