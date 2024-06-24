using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSlotUI : MonoBehaviour
{
    Dictionary<Unit, UnitSlot> _myUnitSlotList = new Dictionary<Unit, UnitSlot>();
    List<UnitSlot> _unitSlots = new List<UnitSlot>();

    public void Enter()
    {
        if (_unitSlots.Count == 0)
            _unitSlots = GetComponentsInChildren<UnitSlot>().ToList();

        _myUnitSlotList = new Dictionary<Unit, UnitSlot>();
    }

    public void Exit()
    {
        _myUnitSlotList = null;
    }

    public void InitUnitSlot(int idx, MyUnit unit)
    {
        UnitSlot myUnitSlot = _unitSlots[idx];
        myUnitSlot.Enter(unit);
        _myUnitSlotList.Add(unit, myUnitSlot);
    }

    public void SetGaugeFillAmount(Unit unit, int idx, float amount)
    {
        _myUnitSlotList[unit].SetGaugeFillAmount(idx, amount);
    }

    public void DieUnitSlot(Unit unit)
    {
        _myUnitSlotList[unit].SetActiveDieImage(true);
    }
}
