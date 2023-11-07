using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;
using Joywire;

public class MissionElementUI : UIScript
{
    private const string LastTimeCheck = "LastTimeClicked";
    private const float TIMEBETWEENRESET = 7200;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button refreshBtn;
    [SerializeField] private GameObject mainObject;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private List<MissionUI> missions;

    private float _timeLeft = TIMEBETWEENRESET;
    private bool _timerOn = true;
    private ulong _lastTimeClicked;

    public override void Initialized()
    {
        base.Initialized();
        GameEvent.ReInitializedMission += ReInitialized;
        ThirdParties.Find<IMissionController>(out var missionController);
        if (PlayerPrefs.HasKey(LastTimeCheck))
        {
            _lastTimeClicked = ulong.Parse(PlayerPrefs.GetString(LastTimeCheck));
        }
        else
        {
            _lastTimeClicked = (ulong)DateTime.Now.Ticks;
            PlayerPrefs.SetString(LastTimeCheck, _lastTimeClicked.ToString());
        }
        ulong diff = ((ulong)DateTime.Now.Ticks - _lastTimeClicked);
        ulong m = diff / TimeSpan.TicksPerMillisecond;
        _timeLeft = (float)(_timeLeft * 1000 - m) / 1000.0f;
        refreshBtn.gameObject.SetActive(false);
        refreshBtn.onClick.AddListener(RefreshMission);
        refreshBtn.transform.DOScale(Vector3.one * 1.1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        for (int i = 0; i < missions.Count; i++)
        {
            missions[i].Initialized(missionController.MissionList.currentMissions[i]);
            if (missions[i].IsComplete)
            {
                refreshBtn.gameObject.SetActive(true);
            }
        }
        closeBtn.onClick.AddListener(Hide);
        Helpers.Camera.GetComponentInParent<CameraController>().StartCoroutine(CountDowntStart());
    }

    public override void Show()
    {
        base.Show();
        mainObject.transform.localScale = Vector3.zero;
        mainObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public override void Hide()
    {
        mainObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => base.Hide());
    }

    private IEnumerator CountDowntStart()
    {
        while (true)
        {
            yield return Helpers.GetWait(0.01f);
            if (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
                UpdateTimer(_timeLeft);
            }
            else
            {
                _timeLeft = 0;
                break;
            }
        }
        RefreshNotDoneMission();
    }
    private void RefreshMission()
    {
        if (!admobController.IsRewardedAdLoaded)
        {
            return;
        }
        admobController.ShowRewardedAd(() =>
        {
            refreshBtn.gameObject.SetActive(false);
            ThirdParties.Find<IMissionController>(out var missionController);
            missionController.RefreshMission();
            for (int i = 0; i < missions.Count; i++)
            {
                missions[i].Initialized(missionController.MissionList.currentMissions[i]);
            }
        });
    }
    private void ReInitialized()
    {
        ThirdParties.Find<IMissionController>(out var missionController);
        for (int i = 0; i < missions.Count; i++)
        {
            missions[i].Initialized(missionController.MissionList.currentMissions[i]);
        }
    }
    private void RefreshNotDoneMission()
    {
        _timeLeft = TIMEBETWEENRESET;
        _lastTimeClicked = (ulong)DateTime.Now.Ticks;
        PlayerPrefs.SetString(LastTimeCheck, _lastTimeClicked.ToString());
        ThirdParties.Find<IMissionController>(out var missionController);
        missionController.RefreshNotDoneMission();
        for (int i = 0; i < missions.Count; i++)
        {
            missions[i].Initialized(missionController.MissionList.currentMissions[i]);
        }
        Helpers.Camera.GetComponentInParent<CameraController>().StartCoroutine(CountDowntStart());
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;
        float hours = Mathf.FloorToInt(currentTime / 3600) % 24;
        float minutes = Mathf.FloorToInt(currentTime / 60) % 60;
        float seconds = Mathf.FloorToInt(currentTime % 60);

        StringBuilder nextMissionString = new StringBuilder();
        nextMissionString.AppendFormat("NEXT MISSIONS IN {0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        timeText.text = nextMissionString.ToString();
    }
}