using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class AddItemDirectingPanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] List<AddItemDirecting> itemList = new List<AddItemDirecting>();
    [SerializeField] Image mainImg;

    [SerializeField] float fadeTime;

    public void Enter(Dictionary<eProductType, Dictionary<int, int>> addItems)
    {
        int idx = 0;
        foreach (var addItem in addItems)
        {
            Sprite spr = null;
            int[] value = new int[2];
            if (addItem.Key == eProductType.Gold)
            {
                spr = AtlasManager.Inst.GetSprite(eAtlasType.UI, "coin_05");
                value[0] = addItem.Value[0];

                itemList[idx].Init(addItem.Key, spr, value);
                idx++;
            }
            else if (addItem.Key == eProductType.Dia)
            {
                spr = AtlasManager.Inst.GetSprite(eAtlasType.UI, "dia");
                value[0] = addItem.Value[0];

                itemList[idx].Init(addItem.Key, spr, value);
                idx++;
            }
            else if (addItem.Key == eProductType.Unit)
            {
                var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
                foreach (var unit in addItem.Value)
                {
                    var data = udm.GetUnitData((eUnit)unit.Key);
                    spr = AtlasManager.Inst.GetSprite(eAtlasType.Unit, data.Name);

                    value[0] = unit.Value;
                    value[1] = data.Grade;

                    itemList[idx].Init(addItem.Key, spr, value);
                    idx++;
                }
            }
            else if (addItem.Key == eProductType.Card)
            {
                var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);
                foreach (var card in addItem.Value)
                {
                    var data = cdm.GetCardData(card.Key);
                    spr = AtlasManager.Inst.GetSprite(eAtlasType.Card, string.Format("card{0}{1}", data.Code, data.Grade));

                    value[0] = card.Value;
                    value[1] = data.Grade;

                    itemList[idx].Init(addItem.Key, spr, value);
                    idx++;
                }
            }
        }

        gameObject.SetActive(true);

        StartCoroutine(ShowAnim(idx));
    }

    public void Exit()
    {
        foreach (var itemSlot in itemList)
            itemSlot.Exit();

        gameObject.SetActive(false);
        mainImg.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Exit();
    }

    IEnumerator ShowAnim(int cnt)
    {
        bool endFx = false;
        Color fabeColor = new Color(0f, 0f, 0f, 0.9f);
        mainImg.DOColor(fabeColor, fadeTime).OnComplete(() =>
        {
            endFx = true;
        });

        yield return new WaitUntil(() => endFx);

        for (int i = 0; i < cnt; i++)
        {
            itemList[i].Enter();
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }
}
