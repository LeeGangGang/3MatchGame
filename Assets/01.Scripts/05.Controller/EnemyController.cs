using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : UnitController<Enemy>
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
        GameObject prefab = Resources.Load(string.Format("Unit/Enemy/{0}", unitName)) as GameObject;
        Enemy addUnit = Instantiate(prefab, _createPos[idx], Quaternion.identity, _unitRoot).GetComponent<Enemy>();
        addUnit.Init(unitName, level);
        AddCardCollectionData(addUnit);
        addUnit.Enter();

        _unitList.Add(addUnit);
    }

    public override void RemoveUnit(string unitName)
    {
        Unit removeUnit = _unitList.Find(x => x.name == unitName);

        _unitList.Remove(removeUnit);
    }

    public void AddStack(int stack)
    {
        foreach (Enemy unit in _unitList)
            unit.AddStack(stack);
    }

    public override List<Unit> FindFullStackUnit()
    {
        List<Unit> fullStackUnitList = new List<Unit>();

        foreach (Enemy enemy in _unitList)
        {
            if (enemy.IsDie())
                continue;

            if (enemy.IsFullStack())
                fullStackUnitList.Add(enemy);
        }

        return fullStackUnitList;
    }

    public override List<Unit> FindLiveUnit()
    {
        List<Unit> liveUnitList = new List<Unit>();

        foreach (Enemy enemy in _unitList)
        {
            if (enemy.IsDie() == false)
                liveUnitList.Add(enemy);
        }

        return liveUnitList;
    }

    public override void AddCardCollectionData(Enemy unit)
    {
        var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);

        foreach (var kind in Enum.GetValues(typeof(eCardKindStat)))
        {
            var myCardList = cdm.MyCard.GetSelectMyCardCollectionDataList((int)eCardOption.DeBuff, (int)eCardType.Statistics, (int)kind);
            int value = cdm.GetCardSumValue(myCardList);
            if (value == 0)
                continue;

            switch (kind)
            {
                case eCardKindStat.Atk:
                    unit.minAtk = Mathf.RoundToInt(unit.minAtk * (1f - ((float)value / 100f)));
                    unit.maxAtk = Mathf.RoundToInt(unit.maxAtk * (1f - ((float)value / 100f)));
                    break;
                case eCardKindStat.Hp:
                    unit.maxHp = Mathf.RoundToInt(unit.maxHp * (1f - ((float)value / 100f)));
                    break;
                case eCardKindStat.Def:
                    unit.defence = Mathf.RoundToInt(unit.defence * (1f - ((float)value / 100f)));
                    break;
                case eCardKindStat.Cri:
                    unit.critical_Per = Mathf.RoundToInt(unit.critical_Per * (1f - ((float)value / 100f)));
                    break;
            }
        }
    }
}
