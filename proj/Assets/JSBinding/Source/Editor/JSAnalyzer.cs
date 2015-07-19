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
    /// <summary>
    /// Exams the component to see if there is something not supported.
    /// Currently check List only.
    /// </summary>
    /// <param name="com">The component.</param>
    /// <returns>An error list.</returns>
    static List<string> ExamMonoBehaviour(MonoBehaviour com)
    {
        List<string> lstProblem = new List<string>();
        StringBuilder sbProblem = new StringBuilder();
        MonoBehaviour behaviour = com as MonoBehaviour;
        Type type = behaviour.GetType();
        FieldInfo[] fields = JSSerializerEditor.GetMonoBehaviourSerializedFields(behaviour);
        for (var i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            Type fieldType = field.FieldType;
            if (fieldType.IsArray)
            {
                fieldType = fieldType.GetElementType();
            }
            // ! List is not supported
            // 
            if (fieldType.IsGenericType)
            {
                lstProblem.Add(new StringBuilder().AppendFormat("{0} {1}.{2} serialization not supported.", fieldType.Name, com.GetType().Name, field.Name).ToString());
                //continue;
            }

            // if this MonoBehaviour refer to another MonoBehaviour (A)
            // A must be export or compiled to JavaScript as well
            if (typeof(MonoBehaviour).IsAssignableFrom(fieldType))
            {
                if (!JSSerializerEditor.WillTypeBeAvailableInJavaScript(fieldType))
                {
                    lstProblem.Add(new StringBuilder().AppendFormat("{0} {1}.{2} not available in JavaScript.", fieldType.Name, com.GetType().Name, field.Name).ToString());
                }
            }

            //more to exam
        }
        return lstProblem;
    }

    public enum TraverseOp
    {
        CopyMonoBehaviour,
        RemoveOldBehaviour, 
        Analyze
    }
    static string GetTempFileNameFullPath(string shortPath)
    {
        Directory.CreateDirectory(Application.dataPath + "/Temp/");
        return Application.dataPath + "/Temp/" + shortPath;
    }

    /// <summary>
    /// Do some actions to GameObject hierachy.
    /// </summary>
    /// <param name="sbLog">The log.</param>
    /// <param name="go">The gameobject.</param>
    /// <param name="tab">The tab.</param>
    /// <param name="op">The operation.</param>
    public static void TraverseGameObject(StringBuilder sbLog, GameObject go, int tab, TraverseOp op)
    {
        for (var t = 0; t < tab; t++)
        {
            sbLog.Append("    ");
        }
        sbLog.AppendFormat("{0}", go.name);

        bool hasChecked = false;
        bool hasReplaced = false;
        bool hasError = false;

        // disconnect prefab instance !!
        if ((op == TraverseOp.CopyMonoBehaviour || op == TraverseOp.RemoveOldBehaviour) 
            && PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
        {
            PrefabUtility.DisconnectPrefabInstance(go);
        }

        // action!
        switch (op)
        {
            case TraverseOp.CopyMonoBehaviour:
                {
                    bool bReplaced = JSSerializerEditor.CopyGameObject<JSComponent>(go);
                    if (bReplaced && !hasReplaced)
                    {
                        hasReplaced = true;
                        sbLog.Append(" (REPLACED)");
                    }
                }
                break;
            case TraverseOp.RemoveOldBehaviour:
                {
                    JSSerializerEditor.RemoveOtherMonoBehaviours(go);
                }
                break;
            case TraverseOp.Analyze:
                {
                    var coms = go.GetComponents(typeof(MonoBehaviour));

                    // Calculate MonoBehaviour's Count
                    // Only check scripts that has JsType attribute
                    Dictionary<Type, int> dictMono = new Dictionary<Type, int>();
                    for (var c = 0; c < coms.Length; c++)
                    {
                        MonoBehaviour mb = (MonoBehaviour)coms[c];
                        if (JSSerializerEditor.WillTypeBeTranslatedToJavaScript(mb.GetType()))
                        {
                            if (!dictMono.ContainsKey(mb.GetType()))
                                dictMono.Add(mb.GetType(), 1);
                            else
                                dictMono[mb.GetType()]++;
                        }
                    }
                    foreach (var t in dictMono)
                    {
                        if (!hasChecked)
                        {
                            hasChecked = true;
                            sbLog.Append(" (CHECKED)");
                        }

                        if (t.Value > 1)
                        {
                            if (!hasError) { hasError = true;  sbLog.Append(" ERROR: "); }
                            sbLog.AppendFormat("Same MonoBehaviour more than once. Name: {0}, Count: {1} ", t.Key.Name, t.Value);
                        }
                    }

                    for (var c = 0; c < coms.Length; c++)
                    {
                        MonoBehaviour mb = (MonoBehaviour)coms[c];
                        if (JSSerializerEditor.WillTypeBeTranslatedToJavaScript(mb.GetType()))
                        {
                            List<string> lstError = ExamMonoBehaviour(mb);
                            for (var x = 0; x < lstError.Count; x++)
                            {
                                if (!hasError) { hasError = true; sbLog.Append(" ERROR: "); }
                                sbLog.Append(lstError[x] + " ");
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
        sbLog.Append("\n");

        // traverse children
        var childCount = go.transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            TraverseGameObject(sbLog, child.gameObject, tab + 1, op);
        }
    }

    /// <summary>
    /// Find all types in whole application
    /// if type has JsType attribute, output a 'CS.require' line to require the file containing the type
    /// </summary>
    [MenuItem("JSB/Generate SharpKit JsType file CS.require list", false, 53)]
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

    /// <summary>
    /// Does file path end with underscore?
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns></returns>
    static bool FileNameBeginsWithUnderscore(string path)
    {
        string shortName = path.Substring(Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\')) + 1);
        return (shortName[0] == '_');
    }

    /// <summary>
    /// Iterates all scenes and all prefabs in the project.
    /// Checks all MonoBehaviours who has JsType attribute.
    /// Save current scene before this action.
    /// 3 things will be checked:
    /// 1) Did you bind a MonoBehaviour(with JsType attribute) to a GameObject twice or more? (Support only one)
    /// 2) Did your MonoBehaviour(with JsType attribute) refer to other MonoBehaviour that is not available in JavaScript?
    /// 3) Did your MonoBehaviour(with JsType attribute) have not-supported public fields? (List, for example)
    /// </summary>
    [MenuItem("JSB/Check All Monos for all Prefabs and Scenes", false, 112)]
    public static void CheckAllMonos()
    {
        bool bContinue = EditorUtility.DisplayDialog("WARNING",
            @"1) You mush save current scene before this action.
2) Make proper settings to 'JSBindingSettings.PathsNotToCheckOrReplace' field before this action.
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

            // Skip paths in JSBindingSettings.PathsNotToCheckOrReplace
            foreach (var p in JSBindingSettings.PathsNotToCheckOrReplace)
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
        string fileName = GetTempFileNameFullPath("CheckList.txt");
        File.WriteAllText(fileName, sb.ToString());
        bContinue = EditorUtility.DisplayDialog("TIP",
             "Files list are in " + fileName + ". please verify.",
             "OK",
             "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return;
        }

        StringBuilder sbCheckLog = new StringBuilder();
        sbCheckLog.Append(@"// Usage
// search 'ERROR' to see if any error occurs.
// search 'CHECKED' to see whether a GameObject has been checked or not. (a GameObject will be checked if he has a MonoBehaviour with JsType attribute)

");

        var ops = new TraverseOp[] { TraverseOp.Analyze };
        foreach (var op in ops)
        {
            foreach (var p in lstScenes)
            {
                StringBuilder sbLog = new StringBuilder();
                sbLog.AppendFormat("FILE: {0}\n", p);

                EditorApplication.OpenScene(p);
                foreach (var go in SceneRoots())
                {
                    TraverseGameObject(sbLog, go, 1, op);
                }
                EditorApplication.SaveScene();

                sbCheckLog.Append(sbLog + "\n");
            }

            foreach (var p in lstPrefabs)
            {
                UnityEngine.Object mainAsset = AssetDatabase.LoadMainAssetAtPath(p);
                if (mainAsset is GameObject)
                {
                    StringBuilder sbLog = new StringBuilder();
                    sbLog.AppendFormat("FILE: {0}\n", p);

                    TraverseGameObject(sbLog, (GameObject)mainAsset, 1, op);
                    sbCheckLog.Append(sbLog + "\n");
                }
            }
            EditorApplication.SaveAssets();
        }
        fileName = GetTempFileNameFullPath("CheckResult.txt");
        File.WriteAllText(fileName, sbCheckLog.ToString());
        Debug.Log("Check finished. Output file: " + fileName);
        AssetDatabase.Refresh();
    }

    // delegate return true: not to replace
    public delegate bool DelFilterReplaceFile(string fullpath);

    /// <summary>
    /// Iterates all scenes and all prefabs in the project.
    /// Replaces all MonoBehaviours who has JsType attribute with JSComponent!
    /// Care muse be taken when executing this menu.
    /// </summary>
    [MenuItem("JSB/Replace All Monos for all Prefabs and Scenes", false, 113)]
    public static void ReplaceAllMonos()
    {
        bool bContinue = EditorUtility.DisplayDialog("WARNING",
            @"1) This action may cause data loss. You better save current scene and backup whole project before executing this action.
2) Make proper settings to 'JSBindingSettings.PathsNotToCheckOrReplace' field before this action.
3) Execute 'JSB | Check All Monos for all Prefabs and Scenes' menu before this action.
4) Scenes and prefabs whose names begin with '_' will be skipped.", 
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

            // Skip paths in JSBindingSettings.PathsNotToCheckOrReplace
            foreach (var p in JSBindingSettings.PathsNotToCheckOrReplace)
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
        string fileName = GetTempFileNameFullPath("FilesToReplace.txt");
        File.WriteAllText(fileName, sb.ToString());
        bContinue = EditorUtility.DisplayDialog("TIP",
             "Files list are in " + fileName + ". please verify.", 
             "OK",
             "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return;
        }

        StringBuilder sbCheckLog = new StringBuilder();
        sbCheckLog.Append(@"// Usage
// search 'REPLACED' to see whether a GameObject has been replace or not.

");

        // first copy
        // then remove
        var ops = new TraverseOp[] { TraverseOp.CopyMonoBehaviour, TraverseOp.RemoveOldBehaviour};
        foreach (var op in ops)
        {
            foreach (var p in lstScenes)
            {
                StringBuilder sbLog = new StringBuilder();
                sbLog.AppendFormat("FILE: {0}\n", p);

                EditorApplication.OpenScene(p);
                foreach (var go in SceneRoots())
                {
                    TraverseGameObject(sbLog, go, 0, op);
                }
                EditorApplication.SaveScene();
                sbCheckLog.Append(sbLog + "\n");
            }

            foreach (var p in lstPrefabs)
            {
                StringBuilder sbLog = new StringBuilder();
                sbLog.AppendFormat("FILE: {0}\n", p);

                UnityEngine.Object mainAsset = AssetDatabase.LoadMainAssetAtPath(p);
                if (mainAsset is GameObject)
                {
                    TraverseGameObject(sbLog, (GameObject)mainAsset, 1, op);
                    sbCheckLog.Append(sbLog + "\n");
                }
            }
            EditorApplication.SaveAssets();
        }
        fileName = GetTempFileNameFullPath("ReplaceResult.txt");
        File.WriteAllText(fileName, sbCheckLog.ToString());
        Debug.Log("Replace finished. Output file: " + fileName);
        AssetDatabase.Refresh();
    }
    public static IEnumerable<GameObject> SceneRoots()
    {
        var prop = new HierarchyProperty(HierarchyType.GameObjects);
        var expanded = new int[0];
        while (prop.Next(expanded))
        {
            yield return prop.pptrValue as GameObject;
        }
    }
//     [MenuItem("JSB/Test/Output all GameObject names in current scene", false, 1001)]
//     public static void OutputAllGameObjectNamesInCurrentScene()
//     {
//         foreach (var root in SceneRoots())
//         {
//             Debug.Log(root.name);
//         }
//     }

    /// <summary>
    /// Iterates all game objects in the scene.
    /// NOTE !!! this function can ONLY deal with ACTIVE root GameObjects and their children, inactive root GameObjects and their children will be omitted!
    /// </summary>
    /// <param name="op">The operation.</param>
//     public static void IterateAllGameObjectsInTheScene(TraverseOp op)
//     {
//         initAnalyze();
//         foreach (var go in SceneRoots())
//         {
//             TraverseGameObject(sbHierachy, go, 0, op);
//         }
//         Debug.Log(sbHierachy);
//     }
    /// <summary>
    /// Gets all scene paths.
    /// path begins with 'Assets/'
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns></returns>
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
    /// <summary>
    /// Gets all prefab paths.
    /// path begins with 'Assets/'
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns></returns>
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
//     public static void IterateAllPrefabs(TraverseOp op, DelFilterReplaceFile filter)
//     {
//         initAnalyze();
//         string[] GUIDs = AssetDatabase.FindAssets("t:Prefab");
//         foreach (var guid in GUIDs)
//         {
//             string path = AssetDatabase.GUIDToAssetPath(guid);
//             if (filter(path))
//             {
//                 continue;
//             }
// 
//             UnityEngine.Object mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
//             if (mainAsset is GameObject)
//             {
//                 TraverseGameObject(sbHierachy, (GameObject)mainAsset, 1, op);
//             }
//             sbHierachy.Append("\n");
//         }
//         Debug.Log(sbHierachy);
//     }
    // Alt + Shift + Q
    [MenuItem("JSB/Copy Selected GameObjects MonoBehaviours &#q", false, 110)]
    public static void CopyGameObjectMonoBehaviours()
    {
        Debug.Log("CopyGameObjectMonoBehaviours");
        foreach(var go in Selection.gameObjects)
            JSSerializerEditor.CopyGameObject<JSComponent>(go);
    }
    // Alt + Shift + W
    [MenuItem("JSB/Remove Selected GameObjects Other MonoBehaviours &#w", false, 111)]
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
        if (addJsType)
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

    // including '/'
    // e.g. E:/code/qjsbunitynew/proj/Assets/Src/Tween_Scripts/TestHighConcurrencyGroup.cs -> Tween_Scripts/
    static string nextPath = string.Empty;
    static bool matched = false;
    static bool addJsType = true; // next operation, add or remove?

    /// <summary>
    /// See 'MakeJsTypeAttributeInSrc' for detail.
    /// </summary>
    [MenuItem("JSB/Delete SharpKit JsType Attribute for all Structs and Classes", false, 52)]
    public static void DelJsTypeAttributeInSrc()
    {
        addJsType = false;
        if (MakeJsTypeAttributeInSrc())
        {
            EditorUtility.DisplayDialog("Tip",
            "You have to execute this menu later: \nJSB | Generate SharpKit JsType file CS.require list",
                        "OK");
        }
    }
    /// <summary>
    /// See 'MakeJsTypeAttributeInSrc' for detail.
    /// </summary>
    [MenuItem("JSB/Add SharpKit JsType Attribute for all Structs and Classes", false, 51)]
    public static void AddJsTypeAttributeInSrc()
    {
        addJsType = true;
        if (MakeJsTypeAttributeInSrc())
        {
            EditorUtility.DisplayDialog("Tip",
            "You have to execute this menu later: \nJSB | Generate SharpKit JsType file CS.require list",
                        "OK");
        }
    }
    /// <summary>
    /// MakeJsTypeAttributeInSrc
    /// Automatically add or remove 'JsType' attribute for all structures and classes in .cs files
    /// .cs files in 'JSBindingSettings.DirectoriesNotToExport' directories 
    /// but not in 'JSBindingSettings.DirectoriesToExport' directories will be ignored
    /// </summary>
    public static bool MakeJsTypeAttributeInSrc()
    {
        bool bContinue = EditorUtility.DisplayDialog("Tip",
@"Make sure you have made proper settings to 

JSBindingSettings.DirectoriesNotToExport 
JSBindingSettings.DirectoriesToExport 

fields before this action.",
            "Continue",
            "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return false;
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

            // ignore Editor scripts!
            if (subPath.IndexOf("Editor/") == 0 ||
                subPath.IndexOf("/Editor/") > 0)
            {
                export = false;
                continue;
            }

            foreach (string dir in JSBindingSettings.PathsNotToJavaScript)
            {
                if (subPath.IndexOf(dir) == 0)
                {
                    export = false;
                    break;
                }
            }
            if (!export && JSBindingSettings.PathsToJavaScript != null)
            {
                foreach (string dir in JSBindingSettings.PathsToJavaScript)
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

        string fileName = GetTempFileNameFullPath("FilesToAddJsType.txt");
        File.WriteAllText(fileName, sb.ToString());
        bContinue = EditorUtility.DisplayDialog("TIP",
             "Files list are in " + fileName + ". please verify.",
             "OK",
             "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return false;
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
            var reg = new Regex(@"(?>^\s*\[\s*JsType.*$)?\s*(?<ClassDefinition>^(?>(?>public|protected|private|static|partial|abstract|internal)*\s*)*(?>class|struct|enum)\s+(?<ClassName>\w+)\s*(?::\s*\w+\s*(?:\,\s*\w+)*)?\s*\{)", RegexOptions.Multiline);
            content = reg.Replace(content, MyMatchEvaluator);

            if (matched && content.IndexOf("using SharpKit.JavaScript;") < 0)
            {
                content = "using SharpKit.JavaScript;\n" + content;
            }
            File.WriteAllText(path, content);
        }
        Debug.Log("Make JsType Attribute OK.");
        AssetDatabase.Refresh();
        return true;
    }
    [MenuItem("JSB/Correct JavaScript Yield code", false, 131)]
    public static void CorrectJavaScriptYieldCode()
    {
        string YIELD_DEF = "var $yield = [];"; // to delete
        string YIELD_PUSH = "$yield.push"; // to replace with "yield "
        string YIELD_RET = "return $yield;"; // to delete
        string FUN_DEC = "function (){"; // to replace with "function* (){"

        string[] files = Directory.GetFiles(JSBindingSettings.jsDir, "*.javascript", SearchOption.AllDirectories);
        List<string> lstFiles = new List<string>();
        StringBuilder sb = new StringBuilder();
        foreach (var f in files)
        {
            string str = File.ReadAllText(f);
            if (str.IndexOf(YIELD_DEF) != -1)
            {
                lstFiles.Add(f);
                sb.AppendFormat("{0}", f);
                sb.AppendLine();
            }
        }
        string fileName = GetTempFileNameFullPath("FilesToCorrectYield.txt");
        File.WriteAllText(fileName, sb.ToString());
        bool bContinue = EditorUtility.DisplayDialog("TIP",
             "Files to correct yield are in " + fileName + ". please verify.",
             "OK",
             "Cancel");

        if (!bContinue)
        {
            Debug.Log("Correct JavaScript Yield code canceled.");
            return;
        }

        StringBuilder sbFail = new StringBuilder();
        // path in lstFiles has full path
        foreach (string f in lstFiles)
        {
            sb.Remove(0, sb.Length);

            bool suc = true;
            string str = File.ReadAllText(f);
            int lastIndex = 0, yildDefIndex, funStart = 0;
            while (true)
            {
                yildDefIndex = str.IndexOf(YIELD_DEF, lastIndex);
                if (yildDefIndex < 0) { break; }

                funStart = str.LastIndexOf(FUN_DEC, yildDefIndex);
                if (funStart < 0) { suc = false; break; }

                sb.Append(str.Substring(lastIndex, funStart - lastIndex));
                sb.Append("function* (){");

                funStart += FUN_DEC.Length;
                lastIndex = str.IndexOf(YIELD_RET, yildDefIndex);
                if (lastIndex < 0) { suc = false; break; }
                lastIndex += YIELD_RET.Length;

                sb.Append(str.Substring(funStart, lastIndex - funStart).Replace(YIELD_DEF, "").Replace(YIELD_PUSH, "yield ").Replace(YIELD_RET, ""));
            }
            if (suc)
            {
                sb.Append(str.Substring(lastIndex));
                File.WriteAllText(f, sb.ToString());
            }
            else
            {
                sbFail.AppendLine();
                sbFail.Append(f);
            }
        }
        if (sbFail.Length == 0)
            Debug.Log("Correct JavaScript Yield code OK.");
        else
            Debug.LogError("Correct JavaScript Yield code failed. Error files: " + sbFail.ToString());
    }
    [MenuItem("JSB/Online Documents", false, 151)]
    public static void OpenHelp()
    {
        Application.OpenURL("http://www.cnblogs.com/answerwinner/p/4469021.html");
    }
}
