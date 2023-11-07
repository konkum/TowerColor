using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
public class LoseGameUI : UIScript
{
    [SerializeField] private Button additionalBallBtn;
    [SerializeField] private Button skipBtn;
    public override void Initialized()
    {
        base.Initialized();
        skipBtn.onClick.AddListener(OnSkipButtonClicked);
        additionalBallBtn.onClick.AddListener(OnAdditionalBallClicked);
    }
    public override void Show()
    {
        base.Show();
        additionalBallBtn.transform.localScale = Vector3.zero;
        var image = skipBtn.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0;
        image.color = tempColor;
        var text = skipBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        var textColor = text.color;
        textColor.a = 0;
        text.color = textColor;
        var sequence = DOTween.Sequence();
        sequence.Append(additionalBallBtn.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            additionalBallBtn.transform.DOScale(Vector3.one * 1.1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }));
        sequence.Append(image.DOFade(1f, 1f));
        sequence.Join(text.DOFade(1f, 1f));
        sequence.Play();
    }
    public override void Hide()
    {
        base.Hide();
    }
    private void OnAdditionalBallClicked()
    {
        if (!admobController.IsRewardedAdLoaded)
        {
            return;
        }
        admobController.ShowRewardedAd(() =>
        {
            this.Hide();
            GameManager.Instance.IsEndGame = false;
            GameEvent.OnAddAditionalBall?.Invoke();
        });
    }
    private void OnSkipButtonClicked()
    {
        if (!admobController.IsInterstitialAdLoaded)
        {

        }
        admobController.ShowInterstitial(() => SceneManager.LoadScene("Game"));
    }
}
