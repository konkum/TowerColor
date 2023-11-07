using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joywire;
public class MissionController : MonoBehaviour, IMissionController
{
    private const string MISSIONDATA = "MISSIONDATA";
    [SerializeField] private List<MissionData> defaultMissions;
    [SerializeField] private List<MissionData> missionDatas;

    private MissionList _missionList;
    public MissionList MissionList { get => _missionList; set => _missionList = value; }

    private string _saveData => JsonUtility.ToJson(_missionList);
    private void Start()
    {
        ThirdParties.Register<IMissionController>(this);
        Initialize();
        DontDestroyOnLoad(this);
    }
    public void Initialize()
    {
        string data = "";
        if (string.IsNullOrEmpty(_saveData))
        {
            _missionList = new MissionList();
        }

        if (PlayerPrefs.HasKey(MISSIONDATA))
        {
            data = PlayerPrefs.GetString(MISSIONDATA);
        }

        Load(data);
        Save();
    }

    public void Load(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            _missionList = JsonUtility.FromJson<MissionList>(data);
        }
        else
        {
            for (int i = 0; i < defaultMissions.Count; i++)
            {
                _missionList.currentMissions.Add(new Mission(defaultMissions[i]));
            }
        }
    }
    public void UpdateMission(MissionType missionType, int value)
    {
        for (int i = 0; i < _missionList.currentMissions.Count; i++)
        {
            if (missionType == _missionList.currentMissions[i].MissionType)
            {
                if (_missionList.currentMissions[i].IsComplete)
                {
                    continue;
                }
                AddLogic(missionType, i, value);
                if (_missionList.currentMissions[i].CurrentAmmount >= _missionList.currentMissions[i].AmmountToComplete)
                {
                    _missionList.currentMissions[i].CurrentAmmount = _missionList.currentMissions[i].AmmountToComplete;
                    _missionList.currentMissions[i].IsComplete = true;
                }
            }
            else
            {
                continue;
            }
        }
    }
    private void AddLogic(MissionType missionType, int index, int value)
    {
        switch (missionType)
        {
            case MissionType.ExplodeBalls:
                _missionList.currentMissions[index].CurrentAmmount = value;
                break;
            default:
                _missionList.currentMissions[index].CurrentAmmount += value;
                break;
        }
    }
    public int GetCurrentAmmount(MissionType missionType)
    {
        for (int i = 0; i < _missionList.currentMissions.Count; i++)
        {
            if (missionType == _missionList.currentMissions[i].MissionType && !_missionList.currentMissions[i].IsComplete)
            {
                return _missionList.currentMissions[i].CurrentAmmount;
            }
        }
        return 0;
    }
    public void RefreshMission()
    {
        for (int i = 0; i < _missionList.currentMissions.Count; i++)
        {
            if (_missionList.currentMissions[i].IsComplete)
            {
                var rand = Random.Range(0, missionDatas.Count);
                _missionList.currentMissions[i] = new Mission(missionDatas[rand]);
            }
        }
        Save();
    }
    public void RefreshNotDoneMission()
    {
        for (int i = 0; i < _missionList.currentMissions.Count; i++)
        {
            if (_missionList.currentMissions[i].CurrentAmmount == 0)
            {
                var rand = Random.Range(0, missionDatas.Count);
                _missionList.currentMissions[i] = new Mission(missionDatas[rand]);
            }
        }
    }
    public void Save()
    {
        var data = _saveData;
        PlayerPrefs.SetString(MISSIONDATA, data);
    }
}
[System.Serializable]
public class Mission
{
    public MissionType MissionType;
    public RewardType RewardType;
    public int AmmountToComplete;
    public int CurrentAmmount;
    public int MoneyAmmount;
    public ItemData Item;
    public bool IsComplete;
    public bool IsClaimed;

    public Mission(MissionData missionData)
    {
        MissionType = missionData.MissionType;
        RewardType = missionData.RewardType;
        AmmountToComplete = missionData.AmmountToComplete;
        CurrentAmmount = missionData.CurrentAmmount;
        MoneyAmmount = missionData.MoneyAmmount;
        Item = missionData.Item;
        IsComplete = missionData.IsComplete;
        IsClaimed = missionData.IsClaimed;
    }
}
[System.Serializable]
public class MissionList
{
    public List<Mission> currentMissions = new List<Mission>();
}
public interface IMissionController
{
    MissionList MissionList { get; }
    public void Load(string data);
    public void Save();
    public void UpdateMission(MissionType missionType, int value);
    public int GetCurrentAmmount(MissionType missionType);
    public void RefreshMission();
    public void RefreshNotDoneMission();
}
