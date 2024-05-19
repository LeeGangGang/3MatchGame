using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalPanel : MonoBehaviour
{
    [SerializeField] Text lifeCntTxt;
    [SerializeField] Text lifeTimeTxt;
    [SerializeField] Button lifeAddBtn;

    [SerializeField] Text goldTxt;
    [SerializeField] Text diaTxt;

    [SerializeField] Button settingBtn;
    [SerializeField] FadePanel fadePanel;
    [SerializeField] LoadingPanel loadPanel;
    [SerializeField] AddItemDirectingPanel additemPanel;

    float playTimer;

    public void Init()
    {
        playTimer = 0f;

        settingBtn.onClick.AddListener(() =>
        {
            UIManager.Inst.Popup.OpenSettingPopup();
        });

        lifeAddBtn.onClick.AddListener(() =>
        {
            UIManager.Inst.Popup.OpenLifeBuyPopup();
        });

        var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
        SetGoldText(mwdm.Gold);
        SetDiaText(mwdm.Dia);

        mwdm.AddGoldEvent(SetGoldText);
        mwdm.AddDiaEvent(SetDiaText);
    }

    public void SetActiveLifeAddBtn(bool isActive)
    {
        lifeAddBtn.gameObject.SetActive(isActive);
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
        if (mwdm.LifeCnt < 5)
        {
            TimeSpan sp = DateTime.UtcNow - mwdm.LifeDate;
            TimeSpan fiveMin = new TimeSpan(0, 5, 0);
            int cnt = (int)(sp.TotalMinutes / 5);
            if (cnt > 0)
            {
                if (mwdm.LifeCnt + cnt > 5)
                    cnt = 5 - mwdm.LifeCnt;

                mwdm.LifeCnt += cnt;
                mwdm.LifeDate = mwdm.LifeDate.AddMinutes(fiveMin.Minutes * cnt);
                sp = DateTime.UtcNow - mwdm.LifeDate;
            }

            lifeTimeTxt.text = (fiveMin - sp).ToString(@"mm\:ss");
        }
        else
        {
            lifeTimeTxt.text = "MAX";
        }

        lifeCntTxt.text = mwdm.LifeCnt.ToString();

        playTimer += Time.deltaTime;
        if (playTimer >= 60f)
        {
            playTimer -= 60f;
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
