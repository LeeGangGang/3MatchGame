using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUnitSlot : MonoBehaviour
{
    [SerializeField] Image unitIconImg;

    [SerializeField] Image[] starImgs;

    [SerializeField] Button openInfoPopupBtn;

    eUnit currUnit;

    public void Init()
    {
        openInfoPopupBtn.onClick.AddListener(() =>
        {
            UIManager.Inst.Popup.OpenUnitInfoPopup(currUnit);
        });
    }

    public void Enter(eUnit type, int level)
    {
        currUnit = type;

        SetUnitIconImage(currUnit);

        SetStarImage(level);
    }

    public void Exit()
    {

    }

    void SetUnitIconImage(eUnit type)
    {
        unitIconImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Unit, type.ToString());
    }

    void SetStarImage(int level)
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
}
