using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eUnit
{
    Wizard_0,
    Archer_0,
    Warrior_0,
    Knight_0,

    KDJ,
}

public class UnitData
{
    public string Name;
    public int Grade;
    public int[] Hp = new int[4];
    public int[] Critical_Per = new int[4];
    public int[] Defence = new int[4];
    public int[] Skill = new int[2];
}

public class UnitDataModel : ADataModel
{
    MyUnitCollectionDataModel myUnitData = new MyUnitCollectionDataModel();
    public MyUnitCollectionDataModel MyUnit => myUnitData;

    Dictionary<eUnit, UnitData> unitDataList = new Dictionary<eUnit, UnitData>();

    public override void Load()
    {
        var datas = Util.CSVReadFromResourcesFolder("DataModel/UnitData");
        for (int i = 0; i < datas.Count; i++)
        {
            eUnit type = (eUnit)Enum.Parse(typeof(eUnit), (string)datas[i]["Name"]);

            UnitData unit = new UnitData();
            unit.Name = (string)datas[i]["Name"];
            unit.Grade = (int)datas[i]["Grade"];
            for (int j = 0; j < 4; j++)
            {
                unit.Hp[j] = Util.ParseToInt(((string)datas[i]["Hp"]).Split('_')[j]);
                unit.Critical_Per[j] = Util.ParseToInt(((string)datas[i]["Critical_Per"]).Split('_')[j]);
                unit.Defence[j] = Util.ParseToInt(((string)datas[i]["Defence"]).Split('_')[j]);
            }

            unit.Skill[0] = (int)datas[i]["Skill_1"];
            unit.Skill[1] = (int)datas[i]["Skill_2"];

            unitDataList.Add(type, unit);
        }

        myUnitData.Load();
    }

    public UnitData GetUnitData(eUnit type)
    {
        return unitDataList[type];
    }
}
