using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using EzySlice;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombine : MonoBehaviour
{
    public bool isBaseFixed;
    public int hitCount;

    private void Start()
    {
        if (transform.childCount == 0)
            return;

        MeshFilter[] filterList = GetComponentsInChildren<MeshFilter>();
        if (filterList.Length == 1)
            return;

        Vector3 position = transform.position;
        transform.position = Vector3.zero;

        List<MeshFilter> meshFilters = new List<MeshFilter>();
        for (int j = 1; j < filterList.Length; j++)
        {
            meshFilters.Add(filterList[j]);
        }


        CombineInstance[] combine = new CombineInstance[meshFilters.Count];

        int i = 0;
        while (i < meshFilters.Count)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
        transform.gameObject.SetActive(true);

        transform.position = position;
        transform.localScale = Vector3.one;
    }

}