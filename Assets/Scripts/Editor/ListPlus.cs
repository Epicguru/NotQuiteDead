using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(Backgrounds))]
public class ListPlus : Editor
{
    private ReorderableList list;

    public void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("loaded"), true, true, true, true);

        list.drawHeaderCallback = rect => {
            EditorGUI.LabelField(rect, "Sorted Backgrounds", EditorStyles.boldLabel);
        };

        list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}