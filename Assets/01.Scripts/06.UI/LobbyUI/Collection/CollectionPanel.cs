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
        var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
        int updateCollectSlotCnt = 0;
        foreach (var myUnit in udm.MyUnit.GetMyUnitCollectionDataList())
        {
            int grade = udm.GetUnitData(myUnit.Key).Grade;
            if (udm.MyUnit.GetMySelectUnitDataList().Keys.Contains(myUnit.Key))
            {
                int unitIdx = udm.MyUnit.GetMySelectUnitDataList()[myUnit.Key] - 1;
                selectUnitSlotList[unitIdx].Enter(myUnit.Key, grade, myUnit.Value[0]);
            }
            else
            {
                var slot = GetCollectionUnitSlot(updateCollectSlotCnt);
                slot.Enter(myUnit.Key, grade, myUnit.Value[0]);
                updateCollectSlotCnt++;
            }
        }

        // Card Info Update
        int cardIdx = 0;
        var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);
        Dictionary<int, int[]> myCards = cdm.MyCard.GetMyCardCollectionDataList();
        foreach (var cardData in cdm.GetCardDataList())
        {
            var slot = GetCollectionCardSlot(cardData.Key);
            slot.gameObject.transform.SetSiblingIndex(cardIdx);
            if (myCards.ContainsKey(cardData.Key))
            {
                slot.Enter(myCards[cardData.Key][0], cardData.Value.Grade, myCards[cardData.Key][1]);
                cardIdx++;
            }
            else
            {
                slot.Enter(-1, cardData.Value.Grade, 0);
            }
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
