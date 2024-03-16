using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASubTile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer mainSpr;
    protected int Hp;
    public eSubTile Kind;
    public bool IsGetHitCurrentTurn = false;

    public virtual void Init(eSubTile kind, int hp, bool isInit)
    {
        this.Kind = kind;
        this.Hp = hp;
        ChangeSprite(hp);

        SetActive(true);
    }

    public virtual void SetActive(bool isActive)
    {
        mainSpr.gameObject.SetActive(isActive);
    }

    public bool GetActive()
    {
        return mainSpr.gameObject.activeSelf;
    }

    protected virtual void ChangeSprite(int hp)
    {

    }

    public virtual void Recover(Position pos)
    {

    }

    public virtual void GetHit(Position pos)
    {

    }

    public virtual void Spread(HashSet<ABlock> matchList)
    {
        
    }
}
