using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPopup : APopup
{
    [SerializeField] Button closeBtn;

    [SerializeField] Text nameTxt;
    [SerializeField] Image unitIconImg;

    [SerializeField] Image backImg;
    [SerializeField] Color[] gradeColors;

    [SerializeField] Image[] starImgs = new Image[3];
    [SerializeField] Scrollbar expSb;
    [SerializeField] Text expTxt;

    [SerializeField] Text unitInfoTxt;

    [SerializeField] Text skill_0_NameTxt;
    [SerializeField] Text skill_0_InfoTxt;
    [SerializeField] Image skill_0_IconImg;
    [SerializeField] Image skill_0_BlockImg;

    [SerializeField] Text skill_1_NameTxt;
    [SerializeField] Text skill_1_InfoTxt;
    [SerializeField] Image skill_1_IconImg;
    [SerializeField] Image skill_1_BlockImg;

    [SerializeField] GameObject botPanel;
    [SerializeField] Button upgradeBtn;
    [SerializeField] Button changeBtn;

    eUnit unit;

    public override void Init()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Exit();
        });

        upgradeBtn.onClick.AddListener(() =>
        {
            OnClickUpgradeBtn();
        });

        changeBtn.onClick.AddListener(() =>
        {
            OnClickChangeBtn();
        });
    }

    public void Enter(eUnit type)
    {
        UpdateUI(type);
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    void UpdateUI(eUnit type)
    {
        unit = type;

        var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);

        var data = udm.GetUnitData(unit);
        int level = udm.MyUnit.GetMyUnitData(unit)[0];
        int exp = udm.MyUnit.GetMyUnitData(unit)[1];

        var sdm = (SkillDataModel)DataModelController.Inst.GetDataModel(eDataModel.SkillDataModel);
        for (int i = 0; i < data.Skill.Length; i++)
        {
            SkillData sd = sdm.GetData(data.Skill[i]);
            SetSkillInfo(sd, i);
        }

        SetUnitInfo(data, level);
        SetUnitLevel(level, exp);
        
        // 선택한 유닛일 경우에만
        changeBtn.gameObject.SetActive(udm.MyUnit.GetMySelectUnitDataList().Keys.ToList().Contains(type));

        int grade = udm.GetUnitData(unit).Grade;
        SetGradeImg(grade);
    }

    void SetUnitInfo(UnitData data, int level)
    {
        unitIconImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Unit, data.Name);
        nameTxt.text = data.Name;
        unitInfoTxt.text = string.Format("HP : {0}\r\nDEFENCE : {1}\r\nCRITICAL : {2}%",
            data.Hp[level], data.Defence[level], data.Critical_Per[level]);
    }

    void SetUnitLevel(int level, int exp)
    {
        int maxExp = 0;
        if (level == 0)
        {
            starImgs[0].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");
            starImgs[1].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");
            starImgs[2].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");

            maxExp = 5;
        }
        else if (level == 1)
        {
            starImgs[0].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[1].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");
            starImgs[2].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");

            maxExp = 10;
        }
        else if (level == 2)
        {
            starImgs[0].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[1].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[2].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_0");

            maxExp = 20;
        }
        else
        {
            starImgs[0].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[1].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
            starImgs[2].sprite = AtlasManager.Inst.GetSprite(eAtlasType.UI, "Star_1");
        }

        if (maxExp != 0)
        {
            expTxt.text = string.Format("{0} / {1}", exp, maxExp);
            expSb.size = (float)exp / (float)maxExp;
        }
        else
        {
            expTxt.text = string.Format("MAX");
            expSb.size = 1f;
        }

        if (level <= 2 && exp >= maxExp)
        {
            upgradeBtn.gameObject.SetActive(true);
        }
        else
        {
            upgradeBtn.gameObject.SetActive(false);
        }
    }

    void SetSkillInfo(SkillData data, int idx)
    {
        if (idx == 0)
        {
            skill_0_NameTxt.text = data.Name;
            skill_0_InfoTxt.text = string.Format("Stack : {0}\r\nValue : {1}", data.Stack, data.Value);
            skill_0_IconImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Skill, data.Name);
            skill_0_BlockImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, string.Format("item_{0:00}", (int)data.Color));
        }
        else
        {
            skill_1_NameTxt.text = data.Name;
            skill_1_InfoTxt.text = string.Format("Stack : {0}\r\nValue : {1}", data.Stack, data.Value);
            skill_1_IconImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Skill, data.Name);
            skill_1_BlockImg.sprite = AtlasManager.Inst.GetSprite(eAtlasType.Block, string.Format("item_{0:00}", (int)data.Color));
        }
    }

    void SetGradeImg(int grade)
    {
        backImg.color = gradeColors[grade];
    }

    void OnClickUpgradeBtn()
    {
        var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
        udm.MyUnit.SetUpgradeUnit(unit, (isSuccess) =>
        {
            if (isSuccess)
            {
                UpdateUI(unit);
                UIManager.Inst.SetUpdateLobbyUI();
            }
            else
            {
                return;
            }
        });
    }

    void OnClickChangeBtn()
    {
        UIManager.Inst.Popup.OpenUnitChangePopup(unit, (changeUnit) =>
        {
            unit = changeUnit;
            UpdateUI(changeUnit);
            UIManager.Inst.SetUpdateLobbyUI();
        });
    }
}
