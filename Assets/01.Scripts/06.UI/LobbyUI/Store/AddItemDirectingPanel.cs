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

    public void Enter(eProductType type, params int[] items)
    {
        int idx = 0;
        foreach (var item in items)
        {
            Sprite spr = null;
            int grade = -1;
            if (type == eProductType.Unit)
            {
                var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
                var data = udm.GetUnitData((eUnit)item);
                spr = AtlasManager.Inst.GetSprite(eAtlasType.Unit, data.Name);
                grade = data.Grade;
            }
            else
            {
                var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);
                var data = cdm.GetCardData(item);
                spr = AtlasManager.Inst.GetSprite(eAtlasType.Card, string.Format("card{0}{1}", data.Code, data.Grade));
                grade = data.Grade;
            }

            itemList[idx].Init(spr, grade);
            idx++;
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
        mainImg.DOColor(Color.black, fadeTime).OnComplete(() =>
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
