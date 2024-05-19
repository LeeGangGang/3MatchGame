using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MyMissionDataModel : ADataModel
{
    // value == -1, 보상 받음
    Dictionary<int, int> myMissionList = new Dictionary<int, int>();
    
    string key = "MyMission";

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
}
