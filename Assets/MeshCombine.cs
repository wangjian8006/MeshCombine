using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombine : BaseMeshCombine
{
    public void Combine(MeshRenderer newMeshRender,
        MeshRenderer[] needCombines,
        string[] texPropertyNames,
        Material newMaterial = null,
        int texWidth = -1,
        int texHeight = -1,
        bool hideRawMesh = true,
        bool transformMaterix = true)
    {
        combineInstances.Clear();
        uvList.Clear();
        InitTextureDics(texPropertyNames);

        int uvCount = 0;
        int meshLen = needCombines.Length;

        Matrix4x4 matrix = newMeshRender.transform.worldToLocalMatrix;

        for (int i = 0; i < meshLen; ++i)
        {
            MeshRenderer mr = needCombines[i];
            MeshFilter mf = mr.gameObject.GetComponent<MeshFilter>();
            if (mr == newMeshRender) continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            if (transformMaterix == true) ci.transform = matrix * mf.gameObject.transform.localToWorldMatrix;
            combineInstances.Add(ci);

            uvList.Add(mf.sharedMesh.uv);
            uvCount += mf.sharedMesh.uv.Length;

            Debug.Assert(mr.materials != null && mr.materials.Length == 1, "The materials error.");
            Debug.Assert(mr.material != null, "The material error");

            FindTexture(mr.material, texPropertyNames);

            if (hideRawMesh == true) mr.gameObject.SetActive(false);
        }

        MeshFilter newMeshFilter = newMeshRender.gameObject.GetComponent<MeshFilter>();
        if (newMeshFilter == null)
        {
            newMeshFilter = newMeshRender.gameObject.AddComponent<MeshFilter>();
        }
        newMeshFilter.mesh = new Mesh();
        newMeshFilter.mesh.CombineMeshes(combineInstances.ToArray(), true, true);

        if (texPropertyNames != null) SingleMaterial(newMeshFilter, newMeshRender, texPropertyNames, texWidth, texHeight, uvCount, newMaterial);
        else newMeshRender.materials = MuiltMaterial(newMeshRender, needCombines);
    }

    private Material[] MuiltMaterial(MeshRenderer newMeshRender, MeshRenderer[] rds)
    {
        List<Material> mts = new List<Material>();
        for (int i = 0; i < rds.Length; ++i)
        {
            if (rds[i] != newMeshRender) mts.Add(rds[i].material);
        }
        return mts.ToArray();
    }

    private void SingleMaterial(MeshFilter newMeshFilter,
        MeshRenderer newMeshRender,
        string[] texPropertyNames,
        int width,
        int height,
        int uvCount,
        Material newMaterial)
    {
        Texture2D t = CreateTexture(width, height);
        Rect[] rects = t.PackTextures(this.textureDics[texPropertyNames[0]].ToArray(), 0);

        newMeshFilter.mesh.uv = ResetUV(rects, uvCount);
        newMeshRender.material = newMaterial;

        newMaterial.SetTexture(texPropertyNames[0], t);

        int texLen = texPropertyNames.Length;
        for (int i = 1; i < texLen; ++i)
        {
            Texture2D texture = CustomPackageTexture(texPropertyNames[i], rects, t.width, t.height);
            newMaterial.SetTexture(texPropertyNames[i], texture);
        }
    }
}
