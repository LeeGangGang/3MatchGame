using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    Stack<APopup> _openPopupStk = new Stack<APopup>();
    public Stack<APopup> OpenPopupStk => _openPopupStk;

    [Header("Popup")]
    [SerializeField] ResultPopup resultPopup;
    [SerializeField] SettingPopup settingPopup;
    [SerializeField] StageInfoPopup stageInfoPopup;

    [SerializeField] UnitInfoPopup unitInfoPopup;
    [SerializeField] UnitChangePopup unitChangePopup; 

    [SerializeField] CardInfoPopup cardInfoPopup;

    [SerializeField] ProductInfoPopup productInfoPopup;

    [SerializeField] StaminaBuyPopup staminaBuyPopup;

    [SerializeField] MissionPopup missionPopup;

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_openPopupStk.Count == 0)
                    return;

                APopup popup = _openPopupStk.Peek();
                popup.Exit();
            }
        }
    }

    public void Init()
    {
        resultPopup.Init();
        settingPopup.Init();
        stageInfoPopup.Init();

        unitInfoPopup.Init();
        unitChangePopup.Init();

        cardInfoPopup.Init();

        productInfoPopup.Init();
        staminaBuyPopup.Init();

        missionPopup.Init();
    }
    
    public void ExitAll()
    {
        if (_openPopupStk.Count <= 0)
            return;

        for (int i = 0; i < _openPopupStk.Count; i++)
        {
            APopup openPopup = _openPopupStk.Peek();
            openPopup.Exit();
        }
    }

    public void OpenResultPopup(bool isClear)
    {
        resultPopup.Enter(isClear);
    }

    public void OpenSettingPopup()
    {
        bool isInGame = UIManager.Inst.UIType == eUIType.InGame;
        settingPopup.Enter(isInGame);
    }

    public void OpenStageInfoPopup(int stageNum)
    {
        stageInfoPopup.Enter(stageNum);
    }

    public void OpenUnitInfoPopup(eUnit type)
    {
        unitInfoPopup.Enter(type);
    }

    public void OpenUnitChangePopup(eUnit type, Action<eUnit> onChange)
    {
        unitChangePopup.Enter(type, onChange);
    }

    public void OpenCardInfoPopup(int code)
    {
        cardInfoPopup.Enter(code);
    }

    public void OpenProductInfoPopup(eProductID id, Sprite spr, Action<eProductID> onComplete)
    {
        productInfoPopup.Enter(id, spr, onComplete);
    }

    public void OpenSteminaBuyPopup()
    {
        staminaBuyPopup.Enter();
    }

    public void OpenMissionPopup()
    {
        missionPopup.Enter();
    }
}
