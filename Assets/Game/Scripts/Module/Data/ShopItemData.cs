using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Joywire/ShopItemData")]
public class ShopItemData : ScriptableObject
{
    public string id;
    public ItemData ItemData;
    public ShopPaymentOption[] options;

}

[System.Serializable]
public class ShopPaymentOption
{
    public CurrencyData Currency;
    public int Quantity;
}