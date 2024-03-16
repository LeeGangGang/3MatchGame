using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public enum eGameTextType
{
    Damage, // RPG쪽 데미지 텍스트
    Score,  // Match3 스코어 텍스트
}

public class GameText : MonoBehaviour
{
    [SerializeField] TextMeshPro _gameTxt;
    bool _showing = false;
    public bool Showing => _showing;

    public void ScoreEnter(Vector3 pos, string str)
    {
        _showing = true;

        _gameTxt.color = new Color(1f, 1f, 1f, 0.8f);
        _gameTxt.text = str;
        gameObject.transform.localPosition = pos;
        gameObject.SetActive(true);
        ScoreTextAnim();
    }

    public void DamgeEnter(Vector3 pos, string str, bool isCri)
    {
        _showing = true;

        _gameTxt.color = new Color(1f, 1f, 1f, 0.8f);
        _gameTxt.text = str;
        _gameTxt.fontSize = 2f;

        if (isCri)
        {
            _gameTxt.fontSize *= 1.25f;
            _gameTxt.color = Color.yellow;
        }

        gameObject.transform.localPosition = pos;
        gameObject.SetActive(true);
        DamgeTextAnim();
    }

    public void Exit()
    {
        _gameTxt.gameObject.SetActive(false);
        _showing = false;
    }

    void DamgeTextAnim()
    {
        Sequence textSeq = DOTween.Sequence();

        float y = _gameTxt.transform.localPosition.y + 1f;
        textSeq
            .Append(_gameTxt.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f))
            .Join(_gameTxt.transform.DOLocalMoveY(y, 1f).SetEase(Ease.OutCirc))
            .Join(_gameTxt.DOFade(0.4f, 1f))
            .Insert(0.2f, _gameTxt.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.8f));

        textSeq.AppendCallback(() =>
        {
            Exit();
        });
    }

    void ScoreTextAnim()
    {
        Sequence textSeq = DOTween.Sequence();

        float y = _gameTxt.transform.localPosition.y + 1f;
        textSeq
            .Append(_gameTxt.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f))
            .Join(_gameTxt.transform.DOLocalMoveY(y, 1f).SetEase(Ease.OutCirc))
            .Join(_gameTxt.DOFade(0f, 1f))
            .Insert(0.2f, _gameTxt.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.8f));

        textSeq.AppendCallback(() =>
        {
            Exit();
        });
    }
}
