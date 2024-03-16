using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCulture
{
    KR,
    EN
}

public enum eLocalizationKey
{
    shop_Gold200,
    shop_Gold1000,
    shop_Gold2000,
    shop_Gold5000,
    shop_Gold10000,
    shop_Dia10,
    shop_Dia100,
    shop_Dia600,
    shop_Dia1500,
    shop_Dia5500,
    shop_NormalCardPack,
    shop_MagicCardPack,
    shop_RareCardPack,
    shop_UniqCardPack,
    shop_NormalCardPack_x10,
    shop_MagicCardPack_x10,
    shop_RareCardPack_x10,
    shop_UniqCardPack_x10,
    shop_NormalUnitPack,
    shop_MagicUnitPack,
    shop_RareUnitPack,
    shop_UniqUnitPack,
    shop_NormalUnitPack_x10,
    shop_MagicUnitPack_x10,
    shop_RareUnitPack_x10,
    shop_UniqUnitPack_x10,
    hero,
    card,
    lobby,
    collection,
    shop,
}

public class LocalizationDataModel : ADataModel
{
    Dictionary<eCulture, Dictionary<eLocalizationKey, string>> localizationStringList = new Dictionary<eCulture, Dictionary<eLocalizationKey, string>>();

    public override void Load()
    {
        localizationStringList.Add(eCulture.KR, new Dictionary<eLocalizationKey, string>());
        localizationStringList.Add(eCulture.EN, new Dictionary<eLocalizationKey, string>());

        var datas = Util.CSVReadFromResourcesFolder("DataModel/LocalizationData");
        for (int i = 0; i < datas.Count; i++)
        {
            eLocalizationKey key = (eLocalizationKey)Enum.Parse(typeof(eLocalizationKey), (string)datas[i]["Key"]);
            localizationStringList[eCulture.KR].Add(key, (string)datas[i]["KR"]);
            localizationStringList[eCulture.EN].Add(key, (string)datas[i]["EN"]);
        }
    }

    public string GetString(eLocalizationKey key)
    {
        return localizationStringList[eCulture.KR][key];
    }

    public string GetString(string str)
    {
        eLocalizationKey key = (eLocalizationKey)Enum.Parse(typeof(eLocalizationKey), str);
        return localizationStringList[eCulture.KR][key];
    }
}
