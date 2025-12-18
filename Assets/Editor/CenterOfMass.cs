/* using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rigidbody))]
public class CenterOfMass : Editor
{
    void OnSceneGUI()
    {
        Rigidbody rb = (Rigidbody)target;
        Handles.color = Color.red;
        Handles.SphereHandleCap(
            0,
            rb.transform.TransformPoint(rb.centerOfMass),
            rb.transform.rotation,
            .5f,
            EventType.Repaint
        );
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
} */