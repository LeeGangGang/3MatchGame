using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitChangePopup : APopup
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button changeBtn;

    [SerializeField] ToggleGroup unitTgGp;
    
    Dictionary<eUnit, ChangeUnitSlot> slotList = new Dictionary<eUnit, ChangeUnitSlot>();

    [SerializeField] ChangeUnitSlot slotPrefab;

    eUnit beforeUnit;
    eUnit afterUnit;

    Action<eUnit> onChangeEvent;

    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Exit();
        });

        changeBtn.onClick.AddListener(() =>
        {
            OnClickChangeBtn();
        });
    }

    public void Enter(eUnit before, Action<eUnit> onChange)
    {
        beforeUnit = before;
        onChangeEvent = onChange;

        base.Enter(() =>
        {
            UpdateUI();
        });
    }

    public override void Exit()
    {
        foreach (var slot in slotList.Values)
            slot.Exit();

        base.Exit();
    }

    void UpdateUI()
    {
        var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
        var mudm = (MyUnitCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyUnitCollectionDataModel);
        int idx = 0;
        foreach (var unit in mudm.GetMyUnSelectUnitDataList())
        {
            var slot = GetUnitSlot(unit.Key);
            int grade = udm.GetUnitData(unit.Key).Grade;
            slot.Enter(unit.Key, grade, unit.Value[0]);
            slot.SetSelect(idx == 0);
            slot.gameObject.transform.SetSiblingIndex(idx);

            if (idx == 0)
            {
                afterUnit = unit.Key;
                idx++;
            }
        }
    }

    void OnClickChangeBtn()
    {
        var mudm = (MyUnitCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyUnitCollectionDataModel);
        mudm.SetChangeSelectUnit(beforeUnit, afterUnit);

        onChangeEvent?.Invoke(afterUnit);

        Exit();
    }

    ChangeUnitSlot GetUnitSlot(eUnit type)
    {
        if (slotList.ContainsKey(type) == false)
        {
            ChangeUnitSlot slot = Instantiate(slotPrefab, unitTgGp.transform);
            slot.Init(type, unitTgGp, () =>
            {
                afterUnit = type;
            });
            slotList.Add(type, slot);
        }

        slotList[type].gameObject.SetActive(true);

        return slotList[type];
    }
}
