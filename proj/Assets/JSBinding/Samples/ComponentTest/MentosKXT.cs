using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

[JsType(JsMode.Clr, "~/../Assets/StreamingAssets/JavaScript/SharpKitGeneratedFiles.javascript")]
public class MentosKXT : MonoBehaviour 
{
	void Start () 
    {
        print("Hello this is MentosKXT Start()");
	}

    bool bUpdatePrinted = false;
    void Update()
    {
        if (!bUpdatePrinted)
        {
            bUpdatePrinted = true;
            print("Hello this is MentosKXT Update()");
        }
	}
}
