using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkBackground))]
public class ChunkBGRegeneration : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Regenerate"))
        {
            var cbg = (ChunkBackground)target;

            cbg.Regenerate();
        }

        if (GUILayout.Button("Update Positioning"))
        {
            var cbg = (ChunkBackground)target;

            cbg.SetFinalPosition();
        }

        if (GUILayout.Button("Create New T2D"))
        {
            var cbg = (ChunkBackground)target;

            cbg.CreateTexture2D();
        }
    }
}