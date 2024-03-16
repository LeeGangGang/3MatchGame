using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalBlock : ABlock
{
    // Inspect 용
    public int matchFound_Row = 0;
    public int matchFound_Col = 0;

    // Hint & NoMatch 용
    public int hintMatchFound_Row = 0;
    public int hintMatchFound_Col = 0;

    [SerializeField] SpriteRenderer _iconSpr = null;
    [SerializeField] SpriteRenderer _itemSpr = null;
    [SerializeField] SpriteMask _mainSprMask = null;

    Coroutine _hintDirector = null;

    public override void Init(int x, int y, int step = 1)
    {
        _item = eItem.None;

        _color = (eColor)UnityEngine.Random.Range(0, Match3Manager.Inst.ColorKind);

        matchFound_Row = 0;
        matchFound_Col = 0;

        hintMatchFound_Row = 0;
        hintMatchFound_Col = 0;

        gameObject.name = string.Format("[{0}]None {1}_{2}", step, x, y);

        base.Init(x, y, step);
    }

    public override void ChangeItemSprite(eItem type, bool isInit = false)
    {
        _itemSpr.gameObject.SetActive(false);

        if (!isInit)
        {
            state = eBlockState.Change;
            FxManager.Inst.EnterFx(eFxID.BlockRemove, _color, transform.position);

            HitCrossPosition();
        }

        switch (type)
        {
            case eItem.None:
                mainSpr.sprite = _mainSprMask.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, string.Format("item_{0:00}", (int)_color));
                _iconSpr.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, string.Format("attribute_{0:00}", (int)_color));
                mainSpr.sortingOrder = 6;
                _itemSpr.sortingOrder = 7;
                _iconSpr.gameObject.SetActive(true);
                _itemSpr.gameObject.SetActive(false);
                break;
            case eItem.Liner_Col:
            case eItem.Liner_Row:
            case eItem.Package:
                mainSpr.sprite = _mainSprMask.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, string.Format("item_{0:00}", (int)_color));
                _iconSpr.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, string.Format("attribute_{0:00}", (int)_color));
                _itemSpr.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, type.ToString());
                mainSpr.sortingOrder = 7;
                _itemSpr.sortingOrder = 8;
                _iconSpr.gameObject.SetActive(true);
                _itemSpr.gameObject.SetActive(true);
                break;
            case eItem.MultiColor:
                _color = eColor.None;
                mainSpr.sprite = _mainSprMask.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, "item_ColorRemover_Five");
                mainSpr.sortingOrder = 9;
                _iconSpr.gameObject.SetActive(false);
                _itemSpr.gameObject.SetActive(false);
                break;
        }
    }

    public void ChangeExceptColor(List<eColor> _exceptColors)
    {
        if (_exceptColors.Count == 0)
            return;

        _exceptColors.AddRange(GetExceptColorList());
        _exceptColors.Add(eColor.None);

        List<eColor> colorList = Enum.GetValues(typeof(eColor)).Cast<eColor>().Except(_exceptColors).ToList();
        int colorIdx = UnityEngine.Random.Range(0, colorList.Count);

        _color = colorList[colorIdx];
        //ChangeItemSprite(_item, true);

        state = eBlockState.Idle;
    }

    public void Hint(bool _isActive)
    {
        if (_isActive)
        {
            if (_hintDirector == null)
                _hintDirector = StartCoroutine(HintCo());
        }
        else
        {
            if (_hintDirector != null)
            {
                StopCoroutine(_hintDirector);
                _hintDirector = null;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    IEnumerator HintCo()
    {
        float scale = 0f;
        float weight = 0.2f;
        float spd = 5f;
        while (true)
        {
            scale += Time.deltaTime * spd;
            transform.localScale = new Vector3(1f + Mathf.Sin(scale) * weight, 1f + Mathf.Sin(scale) * weight, 1f);
            yield return null;
        }
    }

    public IEnumerator ShuffleAnimCo(Action _onComplete)
    {
        bool isComplete = false;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Linear))
            .AppendCallback(() => ChangeItemSprite(_item, true))
            .Append(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                _onComplete?.Invoke();
                isComplete = true;
            });

        yield return new WaitUntil(() => isComplete == true);
    }

    public override IEnumerator Remove(ABlock _slaveBlock, Action<bool> _event)
    {
        if (state == eBlockState.WaitSpawn || state == eBlockState.Change)
        {
            _event.Invoke(false);
            yield break;
        }

        // 융합 블록 또는 아이템으로 변경될 블록이 아니면 스포너에 담기위해 상태 변환
        if (state != eBlockState.Compose && state != eBlockState.Change)
            state = eBlockState.WaitSpawn;

        StopAllCoroutines();

        if (state != eBlockState.Compose && _item != eItem.None)
        {
            Match3Manager.Inst.MapBlock[Pos.x, Pos.y] = null;
            yield return Match3Manager.Inst.StartCoroutine(UseItem(_slaveBlock, null));
        }
        else
        {
            FxManager.Inst.EnterFx(eFxID.BlockRemove, _color, transform.position);
        }

        Match3Manager.Inst.MapBlock[Pos.x, Pos.y] = null;

        HitCrossPosition();

        _event?.Invoke(true);

        Match3Manager.Inst.BlockPoolCtrl.AddBlock(this);
        SetActive(false);
    }

    public override IEnumerator Remove(eColor _color, Action<bool> _event)
    {
        if (state == eBlockState.WaitSpawn || state == eBlockState.Change)
        {
            _event.Invoke(false);
            yield break;
        }

        // 융합 블록 또는 아이템으로 변경될 블록이 아니면 스포너에 담기위해 상태 변환
        if (state != eBlockState.Compose && state != eBlockState.Change)
            state = eBlockState.WaitSpawn;

        StopAllCoroutines();

        if (state != eBlockState.Compose && _item != eItem.None)
        {
            Match3Manager.Inst.MapBlock[Pos.x, Pos.y] = null;
            yield return Match3Manager.Inst.StartCoroutine(UseItem(_color, null));
        }
        else
        {
            FxManager.Inst.EnterFx(eFxID.BlockRemove, _color, transform.position);
        }

        Match3Manager.Inst.MapBlock[Pos.x, Pos.y] = null;

        HitCrossPosition();

        _event?.Invoke(true);

        Match3Manager.Inst.BlockPoolCtrl.AddBlock(this);
        SetActive(false);
    }

    List<eColor> GetExceptColorList()
    {
        List<eColor> _exceptColors = new List<eColor>();
        switch (Match3Manager.Inst.ColorKind)
        {
            case 4:
                _exceptColors.Add(eColor.White);
                _exceptColors.Add(eColor.Purple);
                break;
            case 5:
                _exceptColors.Add(eColor.Purple);
                break;
        }

        return _exceptColors;
    }

    public IEnumerator UseItem(eColor targetColor, Action _event)
    {
        Match removeByItem = new Match();
        removeByItem.Init();
        removeByItem.useItem = true;
        removeByItem.MasterBlock = this;

        int maxX = Match3Manager.Inst.MapSizeX[Match3Manager.Inst.StepIdx];
        int maxY = Match3Manager.Inst.MapSizeY[Match3Manager.Inst.StepIdx];

        List<Position> _spreadPosList = new List<Position>();
        switch (_item)
        {
            case eItem.MultiColor:
                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        if (Match3Manager.Inst.MapBlock[x, y] == null)
                            continue;

                        if (Match3Manager.Inst.MapBlock[x, y]._color == targetColor)
                        {
                            removeByItem.RemoveBlockLst.Add(Match3Manager.Inst.MapBlock[x, y]);
                            _spreadPosList.Add(new Position(x, y));
                        }
                    }
                }
                break;
            case eItem.Liner_Col:
                for (int i = 0; i < maxY; i++)
                {
                    _spreadPosList.Add(new Position(Pos.x, i));

                    if (Match3Manager.Inst.MapBlock[Pos.x, i] == null ||
                        (Match3Manager.Inst.MapBlock[Pos.x, i].Pos.x == Pos.x && Match3Manager.Inst.MapBlock[Pos.x, i].Pos.y == Pos.y))
                        continue;

                    ABlock blobk = Match3Manager.Inst.MapBlock[Pos.x, i];
                    removeByItem.RemoveBlockLst.Add(blobk);
                }
                break;
            case eItem.Liner_Row:
                for (int i = 0; i < maxX; i++)
                {
                    _spreadPosList.Add(new Position(i, Pos.y));

                    if (Match3Manager.Inst.MapBlock[i, Pos.y] == null ||
                        (Match3Manager.Inst.MapBlock[i, Pos.y].Pos.x == Pos.x && Match3Manager.Inst.MapBlock[i, Pos.y].Pos.y == Pos.y))
                        continue;

                    ABlock blobk = Match3Manager.Inst.MapBlock[i, Pos.y];
                    removeByItem.RemoveBlockLst.Add(blobk);
                }
                break;
            case eItem.Package:
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        int targetPosX = Pos.x + x;
                        int targetPosY = Pos.y + y;

                        _spreadPosList.Add(new Position(targetPosX, targetPosY));

                        if (Match3Manager.Inst.MapBlock[targetPosX, targetPosY] == null ||
                            (Match3Manager.Inst.MapBlock[targetPosX, targetPosY].Pos.x == Pos.x && Match3Manager.Inst.MapBlock[targetPosX, targetPosY].Pos.y == Pos.y))
                            continue;

                        ABlock blobk = Match3Manager.Inst.MapBlock[targetPosX, targetPosY];
                        removeByItem.RemoveBlockLst.Add(blobk);
                    }
                }
                break;
        }

        int waitCnt = 0;
        float waitTime = 0f;
        if (_item != eItem.MultiColor)
        {
            Match3Manager.Inst.ShowBoardFx(true, true);

            eItemAnimType _animType = eItemAnimType.None;
            if (_item == eItem.Liner_Col)
                _animType = eItemAnimType.Liner_Col;
            else if (_item == eItem.Liner_Row)
                _animType = eItemAnimType.Liner_Row;
            else if (_item == eItem.Package)
                _animType = eItemAnimType.Package;

            yield return Match3Manager.Inst.UseItemWaitAnim(_animType, transform.position);

            // 서브타일 생성 (ex : Jelly)
            bool isContanin = false;
            if (Match3Manager.Inst.MapTile[this.Pos.x, this.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly))
            {
                isContanin = true;
            }
            else
            {
                foreach (var spreadPos in _spreadPosList)
                {
                    if (Match3Manager.Inst.MapTile[spreadPos.x, spreadPos.y] == null)
                        continue;
                    
                    if (Match3Manager.Inst.MapTile[spreadPos.x, spreadPos.y]._subTileList.ContainsKey(eSubTile.Jelly))
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
                    if (Match3Manager.Inst.MapTile[spreadPos.x, spreadPos.y] != null)
                        Match3Manager.Inst.MapTile[spreadPos.x, spreadPos.y].Spread(eSubTile.Jelly);
                }
            }
        }
        else
        {
            Match3Manager.Inst.ShowBoardFx(true, true, (removeByItem.RemoveBlockLst.Count * 0.07f) + 0.1f);
            //HapticManager.Inst.Enter(1000 + 50 * removeByItem.RemoveBlockLst.Count);
            foreach (var removeBlock in removeByItem.RemoveBlockLst)
            {
                // 서브타일 생성 (ex : Jelly)
                if (Match3Manager.Inst.MapTile[this.Pos.x, this.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly))
                    Match3Manager.Inst.MapTile[removeBlock.Pos.x, removeBlock.Pos.y].Spread(eSubTile.Jelly);

                yield return Match3Manager.Inst.UseItemWaitAnim(eItemAnimType.MultiColor, transform.position, removeBlock.transform.position);
            }

            waitTime = 0.02f;
        }

        HashSet<ABlock> exceptList = new HashSet<ABlock>();
        foreach (var removeBlock in removeByItem.RemoveBlockLst)
        {
            waitCnt++;
            yield return new WaitForSeconds(waitTime);
            Match3Manager.Inst.StartCoroutine(removeBlock.Remove(removeByItem.MasterBlock, (isRemove) =>
            {
                if (!isRemove)
                    exceptList.Add(removeBlock);

                waitCnt--;
            }));
        }

        removeByItem.RemoveBlockLst.Add(this);

        yield return new WaitUntil(() => waitCnt == 0);

        Match3Manager.Inst.AddPointSystem(removeByItem);

        yield return null;

        _event?.Invoke();
    }

    public IEnumerator UseItem(ABlock slaveBlock, Action _event)
    {
        Match removeByItem = new Match();
        removeByItem.Init();
        removeByItem.useItem = true;
        removeByItem.MasterBlock = this;

        eColor targetColor = slaveBlock._color;

        int maxX = Match3Manager.Inst.MapSizeX[Match3Manager.Inst.StepIdx];
        int maxY = Match3Manager.Inst.MapSizeY[Match3Manager.Inst.StepIdx];

        List<Position> _spreadPosList = new List<Position>();
        switch (_item)
        {
            case eItem.MultiColor:
                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        if (Match3Manager.Inst.MapBlock[x, y] == null)
                            continue;

                        if (Match3Manager.Inst.MapBlock[x, y]._color == targetColor)
                        {
                            removeByItem.RemoveBlockLst.Add(Match3Manager.Inst.MapBlock[x, y]);
                            _spreadPosList.Add(new Position(x, y));
                        }
                    }
                }
                break;
            case eItem.Liner_Col:
                for (int i = 0; i < maxY; i++)
                {
                    _spreadPosList.Add(new Position(Pos.x, i));

                    if (Match3Manager.Inst.MapBlock[Pos.x, i] == null ||
                        (Match3Manager.Inst.MapBlock[Pos.x, i].Pos.x == Pos.x && Match3Manager.Inst.MapBlock[Pos.x, i].Pos.y == Pos.y))
                        continue;

                    ABlock blobk = Match3Manager.Inst.MapBlock[Pos.x, i];
                    removeByItem.RemoveBlockLst.Add(blobk);
                }
                break;
            case eItem.Liner_Row:
                for (int i = 0; i < maxX; i++)
                {
                    _spreadPosList.Add(new Position(i, Pos.y));

                    if (Match3Manager.Inst.MapBlock[i, Pos.y] == null ||
                        (Match3Manager.Inst.MapBlock[i, Pos.y].Pos.x == Pos.x && Match3Manager.Inst.MapBlock[i, Pos.y].Pos.y == Pos.y))
                        continue;

                    ABlock blobk = Match3Manager.Inst.MapBlock[i, Pos.y];
                    removeByItem.RemoveBlockLst.Add(blobk);
                }
                break;
            case eItem.Package:
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        int targetPosX = Pos.x + x;
                        int targetPosY = Pos.y + y;

                        _spreadPosList.Add(new Position(targetPosX, targetPosY));

                        if (Match3Manager.Inst.MapBlock[targetPosX, targetPosY] == null ||
                            (Match3Manager.Inst.MapBlock[targetPosX, targetPosY].Pos.x == Pos.x && Match3Manager.Inst.MapBlock[targetPosX, targetPosY].Pos.y == Pos.y))
                            continue;

                        ABlock blobk = Match3Manager.Inst.MapBlock[targetPosX, targetPosY];
                        removeByItem.RemoveBlockLst.Add(blobk);
                    }
                }
                break;
        }

        int waitCnt = 0;
        float waitTime = 0f;
        if (_item != eItem.MultiColor)
        {
            Match3Manager.Inst.ShowBoardFx(true, true);

            eItemAnimType _animType = eItemAnimType.None;
            if (_item == eItem.Liner_Col)
                _animType = eItemAnimType.Liner_Col;
            else if (_item == eItem.Liner_Row)
                _animType = eItemAnimType.Liner_Row;
            else if (_item == eItem.Package)
                _animType = eItemAnimType.Package;

            yield return Match3Manager.Inst.UseItemWaitAnim(_animType, transform.position);

            // 서브타일 생성 (ex : Jelly)
            bool isContanin = false;
            if (Match3Manager.Inst.MapTile[this.Pos.x, this.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly))
            {
                isContanin = true;
            }
            else
            {
                foreach (var spreadPos in _spreadPosList)
                {
                    if (Match3Manager.Inst.MapTile[spreadPos.x, spreadPos.y] == null)
                        continue;

                    if (Match3Manager.Inst.MapTile[spreadPos.x, spreadPos.y]._subTileList.ContainsKey(eSubTile.Jelly))
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
                    if (Match3Manager.Inst.MapTile[spreadPos.x, spreadPos.y] != null)
                        Match3Manager.Inst.MapTile[spreadPos.x, spreadPos.y].Spread(eSubTile.Jelly);
                }
            }
        }
        else
        {
            Match3Manager.Inst.ShowBoardFx(true, true, (removeByItem.RemoveBlockLst.Count * 0.07f) + 0.1f);
            //HapticManager.Inst.Enter(1000 + 50 * removeByItem.RemoveBlockLst.Count);
            foreach (var removeBlock in removeByItem.RemoveBlockLst)
            {
                // 서브타일 생성 (ex : Jelly)
                if (Match3Manager.Inst.MapTile[this.Pos.x, this.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly) ||
                    Match3Manager.Inst.MapTile[slaveBlock.Pos.x, slaveBlock.Pos.y]._subTileList.ContainsKey(eSubTile.Jelly))
                {
                    Match3Manager.Inst.MapTile[removeBlock.Pos.x, removeBlock.Pos.y].Spread(eSubTile.Jelly);
                }

                yield return Match3Manager.Inst.UseItemWaitAnim(eItemAnimType.MultiColor, transform.position, removeBlock.transform.position);
            }

            waitTime = 0.02f;
        }

        HashSet<ABlock> exceptList = new HashSet<ABlock>();
        foreach (var removeBlock in removeByItem.RemoveBlockLst)
        {
            waitCnt++;
            yield return new WaitForSeconds(waitTime);
            Match3Manager.Inst.StartCoroutine(removeBlock.Remove(removeByItem.MasterBlock, (isRemove) =>
            {
                if (!isRemove)
                    exceptList.Add(removeBlock);

                waitCnt--;
            }));
        }

        removeByItem.RemoveBlockLst.Add(this);

        yield return new WaitUntil(() => waitCnt == 0);

        Match3Manager.Inst.AddPointSystem(removeByItem);

        yield return null;

        _event?.Invoke();
    }

    void HitCrossPosition()
    {
        // ---- Block Hit ----
        // left
        if (Match3Manager.Inst.MapBlock[Pos.x - 1, Pos.y] != null)
            Match3Manager.Inst.MapBlock[Pos.x - 1, Pos.y].GetHit();

        // right
        if (Match3Manager.Inst.MapBlock[Pos.x + 1, Pos.y] != null)
            Match3Manager.Inst.MapBlock[Pos.x + 1, Pos.y].GetHit();

        // up
        if (Match3Manager.Inst.MapBlock[Pos.x, Pos.y + 1] != null)
            Match3Manager.Inst.MapBlock[Pos.x, Pos.y + 1].GetHit();

        // down
        if (Match3Manager.Inst.MapBlock[Pos.x, Pos.y - 1] != null)
            Match3Manager.Inst.MapBlock[Pos.x, Pos.y - 1].GetHit();

        // ---- Tile Hit ----
        if (Match3Manager.Inst.MapTile[Pos.x, Pos.y] != null)
            Match3Manager.Inst.MapTile[Pos.x, Pos.y].GetHit();
    }
}
