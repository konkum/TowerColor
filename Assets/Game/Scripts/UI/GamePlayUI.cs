using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using DG.Tweening;
public class GamePlayUI : UIScript
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider trackingSlider;
    [SerializeField] private TextMeshProUGUI ballCountText;
    [SerializeField] private Button bombAdsBtn;
    [SerializeField] private Button bombBtn;
    [SerializeField] private Slider bombSlider;
    [SerializeField] private EndGameUI endGameUI;

    private bool _bombShoot = false;
    public override void Initialized()
    {
        base.Initialized();
        levelText.text = playerProgression.PlayerProfile.PlayerProgress.CurrentID.ToUpper();
        bombSlider.maxValue = 30;
        bombAdsBtn.onClick.AddListener(AdsReplaceBallWithBomb);
        bombAdsBtn.gameObject.SetActive(false);
        bombBtn.onClick.AddListener(ReplaceBallWithBomb);
        bombAdsBtn.transform.DOScale(Vector3.one * 1.1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        GameEvent.OnEndGame += HideWhenEndGame;
        GameEvent.UpdateBallCount += UpdateBallCount;
        GameEvent.OnEndGame += OnEndGameCallBack;
        GameEvent.OnAddAditionalBall += Show;
        GameEvent.OnBombSliderUpdate += OnBombSliderUpdateCallBack;
        Helpers.Camera.GetComponentInParent<CameraController>().StartCoroutine(Wait());
    }
    private void Update()
    {
        trackingSlider.value = GameManager.Instance.Tower.Bricks.Count - 70;
        if (bombSlider.value > 0)
        {
            if (bombSlider.value >= bombSlider.maxValue)
            {
                return;
            }
            bombSlider.value -= Time.deltaTime * 10f;
        }
    }
    public override void Show()
    {
        base.Show();
    }
    public override void Hide()
    {
        base.Hide();
    }
    private void OnEndGameCallBack(bool isWin)
    {
        this.Hide();
    }
    private IEnumerator Wait()
    {
        yield return Helpers.GetWait(0.5f);
        trackingSlider.maxValue = GameManager.Instance.Tower.Bricks.Count - 70;
    }
    private void AdsReplaceBallWithBomb()
    {
        if (!admobController.IsRewardedAdLoaded)
        {
            return;
        }
        admobController.ShowRewardedAd(() =>
        {
            bombAdsBtn.gameObject.SetActive(false);
            GameManager.Instance.ReplaceBallWithBomb();
            GameManager.Instance.ReplaceBallWithBomb();
            GameManager.Instance.ReplaceBallWithBomb();
        });
    }
    private void ReplaceBallWithBomb()
    {
        if (bombSlider.value < bombSlider.maxValue)
        {
            return;
        }
        _bombShoot = true;
        bombSlider.value = 0;
        bombAdsBtn.gameObject.SetActive(true);
        GameManager.Instance.ReplaceBallWithBomb();
    }
    private void HideWhenEndGame(bool isWin)
    {
        this.Hide();
    }
    private void UpdateBallCount(int ballCount)
    {
        ballCountText.text = ballCount.ToString();
    }
    private void OnBombSliderUpdateCallBack(float value)
    {
        if (_bombShoot)
        {
            return;
        }
        bombSlider.value += value;
    }
}
