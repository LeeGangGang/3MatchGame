using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public enum eColor
{
    None = -1,
    Red,
    Yellow,
    Green,
    Blue,
    White,
    Purple,
}

public enum eItem
{
    None,
    Liner_Col,
    Liner_Row,
    Package,
    MultiColor,

    Spiral,
    Solid,

    Indestructible,
}

public enum eItemAnimType
{
    None,
    Liner_Col,
    Liner_Row,
    Package,
    MultiColor,

    OneCross,
    ThreeCross,
    BigPackage,
    ChangePackage,
    ChangeLiner,
    AllRemove,
}

public enum eBlockState
{
    Idle,
    Change,
    Compose,
    Move,
    WaitSpawn,
}

public abstract class ABlock : MonoBehaviour
{
    public bool _isSelect = false;

    public bool CanMove = true;

    public eColor _color = eColor.Red;
    public eItem _item = eItem.None;

    public eBlockState state = eBlockState.Idle;

    public Position Pos;

    [SerializeField] protected SpriteRenderer mainSpr = null;

    public delegate void MoveSuccess(int x, int y);
    MoveSuccess moveSuccess;

    public bool IsGetHitCurrentTurn = false;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public virtual void Init(int x, int y, int step = 1)
    {
        Pos = new Position(x, y);

        _isSelect = false;

        state = eBlockState.Idle;
    }

    public virtual void SetActive(bool isActive)
    {
        mainSpr.gameObject.SetActive(isActive);
    }

    public virtual void ChangeItemSprite(eItem type, bool isInit = false)
    {

    }

    public virtual IEnumerator Remove(ABlock slaveBlock, Action<bool> _event)
    {
        _event?.Invoke(false);

        yield return null;
    }

    public virtual IEnumerator Remove(eColor _color, Action<bool> _event)
    {
        _event?.Invoke(false);

        yield return null;
    }

    public IEnumerator Move(Vector3 pos, int moveIdxX, int moveIdxY, MoveSuccess success, Action moveAnimEnd = null)
    {
        state = eBlockState.Move;

        moveSuccess = success;
        Vector3 startPos = transform.position;
        float interpolate = 0f;
        float dragSpeed = 7f;

        while (interpolate < 1f)
        {
            interpolate += Time.deltaTime * dragSpeed;
            transform.position = Vector3.Lerp(startPos, pos, interpolate);
            yield return null;
        }

        moveSuccess(moveIdxX, moveIdxY);

        transform.position = pos;

        // 찌그러트리기
        AnimationCurve animCv = Match3Manager.Inst.boundAnim;
        float timer = 0f;
        // 이동 방향 체크
        Vector3 dir = (pos - startPos) * 0.1f;
        while (timer < animCv.keys[animCv.length - 1].time)
        {
            yield return null;

            timer += Time.deltaTime * 2f;
            Vector3 newPos = transform.position + (dir * animCv.Evaluate(timer) * 0.07f);
            transform.position = newPos;
        }

        transform.position = pos;
        gameObject.name = string.Format("[{0}]{1} {2}_{3}", Match3Manager.Inst.StepIdx, _item, pos.x, pos.y);

        moveAnimEnd?.Invoke();
    }

    public IEnumerator Fall(Vector3 pos, int moveIdxX, int moveIdxY, MoveSuccess success)
    {
        state = eBlockState.Move;
        moveSuccess = success;

        // 내려오기
        float timer = 0f;
        float timerMin = 0.1f;
        float timerMax = 0.2f;

        Vector3 startPos = transform.position;
        Vector3 scale = transform.localScale;
        scale.y = 1.2f;
        transform.localScale = scale;

        float maxTimer = timerMin;
        float dist = Vector3.Distance(startPos, pos);
        maxTimer = Mathf.Clamp(timerMin * dist, timerMin, timerMax);
        while (timer < maxTimer)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, pos, timer / maxTimer);
            yield return null;
        }

        transform.position = pos;
        transform.localScale = Vector3.one;

        moveSuccess(moveIdxX, moveIdxY);

        // 찌그러트리기
        AnimationCurve animCv = Match3Manager.Inst.boundAnim;
        timer = 0f;
        while (timer < animCv.keys[animCv.length - 1].time)
        {
            yield return null;

            if (animCv.keys[1].time <= timer && timer <= animCv.keys[animCv.length - 1].time)
            {
                scale = transform.localScale;
                scale.y = 0.95f;
                scale.x = 1.05f;
                transform.localScale = scale;
            }

            timer += Time.deltaTime;
            Vector3 newPos = transform.position;
            newPos.y = pos.y + animCv.Evaluate(timer) * 0.07f;
            transform.position = newPos;
        }

        transform.position = pos;
        transform.localScale = Vector3.one;

        gameObject.name = string.Format("[{0}]{1} {2}_{3}", Match3Manager.Inst.StepIdx, _item, pos.x, pos.y);
    }

    public virtual void GetHit()
    {

    }
}
