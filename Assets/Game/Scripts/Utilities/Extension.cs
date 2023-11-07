using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static void SetLayerRecursively(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform t in gameObject.transform)
        {
            t.gameObject.SetLayerRecursively(layer);
        }
    }

    public static Vector2 ToV2(this Vector3 input) => new Vector2(input.x, input.y);
    public static Vector3 ToV3(this Vector2 input) => new Vector3(input.x, 0, input.y);
    public static Vector3 Flat(this Vector3 input) => new Vector3(input.x, 0, input.z);
    public static bool IsEqualTo(this Color me, Color other)
    {
        return me.r == other.r && me.g == other.g && me.b == other.b && me.a == other.a;
    }

    public static Vector3Int ToVector3Int(this Vector3 input) => new Vector3Int((int)input.x, (int)input.y, (int)input.z);
    public static Color ToColor(this int color)
    {
        float r = (color >> 24 & 0xFF) / 255f;
        float g = (color >> 16 & 0xFF) / 255f;
        float b = (color >> 8 & 0xFF) / 255f;
        float a = (color & 0xFF) / 255f;
        return new Color(r, g, b, a);
    }
}
