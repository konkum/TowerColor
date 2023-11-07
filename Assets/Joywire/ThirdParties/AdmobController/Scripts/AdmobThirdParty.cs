using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joywire;

public class AdmobThirdParty : MonoBehaviour
{
    private void Start()
    {
        var cmp = GetComponent<AdmobController>();

        ThirdParties.Register<IAdmobController>(cmp);
        Debug.Log("Register thirdparties");

        cmp.Init();
    }
}
