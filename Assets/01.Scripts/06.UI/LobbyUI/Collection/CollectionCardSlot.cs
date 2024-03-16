using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CollectionCardSlot : MonoBehaviour
{
    [SerializeField] Button mainBtn;

    [SerializeField] GameObject noActiveImg;

    [SerializeField] Image iconImg;
    [SerializeField] Text levelTxt;

    [SerializeField] Scrollbar expSb;
    [SerializeField] Text expTxt;

    int code;

    public void Init(int key)
    {
        code = key;

        mainBtn.onClick.AddListener(() =>
        {
            // popup open
            UIManager.Inst.Popup.OpenCardInfoPopup(code);
        });
    }

    public void Enter(int level, int currExp)
    {
        SetIconImage(code, level);
        SetLevelText(level);
        SetExp(level, currExp);

        SetOpenCard(level != -1);
    }

    public void Exit()
    {
        mainBtn.onClick.RemoveAllListeners();
    }

    void SetIconImage(int key, int level)
    {
        if (level == -1)
            level = 0;

        Sprite spr = AtlasManager.Inst.GetSprite(eAtlasType.Card, string.Format("card{0}{1}", key, level));
        iconImg.sprite = spr;
    }

    void SetLevelText(int level)
    {
        if (level != -1)
            levelTxt.text = string.Format("LV {0}", level + 1);
        else
            levelTxt.text = "UnLock";
    }

    void SetExp(int level, int currExp)
    {
        int maxExp = 0;
        if (level == -1)
            maxExp = 1;
        else if (level == 0)
            maxExp = 3;
        else if (level == 1)
            maxExp = 10;
        else
            maxExp = 20;

        if (level == -1)
        {
            expTxt.text = "UnLock";
            expSb.size = 0f;
        }
        else if (level < 3)
        {
            expTxt.text = string.Format("{0} / {1}", currExp, maxExp);
            expSb.size = (float)currExp / (float)maxExp;
        }
        else
        {
            expTxt.text = "Max";
            expSb.size = 1f;
        }
    }

    void SetOpenCard(bool nonOpen)
    {
        noActiveImg.gameObject.SetActive(!nonOpen);
    }
}
