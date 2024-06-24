using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour
{
    [SerializeField] Image unitImg;
    [SerializeField] Image[] gaugeImgs;
    [SerializeField] Image unitDieImg;

    public void Enter(MyUnit unit)
    {
        SetActiveDieImage(false);

        unitImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Unit, unit.Name);

        var sdm = (SkillDataModel)DataModelController.Inst.GetDataModel(eDataModel.SkillDataModel);
        int idx = 0;
        foreach (var skillData in unit.skillList.Keys)
        {
            var skillKey = skillData.key;
            eColor color = sdm.GetData(skillKey).Color;
            gaugeImgs[idx].fillAmount = 0f;
            gaugeImgs[idx].color = GetColorCode(color);
            idx++;
        }
    }

    public void Exit()
    {

    }

    public void SetGaugeFillAmount(int idx, float amount)
    {
        gaugeImgs[idx].fillAmount = amount;
    }

    public void SetActiveDieImage(bool isDie)
    {
        unitDieImg.gameObject.SetActive(isDie);
    }

    Color GetColorCode(eColor color)
    {
        if (color == eColor.Red)
            return Color.red;
        else if (color == eColor.Yellow)
            return Color.yellow;
        else if (color == eColor.Green)
            return Color.green;
        else if (color == eColor.White)
            return Color.gray;
        else if (color == eColor.Blue)
            return Color.blue;
        else if (color == eColor.Purple)
            return Color.black;
        else
            return Color.grey;
    }
}
