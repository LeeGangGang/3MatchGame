using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : APopup
{
    [SerializeField] Button closeBtn;
    [SerializeField] Text resultTxt;
    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            UIManager.Inst.ShowFadePanel(() =>
            {
                UIManager.Inst.GameExit();
            });

            Exit();
        });
    }

    public void Enter(bool isClear)
    {
        SetResultText(isClear);

        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    void SetResultText(bool isClear)
    {
        resultTxt.text = isClear ? "CLEAR" : "FAILED";
    }
}
