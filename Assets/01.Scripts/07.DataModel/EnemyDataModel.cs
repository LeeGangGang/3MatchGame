using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eEnemy
{
    FlyingEye   = 10,
    Goblin      = 11,
    Mushroom    = 12,
    Skeleton    = 13,
    Slime       = 14,
}

public class EnemyData
{
    public string Name;
    public int Hp;
    public int Critical_Per;
    public int Defence;
    public float Stack;
    public int[] AttackRange = new int[2];
    public int AttackMotion;

    public int UpHp;
    public int UpAttack;
    public int UpDefence;
}

public class EnemyDataModel : ADataModel
{
    Dictionary<eEnemy, EnemyData> enemyDataList = new Dictionary<eEnemy, EnemyData>();

    public override void Load()
    {
        var datas = Util.CSVReadFromResourcesFolder("DataModel/EnemyData");
        for (int i = 0; i < datas.Count; i++)
        {
            eEnemy type = (eEnemy)Enum.Parse(typeof(eEnemy), (string)datas[i]["Name"]);

            EnemyData unit = new EnemyData();
            unit.Name = (string)datas[i]["Name"];
            unit.Hp = (int)datas[i]["Hp"];
            unit.Critical_Per = (int)datas[i]["Critical_Per"];
            unit.Defence = (int)datas[i]["Defence"];
            unit.Stack = (float)datas[i]["Stack"];
            unit.AttackRange[0] = int.Parse(((string)datas[i]["AttackRange"]).Split('_')[0]);
            unit.AttackRange[1] = int.Parse(((string)datas[i]["AttackRange"]).Split('_')[1]);
            unit.AttackMotion = (int)datas[i]["AttackMotion"];

            unit.UpHp = (int)datas[i]["UpHp"];
            unit.UpAttack = (int)datas[i]["UpAttack"];
            unit.UpDefence = (int)datas[i]["UpDefence"];

            enemyDataList.Add(type, unit);
        }
    }

    public EnemyData GetData(eEnemy type, int level)
    {
        EnemyData data = new EnemyData();

        data.Name = enemyDataList[type].Name;
        data.Hp = enemyDataList[type].Hp + (enemyDataList[type].UpHp * (level - 1));
        data.Critical_Per = enemyDataList[type].Critical_Per;
        data.Defence = enemyDataList[type].Defence + (enemyDataList[type].UpDefence * (level - 1));
        data.Stack = enemyDataList[type].Stack;

        data.AttackRange[0] += enemyDataList[type].UpAttack * (level - 1);
        data.AttackRange[1] += enemyDataList[type].UpAttack * (level - 1);

        data.AttackMotion = enemyDataList[type].AttackMotion;

        return data;
    }

    public EnemyData GetData(eEnemy type)
    {
        return enemyDataList[type];
    }
}
