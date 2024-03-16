using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndestructibleBlock : ABlock
{
    public override void Init(int x, int y, int step = 1)
    {
        _color = eColor.None;

        gameObject.name = string.Format("[{0}]Indestructible {1}_{2}", step, x, y);

        base.Init(x, y, step);
    }

    public override void ChangeItemSprite(eItem type, bool isInit = false)
    {
        mainSpr.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, _item.ToString().ToLower());
    }
}
