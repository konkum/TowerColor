using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenjinAttributeProvider : MonoBehaviour
{
    public string tenjinId = "INSERT_TENJIN_SDK_HERE";


    private void Start()
    {
        TenjinStart();
    }

    private void OnApplicationPause(bool pause)
    {
        if(!pause)
        {
            TenjinStart();
        }
    }

    private void TenjinStart()
    {
        BaseTenjin instance = Tenjin.getInstance(tenjinId);
        instance.SetAppStoreType(AppStoreType.googleplay);
        instance.Connect();
    }
}
