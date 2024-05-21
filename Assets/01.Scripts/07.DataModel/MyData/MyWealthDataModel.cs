using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWealthDataModel : ADataModel
{
    HashSet<Action<int>> goldEventList = new HashSet<Action<int>>();
    int gold = 0;
    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
            PlayerPrefs.SetInt(goldKey, gold);
            foreach (var evt in goldEventList)
                evt?.Invoke(gold);
        }
    }

    HashSet<Action<int>> diaEventList = new HashSet<Action<int>>();
    int dia = 0;
    public int Dia
    {
        get
        {
            return dia;
        }
        set
        {
            dia = value;
            PlayerPrefs.SetInt(diaKey, dia);
            foreach (var evt in diaEventList)
                evt?.Invoke(dia);
        }
    }

    DateTime steminaDate;
    public DateTime SteminaDate
    {
        get
        {
            return steminaDate;
        }
        set
        {
            steminaDate = value;
            PlayerPrefs.SetString(steminaDateKey, steminaDate.ToString());
        }
    }

    int steminaCnt;
    public int SteminaCnt
    {
        get
        {
            return steminaCnt;
        }
        set
        {
            if (value > 5)
                value = 5;

            steminaCnt = value;
            PlayerPrefs.SetInt(steminaCntKey, steminaCnt);
        }
    }

    string goldKey = "Gold";
    string diaKey = "Dia";
    string steminaCntKey = "SteminaCnt";
    string steminaDateKey = "SteminaDate";

    public override void Load()
    {
        if (!PlayerPrefs.HasKey(goldKey))
            gold = 0;
        else
            gold = PlayerPrefs.GetInt(goldKey, 0);

        if (!PlayerPrefs.HasKey(diaKey))
            dia = 0;
        else
            dia = PlayerPrefs.GetInt(diaKey, 0);

        if (!PlayerPrefs.HasKey(steminaCntKey))
            steminaCnt = 5;
        else
            steminaCnt = PlayerPrefs.GetInt(steminaCntKey, 5);

        if (!PlayerPrefs.HasKey(steminaDateKey))
            steminaDate = DateTime.UtcNow;
        else
            steminaDate = DateTime.Parse(PlayerPrefs.GetString(steminaDateKey, DateTime.UtcNow.ToString()));
    }

    public void AddGoldEvent(Action<int> evt)
    {
        goldEventList.Add(evt);
    }

    public void AddDiaEvent(Action<int> evt)
    {
        diaEventList.Add(evt);
    }
}
