using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum eSubTile
{
    None,
    Stick,
    Wire,
    Jelly,
}

public class Tile : MonoBehaviour
{
    public Position Pos;

    public bool HasSpawner
    {
        get
        {
            return _spawnPos != null;
        }
    }

    SpawnPos _spawnPos = null;

    [SerializeField] SpriteRenderer _mainSpr = null;

    public Dictionary<eSubTile, ASubTile> _subTileList = new Dictionary<eSubTile, ASubTile>();

    public virtual void Init(int x, int y)
    {
        Pos = new Position(x, y);

        _mainSpr.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, string.Format("border_{0:00}", Mathf.Abs(Pos.x + Pos.y) % 2));
    }

    public void AddSubTile(ASubTile subTile)
    {
        _subTileList.Add(subTile.Kind, subTile);
    }

    public SpawnPos GetSpawnPos()
    {
        return _spawnPos;
    }

    public void SetSpawnPos(SpawnPos spawnPos)
    {
        _spawnPos = spawnPos;
    }

    public bool IsCanMove()
    {
        bool isCanMove = true;

        if (_subTileList.ContainsKey(eSubTile.Wire))
        {
            if (_subTileList[eSubTile.Wire].GetActive())
                isCanMove = false;
        }

        return isCanMove;
    }

    public void RemoveSubTile(ASubTile removeTile)
    {
        if (_subTileList.ContainsValue(removeTile))
        {
            _subTileList.Remove(removeTile.Kind);
        }
    }

    public void CoverTileSpecialBlock()
    {
        if (_subTileList == null || _subTileList.Count == 0)
            return;
    }

    public void GetHit(eItem type = eItem.None)
    {
        if (_subTileList == null || _subTileList.Count == 0)
            return;

        switch (type)
        {
            case eItem.Solid:
                break;
            default:
                Dictionary<eSubTile, ASubTile> copyList = new Dictionary<eSubTile, ASubTile>(_subTileList);
                foreach (var subTile in copyList)
                    subTile.Value.GetHit(Pos);
                break;
        }
    }

    public void Spread(eSubTile kind)
    {
        if (_subTileList.ContainsKey(kind))
            return;
        else
        {
            SubTileInfo info = new SubTileInfo();
            info.type = kind;
            Match3Manager.Inst.TileCtrl.AddSubTile(info, this);
        }
    }
}
