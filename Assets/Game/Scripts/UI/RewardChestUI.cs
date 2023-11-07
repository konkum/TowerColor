using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Joywire.Core;
using DG.Tweening;
public class RewardChestUI : UIScript
{
    [SerializeField] private Button chestBtn;
    [SerializeField] private Image cheshImage;
    [SerializeField] private GameObject itemHolder;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI moneyText;

    private bool _isClicked = false;
    private bool _isObject;
    private Item _item;
    public bool IsClicked
    {
        get => _isClicked;
        set => _isClicked = value;
    }
    private int _rewardMoney = 0;
    public int RewardMoney
    {
        get => _rewardMoney;
        set => _rewardMoney = value;
    }
    public bool IsObject
    {
        get => _isObject;
        set
        {
            _isObject = value;
            itemImage.gameObject.SetActive(value);
            moneyText.transform.parent.gameObject.SetActive(!value);
        }
    }
    public Item Item
    {
        get => _item;
        set => _item = value;
    }
    private RewardUI _rewardUI;
    public void Initialized(RewardUI rewardUI)
    {
        base.Initialized();
        _rewardUI = rewardUI;
        if (_item != null)
        {
            resourceLoader.LoadImage(_item.ItemID, out var sprite);
            itemImage.sprite = sprite;
        }
        else
        {
            moneyText.text = _rewardMoney.ToString();
        }
        chestBtn.onClick.AddListener(OnCheshButtonClicked);
    }
    public override void Show()
    {
        base.Show();
    }
    public override void Hide()
    {
        base.Hide();
    }
    private void OnCheshButtonClicked()
    {
        if (_rewardUI.CurrentKeys == 0)
        {
            return;
        }
        if (!_isClicked)
        {
            _isClicked = true;
            _rewardUI.UpdateCurrentKeys();
            cheshImage.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutSine);
            itemHolder.gameObject.SetActive(true);
            itemHolder.transform.localScale = Vector3.zero;
            itemHolder.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutSine);
            if (_isObject)
            {
                playerProgression.PlayerProfile.AddToInventory(_item.ItemID);
                GameEvent.OnShopItemUpdate?.Invoke(_item.ItemID);
            }
            else
            {
                var currentMoney = playerProgression.PlayerProfile.GetCurrency("currency_00").amount;
                var summary = currentMoney + _rewardMoney;
                GameEvent.OnMoneyUpdate?.Invoke(summary);
                playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_00", summary);
            }
        }
    }
}
