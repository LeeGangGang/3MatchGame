using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Unit Target;

    [SerializeField] string HitFxName;

    public void Enter(Unit target)
    {
        Target = target;
        SetActive(true);
    }

    public void Exit()
    {
        ShowHitFx();

        Destroy();
    }

    void ShowHitFx()
    {
        eFxID hitFxId = (eFxID)Enum.Parse(typeof(eFxID),HitFxName);
        FxManager.Inst.EnterFx(hitFxId, transform.position);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
