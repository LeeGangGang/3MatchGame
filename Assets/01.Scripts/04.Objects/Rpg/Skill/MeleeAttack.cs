using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : ASkill
{
    Vector3 orgPos;
    bool moveEnd = false;

    public override void Init(SkillData data, Unit unit)
    {
        base.Init(data, unit);
    }

    public override void Enter(List<Unit> targets)
    {
        orgPos = unit.transform.position;
        this.targets = targets;

        Vector3 movePos = targets[0].transform.position;
        
        moveEnd = false;
        unit.transform.DOMove(movePos, 0.35f).OnComplete(() =>
        {
            moveEnd = true;
        });
    }

    public override void Exit()
    {

    }

    public override IEnumerator During()
    {
        yield return new WaitUntil(() => moveEnd == true);

        bool isCri = Random.Range(0, 100) <= unit.critical_Per;

        FxManager.Inst.EnterFx(eFxID.MeleeAttack, targets[0].transform.position);
        targets[0].Hit(data.Value, isCri);

        unit.transform.DOMove(orgPos, 0.35f).OnComplete(() =>
        {
            moveEnd = true;
        });

        yield return new WaitUntil(() => moveEnd == true);
    }
}
