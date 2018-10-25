using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseMeshCombine
{
    protected List<CombineInstance> combineInstances = new List<CombineInstance>();

    protected Dictionary<string, List<Texture2D>> textureDics = new Dictionary<string, List<Texture2D>>();

    protected List<Vector2[]> uvList = new List<Vector2[]>();

    protected int[] areas = new int[] { 64, 128, 256, 512, 1024, 2048 };

    protected void InitTextureDics(string[] texPropertyNames)
    {
        textureDics.Clear();
        if (texPropertyNames == null) return;
        int texLen = texPropertyNames.Length;
        for (int i = 0; i < texLen; ++i)
        {
            textureDics[texPropertyNames[i]] = new List<Texture2D>();
        }
    }

    protected void FindTexture(Material mt, string[] texPropertyNames)
    {
        int texLen = texPropertyNames.Length;
        for (int i = 0; i < texLen; ++i)
        {
            textureDics[texPropertyNames[i]].Add(mt.GetTexture(texPropertyNames[i]) as Texture2D);
        }
    }

    public Texture2D CustomPackageTexture(string propertyName, Rect[] rects, int width, int height)
    {
        Texture2D t = new Texture2D(width, height);
        t.PackTextures(this.textureDics[propertyName].ToArray(), 0);
        /*List<Texture2D> texs = this.textureDics[propertyName];

        for (int i = 0; i < rects.Length; ++i)
        {
            t.SetPixels((int)rects[i].x * width, (int)rects[i].y * height, (int)rects[i].width * width, (int)rects[i].height * height, texs[i].GetPixels());
        }*/
        return t;
    }

    public Texture2D CreateTexture(int width, int height)
    {
        if (width < 0 || height < 0)
        {
            int textureArea = getTextureArea();
            for (int i = 0; i < areas.Length; ++i)
            {
                if (textureArea < areas[i] * areas[i])
                {
                    return new Texture2D(areas[i], areas[i]);
                }
            }
            return new Texture2D(2048, 2048);
        }
        return new Texture2D(width, height);
    }

    public int getTextureArea()
    {
        foreach (KeyValuePair<string, List<Texture2D>> kv in textureDics)
        {
            int sum = 0;
            for (int i = 0; i < kv.Value.Count; ++i) sum += kv.Value[i].width * kv.Value[i].height;
            return sum;
        }
        return 0;
    }

    public Vector2[] ResetUV(Rect[] rects, int uvCount)
    {
        Vector2[] newUVs = new Vector2[uvCount];

        int sum = 0;
        for (int i = 0; i < uvList.Count; ++i)
        {
            for (int j = 0; j < uvList[i].Length; ++j)
            {
                newUVs[sum].x = Mathf.Lerp(rects[i].xMin, rects[i].xMax, uvList[i][j].x);
                newUVs[sum].y = Mathf.Lerp(rects[i].yMin, rects[i].yMax, uvList[i][j].y);
                ++sum;
            }
        }
        return newUVs;
    }
}