using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshSample : MonoBehaviour
{
    protected bool isInit = false;

    protected void Test()
    {
        if (isInit == true) return;
        isInit = true;
        MeshCombine combine = new MeshCombine();
        combine.Combine(this.gameObject.AddComponent<MeshRenderer>(),
                        this.gameObject.GetComponentsInChildren<MeshRenderer>(),
                        new string[] { "_MainTex", "_BumpMap" },
                        new Material(Shader.Find("Mobile/Bumped Diffuse")),
                        1024,
                        512,
                        true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) == true)
        {
            Test();
        }
    }
}