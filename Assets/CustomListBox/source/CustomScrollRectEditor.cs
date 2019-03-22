using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomScrollRect))]
public class CustomScrollRectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CustomScrollRect component = (CustomScrollRect) target;

        base.OnInspectorGUI();

        component.dragPlaceHolder = (GameObject) EditorGUILayout.ObjectField("Drag PLaceholder",
            component.dragPlaceHolder, typeof(GameObject), true);

        component.demarcation = (GameObject)EditorGUILayout.ObjectField("Demarcation",
            component.demarcation, typeof(GameObject), true);
    }
}