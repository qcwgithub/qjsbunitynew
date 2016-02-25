using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System;

//此类做CS and JS bindings
[JsType(JsMode.Clr, "~/Assets/StreamingAssets/JavaScript/SharpKitGeneratedFiles.javascript")]
public class LoginManager
{
    private static LoginManager _instance;
    public static LoginManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LoginManager();
            }
            return _instance;
        }
    }

     public event Action onHaConnected;
}


//此类转JS
[JsType(JsMode.Clr, "~/Assets/StreamingAssets/JavaScript/SharpKitGeneratedFiles.javascript")]
public class TestLogin
{
    private void Dofun()
    {
	    LoginManager.Instance.onHaConnected += HandleonHaConnected;
    }

    private void HandleonHaConnected()
    {
    }
}
[JsType(JsMode.Clr, "~/Assets/StreamingAssets/JavaScript/SharpKitGeneratedFiles.javascript")]
public class EventTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
