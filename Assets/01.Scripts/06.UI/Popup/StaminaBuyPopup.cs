using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBuyPopup : APopup
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button buyBtn;
    [SerializeField] Button adBtn;

    [SerializeField] Text goldTxt;

    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Exit();
        });

        buyBtn.onClick.AddListener(() =>
        {
            OnClickBtn(ePriceType.Gold);
        });

        adBtn.onClick.AddListener(() =>
        {
            OnClickBtn(ePriceType.AD);
        });
    }

    public void Enter()
    {
        base.Enter(() =>
        {
            UpdateUI();
        });
    }

    public override void Exit()
    {
        base.Exit();
    }

    void UpdateUI()
    {
        var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
        if (mwdm.Gold >= 100)
        {
            goldTxt.color = Color.white;
            buyBtn.interactable = true;
        }
        else
        {
            goldTxt.color = Color.red;
            buyBtn.interactable = false;
        }
    }

    void OnClickBtn(ePriceType type)
    {
        if (type == ePriceType.AD)
        {
            // AD ¿¬µ¿
        }
        else
        {
            var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
            mwdm.Gold -= 100;
            mwdm.LifeCnt++;
        }

        UpdateUI();
    }
}