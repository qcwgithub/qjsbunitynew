using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SharpKit.JavaScript;

public static class JSAnalyzer
{
    static StringBuilder sbHierachy = new StringBuilder();
    static Dictionary<string, List<string>> dictProblem;
    static Dictionary<Type, bool> dictExport = new Dictionary<Type,bool>();


    static List<string> ExamComponent(Component com)
    {
        List<string> lstProblem = new List<string>();
        if (com is MonoBehaviour)
        {
            StringBuilder sbProblem = new StringBuilder();
            MonoBehaviour behaviour = com as MonoBehaviour;
            Type type = behaviour.GetType();
            FieldInfo[] fields = ExtraHelper.GetMonoBehaviourSerializedFields(behaviour);
            for (var i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                Type elementType = field.FieldType;
                if (elementType.IsGenericType)
                {
                    lstProblem.Add("[" + com.GetType().Name + "] " + field.Name + " => " + elementType.Name + " not supported.");
                    continue;
                }
                bool bArray = elementType.IsArray;
                if (bArray)
                {
                    elementType = elementType.GetElementType();
                }

                if (elementType.IsPrimitive || 
                    elementType.IsEnum || 
                    elementType == typeof(string) ||
                    elementType == typeof(bool) ||
                    elementType == typeof(Vector2) ||
                    elementType == typeof(Vector3))
                {

                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(elementType))
                {
                    // if this is a monobehaviour
                    if (typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(elementType))
                    {
                        bool supported = false;
                        // this class exists in both c# and js?
                        if (dictExport.ContainsKey(elementType))
                        {
                            supported = true;
                        }
                        else
                        {
                            // this class only exists in js?
                            foreach (var attr in elementType.GetCustomAttributes(false))
                            {
                                if (attr is JsTypeAttribute)
                                {
                                    supported = true;
                                    break;
                                }
                            }
                        }
                        if (!supported)
                        {
                            lstProblem.Add("[" + com.GetType().Name + "] " + field.Name + " => " + elementType.Name + " not exported!");
                        }
                    }
                }
                else if (elementType.IsValueType)
                {
                    lstProblem.Add("[" + com.GetType().Name + "] " + field.Name + " => " + elementType.Name  + ": struct not supported!");
                }
                else
                {
                    lstProblem.Add("[" + com.GetType().Name + "] " + field.Name + " => " + elementType.Name + ": Unknown type!");
                }
            }
        }
        return lstProblem;
    }

    public static void TraverseGameObject(StringBuilder sb, GameObject go, int tab)
    {
        for (var t = 0; t < tab; t++)
        {
            sb.Append("    ");
        }
        sb.Append(go.name + "   -->("+go.tag+")");


        ExtraHelper.CopyGameObject<JSComponent_SharpKit>(go);
        ExtraHelper.RemoveOtherMonoBehaviours(go);

        var coms = go.GetComponents(typeof(Component));
        bool hasProblem = false;
        for (var c = 0; c < coms.Length; c++)
        {
//             sb.Append(coms[c].GetType().Name);
//             if (c != coms.Length - 1)
//             {
//                 sb.Append(" | ");
//             }

            List<string> lstError = ExamComponent(coms[c]);
            for (var x = 0; x < lstError.Count; x++ )
            {
                hasProblem = true;
                sb.Append("\n");
                for (var t = 0; t < tab + 1; t++)
                {
                    sb.Append("    ");
                }
                sb.Append(lstError[x]);
            }
        }
        sb.Append("\n");

        var childCount = go.transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            TraverseGameObject(sb, child.gameObject, tab + 1);
        }
    }

    static void initAnalyze()
    {
        dictExport.Clear();
        foreach (var type in JSBindingSettings.classes)
        {
            if (!dictExport.ContainsKey(type))
            {
                dictExport.Add(type, true);
            }
        }
        sbHierachy.Remove(0, sbHierachy.Length);
    }

    [MenuItem("JSB/Iterate All GameObjects In the Scene")]
    public static void IterateAllGameObjectsInTheScene()
    {
        initAnalyze();
        GameObject[] gameObjects = GameObject.FindObjectsOfType <GameObject>();
        foreach (GameObject go in gameObjects)
        {
            if (go.transform.root == go.transform)
            {
                TraverseGameObject(sbHierachy, go, 0);
                //sbHierachy.Append("\n");
            }
        }
        Debug.Log(sbHierachy);
    }

    [MenuItem("JSB/Iterate All Prefabs")]
    public static void IterateAllPrefabs()
    {
        initAnalyze();
        string[] GUIDs = AssetDatabase.FindAssets("t:prefab");
        foreach (var guid in GUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Object mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
            if (mainAsset is GameObject)
            {
                TraverseGameObject(sbHierachy, (GameObject)mainAsset, 1);
            }
            sbHierachy.Append("\n");
        }
        Debug.Log(sbHierachy);
    }
    // Alt + Shift + Q
    [MenuItem("JSB/Copy GameObject MonoBehaviours &#q")]
    public static void CopyGameObjectMonoBehaviours()
    {
        Debug.Log("CopyGameObjectMonoBehaviours");
        GameObject go = Selection.activeGameObject;
        ExtraHelper.CopyGameObject<JSComponent_SharpKit>(go);
    }
    // Alt + Shift + W
    [MenuItem("JSB/Remove Other MonoBehaviours &#w")]
    public static void RemoveOtherMonoBehaviours()
    {
        Debug.Log("RemoveOtherMonoBehaviours");
        GameObject go = Selection.activeGameObject;
        ExtraHelper.RemoveOtherMonoBehaviours(go);
    }
}
