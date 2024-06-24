using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataModel : ADataModel
{
    Dictionary<int, SkillData> skillDataList = new Dictionary<int, SkillData>();

    public override void Load()
    {
        var datas = Util.CSVReadFromResourcesFolder("DataModel/SkillData");
        for (int i = 0; i < datas.Count; i++)
        {
            int key = (int)datas[i]["Key"];

            SkillData skill = new SkillData();
            skill.key = (int)datas[i]["Key"];
            skill.Type = (int)datas[i]["Type"];
            skill.Color = (eColor)datas[i]["Color"];
            skill.Stack = (int)datas[i]["Stack"];
            skill.Value = (int)datas[i]["Value"];
            skill.Name = (string)datas[i]["Name"];

            skillDataList.Add(key, skill);
        }
    }

    public SkillData GetData(int key)
    {
        return skillDataList[key];
    }
}
