using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Joywire/Library")]
public class ResourceLibraryData : ScriptableObject
{
    public List<ItemData> itemList;
    public List<TypeData> typeList;
    public List<CurrencyData> currencyList;


    [Header("================DEFAULT REGION=================")]
    public List<ItemData> defaultItems;

    [Header("================SHOP MENU======================")]
    public List<ShopItemData> ShopItems;

    
}
