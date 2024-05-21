using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GlobalPanel : MonoBehaviour
{
    [SerializeField] Text steminaCntTxt;
    [SerializeField] Text steminaTimeTxt;
    [SerializeField] Button steminaAddBtn;

    [SerializeField] Text goldTxt;
    [SerializeField] Text diaTxt;

    [SerializeField] Button settingBtn;
    [SerializeField] FadePanel fadePanel;
    [SerializeField] LoadingPanel loadPanel;
    [SerializeField] AddItemDirectingPanel additemPanel;

    float playTimer;
    DateTime loginDate;

    public void Init()
    {
        playTimer = 0f;
        loginDate = DateTime.UtcNow;

        settingBtn.onClick.AddListener(() =>
        {
            UIManager.Inst.Popup.OpenSettingPopup();
        });

        steminaAddBtn.onClick.AddListener(() =>
        {
            UIManager.Inst.Popup.OpenSteminaBuyPopup();
        });

        var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
        SetGoldText(mwdm.Gold);
        SetDiaText(mwdm.Dia);

        mwdm.AddGoldEvent(SetGoldText);
        mwdm.AddDiaEvent(SetDiaText);
    }

    public void SetActiveLifeAddBtn(bool isActive)
    {
        steminaAddBtn.gameObject.SetActive(isActive);
    }

    public void ShowFade(Action onComplete)
    {
        fadePanel.Enter(onComplete);
    }

    public void SetActiveLoading(bool isActive)
    {
        loadPanel.gameObject.SetActive(isActive);
    }

    public void SetAddItemPanel(Dictionary<eProductType, Dictionary<int, int>> addItems)
    {
        additemPanel.Enter(addItems);
    }

    // Update is called once per frame
    void Update()
    {
        LobbyTimer();
    }

    void LobbyTimer()
    {
        var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
        if (mwdm.SteminaCnt < 5)
        {
            TimeSpan sp = DateTime.UtcNow - mwdm.SteminaDate;
            TimeSpan fiveMin = new TimeSpan(0, 5, 0);
            int cnt = (int)(sp.TotalMinutes / 5);
            if (cnt > 0)
            {
                if (mwdm.SteminaCnt + cnt > 5)
                    cnt = 5 - mwdm.SteminaCnt;

                mwdm.SteminaCnt += cnt;
                mwdm.SteminaDate = mwdm.SteminaDate.AddMinutes(fiveMin.Minutes * cnt);
                sp = DateTime.UtcNow - mwdm.SteminaDate;
            }

            steminaTimeTxt.text = (fiveMin - sp).ToString(@"mm\:ss");
        }
        else
        {
            steminaTimeTxt.text = "MAX";
        }

        steminaCntTxt.text = mwdm.SteminaCnt.ToString();

        playTimer += Time.deltaTime;
        if (playTimer >= 60f)
        {
            playTimer -= 60f;

            var mdm = (MissionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MissionDataModel);
            foreach (int key in mdm.GetKeys(eMissionKind.PlayTime))
                mdm.MyMission.SetAddMission(key, 1);
        }

        if (DateTime.UtcNow.DayOfYear > loginDate.DayOfYear)
        {
            var mdm = (MissionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MissionDataModel);
            if (mdm.MyMission.IsReSetMissionData())
            {
                mdm.GetKeys(eMissionKind.Login).ToList().ForEach(key =>
                {
                    mdm.MyMission.SetAddMission(key, 1);
                });
            }
            loginDate = DateTime.UtcNow;
        }
    }

    void SetGoldText(int value)
    {
        goldTxt.text = string.Format("{0:N0}", value);
    }

    void SetDiaText(int value)
    {
        diaTxt.text = string.Format("{0:N0}", value);
    }
}
