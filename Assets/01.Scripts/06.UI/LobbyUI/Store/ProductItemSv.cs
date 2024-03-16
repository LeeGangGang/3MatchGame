using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductItemSv : MonoBehaviour
{
    [SerializeField] public eProductType type;
    [SerializeField] List<ProductItem> itemList = new List<ProductItem>();

    public void Init(Action<eProductID> buyEvent)
    {
        foreach (var item in itemList)
            item.Init(buyEvent);
    }

    public void Enter()
    {
        gameObject.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }
}
