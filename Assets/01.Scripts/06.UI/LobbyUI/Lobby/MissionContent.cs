using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eMissionType
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
}

public enum eMissionKind
{
    Login,      // 00 : 로그인
    Stemina,    // 01 : 스테미너 사용
    AD,         // 02 : AD 시청
    BuyItem,    // 03 : 도감 구매
    PlayTime,   // 04 : 플레이 타임
}

public class MissionContent : MonoBehaviour
{
    public eMissionType type;

    [SerializeField] MissionItem missionPrefab;

    List<MissionItem> missionList = new List<MissionItem>();

    public void Init()
    {
        var mdm = (MissionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MissionDataModel);
        foreach (var data in mdm.GetDatas(type))
        {
            MissionItem mi = Instantiate(missionPrefab, transform);
            mi.Init(data.Key, data.Value);
            mi.gameObject.SetActive(true);

            missionList.Add(mi);
        }
    }

    public void Enter()
    {
        UpdateUI();

        gameObject.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        foreach (var data in missionList)
            data.UpdateUI();
    }
}
