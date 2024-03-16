using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum eCardOption
{
    Buff = 1,
    DeBuff = 2,
}

public enum eCardType
{
    Gold,       // ÀçÈ­ È¹µæ
    Statistics, // À¯´Ö ½ºÅÈ
    SkillStack, // ½ºÅ³ ½ºÅÃ
}

// eCardType == Statistics
public enum eCardKindStat
{
    Atk,    // °ø°Ý·Â
    Hp,     // Ã¼·Â
    Def,    // ¹æ¾î·Â 
    Cri,    // Å©¸®Æ¼ÄÃ È®·ü
}

// eCardType == SkillStack
public enum eCardKindStack
{
    Red,    // »¡°­
    Yellow, // ³ë¶û
    Green,  // ÃÊ·Ï
    Blue,   // ÆÄ¶û
    White,  // Èò»ö
    Block,  // °ËÁ¤
}

public class MyCardCollectionDataModel : ADataModel
{
    int[] maxExp = new int[3] { 3, 10, 20 };

    Dictionary<int, int[]> myCardCollectionList = new Dictionary<int, int[]>();

    string key = "MyCardCollection";

    public override void Load()
    {
        if (PlayerPrefs.HasKey(key))
        {
            var datas = PlayerPrefs.GetString(key).Split(',');
            for (int i = 0; i < datas.Length; i++)
            {
                int code = Util.ParseToInt(datas[i].Split('_')[0]);
                int level = Util.ParseToInt(datas[i].Split('_')[1]);
                int exp = Util.ParseToInt(datas[i].Split('_')[2]);

                myCardCollectionList.Add(code, new int[2] { level, exp });
            }
        }
    }

    public void SetAddCard(int code, int cnt)
    {
        if (myCardCollectionList.ContainsKey(code))
            myCardCollectionList[code][1] += cnt;
        else
            myCardCollectionList.Add(code, new int[2] { 0, cnt - 1 });

        Save();
    }

    public void SetUpgradeCard(int code, Action<bool> onComplete)
    {
        bool isSuccess = false;
        int currLevel = myCardCollectionList[code][0];
        if (maxExp.Length > currLevel)
        {
            if (maxExp[currLevel] <= myCardCollectionList[code][1])
            {
                myCardCollectionList[code][0]++;
                myCardCollectionList[code][1] -= maxExp[currLevel];

                isSuccess = true;

                Save();
            }
        }

        onComplete?.Invoke(isSuccess);
    }

    public void Save()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var myCardInfo in myCardCollectionList)
        {
            string str = string.Format("{0}_{1}_{2},", myCardInfo.Key, myCardInfo.Value[0], myCardInfo.Value[1]);

            sb.Append(str);
        }
        sb.Remove(sb.Length - 1, 1);

        PlayerPrefs.SetString(key, sb.ToString());
    }

    public Dictionary<int, int[]> GetMyCardCollectionDataList()
    {
        return myCardCollectionList;
    }

    public int[] GetMyCardCollectionData(int code)
    {
        if (myCardCollectionList.ContainsKey(code))
            return myCardCollectionList[code];
        else
            return null;
    }

    public Dictionary<int, int> GetSelectMyCardCollectionDataList(int opt, int type, int kind)
    {
        Dictionary<int, int> selectList = new Dictionary<int, int>();

        foreach (var myCard in myCardCollectionList)
        {
            int mOpt = myCard.Key / 1000;
            int mType = (myCard.Key % 1000) / 100;
            int mKind = (myCard.Key % 100) / 10;

            if (mOpt == opt && mType == type && mKind == kind)
                selectList.Add(myCard.Key, myCard.Value[0]);
        }

        return selectList;
    }
}
