using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eGameMode
{
    None,
    Time,
}

public abstract class AMode : MonoBehaviour
{
    public eGameMode Mode;
    public int Kind;

    protected bool _isStart = false;
    protected bool _isBoardAnim = false;

    public abstract void Init(int kind);
    public abstract void Enter();
    public abstract void Exit();
    public abstract void GameStart();
    public abstract void Pause();
    public abstract void Restart();

    public abstract void SetScore(int score);

    public abstract void RemoveBlocks(HashSet<ABlock> blocks);
    public abstract void RemoveBlocks(Match match);

    public abstract void RemoveBlock(ABlock block);

    public virtual void RemoveSubTiles(HashSet<ASubTile> subTiles)
    {

    }

    public virtual void RemoveSubTile(ASubTile subTile)
    {

    }

    public abstract void AddAttackEvent(AttackEvent data);
    public abstract void RemoveAttackEvent(AttackEvent data);
    public abstract void RemoveDieAttackEvent(List<AttackEvent> curDatas, List<AttackEvent> dieDatas);
}
