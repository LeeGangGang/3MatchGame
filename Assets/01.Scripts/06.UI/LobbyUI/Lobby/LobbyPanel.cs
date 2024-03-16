using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel : ALobbyPanel
{
    [SerializeField] StageBtnSV stageSv;

    public override void Init()
    {
        type = eLobbyPanel.Lobby;
        isEnter = true;

        var sdm = (StageInfoDataModel)DataModelController.Inst.GetDataModel(eDataModel.StageInfoDataModel);
        stageSv.Init(sdm.GetStageTotalCount());
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
}
