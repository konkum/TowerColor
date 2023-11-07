using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joywire.ThirdParty
{
    public interface IAnalytics
    {
        void StartLogging();
        void LogScreen();
        void LogCustomEvent();
    }

    public class Analytics : MonoBehaviour, IAnalytics
    {
        [SerializeField] private AnalyticsImpl[] elements;

        private IEnumerator Start()
        {
            yield return null;
            StartLogging();
        }

        public void StartLogging()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                var ele = elements[i];
                try
                {
                    ele.StartLogging();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            //Array.ForEach(elements, t =>
            //{
            //    try
            //    {
            //        t.StartLogging();
            //    }
            //    catch (Exception ex) { }
            //});
        }
        public void LogCustomEvent()
        {
            Array.ForEach(elements, t =>
            {
                t.LogCustomEvent();
            });
        }
        public void LogScreen()
        {
            Array.ForEach(elements, t =>
            {
                t.LogScreen();
            });
        }
    }

    public class AnalyticsImpl : MonoBehaviour, IAnalytics
    {
        public virtual void LogCustomEvent()
        {
            
        }

        public virtual void LogScreen()
        {
            
        }

        public virtual void StartLogging()
        {
            
        }
    }
}

