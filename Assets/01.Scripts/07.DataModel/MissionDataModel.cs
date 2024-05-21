using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionData
{
    public eMissionType Type;
    public eMissionKind Kind;
    public int ClearCnt;
    public int RewardType;
    public int RewardCnt;
}

public class MissionDataModel : ADataModel
{
    MyMissionDataModel myMissionData = new MyMissionDataModel();
    public MyMissionDataModel MyMission => myMissionData;

    Dictionary<int, MissionData> missionDataList = new Dictionary<int, MissionData>();

    public override void Load()
    {
        var datas = Util.CSVReadFromResourcesFolder("DataModel/MissionData");
        for (int i = 0; i < datas.Count; i++)
        {
            int key = (int)datas[i]["Key"];

            eMissionType type = (eMissionType)datas[i]["Type"];
            eMissionKind kind = (eMissionKind)datas[i]["Kind"];

            int clearCnt = (int)datas[i]["Count"];
            int rewardType = (int)datas[i]["RewardType"];
            int rewardCnt = (int)datas[i]["RewardCount"];

            MissionData mission = new MissionData();
            mission.Type = type;
            mission.Kind = kind;
            mission.ClearCnt = clearCnt;
            mission.RewardType = rewardType;
            mission.RewardCnt = rewardCnt;

            missionDataList.Add(key, mission);
        }

        myMissionData.Load();
    }

    public MissionData GetData(int key)
    {
        return missionDataList[key];
    }

    public Dictionary<int, MissionData> GetAllData()
    {
        return missionDataList;
    }

    public HashSet<int> GetKeys(eMissionKind type)
    {
        var keys = missionDataList.Where(data => data.Key % 100 == (int)type).Select(data => data.Key);
        return keys.ToHashSet();
    }

    public Dictionary<int, MissionData> GetDatas(eMissionType type)
    {
        return missionDataList.Where(data => data.Value.Type == type).ToDictionary(data => data.Key, data => data.Value);
    }
}
