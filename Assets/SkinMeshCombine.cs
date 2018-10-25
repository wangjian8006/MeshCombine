using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshCombine : BaseMeshCombine
{
    protected List<Transform> boneList = new List<Transform>();

    public void Combine(SkinnedMeshRenderer newSkinMeshRender, 
        Transform[] bones,
        SkinnedMeshRenderer[] needCombines, 
        string[] texPropertyNames,
        Material newMaterial = null,
        int texWidth = -1,
        int texHeight = -1,
        bool hideRawMesh = true)
    {
        combineInstances.Clear();
        uvList.Clear();
        boneList.Clear();
        InitTextureDics(texPropertyNames);

        int uvCount = 0;
        int meshLen = needCombines.Length;

        for (int i = 0; i < meshLen; ++i)
        {
            SkinnedMeshRenderer smr = needCombines[i];
            if (smr == newSkinMeshRender) continue;
            for (int j = 0; j < smr.sharedMesh.subMeshCount; ++j)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = j;
                combineInstances.Add(ci);
            }

            uvList.Add(smr.sharedMesh.uv);
            uvCount += smr.sharedMesh.uv.Length;

            Debug.Assert(smr.materials != null && smr.materials.Length == 1, "The materials error.");
            Debug.Assert(smr.material != null, "The material error");

            FindTexture(smr.material, texPropertyNames);
            FindBones(bones, smr);

            if (hideRawMesh == true) smr.gameObject.SetActive(false);
        }

        newSkinMeshRender.sharedMesh = new Mesh();
        newSkinMeshRender.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, false);
        newSkinMeshRender.bones = boneList.ToArray();

        if (texPropertyNames != null) SingleMaterial(newSkinMeshRender, texPropertyNames, texWidth, texHeight, uvCount, newMaterial);
        else newSkinMeshRender.materials = MuiltMaterial(newSkinMeshRender, needCombines);
    }

    private Material[] MuiltMaterial(SkinnedMeshRenderer newSkinMeshRender, SkinnedMeshRenderer[] rds)
    {
        List<Material> mts = new List<Material>();
        for (int i = 0; i < rds.Length; ++i)
        {
            if (rds[i] != newSkinMeshRender) mts.Add(rds[i].material);
        }
        return mts.ToArray();
    }

    private void SingleMaterial(SkinnedMeshRenderer newSkinMeshRender, 
        string[] texPropertyNames, 
        int width, 
        int height, 
        int uvCount, 
        Material newMaterial)
    {
        Texture2D t = CreateTexture(width, height);
        Rect[] rects = t.PackTextures(this.textureDics[texPropertyNames[0]].ToArray(), 0);

        newSkinMeshRender.sharedMesh.uv = ResetUV(rects, uvCount);
        newSkinMeshRender.material = newMaterial;

        newMaterial.SetTexture(texPropertyNames[0], t);

        int texLen = texPropertyNames.Length;
        for (int i = 1; i < texLen; ++i)
        {
            Texture2D texture = CustomPackageTexture(texPropertyNames[i], rects, t.width, t.height);
            newMaterial.SetTexture(texPropertyNames[i], texture);
        }
    }

    private void FindBones(Transform[] bones, SkinnedMeshRenderer smr)
    {
        if (bones == null)
        {
            boneList.AddRange(smr.bones);
            return;
        }

        int boneLen = bones.Length;
        int smrBoneLen = smr.bones.Length;

        for (int i = 0; i < smrBoneLen; ++i)
        {
            for (int j = 0; j < bones.Length; ++j)
            {
                if (bones[j].name != smr.bones[i].name) continue;
                boneList.Add(bones[j]);
            }
        }
    }
}