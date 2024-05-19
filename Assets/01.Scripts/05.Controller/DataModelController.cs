using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eDataModel
{
    UnitDataModel,
    EnemyDataModel,
    SkillDataModel,
    StageInfoDataModel,
    CardDataModel,
    MissionDataModel,

    StoreDataModel,
    StoreChartDataModel,
    LocalizationDataModel,

    MyUnitCollectionDataModel,
    MyCardCollectionDataModel,
    MyWealthDataModel,
    MyMissionDataModel,
}

public class DataModelController : MonoBehaviour
{
    public static DataModelController Inst;

    Dictionary<eDataModel, ADataModel> _dataModels = new Dictionary<eDataModel, ADataModel>();

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        Inst = this;

        LoadDataModel();
    }

    public void LoadDataModel()
    {
        foreach (eDataModel dm in Enum.GetValues(typeof(eDataModel)))
        {
            Type type = Type.GetType(dm.ToString());
            ADataModel obj = Activator.CreateInstance(type) as ADataModel;
            obj.Load();

            _dataModels.Add(dm, obj);
        }
    }

    public ADataModel GetDataModel(eDataModel type)
    {
        return _dataModels[type];
    }
}
