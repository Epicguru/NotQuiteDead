
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioSauce))]
public class AudioSauceInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Play"))
        {
            var sauce = (AudioSauce)target;

            if (Camera.main != null)
            {
                sauce.Play(Camera.main.transform);
            }
            else
            {
                Debug.LogWarning("Cannot play audio, camera is null.");
            }
        }
    }
}