using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AttackEventPrefab : MonoBehaviour
{
    [SerializeField] Image backImg;
    [SerializeField] Image iconImg;
    [SerializeField] Image skillImg;

    [SerializeField] Text idxTxt;

    public void Enter(AttackEvent data)
    {
        SetIconImg(data.MyName);
        SetSkillImg(data.SkillName);

        gameObject.SetActive(true);

        Sequence animSeq = DOTween.Sequence();
        animSeq
            .Append(backImg.transform.DOLocalMove(Vector2.zero, 0.25f).SetEase(Ease.OutCirc))
            .Join(backImg.transform.DOScale(new Vector3(0.8f, 1.2f, 1f), 0.25f))
            .Insert(0.25f, backImg.transform.DOScale(Vector3.one, 0.25f));

        idxTxt.text = data.Index.ToString();
    }

    public void Exit(Action onComplete)
    {
        Sequence animSeq = DOTween.Sequence();
        animSeq
            .Append(backImg.transform.DOLocalMoveY(10f, 0.25f).SetEase(Ease.OutCirc))
            .Join(backImg.transform.DOScale(new Vector3(0.8f, 1.2f, 1f), 0.25f))
            .Append(backImg.transform.DOLocalMoveY(-140f, 0.25f).SetEase(Ease.OutCirc))
            .Insert(0.3f, backImg.transform.DOScale(Vector3.one, 0.15f));

        animSeq.AppendCallback(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public bool isActive()
    {
        return gameObject.activeSelf;
    }

    void SetBackImage(int type)
    {
        
    }

    void SetIconImg(string name)
    {
        iconImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Unit, name);
    }

    void SetSkillImg(string skillName)
    {
        skillImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Skill, skillName);
    }
}
