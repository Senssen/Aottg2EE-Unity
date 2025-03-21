using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MeshCombiner : MonoBehaviour
{
    // Function to combine all immediate child meshes
    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        int i = 0;

        // Loop through immediate children and combine meshes
        foreach (var meshFilter in meshFilters)
        {
            if (meshFilter.transform.parent == transform)
            {
                combineInstances[i].mesh = meshFilter.sharedMesh;
                combineInstances[i].transform = meshFilter.transform.localToWorldMatrix;
                i++;
            }
        }

        // Create a new mesh and combine all the mesh data into it
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances);

        // Add or get the MeshFilter and assign the combined mesh
        MeshFilter meshFilterComponent = gameObject.GetComponent<MeshFilter>();
        if (meshFilterComponent == null)
        {
            meshFilterComponent = gameObject.AddComponent<MeshFilter>();
        }
        meshFilterComponent.sharedMesh = combinedMesh;

        // Optionally, remove the mesh filters from the immediate children
        foreach (var meshFilter in meshFilters)
        {
            if (meshFilter.transform.parent == transform)
            {
                DestroyImmediate(meshFilter.gameObject);
            }
        }

        // Ensure the MeshRenderer is assigned on the parent
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
    }
}