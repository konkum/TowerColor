using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameUI : UIScript
{
    [SerializeField] private LoseGameUI loseGameUI;
    [SerializeField] private WinScreenUI winScreenUI;
    [SerializeField] private RewardUI rewardUI;
    public override void Initialized()
    {
        base.Initialized();
        loseGameUI.Initialized();
        winScreenUI.Initialized();
        GameEvent.OnEndGame += ShowEndGame;
    }
    public override void Show()
    {
        base.Show();
    }
    private void ShowEndGame(bool isWin)
    {
        if (isWin)
        {
            SoundManager.Instance.PlaySound(Sound.Win);
            if (playerProgression.PlayerProfile.GetCurrency("currency_01").amount == 3)
            {
                rewardUI.Show();
            }
            else
            {
                winScreenUI.Show();
            }
        }
        else
        {
            loseGameUI.Show();
        }
    }
    public override void Hide()
    {
        base.Hide();
    }
}
