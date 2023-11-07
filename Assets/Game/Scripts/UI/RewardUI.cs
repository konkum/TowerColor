using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Joywire.Monetization;
using Joywire;
using Joywire.Core;
using TMPro;
using DG.Tweening;
public class RewardUI : UIScript
{
    [System.Serializable]
    private class Key
    {
        [SerializeField] private Image fill;
        public void SwitchState(bool isOn)
        {
            fill.gameObject.SetActive(isOn);
        }
    }
    [SerializeField] private WinScreenUI winScreenUI;
    [SerializeField] private GameObject glowingObject;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button skipBtn;
    [SerializeField] private Button adsBtn;
    [SerializeField] private GameObject keyHolder;
    [SerializeField] private List<Key> keys;
    [SerializeField] private List<RewardChestUI> rewardChests;

    private IGameShopManager _shopManager;
    private int _currentKeys = 0;
    private List<int> moneyReward = new List<int>()
    {
        10,10,15,15,25,25,30,30
    };
    public int CurrentKeys => _currentKeys;
    public override void Initialized()
    {
        base.Initialized();
        ThirdParties.Find<IGameShopManager>(out _shopManager);
        GameEvent.OnShowRewardUI += Show;
        skipBtn.onClick.AddListener(OnSkipButtonClicked);
        adsBtn.onClick.AddListener(OnAdsButtonClicked);
        adsBtn.transform.DOScale(Vector3.one * 1.1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        glowingObject.transform.DORotate(new Vector3(0, 0, 90), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }
    public override void Show()
    {
        base.Show();
        PopulateReward();
        keyHolder.gameObject.SetActive(true);
        adsBtn.gameObject.SetActive(false);
        skipBtn.gameObject.SetActive(false);
        StartCoroutine(SetKeyAmmount());
    }
    public override void Hide()
    {
        base.Hide();
    }
    private IEnumerator SetKeyAmmount(float time = 0)
    {
        _currentKeys = playerProgression.PlayerProfile.GetCurrency("currency_01").amount;
        for (int i = 0; i < _currentKeys; i++)
        {
            keys[i].SwitchState(true);
            yield return Helpers.GetWait(time);
        }
    }
    private void PopulateReward()
    {
        int rand = 0;
        List<Item> items = new List<Item>();
        for (int i = 28; i < 37; i++)
        {
            if (playerProgression.PlayerProfile.PlayerInventory.Contains(_shopManager.ShopItems[i].itemId))
            {
                continue;
            }
            items.Add(dataManager.GetItem(_shopManager.ShopItems[i].itemId));
        }
        if (items.Count != 0)
        {
            rand = Random.Range(0, items.Count);
        }
        var randRewardPlace = Random.Range(0, rewardChests.Count);
        for (int i = 0; i < rewardChests.Count; i++)
        {
            if (randRewardPlace == i)
            {
                if (items.Count == 0)
                {
                    rewardChests[i].IsObject = false;
                    rewardChests[i].RewardMoney = 50;
                }
                else
                {
                    rewardChests[i].IsObject = true;
                    rewardChests[i].Item = items[rand];
                }
            }
            else
            {
                rewardChests[i].IsObject = false;
                var randMoney = Random.Range(0, moneyReward.Count);
                rewardChests[i].RewardMoney = moneyReward[randMoney];
                moneyReward.RemoveAt(randMoney);
            }
            rewardChests[i].Initialized(this);
        }
        if (items.Count != 0)
        {
            itemImage.gameObject.SetActive(true);
            moneyText.transform.parent.gameObject.SetActive(false);
            resourceLoader.LoadImage(items[rand].ItemID, out var sprite);
            itemImage.sprite = sprite;
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            moneyText.transform.parent.gameObject.SetActive(true);
            moneyText.text = 50.ToString();
        }
    }
    public void UpdateCurrentKeys()
    {
        if (_currentKeys > 0)
        {
            _currentKeys--;
        }
        keys[_currentKeys].SwitchState(false);
        if (_currentKeys == 0)
        {
            int count = 0;
            for (int i = 0; i < rewardChests.Count; i++)
            {
                if (!rewardChests[i].IsClicked)
                {
                    count++;
                }
            }
            if (count != 0)
                adsBtn.gameObject.SetActive(true);
            StartCoroutine(Wait());
            keyHolder.gameObject.SetActive(false);
        }
    }
    private IEnumerator Wait()
    {
        yield return Helpers.GetWait(1.5f);
        skipBtn.gameObject.SetActive(true);
        var image = skipBtn.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0;
        image.color = tempColor;
        skipBtn.GetComponent<Image>().DOFade(1f, 1f);
    }
    private void OnAdsButtonClicked()
    {
        if (!admobController.IsRewardedAdLoaded)
        {
            return;
        }
        admobController.ShowRewardedAd(() =>
        {
            playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_01", 3);
            adsBtn.gameObject.SetActive(false);
            skipBtn.gameObject.SetActive(false);
            keyHolder.gameObject.SetActive(true);
            StartCoroutine(SetKeyAmmount(0.5f));
        });
    }
    private void OnSkipButtonClicked()
    {
        playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_01", 0);
        if (GameManager.Instance.IsEndGame)
        {
            winScreenUI.Show();
        }
        Hide();
        playerProgression.Save();
        GameEvent.ReInitializedMission?.Invoke();
    }
}
