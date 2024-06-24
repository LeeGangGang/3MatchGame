using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillData
{
    public int key;
    public int Type;
    public eColor Color;
    public int Stack;

    public int Value;
    public string Name;
}

public abstract class ASkill
{
    protected SkillData data = new SkillData();
    protected Unit unit;
    protected List<Unit> targets;

    public virtual void Init(SkillData data, Unit unit)
    {
        this.data = data;
        this.unit = unit;
    }

    public abstract void Enter(List<Unit> targets);
    public abstract IEnumerator During();
    public abstract void Exit();
}
