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
    protected int curHp;

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

    protected Action onComplete;

    protected UnitAnim anim;

    public abstract void Init(string name, int level);
    public abstract void Enter();

    public abstract void Hit(float dmg, bool isCritical);
    public abstract bool IsFullStack();
    public abstract bool IsDie();

    public abstract void UseSkill(List<Unit> targets, Action onComplete);
    public abstract IEnumerator UseSkillCo();

    public abstract void Die();
}
