using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreChartDataModel : ADataModel
{
    Dictionary<eProductID, Dictionary<int, float>> cardPackChartList = new Dictionary<eProductID, Dictionary<int, float>>();
    Dictionary<eProductID, Dictionary<eUnit, float>> unitPackChartList = new Dictionary<eProductID, Dictionary<eUnit, float>>();

    public override void Load()
    {
        // CardChart
        var datas = Util.CSVReadFromResourcesFolder("DataModel/Chart/NormalCardPackChart");
        Dictionary<int, float> cardChart = new Dictionary<int, float>();
        for (int i = 0; i < datas.Count; i++)
        {
            int code = (int)datas[i]["Code"];
            float per = (float)datas[i]["Percent"];
            cardChart.Add(code, per);
        }
        cardPackChartList.Add(eProductID.NormalCardPack, cardChart);

        cardChart = new Dictionary<int, float>();
        datas = Util.CSVReadFromResourcesFolder("DataModel/Chart/MagicCardPackChart");
        for (int i = 0; i < datas.Count; i++)
        {
            int code = (int)datas[i]["Code"];
            float per = (float)datas[i]["Percent"];
            cardChart.Add(code, per);
        }
        cardPackChartList.Add(eProductID.MagicCardPack, cardChart);

        cardChart = new Dictionary<int, float>();
        datas = Util.CSVReadFromResourcesFolder("DataModel/Chart/RareCardPackChart");
        for (int i = 0; i < datas.Count; i++)
        {
            int code = (int)datas[i]["Code"];
            float per = (float)datas[i]["Percent"];
            cardChart.Add(code, per);
        }
        cardPackChartList.Add(eProductID.RareCardPack, cardChart);

        cardChart = new Dictionary<int, float>();
        datas = Util.CSVReadFromResourcesFolder("DataModel/Chart/UniqCardPackChart");
        for (int i = 0; i < datas.Count; i++)
        {
            int code = (int)datas[i]["Code"];
            float per = (float)datas[i]["Percent"];
            cardChart.Add(code, per);
        }
        cardPackChartList.Add(eProductID.UniqCardPack, cardChart);

        // UnitChart
        Dictionary<eUnit, float> unitChart = new Dictionary<eUnit, float>();
        datas = Util.CSVReadFromResourcesFolder("DataModel/Chart/NormalUnitPackChart");
        for (int i = 0; i < datas.Count; i++)
        {
            eUnit code = (eUnit)Enum.Parse(typeof(eUnit), (string)datas[i]["Code"]);
            float per = (float)datas[i]["Percent"];
            unitChart.Add(code, per);
        }
        unitPackChartList.Add(eProductID.NormalUnitPack, unitChart);

        unitChart = new Dictionary<eUnit, float>();
        datas = Util.CSVReadFromResourcesFolder("DataModel/Chart/MagicUnitPackChart");
        for (int i = 0; i < datas.Count; i++)
        {
            eUnit code = (eUnit)Enum.Parse(typeof(eUnit), (string)datas[i]["Code"]);
            float per = (float)datas[i]["Percent"];
            unitChart.Add(code, per);
        }
        unitPackChartList.Add(eProductID.MagicUnitPack, unitChart);

        unitChart = new Dictionary<eUnit, float>();
        datas = Util.CSVReadFromResourcesFolder("DataModel/Chart/RareUnitPackChart");
        for (int i = 0; i < datas.Count; i++)
        {
            eUnit code = (eUnit)Enum.Parse(typeof(eUnit), (string)datas[i]["Code"]);
            float per = (float)datas[i]["Percent"];
            unitChart.Add(code, per);
        }
        unitPackChartList.Add(eProductID.RareUnitPack, unitChart);

        unitChart = new Dictionary<eUnit, float>();
        datas = Util.CSVReadFromResourcesFolder("DataModel/Chart/UniqUnitPackChart");
        for (int i = 0; i < datas.Count; i++)
        {
            eUnit code = (eUnit)Enum.Parse(typeof(eUnit), (string)datas[i]["Code"]);
            float per = (float)datas[i]["Percent"];
            unitChart.Add(code, per);
        }
        unitPackChartList.Add(eProductID.UniqUnitPack, unitChart);
    }

    public int GetCardCode(eProductID id)
    {
        int code = -1;

        float total = 0f;
        foreach (var card in cardPackChartList[id])
            total += card.Value; 
        
        float randomPoint = total * UnityEngine.Random.value;
        foreach (var card in cardPackChartList[id])
        {
            if (randomPoint < card.Value)
            {
                code = card.Key;
                break;
            }
            else
            {
                randomPoint -= card.Value;
            }
        }
        return code;
    }

    public eUnit GetUnitCode(eProductID id)
    {
        eUnit code = eUnit.Wizard_0;

        float total = 0f;
        foreach (var card in unitPackChartList[id])
            total += card.Value;

        float randomPoint = total * UnityEngine.Random.value;
        foreach (var unit in unitPackChartList[id])
        {
            if (randomPoint < unit.Value)
            {
                code = unit.Key;
                break;
            }
            else
            {
                randomPoint -= unit.Value;
            }
        }
        return code;
    }
}
