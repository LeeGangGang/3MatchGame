using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionItem : MonoBehaviour
{
    public bool isGet = false;
    public int key;

    int maxCnt;
    int curCnt;

    int rewardKey;
    int rewardCnt;

    [SerializeField] Text infoTxt;
    [SerializeField] Text cntTxt;
    [SerializeField] Scrollbar cntSb;

    [SerializeField] Image rewardImg;
    [SerializeField] Text rewardCntTxt;

    [SerializeField] Button getRewardBtn;

    public void Init(int missionKey, MissionData data)
    {
        key = missionKey;

        maxCnt = data.ClearCnt;
        rewardKey = data.RewardType;
        rewardCnt = data.RewardCnt;

        string infoStr = missionKey.ToString();
        SetMissionText(infoStr);
        SetRewardInfo(rewardKey, rewardCnt);

        UpdateUI();

        getRewardBtn.onClick.AddListener(OnClickGetRewardBtn);
    }

    public void UpdateUI()
    {
        var mdm = (MissionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MissionDataModel);
        curCnt = mdm.MyMission.GetMissionCount(key);

        SetCountInfo(maxCnt, curCnt);
        SetRewardBtnInteractable(maxCnt, curCnt);
    }

    void SetMissionText(string info)
    {
        infoTxt.text = info;
    }

    void SetRewardInfo(int rewardKey, int rewardCnt)
    {
        Sprite spr = AtlasManager.Inst.GetSprite(eAtlasType.UI, string.Format("reward_{0}", rewardKey));
        rewardImg.sprite = spr;

        rewardCntTxt.text = string.Format("{0:N0}", rewardCnt);
    }

    void SetCountInfo(int maxCnt, int curCnt)
    {
        int cnt = curCnt == -1 || curCnt > maxCnt ? maxCnt : curCnt;
        cntTxt.text = string.Format("{0:N0} / {1:N0}", cnt, maxCnt);
        cntSb.size = (float)cnt / (float)maxCnt;
    }

    void SetRewardBtnInteractable(int maxCnt, int curCnt)
    {
        if (maxCnt <= curCnt)
            getRewardBtn.interactable = true;
        else
            getRewardBtn.interactable = false;
    }

    public void OnClickGetRewardBtn()
    {
        var mdm = (MissionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MissionDataModel);
        mdm.MyMission.SetAddReward(key);

        Dictionary<eProductType, Dictionary<int, int>> addItemsList = new Dictionary<eProductType, Dictionary<int, int>>();
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

        // 리워드 받으면 버튼 비활성화
        isGet = true;
        getRewardBtn.interactable = false;

        UIManager.Inst.SetAddItemPanel(addItemsList);
    }
}
