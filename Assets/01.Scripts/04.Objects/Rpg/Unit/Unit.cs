using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AnimState
{
    Null,
    Idle,
    Walk,
    Attack,
    Skill,
    Hit,
    Die
}

public abstract class Unit : MonoBehaviour
{
    public string Name;
    public int Index;

    protected int curHp;
    public int maxHp;
    public int critical_Per;
    public int defence;

    public bool IsAttacking;

    private AnimState state;
    public AnimState CurState
    {
        get
        {
            return state;
        }
        set
        {
            if (state == value || state == AnimState.Die)
                return;

            state = value;
            anim.Invoke(value.ToString(), 0f);
        }
    }

    [SerializeField] public Transform HitPos;
    [SerializeField] protected Image hpBarImg;

    protected List<Unit> targets;

    protected UnitAnim anim;

    public abstract void Init(int idx, string name, int level);
    public abstract void Enter();

    public abstract void Hit(float dmg, bool isCritical);
    public abstract bool IsFullStack();
    public abstract bool IsDie();

    public abstract void CheckUseSkill(List<Unit> targets, Action<List<AttackEvent>> onAddAtkEvents);
    public virtual void AttackStart(string skillName, int idx)
    {
        IsAttacking = true;
        anim.skillName = skillName;
        if (idx == 0)
            CurState = AnimState.Attack;
        else
            CurState = AnimState.Skill;
    }

    public abstract IEnumerator UseSkillCo(string skillName);

    public abstract void Die();
}
