if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var PerformanceTest1 = {
    fullname: "PerformanceTest1",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj2010",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.mTransform = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            this.mTransform = this.get_transform();
        },
        Test0: function (){
            var N = 10000000;
            var sw = new System.Diagnostics.Stopwatch.ctor();
            sw.Start();
            var g = 0;
            var f = 0;
            for (var i = 0; i < N; i++){
                g += 1;
                f += 1;
            }
            sw.Stop();
            UnityEngine.Debug.Log$$Object("test0 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        Test1: function (){
            var sw = new System.Diagnostics.Stopwatch.ctor();
            sw.Start();
            var m;
            for (var i = 0; i < 2000; i++){
                m = this.mTransform.get_position();
                this.mTransform.set_position(m);
            }
            UnityEngine.Debug.Log$$Object("test1 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        Test2: function (){
            var sw = new System.Diagnostics.Stopwatch.ctor();
            sw.Start();
            var m = this.mTransform.get_position();
            for (var i = 0; i < 2000; i++){
                m = UnityEngine.Vector3.Normalize$$Vector3(m);
            }
            UnityEngine.Debug.Log$$Object("test2 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        Test3: function (){
            var sw = new System.Diagnostics.Stopwatch.ctor();
            sw.Start();
            var m = this.mTransform.get_position();
            for (var i = 0; i < 2000; i++){
                m.Normalize();
            }
            UnityEngine.Debug.Log$$Object("test3 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        Test4: function (){
            var sw = new System.Diagnostics.Stopwatch.ctor();
            sw.Start();
            var m = this.mTransform.get_position();
            for (var i = 0; i < 2000; i++){
                this.mTransform.set_position(m);
            }
            UnityEngine.Debug.Log$$Object("test4 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        Test5: function (){
            var sw = new System.Diagnostics.Stopwatch.ctor();
            sw.Start();
            var m = UnityEngine.Vector3.get_one();
            for (var i = 0; i < 2000; i++){
                m = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(i, i, i);
            }
            UnityEngine.Debug.Log$$Object("test5 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        Test6: function (){
            var sw = new System.Diagnostics.Stopwatch.ctor();
            sw.Start();
            for (var i = 0; i < 500000; i++){
                var go = new UnityEngine.GameObject.ctor$$String("init");
                UnityEngine.Object.DestroyImmediate$$Object(go);
            }
            UnityEngine.Debug.Log$$Object("test6 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        Update: function (){
            if (UnityEngine.Input.GetMouseButton(0)){
                this.Test0();
                this.Test1();
                this.Test2();
                this.Test3();
                this.Test4();
                this.Test5();
                this.Test6();
            }
        }
    }
};
JsTypes.push(PerformanceTest1);

