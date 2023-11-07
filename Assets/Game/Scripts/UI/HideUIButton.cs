using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HideUIButton : MonoBehaviour
{
    [SerializeField] private Button _hideButton;
    [SerializeField] private HomeScreenElement _homeScreenUI;
    [SerializeField] private GamePlayUI _gamePlayUI;
    [SerializeField] private EndGameUI _endGameUI;

    private bool isHomeScreenActive = true;
    private bool isGamePlayUIActive = true;
    private bool isEndGameUIActive = true;
    void Start()
    {
        _hideButton.onClick.AddListener(OnHideButtonClicked);
    }
    private void OnHideButtonClicked()
    {
        if (!GameManager.Instance.IsGameStart)
        {
            isHomeScreenActive = !isHomeScreenActive;
            _homeScreenUI.gameObject.SetActive(isHomeScreenActive);
        }
        else
        {
            if (!GameManager.Instance.IsEndGame)
            {
                isGamePlayUIActive = !isGamePlayUIActive;
                _gamePlayUI.gameObject.SetActive(isGamePlayUIActive);
            }
            else
            {
                isEndGameUIActive = !isEndGameUIActive;
                _endGameUI.gameObject.SetActive(isEndGameUIActive);
            }
        }
    }
}
