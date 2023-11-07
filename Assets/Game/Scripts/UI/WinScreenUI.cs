using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Joywire;
public class WinScreenUI : UIScript
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button adsBtn;
    [SerializeField] private Button skipBtn;
    private int _money;
    private IMissionController missionController;
    public override void Initialized()
    {
        base.Initialized();
        adsBtn.onClick.AddListener(OnAdsButtonClicked);
        skipBtn.onClick.AddListener(OnSkipButtonClicked);
        var rand = Random.Range(10, 30);
        _money = rand;
        moneyText.text = rand.ToString();
        ThirdParties.Find<IMissionController>(out missionController);
    }
    public override void Show()
    {
        base.Show();
        moneyText.transform.parent.transform.localScale = Vector3.zero;
        adsBtn.transform.localScale = Vector3.zero;
        var image = skipBtn.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0;
        image.color = tempColor;
        var text = skipBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        var textColor = text.color;
        textColor.a = 0;
        text.color = textColor;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(moneyText.transform.parent.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce));
        sequence.Append(adsBtn.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce).OnComplete(() => adsBtn.transform.DOScale(Vector3.one * 1.1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)));
        sequence.Append(skipBtn.GetComponent<Image>().DOFade(1f, 1f));
        sequence.Join(text.DOFade(1f, 1f));
        sequence.Play();
    }
    public override void Hide()
    {
        base.Hide();
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
            var summary = currentMoney + _money * 10;
            playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_00", summary);
            playerProgression.PlayerProfile.PlayerProgress.CompleteCurrentLevel();
            missionController.UpdateMission(MissionType.FinishLevel, 1);
            missionController.Save();
            playerProgression.Save();
            SceneManager.LoadScene("Game");
        });
    }
    private void OnSkipButtonClicked()
    {
        if (!admobController.IsInterstitialAdLoaded)
        {

        }
        admobController.ShowInterstitial(() =>
        {
            var currentMoney = playerProgression.PlayerProfile.GetCurrency("currency_00").amount;
            var summary = currentMoney + _money;
            playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_00", summary);
            playerProgression.PlayerProfile.PlayerProgress.CompleteCurrentLevel();
            missionController.UpdateMission(MissionType.FinishLevel, 1);
            missionController.Save();
            playerProgression.Save();
            SceneManager.LoadScene("Game");
        });
    }
}
