using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageBtn : MonoBehaviour
{
    [SerializeField] Button mainBtn;
    [SerializeField] Text stageNumTxt;

    public void Init(int stageNum)
    {
        SetStageNumText(stageNum);

        mainBtn.onClick.AddListener(() =>
        {
            UIManager.Inst.Popup.OpenStageInfoPopup(stageNum);
        });
    }

    void SetStageNumText(int num)
    {
        stageNumTxt.text = string.Format("STAGE\r\n{0}", num);
    }
}
