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

    DateTime lifeDate;
    public DateTime LifeDate
    {
        get
        {
            return lifeDate;
        }
        set
        {
            lifeDate = value;
            PlayerPrefs.SetString(lifeDateKey, lifeDate.ToString());
        }
    }

    int lifeCnt;
    public int LifeCnt
    {
        get
        {
            return lifeCnt;
        }
        set
        {
            if (value > 5)
                value = 5;

            lifeCnt = value;
            PlayerPrefs.SetInt(lifeCntKey, lifeCnt);
        }
    }

    string goldKey = "Gold";
    string diaKey = "Dia";
    string lifeCntKey = "LifeCnt";
    string lifeDateKey = "LifeDate";

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

        if (!PlayerPrefs.HasKey(lifeCntKey))
            lifeCnt = 5;
        else
            lifeCnt = PlayerPrefs.GetInt(lifeCntKey, 5);

        if (!PlayerPrefs.HasKey(lifeDateKey))
            lifeDate = DateTime.UtcNow;
        else
            lifeDate = DateTime.Parse(PlayerPrefs.GetString(lifeDateKey, DateTime.UtcNow.ToString()));
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
