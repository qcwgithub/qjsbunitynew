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
    protected int jsObjID = 0;

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
    int idStartSinking = 0;
    int idRestartLevel = 0;
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
        idStartSinking = JSApi.getObjFunction(jsObjID, "StartSinking");
        idRestartLevel = JSApi.getObjFunction(jsObjID, "RestartLevel");
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

    int jsState = 0;
    bool jsSuccess { get { return jsState == 1; } set { if (value) jsState = 1; } }
    public bool jsFail { get { return jsState == 2; } set { if (value) jsState = 2; else jsState = 0; } }

    protected void callIfExist(int funID, params object[] args)
    {
        if (funID > 0)
        {
            JSMgr.vCall.CallJSFunctionValue(jsObjID, funID, args);
        }
    }

    public void initJS()
    {
        if (jsFail || jsSuccess) return;

        if (string.IsNullOrEmpty(jsClassName))
        {
            jsFail = true;
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
            jsFail = true;
            return;
        } 
        JSMgr.addJSCSRel(jsObjID, this);
        initMemberFunction();
        jsSuccess = true;
    }

    //
    // 有几个事情要做
    // A) initJS()
    // B) initSerializedData(jsObjID)
    // C) callIfExist(idAwake);
    //
    // 不同时候要做的事情，假设有2个类 X 和 Y
    // 1) 假设 X 类不被其他类所引用，则 X 类 Awake 时：A + B + C
    // 2) 在 X 类 initSerializedData 时发现引用了 Y 类，而 Y 类的 Awake 还没有被调用，那么会马上调用 Y 类的 A（看 GetJSObjID 函数），之后 Y 类的 Awake 里：B + C
    //    看 JSSerializer.GetGameObjectMonoBehaviourJSObj 函数
    //    为什么第1步只调用Y的A，而不调B？因为那时候X类正在处理序列化，不想中间又穿插Y的序列化处理，也用不到
    // 3) 在 AddComponent<X>() 时，我们知道他会调用 Awake()，但是此时由于 jsClassName 未被设置，所以会 jsFail=true，但紧接着我们又设置 jsFail=false，然后调用 init(true) 和 callAwake()，做的事情也是 A + B + C
    //    看 Components.cs 里的 GameObject_AddComponentT1 函数
    // 4) 在 GetComponent<X>() 时，如果 X 的 Awake() 还未调用，我们会调用 X 的 init(true)，他做了 A + B，之后 X 的 Awake() 再做 C
    //    看 Components.cs 里的 help_searchAndRetCom 和 help_searchAndRetComs 函数
    //
    //
    // 总结：以上那么多分类，做的事情其实就是，当一个类X要在Awake时去获取Y类组件，甚至访问Y类成员，如果此时Y类的Awake还没有调用，此时会得到undefined，那么我们只好先初始化一下Y类的JS对象。
    //
    bool dataSerialized = false;
    public void init(bool callSerialize)
    {
        if (!JSEngine.initSuccess && !JSEngine.initFail)
        {
            JSEngine.FirstInit();
        }
        if (!JSEngine.initSuccess)
        {
            return;
        }

        initJS();

        if (jsSuccess && callSerialize && !dataSerialized)
        {
            dataSerialized = true;
            initSerializedData(jsObjID);
        }
    }
    public void callAwake()
    {
        if (jsSuccess)
        {
            callIfExist(idAwake);
        }
    }
    void Awake()
    {
        init(true);
        callAwake();
    }
    /// <summary>
    /// get javascript object id of this JSComponent.
    /// jsObjID may == 0 when this function is called, because other scripts refer to this JSComponent.
    /// in this case, we call initJS() for this JSComponent immediately.
    /// </summary>
    /// <returns></returns>
    /// 
    public int GetJSObjID(bool callSerialize)
    {
        if (jsObjID == 0)
        {
            init(callSerialize);
        }
        return jsObjID;
    }

    void Start() 
    {
        callIfExist(idStart);
    }

    void OnDestroy()
    {
        if (!JSMgr.isShutDown)
        {
            callIfExist(idOnDestroy);
        }

        if (jsSuccess)
        {
            // remove this jsObjID even if JSMgr.isShutDown is true
            JSMgr.removeJSCSRel(jsObjID);
        }

        if (JSMgr.isShutDown)
        {
            return;
        }

        if (jsSuccess)
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
    void StartSinking()
    {
        callIfExist(idStartSinking);
    }
    void RestartLevel()
    {
        callIfExist(idRestartLevel);
    }
}
