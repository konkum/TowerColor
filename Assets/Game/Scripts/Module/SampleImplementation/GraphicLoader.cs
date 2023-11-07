using UnityEngine;
using Joywire.Core;
using Joywire;

public class GraphicLoader : MonoBehaviour
{
    [SerializeField] private GameObject Graphic;

    private IResourceLoader resourceLoader;

    public System.Action<GameObject> OnGraphicRefresh;


    public void LoadGraphic(string id)
    {
        if (resourceLoader == null)
        {
            ThirdParties.Find<IResourceLoader>(out resourceLoader);
        }
        for (int i = 0; i < Graphic.transform.childCount; i++)
        {
            Destroy(Graphic.transform.GetChild(i).gameObject);
        }
        resourceLoader.LoadObject(id, out var result);
        if (result != null)
        {
            var graphicObject = Instantiate(result, Graphic.transform);
            graphicObject.transform.localRotation = Quaternion.identity;
            OnGraphicRefresh?.Invoke(graphicObject);
        }
        Debug.Log("graphic object cannot be found");
    }



}
