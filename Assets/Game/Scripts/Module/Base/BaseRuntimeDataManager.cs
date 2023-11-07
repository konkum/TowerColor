using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joywire;
using Joywire.Core;

public abstract class BaseRuntimeDataManager : MonoBehaviour, IRuntimeDataManager
{

    public ResourceLibraryData Library;

    [SerializeField] protected List<Item> ItemList;
    [SerializeField] protected List<ItemType> ItemTypeList;
    [SerializeField] protected List<Currency> CurrencyList;


    private List<Item> SortedList; 

    private void Start()
    {
        ThirdParties.Register<IRuntimeDataManager>(this);
        DontDestroyOnLoad(gameObject);
        this.Init();
    }

    public Currency GetCurrency(string id)
    {
        for(int i = 0; i < CurrencyList.Count; i++)
        {
            if(CurrencyList[i].CurrencyID == id)
            {
                return CurrencyList[i];
            }
        }
        Debug.Log("Currency is not defined");
        return null;
    }

    public Item GetItem(string id)
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            if(ItemList[i].ItemID == id)
            {
                return ItemList[i];
            }
        }
        Debug.Log("Item is not defined");
        return null;
        
    }

    public Item[] GetItemByType(string typeId)
    {
        SortedList.Clear();
        for (int i = 0; i < ItemList.Count; i++)
        {
            if(ItemList[i].TypeID == typeId)
            {
                SortedList.Add(ItemList[i]);
            }
        }
        return SortedList.ToArray();
    }

    public ItemType GetItemType(string id)
    {
        for (int i = 0; i < ItemTypeList.Count; i++)
        {
            if(ItemTypeList[i].TypeId == id)
            {
                return ItemTypeList[i];
            }
        }

        Debug.Log("Item type is not defined");
        return null;
    }

    public virtual void Init()
    {
        SortedList = new List<Item>();

        //init item data 
        foreach(var data in Library.itemList)
        {
            Item item = new Item(data);
            ItemList.Add(item);
        }

        // init type data
        foreach(var data in Library.typeList)
        {
            ItemType type = new ItemType(data);
            ItemTypeList.Add(type);
        }

        //init currency data 
        foreach(var data in Library.currencyList)
        {
            Currency currency = new Currency(data);
            CurrencyList.Add(currency);
        }
    }

    public Currency[] GetAllCurrency()
    {
        return CurrencyList.ToArray();
    }

    public Item[] GetDefaultItems()
    {
        SortedList = new List<Item>();
        for(int i = 0; i < Library.defaultItems.Count; i++)
        {
            var item = GetItem(Library.defaultItems[i].ItemID);
            if(item!= null)
                SortedList.Add(item);
        }

        return SortedList.ToArray();
    }
}
