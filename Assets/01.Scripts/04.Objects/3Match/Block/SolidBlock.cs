using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBlock : ABlock
{
    int Hp = 1;

    public override void Init(int x, int y, int step = 1)
    {
        _color = eColor.None;

        gameObject.name = string.Format("[{0}]Solid {1}_{2}", step, x, y);

        base.Init(x, y, step);
    }

    public override void ChangeItemSprite(eItem type, bool isInit = false)
    {
        string imgName = string.Format("{0}_{1}", _item.ToString().ToLower(), Hp);
        mainSpr.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, imgName);
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
            eFxID fxID = (eFxID)Enum.Parse(typeof(eFxID), string.Format("SolidSplash_{0}", Hp));
            FxManager.Inst.EnterFx(fxID, this.transform.position);

            Hp--;
            if (Hp == 0)
            {
                GameManager.Inst.RemoveBlock(this);
                Match3Manager.Inst.MapBlock[Pos.x, Pos.y] = null;

                // ---- Tile Hit ----
                if (Match3Manager.Inst.MapTile[Pos.x, Pos.y] != null)
                    Match3Manager.Inst.MapTile[Pos.x, Pos.y].GetHit(_item);

                SetActive(false);
            }
            else
            {
                string imgName = string.Format("{0}_{1}", _item.ToString().ToLower(), Hp);
                mainSpr.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, imgName);
            }

            IsGetHitCurrentTurn = true;
        }
    }

    public void SetHp(int hp)
    {
        Hp = hp;
    }
}
