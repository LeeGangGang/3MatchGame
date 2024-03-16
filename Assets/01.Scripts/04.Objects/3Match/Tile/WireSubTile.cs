using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireSubTile : ASubTile
{
    public override void Init(eSubTile kind, int hp, bool isInit)
    {
        base.Init(kind, hp, isInit);
    }

    protected override void ChangeSprite(int hp)
    {
        switch (hp)
        {
            case 0:
                mainSpr.gameObject.SetActive(false);
                break;
            default:
                mainSpr.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, "wire");
                break;
        }
    }

    public override void GetHit(Position pos)
    {
        if (IsGetHitCurrentTurn == false)
        {
            Hp--;
            if (Hp == 0)
            {
                FxManager.Inst.EnterFx(eFxID.WireRemove, this.transform.position);
                GameManager.Inst.RemoveSubTile(this);
                SetActive(false);
            }

            IsGetHitCurrentTurn = true;
        }
    }
}
