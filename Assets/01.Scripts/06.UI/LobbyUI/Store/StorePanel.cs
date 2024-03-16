using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum eProductType
{
    Gold,
    Dia,
    Unit,
    Card,
}

public class StorePanel : ALobbyPanel
{
    [SerializeField] Toggle goldTabTg;
    [SerializeField] Toggle diaTabTg;
    [SerializeField] Toggle unitTabTg;
    [SerializeField] Toggle cardTabTg;

    Dictionary<eProductType, ProductItemSv> itemSvList = new Dictionary<eProductType, ProductItemSv>();

    public override void Init()
    {
        type = eLobbyPanel.Store;
        isEnter = false;

        itemSvList = FindObjectsOfType<ProductItemSv>(true).ToDictionary(x => x.type);
        foreach (var sv in itemSvList.Values)
        {
            sv.Init(OnClickBuyEvent);
        }

        goldTabTg.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
                itemSvList[eProductType.Gold].Enter();
            else
                itemSvList[eProductType.Gold].Exit();
        });
        diaTabTg.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
                itemSvList[eProductType.Dia].Enter();
            else
                itemSvList[eProductType.Dia].Exit();
        });
        unitTabTg.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
                itemSvList[eProductType.Unit].Enter();
            else
                itemSvList[eProductType.Unit].Exit();
        });
        cardTabTg.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
                itemSvList[eProductType.Card].Enter();
            else
                itemSvList[eProductType.Card].Exit();
        });
    }

    public override void Enter()
    {
        if (isEnter)
            return;

        isEnter = true;

        // Time 갱신
        //

        // Shop Timer Run
        //

        gameObject.SetActive(true);
    }

    public override void Exit()
    {
        if (!isEnter)
            return;

        // Shop Timer Stop

        isEnter = false;
        gameObject.SetActive(false);
    }

    public override void UpdateUI()
    {

    }

    void OnClickBuyEvent(eProductID id)
    {
        UIManager.Inst.SetActiveLoading(true);

        var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
        var scdm = (StoreChartDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreChartDataModel);
        var sdm = (StoreDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreDataModel);

        var shopData = sdm.GetData(id);
        int price = shopData.price;
        int cnt = shopData.count;
        switch (id)
        {
            case eProductID.Gold200:
                // ADS 연동
                mwdm.Gold += cnt;
                break;
            case eProductID.Gold1000:
            case eProductID.Gold2000:
            case eProductID.Gold5000:
            case eProductID.Gold10000:
                if (mwdm.Dia >= price)
                {
                    mwdm.Gold += cnt;
                    mwdm.Dia -= price;
                }
                break;
            case eProductID.Dia10:
                // ADS 연동
                mwdm.Dia += cnt;
                break;
            case eProductID.Dia100:
            case eProductID.Dia600:
            case eProductID.Dia1500:
            case eProductID.Dia5500:
                // 인 앱 연동
                mwdm.Dia += cnt;
                break;
            case eProductID.NormalCardPack:
            case eProductID.MagicCardPack:
            case eProductID.RareCardPack:
            case eProductID.UniqCardPack:
                if (mwdm.Gold >= price)
                {
                    var mcdm = (MyCardCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyCardCollectionDataModel);
                    int code = scdm.GetCardCode(id);
                    mcdm.SetAddCard(code, 1);
                    mwdm.Gold -= price;
                    
                    UIManager.Inst.SetAddItemPanel(eProductType.Card, code);
                }
                break;
            case eProductID.NormalUnitPack:
            case eProductID.MagicUnitPack:
            case eProductID.RareUnitPack:
            case eProductID.UniqUnitPack:
                if (mwdm.Gold >= price)
                {
                    var mudm = (MyUnitCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyUnitCollectionDataModel);
                    eUnit code = scdm.GetUnitCode(id);
                    mudm.SetAddUnit(code, 1);
                    mwdm.Gold -= price;

                    UIManager.Inst.SetAddItemPanel(eProductType.Unit, (int)code);
                }
                break;
            case eProductID.NormalCardPack_x10:
            case eProductID.MagicCardPack_x10:
            case eProductID.RareCardPack_x10:
            case eProductID.UniqCardPack_x10:
                if (id == eProductID.NormalCardPack_x10)
                    id = eProductID.NormalCardPack;
                else if (id == eProductID.MagicCardPack_x10)
                    id = eProductID.MagicCardPack;
                else if (id == eProductID.RareCardPack_x10)
                    id = eProductID.RareCardPack;
                else if (id == eProductID.UniqCardPack_x10)
                    id = eProductID.UniqCardPack;

                if (mwdm.Gold >= price)
                {
                    var mcdm = (MyCardCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyCardCollectionDataModel);
                    int[] addCardList = new int[cnt];
                    for (int i = 0; i < cnt; i++)
                    {
                        int code = scdm.GetCardCode(id);
                        addCardList[i] = code;
                        mcdm.SetAddCard(code, 1);
                    }
                    mwdm.Gold -= price;

                    UIManager.Inst.SetAddItemPanel(eProductType.Card, addCardList);
                }
                break;
            case eProductID.NormalUnitPack_x10:
            case eProductID.MagicUnitPack_x10:
            case eProductID.RareUnitPack_x10:
            case eProductID.UniqUnitPack_x10:
                if (id == eProductID.NormalUnitPack_x10)
                    id = eProductID.NormalUnitPack;
                else if (id == eProductID.MagicUnitPack_x10)
                    id = eProductID.MagicUnitPack;
                else if (id == eProductID.RareUnitPack_x10)
                    id = eProductID.RareUnitPack;
                else if (id == eProductID.UniqUnitPack_x10)
                    id = eProductID.UniqUnitPack;

                if (mwdm.Gold >= price)
                {
                    var mudm = (MyUnitCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyUnitCollectionDataModel);
                    int[] addCardList = new int[cnt];
                    for (int i = 0; i < cnt; i++)
                    { 
                        eUnit code = scdm.GetUnitCode(id);
                        addCardList[i] = (int)code;
                        mudm.SetAddUnit(code, 1);
                    }
                    mwdm.Gold -= price;

                    UIManager.Inst.SetAddItemPanel(eProductType.Unit, addCardList);
                }
                break;
        }

        UIManager.Inst.SetActiveLoading(false);
    }
}
