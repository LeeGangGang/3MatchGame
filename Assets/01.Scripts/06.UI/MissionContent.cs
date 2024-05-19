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
    Login,      // 00 : �α���
    Stemina,    // 01 : ���׹̳� ���
    AD,         // 02 : AD ��û
    BuyItem,    // 03 : ���� ����
    PlayTime,   // 04 : �÷��� Ÿ��
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
