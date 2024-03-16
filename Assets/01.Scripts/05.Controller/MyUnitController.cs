using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUnitController : UnitController<MyUnit>
{
    [SerializeField] Vector3[] _createPos;

    public void Start()
    {
        _unitRoot = this.gameObject.transform;
    }

    public override void Enter()
    {
        _unitList = new List<Unit>();
    }

    public override void Exit()
    {
        foreach (var unit in _unitList)
        {
            if (unit == null)
                continue;

            Destroy(unit.gameObject);
        }

        _unitList = null;
    }

    public override bool ChackExtermination()
    {
        bool rtn = false;

        foreach (var unit in _unitList)
        {
            if (unit.CurState == AnimState.Die)
            {
                rtn = true;
                break;
            }
        }

        return rtn;
    }

    public override void InsetUnit(int idx, string unitName, int level)
    {
        GameObject prefab = Resources.Load(string.Format("Unit/MyUnit/{0}", unitName)) as GameObject;
        MyUnit addUnit = Instantiate(prefab, _createPos[idx], Quaternion.identity, _unitRoot).GetComponent<MyUnit>();
        addUnit.Init(unitName, level);
        AddCardCollectionData(addUnit);
        addUnit.Enter();

        UIManager.Inst.Game.UnitSlotUI.InitUnitSlot(idx, addUnit);

        _unitList.Add(addUnit);
    }

    public override void RemoveUnit(string unitName)
    {
        Unit removeUnit = _unitList.Find(x => x.name == unitName);

        _unitList.Remove(removeUnit);
    }

    public void AddStack(eColor color, int stack)
    {
        foreach (MyUnit myUnit in _unitList)
        {
            if (myUnit.IsDie())
                continue;

            myUnit.AddStack(color, stack);
        }
    }

    public override List<Unit> FindFullStackUnit()
    {
        List<Unit> fullStackUnitList = new List<Unit>();

        foreach (MyUnit myUnit in _unitList)
        {
            if (myUnit.IsDie())
                continue;

            if (myUnit.IsFullStack())
                fullStackUnitList.Add(myUnit);
        }

        return fullStackUnitList;
    }

    public override List<Unit> FindLiveUnit()
    {
        List<Unit> liveUnitList = new List<Unit>();

        foreach (MyUnit myUnit in _unitList)
        {
            if (myUnit.IsDie() == false)
                liveUnitList.Add(myUnit);
        }

        return liveUnitList;
    }

    public override void AddCardCollectionData(MyUnit unit)
    {
        var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);
        var mcdm = (MyCardCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyCardCollectionDataModel);

        foreach (var kind in Enum.GetValues(typeof(eCardKindStat)))
        {
            var myCardList = mcdm.GetSelectMyCardCollectionDataList((int)eCardOption.Buff, (int)eCardType.Statistics, (int)kind);
            int value = cdm.GetCardSumValue(myCardList);
            if (value == 0)
                continue;

            switch (kind)
            {
                case eCardKindStat.Atk:
                    unit.skillList[0].Value = Mathf.RoundToInt(unit.skillList[0].Value * (1f + ((float)value / 100f)));
                    unit.skillList[1].Value = Mathf.RoundToInt(unit.skillList[1].Value * (1f + ((float)value / 100f)));
                    break;
                case eCardKindStat.Hp:
                    unit.maxHp = Mathf.RoundToInt(unit.maxHp * (1f + ((float)value / 100f)));
                    break;
                case eCardKindStat.Def:
                    unit.defence = Mathf.RoundToInt(unit.defence * (1f + ((float)value / 100f)));
                    break;
                case eCardKindStat.Cri:
                    unit.critical_Per = Mathf.RoundToInt(unit.critical_Per * (1f + ((float)value / 100f)));
                    break;
                default:
                    break;
            }
        }

        foreach (var kind in Enum.GetValues(typeof(eCardKindStack)))
        {
            var myCardList = mcdm.GetSelectMyCardCollectionDataList((int)eCardOption.Buff, (int)eCardType.SkillStack, (int)kind);
            int value = cdm.GetCardSumValue(myCardList);
            if (value == 0)
                continue;

            foreach (var skillData in unit.skillList)
            {
                if ((int)skillData.Color == (int)kind)
                    skillData.Stack = Mathf.RoundToInt(skillData.Stack * (1f - ((float)value / 100f)));
            }
        }
    }
}
