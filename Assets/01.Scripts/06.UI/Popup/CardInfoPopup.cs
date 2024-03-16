using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoPopup : APopup
{
    [SerializeField] Button closeBtn;
    [SerializeField] Button upgradeBtn;

    [SerializeField] Text nameTxt;
    [SerializeField] Image cardIconImg;
    [SerializeField] Text levelTxt;

    [SerializeField] Text cardInfoTxt;
    [SerializeField] GameObject nextArrowImg;
    [SerializeField] Text cardNextInfoTxt;

    [SerializeField] Scrollbar expSb;
    [SerializeField] Text expTxt;

    [SerializeField] GameObject bot;

    int cardCode;

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
    }

    public void Enter(int code)
    {
        cardCode = code;

        UpdateUI(cardCode);

        base.Enter();
    }

    void UpdateUI(int code)
    {
        var mcdm = (MyCardCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyCardCollectionDataModel);

        int level;
        int exp;
        int[] info = mcdm.GetMyCardCollectionData(code);
        if (info == null)
        {
            level = -1;
            exp = 0;
        }
        else
        {
            level = info[0];
            exp = info[1];
        }

        SetIconImage(level);
        SetCardInfo(level);
        SetCardLevel(level, exp);
    }

    public override void Exit()
    {
        base.Exit();
    }

    void OnClickUpgradeBtn()
    {
        var mcdm = (MyCardCollectionDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyCardCollectionDataModel);
        mcdm.SetUpgradeCard(cardCode, (isSuccess) =>
        {
            if (isSuccess)
            {
                UpdateUI(cardCode);
                UIManager.Inst.SetUpdateLobbyUI();
            }
            else
            {
                return;
            }
        });
    }

    void SetIconImage(int level)
    {
        if (level == -1)
            level = 0;

        Sprite spr = AtlasManager.Inst.GetSprite(eAtlasType.Card, string.Format("card{0}{1}", cardCode, level));
        cardIconImg.sprite = spr;
    }

    void SetCardInfo(int level)
    {
        var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);
        CardData data = cdm.GetCardData(cardCode);

        string infoStr = "";

        if (level == -1)
        {
            cardInfoTxt.text = string.Format("{0} : {1}%", infoStr, data.Value[level + 1]);

            cardNextInfoTxt.gameObject.SetActive(false);
            nextArrowImg.SetActive(false);
        }
        else if (level == 0)
        {
            cardInfoTxt.text = string.Format("{0} : {1}%", infoStr, data.Value[level]);
            cardNextInfoTxt.text = string.Format("{0} : {1}%", infoStr, data.Value[level + 1]);

            cardNextInfoTxt.gameObject.SetActive(true);
            nextArrowImg.SetActive(true);
        }
        else if (level == 1)
        {
            cardInfoTxt.text = string.Format("{0} : {1}%", infoStr, data.Value[level]);
            cardNextInfoTxt.text = string.Format("{0} : {1}%", infoStr, data.Value[level + 1]);

            cardNextInfoTxt.gameObject.SetActive(true);
            nextArrowImg.SetActive(true);
        }
        else if (level == 2)
        {
            cardInfoTxt.text = string.Format("{0} : {1}%", infoStr, data.Value[level]);
            cardNextInfoTxt.text = string.Format("{0} : {1}%", infoStr, data.Value[level + 1]);

            cardNextInfoTxt.gameObject.SetActive(true);
            nextArrowImg.SetActive(true);
        }
        else
        {
            cardInfoTxt.text = string.Format("{0} : {1}%", infoStr, data.Value[level]);

            cardNextInfoTxt.gameObject.SetActive(false);
            nextArrowImg.SetActive(false);
        }
    }

    void SetCardLevel(int level, int currExp)
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
            levelTxt.text = "UnLock";
            expTxt.text = "UnLock";
            expSb.size = 0f;

            bot.SetActive(false);
        }
        else if (level < 3)
        {
            levelTxt.text = string.Format("LV {0}", level + 1);
            expTxt.text = string.Format("{0} / {1}", currExp, maxExp);
            expSb.size = (float)currExp / (float)maxExp;
            
            if (currExp >= maxExp)
                bot.SetActive(true);
            else
                bot.SetActive(false);
        }
        else
        {
            levelTxt.text = string.Format("LV {0}", level + 1);
            expTxt.text = "Max";
            expSb.size = 1f;

            bot.SetActive(false);
        }
    }
}
