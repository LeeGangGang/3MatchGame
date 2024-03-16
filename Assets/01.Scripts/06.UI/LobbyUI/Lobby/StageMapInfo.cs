using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockInfo
{
    public eItem item;
    public eColor color;
}

[Serializable]
public class TileInfo
{
    public bool hasSpawnPos = false;
    public List<SubTileInfo> subTileList;
}

[Serializable]
public class SubTileInfo
{
    public eSubTile type;
    public int kind;
}

[Serializable]
public class StageMapInfo
{
    public int stageLevel;
    public int stepCount;

    public int colorKind;

    public List<int> mapSizeX;
    public List<int> mapSizeY;

    public bool itemMake;

    public List<TileInfo[,]> tileList;
    public List<BlockInfo[,]> blockList;
}
