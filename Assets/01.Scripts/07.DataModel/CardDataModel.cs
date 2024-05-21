using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public int Code;
    public int Option;
    public int Type;
    public int Kind;
    public int Grade;
    public int[] Value = new int[4];
}

public class CardDataModel : ADataModel
{
    MyCardCollectionDataModel myCardData = new MyCardCollectionDataModel();
    public MyCardCollectionDataModel MyCard => myCardData;

    Dictionary<int, CardData> cardDataList = new Dictionary<int, CardData>();

    public override void Load()
    {
        var datas = Util.CSVReadFromResourcesFolder("DataModel/CardData");
        for (int i = 0; i < datas.Count; i++)
        {
            int key = (int)datas[i]["Code"];

            CardData data = new CardData();
            data.Code = key;
            data.Option = (int)datas[i]["Option"];
            data.Type = (int)datas[i]["Type"];
            data.Kind = (int)datas[i]["Kind"];
            data.Grade = (int)datas[i]["Grade"];
            for (int j = 0; j < 4; j++)
                data.Value[j] = (int)datas[i][string.Format("Value{0}", j)];

            cardDataList.Add(key, data);
        }

        myCardData.Load();
    }

    public Dictionary<int, CardData> GetCardDataList()
    {
        return cardDataList;
    }

    public CardData GetCardData(int key)
    {
        return cardDataList[key];
    }

    public int GetCardSumValue(Dictionary<int, int> myCardList)
    {
        int value = 0;

        foreach (var myCard in myCardList)
            value += cardDataList[myCard.Key].Value[myCard.Value];

        return value;
    }
}
