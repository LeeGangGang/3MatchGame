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
    public int TargetCnt;

    public int Value;
    public string Name;
}

public abstract class ASkill : MonoBehaviour
{
    public SkillData data = new SkillData();

    public abstract void Enter(List<Unit> target);
    public abstract IEnumerator During();
    public abstract void Exit();
}
