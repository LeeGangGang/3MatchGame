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

public class AddItem
{
    public int Key;
    public int Count;
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

        // Time ∞ªΩ≈
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
        // æ∆¿Ã≈€ »πµÊ »Ωºˆ
        int buyCollectionCnt = 0;

        UIManager.Inst.SetActiveLoading(true);

        var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
        var scdm = (StoreChartDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreChartDataModel);
        var sdm = (StoreDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreDataModel);

        var shopData = sdm.GetData(id);
        int price = shopData.price;
        int cnt = shopData.count;
        
        Dictionary<eProductType, Dictionary<int, int>> addItemsList = new Dictionary<eProductType, Dictionary<int, int>>();
        switch (id)
        {
            case eProductID.Gold200:
                {
                    // ADS ø¨µø
                    Dictionary<int, int> additems = new Dictionary<int, int>();
                    additems.Add(0, cnt);
                    addItemsList.Add(eProductType.Gold, additems);

                    mwdm.Gold += cnt;
                }
                break;
            case eProductID.Gold1000:
            case eProductID.Gold2000:
            case eProductID.Gold5000:
            case eProductID.Gold10000:
                if (mwdm.Dia >= price)
                {
                    Dictionary<int, int> additems = new Dictionary<int, int>();
                    additems.Add(0, cnt);
                    addItemsList.Add(eProductType.Gold, additems);

                    mwdm.Gold += cnt;
                    mwdm.Dia -= price;
                }
                break;
            case eProductID.Dia10:
                {
                    // ADS ø¨µø
                    Dictionary<int, int> additems = new Dictionary<int, int>();
                    additems.Add(0, cnt);
                    addItemsList.Add(eProductType.Dia, additems);

                    mwdm.Dia += cnt;
                }
                break;
            case eProductID.Dia100:
            case eProductID.Dia600:
            case eProductID.Dia1500:
            case eProductID.Dia5500:
                {
                    // ¿Œ æ€ ø¨µø
                    Dictionary<int, int> additems = new Dictionary<int, int>();
                    additems.Add(0, cnt);
                    addItemsList.Add(eProductType.Dia, additems);

                    mwdm.Dia += cnt;
                }
                break;
            case eProductID.NormalCardPack:
            case eProductID.MagicCardPack:
            case eProductID.RareCardPack:
            case eProductID.UniqCardPack:
                if (mwdm.Gold >= price)
                {
                    var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);
                    int code = scdm.GetCardCode(id);
                    if (addItemsList.ContainsKey(eProductType.Card))
                    {
                        if (addItemsList[eProductType.Card].ContainsKey(code))
                            addItemsList[eProductType.Card][code]++;
                        else
                            addItemsList[eProductType.Card].Add(code, 1);
                    }
                    else
                    {
                        Dictionary<int, int> additems = new Dictionary<int, int>();
                        additems.Add(code, 1);
                        addItemsList.Add(eProductType.Card, additems);
                    }

                    cdm.MyCard.SetAddCard(code, 1);
                    mwdm.Gold -= price;

                    buyCollectionCnt = 1;
                }
                break;
            case eProductID.NormalUnitPack:
            case eProductID.MagicUnitPack:
            case eProductID.RareUnitPack:
            case eProductID.UniqUnitPack:
                if (mwdm.Gold >= price)
                {
                    var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
                    eUnit code = scdm.GetUnitCode(id);
                    if (addItemsList.ContainsKey(eProductType.Unit))
                    {
                        if (addItemsList[eProductType.Unit].ContainsKey((int)code))
                            addItemsList[eProductType.Unit][(int)code]++;
                        else
                            addItemsList[eProductType.Unit].Add((int)code, 1);
                    }
                    else
                    {
                        Dictionary<int, int> additems = new Dictionary<int, int>();
                        additems.Add((int)code, 1);
                        addItemsList.Add(eProductType.Unit, additems);
                    }

                    udm.MyUnit.SetAddUnit(code, 1);
                    mwdm.Gold -= price;

                    buyCollectionCnt = 1;
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
                    var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);
                    int[] addCardList = new int[cnt];
                    for (int i = 0; i < cnt; i++)
                    {
                        int code = scdm.GetCardCode(id);
                        addCardList[i] = code;

                        if (addItemsList.ContainsKey(eProductType.Card))
                        {
                            if (addItemsList[eProductType.Card].ContainsKey(code))
                                addItemsList[eProductType.Card][code]++;
                            else
                                addItemsList[eProductType.Card].Add(code, 1);
                        }
                        else
                        {
                            Dictionary<int, int> additems = new Dictionary<int, int>();
                            additems.Add(code, 1);
                            addItemsList.Add(eProductType.Card, additems);
                        }
                    }

                    foreach (var card in addItemsList[eProductType.Card])
                        cdm.MyCard.SetAddCard(card.Key, card.Value);

                    mwdm.Gold -= price;

                    buyCollectionCnt = 10;
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
                    var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
                    int[] addCardList = new int[cnt];
                    for (int i = 0; i < cnt; i++)
                    { 
                        eUnit code = scdm.GetUnitCode(id);
                        addCardList[i] = (int)code;

                        if (addItemsList.ContainsKey(eProductType.Unit))
                        {
                            if (addItemsList[eProductType.Unit].ContainsKey((int)code))
                                addItemsList[eProductType.Unit][(int)code]++;
                            else
                                addItemsList[eProductType.Unit].Add((int)code, 1);
                        }
                        else
                        {
                            Dictionary<int, int> additems = new Dictionary<int, int>();
                            additems.Add((int)code, 1);
                            addItemsList.Add(eProductType.Unit, additems);
                        }
                    }

                    foreach (var unit in addItemsList[eProductType.Unit])
                        udm.MyUnit.SetAddUnit((eUnit)unit.Key, unit.Value);

                    mwdm.Gold -= price;

                    buyCollectionCnt = 10;
                }
                break;
        }

        if (addItemsList.Count > 0)
            UIManager.Inst.SetAddItemPanel(addItemsList);

        if (buyCollectionCnt > 0)
        {
            var mdm = (MissionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MissionDataModel);
            foreach (int key in mdm.GetKeys(eMissionKind.BuyItem))
                mdm.MyMission.SetAddMission(key, buyCollectionCnt);
        }

        UIManager.Inst.SetActiveLoading(false);
    }
}
