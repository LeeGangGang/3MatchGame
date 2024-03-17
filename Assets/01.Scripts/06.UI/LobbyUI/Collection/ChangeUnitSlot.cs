using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeUnitSlot : MonoBehaviour
{
    [SerializeField] Image backImg;
    [SerializeField] Color[] gradeColors;

    [SerializeField] Image unitIconImg;

    [SerializeField] Image[] starImgs;

    [SerializeField] Toggle mainTg;

    eUnit currUnit;

    public void Init(eUnit type, ToggleGroup tgGp, Action onChange)
    {
        currUnit = type;
        mainTg.group = tgGp;
        mainTg.onValueChanged.AddListener((isOn) => 
        {
            if (isOn)
            {
                onChange?.Invoke();
            }
        });
    }

    public void Enter(eUnit type, int grade, int level)
    {
        currUnit = type;

        SetUnitIcon(currUnit);

        SetStarImg(level);

        SetGradeImg(grade);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }

    public void SetSelect(bool isSelect)
    {
        mainTg.SetIsOnWithoutNotify(isSelect);
    }

    void SetUnitIcon(eUnit type)
    {
        unitIconImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Unit, type.ToString());
    }

    void SetStarImg(int level)
    {
        if (level == 0)
        {
            starImgs[0].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");
            starImgs[1].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");
            starImgs[2].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");
        }
        else if (level == 1)
        {
            starImgs[0].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[1].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");
            starImgs[2].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");
        }
        else if (level == 2)
        {
            starImgs[0].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[1].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[2].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");
        }
        else
        {
            starImgs[0].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[1].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[2].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
        }
    }

    void SetGradeImg(int grade)
    {
        backImg.color = gradeColors[grade];
    }
}
