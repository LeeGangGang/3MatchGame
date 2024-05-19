using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FadePanel : MonoBehaviour
{
    [SerializeField] Image fadeImg;

    public void Enter(Action onComplete)
    {
        gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(fadeImg.DOFade(1f, 0.5f))
            .AppendCallback(() =>
            {
                onComplete?.Invoke();
                fadeImg.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
    }
}
