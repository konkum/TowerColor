using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class HomeScreenElement : UIScript
{
    [System.Serializable]
    private class CustomToggle
    {
        [SerializeField] private Button toggleBtn;
        [SerializeField] private Image onImage;
        [SerializeField] private Image offImage;
        private bool _isOn = true;
        public bool IsOn
        {
            get => _isOn;
            set => _isOn = value;
        }
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        public void Initialized(System.Action action = null, Transform startPosition = null)
        {
            toggleBtn.onClick.AddListener(() =>
            {
                _isOn = !_isOn;
                onImage.gameObject.SetActive(_isOn);
                offImage.gameObject.SetActive(!_isOn);
                action?.Invoke();
            });
            onImage.gameObject.SetActive(_isOn);
            offImage.gameObject.SetActive(!_isOn);
            _startPosition = startPosition.position;
            _endPosition = toggleBtn.transform.position;
            toggleBtn.transform.position = _startPosition;
        }
        public void MoveAnim(bool isOn)
        {
            if (isOn)
            {
                toggleBtn.transform.DOMove(_endPosition, 0.5f).SetEase(Ease.OutSine);
            }
            else
            {
                toggleBtn.transform.DOMove(_startPosition, 0.5f).SetEase(Ease.OutSine);
            }
        }
    }
    [System.Serializable]
    private class KeyElement
    {
        [SerializeField] private Image fill;
        public void SetActive(bool isOn)
        {
            fill.gameObject.SetActive(isOn);
        }
    }
    [System.Serializable]
    private class HomeScreenElementUI
    {
        [SerializeField] private Button elementBtn;
        [SerializeField] private UIScript screen;

        public void Initialized()
        {
            elementBtn.onClick.AddListener(() =>
            {
                screen.Show();
            });
            screen.Initialized();
        }
    }
    private const string VIBRATE = "VIBRATE";
    private const string SOUND = "SOUND";
    [Header("ELEMENT SECTION")]
    [SerializeField] private HomeScreenElementUI shopElementUI;
    [SerializeField] private HomeScreenElementUI missionElementUI;
    [SerializeField] private HomeScreenElementUI shopMoneyElementUI;
    [SerializeField] private HomeScreenElementUI noAdsElementUI;

    [Header("TOP SECTION")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button moneyBtn;
    [SerializeField] private GameObject settingsObject;
    [SerializeField] private CustomToggle vibrationToggle;
    [SerializeField] private CustomToggle soundToggle;
    [SerializeField] private Button textButton;
    [SerializeField] private List<KeyElement> keys;
    [Header("BOTTOM SECTION")]
    [SerializeField] private Button startBtn;

    private bool _settingsIsOn = false;
    private int _currentKeysAmmount = 0;
    private Vector3 textEndPosition;
    public override void Initialized()
    {
        base.Initialized();
        shopElementUI.Initialized();
        missionElementUI.Initialized();
        noAdsElementUI.Initialized();
        InitializeSettings();
        levelText.text = playerProgression.PlayerProfile.PlayerProgress.CurrentID;
        textEndPosition = textButton.transform.position;
        textButton.transform.position = settingsButton.transform.position;
        GameEvent.OnMoneyUpdate += OnMoneyUpdateCallBack;
        OnMoneyUpdateCallBack(playerProgression.PlayerProfile.GetCurrency("currency_00").amount);
        settingsButton.onClick.AddListener(SettingsButtonClicked);
        startBtn.onClick.AddListener(OnStartButtonClicked);
        moneyBtn.onClick.AddListener(OnMoneyButtonClickedCallBack);
        _currentKeysAmmount = playerProgression.PlayerProfile.GetCurrency("currency_01").amount;
        for (int i = 0; i < _currentKeysAmmount; i++)
        {
            keys[i].SetActive(true);
        }
    }
    private void InitializeSettings()
    {
        if (PlayerPrefs.HasKey(SOUND))
        {
            soundToggle.IsOn = PlayerPrefs.GetInt(SOUND) == 1 ? true : false;
        }
        else
        {
            soundToggle.IsOn = true;
            PlayerPrefs.SetInt(SOUND, 1);
        }
        if (PlayerPrefs.HasKey(VIBRATE))
        {
            vibrationToggle.IsOn = PlayerPrefs.GetInt(VIBRATE) == 1 ? true : false;
        }
        else
        {
            vibrationToggle.IsOn = true;
            PlayerPrefs.SetInt(VIBRATE, 1);
        }
        Vibrator.IsVibrate = vibrationToggle.IsOn;
        AudioListener.pause = !soundToggle.IsOn;
        vibrationToggle.Initialized(OnVibrationToggleClicked, settingsButton.transform);
        soundToggle.Initialized(OnSoundToggleClicked, settingsButton.transform);
    }
    private void OnVibrationToggleClicked()
    {
        PlayerPrefs.SetInt(VIBRATE, vibrationToggle.IsOn == true ? 1 : 0);
        Vibrator.IsVibrate = vibrationToggle.IsOn;
    }
    private void OnSoundToggleClicked()
    {
        PlayerPrefs.SetInt(SOUND, soundToggle.IsOn == true ? 1 : 0);
        AudioListener.pause = !soundToggle.IsOn;
    }
    private void OnStartButtonClicked()
    {
        GameManager.Instance.IsGameStart = true;
        this.Hide();
    }
    private void SettingsButtonClicked()
    {
        _settingsIsOn = !_settingsIsOn;
        vibrationToggle.MoveAnim(_settingsIsOn);
        soundToggle.MoveAnim(_settingsIsOn);
        if (_settingsIsOn)
        {
            textButton.transform.DOMove(textEndPosition, 0.5f).SetEase(Ease.OutSine);
        }
        else
        {
            textButton.transform.DOMove(settingsButton.transform.position, 0.5f).SetEase(Ease.OutSine);
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
    private void OnMoneyButtonClickedCallBack()
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
    private void OnMoneyUpdateCallBack(int money)
    {
        moneyText.text = money.ToString();
    }
}
