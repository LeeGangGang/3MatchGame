using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddItemDirecting : MonoBehaviour
{
    [SerializeField] Image iconImg;
    [SerializeField] Image bgImg;

    [SerializeField] Color[] colors = new Color[5];

    public void Init(Sprite spr, int grade)
    {
        SetIconImage(spr);
        SetGradeImage(grade);
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
}
