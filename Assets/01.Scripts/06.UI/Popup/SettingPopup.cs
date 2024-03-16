using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : APopup
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button exitBtn;
    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Exit();
        });
        
        exitBtn.onClick.AddListener(() =>
        {
            UIManager.Inst.ShowFadePanel(() =>
            {
                UIManager.Inst.GameExit();
            });

            Exit();
        });
    }

    public void Enter(bool isInGame)
    {
        exitBtn.gameObject.SetActive(isInGame);

        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
