using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinMeshSample : MonoBehaviour
{
    protected bool isInit = false;

    protected void Test()
    {
        if (isInit == true) return;
        isInit = true;
        SkinnedMeshCombine combine = new SkinnedMeshCombine();
        combine.Combine(this.gameObject.AddComponent<SkinnedMeshRenderer>(),
                        null,
                        this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(),
                        new string[] { "_MainTex", "_BumpMap" },
                        new Material(Shader.Find("Mobile/Bumped Diffuse")),
                        1024,
                        512,
                        true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) == true)
        {
            Test();
        }
    }
}