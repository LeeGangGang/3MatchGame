using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardTileController : MonoBehaviour
{
    [SerializeField] Transform _titleRoot = null;

    [Header("---------- Tile Prefab ----------")]
    [SerializeField] GameObject _tileObj = null;
    [SerializeField] GameObject _stickObj = null;
    [SerializeField] GameObject _wireObj = null;
    //[SerializeField] GameObject _jellyObj = null;

    // 오브젝트 삭제용 캐싱
    List<MapObjects<Tile>> _allTile = new List<MapObjects<Tile>>();

    float scaleX = 1f;
    float scaleY = 1f;

    // 스테이지의 Step별 거리
    int stepDistance = 10;

    public void CreateBoardTile(int step, int maxX, int maxY, List<TileInfo[,]> tileList)
    {
        _allTile.Add(new MapObjects<Tile>(maxX, maxY));

        Dictionary<int, List<Position>> marshmellowList = new Dictionary<int, List<Position>>();

        // 스테이지 저장 데이터 미리 셋팅
        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                Tile tile = null;
                TileInfo tileinfo = tileList[step][x, y];

                if (tileinfo != null)
                {
                    tile = Instantiate(_tileObj, new Vector3(x * scaleX + (step * stepDistance), y * scaleY, 0), Quaternion.identity, _titleRoot).GetComponent<Tile>();
                    tile.Init(x, y);

                    if (tileinfo.hasSpawnPos)
                    {
                        SpawnPos spawnPos = Match3Manager.Inst.BlockPoolCtrl.CreateSpawnPos(new Vector3(x * scaleX + (step * stepDistance), y * scaleY, 0));
                        spawnPos.Init(x, y);
                        tile.SetSpawnPos(spawnPos);
                    }

                    if (tileinfo.subTileList != null)
                    {
                        foreach (var subTile in tileinfo.subTileList)
                            CreateSubTile(subTile, tile);
                    }

                    Match3Manager.Inst.MapTiles[step][x, y] = tile;
                }

                _allTile[step][x, y] = tile;
            }
        }
    }

    public void RemoveBoardTile()
    {
        for (int i = 0; i < _allTile.Count; i++)
        {
            foreach (var tile in _allTile[i].All())
            {
                if (tile == null)
                    continue;

                Destroy(tile.gameObject);
            }
        }

        _allTile.Clear();
    }

    /// <summary>
    /// 게임 중 생성되는 SubTile
    /// </summary>
    public void AddSubTile(SubTileInfo info, Tile tile)
    {
        if (Match3Manager.Inst.MapBlock[tile.Pos.x, tile.Pos.y] != null)
        {
            if (Match3Manager.Inst.MapBlock[tile.Pos.x, tile.Pos.y]._item == eItem.Indestructible)
                return;
        }

        ASubTile subTile = null;

        subTile.Init(info.type, info.kind, false);

        tile.AddSubTile(subTile);
    }

    /// <summary>
    /// 스테이지 생성용 1칸 SubTile
    /// </summary>
    void CreateSubTile(SubTileInfo info, Tile tile)
    {
        if (Match3Manager.Inst.MapBlock[tile.Pos.x, tile.Pos.y] != null)
        {
            if (Match3Manager.Inst.MapBlock[tile.Pos.x, tile.Pos.y]._item == eItem.Indestructible)
                return;
        }

        ASubTile subTile = null;
        if (info.type == eSubTile.Stick)
            subTile = Instantiate(_stickObj, tile.transform.position, Quaternion.identity, tile.transform).GetComponent<ASubTile>();
        else if (info.type == eSubTile.Wire)
            subTile = Instantiate(_wireObj, tile.transform.position, Quaternion.identity, tile.transform).GetComponent<ASubTile>();

        subTile.Init(info.type, info.kind, true);

        tile.AddSubTile(subTile);
    }
}
