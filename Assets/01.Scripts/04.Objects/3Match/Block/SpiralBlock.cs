using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralBlock : ABlock
{
    int Hp = 1;

    public override void Init(int x, int y, int step = 1)
    {
        _item = eItem.Spiral;

        _color = eColor.None;

        gameObject.name = string.Format("[{0}]Spiral {1}_{2}", step, x, y);

        base.Init(x, y, step);
    }

    public override void ChangeItemSprite(eItem type, bool isInit = false)
    {
        mainSpr.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, _item.ToString().ToLower());
    }

    public override IEnumerator Remove(ABlock slaveBlock, Action<bool> _event)
    {
        GetHit();

        _event?.Invoke(false);

        yield return null;
    }

    public override void GetHit()
    {
        if (IsGetHitCurrentTurn == false)
        {
            Hp--;
            IsGetHitCurrentTurn = true;
        }

        if (Hp == 0)
        {
            FxManager.Inst.EnterFx(eFxID.SpiralRemove, this.transform.position);

            GameManager.Inst.RemoveBlock(this);
            Match3Manager.Inst.MapBlock[Pos.x, Pos.y] = null;
            SetActive(false);
        }
    }
}
