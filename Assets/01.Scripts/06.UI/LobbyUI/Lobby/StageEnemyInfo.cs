using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEnemyInfo : MonoBehaviour
{
    [SerializeField] Image iconImg;
    [SerializeField] Text levelTxt;
    [SerializeField] Text countTxt;

    public void Enter(string name, int level, int cnt)
    {
        Sprite spr = AtlasManager.Inst.GetSprite(eAtlasType.Unit, name);
        SetEnemyImage(spr);

        SetLevelText(level);

        SetCountText(cnt);
        
        SetActive(true);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    void SetEnemyImage(Sprite spr)
    {
        iconImg.sprite = spr;
    }

    void SetLevelText(int level)
    {
        levelTxt.text = string.Format("Lv.{0}", level);
    }

    void SetCountText(int cnt)
    {
        countTxt.text = string.Format("x {0}", cnt);
    }
}
