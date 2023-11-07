using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joywire.UI;
using UnityEngine.UI;
using System;

public class SimpleMessageWindow : MonoBehaviour, IPopupWindow
{
    [SerializeField] private PopupType type = PopupType.SIMPLE_MESSAGE;
    public PopupType Type => type;


    [SerializeField] private Text MessageText;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
    }


    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show(string message = "", Action OnAction = null)
    {
        this.gameObject.SetActive(true);
        this.MessageText.text = message;
    }

     

   
}
