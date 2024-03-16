using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageModeUI : MonoBehaviour
{
    [SerializeField] Text _stageNumTxt;
    [SerializeField] Text _moveCntTxt;

    public void Enter(int stageNum)
    {
        SetMoveCountText(0);
        SetStageNumText(stageNum);
    }

    public void Exit()
    {

    }

    public void SetMoveCountText(int moveCnt)
    {
        _moveCntTxt.text = string.Format("MOVE : {0}", moveCnt);
    }

    public void SetStageNumText(int num)
    {
        _stageNumTxt.text = string.Format("Stage\r\n{0}", num);
    }
}
