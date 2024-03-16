using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPos : Tile
{
    public override void Init(int x, int y)
    {
        Pos = new Position(x, y);
    }
}
