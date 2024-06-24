using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnim : MonoBehaviour
{
    private Animator anim = new Animator();
    private string animTrigger = "AnimState";

    private Unit unit;

    public string skillName;

    public void Init()
    {
        anim = GetComponent<Animator>();
        unit = GetComponentInParent<Unit>();
    }

    void Idle()
    {
        anim.SetInteger(animTrigger, 0);
    }

    void Walk()
    {
        anim.SetInteger(animTrigger, 1);
    }

    void Attack()
    {
        anim.SetInteger("Attack", 0);
    }

    void Skill()
    {
        anim.SetInteger("Attack", 1);
    }

    void Hit()
    {
        anim.SetTrigger("Hit");
    }

    void Die()
    {
        anim.SetBool("Die", true);

        //Destroy(this.gameObject);
    }

    void AttackStart()
    {
        StartCoroutine(unit.UseSkillCo(skillName));
    }

    void ActionAnimEnd()
    {
        anim.SetInteger("Attack", -1);
        unit.CurState = AnimState.Idle;
    }
}
