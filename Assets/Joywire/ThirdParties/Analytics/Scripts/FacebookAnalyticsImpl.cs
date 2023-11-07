using Joywire.ThirdParty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookAnalyticsImpl : AnalyticsImpl
{
    public override void LogCustomEvent()
    {
        
    }

    public override void LogScreen()
    {
        
    }

    public override void StartLogging()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            FB.Mobile.SetAutoLogAppEventsEnabled(true);
            FB.Mobile.SetAdvertiserTrackingEnabled(true);
            FB.LogAppEvent("AppInit");
        }
        else
        {
            //Handle FB.Init
            FB.Init(() => {
                FB.ActivateApp();
                FB.Mobile.SetAutoLogAppEventsEnabled(true);
                FB.Mobile.SetAdvertiserTrackingEnabled(true);
                FB.LogAppEvent("AppInit");
            });
        }
    }


    // Unity will call OnApplicationPause(false) when an app is resumed
    // from the background
    void OnApplicationPause(bool pauseStatus)
    {
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!pauseStatus)
        {
            //app resume
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() => {
                    FB.ActivateApp();
                });
            }
        }
    }
}
