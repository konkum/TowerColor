using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joywire;
using Joywire.Core;
using Joywire.Monetization;

public class ShopManager : MonoBehaviour, IGameShopManager
{

    public ResourceLibraryData Library;

    [SerializeField]private List<ShopItem> ShopItemList;

    private List<ShopItem> SortedItemList;

    private IRuntimeDataManager dataManager;

    public ShopItem[] ShopItems => ShopItemList.ToArray();

    private void Awake()
    {
        ThirdParties.Register<IGameShopManager>(this);
        ThirdParties.Find<IRuntimeDataManager>(out dataManager);
        Initialize();
    }

    private void Initialize()
    {
        ShopItemList = new List<ShopItem>();
        SortedItemList = new List<ShopItem>();

        for(int i = 0; i < Library.ShopItems.Count; i++)
        {
            var item = Library.ShopItems[i];
            ShopItem shopItem = new ShopItem(item);
            ShopItemList.Add(shopItem);
            
        }
    }

    public ShopItem GetShopItem(string shopId)
    {
        for(int i = 0; i < ShopItemList.Count; i++)
        {
            if(ShopItemList[i].shopItemId == shopId)
            {
                return ShopItemList[i];
            }
        }
        return null;
    }

    public ShopItem[] GetShopItems(string filterId)
    {
        SortedItemList.Clear();
        for(int i = 0; i < ShopItemList.Count; i++)
        {
            var item = dataManager.GetItem(ShopItemList[i].itemId);
            if(item.TypeID == filterId)
            {
                SortedItemList.Add(ShopItemList[i]);
            }
        }

        return SortedItemList.ToArray();
    }

  
}
