using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class JSComponentGenerator
{
    class Info
    {
        public string signature;
        public string group;

        public Info(string s, string g)
        {
            signature = s;
            group = g;
        }
        string VariableName
        {
            get { return "id" + methodName; }
        }
        string methodName
        {
            get
            {
                int i = signature.IndexOf('(');
                return signature.Substring(0, i);
            }
        }
        string argList
        {
            get
            {
                cg.args a = new cg.args();
                a.Add(this.VariableName);

                if (signature.IndexOf("()") >= 0)
                    return a.Format(cg.args.ArgsFormat.OnlyList);

                int i = signature.IndexOf('(');
                var content = signature.Substring(i + 1, signature.Length - i - 2); // string in ()
                string[] ps = content.Split(',');
                foreach (var p in ps)
                {
                    a.Add(p.Substring(p.LastIndexOf(' ') + 1));
                }
                return a.Format(cg.args.ArgsFormat.OnlyList);
            }
        }

        public string FunctionDeclaration
        {
            get
            {
                return new StringBuilder().AppendFormat(@"    void {0}
    {{
        callIfExist({1});
    }}
"
                    , this.signature
                    , this.argList)

                    .ToString();
            }
        }
        public string VariableDeclaration
        {
            get { return "    int " + this.VariableName + ";\r\n"; }
        }
        public string GetInfoVariableInit
        {
            get { return new StringBuilder().AppendFormat("        {0} = JSApi.getObjFunction(jsObjID, \"{1}\");\r\n", this.VariableName, this.methodName).ToString(); }
        }
    }

    static Info[] infos = new Info[]
    {
        // already in JSComponent
//      new Info("Awake()"),
//      new Info("Start()"),
//      new Info("OnDestroy()"),

        // Performance killer
//         new Info("Update()", 0),
// 
//         new Info("FixedUpdate()", 0),
// 
//         new Info("LateUpdate()", 0),
// 
//         new Info("OnGUI()", 0),
// 
//         new Info("OnDisable()", 10),
//         new Info("OnEnable()", 10),
//         new Info("OnBecameInvisible()", 10),
//         new Info("OnBecameVisible()", 10),
// 
//         new Info("OnTransformChildrenChanged()", 80),
//         new Info("OnTransformParentChanged()", 80),
// 
//         new Info("OnApplicationFocus(bool focusStatus)", 20),
//         new Info("OnApplicationPause(bool pauseStatus)", 20),
//         new Info("OnApplicationQuit()", 20),
//         new Info("OnAudioFilterRead(float[] data, int channels)", 20),
//         new Info("OnLevelWasLoaded(int level)", 20),
// 
//         new Info("OnAnimatorIK(int layerIndex)", 30),
//         new Info("OnAnimatorMove()", 30),
//         new Info("OnJointBreak(float breakForce)", 30),

        new Info("OnParticleCollision(GameObject other)", "Physics"),
        new Info("OnCollisionEnter(Collision collisionInfo)", "Physics"),
        new Info("OnCollisionEnter2D(Collision2D coll)", "Physics"),
        new Info("OnCollisionExit(Collision collisionInfo)", "Physics"),
        new Info("OnCollisionExit2D(Collision2D coll)", "Physics"),
        new Info("OnCollisionStay(Collision collisionInfo)", "Physics"),
        new Info("OnCollisionStay2D(Collision2D coll)", "Physics"),
        new Info("OnTriggerEnter(Collider other)", "Physics"),
        new Info("OnTriggerEnter2D(Collider2D other)", "Physics"),
        new Info("OnTriggerExit(Collider other)", "Physics"),
        new Info("OnTriggerExit2D(Collider2D other)", "Physics"),
        new Info("OnTriggerStay(Collider other)", "Physics"),
        new Info("OnTriggerStay2D(Collider2D other)", "Physics"),
        new Info("OnControllerColliderHit(ControllerColliderHit hit)", "Physics"),

        new Info("OnConnectedToServer()", "Server"),
        new Info("OnDisconnectedFromServer(NetworkDisconnection info)", "Server"),
        new Info("OnFailedToConnect(NetworkConnectionError error)", "Server"),
        new Info("OnFailedToConnectToMasterServer(NetworkConnectionError info)", "Server"),
        new Info("OnMasterServerEvent(MasterServerEvent msEvent)", "Server"),
        new Info("OnNetworkInstantiate(NetworkMessageInfo info)", "Server"),
        new Info("OnPlayerConnected(NetworkPlayer player)", "Server"),
        new Info("OnPlayerDisconnected(NetworkPlayer player)", "Server"),
        new Info("OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)", "Server"),
        new Info("OnServerInitialized()", "Server"),

        new Info("OnMouseDown()", "Mouse"),
        new Info("OnMouseDrag()", "Mouse"),
        new Info("OnMouseEnter()", "Mouse"),
        new Info("OnMouseExit()", "Mouse"),
        new Info("OnMouseOver()", "Mouse"),
        new Info("OnMouseUp()", "Mouse"),
        new Info("OnMouseUpAsButton()", "Mouse"),

        new Info("OnPostRender()", "Render"),
        new Info("OnPreCull()", "Render"),
        new Info("OnPreRender()", "Render"),
        new Info("OnRenderImage(RenderTexture src, RenderTexture dest)", "Render"),
        new Info("OnRenderObject()", "Render"),
        new Info("OnWillRenderObject()", "Render"),


        // Editor only
        //
        // Reset
        // OnDrawGizmos
        // OnDrawGizmosSelected
        // OnValidate
    };

    public static JSComponent CreateJSComponentInstance(MonoBehaviour behav)
    {
        return null;
    }

    //[MenuItem("JSB/Gen JSComopnents", false, 1000)]
    public static void GenJSComponents()
    {
        //
        // 0 suffix
        // 1 variables declare
        // 2 variables init
        // 3 functions
        //
        string fileFormat = @"//
// Automatically generated by JSComponentGenerator.
//
using UnityEngine;

public class JSComponent{0} : JSComponent
{{
{1}
    protected override void initMemberFunction()
    {{
        base.initMemberFunction();
{2}    }}

{3}
}}";
        // group ->  List<Info>
        var dict = new Dictionary<string, List<Info>>();
        for (var i = 0; i < infos.Length; i++)
        {
            Info info = infos[i];
            List<Info> lst;
            if (!dict.TryGetValue(info.group, out lst))
            {
                lst = dict[info.group] = new List<Info>();
            }
            lst.Add(info);
        }

        // index -> group
        int ind = 0;
        var dict2 = new Dictionary<int, string>();
        foreach (var d in dict)
        {
            dict2[ind++] = d.Key;
        }

        // arr: 0,1,2,...N
        int N = dict.Count;
        int[] arr = new int[N];
        for (var i = 0; i < N; i++)
        {
            arr[i] = i;
        }
        List<int[]>[] arrLstCombination = new List<int[]>[N];

        int C = 0;
        for (var i = 0; i < N; i++)
        {
            arrLstCombination[i] = Algorithms.PermutationAndCombination<int>.GetCombination(arr, i + 1);
            C += arrLstCombination[i].Count;
        }

        bool bContinue = EditorUtility.DisplayDialog("WARNING",
            @"Total files: " + C,
            "Continue",
            "Cancel");

        if (!bContinue)
        {
            Debug.Log("Operation canceled.");
            return;
        }

        for (var i = 0; i < N; i++)
        {
            List<int[]> l = arrLstCombination[i];
            for (var j = 0; j < l.Count; j++)
            {
                // gen a .cs file!

                int[] a = l[j];
                var suffix = string.Empty;
                StringBuilder sbVariableDeclaration = new StringBuilder();
                StringBuilder sbVariableInit = new StringBuilder();
                StringBuilder sbFunctions = new StringBuilder();
                StringBuilder sbFile = new StringBuilder();
                for (var k = 0; k < a.Length; k++)
                {
                    var group = dict2[a[k]];
                    suffix += "_" + group;
                    List<Info> lstInfo = dict[group];
                    foreach (var li in lstInfo)
                    {
                        sbVariableDeclaration.Append(li.VariableDeclaration);
                        sbVariableInit.Append(li.GetInfoVariableInit);
                        sbFunctions.Append(li.FunctionDeclaration);
                    }
                }
                sbFile.AppendFormat(fileFormat, suffix, sbVariableDeclaration, sbVariableInit, sbFunctions);

                string fileName = Application.dataPath + "/JSBinding/Source/JSComponent/Generated/JSComponent" + suffix + ".cs";
                var w = new StreamWriter(fileName, false/* append */, Encoding.UTF8);
                w.Write(sbFile.ToString());
                w.Close();
            }
        }

        AssetDatabase.Refresh();
    }
}
