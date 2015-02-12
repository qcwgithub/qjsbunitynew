using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(JSEngine))]
public class JSEngineInspector : Editor
{
    public override void OnInspectorGUI()
    {
		serializedObject.Update ();

        SerializedProperty propDebug = serializedObject.FindProperty("debug");
        EditorGUILayout.PropertyField(propDebug);

        // JSEngine je = target as JSEngine;

        if (propDebug.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("port"));
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("GCInterval"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("jsLoader"));

        serializedObject.ApplyModifiedProperties();
    }
}