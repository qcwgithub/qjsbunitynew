using UnityEngine;
using System.Collections;
using System.IO;


/*
 * JSFileLoader
 * All js files are loaded by this class.
 */
public class JSFileLoader : MonoBehaviour 
{
    //public bool Async = true;
    public delegate void OnLoadJS(string shortName, byte[] bytes, string fullName);

//    public void LoadJSAsync(string shortName, bool bGenerated, OnLoadJS onLoadJS)
//    {
//        StartCoroutine(WWWLoad(shortName, bGenerated, onLoadJS));
//    }

//    public void LoadJSSync(string shortName, bool bGenerated, OnLoadJS onLoadJS)
//    {
//        string fullName = JSMgr.getJSFullName(shortName, bGenerated);
//        byte[] bytes = LoadJSSync(fullName);
//        onLoadJS(shortName, bytes, fullName);
//    }
    public virtual byte[] LoadJSSync(string fullName)
    {
        byte[] bytes = null;

        //
        // Android
        // streaming assets are in apk file
        // only way to load file from streamingAssetsPath synchronously is to loop waiting for WWW to finish
        // (as far as I know, if you have a better way, please tell me)
        //
#if UNITY_ANDROID && !UNITY_EDITOR_WIN && !UNITY_EDITOR_OSX
        WWW w = new WWW(fullName);
        while (true)
        {
            if (w.error != null && w.error.Length > 0)
            {
                Debug.Log("ERROR: /// " + w.error);
                break;
            }

            if (w.isDone)
                break;
        }
        bytes = w.bytes;
#else
        try
        {
            bytes = File.ReadAllBytes(fullName);
        }
        catch (System.Exception exp) 
        {
            Debug.LogError(exp.ToString());
        }
        //
        // FileStream does not work on iOS, UnauthorizedAccessException
        // I don't know why
        // 
        /* FileStream fs = new FileStream(fullName, FileMode.Open);
        if (fs != null)
        {
            bytes = new byte[fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            fs.Close();
        } */ 
#endif
        return bytes;
    }

//    IEnumerator WWWLoad(string shortName, bool bGenerated, OnLoadJS onLoadJS)
//    {
//        string fullName = JSMgr.getJSFullName(shortName, bGenerated);
//
//        WWW www = new WWW(fullName);
//        yield return www;
//
//        if (www.error != null && www.error.Length > 0)
//        {
//            Debug.Log("Error loading JS: " + fullName + " " + www.error);
//            onLoadJS(shortName, null, fullName);
//        }
//        else
//        {
//            onLoadJS(shortName, www.bytes, fullName);
//        }
//    }
}
