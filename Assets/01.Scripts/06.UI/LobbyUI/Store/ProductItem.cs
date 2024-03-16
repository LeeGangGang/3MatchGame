using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eProductID
{
    Gold200,
    Gold1000,
    Gold2000,
    Gold5000,
    Gold10000,

    Dia10,
    Dia100,
    Dia600,
    Dia1500,
    Dia5500,

    NormalCardPack,
    MagicCardPack,
    RareCardPack,
    UniqCardPack,
    NormalCardPack_x10,
    MagicCardPack_x10,
    RareCardPack_x10,
    UniqCardPack_x10,

    NormalUnitPack,
    MagicUnitPack,
    RareUnitPack,
    UniqUnitPack,
    NormalUnitPack_x10,
    MagicUnitPack_x10,
    RareUnitPack_x10,
    UniqUnitPack_x10,
}

public class ProductItem : MonoBehaviour
{
    [SerializeField] eProductID productID;

    [SerializeField] Image iconImg = null;
    [SerializeField] Text priceTxt = null;
    [SerializeField] Text nameTxt = null;
    [SerializeField] Button buyBtn = null;

    public void Init(Action<eProductID> onComplete)
    {
        SetNameText();
        SetPriceText();
        SetBuyBtnEvent(onComplete);
    }

    void SetNameText()
    {
        var ldm = (LocalizationDataModel)DataModelController.Inst.GetDataModel(eDataModel.LocalizationDataModel);
        string str = ldm.GetString(string.Format("shop_{0}", productID));
        nameTxt.text = str;
    }

    void SetPriceText()
    {
        string price = string.Empty;

        var sdm = (StoreDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreDataModel);
        var data = sdm.GetData(productID);
        if (data.priceType == ePriceType.Gold || data.priceType == ePriceType.Dia)
            price = string.Format("{0:N0}", sdm.GetData(productID).price);
        else if (data.priceType == ePriceType.InApp)
            price = string.Format("{0:N0}", sdm.GetData(productID).price);
        else
            price = "AD";

        priceTxt.text = price;
    }

    void SetBuyBtnEvent(Action<eProductID> onComplete)
    {
        buyBtn.onClick.AddListener(() =>
        {
            UIManager.Inst.Popup.OpenProductInfoPopup(productID, iconImg.sprite, onComplete);
        });
    }
}
