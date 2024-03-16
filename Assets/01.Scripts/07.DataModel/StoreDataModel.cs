using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePriceType
{
    AD,
    InApp,
    Gold,
    Dia
}

public class ProductData
{
    public eProductID id;
    public string nameKey;
    public ePriceType priceType;
    public int price;
    public int count;
}

public class StoreDataModel : ADataModel
{
    Dictionary<eProductID, ProductData> productDataList = new Dictionary<eProductID, ProductData>();

    public override void Load()
    {
        var datas = Util.CSVReadFromResourcesFolder("DataModel/StoreData");
        for (int i = 0; i < datas.Count; i++)
        {
            eProductID key = (eProductID)Enum.Parse(typeof(eProductID), (string)datas[i]["Key"]);
            string name = (string)datas[i]["Name"];
            ePriceType type = (ePriceType)(int)datas[i]["PriceType"];
            int price = (int)datas[i]["Price"];
            int count = (int)datas[i]["Count"];

            ProductData data = new ProductData();
            data.id = key;
            data.nameKey = name;
            data.priceType = type;
            data.price = price;
            data.count = count;

            productDataList.Add(key, data);
        }
    }

    public ProductData GetData(eProductID id)
    {
        return productDataList[id];
    }
}
