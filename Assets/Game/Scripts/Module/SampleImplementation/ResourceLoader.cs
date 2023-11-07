using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joywire;
using Joywire.Core;

public class ResourceLoader : BaseResourceLoader
{
    //considering using the resources to load the asset
    //can write the own implementation upon using the another resources loader

    private string ballPath = "Assets/Balls/";
    private string imagePath = "Assets/Images/";

    private IRuntimeDataManager dataManager;

    public override void LoadImage(string id, out Sprite result)
    {
        ThirdParties.Find<IRuntimeDataManager>(out dataManager);

        result = null;
        var item = dataManager.GetItem(id);

        if (!string.IsNullOrEmpty(item.ItemPath))
        {
            var path = imagePath + item.ItemPath;
            result = Resources.Load<Sprite>(path);
        }
        else
        {
            // //could load the money
            // var moneyItem = dataManager.GetCurrency(id);
            // if(!string.IsNullOrEmpty(moneyItem.CurrencyPath))
            // {
            //     var path = imagePath + moneyItem.CurrencyPath;
            //     result = Resources.Load<Sprite>(path);
            // }
        }
    }

    public override void LoadObject(string id, out GameObject result)
    {
        ThirdParties.Find<IRuntimeDataManager>(out dataManager);

        result = null;
        string path = string.Empty;
        var item = dataManager.GetItem(id);
        if (!string.IsNullOrEmpty(item.ItemName))
        {
            path = ballPath + item.ItemName;
            result = Resources.Load<GameObject>(path);
        }
    }
    public override void LoadTexture(string id, out Texture2D texture)
    {
        ThirdParties.Find<IRuntimeDataManager>(out dataManager);
        texture = null;
        var item = dataManager.GetItem(id);

        if (!string.IsNullOrEmpty(item.ItemName))
        {
            var path = imagePath + item.ItemName;
            texture = Resources.Load<Texture2D>(path);
        }
    }
    public override void LoadImageForShop(BallMode ballMode, out Sprite result)
    {
        result = null;
        if (ballMode != BallMode.NONE)
        {
            var path = imagePath + ballMode.ToString();
            result = Resources.Load<Sprite>(path);
        }
    }

    protected override void Start()
    {
        base.Start();
    }



}
