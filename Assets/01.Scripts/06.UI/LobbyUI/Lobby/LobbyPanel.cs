using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : ALobbyPanel
{
    [SerializeField] StageBtnSV stageSv;

    [SerializeField] Button missionBtn;
    [SerializeField] Button achieveBtn;
    [SerializeField] Button battlepassBtn;

    public override void Init()
    {
        type = eLobbyPanel.Lobby;
        isEnter = true;

        var sdm = (StageInfoDataModel)DataModelController.Inst.GetDataModel(eDataModel.StageInfoDataModel);
        stageSv.Init(sdm.GetStageTotalCount());

        missionBtn.onClick.AddListener(OnClickMissionBtn);
        achieveBtn.onClick.AddListener(OnClickAchieveBtn);
        battlepassBtn.onClick.AddListener(OnClickBattlePassBtn);
    }

    public override void Enter()
    {
        if (isEnter)
            return;

        isEnter = true;

        gameObject.SetActive(true);
    }

    public override void Exit()
    {
        if (!isEnter)
            return;

        isEnter = false;
        gameObject.SetActive(false);
    }

    public override void UpdateUI()
    {

    }

    void OnClickMissionBtn()
    {
        UIManager.Inst.Popup.OpenMissionPopup();
    }

    void OnClickAchieveBtn()
    {

    }

    void OnClickBattlePassBtn()
    {

    }
}
