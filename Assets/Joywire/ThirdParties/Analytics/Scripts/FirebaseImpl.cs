using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joywire.ThirdParty
{
    using Firebase.Analytics;
    public class FirebaseImpl : AnalyticsImpl
    {
        public override void StartLogging()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(t =>
            {
                if(t.Result == Firebase.DependencyStatus.Available)
                {
                    FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
                }
            });
        }
    }

}
