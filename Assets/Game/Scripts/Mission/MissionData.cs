using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Mission")]
public class MissionData : ScriptableObject
{
    public MissionType MissionType;
    public RewardType RewardType;
    public int AmmountToComplete;
    public int CurrentAmmount;
    public int MoneyAmmount;
    public ItemData Item;
    public bool IsComplete;
    public bool IsClaimed;
}
public enum MissionType
{
    None,
    FinishLevel,
    ExplodeBalls,
    CollectKeys
}
public enum RewardType
{
    Money,
    Item
}
