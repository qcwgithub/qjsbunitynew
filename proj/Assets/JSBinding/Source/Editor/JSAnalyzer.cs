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
            FieldInfo[] fields = JSSerializerEditor.GetMonoBehaviourSerializedFields(behaviour);
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

        // disconnect!!
        if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
            PrefabUtility.DisconnectPrefabInstance(go);

        switch (op)
        {
            case TraverseOp.CopyMonoBehaviour:
                JSSerializerEditor.CopyGameObject<JSComponent>(go);
                break;
            case TraverseOp.RemoveOldBehaviour:
                JSSerializerEditor.RemoveOtherMonoBehaviours(go);
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

    [MenuItem("JSB/Tool/Gen JsType file list")]
    public static void OutputAllTypesWithJsTypeAttribute()
    {
        var sb = new StringBuilder();
        sb.Append(@"/* Generated by JSBinding.
* see JSAnalyzer.cs / OutputAllTypesWithJsTypeAttribute() function
* better not modify manually.
*/

");
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types = a.GetTypes();
            foreach (Type t in types)
            {
                if (JSSerializerEditor.WillTypeBeTranslatedToJavaScript(t))
                {
                    System.Object[] attrs = t.GetCustomAttributes(typeof(JsTypeAttribute), false);
                    JsTypeAttribute jsTypeAttr = (JsTypeAttribute)attrs[0];
                    if (jsTypeAttr.Filename != null)
                    {
                        //Debug.Log(jsTypeAttr.filename);

                        string mustBegin = "StreamingAssets/JavaScript/";
                        //string mustBegin = JSBindingSettings.sharpKitGenFileDir;
                        int index = 0;
                        if ((index = jsTypeAttr.Filename.IndexOf(mustBegin)) >= 0)
                        {
                            sb.AppendFormat("CS.require(\"{0}\");\n", jsTypeAttr.Filename.Substring(index + mustBegin.Length));
                        }
                        else
                        {
                            Debug.LogError(JSNameMgr.GetTypeFullName(t) + " is ignored because JsType.filename doesn't contain \"" + mustBegin + "\"");
                        }
                    }
                }
            }
        }

        Debug.Log(sb);

        string path = JSBindingSettings.sharpkitGeneratedFiles;
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

    public static JSEngine FindJSEngine()
    {
		string path = "Assets/JSBinding/Prefabs/_JSEngine.prefab";
		GameObject goEngine = (GameObject)AssetDatabase.LoadAssetAtPath (path, typeof(GameObject));
        //GameObject goEngine = GameObject.Find("_JSEngine");
        JSEngine jsEngine = goEngine != null ? goEngine.GetComponent<JSEngine>() : null;
        if (jsEngine == null)
        {
			Debug.LogError(path + " not found.");
		}
		return jsEngine;
    }

    // delegate return true: not to replace
    public delegate bool DelFilterReplaceFile(string fullpath);

    [MenuItem("JSB/Replace all monos for all prefabs and scenes")]
    public static void ReplaceAllMonos()
    {
        JSEngine jsEngine = FindJSEngine();
        if (jsEngine == null)
        {
            return;
        }

        /////////////////

        bool bContinue = EditorUtility.DisplayDialog("WARNING",
            @"1) This action may cause data loss. You better backup whole project before executing this action.
2) Make sure you have made proper settings to JSEngine.DirectoriesNotToReplace fields before this action.
3) Scenes and prefabs whose names begin with '_' will be skipped.", 
            "Continue", 
            "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return;
        }

        DelFilterReplaceFile filter = (path) => 
        {
            // path begins witn Assets/
            var subPath = path.Substring("Assets/".Length);

            // Skip paths in jsEngine.DirectoriesNotToReplace
            foreach (var p in jsEngine.DirectoriesNotToReplace)
            {
                if (subPath.IndexOf(p) == 0)
                    return true;
            }
            // Skip underscore
            if (FileNameBeginsWithUnderscore(path))
                return true;

            return false;
        };

        var lstPrefabs = GetAllPrefabPaths(filter);
        var lstScenes = GetAllScenePaths(filter);

        var sb = new StringBuilder();
        foreach (var p in lstPrefabs)
        {
            sb.Append(p.Substring("Assets/".Length) + "\r\n");
        }
        foreach (var p in lstScenes)
        {
            sb.Append(p.Substring("Assets/".Length) + "\r\n");
        }
        File.WriteAllText(Application.dataPath + "/Replace.txt", sb.ToString());
        bContinue = EditorUtility.DisplayDialog("TIP",
             "Files list are in " + Application.dataPath + "/Replace.txt. please verify.", 
             "OK",
             "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return;
        }

        initAnalyze();

        // first copy
        // then remove
        var ops = new TraverseOp[] { TraverseOp.CopyMonoBehaviour, TraverseOp.RemoveOldBehaviour};
        foreach (var op in ops)
        {
            foreach (var p in lstScenes)
            {
                EditorApplication.OpenScene(p);
                IterateAllGameObjectsInTheScene(op);
                EditorApplication.SaveScene();
            }

            foreach (var p in lstPrefabs)
            {
                UnityEngine.Object mainAsset = AssetDatabase.LoadMainAssetAtPath(p);
                if (mainAsset is GameObject)
                {
                    TraverseGameObject(sbHierachy, (GameObject)mainAsset, 1, op);
                }
            }
            EditorApplication.SaveAssets();
        }
        Debug.Log("Replace OK.");
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
    // path begins with Assets/
    public static List<string> GetAllScenePaths(DelFilterReplaceFile filter)
    {
        var lst = new List<string>();

        string[] GUIDs = AssetDatabase.FindAssets("t:Scene");
        foreach (var guid in GUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (filter(path))
            {
                continue;
            }

            lst.Add(path);
        }
        return lst;
    }
    // path begins with Assets/
    public static List<string> GetAllPrefabPaths(DelFilterReplaceFile filter)
    {
        var lst = new List<string>();

        string[] GUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in GUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (filter(path))
            {
                continue;
            }

            lst.Add(path);
        }
        return lst;
    }

    // [MenuItem("JSB/Replace MonoBehaviours of all prefabs")]
    public static void IterateAllPrefabs(TraverseOp op, DelFilterReplaceFile filter)
    {
        initAnalyze();
        string[] GUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in GUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (filter(path))
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
    [MenuItem("JSB/Copy GameObjects MonoBehaviours &#q")]
    public static void CopyGameObjectMonoBehaviours()
    {
        Debug.Log("CopyGameObjectMonoBehaviours");
        foreach(var go in Selection.gameObjects)
            JSSerializerEditor.CopyGameObject<JSComponent>(go);
    }
    // Alt + Shift + W
    [MenuItem("JSB/Remove GameObjects Other MonoBehaviours &#w")]
    public static void RemoveOtherMonoBehaviours()
    {
        Debug.Log("RemoveOtherMonoBehaviours");
        foreach (var go in Selection.gameObjects)
            JSSerializerEditor.RemoveOtherMonoBehaviours(go);
    }

    public static string MyMatchEvaluator(Match m)
    {
        matched = true;
        var sb = new StringBuilder();
        if (addjstype)
        {
            var lastDir = "";
            {
                // add "../../...." to reach Assets/
                int i = 0;
                var np = nextPath;
                while (true)
                {
                    i = np.IndexOf('/', i);
                    if (i < 0) break;
                    lastDir += "../";
                    i++;
                }
            }

            sb.AppendFormat("\n[JsType(JsMode.Clr,\"{0}{1}{2}{3}{4}\")]\n{5}",
                lastDir, JSBindingSettings.sharpKitGenFileDir, nextPath, m.Groups["ClassName"], JSBindingSettings.jsExtension, 
                m.Groups["ClassDefinition"]);
        }
        else
        {
            sb.AppendFormat("\n{0}", m.Groups["ClassDefinition"]);
        }
        return sb.ToString();
    }

    // 包含/
    // e.g. E:/code/qjsbunitynew/proj/Assets/Src/Tween_Scripts/TestHighConcurrencyGroup.cs -> Tween_Scripts/
    // 获得相对于 Src 的目录
    static string nextPath = string.Empty;
    static bool matched = false;
    static bool addjstype = true;

    [MenuItem("JSB/Tool/Delete JsType Attribute for all structs and classes")]
    public static void DelJsTypeAttributeInSrc()
    {
        addjstype = false;
        MakeJsTypeAttributeInSrc();
    }
    [MenuItem("JSB/Tool/Add JsType Attribute for all structs and classes")]
    public static void AddJsTypeAttributeInSrc()
    {
        addjstype = true;
        MakeJsTypeAttributeInSrc();
    }
    public static void MakeJsTypeAttributeInSrc()
    {
        JSEngine jsEngine = FindJSEngine();
        if (jsEngine == null)
        {
            return;
        }

        bool bContinue = EditorUtility.DisplayDialog("Tip",
@"Make sure you have made proper settings to 

JSEngine.DirectoriesNotToExport 
JSEngine.DirectoriesToExport 

fields before this action.",
            "Continue",
            "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return;
        }

        var sb = new StringBuilder();
        //sb.Append("files to handle:\n-----------------------------------------\n");
        string srcFolder = Application.dataPath.Replace('\\', '/');

        // Get all cs files in the project
        string[] files = Directory.GetFiles(srcFolder, "*.cs", SearchOption.AllDirectories);
        List<string> lstFiles = new List<string>();

        // filter files
        foreach (var f in files)
        {
            var path = f.Replace('\\', '/');
            var subPath = path.Substring(srcFolder.Length + 1);
            bool export = true;
            foreach (string dir in jsEngine.DirectoriesNotToExport)
            {
                if (subPath.IndexOf(dir) == 0)
                {
                    export = false;
                    break;
                }
            }
            if (!export && jsEngine.DirectoriesToExport != null)
            {
                foreach (string dir in jsEngine.DirectoriesToExport)
                {
                    if (subPath.IndexOf(dir) == 0)
                    {
                        export = true;
                        break;
                    }
                }
            }
            if (export)
            {
                sb.Append(subPath + "\r\n");
                lstFiles.Add(path);
            }
        }

        File.WriteAllText(Application.dataPath + "/FilesToAddJsType.txt", sb.ToString());
        bContinue = EditorUtility.DisplayDialog("TIP",
             "Files list are in " + Application.dataPath + "/FilesToAddJsType.txt. please verify.",
             "OK",
             "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return;
        }

        // path in lstFiles has full path
        foreach (string path in lstFiles)
        {
            matched = false;
            // 
			// "E:/code/qjsbunitynew/proj/Assets/JSBinding/Samples/Serialization/SerializeSimple.cs"
			// ->
            // "JSBinding/Samples/Serialization/"
            nextPath = path.Substring(srcFolder.Length + 1, path.LastIndexOf('/') - srcFolder.Length);
            //Debug.Log(path + " -> " + nextPath);

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
