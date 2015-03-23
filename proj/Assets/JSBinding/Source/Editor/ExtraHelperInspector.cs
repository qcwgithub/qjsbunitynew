using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ExtraHelper))]
public class ExtraHelperInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
