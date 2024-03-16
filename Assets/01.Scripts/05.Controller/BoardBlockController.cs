using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardBlockController : MonoBehaviour
{
    [SerializeField] Transform _blockRoot = null;
    public Transform BlockRoot => _blockRoot;

    [Header("---------- Block Prefab ----------")]
    [SerializeField] GameObject _solidObj = null;
    [SerializeField] GameObject _spiralObj = null;
    [SerializeField] GameObject _indestructibleObj = null;
    [SerializeField] GameObject _blockObj = null;

    // 오브젝트 삭제용 캐싱
    List<ABlock> _allBlock = new List<ABlock>();

    float scaleX = 1f;
    float scaleY = 1f;

    // 스테이지의 Step별 거리
    int stepDistance = 10;

    // Shuffle 후 스프라이트 변경이 필요한 블럭 리스트
    HashSet<NormalBlock> _changeSpriteBlockList = new HashSet<NormalBlock>();

    public void CreateBoardBlock(int step, int maxX, int maxY, List<BlockInfo[,]> blockList)
    {
        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                if (Match3Manager.Inst.MapTiles[step][x, y] == null)
                    continue;

                if (blockList[step][x, y] != null)
                {
                    ABlock block;
                    BlockInfo info = blockList[step][x, y];
                    Vector3 pos = new Vector3(x * scaleX + (step * stepDistance), y * scaleY);
                    if (info.item == eItem.Spiral)
                    {
                        block = Instantiate(_spiralObj, pos, Quaternion.identity, _blockRoot).GetComponent<SpiralBlock>();
                        block.Init(x, y, step);
                        block._item = info.item;
                        block.SetActive(true);
                    }
                    else if (info.item == eItem.Solid)
                    {
                        block = Instantiate(_solidObj, pos, Quaternion.identity, _blockRoot).GetComponent<SolidBlock>();
                        block.Init(x, y, step);
                        block._item = info.item;
                        ((SolidBlock)block).SetHp((int)info.color);
                        block.SetActive(true);
                    }
                    else if (info.item == eItem.Indestructible)
                    {
                        block = Instantiate(_indestructibleObj, pos, Quaternion.identity, _blockRoot).GetComponent<IndestructibleBlock>();
                        block.Init(x, y, step);
                        block._item = info.item;
                        block.SetActive(true);
                    }
                    else
                    {
                        block = Instantiate(_blockObj, pos, Quaternion.identity, _blockRoot).GetComponent<NormalBlock>();
                        block.Init(x, y, step);
                        block._item = info.item;
                        block._color = info.color;
                    }

                    block.ChangeItemSprite(info.item, true);
                    Match3Manager.Inst.MapBlocks[step][x, y] = block;

                    _allBlock.Add(block);
                }
            }
        }

        // 랜덤 블럭 생성
        for (int x = 0; x < maxX; x++)
        {
            List<eColor> exceptList = new List<eColor>();
            for (int y = 0; y < maxY; y++)
            {
                if (Match3Manager.Inst.MapBlocks[step][x, y] != null || Match3Manager.Inst.MapTiles[step][x, y] == null)
                    continue;

                NormalBlock block = Instantiate(_blockObj, Match3Manager.Inst.MapTiles[step][x, y].transform.position, Quaternion.identity, _blockRoot).GetComponent<NormalBlock>();
                block.Init(x, y, step);
                block.ChangeItemSprite(block._item, true);

                // 맵 생성 시 블록 매칭 안되도록 제한
                if (typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x - 1, y]) && typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x - 2, y]))
                {
                    if (Match3Manager.Inst.MapBlocks[step][x - 1, y] != null && Match3Manager.Inst.MapBlocks[step][x - 2, y] != null)
                    {
                        if (Match3Manager.Inst.MapBlocks[step][x - 1, y]._color == block._color && Match3Manager.Inst.MapBlocks[step][x - 2, y]._color == block._color)
                            exceptList.Add(block._color);
                    }
                }
                if (typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x + 1, y]) && typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x + 2, y]))
                {
                    if (Match3Manager.Inst.MapBlocks[step][x + 1, y] != null && Match3Manager.Inst.MapBlocks[step][x + 2, y] != null)
                    {
                        if (Match3Manager.Inst.MapBlocks[step][x + 1, y]._color == block._color && Match3Manager.Inst.MapBlocks[step][x + 2, y]._color == block._color)
                            exceptList.Add(block._color);
                    }
                }

                if (typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x, y - 1]) && typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x, y - 2]))
                {
                    if (Match3Manager.Inst.MapBlocks[step][x, y - 1] != null && Match3Manager.Inst.MapBlocks[step][x, y - 2] != null)
                    {
                        if (Match3Manager.Inst.MapBlocks[step][x, y - 1]._color == block._color && Match3Manager.Inst.MapBlocks[step][x, y - 2]._color == block._color)
                            exceptList.Add(block._color);
                    }
                }
                if (typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x, y + 1]) && typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x, y + 2]))
                {
                    if (Match3Manager.Inst.MapBlocks[step][x, y + 1] != null && Match3Manager.Inst.MapBlocks[step][x, y + 2] != null)
                    {
                        if (Match3Manager.Inst.MapBlocks[step][x, y + 1]._color == block._color && Match3Manager.Inst.MapBlocks[step][x, y + 2]._color == block._color)
                            exceptList.Add(block._color);
                    }
                }

                if (typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x, y + 1]) && typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x, y - 1]))
                {
                    if (Match3Manager.Inst.MapBlocks[step][x, y + 1] != null && Match3Manager.Inst.MapBlocks[step][x, y - 1] != null)
                    {
                        if (Match3Manager.Inst.MapBlocks[step][x, y + 1]._color == block._color && Match3Manager.Inst.MapBlocks[step][x, y - 1]._color == block._color)
                            exceptList.Add(block._color);
                    }
                }
                if (typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x + 1, y]) && typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlocks[step][x - 1, y]))
                {
                    if (Match3Manager.Inst.MapBlocks[step][x + 1, y] != null && Match3Manager.Inst.MapBlocks[step][x - 1, y] != null)
                    {
                        if (Match3Manager.Inst.MapBlocks[step][x + 1, y]._color == block._color && Match3Manager.Inst.MapBlocks[step][x - 1, y]._color == block._color)
                            exceptList.Add(block._color);
                    }
                }

                if (exceptList.Count > 0)
                {
                    block.ChangeExceptColor(exceptList);
                    exceptList.Clear();

                    _changeSpriteBlockList.Add(block);
                }
                // 여기까지 컬러중복 제한

                Match3Manager.Inst.MapBlocks[step][x, y] = block;
                _allBlock.Add(block);
            }
        }

        // BlockPool에 미리 생성
        for (int i = 0; i < 20; i++)
            CreateSupplementBlock();
    }

    public void RemoveBoardBlock()
    {
        foreach (var block in _allBlock)
        {
            if (block == null)
                continue;

            Destroy(block.gameObject);
        }

        Match3Manager.Inst.BlockPoolCtrl.Clear();
        _allBlock.Clear();
    }

    public void AddChangeSpriteBlockList(NormalBlock block)
    {
        _changeSpriteBlockList.Add(block);
    }

    public void ChangeBlockSprite()
    {
        foreach (var block in _changeSpriteBlockList)
        {
            if (block != null)
                block.ChangeItemSprite(block._item, true);
        }

        _changeSpriteBlockList.Clear();
    }

    public void CreateSupplementBlock()
    {
        ABlock block = Instantiate(_blockObj, Vector3.zero, Quaternion.identity, _blockRoot).GetComponent<NormalBlock>();
        Match3Manager.Inst.BlockPoolCtrl.AddBlock(block);
        _allBlock.Add(block);
    }

    public IEnumerator RemoveOrChangeBlocks(Match match, bool itemMake, Action<bool> matchEvent)
    {
        bool isMatch = false;
        int waitCnt = 0;
        if (match.MatchBlockLst.Count > 2)
        {
            int maxRow = match.MatchBlockLst.Max(x => x.matchFound_Row);
            int maxCol = match.MatchBlockLst.Max(x => x.matchFound_Col);
            eItem createItemType = eItem.None;
            NormalBlock masterBlock = null;

            if (2 <= maxCol && maxCol < 4 && 2 <= maxRow && maxRow < 4)
            {
                // 가지고 있는 리스트중 제일 각각 Row, Col이 큰 녀석의 포지션 교집합을 Master로 적용
                List<NormalBlock> masterBlocks = match.MatchBlockLst.Where(x => x.matchFound_Row == maxRow && x.matchFound_Col == maxCol).ToList();

                if (itemMake)
                    createItemType = eItem.Package;

                isMatch = true;

                if (masterBlocks.Count > 0)
                {
                    masterBlock = masterBlocks.First();

                    foreach (var mb in match.MatchBlockLst)
                    {
                        if (mb.Pos.x != masterBlock.Pos.x || mb.Pos.y != masterBlock.Pos.y)
                        {
                            if (mb.Pos.x == masterBlock.Pos.x || mb.Pos.y == masterBlock.Pos.y)
                            {
                                waitCnt++;
                                StartCoroutine(mb.Remove(mb._color, (isRemove) =>
                                {
                                    match.RemoveBlockLst.Add(mb);
                                    waitCnt--;
                                }));
                            }
                        }
                    }

                    if (createItemType != eItem.None)
                    {
                        if (masterBlock._item != eItem.None)
                            yield return StartCoroutine(masterBlock.UseItem(masterBlock, null));

                        masterBlock._item = createItemType;
                        masterBlock.ChangeItemSprite(createItemType);
                        match.RemoveBlockLst.Add(masterBlock);
                    }
                    else
                    {
                        waitCnt++;
                        StartCoroutine(masterBlock.Remove(masterBlock._color, (isRemove) =>
                        {
                            if (isRemove)
                                match.RemoveBlockLst.Add(masterBlock);

                            waitCnt--;
                        }));
                    }
                }
            }
            else
            {
                // 어느쪽이 길게 연결되어있는지 여기서 확인
                bool isRowMatch = false;
                if (maxRow > maxCol)
                    isRowMatch = true;

                if (isRowMatch)
                {
                    List<NormalBlock> masterBlocks = match.MatchBlockLst.Where(x => x.matchFound_Row == maxRow).ToList();
                    if (masterBlocks.Count > 0)
                    {
                        if (masterBlocks.Contains(match.MasterBlock))
                            masterBlock = match.MasterBlock;
                        else
                            masterBlock = masterBlocks.First();

                        if (masterBlock.matchFound_Row >= 2)
                        {
                            // 가로
                            createItemType = eItem.None;
                            // 3개 일반
                            // 4개 가로 펑! 아이템 생성
                            // 5개 초코볼 아이템 생성
                            if (itemMake)
                            {
                                if (masterBlock.matchFound_Row == 3)
                                    createItemType = eItem.Liner_Col;
                                else if (masterBlock.matchFound_Row >= 4)
                                    createItemType = eItem.MultiColor;
                            }

                            foreach (var mb in match.MatchBlockLst)
                            {
                                if (mb.Pos.y == masterBlock.Pos.y)
                                {
                                    if (mb.Pos.x != masterBlock.Pos.x)
                                    {
                                        waitCnt++;
                                        StartCoroutine(mb.Remove(mb._color, (isRemove) =>
                                        {
                                            match.RemoveBlockLst.Add(mb);
                                            waitCnt--;
                                        }));
                                    }
                                }
                            }

                            if (createItemType != eItem.None)
                            {
                                if (masterBlock._item != eItem.None)
                                    yield return StartCoroutine(masterBlock.UseItem(masterBlock, null));

                                masterBlock._item = createItemType;
                                masterBlock.ChangeItemSprite(createItemType);
                                match.RemoveBlockLst.Add(masterBlock);
                            }
                            else
                            {
                                waitCnt++;
                                StartCoroutine(masterBlock.Remove(masterBlock._color, (isRemove) =>
                                {
                                    if (isRemove)
                                        match.RemoveBlockLst.Add(masterBlock);

                                    waitCnt--;
                                }));
                            }

                            isMatch = true;
                        }
                    }
                }
                else
                {
                    List<NormalBlock> masterBlocks = match.MatchBlockLst.Where(x => x.matchFound_Col == maxCol).ToList();
                    if (masterBlocks.Count > 0)
                    {
                        if (masterBlocks.Contains(match.MasterBlock))
                            masterBlock = match.MasterBlock;
                        else
                            masterBlock = masterBlocks.First();

                        if (masterBlock.matchFound_Col >= 2)
                        {
                            // 세로
                            createItemType = eItem.None;
                            // 3개 일반
                            // 4개 세로 펑! 아이템 생성
                            // 5개 초코볼 아이템 생성
                            if (itemMake)
                            {
                                if (masterBlock.matchFound_Col == 3)
                                    createItemType = eItem.Liner_Row;
                                else if (masterBlock.matchFound_Col >= 4)
                                    createItemType = eItem.MultiColor;
                            }

                            foreach (var mb in match.MatchBlockLst)
                            {
                                if (mb.Pos.x == masterBlock.Pos.x)
                                {
                                    if (mb.Pos.y != masterBlock.Pos.y)
                                    {
                                        waitCnt++;
                                        StartCoroutine(mb.Remove(mb._color, (isRemove) =>
                                        {
                                            match.RemoveBlockLst.Add(mb);
                                            waitCnt--;
                                        }));
                                    }
                                }
                            }

                            if (createItemType != eItem.None)
                            {
                                if (masterBlock._item != eItem.None)
                                    yield return StartCoroutine(masterBlock.UseItem(masterBlock, null));

                                masterBlock._item = createItemType;
                                masterBlock.ChangeItemSprite(createItemType);
                                match.RemoveBlockLst.Add(masterBlock);
                            }
                            else
                            {
                                waitCnt++;
                                StartCoroutine(masterBlock.Remove(masterBlock._color, (isRemove) =>
                                {
                                    if (isRemove)
                                        match.RemoveBlockLst.Add(masterBlock);

                                    waitCnt--;
                                }));
                            }

                            isMatch = true;
                        }
                    }
                }
            }
        }

        yield return new WaitUntil(() => waitCnt == 0);

        matchEvent.Invoke(isMatch);
    }

    public IEnumerator Falling(bool itemMake, Action _onComplete)
    {
        WaitForSeconds fallDelay = new WaitForSeconds(0.05f);

        bool isFull = false;
        List<Coroutine> waitCheckCoLst = new List<Coroutine>();
        while (!isFull)
        {
            for (int x = 0; x < Match3Manager.Inst.MapSizeX[Match3Manager.Inst.StepIdx]; x++)
                waitCheckCoLst.Add(StartCoroutine(FallingOneLine(x)));

            foreach (var waitCo in waitCheckCoLst)
                yield return waitCo;

            yield return StartCoroutine(AllInspectNoMatch(itemMake, (value) =>
            {
                if (value)
                {
                    isFull = true;
                    _onComplete?.Invoke();
                }
            }));
        }
    }

    IEnumerator FallingOneLine(int x)
    {
        var mapBlock = Match3Manager.Inst.MapBlock;
        var mapTile = Match3Manager.Inst.MapTile;

        int cnt = 0;
        float spawnDelayTime = 0.1f;
        WaitForSeconds spawnDelay = new WaitForSeconds(spawnDelayTime);

        for (int y = 0; y < Match3Manager.Inst.MapSizeY[Match3Manager.Inst.StepIdx]; y++)
        {
            if (mapTile[x, y] == null)
                continue;

            if (mapTile[x, y].IsCanMove() == false)
                continue;

            if (mapBlock[x, y] == null)
            {
                ABlock aboveBlock = FindAboveBlock(new Position(x, y));
                Vector3 tilePos = mapTile[x, y].transform.position;
                if (aboveBlock != null)
                {
                    mapBlock[aboveBlock.Pos.x, aboveBlock.Pos.y] = null;
                    cnt++;
                    StartCoroutine(aboveBlock.Fall(tilePos, x, y, (x, y) =>
                    {
                        aboveBlock.Pos.x = x;
                        aboveBlock.Pos.y = y;
                        mapBlock[x, y] = aboveBlock;
                        cnt--;
                    }));
                }
                else
                {
                    Position lastFindPos = new Position(x, y);
                    SpawnPos spawn = FindAboveSpawnPos(x, y, ref lastFindPos);
                    if (spawn == null)
                    {
                        continue;
                    }
                    else
                    {
                        // 스폰 블럭은 딜레이 줬다가 드랍
                        yield return spawnDelay;

                        aboveBlock = Match3Manager.Inst.BlockPoolCtrl.SetFallBlock(spawn);
                        if (aboveBlock == null)
                            continue;

                        cnt++;
                        StartCoroutine(aboveBlock.Fall(tilePos, x, y, (x, y) =>
                        {
                            aboveBlock.Pos.x = x;
                            aboveBlock.Pos.y = y;
                            mapBlock[x, y] = aboveBlock;
                            cnt--;
                        }));
                    }
                }
            }
        }

        yield return new WaitUntil(() => cnt == 0);
    }

    SpawnPos FindAboveSpawnPos(int x, int y, ref Position lastFindPos)
    {
        var mapBlock = Match3Manager.Inst.MapBlock;
        var mapTile = Match3Manager.Inst.MapTile;

        if (y >= Match3Manager.Inst.MapSizeY[Match3Manager.Inst.StepIdx])
            return null;

        if (mapBlock[x, y] != null)
            return null;

        SpawnPos aboveSpawnPos;
        if (mapTile[x, y] == null)
        {
            aboveSpawnPos = FindAboveSpawnPos(x, y + 1, ref lastFindPos);
        }
        else
        {
            aboveSpawnPos = mapTile[x, y].GetSpawnPos();
            lastFindPos = new Position(mapTile[x, y].Pos.x, mapTile[x, y].Pos.y);
            if (aboveSpawnPos == null)
                aboveSpawnPos = FindAboveSpawnPos(x, y + 1, ref lastFindPos);
        }

        return aboveSpawnPos;
    }

    ABlock FindAboveBlock(Position abovePos)
    {
        var mapBlock = Match3Manager.Inst.MapBlock;
        var mapTile = Match3Manager.Inst.MapTile;

        ABlock aboveBlock = null;
        if (abovePos.y >= Match3Manager.Inst.MapSizeY[Match3Manager.Inst.StepIdx])
            return null;

        if (mapTile[abovePos.x, abovePos.y] == null)
        {
            aboveBlock = FindAboveBlock(new Position(abovePos.x, abovePos.y + 1));
        }
        else
        {
            if (mapTile[abovePos.x, abovePos.y].IsCanMove() == false)
                return null;

            if (mapBlock[abovePos.x, abovePos.y] == null)
            {
                if (mapTile[abovePos.x, abovePos.y].HasSpawner)
                    return null;

                aboveBlock = FindAboveBlock(new Position(abovePos.x, abovePos.y + 1));
            }
            else
            {
                if (mapBlock[abovePos.x, abovePos.y].CanMove == false)
                    return null;
                else
                    aboveBlock = mapBlock[abovePos.x, abovePos.y];
            }
        }
        return aboveBlock;
    }

    private IEnumerator AllInspectNoMatch(bool itemMake, Action<bool> _event)
    {
        var mapBlock = Match3Manager.Inst.MapBlock;

        bool noMatch = true;
        int waitCnt = 0;

        for (int x = 0; x < Match3Manager.Inst.MapSizeX[Match3Manager.Inst.StepIdx]; x++)
        {
            for (int y = 0; y < Match3Manager.Inst.MapSizeY[Match3Manager.Inst.StepIdx]; y++)
            {
                if (mapBlock[x, y] == null)
                    continue;

                if (typeof(NormalBlock).IsInstanceOfType(mapBlock[x, y]))
                {
                    NormalBlock nCurrBlock = (NormalBlock)mapBlock[x, y];

                    Match match = new Match();
                    match.Init();
                    match.MasterBlock = nCurrBlock;
                    match.MatchBlockLst.Add(nCurrBlock);
                    CheckMatch(nCurrBlock, match.MatchBlockLst);
                    HashSet<NormalBlock> _copyFindBlock = new HashSet<NormalBlock>();
                    foreach (var findBlock in match.MatchBlockLst)
                    {
                        if (findBlock.Pos.x != match.MasterBlock.Pos.x || findBlock.Pos.y != match.MasterBlock.Pos.y)
                            _copyFindBlock.Add(findBlock);
                    }

                    foreach (var findBlock in _copyFindBlock)
                        CheckMatch(findBlock, match.MatchBlockLst);

                    match.MatchBlockLst.Add(nCurrBlock);

                    waitCnt++;
                    StartCoroutine(RemoveOrChangeBlocks(match, itemMake, (value) =>
                    {
                        waitCnt--;

                        if (noMatch)
                            noMatch = !value;

                        if (value)
                            Match3Manager.Inst.AddComboCount();

                        Match3Manager.Inst.AddPointSystem(match);
                    }));
                }
            }
        }

        yield return new WaitUntil(() => waitCnt == 0);

        _event.Invoke(noMatch);

        Match3Manager.Inst.SetBoardInit();
    }

    private HashSet<NormalBlock> CheckMatch(NormalBlock block, HashSet<NormalBlock> matchingBlocks)
    {
        int x = block.Pos.x;
        int y = block.Pos.y;

        block.matchFound_Col = 0;
        block.matchFound_Row = 0;

        Vector2[] path = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        for (int i = 0; i < path.Length; i++)
            FindMatch(x, y, path[i], block._color, matchingBlocks);

        return matchingBlocks;
    }

    private HashSet<NormalBlock> FindMatch(int x, int y, Vector2 dir, eColor color, HashSet<NormalBlock> matchingTiles)
    {
        int nextX = (int)(x + dir.x);
        int nextY = (int)(y + dir.y);

        int stepIdx = Match3Manager.Inst.StepIdx;
        var mapBlock = Match3Manager.Inst.MapBlock;

        if (typeof(NormalBlock).IsInstanceOfType(Match3Manager.Inst.MapBlock[x, y]))
        {
            NormalBlock currBlock = (NormalBlock)Match3Manager.Inst.MapBlock[x, y];
            while (nextX >= 0 && nextX < Match3Manager.Inst.MapSizeX[stepIdx] && nextY >= 0 && nextY < Match3Manager.Inst.MapSizeY[stepIdx])
            {
                if (typeof(NormalBlock).IsInstanceOfType(mapBlock[nextX, nextY]) == false)
                    break;

                NormalBlock nextBlock = (NormalBlock)mapBlock[nextX, nextY];
                if (nextBlock == null || currBlock == null)
                    break;

                if (color == nextBlock._color)
                {
                    matchingTiles.Add(nextBlock);
                    if (dir == Vector2.left) // 좌
                    {
                        currBlock.matchFound_Row++;
                        nextX--;
                    }
                    else if (dir == Vector2.right) // 우
                    {
                        currBlock.matchFound_Row++;
                        nextX++;
                    }
                    else if (dir == Vector2.up) // 상
                    {
                        currBlock.matchFound_Col++;
                        nextY++;
                    }
                    else if (dir == Vector2.down) // 하
                    {
                        currBlock.matchFound_Col++;
                        nextY--;
                    }
                }
                else
                    break;
            }
        }
        return matchingTiles;
    }
}
