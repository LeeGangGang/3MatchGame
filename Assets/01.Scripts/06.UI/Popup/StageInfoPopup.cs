using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPopup : APopup
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button startBtn;

    [SerializeField] Text stageNumTxt;

    [SerializeField] List<StageEnemyInfo> enemyInfoList = new List<StageEnemyInfo>();

    [SerializeField] List<GameObject> colorList = new List<GameObject>();
    [SerializeField] Text colorCntTxt;

    int stageNum;

    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Exit();
        });

        startBtn.onClick.AddListener(() =>
        {
            Exit();

            UIManager.Inst.ShowFadePanel(() =>
            {
                UIManager.Inst.ClickGameModeBtn(eGameMode.None, stageNum);
            });
        });
    }

    public void Enter(int num)
    {
        stageNum = num;
        base.Enter(() =>
        {
            SetStageNumText(stageNum);
            SetEnemyInfo(stageNum);
            SetMapBlockColorInfo(stageNum);
        });
    }

    public override void Exit()
    {
        base.Exit();
    }

    void SetStageNumText(int num)
    {
        stageNumTxt.text = string.Format("STAGE {0}", num);
    }

    void SetEnemyInfo(int num)
    {
        var sdm = (StageInfoDataModel)DataModelController.Inst.GetDataModel(eDataModel.StageInfoDataModel);

        int idx = 0;
        foreach (var enemyInfos in sdm.GetStageEnemyInfo(num))
        {
            string name = enemyInfos.Key.ToString();
            foreach (var enemyInfo in enemyInfos.Value)
            {
                this.enemyInfoList[idx].Enter(name, enemyInfo.Key, enemyInfo.Value);
                idx++;
            }
        }

        for (int i = idx; i < enemyInfoList.Count; i++)
            enemyInfoList[i].SetActive(false);
    }

    void SetMapBlockColorInfo(int cnt)
    {
        var sdm = (StageInfoDataModel)DataModelController.Inst.GetDataModel(eDataModel.StageInfoDataModel);

        int colorCnt = sdm.GetStageMapInfo(cnt).colorKind;
        colorCntTxt.text = colorCnt.ToString();
        for (int i = 0; i < colorList.Count; i++)
        {
            if (i < colorCnt)
                colorList[i].SetActive(true);
            else
                colorList[i].SetActive(false);
        }
    }
}
