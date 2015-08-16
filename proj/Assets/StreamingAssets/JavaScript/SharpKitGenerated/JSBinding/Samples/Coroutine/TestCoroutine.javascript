if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestCoroutine = {
    fullname: "TestCoroutine",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            this.StartCoroutine$$String("DoTest");
            this.StartCoroutine$$IEnumerator(this.DoTest2($CreateAnonymousDelegate(this, function (){
                UnityEngine.Debug.Log$$Object("Action called!");
            })));
            this.InvokeRepeating("PrintHelloInvoke", 4, 1);
            this.Invoke("DelayInvoke", 5);
        },
        Update: function (){
            if (UnityEngine.Input.GetMouseButtonUp(0)){
                this.StopCoroutine$$String("DoTest");
            }
        },
        LateUpdate: function (){
            jsimp.Coroutine.UpdateCoroutineAndInvoke(this);
        },
        WaitForCangJingKong: function* (){
            
            yield (new UnityEngine.WaitForSeconds.ctor(2));
            
        },
        DoTest: function* (){
            
            UnityEngine.Debug.Log$$Object("DoTest 1");
            yield (null);
            UnityEngine.Debug.Log$$Object("DoTest 2");
            yield (new UnityEngine.WaitForSeconds.ctor(1));
            var www = new UnityEngine.WWW.ctor$$String("file://" + UnityEngine.Application.get_dataPath() + "/JSBinding/Samples/Coroutine/CoroutineReadme.txt");
            yield (www);
            UnityEngine.Debug.Log$$Object("DoTest 3 Text from WWW: " + www.get_text());
            yield (this.StartCoroutine$$IEnumerator(this.WaitForCangJingKong()));
            UnityEngine.Debug.Log$$Object("DoTest 4 Wait for CangJingKong finished!");
            
        },
        DoTest2: function* (a){
            
            UnityEngine.Debug.Log$$Object("will call action 2 seconds later");
            yield (new UnityEngine.WaitForSeconds.ctor(2));
            a();
            
        },
        PrintHelloInvoke: function (){
            UnityEngine.MonoBehaviour.print("Hello, Invoke! (every 1 second)");
        },
        DelayInvoke: function (){
            UnityEngine.MonoBehaviour.print("This is call 5 seconds later, only once!");
        }
    }
};
JsTypes.push(TestCoroutine);

