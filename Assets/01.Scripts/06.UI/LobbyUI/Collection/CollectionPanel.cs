using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionPanel : ALobbyPanel
{
    [Header("Unit Info")]
    [SerializeField] List<SelectUnitSlot> selectUnitSlotList = new List<SelectUnitSlot>();
    [SerializeField] Transform collectUnitSlotTr;
    [SerializeField] CollectionUnitSlot cusPrefab;

    List<CollectionUnitSlot> collectUnitSlotList = new List<CollectionUnitSlot>();

    [Header("Card Info")]
    [SerializeField] Transform collectCardSlotTr;
    [SerializeField] CollectionCardSlot ccsPrefab;

    Dictionary<int, CollectionCardSlot> collectionCardSlotList = new Dictionary<int, CollectionCardSlot>();

    public override void Init()
    {
        type = eLobbyPanel.Collection;
        isEnter = false;

        foreach (var selectUnitSlot in selectUnitSlotList)
            selectUnitSlot.Init();

        var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);

        foreach (var cardData in cdm.GetCardDataList())
        {
            var slot = GetCollectionCardSlot(cardData.Key);
            slot.Enter(-1, 0);
        }
    }

    public override void Enter()
    {
        if (isEnter)
            return;

        isEnter = true;

        UpdateUI();

        gameObject.SetActive(true);
    }

    public override void Exit()
    {
        if (!isEnter)
            return;

        foreach (var slot in collectUnitSlotList)
            slot.Exit();

        isEnter = false;
        gameObject.SetActive(false);
    }

    public override void UpdateUI()
    {
        // Unit Info Update
        var mudm = (MyUnitCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyUnitCollectionDataModel);
        int updateCollectSlotCnt = 0;
        foreach (var myUnit in mudm.GetMyUnitCollectionDataList())
        {
            if (mudm.GetMySelectUnitDataList().Keys.Contains(myUnit.Key))
            {
                int unitIdx = mudm.GetMySelectUnitDataList()[myUnit.Key] - 1;
                selectUnitSlotList[unitIdx].Enter(myUnit.Key, myUnit.Value[0]);
            }
            else
            {
                var slot = GetCollectionUnitSlot(updateCollectSlotCnt);
                slot.Enter(myUnit.Key, myUnit.Value[0]);
                updateCollectSlotCnt++;
            }
        }

        // Card Info Update
        int cardIdx = 0;
        var mcdm = (MyCardCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyCardCollectionDataModel);
        foreach (var myCard in mcdm.GetMyCardCollectionDataList())
        {
            var slot = collectionCardSlotList[myCard.Key];
            slot.Enter(myCard.Value[0], myCard.Value[1]);
            slot.gameObject.transform.SetSiblingIndex(cardIdx);
            cardIdx++;
        }
    }

    CollectionUnitSlot GetCollectionUnitSlot(int idx)
    {
        if (collectUnitSlotList.Count <= idx)
        {
            CollectionUnitSlot slot = Instantiate(cusPrefab, collectUnitSlotTr);
            slot.Init();

            collectUnitSlotList.Insert(idx, slot);
        }

        collectUnitSlotList[idx].gameObject.SetActive(true);

        return collectUnitSlotList[idx];
    }

    CollectionCardSlot GetCollectionCardSlot(int code)
    {
        if (!collectionCardSlotList.ContainsKey(code))
        {
            CollectionCardSlot slot = Instantiate(ccsPrefab, collectCardSlotTr);
            slot.Init(code);

            collectionCardSlotList.Add(code, slot);
        }

        collectionCardSlotList[code].gameObject.SetActive(true);

        return collectionCardSlotList[code];
    }
}
