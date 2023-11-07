using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joywire.ThirdParty
{
    using GameAnalyticsSDK;

    [System.Serializable]
    public class GameAnalyticsImpl : AnalyticsImpl
    {
        public override void StartLogging()
        {
            //Debug.LogError("Game analytics implement!");
            GameAnalytics.Initialize();

            //GameAnalytics.StartSession();
            GameAnalytics.NewDesignEvent("gamestart");
            //force check settings load!
            // var settings = GameAnalytics.SettingsGA;
            //GameAnalytics.StartSession();
        }
    }

}