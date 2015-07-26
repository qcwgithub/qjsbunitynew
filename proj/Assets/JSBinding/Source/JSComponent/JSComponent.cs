using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security;

using jsval = JSApi.jsval;

/// <summary>
/// JSComponent
/// A class redirect event functions (Awake, Start, Update, etc.) to JavaScript
/// Support serializations
/// </summary>
public class JSComponent : JSSerializer
{
    [HideInInspector]
    [NonSerialized]
    public int jsObjID = 0;

    int idAwake = 0;
    int idStart = 0;
    int idOnDestroy = 0;

//     int idFixedUpdate = 0;
//     int idUpdate = 0;
//     int idLateUpdate = 0;
//     int idOnGUI = 0;
//     int idOnEnable = 0;
//     int idOnTriggerEnter2D = 0;
//     int idOnTriggerStay = 0;
//     int idOnTriggerExit = 0;
//     int idOnAnimatorMove = 0;
//     int idOnAnimatorIK = 0;

    //
    // 2D Platformer 
    //
//     int idDestroyChildGameObject = 0;
//     int idDisableChildGameObject = 0;
//     int idDestroyGameObject = 0;

    /// <summary>
    /// Initializes the member function.
    /// </summary>
    protected virtual void initMemberFunction()
    {
        idAwake = JSApi.getObjFunction(jsObjID, "Awake");
        idStart = JSApi.getObjFunction(jsObjID, "Start");
        idOnDestroy = JSApi.getObjFunction(jsObjID, "OnDestroy");

//         idFixedUpdate = JSApi.getObjFunction(jsObjID, "FixedUpdate");
//         idUpdate = JSApi.getObjFunction(jsObjID, "Update");
//         idLateUpdate = JSApi.getObjFunction(jsObjID, "LateUpdate");
//         idOnGUI = JSApi.getObjFunction(jsObjID, "OnGUI");
//         idOnEnable = JSApi.getObjFunction(jsObjID, "OnEnable");
//         idOnTriggerEnter2D = JSApi.getObjFunction(jsObjID, "OnTriggerEnter2D");
//         idOnTriggerStay = JSApi.getObjFunction(jsObjID, "OnTriggerStay");
//         idOnTriggerExit = JSApi.getObjFunction(jsObjID, "OnTriggerExit");
//         idOnAnimatorMove = JSApi.getObjFunction(jsObjID, "OnAnimatorMove");
//         idOnAnimatorIK = JSApi.getObjFunction(jsObjID, "OnAnimatorIK");

//         idDestroyChildGameObject = JSApi.getObjFunction(jsObjID, "DestroyChildGameObject");
//         idDisableChildGameObject = JSApi.getObjFunction(jsObjID, "DisableChildGameObject");
//         idDestroyGameObject = JSApi.getObjFunction(jsObjID, "DestroyGameObject");
    }
    /// <summary>
    /// Removes if exist.
    /// </summary>
    /// <param name="id">The identifier.</param>
    void removeIfExist(int id)
    {
        if (id != 0)
        {
            JSApi.removeByID(id);
        }
    }
    void removeMemberFunction()
    {
        // ATTENSION
        // same script have same idAwake idStart ... values
        // if these lines are executed in OnDestroy (for example  for gameObject A)
        // other gameObjects (for example B) with the same script
        // will also miss these functions
        // 
        // and if another C (with the same script) is born later   
        // it will re-get these values  but they are new values 
        // 
        // 
        // but if they are not removed in OnDestroy 
        // C valueMap may grow to a very big size
        //
//         removeIfExist(idAwake);
//         removeIfExist(idStart);
//         removeIfExist(idFixedUpdate);
//         removeIfExist(idUpdate);
//         removeIfExist(idOnDestroy);
//         removeIfExist(idOnGUI);
//         removeIfExist(idOnEnable);
//         removeIfExist(idOnTriggerEnter2D);
//         removeIfExist(idOnTriggerStay);
//         removeIfExist(idOnTriggerExit);
//         removeIfExist(idOnAnimatorMove);
//         removeIfExist(idOnAnimatorIK);
//         removeIfExist(idDestroyChildGameObject);
//         removeIfExist(idDisableChildGameObject);
//         removeIfExist(idDestroyGameObject);
    }

    int initState = 0;
    bool initSuccess { get { return initState == 1; } set { if (value) initState = 1; } }
    public bool initFail { get { return initState == 2; } set { if (value) initState = 2; else initState = 0; } }

    protected void callIfExist(int funID, params object[] args)
    {
        if (funID > 0)
        {
            JSMgr.vCall.CallJSFunctionValue(jsObjID, funID, args);
        }
    }

    public void initJS()
    {
        if (initFail || initSuccess) return;

        if (string.IsNullOrEmpty(jsClassName))
        {
            initFail = true;
            return;
        }

        // ATTENSION
        // cannot use createJSClassObject here
        // because we have to call ctor, to run initialization code
        // this object will not have finalizeOp
        jsObjID = JSApi.newJSClassObject(this.jsClassName);
        JSApi.setTraceS(jsObjID, true);
        if (jsObjID == 0)
        {
            Debug.LogError("New MonoBehaviour \"" + this.jsClassName + "\" failed. Did you forget to export that class?");
            initFail = true;
            return;
        } 
        JSMgr.addJSCSRel(jsObjID, this);
        initMemberFunction();
        initSuccess = true;
    }
    public void Awake()
    {
        if (!JSEngine.initSuccess && !JSEngine.initFail)
        {
            JSEngine.FirstInit();
        }
        if (JSEngine.initSuccess)
        {
            initJS();

            if (initSuccess)
            {
                initSerializedData(jsObjID);
            }
        }
    }
    /// <summary>
    /// get javascript object id of this JSComponent.
    /// jsObjID may == 0 when this function is called, because other scripts refer to this JSComponent.
    /// in this case, we call initJS() for this JSComponent immediately.
    /// </summary>
    /// <returns></returns>
    /// 
    public int GetJSObjID()
    {
        if (jsObjID == 0)
        {
            if (!initFail) 
                initJS();
        }
        return jsObjID;
    }

    private bool firstStart = true;
    void Start() 
    {
        if (firstStart)
        {
            firstStart = false;
            callIfExist(idAwake);
        }
        callIfExist(idStart);
    }

    void OnDestroy()
    {
        if (!JSMgr.isShutDown)
        {
            callIfExist(idOnDestroy);
        }

        if (initSuccess)
        {
            // remove this jsObjID even if JSMgr.isShutDown is true
            JSMgr.removeJSCSRel(jsObjID);
        }

        if (JSMgr.isShutDown)
        {
            return;
        }

        if (initSuccess)
        {
            // JSMgr.RemoveRootedObject(jsObj);
            JSApi.setTraceS(jsObjID, false);
            // JSMgr.removeJSCSRel(jsObjID); // Move upwards

            //
            // jsObj doesn't have finalize
            // we must remove it here
            // having a finalize is another approach
            //
            JSApi.removeByID(jsObjID);
            removeMemberFunction();
        }
    }

//     void FixedUpdate()
//     {
//         callIfExist(idFixedUpdate);
//     }
//     void Update()
//     {
//         callIfExist(idUpdate);
//     }
//     void LateUpdate()
//     {
//         callIfExist(idLateUpdate);
//     }
// 
//     void OnEnable()
//     {
//         callIfExist(idOnEnable);
//     }
//     void OnGUI()
//     {
//         callIfExist(idOnGUI);
//     }
// 
//     void OnTriggerEnter2D (Collider2D other)
//     {
//         callIfExist(idOnTriggerEnter2D, other);
//     }
//     void OnTriggerStay(Collider other)
//     {
//         callIfExist(idOnTriggerStay, other);
//     }
//     void OnTriggerExit(Collider other)
//     {
//         callIfExist(idOnTriggerExit, other);
//     }
//     void OnAnimatorMove()
//     {
//         callIfExist(idOnAnimatorMove);
//     }
//     void OnAnimatorIK(int layerIndex)
//     {
//         callIfExist(idOnAnimatorIK);
//     }
// 
    //
    // 2DPlatformer
    //

//     void DestroyChildGameObject()
//     {
//         callIfExist(idDestroyChildGameObject);
//     }
// 
//     void DisableChildGameObject()
//     {
//         callIfExist(idDisableChildGameObject);
//     }
// 
//     void DestroyGameObject()
//     {
//         callIfExist(idDestroyGameObject);
//     }
}
