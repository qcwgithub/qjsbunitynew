using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using SharpKit.JavaScript;
using System.IO;

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

    public enum TraverseOp
    {
        CopyMonoBehaviour,
        RemoveOldBehaviour, 
        Analyze
    }

    public static void TraverseGameObject(StringBuilder sb, GameObject go, int tab, TraverseOp op)
    {
        for (var t = 0; t < tab; t++)
        {
            sb.Append("    ");
        }
        sb.Append(go.name + "   -->("+go.tag+")");

        switch (op)
        {
            case TraverseOp.CopyMonoBehaviour:
                ExtraHelper.CopyGameObject<JSComponent_SharpKit>(go);
                break;
            case TraverseOp.RemoveOldBehaviour:
                ExtraHelper.RemoveOtherMonoBehaviours(go);
                break;
            case TraverseOp.Analyze:
            default:
                break;
        }

        var coms = go.GetComponents(typeof(Component));
        bool hasProblem = false;
        for (var c = 0; c < coms.Length; c++)
        {
             sb.Append(coms[c].GetType().Name);
             if (c != coms.Length - 1)
             {
                 sb.Append(" | ");
             }

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
            TraverseGameObject(sb, child.gameObject, tab + 1, op);
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

    [MenuItem("JSB/Gen JsType file list")]
    public static void OutputAllTypesWithJsTypeAttribute()
    {
        var sb = new StringBuilder();
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types = a.GetTypes();
            foreach (Type t in types)
            {
                if (ExtraHelper.WillTypeBeTranslatedToJavaScript(t))
                {
                    sb.AppendFormat("CS.require(\"SharpKitGenerated/{0}\");\n", t.Name);
                }
            }
        }

        Debug.Log(sb);

        string path = JSBindingSettings.jsDir + "/includes.javascript";
        File.WriteAllText(path, sb.ToString());
        Debug.Log("OK. File: " + path);
        // AssetDatabase.Refresh();
    }

    // [MenuItem("JSB/Analyze current scene")]
    public static void AnalyzeCurrentScene()
    {
        initAnalyze();
        GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject go in gameObjects)
        {
            if (go.transform.root == go.transform)
            {
                TraverseGameObject(sbHierachy, go, 0, TraverseOp.Analyze);
            }
        }
        Debug.Log(sbHierachy);
    }
    static bool FileNameBeginsWithUnderscore(string path)
    {
        string shortName = path.Substring(Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\')) + 1);
        // 忽略以_开头的prefab
        return (shortName[0] == '_');
    }


    [MenuItem("JSB/One Key Replace All (Danger!)")]
    public static void OneKeyReplaceAll()
    {
        bool bContinue = EditorUtility.DisplayDialog("WARNING", 
            "This action may cause data loss. You must backup whole project before executing this action.", 
            "Continue", 
            "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return;
        }

        // first copy
        // then remove
        var ops = new TraverseOp[] { TraverseOp.CopyMonoBehaviour, TraverseOp.RemoveOldBehaviour};
        foreach (var op in ops)
        {
            IterateAllPrefabs(op);
            EditorApplication.SaveAssets();

            string[] GUIDs = AssetDatabase.FindAssets("t:Scene");
            foreach (var guid in GUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (FileNameBeginsWithUnderscore(path))
                {
                    continue;
                }
                // Debug.Log("Replace Scene: " + path);
                EditorApplication.OpenScene(path);
                IterateAllGameObjectsInTheScene(op);
                EditorApplication.SaveScene();
            }
        }
        Debug.Log("Replace all OK.");
        AssetDatabase.Refresh();
    }

    // [MenuItem("JSB/Replace MonoBehaviours of all scenes")]
    public static void IterateAllGameObjectsInTheScene(TraverseOp op)
    {
        initAnalyze();
        GameObject[] gameObjects = GameObject.FindObjectsOfType <GameObject>();
        foreach (GameObject go in gameObjects)
        {
            if (go.transform.root == go.transform)
            {
                TraverseGameObject(sbHierachy, go, 0, op);
                //sbHierachy.Append("\n");
            }
        }
        Debug.Log(sbHierachy);
    }

    // [MenuItem("JSB/Replace MonoBehaviours of all prefabs")]
    public static void IterateAllPrefabs(TraverseOp op)
    {
        initAnalyze();
        string[] GUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in GUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (FileNameBeginsWithUnderscore(path))
            {
                continue;
            }

            UnityEngine.Object mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
            if (mainAsset is GameObject)
            {
                TraverseGameObject(sbHierachy, (GameObject)mainAsset, 1, op);
            }
            sbHierachy.Append("\n");
        }
        Debug.Log(sbHierachy);
    }
    // Alt + Shift + Q
    //[MenuItem("JSB/Copy GameObject MonoBehaviours &#q")]
    public static void CopyGameObjectMonoBehaviours()
    {
        Debug.Log("CopyGameObjectMonoBehaviours");
        GameObject go = Selection.activeGameObject;
        ExtraHelper.CopyGameObject<JSComponent_SharpKit>(go);
    }
    // Alt + Shift + W
    //[MenuItem("JSB/Remove Other MonoBehaviours &#w")]
    public static void RemoveOtherMonoBehaviours()
    {
        Debug.Log("RemoveOtherMonoBehaviours");
        GameObject go = Selection.activeGameObject;
        ExtraHelper.RemoveOtherMonoBehaviours(go);
    }

    public static string MyMatchEvaluator(Match m)
    {
        matched = true;
        var sb = new StringBuilder();
        sb.AppendFormat("\n[JsType(JsMode.Clr,\"../StreamingAssets/JavaScript/SharpKitGenerated/{0}{1}.javascript\")]\n{2}", ""/*nextPath*/, m.Groups["ClassName"], m.Groups["ClassDefinition"]);
        return sb.ToString();
    }

    // 包含/
    static string nextPath = string.Empty;
    static bool matched = false;

    [MenuItem("JSB/Add JsType Attribute for all files in Src Folder(Beta)")]
    public static void MakeJsTypeAttributeInSrc()
    {
        string srcFolder = Application.dataPath + "/Src";
        string[] files = Directory.GetFiles(srcFolder, "*.cs", SearchOption.AllDirectories);
        foreach (var f in files)
        {
            var path = f.Replace('\\', '/');

            matched = false;
            nextPath = path.Substring(srcFolder.Length + 1, path.LastIndexOf('/') - srcFolder.Length);

            string content = File.ReadAllText(path);
            var reg = new Regex(@"(?>^\s*\[\s*JsType.*$)?\s*(?<ClassDefinition>^(?>(?>public|protected|private|static|partial|abstract|internal)*\s*)*(?>class|struct)\s+(?<ClassName>\w+)\s*(?::\s*\w+\s*(?:\,\s*\w+)*)?\s*\{)", RegexOptions.Multiline);
            content = reg.Replace(content, MyMatchEvaluator);

            if (matched && content.IndexOf("using SharpKit.JavaScript;") < 0)
            {
                content = "using SharpKit.JavaScript;\n" + content;
            }
            File.WriteAllText(path, content);
        }
        Debug.Log("Make JsType Attribute OK.");
        AssetDatabase.Refresh();
    }
}
