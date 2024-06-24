using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Position
{
    public int x = 0;
    public int y = 0;
    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class Match
{
    public NormalBlock MasterBlock;
    public HashSet<NormalBlock> MatchBlockLst;
    public HashSet<ABlock> RemoveBlockLst;
    public bool useItem = false; // 블럭의 아이템 효과로 삭제된 Match면 true

    public void Init()
    {
        if (MasterBlock != null)
            MasterBlock = null;

        if (MatchBlockLst == null)
            MatchBlockLst = new HashSet<NormalBlock>();
        else
            MatchBlockLst.Clear();

        if (RemoveBlockLst == null)
            RemoveBlockLst = new HashSet<ABlock>();
        else
            RemoveBlockLst.Clear();

        useItem = false;
    }
}

public enum eState
{
    Idle,
    Moving,
    Inspect,
    Falling,
    NoMatch,
    End
}

public class Match3Manager : MonoBehaviour
{
    public static Match3Manager Inst;

    public eState _state = eState.End;
    public eState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state != eState.End)
                _state = value;
        }
    }

    public int ColorKind = 5;
    public bool ItemMake = true; // 아이템 생성 여부

    public List<int> MapSizeX = new List<int>();
    public List<int> MapSizeY = new List<int>();

    [SerializeField] Transform _boardRoot = null;

    ABlock _currSelectBlock = null;

    public int StepIdx = 0;

    List<MapObjects<ABlock>> _mapBlocks = new List<MapObjects<ABlock>>();
    public List<MapObjects<ABlock>> MapBlocks => _mapBlocks;
    public MapObjects<ABlock> MapBlock
    {
        get
        {
            return _mapBlocks[StepIdx];
        }
        set
        {
            _mapBlocks[StepIdx] = value;
        }
    }

    List<MapObjects<Tile>> _mapTiles = new List<MapObjects<Tile>>();
    public List<MapObjects<Tile>> MapTiles => _mapTiles;

    public MapObjects<Tile> MapTile
    {
        get
        {
            return _mapTiles[StepIdx];
        }
        set
        {
            _mapTiles[StepIdx] = value;
        }
    }

    [SerializeField] WaitSpawnBlockController _waitSpawnBlockCtrl;
    public WaitSpawnBlockController BlockPoolCtrl => _waitSpawnBlockCtrl;

    [SerializeField] BoardTileController boardTileCtrl;
    public BoardTileController TileCtrl => boardTileCtrl;

    [SerializeField] BoardBlockController boardBlockCtrl;
    public BoardBlockController BlockCtrl => boardBlockCtrl;

    [SerializeField] public GameTextPool TextPool;

    [SerializeField] public AnimationCurve boundAnim = null;

    public int MatchCnt = 0;

    bool _showingHint = false;
    List<NormalBlock> hintCombine = new List<NormalBlock>();
    float _hintTimer = 0f;

    int _score = 0;
    int _scoreTemp = 0;

    int _combo = 0;
    int _moveCnt = 0;

    void Awake()
    {
        Inst = GetComponent<Match3Manager>();

        TextPool.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (State == eState.End)
            return;

        if (State == eState.Idle)
        {
            if (UIManager.Inst.Popup.OpenPopupStk.Count() == 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (_currSelectBlock != null)
                        return;

                    Vector3 downPos = CameraManager.Inst.BoardCam.ScreenToWorldPoint(Input.mousePosition);
                    var hit = Physics2D.OverlapPoint(downPos, 1 << LayerMask.NameToLayer("Block"));
                    if (hit != null)
                    {
                        ABlock selectBlock = hit.gameObject.GetComponentInParent<ABlock>();
                        if (selectBlock.CanMove && MapTile[selectBlock.Pos.x, selectBlock.Pos.y].IsCanMove())
                        {
                            State = eState.Moving;
                            _currSelectBlock = selectBlock;
                            _currSelectBlock._isSelect = true;
                            StartCoroutine(CheckDragCo(downPos));
                        }

                        ShowHint(false);
                    }
                }
            }
        }

        if (State == eState.Idle)
        {
            if (hintCombine.Count != 0 && !_showingHint)
            {
                _hintTimer -= Time.deltaTime;
                if (_hintTimer < 0f)
                    ShowHint(true);
            }
        }
    }

    public void GameStart(int num)
    {
        _hintTimer = 2f;

        StepIdx = 0;
        MatchCnt = 0;
        _boardRoot.localPosition = Vector3.zero;
        
        _combo = 0;
        _moveCnt = 0;

        _score = 0;
        _scoreTemp = 0;

        var sdm = (StageInfoDataModel)DataModelController.Inst.GetDataModel(eDataModel.StageInfoDataModel);
        StageMapInfo mapInfo = sdm.GetStageMapInfo(num);
        CreateStage(mapInfo);
    }

    public void GameExit()
    {
        State = eState.End;

        StopAllCoroutines();

        CameraManager.Inst.SetActiveInGameCamera(false);

        for (int i = 0; i < _mapTiles.Count; i++)
        {
            for (int x = 0; x < MapSizeX[i]; x++)
            {
                for (int y = 0; y < MapSizeY[i]; y++)
                {
                    _mapTiles[i][x, y] = null;
                    _mapBlocks[i][x, y] = null;
                }
            }
        }

        boardTileCtrl.RemoveBoardTile();
        boardBlockCtrl.RemoveBoardBlock();
        
        _mapTiles.Clear();
        _mapBlocks.Clear();

        hintCombine.Clear();
    }

    public IEnumerator FindMatchSystem()
    {
        hintCombine.Clear();
        if (FindPossibleMatch(StepIdx))
        {
            if (State == eState.NoMatch)
                State = eState.Idle;

            _hintTimer = 2f;
        }
        else
        {
            State = eState.NoMatch;

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(ShuffleAnim());

            State = eState.Idle;
        }
    }

    bool FindPossibleMatch(int stepIdx)
    {
        hintCombine.Clear();
        MatchCnt = 0;

        int maxX = MapSizeX[stepIdx];
        int maxY = MapSizeY[stepIdx];

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                if (typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x, y]) == false)
                    continue;

                if (_mapTiles[stepIdx][x, y].IsCanMove() == false)
                    continue;

                NormalBlock nBlock = (NormalBlock)_mapBlocks[stepIdx][x, y];
                List<NormalBlock> matchBlockLst;
                // 상
                if (_mapBlocks[stepIdx][x, y + 1] != null && _mapTiles[stepIdx][x, y + 1] != null && _mapTiles[stepIdx][x, y + 1].IsCanMove() &&
                    typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x, y + 1]))
                {
                    matchBlockLst = new List<NormalBlock>();
                    CheckMatchHint(stepIdx, x, y, _mapBlocks[stepIdx][x, y + 1]._color, Vector2.up, matchBlockLst);

                    if (matchBlockLst.Count >= 2 && (nBlock.hintMatchFound_Col >= 2 || nBlock.hintMatchFound_Row >= 2))
                    {
                        if (nBlock.hintMatchFound_Col >= 2)
                        {
                            matchBlockLst.RemoveAll(block => block.Pos.x != x);
                            matchBlockLst.Add((NormalBlock)_mapBlocks[stepIdx][x, y + 1]);
                        }
                        if (nBlock.hintMatchFound_Row >= 2)
                        {
                            matchBlockLst.RemoveAll(block => block.Pos.y != y);
                            matchBlockLst.Add((NormalBlock)_mapBlocks[stepIdx][x, y + 1]);
                        }

                        if (hintCombine.Concat(matchBlockLst).Any())
                        {
                            if (hintCombine.Count == 0)
                                hintCombine.AddRange(matchBlockLst);

                            goto Break;
                        }
                    }
                }

                // 하
                if (_mapBlocks[stepIdx][x, y - 1] != null && _mapTiles[stepIdx][x, y - 1] != null && _mapTiles[stepIdx][x, y - 1].IsCanMove() &&
                    typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x, y - 1]))
                {
                    matchBlockLst = new List<NormalBlock>();
                    CheckMatchHint(stepIdx, x, y, _mapBlocks[stepIdx][x, y - 1]._color, Vector2.down, matchBlockLst);

                    if (matchBlockLst.Count >= 2 && (nBlock.hintMatchFound_Col >= 2 || nBlock.hintMatchFound_Row >= 2))
                    {
                        if (nBlock.hintMatchFound_Col >= 2)
                        {
                            matchBlockLst.RemoveAll(block => block.Pos.x != x);
                            matchBlockLst.Add((NormalBlock)_mapBlocks[stepIdx][x, y - 1]);
                        }
                        if (nBlock.hintMatchFound_Row >= 2)
                        {
                            matchBlockLst.RemoveAll(block => block.Pos.y != y);
                            matchBlockLst.Add((NormalBlock)_mapBlocks[stepIdx][x, y - 1]);
                        }

                        if (hintCombine.Concat(matchBlockLst).Any())
                        {
                            if (hintCombine.Count == 0)
                                hintCombine.AddRange(matchBlockLst);

                            goto Break;
                        }
                    }
                }

                // 좌
                if (_mapBlocks[stepIdx][x - 1, y] != null && _mapTiles[stepIdx][x - 1, y] != null && _mapTiles[stepIdx][x - 1, y].IsCanMove() &&
                    typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x - 1, y]))
                {
                    matchBlockLst = new List<NormalBlock>();
                    CheckMatchHint(stepIdx, x, y, _mapBlocks[stepIdx][x - 1, y]._color, Vector2.left, matchBlockLst);

                    if (matchBlockLst.Count >= 2 && (nBlock.hintMatchFound_Col >= 2 || nBlock.hintMatchFound_Row >= 2))
                    {
                        if (nBlock.hintMatchFound_Col >= 2)
                        {
                            matchBlockLst.RemoveAll(block => block.Pos.x != x);
                            matchBlockLst.Add((NormalBlock)_mapBlocks[stepIdx][x - 1, y]);
                        }
                        if (nBlock.hintMatchFound_Row >= 2)
                        {
                            matchBlockLst.RemoveAll(block => block.Pos.y != y);
                            matchBlockLst.Add((NormalBlock)_mapBlocks[stepIdx][x - 1, y]);
                        }

                        if (hintCombine.Concat(matchBlockLst).Any())
                        {
                            if (hintCombine.Count == 0)
                                hintCombine.AddRange(matchBlockLst);

                            goto Break;
                        }
                    }
                }

                // 우
                if (_mapBlocks[stepIdx][x + 1, y] != null && _mapTiles[stepIdx][x + 1, y] != null && _mapTiles[stepIdx][x + 1, y].IsCanMove() &&
                    typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x + 1, y]))
                {
                    matchBlockLst = new List<NormalBlock>();
                    CheckMatchHint(stepIdx, x, y, _mapBlocks[stepIdx][x + 1, y]._color, Vector2.right, matchBlockLst);

                    if (matchBlockLst.Count >= 2 && (nBlock.hintMatchFound_Col >= 2 || nBlock.hintMatchFound_Row >= 2))
                    {
                        if (nBlock.hintMatchFound_Col >= 2)
                        {
                            matchBlockLst.RemoveAll(block => block.Pos.x != x);
                            matchBlockLst.Add((NormalBlock)_mapBlocks[stepIdx][x + 1, y]);
                        }
                        if (nBlock.hintMatchFound_Row >= 2)
                        {
                            matchBlockLst.RemoveAll(block => block.Pos.y != y);
                            matchBlockLst.Add((NormalBlock)_mapBlocks[stepIdx][x + 1, y]);
                        }

                        if (hintCombine.Concat(matchBlockLst).Any())
                        {
                            if (hintCombine.Count == 0)
                                hintCombine.AddRange(matchBlockLst);

                            goto Break;
                        }
                    }
                }
            }
        }

        // 일반 블록으로 노매치일 경우 아이템 조합이 가능한지 확인
        if (hintCombine.Count == 0)
        {
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (_mapTiles[stepIdx][x, y] == null || _mapTiles[stepIdx][x, y].IsCanMove() == false)
                        continue;

                    if (_mapBlocks[stepIdx][x, y] == null)
                        continue;

                    if (typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x, y]) == false)
                        continue;

                    NormalBlock nCurrBlock = (NormalBlock)_mapBlocks[stepIdx][x, y];
                    if (nCurrBlock._item == eItem.None)
                        continue;

                    NormalBlock nBlock;
                    if (_mapBlocks[stepIdx][x, y]._item == eItem.MultiColor)
                    {
                        if (hintCombine.Count == 0)
                            hintCombine.Add(nCurrBlock);

                        goto Break;
                    }
                    else
                    {
                        // 상
                        if (_mapBlocks[stepIdx][x, y + 1] != null && _mapTiles[stepIdx][x, y + 1] != null &&
                            _mapTiles[stepIdx][x, y + 1].IsCanMove() && typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x, y + 1]))
                        {
                            nBlock = (NormalBlock)_mapBlocks[stepIdx][x, y + 1];
                            if (nBlock._item != eItem.None)
                            {
                                if (hintCombine.Count == 0)
                                {
                                    hintCombine.Add(nCurrBlock);
                                    hintCombine.Add(nBlock);
                                }

                                goto Break;
                            }
                        }

                        // 하
                        if (_mapBlocks[stepIdx][x, y - 1] != null && _mapTiles[stepIdx][x, y - 1] != null &&
                            _mapTiles[stepIdx][x, y - 1].IsCanMove() && typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x, y - 1]))
                        {
                            nBlock = (NormalBlock)_mapBlocks[stepIdx][x, y - 1];
                            if (nBlock._item != eItem.None)
                            {
                                if (hintCombine.Count == 0)
                                {
                                    hintCombine.Add(nCurrBlock);
                                    hintCombine.Add(nBlock);
                                }

                                goto Break;
                            }
                        }

                        // 좌
                        if (_mapBlocks[stepIdx][x - 1, y] != null && _mapTiles[stepIdx][x - 1, y] != null &&
                            _mapTiles[stepIdx][x - 1, y].IsCanMove() && typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x - 1, y]))
                        {
                            nBlock = (NormalBlock)_mapBlocks[stepIdx][x - 1, y];
                            if (nBlock._item != eItem.None)
                            {
                                if (hintCombine.Count == 0)
                                {
                                    hintCombine.Add(nCurrBlock);
                                    hintCombine.Add(nBlock);
                                }

                                goto Break;
                            }
                        }

                        // 우
                        if (_mapBlocks[stepIdx][x + 1, y] != null && _mapTiles[stepIdx][x + 1, y] != null &&
                            _mapTiles[stepIdx][x + 1, y].IsCanMove() && typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x + 1, y]))
                        {
                            nBlock = (NormalBlock)_mapBlocks[stepIdx][x + 1, y];
                            if (nBlock._item != eItem.None)
                            {
                                if (hintCombine.Count == 0)
                                {
                                    hintCombine.Add(nCurrBlock);
                                    hintCombine.Add(nBlock);
                                }

                                goto Break;
                            }
                        }
                    }
                }
            }
        }

    Break:
        return hintCombine.Count > 0;
    }

    public void ShowHint(bool _isShow)
    {
        _showingHint = _isShow;
        if (!_isShow)
            _hintTimer = 2f;

        foreach (var hint in hintCombine)
            hint.Hint(_isShow);
    }

    public void CreateStage(StageMapInfo stage)
    {
        int stepCnt = stage.stepCount;
        if (stepCnt == 0)
            stepCnt = 1;

        ColorKind = stage.colorKind;
        ItemMake = stage.itemMake;

        MapSizeX = stage.mapSizeX;
        MapSizeY = stage.mapSizeY;
        for (int step = 0; step < stepCnt; step++)
        {
            int maxX = MapSizeX[step];
            int maxY = MapSizeY[step];

            _mapBlocks.Add(new MapObjects<ABlock>(maxX, maxY));
            _mapTiles.Add(new MapObjects<Tile>(maxX, maxY));

            // 스테이지 저장 데이터 미리 셋팅
            boardTileCtrl.CreateBoardTile(step, maxX, maxY, stage.tileList);
            boardBlockCtrl.CreateBoardBlock(step, maxX, maxY, stage.blockList);

            while (true)
            {
                if (FindPossibleMatch(step))
                    break;
                else
                    ShuffleBlock(step);
            }
        }

        boardBlockCtrl.ChangeBlockSprite();

        CameraManager.Inst.SetBoardCameraPos(MapSizeX[StepIdx], MapSizeY[StepIdx]);
        CameraManager.Inst.SetActiveInGameCamera(true);
    }

    IEnumerator ShuffleAnim()
    {
        while (true)
        {
            if (FindPossibleMatch(StepIdx))
                break;
            else
                ShuffleBlock(StepIdx);
        }

        float waitCnt = 0;
        for (int x = 0; x < MapSizeX[StepIdx]; x++)
        {
            for (int y = 0; y < MapSizeY[StepIdx]; y++)
            {
                if (MapBlock[x, y] == null)
                    continue;

                if (typeof(NormalBlock).IsInstanceOfType(MapBlock[x, y]) == false)
                    continue;

                NormalBlock block = (NormalBlock)MapBlock[x, y];

                waitCnt++;
                StartCoroutine(block.ShuffleAnimCo(() =>
                {
                    waitCnt--;
                }));
            }
        }

        yield return new WaitUntil(() => waitCnt == 0);

        State = eState.Idle;
    }

    void ShuffleBlock(int stepIdx)
    {
        for (int x = 0; x < MapSizeX[stepIdx]; x++)
        {
            List<eColor> exceptList = new List<eColor>();
            for (int y = 0; y < MapSizeY[stepIdx]; y++)
            {
                if (_mapBlocks[stepIdx][x, y] == null)
                    continue;

                if (typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x, y]) == false)
                    continue;

                int beforeSameColorXCnt = 0;
                int beforeSameColorYCnt = 0;

                NormalBlock block = (NormalBlock)_mapBlocks[stepIdx][x, y];

                if (block._item == eItem.None)
                {
                    block.Init(x, y, stepIdx);
                    boardBlockCtrl.AddChangeSpriteBlockList(block);
                }

                if (_mapBlocks[stepIdx][x - 1, y] != null && _mapBlocks[stepIdx][x - 2, y] != null)
                {
                    if (_mapBlocks[stepIdx][x - 1, y]._color == block._color && _mapBlocks[stepIdx][x - 2, y]._color == block._color)
                    {
                        exceptList.Add(_mapBlocks[stepIdx][x, y]._color);
                        beforeSameColorXCnt = 2;
                    }
                }

                if (_mapBlocks[stepIdx][x, y - 1] != null && _mapBlocks[stepIdx][x, y - 2] != null)
                {
                    if (_mapBlocks[stepIdx][x, y - 1]._color == block._color && _mapBlocks[stepIdx][x, y - 2]._color == block._color)
                    {
                        exceptList.Add(_mapBlocks[stepIdx][x, y]._color);
                        beforeSameColorYCnt = 2;
                    }
                }

                if (exceptList.Count > 0)
                {
                    if (block._item != eItem.None)
                    {
                        //왼쪽블럭 색깔 변경
                        if (beforeSameColorXCnt >= 2)
                        {
                            exceptList.Clear();

                            //왼쪽블럭을 기준블럭으로
                            if (_mapBlocks[stepIdx][x - 1, y] != null)
                            {
                                block = (NormalBlock)_mapBlocks[stepIdx][x - 1, y];
                                exceptList.Add(block._color);

                                //기준블럭의 위 검사
                                if (_mapBlocks[stepIdx][x - 1, y + 1] != null)
                                    exceptList.Add(_mapBlocks[stepIdx][x - 1, y + 1]._color);

                                //기준블럭의 아래 검사
                                if (_mapBlocks[stepIdx][x - 1, y - 1] != null)
                                    exceptList.Add(_mapBlocks[stepIdx][x - 1, y - 1]._color);

                                block.ChangeExceptColor(exceptList);
                            }
                        }

                        if (beforeSameColorYCnt >= 2)
                        {
                            exceptList.Clear();

                            //아래블럭을 기준블럭으로
                            if (_mapBlocks[stepIdx][x, y - 1] != null)
                            {
                                block = (NormalBlock)_mapBlocks[stepIdx][x, y - 1];
                                exceptList.Add(block._color);

                                //기준블럭의 왼쪽 검사
                                if (_mapBlocks[stepIdx][x - 1, y - 1] != null)
                                    exceptList.Add(_mapBlocks[stepIdx][x - 1, y - 1]._color);

                                //기준블럭의 오른쪽 검사
                                if (_mapBlocks[stepIdx][x + 1, y - 1] != null)
                                    exceptList.Add(_mapBlocks[stepIdx][x + 1, y - 1]._color);

                                block.ChangeExceptColor(exceptList);
                            }
                        }
                    }
                    else
                    {
                        if (_mapBlocks[stepIdx][x - 1, y] != null)
                            exceptList.Add(_mapBlocks[stepIdx][x - 1, y]._color);

                        if (_mapBlocks[stepIdx][x + 1, y] != null)
                            exceptList.Add(_mapBlocks[stepIdx][x + 1, y]._color);

                        if (_mapBlocks[stepIdx][x, y - 1] != null)
                            exceptList.Add(_mapBlocks[stepIdx][x, y - 1]._color);

                        if (_mapBlocks[stepIdx][x, y + 1] != null)
                            exceptList.Add(_mapBlocks[stepIdx][x, y + 1]._color);

                        block.ChangeExceptColor(exceptList);
                    }

                    exceptList.Clear();
                }
                // 여기까지 컬러중복 제한
            }
        }
    }

    private IEnumerator CheckDragCo(Vector3 downPos)
    {
        while (State == eState.Moving && _currSelectBlock != null && _currSelectBlock._isSelect)
        {
            yield return null;
            Vector3 offset = downPos - CameraManager.Inst.BoardCam.ScreenToWorldPoint(Input.mousePosition);

            if (Vector3.Magnitude(offset) > 1f)
            {
                StartCoroutine(SwitchBlocks(offset));
                break;
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (_currSelectBlock != null)
                    {
                        UnselectBlock();
                        State = eState.Idle;
                        yield break;
                    }
                }
            }
        }
    }

    private IEnumerator SwitchBlocks(Vector3 offset)
    {
        float deg = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        if (deg < 0f)
            deg += 360f;
        else if (deg >= 360f)
            deg -= 360f;

        Vector2 dir = Vector2.zero;
        if (30 > deg || deg > 330) // 좌
            dir = Vector2.left;
        else if (150 < deg && deg < 210) // 우
            dir = Vector2.right;
        else if (240 < deg && deg < 300) // 상
            dir = Vector2.up;
        else if (60 < deg && deg < 120) // 하
            dir = Vector2.down;

        int swapPosX = (int)(_currSelectBlock.Pos.x + dir.x);
        int swapPosY = (int)(_currSelectBlock.Pos.y + dir.y);
        if (dir != Vector2.zero && swapPosX >= 0 && swapPosY >= 0 &&
            swapPosX < MapSizeX[StepIdx] && swapPosY < MapSizeY[StepIdx] && _currSelectBlock._isSelect)
        {
            State = eState.Inspect;

            ABlock swapBlock = MapBlock[swapPosX, swapPosY];
            if (swapBlock != null && swapBlock.CanMove && MapTile[swapPosX, swapPosY].IsCanMove())
            {
                _currSelectBlock._isSelect = false;

                bool noMatch = true;
                int moveAnimEnd = 0;
                // 융합 가능한 아이템일 경우, 검사식 타지말고 아이템 효과 발동
                if ((swapBlock._item != eItem.Spiral && _currSelectBlock._item != eItem.Spiral) &&
                    (swapBlock._item == eItem.MultiColor || _currSelectBlock._item == eItem.MultiColor ||
                    (swapBlock._item != eItem.None && _currSelectBlock._item != eItem.None)))
                {
                    GameManager.Inst.Pause();

                    bool endCo = false;
                    Vector3 movePos = new Vector3(swapBlock.transform.position.x, swapBlock.transform.position.y, 0f);
                    Coroutine co = StartCoroutine(_currSelectBlock.Move(movePos, swapPosX, swapPosY, (x, y) =>
                    {
                        if (swapBlock._item == eItem.MultiColor && _currSelectBlock._item == eItem.None)
                        {
                            StartCoroutine(swapBlock.Remove(_currSelectBlock, (isRemove) =>
                            {
                                endCo = true;
                            }));
                        }
                        else if (_currSelectBlock._item == eItem.MultiColor && swapBlock._item == eItem.None)
                        {
                            StartCoroutine(_currSelectBlock.Remove(swapBlock, (isRemove) =>
                            {
                                endCo = true;
                            }));
                        }
                        else
                        {
                            StartCoroutine(ComposeItem((NormalBlock)_currSelectBlock, (NormalBlock)swapBlock, () =>
                            {
                                endCo = true;
                            }));
                        }
                    }));

                    yield return new WaitUntil(() => endCo);

                    AddComboCount();
                    noMatch = false;
                }
                else
                {
                    // 일반 블럭 매치 검사
                    yield return StartCoroutine(SwapMove(dir, () =>
                    {
                        moveAnimEnd++;
                    }));

                    Match[] matchs = new Match[2];
                    ABlock[] blocks = new ABlock[2] { _currSelectBlock, swapBlock };
                    eItem[] changeItemTypes = new eItem[2] { eItem.None, eItem.None };
                    for (int i = 0; i < matchs.Length; i++)
                    {
                        if (typeof(NormalBlock).IsInstanceOfType(blocks[i]) == false)
                            continue;

                        matchs[i] = new Match();
                        matchs[i].Init();
                        matchs[i].MasterBlock = (NormalBlock)blocks[i];
                        CheckMatch((NormalBlock)blocks[i], matchs[i].MatchBlockLst);

                        // DeepCopy 기준 블럭 검사 후 잡힌 모든 블럭 1회 검사
                        HashSet<NormalBlock> _copyFindBlock = new HashSet<NormalBlock>();
                        foreach (var findBlock in matchs[i].MatchBlockLst)
                        {
                            if (findBlock.Pos.x != matchs[i].MasterBlock.Pos.x || findBlock.Pos.y != matchs[i].MasterBlock.Pos.y)
                                _copyFindBlock.Add(findBlock);
                        }

                        foreach (var findBlock in _copyFindBlock)
                            CheckMatch(findBlock, matchs[i].MatchBlockLst);

                        matchs[i].MatchBlockLst.Add((NormalBlock)blocks[i]);

                        CheckSwiepBlocks(matchs[i], (value, changeItem) =>
                        {
                            changeItemTypes[i] = changeItem;

                            if (noMatch)
                                noMatch = !value;

                            if (value)
                                AddComboCount();
                        });
                    }

                    int waitCnt = 0;
                    for (int i = 0; i < matchs.Length; i++)
                    {
                        if (matchs[i] == null)
                            continue;

                        foreach (var block in matchs[i].RemoveBlockLst)
                        {
                            if (block.state == eBlockState.Change && typeof(NormalBlock).IsInstanceOfType(block))
                            {
                                if (block._item != eItem.None)
                                {
                                    waitCnt++;
                                    yield return StartCoroutine(((NormalBlock)block).UseItem(block, () =>
                                    {
                                        waitCnt--;
                                    }));
                                }
                                block._item = changeItemTypes[i];
                            }
                            else
                            {
                                waitCnt++;
                                StartCoroutine(block.Remove(block._color, (value) =>
                                {
                                    waitCnt--;
                                }));
                            }
                        }
                    }

                    yield return new WaitUntil(() => waitCnt == 0);
                }

                if (noMatch) // 이동 후 매칭이 없으면 되돌아오기
                {
                    yield return new WaitUntil(() => moveAnimEnd == 2);

                    yield return StartCoroutine(SwapMove(-dir));
                    State = eState.Idle;
                }
                else
                {
                    GameManager.Inst.Pause();

                    AddMoveCount();

                    SetBoardInit();

                    State = eState.Falling;
                    yield return StartCoroutine(BlockCtrl.Falling(ItemMake, () =>
                    {
                        _score += _scoreTemp;
                        _combo = 0;
                        _scoreTemp = 0;

                        UnselectBlock();
                    }));

                    State = eState.Idle;
                    StartCoroutine(FindMatchSystem());

                    StartCoroutine(GameManager.Inst.Restart());
                }

                State = eState.Idle;
                StartCoroutine(FindMatchSystem());

                UnselectBlock();
                yield break;
            }
            else // 엄한 곳 드래그 할 경우
            {
                State = eState.Idle;
                UnselectBlock();
            }
        }
        else // 대각, 맵 밖으로 드래그 할 경우
        {
            State = eState.Idle;
            UnselectBlock();
        }
    }

    void UnselectBlock()
    {
        if (_currSelectBlock != null)
        {
            _currSelectBlock._isSelect = false;
            _currSelectBlock = null;
        }
    }

    public void SetBoardInit()
    {
        for (int x = 0; x < MapSizeX[StepIdx]; x++)
        {
            for (int y = 0; y < MapSizeY[StepIdx]; y++)
            {
                if (MapBlock[x, y] != null)
                {
                    MapBlock[x, y].state = eBlockState.Idle;
                    MapBlock[x, y].IsGetHitCurrentTurn = false;
                    if (typeof(NormalBlock).IsInstanceOfType(MapBlock[x, y]))
                    {
                        ((NormalBlock)MapBlock[x, y]).matchFound_Col = 0;
                        ((NormalBlock)MapBlock[x, y]).matchFound_Row = 0;
                    }
                }
                if (MapTile[x, y] != null)
                {
                    if (MapTile[x, y]._subTileList.Count != 0)
                    {
                        foreach (var subTile in MapTile[x, y]._subTileList.Values)
                        {
                            subTile.IsGetHitCurrentTurn = false;
                        }
                    }
                }
            }
        }
    }

    private IEnumerator SwapMove(Vector2 dir, Action moveAnimEnd = null)
    {
        int currPosX = _currSelectBlock.Pos.x;
        int currPosY = _currSelectBlock.Pos.y;
        int swapPosX = (int)(_currSelectBlock.Pos.x + dir.x);
        int swapPosY = (int)(_currSelectBlock.Pos.y + dir.y);

        ABlock swapBlock = MapBlock[swapPosX, swapPosY];
        if (swapBlock != null && _currSelectBlock != null)
        {
            Vector3 dragPos = swapBlock.gameObject.transform.position;
            Vector3 currPos = _currSelectBlock.transform.position;

            // 이동 이펙트 연출
            Quaternion rot = default;
            if (currPosY == swapPosY)
                rot = Quaternion.Euler(new Vector3(0, 0, 90));

            float fxPosX = (currPosX + swapPosX) / 2f;
            float fxPosY = (swapPosY + currPosY) / 2f;
            FxManager.Inst.EnterFx(eFxID.BlockMove, new Vector3(fxPosX, fxPosY, 0f), rot);

            List<bool> waits = new List<bool>() { false, false };
            Coroutine co1 = StartCoroutine(swapBlock.Move(currPos, currPosX, currPosY, (x, y) =>
            {
                MapBlock[x, y] = swapBlock;
                swapBlock.Pos.x = x;
                swapBlock.Pos.y = y;
                waits[0] = true;
            }, moveAnimEnd));
            Coroutine co2 = StartCoroutine(_currSelectBlock.Move(dragPos, swapPosX, swapPosY, (x, y) =>
            {
                MapBlock[x, y] = _currSelectBlock;
                _currSelectBlock.Pos.x = x;
                _currSelectBlock.Pos.y = y;
                waits[1] = true;
            }, moveAnimEnd));

            yield return new WaitUntil(() => waits.All(x => x == true));
        }
    }

    private List<NormalBlock> CheckMatchHint(int stepIdx, int x, int y, eColor color, Vector2 swapDir, List<NormalBlock> matchingBlocks)
    {
        Vector2[] paths = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        ((NormalBlock)_mapBlocks[stepIdx][x, y]).hintMatchFound_Row = 0;
        ((NormalBlock)_mapBlocks[stepIdx][x, y]).hintMatchFound_Col = 0;

        for (int i = 0; i < paths.Length; i++)
            FindMatchHint(stepIdx, x, y, swapDir, paths[i], color, matchingBlocks);

        return matchingBlocks;
    }

    private List<NormalBlock> FindMatchHint(int stepIdx, int x, int y, Vector2 swapDir, Vector2 dir, eColor color, List<NormalBlock> matchingTiles)
    {
        int nextX = (int)(x + dir.x);
        int nextY = (int)(y + dir.y);

        int swapX = (int)(x + swapDir.x);
        int swapY = (int)(y + swapDir.y);

        if (typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][x, y]))
        {
            NormalBlock currBlock = (NormalBlock)_mapBlocks[stepIdx][x, y];
            while (nextX >= 0 && nextX < MapSizeX[stepIdx] && nextY >= 0 && nextY < MapSizeY[stepIdx])
            {
                if (typeof(NormalBlock).IsInstanceOfType(_mapBlocks[stepIdx][nextX, nextY]) == false)
                    break;

                NormalBlock nextBlock = (NormalBlock)_mapBlocks[stepIdx][nextX, nextY];
                eColor checkColor = nextBlock._color;
                if (nextX == swapX && nextY == swapY)
                    checkColor = _mapBlocks[stepIdx][x, y]._color;

                if (nextBlock == null || currBlock == null)
                    break;

                if (color == checkColor)
                {
                    if (matchingTiles.Contains(nextBlock) == false)
                        matchingTiles.Add(nextBlock);

                    if (dir == Vector2.left) // 좌
                    {
                        currBlock.hintMatchFound_Row++;
                        nextX--;
                    }
                    else if (dir == Vector2.right) // 우
                    {
                        currBlock.hintMatchFound_Row++;
                        nextX++;
                    }
                    else if (dir == Vector2.up) // 상
                    {
                        currBlock.hintMatchFound_Col++;
                        nextY++;
                    }
                    else if (dir == Vector2.down) // 하
                    {
                        currBlock.hintMatchFound_Col++;
                        nextY--;
                    }
                }
                else
                    break;
            }
        }
        return matchingTiles;
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

        if (typeof(NormalBlock).IsInstanceOfType(MapBlock[x, y]))
        {
            NormalBlock currBlock = (NormalBlock)MapBlock[x, y];
            while (nextX >= 0 && nextX < MapSizeX[StepIdx] && nextY >= 0 && nextY < MapSizeY[StepIdx])
            {
                if (typeof(NormalBlock).IsInstanceOfType(MapBlock[nextX, nextY]) == false)
                    break;

                NormalBlock nextBlock = (NormalBlock)MapBlock[nextX, nextY];
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

    private void CheckSwiepBlocks(Match match, Action<bool, eItem> matchEvent)
    {
        bool isMatch = false;
        eItem createItemType = eItem.None;

        if (match.MatchBlockLst.Count > 2)
        {
            int maxRow = match.MatchBlockLst.Max(x => x.matchFound_Row);
            int maxCol = match.MatchBlockLst.Max(x => x.matchFound_Col);
            NormalBlock masterBlock = null;

            if (2 <= maxCol && maxCol < 4 && 2 <= maxRow && maxRow < 4)
            {
                // 가지고 있는 리스트중 제일 각각 Row, Col이 큰 녀석의 포지션 교집합을 Master로 적용
                List<NormalBlock> masterBlocks = match.MatchBlockLst.Where(x => x.matchFound_Row == maxRow && x.matchFound_Col == maxCol).ToList();

                if (ItemMake)
                    createItemType = eItem.Package;

                if (masterBlocks.Count > 0)
                {
                    masterBlock = masterBlocks.First();

                    foreach (var mb in match.MatchBlockLst)
                    {
                        if (mb.Pos.x != masterBlock.Pos.x || mb.Pos.y != masterBlock.Pos.y)
                        {
                            if (mb.Pos.x == masterBlock.Pos.x || mb.Pos.y == masterBlock.Pos.y)
                            {
                                match.RemoveBlockLst.Add(mb);
                            }
                        }
                    }

                    if (createItemType != eItem.None)
                        masterBlock.ChangeItemSprite(createItemType);

                    match.RemoveBlockLst.Add(masterBlock);
                }

                isMatch = true;
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
                            if (ItemMake)
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
                                        match.RemoveBlockLst.Add(mb);
                                    }
                                }
                            }

                            if (createItemType != eItem.None)
                                masterBlock.ChangeItemSprite(createItemType);

                            match.RemoveBlockLst.Add(masterBlock);

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
                            if (ItemMake)
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
                                        match.RemoveBlockLst.Add(mb);
                                    }
                                }
                            }

                            if (createItemType != eItem.None)
                                masterBlock.ChangeItemSprite(createItemType);

                            match.RemoveBlockLst.Add(masterBlock);

                            isMatch = true;
                        }
                    }
                }
            }
        }

        AddPointSystem(match);

        matchEvent.Invoke(isMatch, createItemType);
    }

    public IEnumerator ComposeItem(NormalBlock _masterBlock, NormalBlock _slaveBlock, Action _event)
    {
        eItemAnimType composeType = eItemAnimType.None;

        // 합성한 두 블록은 제거
        _masterBlock.state = eBlockState.Compose;
        _slaveBlock.state = eBlockState.Compose;

        Match removeByItem = new Match();
        removeByItem.Init();
        removeByItem.useItem = true;
        removeByItem.MasterBlock = _masterBlock;
        removeByItem.RemoveBlockLst.Add(_masterBlock);
        removeByItem.RemoveBlockLst.Add(_slaveBlock);

        List<Position> _spreadPosList = new List<Position>();

        int waitCnt = 0;
        switch (_masterBlock._item)
        {
            case eItem.MultiColor:
                {
                    removeByItem.MasterBlock = _slaveBlock;
                    if (_slaveBlock._item == eItem.Liner_Col || _slaveBlock._item == eItem.Liner_Row)
                    {
                        composeType = eItemAnimType.ChangeLiner;
                        for (int x = 0; x < MapSizeX[StepIdx]; x++)
                        {
                            for (int y = 0; y < MapSizeY[StepIdx]; y++)
                            {
                                if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                    continue;

                                if (MapBlock[x, y]._color == _slaveBlock._color)
                                {
                                    int randomDir = UnityEngine.Random.Range((int)eItem.Liner_Col, (int)eItem.Liner_Row + 1);
                                    ABlock blobk = MapBlock[x, y];
                                    blobk._item = (eItem)randomDir;
                                    removeByItem.RemoveBlockLst.Add(blobk);
                                }
                            }
                        }
                    }
                    else if (_slaveBlock._item == eItem.Package)
                    {
                        composeType = eItemAnimType.ChangePackage;
                        for (int x = 0; x < MapSizeX[StepIdx]; x++)
                        {
                            for (int y = 0; y < MapSizeY[StepIdx]; y++)
                            {
                                if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                    continue;

                                if (MapBlock[x, y]._color == _slaveBlock._color)
                                {
                                    ABlock blobk = MapBlock[x, y];
                                    blobk._item = eItem.Package;
                                    removeByItem.RemoveBlockLst.Add(blobk);
                                }
                            }
                        }
                    }
                    else if (_slaveBlock._item == eItem.MultiColor)
                    {
                        composeType = eItemAnimType.AllRemove;
                        for (int x = 0; x < MapSizeX[StepIdx]; x++)
                        {
                            for (int y = 0; y < MapSizeY[StepIdx]; y++)
                            {
                                if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                    continue;

                                ABlock blobk = MapBlock[x, y];
                                removeByItem.RemoveBlockLst.Add(blobk);
                            }
                        }
                    }
                }
                break;
            case eItem.Liner_Col:
            case eItem.Liner_Row:
                {
                    if (_slaveBlock._item == eItem.Liner_Col || _slaveBlock._item == eItem.Liner_Row)
                    {
                        composeType = eItemAnimType.OneCross;
                        for (int x = 0; x < MapSizeX[StepIdx]; x++)
                        {
                            int y = _slaveBlock.Pos.y;

                            _spreadPosList.Add(new Position(x, y));

                            if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                continue;

                            ABlock blobk = MapBlock[x, y];
                            removeByItem.RemoveBlockLst.Add(blobk);
                        }
                        for (int y = 0; y < MapSizeY[StepIdx]; y++)
                        {
                            int x = _slaveBlock.Pos.x;

                            _spreadPosList.Add(new Position(x, y));

                            if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                continue;

                            ABlock blobk = MapBlock[x, y];
                            removeByItem.RemoveBlockLst.Add(blobk);
                        }
                    }
                    else if (_slaveBlock._item == eItem.Package)
                    {
                        composeType = eItemAnimType.ThreeCross;
                        for (int i = -1; i < 2; i++)
                        {
                            for (int x = 0; x < MapSizeX[StepIdx]; x++)
                            {
                                int y = _slaveBlock.Pos.y + i;

                                _spreadPosList.Add(new Position(x, y));

                                if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                    continue;

                                ABlock blobk = MapBlock[x, y];
                                removeByItem.RemoveBlockLst.Add(blobk);
                            }
                            for (int y = 0; y < MapSizeY[StepIdx]; y++)
                            {
                                int x = _slaveBlock.Pos.x + i;

                                _spreadPosList.Add(new Position(x, y));

                                if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                    continue;

                                ABlock blobk = MapBlock[x, y];
                                removeByItem.RemoveBlockLst.Add(blobk);
                            }
                        }
                    }
                    else if (_slaveBlock._item == eItem.MultiColor)
                    {
                        composeType = eItemAnimType.ChangeLiner;
                        for (int x = 0; x < MapSizeX[StepIdx]; x++)
                        {
                            for (int y = 0; y < MapSizeY[StepIdx]; y++)
                            {
                                if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                    continue;

                                if (MapBlock[x, y]._color == _masterBlock._color)
                                {
                                    int randomDir = UnityEngine.Random.Range((int)eItem.Liner_Col, (int)eItem.Liner_Row + 1);
                                    ABlock blobk = MapBlock[x, y];
                                    blobk._item = (eItem)randomDir;
                                    removeByItem.RemoveBlockLst.Add(blobk);
                                }
                            }
                        }
                    }
                }
                break;
            case eItem.Package:
                {
                    if (_slaveBlock._item == eItem.Liner_Col || _slaveBlock._item == eItem.Liner_Row)
                    {
                        composeType = eItemAnimType.ThreeCross;
                        for (int i = -1; i < 2; i++)
                        {
                            for (int x = 0; x < MapSizeX[StepIdx]; x++)
                            {
                                int y = _slaveBlock.Pos.y + i;

                                _spreadPosList.Add(new Position(x, y));

                                if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                    continue;

                                ABlock blobk = MapBlock[x, y];
                                removeByItem.RemoveBlockLst.Add(blobk);
                            }
                            for (int y = 0; y < MapSizeY[StepIdx]; y++)
                            {
                                int x = _slaveBlock.Pos.x + i;

                                _spreadPosList.Add(new Position(x, y));

                                if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                    continue;

                                ABlock blobk = MapBlock[x, y];
                                removeByItem.RemoveBlockLst.Add(blobk);
                            }
                        }
                    }
                    else if (_slaveBlock._item == eItem.Package)
                    {
                        composeType = eItemAnimType.BigPackage;
                        for (int x = -2; x < 3; x++)
                        {
                            for (int y = -2; y < 3; y++)
                            {
                                int targetPosX = _slaveBlock.Pos.x + x;
                                int targetPosY = _slaveBlock.Pos.y + y;

                                _spreadPosList.Add(new Position(targetPosX, targetPosY));

                                if (MapBlock[targetPosX, targetPosY] == null ||
                                    (_masterBlock.Pos.x == targetPosX && _masterBlock.Pos.y == targetPosY) ||
                                    (_slaveBlock.Pos.x == targetPosX && _slaveBlock.Pos.y == targetPosY))
                                    continue;

                                ABlock blobk = MapBlock[targetPosX, targetPosY];
                                removeByItem.RemoveBlockLst.Add(blobk);
                            }
                        }
                    }
                    else if (_slaveBlock._item == eItem.MultiColor)
                    {
                        composeType = eItemAnimType.ChangePackage;
                        for (int x = 0; x < MapSizeX[StepIdx]; x++)
                        {
                            for (int y = 0; y < MapSizeY[StepIdx]; y++)
                            {
                                if (MapBlock[x, y] == null || (_masterBlock.Pos.x == x && _masterBlock.Pos.y == y) || (_slaveBlock.Pos.x == x && _slaveBlock.Pos.y == y))
                                    continue;

                                if (MapBlock[x, y]._color == _masterBlock._color)
                                {
                                    ABlock blobk = MapBlock[x, y];
                                    blobk._item = eItem.Package;
                                    removeByItem.RemoveBlockLst.Add(blobk);
                                }
                            }
                        }
                    }
                }
                break;
        }

        float waitTime = 0f;
        Vector3 masterPos = _masterBlock.transform.position;
        switch (composeType)
        {
            case eItemAnimType.OneCross:
                ShowBoardFx(true, true, 0.1f);
                yield return UseItemWaitAnim(eItemAnimType.OneCross, masterPos);

                MapBlock[_masterBlock.Pos.x, _masterBlock.Pos.y] = null;
                MapBlock[_slaveBlock.Pos.x, _slaveBlock.Pos.y] = null;
                BlockPoolCtrl.AddBlock(_masterBlock);
                BlockPoolCtrl.AddBlock(_slaveBlock);
                break;
            case eItemAnimType.ThreeCross:
                ShowBoardFx(true, true, 0.1f);
                yield return UseItemWaitAnim(eItemAnimType.ThreeCross, masterPos);

                MapBlock[_masterBlock.Pos.x, _masterBlock.Pos.y] = null;
                MapBlock[_slaveBlock.Pos.x, _slaveBlock.Pos.y] = null;
                BlockPoolCtrl.AddBlock(_masterBlock);
                BlockPoolCtrl.AddBlock(_slaveBlock);
                break;
            case eItemAnimType.BigPackage:
                ShowBoardFx(true, true, 0.1f);
                yield return UseItemWaitAnim(eItemAnimType.BigPackage, masterPos);

                MapBlock[_masterBlock.Pos.x, _masterBlock.Pos.y] = null;
                MapBlock[_slaveBlock.Pos.x, _slaveBlock.Pos.y] = null;
                BlockPoolCtrl.AddBlock(_masterBlock);
                BlockPoolCtrl.AddBlock(_slaveBlock);
                break;
            case eItemAnimType.ChangeLiner:
            case eItemAnimType.ChangePackage:
                ShowBoardFx(true, true, (removeByItem.MatchBlockLst.Count * 0.07f) + 0.1f);

                foreach (var removeBlock in removeByItem.RemoveBlockLst)
                {
                    yield return UseItemWaitAnim(eItemAnimType.MultiColor, masterPos, removeBlock.transform.position);
                    removeBlock.ChangeItemSprite(removeBlock._item);
                    removeBlock.state = eBlockState.Idle;

                    // 서브타일 생성 (ex : Jelly)
                    if (MapTile[_masterBlock.Pos.x, _masterBlock.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly) ||
                        MapTile[_slaveBlock.Pos.x, _slaveBlock.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly))
                    {
                        MapTile[removeBlock.Pos.x, removeBlock.Pos.y].Spread(eSubTile.Jelly);
                    }
                }

                MapBlock[_masterBlock.Pos.x, _masterBlock.Pos.y] = null;
                MapBlock[_slaveBlock.Pos.x, _slaveBlock.Pos.y] = null;
                BlockPoolCtrl.AddBlock(_masterBlock);
                BlockPoolCtrl.AddBlock(_slaveBlock);

                waitTime = 0.02f;
                break;
            case eItemAnimType.AllRemove:
                waitCnt++;
                waitCnt++;
                Vector3 center = new Vector3(MapSizeX[StepIdx] / 2f - 0.5f, MapSizeY[StepIdx] / 2f - 0.5f);
                StartCoroutine(_masterBlock.Move(center, 4, 4, (x, y) =>
                {
                    waitCnt--;
                }));
                StartCoroutine(_slaveBlock.Move(center, 4, 4, (x, y) =>
                {
                    waitCnt--;
                }));

                yield return new WaitUntil(() => waitCnt == 0);

                MapBlock[_masterBlock.Pos.x, _masterBlock.Pos.y] = null;
                MapBlock[_slaveBlock.Pos.x, _slaveBlock.Pos.y] = null;

                UseItemWaitAnim(eItemAnimType.AllRemove, center);
                ShowBoardFx(true, true, 1.2f);

                BlockPoolCtrl.AddBlock(_masterBlock);
                BlockPoolCtrl.AddBlock(_slaveBlock);

                yield return new WaitForSeconds(0.08f);

                var rnd = new System.Random();
                removeByItem.RemoveBlockLst = new HashSet<ABlock>(removeByItem.RemoveBlockLst.OrderBy(block => rnd.Next()));

                waitTime = 0.001f;
                break;
            default:
                waitCnt = 0;
                break;
        }

        HashSet<ABlock> exceptList = new HashSet<ABlock>();

        // 연출 및 조건으로 인해 분기 : 서브타일 생성 (ex : Jelly)
        if (composeType != eItemAnimType.AllRemove)
        {
            // 서브타일 생성 (ex : Jelly)
            bool isContanin = false;
            if (MapTile[_masterBlock.Pos.x, _masterBlock.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly) ||
                MapTile[_slaveBlock.Pos.x, _slaveBlock.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly))
            {
                isContanin = true;
            }
            else
            {
                foreach (var spreadPos in _spreadPosList)
                {
                    if (MapTile[spreadPos.x, spreadPos.y] == null)
                        continue;

                    if (MapTile[spreadPos.x, spreadPos.y]._subTileList.ContainsKey(eSubTile.Jelly))
                    {
                        isContanin = true;
                        break;
                    }
                }
            }

            if (isContanin)
            {
                foreach (var spreadPos in _spreadPosList)
                {
                    if (MapTile[spreadPos.x, spreadPos.y] != null)
                        MapTile[spreadPos.x, spreadPos.y].Spread(eSubTile.Jelly);
                }
            }
        }

        foreach (var removeBlock in removeByItem.RemoveBlockLst)
        {
            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);

            if (composeType == eItemAnimType.AllRemove)
            {
                // 연출 및 조건으로 인해 분기 : 서브타일 생성 (ex : Jelly)
                if (MapTile[_masterBlock.Pos.x, _masterBlock.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly) ||
                    MapTile[_slaveBlock.Pos.x, _slaveBlock.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly))
                {
                    MapTile[removeBlock.Pos.x, removeBlock.Pos.y].Spread(eSubTile.Jelly);
                }
            }

            waitCnt++;
            StartCoroutine(removeBlock.Remove(removeByItem.MasterBlock, (isRemove) =>
            {
                if (!isRemove)
                    exceptList.Add(removeBlock);

                waitCnt--;
            }));
        }

        yield return new WaitUntil(() => waitCnt == 0);

        removeByItem.RemoveBlockLst.ExceptWith(exceptList);

        AddPointSystem(removeByItem);

        _event.Invoke();
    }

    public void RemoveMapBlock(int x, int y)
    {
        if (MapBlock[x, y] != null)
            MapBlock[x, y] = null;
    }

    public IEnumerator EndingUseItem()
    {
        while (true)
        {
            int waitCnt = 0;
            bool hasItem = false;
            for (int x = 0; x < MapSizeX[StepIdx]; x++)
            {
                for (int y = 0; y < MapSizeY[StepIdx]; y++)
                {
                    ABlock block = MapBlock[x, y];
                    if (block != null && block._item != eItem.None &&
                        block._item != eItem.Spiral && block._item != eItem.Solid &&
                        block._item != eItem.Indestructible)
                    {
                        // 아이템 사용
                        eColor currColor = block._color;
                        if (block._item == eItem.MultiColor)
                            currColor = (eColor)UnityEngine.Random.Range(0, ColorKind);

                        waitCnt++;
                        yield return StartCoroutine(((NormalBlock)block).Remove(currColor, (isRemove) =>
                        {
                            waitCnt--;
                            if (!hasItem)
                                hasItem = true;
                        }));
                    }
                }
            }

            yield return new WaitUntil(() => waitCnt == 0);

            if (hasItem)
            {
                State = eState.Falling;
                yield return StartCoroutine(BlockCtrl.Falling(ItemMake, () =>
                {
                    _score += _scoreTemp;
                    _combo = 0;
                    _scoreTemp = 0;

                    UnselectBlock();
                }));

                StartCoroutine(GameManager.Inst.Restart());

                State = eState.Idle;
                StartCoroutine(FindMatchSystem());
            }
            else
            {
                yield break;
            }
        }
    }

    public Coroutine UseItemWaitAnim(eItemAnimType itemType, Vector3 masterPos, Vector3 targetPos = default)
    {
        return StartCoroutine(UseItemWaitAnimCo(itemType, masterPos, targetPos));
    }

    IEnumerator UseItemWaitAnimCo(eItemAnimType itemType, Vector3 masterPos, Vector3 targetPos = default)
    {
        switch (itemType)
        {
            case eItemAnimType.None:
                break;
            case eItemAnimType.Liner_Row:
                FxManager.Inst.EnterFx(eFxID.Liner, masterPos);
                yield return new WaitForSeconds(0.1f);
                break;
            case eItemAnimType.Liner_Col:
                FxManager.Inst.EnterFx(eFxID.Liner, masterPos, Quaternion.Euler(new Vector3(0, 0, 90)));
                yield return new WaitForSeconds(0.1f);
                break;
            case eItemAnimType.Package:
                FxManager.Inst.EnterFx(eFxID.Package3x3, masterPos);
                yield return new WaitForSeconds(0.1f);
                break;
            case eItemAnimType.MultiColor:
                FxManager.Inst.EnterLineRenderFx(eFxID.Lightning, masterPos, targetPos);
                yield return new WaitForSeconds(0.05f);
                break;
            case eItemAnimType.OneCross:
                FxManager.Inst.EnterFx(eFxID.Liner, masterPos);
                FxManager.Inst.EnterFx(eFxID.Liner, masterPos, Quaternion.Euler(new Vector3(0, 0, 90)));
                yield return new WaitForSeconds(0.1f);
                break;
            case eItemAnimType.ThreeCross:
                FxManager.Inst.EnterFx(eFxID.Liner, new Vector3(masterPos.x, masterPos.y - 1, 0f));
                FxManager.Inst.EnterFx(eFxID.Liner, new Vector3(masterPos.x, masterPos.y, 0f));
                FxManager.Inst.EnterFx(eFxID.Liner, new Vector3(masterPos.x, masterPos.y + 1, 0f));

                FxManager.Inst.EnterFx(eFxID.Liner, new Vector3(masterPos.x - 1, masterPos.y, 0f), Quaternion.Euler(new Vector3(0, 0, 90)));
                FxManager.Inst.EnterFx(eFxID.Liner, new Vector3(masterPos.x, masterPos.y, 0f), Quaternion.Euler(new Vector3(0, 0, 90)));
                FxManager.Inst.EnterFx(eFxID.Liner, new Vector3(masterPos.x + 1, masterPos.y, 0f), Quaternion.Euler(new Vector3(0, 0, 90)));
                yield return new WaitForSeconds(0.1f);
                break;
            case eItemAnimType.BigPackage:
                FxManager.Inst.EnterFx(eFxID.Package5x5, masterPos);
                yield return new WaitForSeconds(0.1f);
                break;
            case eItemAnimType.AllRemove:
                FxManager.Inst.EnterFx(eFxID.MultiColorAllRemove, masterPos);
                break;
        }
    }

    public void AddPointSystem(Match match)
    {
        if (match == null || match.MasterBlock == null || match.RemoveBlockLst.Count == 0)
            return;

        int addScore = 0;
        GameManager.Inst.RemoveBlocks(match);
        foreach (var block in match.RemoveBlockLst)
        {
            //아이템별 점수 계산
            switch (block._item)
            {
                case eItem.None:
                    addScore += 10;
                    break;
                case eItem.Liner_Col:
                case eItem.Liner_Row:
                    addScore += 20;
                    break;
                case eItem.Package:
                    addScore += 30;
                    break;
                case eItem.MultiColor:
                    addScore += 40;
                    break;
            }
        }

        if (addScore == 0)
            return;

        Vector3 pos = new Vector3(match.MasterBlock.Pos.x, match.MasterBlock.Pos.y, 0);
        TextPool.EnterScore(pos, AddPoint(addScore).ToString());
    }

    public int AddPoint(int value)
    {
        int addScore = value * 10;

        _score += addScore;

        return addScore;
    }

    void AddMoveCount()
    {
        _moveCnt++;
    }

    public void AddComboCount(int combo = 1)
    {
        _combo += combo;
    }

    public void ShowBoardFx(bool showFx, bool moveCam, float time = 0.1f)
    {
        if (moveCam)
            CameraManager.Inst.ShakeBoard(time);
    }
}
