using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnim : MonoBehaviour
{
    private Animator anim = new Animator();
    private string animTrigger = "AnimState";
    int atkCnt = 1;

    private Unit unit;

    public void Init(int atkCnt)
    {
        anim = GetComponent<Animator>();
        unit = GetComponentInParent<Unit>();

        this.atkCnt = atkCnt;
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
        int randomAtt = -1;
        randomAtt = Random.Range(0, atkCnt);
        anim.SetInteger("Attack", randomAtt);
    }

    void Skill()
    {
        anim.SetTrigger("Skill");
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
        StartCoroutine(unit.UseSkillCo());
    }

    void ActionAnimEnd()
    {
        anim.SetInteger("Attack", -1);
        unit.CurState = AnimState.Idle;
    }
}
