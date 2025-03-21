using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshCombiner meshCombiner = (MeshCombiner)target;

        if (GUILayout.Button("Combine Meshes"))
        {
            // This will call the CombineMeshes function when the button is clicked
            meshCombiner.CombineMeshes();
        }
    }
}
