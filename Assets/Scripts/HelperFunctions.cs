using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class HelperFunctions
{
    #region Math

    public static int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    #endregion

    #region Strings

    public static string ListToString<T>(List<T> list)
    {
        string txt = "";
        foreach (T elem in list) txt += elem.ToString() + ", ";
        txt = txt.TrimEnd(' ');
        txt = txt.TrimEnd(',');
        return txt;
    }

    public static string GetOrdinalSuffix(int num)
    {
        string number = num.ToString();
        if (number.EndsWith("11")) return "th";
        if (number.EndsWith("12")) return "th";
        if (number.EndsWith("13")) return "th";
        if (number.EndsWith("1")) return "st";
        if (number.EndsWith("2")) return "nd";
        if (number.EndsWith("3")) return "rd";
        return "th";
    }

    #endregion

    #region UI

    /// <summary>
    /// Destroys all children of a GameObject immediately. The first n elements can be skipped with the skip param.
    /// </summary>
    public static void DestroyAllChildredImmediately(GameObject obj, int skip = 0)
    {
        int numChildren = obj.transform.childCount;
        for (int i = 0; i < numChildren - skip; i++) GameObject.DestroyImmediate(obj.transform.GetChild(skip).gameObject);
    }

    public static Sprite Texture2DToSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// Sets the Left, Right, Top and Bottom attribute of a RectTransform
    /// </summary>
    public static void SetRectTransformMargins(RectTransform rt, float left, float right, float top, float bottom)
    {
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    public static void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    /// <summary>
    /// Creates a rectangular Image on the UI displaying a line from point A to point B
    /// </summary>
    public static GameObject CreateLine(Transform parent, Vector2 dotPositionA, Vector2 dotPositionB, Color color, float thickness)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(parent, false);
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchoredPosition = new Vector2((dotPositionB.x + dotPositionA.x) / 2, (dotPositionB.y + dotPositionA.y) / 2);
        rectTransform.sizeDelta = new Vector2(distance, thickness);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        //rectTransform.anchoredPosition = (dotPositionA - new Vector2(graphPositionX, graphPositionY)) + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        gameObject.transform.SetSiblingIndex(0);

        return gameObject;
    }

    private static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    #endregion

    #region Textures / Images

    /// <summary>
    /// Create a sprite from a file path to a .jpg or .png
    /// </summary>
    public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {
        Texture2D SpriteTexture = LoadTexture(FilePath);
        return Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
    }

    /// <summary>
    /// Create a Texture2D from a file path to a .jpg or .png
    /// </summary>
    public static Texture2D LoadTexture(string FilePath)
    {
        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

    #endregion

}
