using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddItemDirecting : MonoBehaviour
{
    [SerializeField] Image iconImg;
    [SerializeField] Image bgImg;
    [SerializeField] Text cntTxt;

    [SerializeField] Color[] colors = new Color[5];

    // valueType : Card, Unit Grade = 0 / Gold, Dia Count = 1
    public void Init(eProductType type, Sprite spr, params int[] values)
    {
        SetIconImage(spr);
        if (type == eProductType.Card || type == eProductType.Unit)
        {
            SetCountText(values[0]);
            SetGradeImage(values[1]);
        }
        else
        {
            SetCountText(values[0]);
            SetGradeImage(5);
        }
    }

    public void Enter()
    {
        gameObject.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }

    void SetIconImage(Sprite spr)
    {
        iconImg.sprite = spr;
    }

    void SetGradeImage(int grade)
    {
        bgImg.color = colors[grade];
    }

    void SetCountText(int count)
    {
        cntTxt.gameObject.SetActive(true);
        cntTxt.text = string.Format("{0:N0}", count);
    }
}
