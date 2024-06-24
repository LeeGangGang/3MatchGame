using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEvent
{
    public int Type;    // 0 : MyUnit, 1 : Enemy
    public string MyName;
    public string SkillName;
    public int Index;
    public int SkillIndex;

    public AttackEvent(int type, string myName, int idx, string skillName, int skillIdx)
    {
        this.Type = type;
        this.MyName = myName;
        this.Index = idx;
        this.SkillName = skillName;
        this.SkillIndex = skillIdx;
    }
}

public abstract class UnitController<T> : MonoBehaviour
{
    protected Transform unitRoot;

    protected List<Unit> unitList;

    public abstract void Enter();
    public abstract void Exit();

    public abstract bool ChackExtermination();

    public abstract void InsetUnit(int idx, string unitName, int level);
    public abstract void RemoveUnit(string unitName);

    public Unit GetUnit(string name, int idx)
    {
        Unit unit = unitList.Find(unit => unit.Name == name && unit.Index == idx);
        return unit;
    }

    public abstract List<Unit> FindFullStackUnit();
    public abstract List<Unit> FindLiveUnit();

    public abstract void AddCardCollectionData(T unit);
}
