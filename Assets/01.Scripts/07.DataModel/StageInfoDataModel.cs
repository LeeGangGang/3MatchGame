using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfoDataModel : ADataModel
{
    Dictionary<int, StageMapInfo> stageMapData = new Dictionary<int, StageMapInfo>();
    Dictionary<int, Dictionary<eEnemy, Dictionary<int, int>>> stageEnemyData = new Dictionary<int, Dictionary<eEnemy, Dictionary<int, int>>>();

    public override void Load()
    {
        foreach (var stageMap in Resources.LoadAll("Stages"))
        {
            TextAsset txtAsset = stageMap as TextAsset;
            string str = txtAsset.text;

            StageMapInfo mapInfo = JsonConvert.DeserializeObject<StageMapInfo>(str);
            stageMapData.Add(mapInfo.stageLevel, mapInfo);
        }

        var datas = Util.CSVReadFromResourcesFolder("DataModel/StageEnemyData");
        for (int i = 0; i < datas.Count; i++)
        {
            int level = (int)datas[i]["Level"];
            Dictionary<eEnemy, Dictionary<int, int>> enemyList = new Dictionary<eEnemy, Dictionary<int, int>>();

            string[] arrStr;
            if (datas[i]["EnemyInfo"].ToString().Contains('_'))
                arrStr = datas[i]["EnemyInfo"].ToString().Split('_');
            else
                arrStr = new string[1] { datas[i]["EnemyInfo"].ToString() };

            foreach (string str in arrStr)
            {
                eEnemy type = (eEnemy)(int.Parse(str.Substring(0, 2)));
                int lv = int.Parse(str.Substring(2, 2));
                int cnt = int.Parse(str.Substring(4, 1));

                if (enemyList.ContainsKey(type))
                    enemyList[type].Add(lv, cnt);
                else
                {
                    Dictionary<int, int> enemyInfo = new Dictionary<int, int>();
                    enemyInfo.Add(lv, cnt);

                    enemyList.Add(type, enemyInfo);
                }
            }

            stageEnemyData.Add(level, enemyList);
        }
    }

    public Dictionary<eEnemy, Dictionary<int, int>> GetStageEnemyInfo(int num)
    {
        return stageEnemyData[num];
    }

    public StageMapInfo GetStageMapInfo(int num)
    {
        return stageMapData[num];
    }

    public int GetStageTotalCount()
    {
        return stageMapData.Count;
    }
}
