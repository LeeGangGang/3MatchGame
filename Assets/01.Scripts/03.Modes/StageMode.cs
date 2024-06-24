using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMode : AMode
{
    protected StageModeUI _ui;
    public StageModeUI UI => _ui;

    [SerializeField] AttackEventPool atkEventPool;

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
        atkEventPool.Exit();
    }

    public override void GameStart()
    {
        _isStart = true;
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
        GameManager.Inst.myUnitCtrl.AddStack(block._color, 1);
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
            GameManager.Inst.myUnitCtrl.AddStack(addValues.Key, addValues.Value);
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
            GameManager.Inst.myUnitCtrl.AddStack(addValues.Key, addValues.Value);
        }
    }

    public override void SetScore(int score)
    {
        
    }

    public override void AddAttackEvent(AttackEvent data)
    {
        atkEventPool.AddAttackEventObject(data);
    }

    public override void RemoveAttackEvent(AttackEvent data)
    {
        atkEventPool.RemoveAttackEventObject(data);
    }

    public override void RemoveDieAttackEvent(List<AttackEvent> curDatas, List<AttackEvent> dieDatas)
    {
        atkEventPool.RemoveDieAttackEventObject(curDatas, dieDatas);
    }
}
