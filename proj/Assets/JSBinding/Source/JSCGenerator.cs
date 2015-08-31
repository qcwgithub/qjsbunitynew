using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class JSCGenerator
{
#if UNITY_EDITOR_WIN
    [MenuItem("JSB/Compile all JavaScript to Bytecode", false, 151)]
    static void ConvertJavaScriptToBytecode()
    {
        // 1)
        MergeAndCopyJavaScript();

        // 2)
        string bat = (Application.dataPath + "/../../tools/cocos2d-console/bin/cocos").Replace('/', '\\');
        string arg = string.Format("jscompile -s \"{0}\" -d \"{1}\"", JSBindingSettings.mergedJsDir.Replace('/', '\\'), JSBindingSettings.jscDir.Replace('/', '\\'));
        System.Diagnostics.Process.Start(bat, arg);
    }
#endif

    // 拷贝整个文件夹 proj/Assets/StreamingAssets/JavaScript/ -> proj/JavaScript_js/
    // 修改文件后缀 .javascript -> .js
    // 可能合并某些文件，目前没有，只是拷贝和改后缀而已
    static void MergeAndCopyJavaScript()
    {
        string[] lst;

        // 1) 删除 mergedJsDir 下所有文件
        if (Directory.Exists(JSBindingSettings.mergedJsDir))
        {
            lst = Directory.GetFiles(JSBindingSettings.mergedJsDir, "*.*", SearchOption.AllDirectories);
            foreach (var l in lst)
            {
                File.Delete(l);
            }
        }

        // 2) 拷贝 jsDir -> mergedJsDir
        lst = Directory.GetFiles(JSBindingSettings.jsDir, "*.javascript", SearchOption.AllDirectories);
        foreach (var l in lst)
        {
            string dst = l.Replace('\\', '/').Replace(JSBindingSettings.jsDir, JSBindingSettings.mergedJsDir).Replace(JSBindingSettings.jsExtension, ".js");
            Directory.CreateDirectory(Path.GetDirectoryName(dst));
            File.Copy(l, dst, true);
        }

        Debug.Log("MergeAndCopyJavaScript finish");
    }
}
