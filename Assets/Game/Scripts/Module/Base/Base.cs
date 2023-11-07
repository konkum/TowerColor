using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Joywire.Core
{
    [Serializable]
    public class Item
    {
        public string ItemID;
        public string TypeID;
        public string ItemName;
        public string ItemPath;

        public Item(ItemData data)
        {
            this.TypeID = data.Type.TypeID;
            this.ItemID = data.ItemID;
            this.ItemName = data.ItemName;
            this.ItemPath = data.ItemImgPath;
        }
    }

    [Serializable]
    public class Currency
    {
        public string CurrencyID;
        public string CurrencyName;
        public string CurrencyPath;

        public Currency(CurrencyData data)
        {
            this.CurrencyID = data.CurrencyID;
            this.CurrencyName = data.CurrencyName;
            this.CurrencyPath = data.CurrencyImgPath;
        }
    }

    [Serializable]
    public class ItemType
    {
        public string TypeId;
        public string TypeName;

        public ItemType(TypeData data)
        {
            this.TypeId = data.TypeID;
            this.TypeName = data.TypeName;
        }
    }

    [Serializable]
    public class PlayerCustom
    {
        public string playerCustomId;
        public string itemId;
    }

    public interface IRuntimeDataManager
    {
        void Init();
        Item GetItem(string id);
        Currency GetCurrency(string id);
        Currency[] GetAllCurrency();
        ItemType GetItemType(string id);
        Item[] GetItemByType(string typeId);
        Item[] GetDefaultItems();

    }

    public interface IResourceLoader
    {
        void LoadObject(string id, out GameObject result);
        void LoadImage(string id, out Sprite result);
        void LoadTexture(string id, out Texture2D texture);
        void LoadImageForShop(BallMode ballMode, out Sprite result);
    }


    [System.Serializable]
    public class PlayerProfile
    {

        //this player profile is use for multiple level games.
        [SerializeField] private string playerName;
        [SerializeField] private List<string> playerInventory;
        [SerializeField] private PlayerCurrency[] playerCurrencies;
        [SerializeField] private ProgressTracker playerProgress;
        public PlayerCustom[] PlayerCustoms;

        public List<PlayerPaymentProgress> paymentProgressList;

        public ProgressTracker PlayerProgress => playerProgress;


        public PlayerProfile(IRuntimeDataManager dataManager)
        {
            //create base player
            this.playerName = "New Player";

            playerInventory = new List<string>();

            var defaultItems = dataManager.GetDefaultItems();
            for (int i = 0; i < defaultItems.Length; i++)
            {
                playerInventory.Add(defaultItems[i].ItemID);
            }

            //init currency
            var currencies = dataManager.GetAllCurrency();
            //Debug.LogError(currencies.Length);
            playerCurrencies = new PlayerCurrency[currencies.Length];
            var index = 0;
            foreach (var currency in currencies)
            {
                playerCurrencies[index] = new PlayerCurrency();
                playerCurrencies[index].id = currency.CurrencyID;
                playerCurrencies[index].amount = 0;
                index++;
            }

            //init progress tracker
            this.playerProgress = new ProgressTracker(dataManager);
            this.PlayerProgress.Initialize();

            paymentProgressList = new List<PlayerPaymentProgress>();

            //Debug.LogError("goes here");
        }

        public PlayerProfile(string playerName, List<String> playerInventory, PlayerCurrency[] currencyProgress, ProgressTracker progress)
        {
            this.playerName = playerName;
            this.playerInventory = playerInventory;
            this.playerCurrencies = currencyProgress;
            this.playerProgress = progress;
        }


        public string PlayerName => playerName;
        public List<string> PlayerInventory => playerInventory;
        public PlayerCurrency[] PlayerCurrencies => playerCurrencies;


        public void ChangePlayerName(string name)
        {
            this.playerName = name;
        }

        public void UpdatePlayerCurrency(string currencyId, int amount)
        {
            foreach (var playerCurrency in playerCurrencies)
            {
                if (playerCurrency.id == currencyId)
                {
                    playerCurrency.amount = amount;
                    break;
                }
            }
        }

        public void AddToInventory(string itemId)
        {
            if (!playerInventory.Contains(itemId))
            {
                playerInventory.Add(itemId);
            }
        }

        public void RemoveInventoryItem(string itemId)
        {
            if (!playerInventory.Contains(itemId))
                playerInventory.Remove(itemId);
        }

        public PlayerCurrency GetCurrency(string id)
        {
            for (int i = 0; i < playerCurrencies.Length; i++)
            {
                if (playerCurrencies[i].id == id)
                {
                    return playerCurrencies[i];
                }
            }
            return null;
        }

        public PlayerCustom GetPlayerCustom(string customId)
        {
            for (int i = 0; i < PlayerCustoms.Length; i++)
            {
                if (PlayerCustoms[i].playerCustomId == customId)
                {
                    return PlayerCustoms[i];
                }
            }
            return null;
        }

    }

    [System.Serializable]
    public class PlayerCurrency
    {
        public string id;
        public int amount;
    }

    [System.Serializable]
    public class LevelTracker
    {
        public string id;
        public int rating;
        public bool isUnlock;
        public bool isWin;

        public System.Action OnCompleteLevel;

        public void UpdateTrackerProgress(int rating)
        {
            this.rating = rating;
        }

        public void CompleteLevel()
        {
            this.isWin = true;
            OnCompleteLevel?.Invoke();
        }


    }

    [System.Serializable]
    public class ProgressTracker
    {
        public string CurrentID;
        public LevelTracker[] Trackers;

        public ProgressTracker(IRuntimeDataManager dataManager)
        {
            var levels = dataManager.GetItemByType("LEVEL_TYPE");
            var defaultItems = dataManager.GetDefaultItems();
            var defaultLevels = new List<Item>();
            foreach (var defaultItem in defaultItems)
            {
                if (defaultItem.TypeID == "LEVEL_TYPE")
                {
                    defaultLevels.Add(defaultItem);
                }
            }

            Trackers = new LevelTracker[levels.Length];
            for (int i = 0; i < levels.Length; i++)
            {
                Trackers[i] = new LevelTracker();
                Trackers[i].id = levels[i].ItemID;
                Trackers[i].rating = 0;
                if (defaultLevels.Contains(levels[i]))
                {
                    Trackers[i].isUnlock = true;
                }
                else
                {
                    Trackers[i].isUnlock = false;
                }

                Trackers[i].isWin = false;

            }



            CurrentID = Trackers[0].id;
        }


        public void Initialize()
        {
            for (int i = 0; i < Trackers.Length; i++)
            {
                Trackers[i].OnCompleteLevel = UpdateProgress;

            }
        }


        private void UpdateProgress()
        {
            var index = -1;
            for (int i = 0; i < Trackers.Length; i++)
            {
                if (Trackers[i].id == CurrentID && Trackers[i].isWin)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1 && (index + 1) < Trackers.Length)
            {
                Trackers[index + 1].isUnlock = true;
                SetCurrentLevel(Trackers[index + 1].id);
            }

        }

        public void SetCurrentLevel(string id)
        {
            this.CurrentID = id;
        }

        public void CompleteCurrentLevel()
        {
            for (int i = 0; i < Trackers.Length; i++)
            {
                if (Trackers[i].id == CurrentID)
                {
                    Trackers[i].CompleteLevel();
                    break;
                }
            }
        }


    }
    public interface IPlayerProgression
    {
        void Initialize();

        void Load(string data);

        string SaveData { get; }

        void Save();

        PlayerProfile PlayerProfile { get; }

        void RequestPaymentProgress(string shopItemID, out PlayerPaymentProgress progress);
    }

    [System.Serializable]
    public class PlayerPaymentProgress
    {
        public string ShopItem;
        [SerializeField] private int progress;
        [SerializeField] private int goal;

        public int Progress => progress;
        public int Goal => goal;

        public PlayerPaymentProgress(string shopItemId, int progress, int goal)
        {
            this.ShopItem = shopItemId;
            this.progress = progress;
            this.goal = goal;
        }

        public void UpdateProgress(Action<bool> OnComplete)
        {
            this.progress++;
            if (this.progress == this.goal)
            {
                OnComplete?.Invoke(true);
                return;
            }
            OnComplete?.Invoke(false);


        }
    }
}


namespace Joywire.Monetization
{
    [System.Serializable]
    public class ShopItem
    {
        public string shopItemId;
        public string itemId;
        public PaymentOption[] payments;

        public ShopItem(ShopItemData data)
        {
            this.shopItemId = data.id;
            this.itemId = data.ItemData.ItemID;
            this.payments = new PaymentOption[data.options.Length];
            for (int i = 0; i < data.options.Length; i++)
            {
                this.payments[i] = new PaymentOption(data.options[i]);
            }
        }
    }

    public class PaymentOption
    {
        public string currencyID;
        public int amount;

        public PaymentOption(ShopPaymentOption option)
        {
            this.currencyID = option.Currency.CurrencyID;
            this.amount = option.Quantity;
        }
    }

    public interface IGameShopManager
    {
        ShopItem[] ShopItems { get; }
        ShopItem[] GetShopItems(string filterId);
        ShopItem GetShopItem(string shopId);

    }



}


namespace Joywire.UI
{
    public interface IPopupManager
    {

        void Initialize();
        void ShowPopupWindow(PopupType type, string message = "", Action OnAction = null);

    }

    public enum PopupType
    {
        SIMPLE_MESSAGE,
        CONFIRMATION,
        SIMPLE_LOADING,
        LOADING_PROGRESS
    }

    public interface IPopupWindow
    {
        PopupType Type { get; }

        void Show(string message = "", Action OnAction = null);

        void Hide();
    }
}