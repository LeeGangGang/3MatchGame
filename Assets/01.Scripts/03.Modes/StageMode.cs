using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMode : AMode
{
    protected StageModeUI _ui;
    public StageModeUI UI => _ui;

    int _moveCnt = 0;
    int _stageNum = 0;

    public override void Init(int stageNum)
    {
        _isStart = false;
        _isBoardAnim = false;

        _moveCnt = 0;
        _stageNum = stageNum;

        _ui = GetComponentInChildren<StageModeUI>();
    }

    public override void Enter()
    {
        _ui.Enter(_stageNum);
    }

    public override void Exit()
    {
        StopAllCoroutines();

        _ui.Exit();
    }

    public override void GameStart()
    {
        _isStart = true;
    }

    public override void CheckingGameEnd()
    {
        StartCoroutine(CheckGameEnd());
    }

    protected override IEnumerator CheckGameEnd()
    {
        bool isClear = false;
        while (true)
        {
            yield return null;

            List<Unit> _liveEnemyList = GameManager.Inst._enemyCtrl.FindLiveUnit();
            if (_liveEnemyList.Count == 0)
            {
                isClear = true;
                break;
            }
            
            List<Unit> _liveMyUnitList = GameManager.Inst._myUnitCtrl.FindLiveUnit();
            if (_liveMyUnitList.Count == 0)
                break;

            if (_isBoardAnim)
                continue;
        }

        yield return new WaitUntil(() => Match3Manager.Inst.State == eState.Idle);

        Match3Manager.Inst.State = eState.End;

        Match3Manager.Inst.ShowHint(false);

        if (isClear == false)
            yield return StartCoroutine(UIManager.Inst.Game.ShowGameEnd(0));
        else
            yield return StartCoroutine(UIManager.Inst.Game.ShowPraise(1));

        yield return StartCoroutine(Match3Manager.Inst.EndingUseItem());

        yield return new WaitForSeconds(1f);

        UIManager.Inst.Popup.OpenResultPopup(isClear);
        
        Exit();
    }

    public override void Pause()
    {
        _isStart = false;
    }

    public override void Restart()
    {
        _isStart = true;
        _moveCnt++;

        _ui.SetMoveCountText(_moveCnt);
    }

    public override void RemoveBlock(ABlock block)
    {
        GameManager.Inst._myUnitCtrl.AddStack(block._color, 1);
    }

    public override void RemoveBlocks(HashSet<ABlock> blocks)
    {
        Dictionary<eColor, int> _addValueList = new Dictionary<eColor, int>();
        foreach (var block in blocks)
        {
            if (_addValueList.ContainsKey(block._color))
                _addValueList[block._color]++;
            else
                _addValueList.Add(block._color, 1);
        }

        foreach (var addValues in _addValueList)
        {
            GameManager.Inst._myUnitCtrl.AddStack(addValues.Key, addValues.Value);
        }
    }

    public override void RemoveBlocks(Match match)
    {
        Dictionary<eColor, int> _addValueList = new Dictionary<eColor, int>();
        foreach (var block in match.RemoveBlockLst)
        {
            if (_addValueList.ContainsKey(block._color))
                _addValueList[block._color]++;
            else
                _addValueList.Add(block._color, 1);
        }

        foreach (var addValues in _addValueList)
        {
            GameManager.Inst._myUnitCtrl.AddStack(addValues.Key, addValues.Value);
        }
    }

    public override void SetScore(int score)
    {
        
    }
}
