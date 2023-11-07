using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private HomeScreenElement homeScreenElement;
    [SerializeField] private GamePlayUI gamePlayUI;
    [SerializeField] private EndGameUI endGameUI;
    [SerializeField] private RewardUI rewardUI;
    private void Start()
    {
        homeScreenElement.Initialized();
        gamePlayUI.Initialized();
        endGameUI.Initialized();
        rewardUI.Initialized();
        GameEvent.OnGameStart += ShowGamePlayUI;
    }
    private void ShowGamePlayUI()
    {
        gamePlayUI.Show();
    }
}
