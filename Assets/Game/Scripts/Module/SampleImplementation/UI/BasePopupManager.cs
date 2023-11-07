using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joywire.Core;
using Joywire;
using Joywire.Monetization;
using Joywire.UI;
using System;

public class BasePopupManager : MonoBehaviour, IPopupManager
{
    [SerializeField]protected IPopupWindow[] windows;


    private void Start()
    {
        Initialize();
        DontDestroyOnLoad(gameObject);
    }


    public void Initialize()
    {
        ThirdParties.Register<IPopupManager>(this);
        windows = GetComponentsInChildren<IPopupWindow>();

        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].Hide();
        }
    }

   
    public void ShowPopupWindow(PopupType type, string message = "", Action OnAction = null)
    {
        IPopupWindow targetedWindow = null;
        for(int i = 0; i < windows.Length; i++)
        {
            if(windows[i].Type == type)
            {
                targetedWindow = windows[i];
            }
            windows[i].Hide();
        }

        targetedWindow.Show(message, OnAction);

       
    }




   
}
