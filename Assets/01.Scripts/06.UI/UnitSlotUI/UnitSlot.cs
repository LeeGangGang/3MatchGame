using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour
{
    [SerializeField] Image _unitImg;
    [SerializeField] Image[] _gaugeImgs;

    public void Enter(MyUnit unit)
    {
        _unitImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Unit, unit.Name);

        var sdm = (SkillDataModel)DataModelController.Inst.GetDataModel(eDataModel.SkillDataModel);
        for (int idx = 0; idx < unit.skillList.Count; idx++)
        {
            var skillKey = unit.skillList[idx].key;
            eColor color = sdm.GetData(skillKey).Color;
            _gaugeImgs[idx].fillAmount = 0f;
            _gaugeImgs[idx].color = GetColorCode(color);
        }
    }

    public void Exit()
    {

    }

    public void SetGaugeFillAmount(int idx, float amount)
    {
        _gaugeImgs[idx].fillAmount = amount;
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
