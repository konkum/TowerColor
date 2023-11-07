using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joywire.Core;
using Joywire;
public class UIScript : MonoBehaviour
{
    protected IPlayerProgression playerProgression;
    protected IRuntimeDataManager dataManager;
    protected IResourceLoader resourceLoader;
    protected IAdmobController admobController;
    public virtual void Initialized()
    {
        ThirdParties.Find<IPlayerProgression>(out playerProgression);
        ThirdParties.Find<IRuntimeDataManager>(out dataManager);
        ThirdParties.Find<IResourceLoader>(out resourceLoader);
        ThirdParties.Find<IAdmobController>(out admobController);
    }
    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
