using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joywire;
using Joywire.Core;

public abstract class BaseResourceLoader : MonoBehaviour, IResourceLoader
{


    public virtual void LoadImage(string id, out Sprite result)
    {
        throw new System.NotImplementedException();
    }

    public virtual void LoadImageForShop(BallMode ballMode, out Sprite result)
    {
        throw new System.NotImplementedException();
    }

    public virtual void LoadObject(string id, out GameObject result)
    {
        throw new System.NotImplementedException();
    }

    public virtual void LoadTexture(string id, out Texture2D texture)
    {
        throw new System.NotImplementedException();
    }

    protected virtual void Start()
    {
        ThirdParties.Register<IResourceLoader>(this);
    }

}
