using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MyMissionDataModel : ADataModel
{
    // value == -1, 보상 받음
    Dictionary<int, int> myMissionList = new Dictionary<int, int>();

    readonly string key = "MyMission";

    readonly string lastAccessDateKey = "LastAccessDate";

    public override void Load()
    {
        if (PlayerPrefs.HasKey(key))
        {
            var datas = PlayerPrefs.GetString(key).Split(',');
            for (int i = 0; i < datas.Length; i++)
            {
                int key = Util.ParseToInt(datas[i].Split('_')[0]);
                int cnt = Util.ParseToInt(datas[i].Split('_')[1]);

                myMissionList.Add(key, cnt);
            }
        }
    }

    public void Save()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var myCardInfo in myMissionList)
        {
            string str = string.Format("{0}_{1},", myCardInfo.Key, myCardInfo.Value);
            sb.Append(str);
        }
        sb.Remove(sb.Length - 1, 1);

        PlayerPrefs.SetString(key, sb.ToString());
    }

    public int GetMissionCount(int key)
    {
        if (myMissionList.ContainsKey(key))
            return myMissionList[key];
        else
            return 0;
    }

    public void SetAddMission(int key, int cnt)
    {
        if (myMissionList.ContainsKey(key))
        {
            if (myMissionList[key] != -1)
                myMissionList[key] += cnt;
        }
        else
            myMissionList.Add(key, cnt);

        Save();
    }

    public void SetAddReward(int key)
    {
        if (myMissionList.ContainsKey(key))
            myMissionList[key] = -1;

        Save();
    }

    public Dictionary<int, int> GetAllData()
    {
        return myMissionList;
    }

    public Dictionary<int, int> GetData(eMissionType type)
    {
        var missions = myMissionList.Where(data => data.Key / 100 == (int)type);
        return missions.ToDictionary(x => x.Key, x => x.Value);
    }

    public Dictionary<int, int> GetData(eMissionKind kind)
    {
        var missions = myMissionList.Where(data => data.Key % 100 == (int)kind);
        return missions.ToDictionary(x => x.Key, x => x.Value);
    }

    public bool IsReSetMissionData()
    {
        bool isReset = false;

        // MEMO : 별도의 서버 없이 미션 초기화
        if (PlayerPrefs.HasKey(lastAccessDateKey))
        {
            DateTime lastAccessDate = Convert.ToDateTime(PlayerPrefs.GetString(lastAccessDateKey));
            DateTime todayDate = DateTime.UtcNow;
            if (todayDate.DayOfYear > lastAccessDate.DayOfYear)
            {
                isReset = true;

                // 일간 초기화
                Debug.Log("Daliy Mission Reset");
                GetData(eMissionType.Daily).Keys.ToList().ForEach(key =>
                {
                    myMissionList[key] = 0;
                });

                // 주간 초기화
                int subDay = todayDate.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)todayDate.DayOfWeek - 1;
                DateTime todayWeekDate = todayDate.AddDays(-subDay);
                subDay = lastAccessDate.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)lastAccessDate.DayOfWeek - 1;
                DateTime lastWeekDate = lastAccessDate.AddDays(-subDay);
                if (todayWeekDate.DayOfYear > lastWeekDate.DayOfYear)
                {
                    Debug.Log("Weekly Mission Reset");
                    GetData(eMissionType.Weekly).Keys.ToList().ForEach(key =>
                    {
                        myMissionList[key] = 0;
                    });
                }

                // 월간 초기화
                if (todayDate.Year > lastAccessDate.Year || (todayDate.Year == lastAccessDate.Year && todayDate.Month > lastAccessDate.Month))
                {
                    Debug.Log("Monthly Mission Reset");
                    GetData(eMissionType.Monthly).Keys.ToList().ForEach(key =>
                    {
                        myMissionList[key] = 0;
                    });
                }

                PlayerPrefs.SetString(lastAccessDateKey, DateTime.UtcNow.Date.ToString());
            }
        }
        else
        {
            isReset = true;

            PlayerPrefs.SetString(lastAccessDateKey, DateTime.UtcNow.Date.ToString());
        }

        return isReset;
    }
}
