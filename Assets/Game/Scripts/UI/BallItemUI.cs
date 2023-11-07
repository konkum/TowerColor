using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Joywire.Monetization;
using TMPro;
public class BallItemUI : UIScript
{
    [SerializeField] private Button itemBtn;
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Image ballImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image checkMark;
    [SerializeField] private GameObject adsImage;
    [SerializeField] private TextMeshProUGUI adsCountText;

    private BallMode _ballMode = BallMode.NONE;
    private bool _isSelected;
    private bool _isPurchased;
    private ShopItem _shopItem;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            checkMark.gameObject.SetActive(value);
        }
    }
    public bool IsPurchased
    {
        get => _isPurchased;
        set => _isPurchased = value;
    }
    public ShopItem ShopItem => _shopItem;
    public System.Action<BallItemUI> OnSelectItemClicked;
    public override void Initialized()
    {
        base.Initialized();
        itemBtn.onClick.AddListener(OnButtonClicked);
    }
    public override void Show()
    {
        base.Show();
    }
    public override void Hide()
    {
        base.Hide();
    }
    public void RefreshButton(bool IsPurchased, ShopItem shopItem, BallMode ballMode = BallMode.NONE)
    {
        _shopItem = shopItem;
        _isPurchased = IsPurchased;
        if (_ballMode == BallMode.NONE)
        {
            _ballMode = ballMode;
            if (ballMode != BallMode.NONE)
            {
                resourceLoader.LoadImageForShop(ballMode, out var sprite);
                resourceLoader.LoadImage(_shopItem.itemId, out var imageSprite);
                itemImage.sprite = imageSprite;
                backGroundImage.sprite = sprite;
                if (ballMode == BallMode.GREEN)
                {
                    if (IsPurchased)
                    {
                        adsImage.SetActive(false);
                    }
                    else
                    {
                        adsImage.SetActive(true);
                        ballImage.gameObject.SetActive(true);
                        playerProgression.RequestPaymentProgress(_shopItem.shopItemId, out var playerPaymentProgress);
                        adsCountText.text = playerPaymentProgress.Progress.ToString() + "/" + playerPaymentProgress.Goal;
                    }
                    return;
                }
            }
        }
        if (IsPurchased)
        {
            adsImage.SetActive(false);
        }
        ballImage.gameObject.SetActive(_isPurchased);
    }
    private void OnButtonClicked()
    {
        if (_ballMode == BallMode.GREEN && !_isPurchased)
        {
            if (!admobController.IsRewardedAdLoaded)
            {
                return;
            }
            admobController.ShowRewardedAd(() =>
            {
                playerProgression.RequestPaymentProgress(_shopItem.shopItemId, out var playerPaymentProgress);
                playerPaymentProgress.UpdateProgress((onComplete) =>
                {
                    if (onComplete)
                    {
                        playerProgression.PlayerProfile.AddToInventory(_shopItem.itemId);
                    }
                    playerProgression.Save();
                    GameEvent.ReInitializedMission?.Invoke();
                });
                adsCountText.text = playerPaymentProgress.Progress.ToString() + "/" + playerPaymentProgress.Goal;
                if (playerPaymentProgress.Progress == playerPaymentProgress.Goal)
                {
                    RefreshButton(true, _shopItem, BallMode.GREEN);
                }
            });
            return;
        }
        if (!_isPurchased)
        {
            return;
        }
        OnSelectItemClicked?.Invoke(this);
    }
}
public enum BallMode
{
    NONE,
    BLUE,
    PINK,
    YELLOW,
    RED,
    GREEN
}
