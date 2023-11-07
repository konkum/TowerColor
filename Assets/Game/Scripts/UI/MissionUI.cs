using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Joywire;

public class MissionUI : UIScript
{
    [SerializeField] private Button missionBtn;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private Image diamondImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Image checkMark;

    private bool _isComplete = false;
    private bool _isClaimed = false;

    public bool IsComplete
    {
        get => _isComplete;
        set
        {
            _isComplete = value;
            if (value)
            {
                this.GetComponent<Image>().color = new Color(0, 209, 255, 255);
            }
            else
            {
                this.GetComponent<Image>().color = Color.white;
            }
        }
    }

    public bool IsClaimed
    {
        get => _isClaimed;
        set
        {
            _isClaimed = value;
            checkMark.gameObject.SetActive(value);
            if (_mission.RewardType == RewardType.Money)
            {
                moneyText.gameObject.SetActive(!value);
                diamondImage.gameObject.SetActive(!value);
                itemImage.gameObject.SetActive(false);
            }
            else if (_mission.RewardType == RewardType.Item)
            {
                if (playerProgression.PlayerProfile.PlayerInventory.Contains(_mission.Item.ItemID))
                {
                    moneyText.gameObject.SetActive(!value);
                    diamondImage.gameObject.SetActive(!value);
                    itemImage.gameObject.SetActive(false);
                }
                else
                {
                    itemImage.gameObject.SetActive(!value);
                    moneyText.gameObject.SetActive(false);
                    diamondImage.gameObject.SetActive(false);
                }
            }
        }
    }
    private Mission _mission;
    public Mission Mission => _mission;
    private IMissionController missionController;
    public void Initialized(Mission mission)
    {
        base.Initialized();
        _mission = mission;
        progressSlider.maxValue = _mission.AmmountToComplete;
        progressSlider.value = _mission.CurrentAmmount;
        InitializedMissionString();
        if (_mission.RewardType == RewardType.Money)
        {
            moneyText.text = _mission.MoneyAmmount.ToString();
        }
        else if (_mission.RewardType == RewardType.Item)
        {
            if (playerProgression.PlayerProfile.PlayerInventory.Contains(_mission.Item.ItemID))
            {
                moneyText.text = _mission.MoneyAmmount.ToString();
            }
            else
            {
                resourceLoader.LoadImage(_mission.Item.ItemID, out var sprite);
                itemImage.sprite = sprite;
            }
        }
        IsComplete = Mission.IsComplete;
        IsClaimed = Mission.IsClaimed;
        missionBtn.onClick.AddListener(OnMissionButtonClicked);
        ThirdParties.Find<IMissionController>(out missionController);
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }
    private void OnMissionButtonClicked()
    {
        if (!_isComplete)
        {
            return;
        }
        if (_isClaimed)
        {
            return;
        }
        IsClaimed = true;
        Mission.IsClaimed = true;
        if (_mission.RewardType == RewardType.Item)
        {
            if (playerProgression.PlayerProfile.PlayerInventory.Contains(_mission.Item.ItemID))
            {
                var currentMoney = playerProgression.PlayerProfile.GetCurrency("currency_00").amount;
                playerProgression.PlayerProfile.UpdatePlayerCurrency("currency_00", currentMoney + Mission.MoneyAmmount);
                GameEvent.OnMoneyUpdate?.Invoke(currentMoney + Mission.MoneyAmmount);
            }
            else
            {
                playerProgression.PlayerProfile.AddToInventory(_mission.Item.ItemID);
                GameEvent.OnUpdateShop?.Invoke(_mission.Item.ItemID);
            }
        }
        missionController.Save();
        playerProgression.Save();
    }
    private void InitializedMissionString()
    {
        switch (_mission.MissionType)
        {
            case MissionType.FinishLevel:
                missionText.text = string.Format("Complete {0} Levels", _mission.AmmountToComplete);
                break;
            case MissionType.ExplodeBalls:
                missionText.text = string.Format("Explode {0} Blocks using 1 Ball", _mission.AmmountToComplete);
                break;
            case MissionType.CollectKeys:
                missionText.text = string.Format("Colect {0} Keys", _mission.AmmountToComplete);
                break;
            default:
                break;
        }
    }
    private void UpdateMission(int value)
    {
        if (value <= progressSlider.maxValue)
            progressSlider.value = value;
    }
}