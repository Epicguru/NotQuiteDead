using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightMesh))]
public class LightMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var lm = (LightMesh)target;

        if (GUILayout.Button("Regenerate"))
        {
            lm.Gen.GenMesh();
        }

        if (GUILayout.Button("Run Colour Set"))
        {
            lm.RunColourSet();
        }

        if (GUILayout.Button("Run Edge Set"))
        {
            lm.RunEdgeSet();
        }
    }
}