using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MyUnitCollectionDataModel : ADataModel
{
    int[] maxExp = new int[3] { 5, 10, 20 };

    // index 0 : level, 1 : exp, 2 : isSelect
    Dictionary<eUnit, int[]> myUnitCollectionList = new Dictionary<eUnit, int[]>();
    Dictionary<eUnit, int> mySelectUnit = new Dictionary<eUnit, int>();

    string key = "MyUnitCollection";

    public override void Load()
    {
        if (!PlayerPrefs.HasKey(key))
        {
            myUnitCollectionList.Add(eUnit.Wizard_0, new int[2] { 0, 300 });
            myUnitCollectionList.Add(eUnit.Archer_0, new int[2] { 1, 4 });
            myUnitCollectionList.Add(eUnit.Warrior_0, new int[2] { 2, 5 });

            mySelectUnit.Add(eUnit.Wizard_0, 1);
            mySelectUnit.Add(eUnit.Archer_0, 2);
            mySelectUnit.Add(eUnit.Warrior_0, 3);

            Save();
        }
        else
        {
            var datas = PlayerPrefs.GetString(key).Split(',');
            for (int i = 0; i < datas.Length; i++)
            {
                eUnit type = (eUnit)Util.ParseToInt(datas[i].Split('_')[0]);
                int[] value = new int[2]
                {
                    Util.ParseToInt(datas[i].Split('_')[1]),
                    Util.ParseToInt(datas[i].Split('_')[2]),
                };
                myUnitCollectionList.Add(type, value);

                if (Util.ParseToInt(datas[i].Split('_')[3]) != 0)
                {
                    int idx = Util.ParseToInt(datas[i].Split('_')[3]);
                    mySelectUnit.Add(type, idx);
                }
            }
        }
    }

    public void SetUpgradeUnit(eUnit type, Action<bool> onComplete)
    {
        bool isSuccess = false;
        int currLevel = myUnitCollectionList[type][0];
        if (maxExp[currLevel] <= myUnitCollectionList[type][1])
        {
            myUnitCollectionList[type][0]++;
            myUnitCollectionList[type][1] -= maxExp[currLevel];

            isSuccess = true;

            Save();
        }

        onComplete?.Invoke(isSuccess);
    }

    public void Save()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var myUnitInfo in myUnitCollectionList)
        {
            int isSelect = 0;
            if (mySelectUnit.Keys.Contains(myUnitInfo.Key))
                isSelect = mySelectUnit[myUnitInfo.Key];
            
            string str = string.Format("{0}_{1}_{2}_{3},", (int)myUnitInfo.Key, myUnitInfo.Value[0], myUnitInfo.Value[1], isSelect);

            sb.Append(str);
        }
        sb.Remove(sb.Length - 1, 1);

        PlayerPrefs.SetString(key, sb.ToString());
    }

    public void SetAddUnit(eUnit type, int cnt)
    {
        if (myUnitCollectionList.ContainsKey(type))
            myUnitCollectionList[type][1] += cnt;
        else
            myUnitCollectionList.Add(type, new int[2] { 0, cnt - 1 });

        Save();
    }

    public void SetChangeSelectUnit(eUnit before, eUnit after)
    {
        int idx = mySelectUnit[before];
        mySelectUnit.Remove(before);
        mySelectUnit.Add(after, idx);

        Save();
    }

    public Dictionary<eUnit, int[]> GetMyUnitCollectionDataList()
    {
        return myUnitCollectionList;
    }

    public int[] GetMyUnitData(eUnit type)
    {
        return myUnitCollectionList[type];
    }

    public Dictionary<eUnit, int[]> GetMyUnSelectUnitDataList()
    {
        var unSelectUnitList = from unSelect in myUnitCollectionList
                               let places = from p in mySelectUnit.Keys select p
                               where !places.Contains(unSelect.Key)
                               select unSelect;

        return unSelectUnitList.ToDictionary(x => x.Key, x => x.Value);
    }

    public Dictionary<eUnit, int> GetMySelectUnitDataList()
    {
        return mySelectUnit;
    }
}
