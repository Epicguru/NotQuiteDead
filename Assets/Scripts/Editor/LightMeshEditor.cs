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

        if (GUILayout.Button("Apply Ambient Light"))
        {
            lm.Chunk.ApplyAmbientLight();
        }

        if (GUILayout.Button("Apply Pending"))
        {
            lm.Interaction.Apply();
        }
    }
}