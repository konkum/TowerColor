using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Joywire.Monetization;
using Joywire;
using DG.Tweening;
public class ShopElementUI : UIScript
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button previousScreenBtn;
    [SerializeField] private Button nextScreenBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private TextMeshProUGUI buyMoneyText;
    [SerializeField] private Button adsBtn;
    [SerializeField] private Button keyBtn;
    [SerializeField] private BallItemUI ballPrefab;
    private List<BallItemUI> _ballShopItem = new List<BallItemUI>();

    private IGameShopManager _shopManager;
    private int _screenCount = 1;
    private int _priced = 250;
    public override void Initialized()
    {
        base.Initialized();
        ThirdParties.Find<IGameShopManager>(out _shopManager);
        GameEvent.OnMoneyUpdate += OnMoneyUpdateCallBack;
        GameEvent.OnShopItemUpdate += OnShopItemUpdate;
        GameEvent.OnUpdateShop += OnUpdateShop;
        OnMoneyUpdateCallBack(playerProgression.PlayerProfile.GetCurrency("currency_00").amount);
        closeBtn.onClick.AddListener(Hide);
        previousScreenBtn.onClick.AddListener(PreviousScreen);
        nextScreenBtn.onClick.AddListener(NextScreen);
        buyBtn.gameObject.SetActive(true);
        keyBtn.onClick.AddListener(OnKeyButtonClicked);
        buyBtn.onClick.AddListener(OnBuyButtonClicked);
        adsBtn.onClick.AddListener(OnAdsButtonClicked);
        adsBtn.gameObject.SetActive(true);
        buyBtn.transform.DOScale(Vector3.one * 1.1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        adsBtn.transform.DOScale(Vector3.one * 1.1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        keyBtn.transform.DOScale(Vector3.one * 1.1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        PopulateShop();
        ShowShopItem();
        SwitchPrice();
        LoadPlayerCustom();
    }
    public override void Show()
    {
        base.Show();
    }
    public override void Hide()
    {
        base.Hide();
    }
    private void OnMoneyUpdateCallBack(int money)
    {
        moneyText.text = money.ToString();
    }
    private void NextScreen()
    {
        if (_screenCount < 5)
        {
            _screenCount++;
            if (_screenCount >= 4)
            {
                buyBtn.gameObject.SetActive(false);
                adsBtn.gameObject.SetActive(false);
                keyBtn.gameObject.SetActive(true);
            }
            if (_screenCount == 5)
            {
                keyBtn.gameObject.SetActive(false);
            }
            ShowShopItem();
            SwitchPrice();
        }
    }
    private void PreviousScreen()
    {
        if (_screenCount > 1)
        {
            _screenCount--;
            if (_screenCount == 4)
            {
                keyBtn.gameObject.SetActive(true);
            }
            if (_screenCount < 4)
            {
                keyBtn.gameObject.SetActive(false);
                adsBtn.gameObject.SetActive(true);
                buyBtn.gameObject.SetActive(true);
            }
            ShowShopItem();
            SwitchPrice();
        }
    }
    private void ShowShopItem()
    {
        for (int i = 0; i < _ballShopItem.Count; i++)
        {
            if (i < _screenCount * 9 && i >= _screenCount * 9 - 9)
            {
                _ballShopItem[i].gameObject.SetActive(true);
            }
            else
            {
                _ballShopItem[i].gameObject.SetActive(false);
            }
        }
    }
    private void SwitchPrice()
    {
        switch (_screenCount)
        {
            case 1:
                _priced = 250;
                buyMoneyText.text = _priced.ToString();
                break;
            case 2:
                _priced = 500;
                buyMoneyText.text = _priced.ToString();
                break;
            case 3:
                _priced = 1000;
                buyMoneyText.text = _priced.ToString();
                break;
            default:
                break;
        }
    }
    private void OnUpdateShop(string id)
    {
        for (int i = 0; i < _ballShopItem.Count; i++)
        {
            if (_ballShopItem[i].ShopItem.itemId == id)
            {
                _ballShopItem[i].RefreshButton(true, _ballShopItem[i].ShopItem);
                return;
            }
        }
    }
    private void OnBuyButtonClicked()
    {
        var currentMoney = playerProgression.PlayerProfile.GetCurrency("currency_00").amount;
        if (currentMoney < _priced)
        {
            return;
        }
        List<BallItemUI> ballItems = new List<BallItemUI>();
        for (int i = 0; i < _ballShopItem.Count; i++)
        {
            if (i < _screenCount * 9 && i >= _screenCount * 9 - 9 && !_ballShopItem[i].IsPurchased)
            {
                ballItems.Add(_ballShopItem[i]);
            }
        }
        if (ballItems.Count == 0)
        {
            return;
        }
        var rand = Random.Range(0, ballItems.Count);
        var summary = currentMoney - _priced;
        playerProgression.PlayerProfile.AddToInventory(ballItems[rand].ShopItem.itemId);
        ballItems[rand].RefreshButton(true, ballItems[rand].ShopItem);
        playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_00", summary);
        GameEvent.OnMoneyUpdate?.Invoke(summary);
        playerProgression.Save();
        GameEvent.ReInitializedMission?.Invoke();
    }
    private void LoadPlayerCustom()
    {
        var playerCustom = playerProgression.PlayerProfile.GetPlayerCustom("BALL_TYPE");
        var item = dataManager.GetItem(playerCustom.itemId);
        for (int i = 0; i < _ballShopItem.Count; i++)
        {
            if (_ballShopItem[i].ShopItem.itemId == item.ItemID)
            {
                _ballShopItem[i].IsSelected = true;
                resourceLoader.LoadImage(playerCustom.itemId, out var sprite);
                itemImage.sprite = sprite;
                return;
            }
        }
    }
    public void OnShopItemUpdate(string id)
    {
        for (int i = 0; i < _ballShopItem.Count; i++)
        {
            if (_ballShopItem[i].ShopItem.itemId == id)
            {
                _ballShopItem[i].RefreshButton(true, _ballShopItem[i].ShopItem, ReturnBallMode(i));
            }
        }
    }
    private void SetPlayerCustom(string id)
    {
        var item = dataManager.GetItem(id);
        var playerCustom = playerProgression.PlayerProfile.GetPlayerCustom(item.TypeID);
        if (playerCustom != null)
        {
            playerCustom.itemId = id;
            playerProgression.Save();
            resourceLoader.LoadImage(id, out var sprite);
            itemImage.sprite = sprite;
            GameEvent.OnGraphicReloadRequest?.Invoke();
        }
    }
    private void PopulateShop()
    {
        for (int i = 0; i < _shopManager.ShopItems.Length; i++)
        {
            var shopItem = _shopManager.ShopItems[i];
            var shopItemLink = dataManager.GetItem(shopItem.itemId);
            if (shopItemLink.TypeID != "BALL_TYPE")
            {
                continue;
            }
            var item = Instantiate(ballPrefab, ballPrefab.transform.parent);
            item.Initialized();
            item.gameObject.SetActive(true);
            if (playerProgression.PlayerProfile.PlayerInventory.Contains(shopItem.itemId))
            {
                item.RefreshButton(true, shopItem, ReturnBallMode(i));
            }
            else
            {
                item.RefreshButton(false, shopItem, ReturnBallMode(i));
            }
            _ballShopItem.Add(item);
            item.OnSelectItemClicked += OnShopItemSelect;
        }
    }
    private BallMode ReturnBallMode(int num)
    {
        if (num < 9)
        {
            return BallMode.BLUE;
        }
        else if (num < 18)
        {
            return BallMode.PINK;
        }
        else if (num < 27)
        {
            return BallMode.YELLOW;
        }
        else if (num < 36)
        {
            return BallMode.RED;
        }
        else if (num < 45)
        {
            return BallMode.GREEN;
        }
        return BallMode.NONE;
    }
    private void OnShopItemSelect(BallItemUI ballItemUI)
    {
        for (int i = 0; i < _ballShopItem.Count; i++)
        {
            _ballShopItem[i].IsSelected = false;
        }
        ballItemUI.IsSelected = true;
        SetPlayerCustom(ballItemUI.ShopItem.itemId);
    }
    private void OnKeyButtonClicked()
    {
        if (!admobController.IsRewardedAdLoaded)
        {
            return;
        }
        admobController.ShowRewardedAd(() =>
        {
            playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_01", 3);
            GameEvent.OnShowRewardUI?.Invoke();
        });
    }
    private void OnAdsButtonClicked()
    {
        if (!admobController.IsRewardedAdLoaded)
        {
            return;
        }
        admobController.ShowRewardedAd(() =>
        {
            var currentMoney = playerProgression.PlayerProfile.GetCurrency("currency_00").amount;
            playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_00", currentMoney + 100);
            GameEvent.OnMoneyUpdate?.Invoke(currentMoney + 100);
            playerProgression.Save();
        });
    }
    [ContextMenu("UPDATEMONEY")]
    private void HackMoney()
    {
        playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_00", 10000);
        playerProgression.Save();
    }
    [ContextMenu("HACKITEM")]
    private void HACKITEM()
    {
        for (int i = 0; i < _shopManager.ShopItems.Length; i++)
        {
            if (!playerProgression.PlayerProfile.PlayerInventory.Contains(_shopManager.ShopItems[i].itemId))
            {
                playerProgression.PlayerProfile.AddToInventory(_shopManager.ShopItems[i].itemId);
            }
        }
        playerProgression.Save();
    }
}
