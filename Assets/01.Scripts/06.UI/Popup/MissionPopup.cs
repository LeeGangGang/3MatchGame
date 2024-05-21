using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MissionPopup : APopup
{
    [SerializeField] Button closeBtn;
    [SerializeField] ScrollRect svRect;
    Dictionary<eMissionType, MissionContent> missionContents;

    [SerializeField] Toggle daliyTg;
    [SerializeField] Toggle weeklyTg;
    [SerializeField] Toggle monthlyTg;

    [SerializeField] Button allGetBtn;

    public override void Init()
    {
        missionContents = FindObjectsOfType<MissionContent>(true).ToDictionary(x => x.type);
        foreach (var content in missionContents.Values)
            content.Init();

        closeBtn.onClick.AddListener(() =>
        {
            Exit();
        });

        daliyTg.onValueChanged.AddListener((isOn) =>
        {
            OnChangeTabToggle(isOn, eMissionType.Daily);
        });

        weeklyTg.onValueChanged.AddListener((isOn) =>
        {
            OnChangeTabToggle(isOn, eMissionType.Weekly);
        });

        monthlyTg.onValueChanged.AddListener((isOn) =>
        {
            OnChangeTabToggle(isOn, eMissionType.Monthly);
        });

        allGetBtn.onClick.AddListener(() =>
        {
            OnClickAllGetBtn();
        });
    }

    public void Enter()
    {
        base.Enter(() =>
        {
            UpdateUI();
            daliyTg.SetIsOnWithoutNotify(true);
            OnChangeTabToggle(true, eMissionType.Daily);
        });
    }

    public override void Exit()
    {
        base.Exit();
    }

    void UpdateUI()
    {

    }

    void OnChangeTabToggle(bool isOn, eMissionType type)
    {
        if (isOn)
        {
            missionContents[type].Enter();
            svRect.content = missionContents[type].GetComponent<RectTransform>();
            svRect.content.localPosition = new Vector3(0, 0, 0);
        }
        else
            missionContents[type].Exit();
    }

    void OnClickAllGetBtn()
    {
        var mdm = (MissionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MissionDataModel);
        
        Dictionary<int, int> rewardList = new Dictionary<int, int>();
        foreach (var missionData in mdm.GetAllData())
        {
            if (mdm.MyMission.GetAllData().ContainsKey(missionData.Key))
            {
                int curCnt = mdm.MyMission.GetMissionCount(missionData.Key);
                int maxCnt = missionData.Value.ClearCnt;
                if (curCnt > maxCnt)
                {
                    if (rewardList.ContainsKey(missionData.Value.RewardType))
                        rewardList[missionData.Value.RewardType] += missionData.Value.RewardCnt;
                    else
                        rewardList.Add(missionData.Value.RewardType, missionData.Value.RewardCnt);

                    mdm.MyMission.SetAddReward(missionData.Key);
                }
            }
        }

        if (rewardList.Count > 0)
        {
            Dictionary<eProductType, Dictionary<int, int>> addItemsList = new Dictionary<eProductType, Dictionary<int, int>>();

            foreach (var reward in rewardList)
                AddRewardData(reward.Key, reward.Value, addItemsList);

            UIManager.Inst.SetAddItemPanel(addItemsList);

            foreach (var content in missionContents.Values)
                content.UpdateUI();
        }
        else
        {
            Debug.Log("받을 리워드가 없음");
        }
    }

    void AddRewardData(int rewardKey, int rewardCnt, Dictionary<eProductType, Dictionary<int, int>> addItemsList)
    {
        if (rewardKey == 100 || rewardKey == 101)
        {
            var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
            if (rewardKey == 100)
            {
                mwdm.Gold += rewardCnt;
            }
            else if (rewardKey == 101)
            {
                mwdm.Dia += rewardCnt;
            }

            Dictionary<int, int> additems = new Dictionary<int, int>();
            additems.Add(0, rewardCnt);
            addItemsList.Add(eProductType.Gold, additems);
        }
        else if (rewardKey == 200 || rewardKey == 201)
        {
            var scdm = (StoreChartDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreChartDataModel);
            var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);

            eProductID id = eProductID.NormalCardPack;
            if (rewardKey == 200)
                id = eProductID.NormalCardPack;
            else if (rewardKey == 201)
                id = eProductID.MagicCardPack;

            for (int i = 0; i < rewardCnt; i++)
            {
                int code = scdm.GetCardCode(id);
                if (addItemsList.ContainsKey(eProductType.Card))
                {
                    if (addItemsList[eProductType.Card].ContainsKey(code))
                        addItemsList[eProductType.Card][code]++;
                    else
                        addItemsList[eProductType.Card].Add(code, 1);
                }
                else
                {
                    Dictionary<int, int> additems = new Dictionary<int, int>();
                    additems.Add(code, 1);
                    addItemsList.Add(eProductType.Card, additems);
                }
            }

            foreach (var card in addItemsList[eProductType.Card])
                cdm.MyCard.SetAddCard(card.Key, card.Value);
        }
        else if (rewardKey == 300 || rewardKey == 301)
        {
            var scdm = (StoreChartDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreChartDataModel);
            var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);

            eProductID id = eProductID.NormalUnitPack;
            if (rewardKey == 200)
                id = eProductID.NormalUnitPack;
            else if (rewardKey == 201)
                id = eProductID.MagicUnitPack;

            for (int i = 0; i < rewardCnt; i++)
            {
                eUnit code = scdm.GetUnitCode(id);
                if (addItemsList.ContainsKey(eProductType.Unit))
                {
                    if (addItemsList[eProductType.Unit].ContainsKey((int)code))
                        addItemsList[eProductType.Unit][(int)code]++;
                    else
                        addItemsList[eProductType.Unit].Add((int)code, 1);
                }
                else
                {
                    Dictionary<int, int> additems = new Dictionary<int, int>();
                    additems.Add((int)code, 1);
                    addItemsList.Add(eProductType.Unit, additems);
                }
            }

            foreach (var unit in addItemsList[eProductType.Unit])
                udm.MyUnit.SetAddUnit((eUnit)unit.Key, unit.Value);
        }
    }
}
