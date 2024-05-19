using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductInfoPopup : APopup
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button buyBtn;

    [SerializeField] Text nameTxt;
    [SerializeField] Image iconImg;
    [SerializeField] Text priceTxt;

    Action<eProductID> buyEvent;
    eProductID productId;

    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Exit();
        });

        buyBtn.onClick.AddListener(() =>
        {
            OnClickBuyBtn();
        });
    }

    public void Enter(eProductID id, Sprite spr, Action<eProductID> onComplete)
    {
        productId = id;
        string price = string.Empty;
        var sdm = (StoreDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreDataModel);
        var data = sdm.GetData(productId);
        if (data.priceType == ePriceType.Gold || data.priceType == ePriceType.Dia)
            price = string.Format("{0:N0}", sdm.GetData(productId).price);
        else if (data.priceType == ePriceType.InApp)
            price = string.Format("{0:N0}", sdm.GetData(productId).price);
        else
            price = "AD";

        buyEvent = onComplete;

        SetNameText();
        SetIconImage(spr);
        SetPriceText(price);
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    void OnClickBuyBtn()
    {
        buyEvent?.Invoke(productId);
        Exit();
    }

    void SetNameText()
    {
        var ldm = (LocalizationDataModel)DataModelController.Inst.GetDataModel(eDataModel.LocalizationDataModel);
        string str = ldm.GetString(string.Format("shop_{0}", productId));
        nameTxt.text = str;
    }

    void SetIconImage(Sprite spr)
    {
        iconImg.sprite = spr;
    }

    void SetPriceText(string price)
    {
        priceTxt.text = price;
    }
}
