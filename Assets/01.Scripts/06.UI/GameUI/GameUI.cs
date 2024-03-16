using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class GameUI : MonoBehaviour
{
    [SerializeField] RectTransform _boardRectTr;
    public RectTransform BoardRectTr => _boardRectTr;

    [SerializeField] UnitSlotUI _unitUI;
    public UnitSlotUI UnitSlotUI => _unitUI;

    public void Enter(eGameMode _mode)
    {
        _unitUI.Enter();

        this.gameObject.SetActive(true);
    }

    public void Exit()
    {
        _unitUI.Exit();

        this.gameObject.SetActive(false);
    }

    public IEnumerator ShowGameStart(Action _onComplete)
    {
        _onComplete.Invoke();

        yield return null;

        //if (_qDirections.Contains(_gameStartLabel) == false)
        //{
        //    _qDirections.Enqueue(_gameStartLabel);

        //    if (_qDirections.Peek().Equals(_gameStartLabel))
        //    {
        //        _qDirections.Peek().Enter(idx, () =>
        //        {
        //            _gameStartLabel.gameObject.SetActive(false);
        //            _onComplete.Invoke();
        //            _qDirections.Dequeue();
        //        });
        //    }
        //    else
        //    {
        //        yield return new WaitUntil(() => _qDirections.Peek().Equals(_gameStartLabel));
        //        _gameStartLabel.Enter(idx, () =>
        //        {
        //            _gameStartLabel.gameObject.SetActive(false);
        //            _onComplete.Invoke();
        //            _qDirections.Dequeue();
        //        });
        //    }
        //}
    }

    public IEnumerator ShowPraise(int idx)
    {
        yield return null;

        //if (_qDirections.Contains(_praiseLabel) == false)
        //{
        //    _qDirections.Enqueue(_praiseLabel);

        //    if (_qDirections.Peek().Equals(_praiseLabel))
        //    {
        //        _qDirections.Peek().Enter(idx, () =>
        //        {
        //            _praiseLabel.gameObject.SetActive(false);
        //            _qDirections.Dequeue();
        //        });
        //    }
        //    else
        //    {
        //        yield return new WaitUntil(() => _qDirections.Peek().Equals(_praiseLabel));
        //        _praiseLabel.Enter(idx, () =>
        //        {
        //            _praiseLabel.gameObject.SetActive(false);
        //            _qDirections.Dequeue();
        //        });
        //    }
        //}
    }

    public IEnumerator ShowGameEnd(int idx)
    {
        yield return null;

        //if (_qDirections.Contains(_gameEndLabel) == false)
        //{
        //    bool isEnd = false;
        //    _qDirections.Enqueue(_gameEndLabel);

        //    if (_qDirections.Peek().Equals(_gameEndLabel))
        //    {
        //        _qDirections.Peek().Enter(idx, () =>
        //        {
        //            isEnd = true;
        //            _qDirections.Dequeue();
        //        });

        //        yield return new WaitUntil(() => isEnd);

        //        yield return new WaitForSeconds(1f);

        //        _gameEndLabel.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        yield return new WaitUntil(() => _qDirections.Peek().Equals(_gameEndLabel));

        //        _gameEndLabel.Enter(idx, () =>
        //        {
        //            isEnd = true;
        //            _qDirections.Dequeue();
        //        });

        //        yield return new WaitUntil(() => isEnd);

        //        yield return new WaitForSeconds(1f);

        //        _gameEndLabel.gameObject.SetActive(false);
        //    }
        //}
    }

    public IEnumerator ShowNoMatch()
    {
        yield return null;

        //if (_qDirections.Contains(_noMatchLabel) == false)
        //{
        //    bool isEnd = false;
        //    _qDirections.Enqueue(_noMatchLabel);

        //    if (_qDirections.Peek().Equals(_noMatchLabel))
        //    {
        //        _qDirections.Peek().Enter(() =>
        //        {
        //            isEnd = true;
        //            _qDirections.Dequeue();
        //        });

        //        yield return new WaitUntil(() => isEnd);

        //        _noMatchLabel.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        yield return new WaitUntil(() => _qDirections.Peek().Equals(_noMatchLabel));
        //        _noMatchLabel.Enter(() =>
        //        {
        //            isEnd = true;
        //            _qDirections.Dequeue();
        //        });

        //        yield return new WaitUntil(() => isEnd);

        //        _noMatchLabel.gameObject.SetActive(false);
        //    }
        //}
    }
}
