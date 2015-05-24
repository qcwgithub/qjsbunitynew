if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var PerformanceTest1 = {
    fullname: "PerformanceTest1",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
            if (UnityEngine.Input.GetMouseButton(0)){
                var sw = new System.Diagnostics.Stopwatch.ctor();
                sw.Start();
                var g = 0;
                var f = 10;
                for (var i = 0; i < 10000000; i++){
                    g += 1;
                    f += 10;
                }
                sw.Stop();
                UnityEngine.Debug.Log$$Object("calc result: " + (f + g).toString());
                UnityEngine.Debug.Log$$Object("time: " + sw.get_ElapsedMilliseconds() + " ms");
            }
        }
    }
};
JsTypes.push(PerformanceTest1);
