if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ComponentExtension = {
    fullname: "ComponentExtension",
    baseTypeName: "System.Object",
    staticDefinition: {
        GetComponentI$1$$Component: function (T, com){
            return com.GetComponent$1(T);
        },
        GetComponentI$1$$GameObject: function (T, go){
            return go.GetComponent$1(T);
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(ComponentExtension);
var jsimp$Coroutine = {
    fullname: "jsimp.Coroutine",
    baseTypeName: "System.Object",
    staticDefinition: {
        UpdateCoroutineAndInvoke: function (mb){
            var elapsed = UnityEngine.Time.get_deltaTime();
            mb.$UpdateAllCoroutines(elapsed);
            mb.$UpdateAllInvokes(elapsed);
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(jsimp$Coroutine);
var jsimp$Misc = {
    fullname: "jsimp.Misc",
    baseTypeName: "System.Object",
    staticDefinition: {
        string_Replace: function (str, str1, str2){
            return str.replace(str1, str2);
        },
        string_Split: function (str, sep){
            return str.split(sep);
        },
        string_Split_RemoveEmptyEntries: function (str, sep){
            var A = str.split(sep);
var j = 0, B = [];
for (var i = 0; i < A.length; i++)
{
    if (A[i].length != 0)
        B[j++] = A[i];
}
return B;
        },
        List_AddRange$1: function (T, lst, collection){
            var L = collection.length;
for (var i = 0; i < L; i++)
{
    lst.Add(collection[i]);
}
        },
        Abs$$Int32: function (v){
            return Math.abs(v);
        },
        Abs$$Single: function (v){
            return Math.abs(v);
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(jsimp$Misc);
var jsimp$Reflection = {
    fullname: "jsimp.Reflection",
    baseTypeName: "System.Object",
    staticDefinition: {
        CreateInstance$1: function (T){
            // 这2个函数，如果T是C#类型，比如说 GameObject，是否仍然有效？
        	// 答案是：有点神奇，我认为是有效的！
        	// 得测试一下
			var ret = new T();
            return ret;
        },
        CreateInstance$$Type: function (type){
            return new type._JsType.ctor();
        },
        SetFieldValue: function (obj, fieldName, value){
            if (obj != null) {
                //if (obj.hasOwnProperty(fieldName))
                {
                    obj[fieldName] = value;
                    return true;
                }
            }
            return false;
        },
        GetFieldType: function (type, fieldName){
            if (type != null) {
                var typeStr = type._JsType.ctor.prototype[fieldName + "$$"];
                //print(type.fullname + "." + fieldName + " = " + typeStr);
                if (typeStr != undefined) {
                    if (typeStr == "System.Int32[]") {
                        //return Int32Array;
                        var fieldType = Typeof(Int32Array);
                        //print(fieldType.fullname);
                        print("[] " + fieldType)
                        return fieldType;
                    } else {
                        var fieldType = Typeof(typeStr);
                        //print(fieldType.fullname);
                        return fieldType;
                    }
                }
            }
            return null;
        },
        SetPropertyValue: function (obj, propertyName, value){
            return this.SetFieldValue(obj, "_" + propertyName, value);
        },
        GetPropertyType: function (type, propertyName){
            return this.GetFieldType(type, propertyName);
        },
        PropertyTypeIsIntArray: function (type, propertyName){
            if (type != null) {
                var typeStr = type._JsType.ctor.prototype[propertyName + "$$"];
                return typeStr == "System.Int32[]";
            }
            return false;
        },
        SimpleTEquals$1: function (T, a, b){
            return (a == b);
        },
        TypeIsIntArray: function (type){
            return type._JsType == Int32Array;
        },
        CallObjMethod: function (obj, methodName, parameters){
            var args = Array.prototype.slice.apply(arguments);
			var obj = args[0];
			var methodName = args[1];
            obj[methodName].apply(obj, args.slice(2));
            return true;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(jsimp$Reflection);
var DelegateTest = {
    fullname: "DelegateTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.lst = null;
            this.mi = 0;
            this.elapsed = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.lst = new System.Collections.Generic.List$1.ctor(System.Int32.ctor);
            for (var i = 0; i < 10; i++){
                this.lst.Add(i);
            }
        },
        Start: function (){
        },
        Update: function (){
            this.elapsed += UnityEngine.Time.get_deltaTime();
            if (this.elapsed > 1){
                this.elapsed = 0;
                var f = this.lst.Find($CreateAnonymousDelegate(this, function (v){
                    return v == this.mi;
                }));
                UnityEngine.Debug.Log$$Object("Found: " + f);
                this.mi++;
                if (this.mi >= 10)
                    this.mi -= 10;
            }
        }
    }
};
JsTypes.push(DelegateTest);
var DictionaryTest = {
    fullname: "DictionaryTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.elapsed = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
            this.elapsed += UnityEngine.Time.get_deltaTime();
            if (this.elapsed > 1){
                this.elapsed = 0;
                var dict = new System.Collections.Generic.Dictionary$2.ctor(System.String.ctor, System.Int32.ctor);
                dict.Add("qiucw", 28);
                dict.Add("helj", 27);
                var age;
                if ((function (){
                    var $1 = {
                        Value: age
                    };
                    var $res = dict.TryGetValue("qiucw", $1);
                    age = $1.Value;
                    return $res;
                }).call(this)){
                    UnityEngine.Debug.Log$$Object("age: " + age.toString());
                }
                else {
                    UnityEngine.Debug.Log$$Object("not found");
                }
                var $it1 = dict.GetEnumerator();
                while ($it1.MoveNext()){
                    var v = $it1.get_Current();
                    UnityEngine.Debug.Log$$Object(v.get_Key().toString() + "->" + v.get_Value().toString());
                }
            }
        }
    }
};
JsTypes.push(DictionaryTest);
var EncodingTest = {
    fullname: "EncodingTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    staticDefinition: {
        byteConverString: function (data, index, count){
            var d = System.Text.Encoding.get_UTF8().GetDecoder();
            var arrSize = d.GetCharCount$$Byte$Array$$Int32$$Int32(data, index, count);
            var chars = new Array(arrSize);
            var charSize = d.GetChars$$Byte$Array$$Int32$$Int32$$Char$Array$$Int32(data, index, count, chars, 0);
            var str = new System.String.ctor$$Char$Array$$Int32$$Int32(chars, 0, charSize);
            return str;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var bytes = new Uint8Array([65, 66, 67, 0]);
            var str = EncodingTest.byteConverString(bytes, 0, 3);
            UnityEngine.Debug.Log$$Object(str);
        }
    }
};
JsTypes.push(EncodingTest);
var LoginManager = {
    fullname: "LoginManager",
    baseTypeName: "System.Object",
    staticDefinition: {
        cctor: function (){
            LoginManager._instance = null;
        },
        Instance$$: "LoginManager",
        get_Instance: function (){
            if (LoginManager._instance == null){
                LoginManager._instance = new LoginManager.ctor();
            }
            return LoginManager._instance;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.onHaConnected = null;
            System.Object.ctor.call(this);
        },
        add_onHaConnected: function (value){
            this.onHaConnected = $CombineDelegates(this.onHaConnected, value);
        },
        remove_onHaConnected: function (value){
            this.onHaConnected = $RemoveDelegate(this.onHaConnected, value);
        }
    }
};
JsTypes.push(LoginManager);
var TestLogin = {
    fullname: "TestLogin",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        },
        Dofun: function (){
            LoginManager.get_Instance().add_onHaConnected($CreateDelegate(this, this.HandleonHaConnected));
        },
        HandleonHaConnected: function (){
        }
    }
};
JsTypes.push(TestLogin);
var EventTest = {
    fullname: "EventTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
        }
    }
};
JsTypes.push(EventTest);
var ListTest = {
    fullname: "ListTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.elapsed = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
            this.elapsed += UnityEngine.Time.get_deltaTime();
            if (this.elapsed > 1){
                this.elapsed = 0;
                var lst = new System.Collections.Generic.List$1.ctor(System.Int32.ctor);
                lst.Add(6);
                lst.Add(95);
                var $it2 = lst.GetEnumerator();
                while ($it2.MoveNext()){
                    var v = $it2.get_Current();
                    UnityEngine.Debug.Log$$Object(v);
                }
                var vFind = lst.Find($CreateAnonymousDelegate(this, function (val){
                    return (val == 6);
                }));
                UnityEngine.Debug.Log$$Object("vFind = " + vFind);
                var lstS = lst.ConvertAll$1(System.String.ctor, $CreateAnonymousDelegate(this, function (v){
                    return "s: " + v;
                }));
                var $it3 = lstS.GetEnumerator();
                while ($it3.MoveNext()){
                    var v = $it3.get_Current();
                    UnityEngine.Debug.Log$$Object(v);
                }
                UnityEngine.Debug.Log$$Object(lstS.get_Item$$Int32(0));
                UnityEngine.Debug.Log$$Object(lstS.get_Item$$Int32(1));
            }
        }
    }
};
JsTypes.push(ListTest);
var PerformanceTest1 = {
    fullname: "PerformanceTest1",
    baseTypeName: "UnityEngine.MonoBehaviour",
    staticDefinition: {
        Run: function (refObject){
            PerTest.StaticObject.x += refObject.x;
            PerTest.StaticObject.y += refObject.y;
            return PerTest.StaticObject;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.mTransform = null;
            this.elapsed = 0;
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
            for (var i = 0; i < 2000; i++){
                new UnityEngine.Vector3.ctor$$Single$$Single$$Single(i, i, i);
            }
            UnityEngine.Debug.Log$$Object("test5 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        Test6: function (){
            var sw = new System.Diagnostics.Stopwatch.ctor();
            sw.Start();
            for (var i = 0; i < 50000; i++){
                var go = new UnityEngine.GameObject.ctor$$String("init");
                UnityEngine.Object.DestroyImmediate$$Object(go);
            }
            UnityEngine.Debug.Log$$Object("test6 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        Test7: function (){
            var sw = new System.Diagnostics.Stopwatch.ctor();
            sw.Start();
            var obj = PerTest.StaticObject;
            for (var i = 0; i < 50000; i++){
                obj = PerformanceTest1.Run(obj);
            }
            sw.Stop();
            UnityEngine.Debug.Log$$Object("test7 time: " + sw.get_ElapsedMilliseconds() + " ms");
        },
        OnChangeEvent: function (){
        },
        Update: function (){
            this.elapsed += UnityEngine.Time.get_deltaTime();
            if (this.elapsed > 3){
                this.elapsed = 0;
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
var SampleViewer = {
    fullname: "SampleViewer",
    baseTypeName: "UnityEngine.MonoBehaviour",
    staticDefinition: {
        cctor: function (){
            SampleViewer.inst = null;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.showScenesList = true;
            this.scenes = [new SampleViewer.stScene.ctor$$String$$String("V3Test", ""), new SampleViewer.stScene.ctor$$String$$String("V3Test_JS", ""), new SampleViewer.stScene.ctor$$String$$String("PerformanceTest1", ""), new SampleViewer.stScene.ctor$$String$$String("PerformanceTest1_JS", ""), new SampleViewer.stScene.ctor$$String$$String("ListTest", ""), new SampleViewer.stScene.ctor$$String$$String("ListTest_JS", ""), new SampleViewer.stScene.ctor$$String$$String("DictionaryTest", ""), new SampleViewer.stScene.ctor$$String$$String("DictionaryTest_JS", ""), new SampleViewer.stScene.ctor$$String$$String("DelegateTest", ""), new SampleViewer.stScene.ctor$$String$$String("DelegateTest_JS", ""), new SampleViewer.stScene.ctor$$String$$String("TestCoroutine", ""), new SampleViewer.stScene.ctor$$String$$String("TestCoroutine_JS", ""), new SampleViewer.stScene.ctor$$String$$String("JSImpTest1", ""), new SampleViewer.stScene.ctor$$String$$String("JSImpTest1_JS", ""), new SampleViewer.stScene.ctor$$String$$String("Car", ""), new SampleViewer.stScene.ctor$$String$$String("Car_JS", ""), new SampleViewer.stScene.ctor$$String$$String("SerializeSimple", ""), new SampleViewer.stScene.ctor$$String$$String("SerializeSimple_JS", ""), new SampleViewer.stScene.ctor$$String$$String("SerializeStruct", ""), new SampleViewer.stScene.ctor$$String$$String("SerializeStruct_JS", ""), new SampleViewer.stScene.ctor$$String$$String("XmlTest", ""), new SampleViewer.stScene.ctor$$String$$String("XmlTest_JS", "")];
            this.scrollPosition = UnityEngine.Vector2.get_zero();
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            if (UnityEngine.Object.op_Inequality(JSEngine.inst, null)){
                UnityEngine.Object.Destroy$$Object(JSEngine.inst.get_gameObject());
            }
            if (UnityEngine.Object.op_Inequality(SampleViewer.inst, null)){
                UnityEngine.Object.Destroy$$Object(this.get_gameObject());
            }
            else {
                SampleViewer.inst = this;
                UnityEngine.Object.DontDestroyOnLoad(this.get_gameObject());
            }
        },
        Start: function (){
        },
        Update: function (){
        },
        OnGUI: function (){
            if (this.showScenesList){
                var h = ((UnityEngine.Screen.get_height() / 10) | 0);
                this.scrollPosition = UnityEngine.GUI.BeginScrollView$$Rect$$Vector2$$Rect$$Boolean$$Boolean(new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(0, 0, UnityEngine.Screen.get_width(), UnityEngine.Screen.get_height()), this.scrollPosition, new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(0, 0, UnityEngine.Screen.get_width(), this.scenes.length * h), false, false);
                for (var i = 0; i < this.scenes.length; i++){
                    if (UnityEngine.GUI.Button$$Rect$$String(new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(((UnityEngine.Screen.get_width() / 4) | 0), h * i, ((UnityEngine.Screen.get_width() / 2) | 0), h), this.scenes[i].showText)){
                        this.showScenesList = false;
                        UnityEngine.Application.LoadLevel$$String(this.scenes[i].levelName);
                        break;
                    }
                }
                UnityEngine.GUI.EndScrollView();
            }
            else {
                var w = ((UnityEngine.Screen.get_width() / 10) | 0);
                var h = ((UnityEngine.Screen.get_height() / 10) | 0);
                if (w < 100)
                    w = 150;
                if (UnityEngine.GUI.Button$$Rect$$String(new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(UnityEngine.Screen.get_width() - w, 0, w, h), "Back To Scene List")){
                    this.showScenesList = true;
                    UnityEngine.Application.LoadLevel$$String("SampleViewer");
                }
            }
        }
    }
};
JsTypes.push(SampleViewer);
var V3Test = {
    fullname: "V3Test",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.elapsed = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
            this.elapsed += UnityEngine.Time.get_deltaTime();
            if (this.elapsed > 1){
                this.elapsed = 0;
                var sb = new System.Text.StringBuilder.ctor();
                this.elapsed = 0;
                var v = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(2, 3, 6);
                var w = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(7, 23, 1);
                var n = v.get_normalized();
                var arr = [n.x, n.y, n.z];
                UnityEngine.Debug.Log$$Object(sb.AppendFormat$$String$$Object$Array("v.normalized = ({0}, {1}, {2})", arr).toString());
                sb.Remove(0, sb.get_Length());
                var cross = UnityEngine.Vector3.Cross(v, w);
                arr = [cross.x, cross.y, cross.z];
                UnityEngine.Debug.Log$$Object(sb.AppendFormat$$String$$Object$Array("Cross(v, w) = ({0}, {1}, {2})", arr).toString());
                UnityEngine.Debug.Log$$Object("v.magnitude = " + v.get_magnitude());
                UnityEngine.Debug.Log$$Object("w.magnitude = " + w.get_magnitude());
                UnityEngine.Debug.Log$$Object("Dot(v, w) = " + UnityEngine.Vector3.Dot(v, w));
                UnityEngine.Debug.Log$$Object("Angle(v, w) = " + UnityEngine.Vector3.Angle(v, w));
                var proj = UnityEngine.Vector3.Project(v, w);
                UnityEngine.Debug.Log$$Object("Project(v,w) = " + proj.toString());
                v.Normalize();
                w.Normalize();
                UnityEngine.Debug.Log$$Object("normalized v = " + v.toString());
                UnityEngine.Debug.Log$$Object("normalized w = " + w.toString());
            }
        }
    }
};
JsTypes.push(V3Test);
var AwakeA = {
    fullname: "AwakeA",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.valueOfA = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            var b = this.GetComponent$1(AwakeB.ctor);
            UnityEngine.MonoBehaviour.print(System.String.Format$$String$$Object$$Object("A.GetComponent<B>: {0}, value: {1}", b.get_name(), b.valueOfB));
        }
    }
};
JsTypes.push(AwakeA);
var AwakeB = {
    fullname: "AwakeB",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.valueOfB = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            var a = this.GetComponent$1(AwakeA.ctor);
            UnityEngine.MonoBehaviour.print(System.String.Format$$String$$Object$$Object("B.GetComponent<A>: {0}, value: {1}", a.get_name(), a.valueOfA));
            var c = this.get_gameObject().AddComponent$1(AwakeC.ctor);
            UnityEngine.MonoBehaviour.print("c.valueOfC = " + c.valueOfC);
        }
    }
};
JsTypes.push(AwakeB);
var AwakeC = {
    fullname: "AwakeC",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.valueOfC = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.valueOfC = 8;
        }
    }
};
JsTypes.push(AwakeC);
var CallJSTest = {
    fullname: "CallJSTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            JSMgr.vCall.CallJSFunctionName(0, "CreateJSBindingInfo", new Array(0));
            var obj = JSMgr.datax.getObject(2);
            if (obj != null){
                var csObj = Cast(obj, CSRepresentedObject.ctor);
                this.PrintJSBindingInfo(csObj.jsObjID);
            }
            else {
                UnityEngine.Debug.Log$$Object("obj is null");
            }
        },
        PrintJSBindingInfo: function (objID){
            JSApi.getProperty(objID, "Version");
            var s = JSApi.getStringS(3);
            UnityEngine.MonoBehaviour.print(s);
            JSApi.getProperty(objID, "QQGroup");
            var i = JSApi.getInt32(3);
            UnityEngine.MonoBehaviour.print(i);
            JSMgr.vCall.CallJSFunctionName(objID, "getDocumentUrl", new Array(0));
            s = JSApi.getStringS(2);
            UnityEngine.MonoBehaviour.print(s);
        },
        Update: function (){
        }
    }
};
JsTypes.push(CallJSTest);
var ComponentTest = {
    fullname: "ComponentTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var eb = this.GetComponent$1(TEnemyBase.ctor);
            if (UnityEngine.Object.op_Inequality(eb, null)){
                eb.set_enemyName("BULL");
                UnityEngine.Debug.Log$$Object("enemyName = " + eb.get_enemyName());
            }
            else {
                UnityEngine.Debug.Log$$Object("GetComponent<TEnemyBase>() returns null!");
            }
            this.get_gameObject().AddComponent$1(MentosKXT.ctor);
        },
        Update: function (){
        }
    }
};
JsTypes.push(ComponentTest);
var MentosKXT = {
    fullname: "MentosKXT",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.bUpdatePrinted = false;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.MonoBehaviour.print("Hello this is MentosKXT Start()");
        },
        Update: function (){
            if (!this.bUpdatePrinted){
                this.bUpdatePrinted = true;
                UnityEngine.MonoBehaviour.print("Hello this is MentosKXT Update()");
            }
        }
    }
};
JsTypes.push(MentosKXT);
var TEnemy = {
    fullname: "TEnemy",
    baseTypeName: "TEnemyBase",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.pos = new UnityEngine.Vector3.ctor();
            TEnemyBase.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
        }
    }
};
JsTypes.push(TEnemy);
var TEnemyBase = {
    fullname: "TEnemyBase",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._enemyID = 0;
            this._enemyName = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        enemyID$$: "System.Int32",
        get_enemyID: function (){
            return this._enemyID;
        },
        set_enemyID: function (value){
            this._enemyID = value;
        },
        enemyName$$: "System.String",
        get_enemyName: function (){
            return this._enemyName;
        },
        set_enemyName: function (value){
            this._enemyName = value;
        },
        Start: function (){
        },
        Update: function (){
        }
    }
};
JsTypes.push(TEnemyBase);
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
        WaitForCangJingKong: function*(){
            yield(new UnityEngine.WaitForSeconds.ctor(2));
        },
        DoTest: function*(){
            UnityEngine.Debug.Log$$Object("DoTest 1");
            yield(null);
            UnityEngine.Debug.Log$$Object("DoTest 2");
            yield(new UnityEngine.WaitForSeconds.ctor(1));
            var www = new UnityEngine.WWW.ctor$$String("file://" + UnityEngine.Application.get_dataPath() + "/JSBinding/Samples/Coroutine/CoroutineReadme.txt");
            yield(www);
            UnityEngine.Debug.Log$$Object("DoTest 3 Text from WWW: " + www.get_text());
            yield(this.StartCoroutine$$IEnumerator(this.WaitForCangJingKong()));
            UnityEngine.Debug.Log$$Object("DoTest 4 Wait for CangJingKong finished!");
        },
        DoTest2: function*(a){
            UnityEngine.Debug.Log$$Object("will call action 2 seconds later");
            yield(new UnityEngine.WaitForSeconds.ctor(2));
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
var JSImpTest1 = {
    fullname: "JSImpTest1",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.elapsed = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
            this.elapsed += UnityEngine.Time.get_deltaTime();
            if (this.elapsed > 1){
                this.elapsed = 0;
                var dict = new System.Collections.Generic.Dictionary$2.ctor(System.String.ctor, System.String.ctor);
                dict.Add("firstName", "De Hua");
                dict.Add("lastName", "Liu");
                dict.Add("age", "55");
                var person = XmlParser.ComvertType$1(JSImpTest1.Person.ctor, dict);
                UnityEngine.Debug.Log$$Object(new System.Text.StringBuilder.ctor().AppendFormat$$String$$Object$$Object$$Object("{0} {1}, {2}", person.lastName, person.firstName, person.age));
            }
        }
    }
};
JsTypes.push(JSImpTest1);
var XmlParser = {
    fullname: "XmlParser",
    baseTypeName: "System.Object",
    staticDefinition: {
        ComvertType$1: function (T, dict){
            var obj = jsimp.Reflection.CreateInstance$1(T);
            var $it4 = dict.GetEnumerator();
            while ($it4.MoveNext()){
                var ele = $it4.get_Current();
                var fieldName = ele.get_Key();
                var fieldValue = ele.get_Value();
                jsimp.Reflection.SetFieldValue(obj, fieldName, fieldValue);
            }
            return obj;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(XmlParser);
var MiscTest = {
    fullname: "MiscTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            PerTest.testString([null, "abc"]);
        },
        PrintStrings: function (s, strs){
            for (var $i6 = 0,$l6 = strs.length,v = strs[$i6]; $i6 < $l6; $i6++, v = strs[$i6])
                UnityEngine.MonoBehaviour.print(v);
        }
    }
};
JsTypes.push(MiscTest);
var AnimationCurveTest = {
    fullname: "AnimationCurveTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.curve = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var s = "";
            s += "length " + this.curve.get_length() + "\n";
            s += "prevWrapMode " + this.curve.get_preWrapMode() + "\n";
            s += "postWrapMode " + this.curve.get_postWrapMode() + "\n";
            s += "\n";
            for (var f = -3; f < 3; f += 0.4){
                s += f.toString() + " = " + this.curve.Evaluate(f) + "\n";
            }
            UnityEngine.MonoBehaviour.print(s);
        },
        Update: function (){
        }
    }
};
JsTypes.push(AnimationCurveTest);
var Car = {
    fullname: "Car",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.wheels = null;
            this.goWheels = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            for (var $i7 = 0,$t7 = this.wheels,$l7 = $t7.length,w = $t7[$i7]; $i7 < $l7; $i7++, w = $t7[$i7]){
                if (UnityEngine.Object.op_Inequality(w, null)){
                    w.setSpeed(UnityEngine.Random.Range$$Single$$Single(1, 4));
                }
            }
            for (var $i8 = 0,$t8 = this.goWheels,$l8 = $t8.length,go = $t8[$i8]; $i8 < $l8; $i8++, go = $t8[$i8]){
                if (UnityEngine.Object.op_Inequality(go, null)){
                    var w = go.GetComponent$1(Wheel.ctor);
                    if (UnityEngine.Object.op_Inequality(w, null)){
                        w.setSpeed(UnityEngine.Random.Range$$Single$$Single(1, 4));
                    }
                }
            }
        }
    }
};
JsTypes.push(Car);
var SerializeSimple = {
    fullname: "SerializeSimple",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.age = 0;
            this.shortAge = 0;
            this.go = null;
            this.firstName = "QIU";
            this.doYouLoveMe = false;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.Debug.Log$$Object("age: " + this.age);
            UnityEngine.Debug.Log$$Object("shortAge: " + this.shortAge);
            if (UnityEngine.Object.op_Inequality(this.go, null))
                UnityEngine.Debug.Log$$Object("go: " + this.go.get_name());
            else
                UnityEngine.Debug.Log$$Object("go: null");
            UnityEngine.Debug.Log$$Object("firstName: " + this.firstName);
            UnityEngine.Debug.Log$$Object("doYouLoveMe: " + (this.doYouLoveMe ? "true" : "false"));
        },
        Update: function (){
        }
    }
};
JsTypes.push(SerializeSimple);
var SerializeStruct = {
    fullname: "SerializeStruct",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.appleInfo = new SerializeStruct.AppleInfo.ctor();
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.Debug.Log$$Object("age: " + this.appleInfo.age);
            if (UnityEngine.Object.op_Inequality(this.appleInfo.go, null))
                UnityEngine.Debug.Log$$Object("go: " + this.appleInfo.go.get_name());
            else
                UnityEngine.Debug.Log$$Object("go: null");
            UnityEngine.Debug.Log$$Object("firstName: " + this.appleInfo.firstName);
            UnityEngine.Debug.Log$$Object("doYouLoveMe: " + (this.appleInfo.doYouLoveMe ? "true" : "false"));
        },
        Update: function (){
        }
    }
};
JsTypes.push(SerializeStruct);
var Wheel = {
    fullname: "Wheel",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.ePos = Wheel.Pos.LeftFront;
            this.speed = 1;
            this.accum = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        setSpeed: function (speed){
            this.speed = speed;
        },
        Start: function (){
        },
        Update: function (){
            this.accum += UnityEngine.Time.get_deltaTime();
            if (this.accum > this.speed){
                switch (this.ePos){
                    case Wheel.Pos.LeftFront:
                        UnityEngine.Debug.Log$$Object("LeftFront go..");
                        break;
                    case Wheel.Pos.RightFront:
                        UnityEngine.Debug.Log$$Object("RightFront go..");
                        break;
                    case Wheel.Pos.LeftBack:
                        UnityEngine.Debug.Log$$Object("LeftBack go..");
                        break;
                    case Wheel.Pos.RightBack:
                        UnityEngine.Debug.Log$$Object("RightBack go..");
                        break;
                }
                this.accum = 0;
            }
        }
    }
};
JsTypes.push(Wheel);
var StreamTest = {
    fullname: "StreamTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var count;
            var byteArray;
            var charArray;
            var uniEncoding = new System.Text.UnicodeEncoding.ctor();
            var firstString = uniEncoding.GetBytes$$String("Invalid file path characters are: ");
            var secondString = uniEncoding.GetBytes$$String("123456789");
            var memStream = new System.IO.MemoryStream.ctor$$Int32(100);
            try{
                memStream.Write(firstString, 0, firstString.length);
                count = 0;
                while (count < secondString.length){
                    memStream.WriteByte(secondString[count++]);
                }
                UnityEngine.Debug.Log$$Object(System.String.Format$$String$$Object$$Object$$Object("Capacity = {0}, Length = {1}, Position = {2}\n", memStream.get_Capacity().toString(), memStream.get_Length().toString(), memStream.get_Position().toString()));
                memStream.Seek(0, 0);
                byteArray = new Uint8Array(memStream.get_Length());
                count = memStream.Read(byteArray, 0, 20);
                while (count < memStream.get_Length()){
                    byteArray[count++] = System.Convert.ToByte$$Int32(memStream.ReadByte());
                }
                charArray = new Array(uniEncoding.GetCharCount$$Byte$Array$$Int32$$Int32(byteArray, 0, count));
                uniEncoding.GetDecoder().GetChars$$Byte$Array$$Int32$$Int32$$Char$Array$$Int32(byteArray, 0, count, charArray, 0);
                UnityEngine.Debug.Log$$Object(charArray);
            }
            finally{
                memStream.Dispose();
            }
        }
    }
};
JsTypes.push(StreamTest);
var MindActUpMide$1 = {
    fullname: "MindActUpMide$1",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (T){
            this.T = T;
            this.Value = null;
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(MindActUpMide$1);
var XmlTest = {
    fullname: "XmlTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var ABC = enumMoneyType.Gold;
            UnityEngine.Debug.Log$$Object(ABC.toString());
            var textAssets = Cast(UnityEngine.Resources.Load$$String("ShopConfig"), UnityEngine.TextAsset.ctor);
            if (UnityEngine.Object.op_Inequality(textAssets, null)){
                UnityEngine.Debug.Log$$Object(textAssets.get_text());
            }
            else {
                UnityEngine.Debug.Log$$Object("unkonw error!");
            }
            var xml = new System.Xml.XmlDocument.ctor();
            xml.LoadXml(textAssets.get_text());
            UnityEngine.Debug.Log$$Object("xmlload");
            var xmlPackets = xml.SelectNodes$$String("root/Packets/Packet");
            var pp = Lavie.XmlUtils.Select$1$$XmlNodeList$$String$$T(System.String.ctor, xmlPackets, "ID", "1");
            UnityEngine.Debug.Log$$Object("pp=" + pp.toString());
            var v1 = Lavie.XmlUtils.NodeValue$1(ItemType.ctor, pp, "ID");
            UnityEngine.Debug.Log$$Object("ID == " + v1.toString());
            var daoJunodeList = pp.get_ChildNodes();
            UnityEngine.Debug.Log$$Object("daojulist Count = " + daoJunodeList.get_Count());
            var daoJumdata = Lavie.XmlUtils.CreateObjectFromXml$1$$XmlNodeList$$String(ShopItemData.ctor, daoJunodeList, "SubType");
            UnityEngine.Debug.Log$$Object("daojudata Count = " + daoJumdata.get_Count());
            var $it8 = daoJumdata.GetEnumerator();
            while ($it8.MoveNext()){
                var shopItemData = $it8.get_Current();
                UnityEngine.Debug.Log$$Object(shopItemData.get_ID());
            }
        }
    }
};
JsTypes.push(XmlTest);
var ShopItemData = {
    fullname: "ShopItemData",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._ID = null;
            this._ItemType = ItemType.Normal;
            this._ItemId = null;
            this._Currency = enumMoneyType.None;
            this._OldPrice = 0;
            this._CurPrice = 0;
            this._Recommend = false;
            this._RefreshRate = 0;
            this._Globe = 0;
            this._IsPack = false;
            this._ItemNum = 0;
            this._item = null;
            this._TimeNum = null;
            this._VipDayNum = null;
            this._BuyNumPrice = null;
            this._VIPLevel = null;
            this._DayNum = null;
            this._hadBuyNum = 0;
            this._maxBuyNum = 0;
            this._ShopCategory = 0;
            System.Object.ctor.call(this);
        },
        ID$$: "System.String",
        get_ID: function (){
            return this._ID;
        },
        set_ID: function (value){
            this._ID = value;
        },
        ItemType$$: "ItemType",
        get_ItemType: function (){
            return this._ItemType;
        },
        set_ItemType: function (value){
            this._ItemType = value;
        },
        ItemId$$: "System.String",
        get_ItemId: function (){
            return this._ItemId;
        },
        set_ItemId: function (value){
            this._ItemId = value;
        },
        Currency$$: "enumMoneyType",
        get_Currency: function (){
            return this._Currency;
        },
        set_Currency: function (value){
            this._Currency = value;
        },
        OldPrice$$: "System.Int32",
        get_OldPrice: function (){
            return this._OldPrice;
        },
        set_OldPrice: function (value){
            this._OldPrice = value;
        },
        CurPrice$$: "System.Int32",
        get_CurPrice: function (){
            return this._CurPrice;
        },
        set_CurPrice: function (value){
            this._CurPrice = value;
        },
        Recommend$$: "System.Boolean",
        get_Recommend: function (){
            return this._Recommend;
        },
        set_Recommend: function (value){
            this._Recommend = value;
        },
        RefreshRate$$: "System.Single",
        get_RefreshRate: function (){
            return this._RefreshRate;
        },
        set_RefreshRate: function (value){
            this._RefreshRate = value;
        },
        Globe$$: "System.Int32",
        get_Globe: function (){
            return this._Globe;
        },
        set_Globe: function (value){
            this._Globe = value;
        },
        IsPack$$: "System.Boolean",
        get_IsPack: function (){
            return this._IsPack;
        },
        set_IsPack: function (value){
            this._IsPack = value;
        },
        ItemNum$$: "System.Int32",
        get_ItemNum: function (){
            return this._ItemNum;
        },
        set_ItemNum: function (value){
            this._ItemNum = value;
        },
        item$$: "ItemCell",
        get_item: function (){
            return this._item;
        },
        set_item: function (value){
            this._item = value;
        },
        TimeNum$$: "LimeTimeNum",
        get_TimeNum: function (){
            return this._TimeNum;
        },
        set_TimeNum: function (value){
            this._TimeNum = value;
        },
        VipDayNum$$: "LimitVipDayNum",
        get_VipDayNum: function (){
            return this._VipDayNum;
        },
        set_VipDayNum: function (value){
            this._VipDayNum = value;
        },
        BuyNumPrice$$: "LimitBuyNumPrices",
        get_BuyNumPrice: function (){
            return this._BuyNumPrice;
        },
        set_BuyNumPrice: function (value){
            this._BuyNumPrice = value;
        },
        VIPLevel$$: "LimitVIPLevel",
        get_VIPLevel: function (){
            return this._VIPLevel;
        },
        set_VIPLevel: function (value){
            this._VIPLevel = value;
        },
        DayNum$$: "LimitDayNum",
        get_DayNum: function (){
            return this._DayNum;
        },
        set_DayNum: function (value){
            this._DayNum = value;
        },
        hadBuyNum$$: "System.Int32",
        get_hadBuyNum: function (){
            return this._hadBuyNum;
        },
        set_hadBuyNum: function (value){
            this._hadBuyNum = value;
        },
        maxBuyNum$$: "System.Int32",
        get_maxBuyNum: function (){
            return this._maxBuyNum;
        },
        set_maxBuyNum: function (value){
            this._maxBuyNum = value;
        },
        ShopCategory$$: "System.Int32",
        get_ShopCategory: function (){
            return this._ShopCategory;
        },
        set_ShopCategory: function (value){
            this._ShopCategory = value;
        },
        Clear: function (){
            this.set_item(null);
            this.set_TimeNum(null);
            this.set_VipDayNum(null);
            this.set_BuyNumPrice(null);
        }
    }
};
JsTypes.push(ShopItemData);
var LimitDayNum = {
    fullname: "LimitDayNum",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    interfaceNames: ["IShopLime"],
    Kind: "Class",
    definition: {
        ctor: function (){
            this._Count = 0;
            System.Object.ctor.call(this);
        },
        Count$$: "System.Int32",
        get_Count: function (){
            return this._Count;
        },
        set_Count: function (value){
            this._Count = value;
        }
    }
};
JsTypes.push(LimitDayNum);
var LimitVIPLevel = {
    fullname: "LimitVIPLevel",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._Level = 0;
            System.Object.ctor.call(this);
        },
        Level$$: "System.Int32",
        get_Level: function (){
            return this._Level;
        },
        set_Level: function (value){
            this._Level = value;
        }
    }
};
JsTypes.push(LimitVIPLevel);
var LimitBuyNumPrices = {
    fullname: "LimitBuyNumPrices",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    interfaceNames: ["IShopLime"],
    Kind: "Class",
    definition: {
        ctor: function (){
            this._Prices = null;
            System.Object.ctor.call(this);
        },
        Prices$$: "System.Int32[]",
        get_Prices: function (){
            return this._Prices;
        },
        set_Prices: function (value){
            this._Prices = value;
        }
    }
};
JsTypes.push(LimitBuyNumPrices);
var LimitVipDayNum = {
    fullname: "LimitVipDayNum",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    interfaceNames: ["IShopLime"],
    Kind: "Class",
    definition: {
        ctor: function (){
            this._Count = null;
            System.Object.ctor.call(this);
        },
        Count$$: "System.Int32[]",
        get_Count: function (){
            return this._Count;
        },
        set_Count: function (value){
            this._Count = value;
        }
    }
};
JsTypes.push(LimitVipDayNum);
var LimeTimeNum = {
    fullname: "LimeTimeNum",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    interfaceNames: ["IShopLime"],
    Kind: "Class",
    definition: {
        ctor: function (){
            this._Interval = 0;
            this._Count = 0;
            System.Object.ctor.call(this);
        },
        Interval$$: "System.Int32",
        get_Interval: function (){
            return this._Interval;
        },
        set_Interval: function (value){
            this._Interval = value;
        },
        Count$$: "System.Int32",
        get_Count: function (){
            return this._Count;
        },
        set_Count: function (value){
            this._Count = value;
        }
    }
};
JsTypes.push(LimeTimeNum);
var IShopLime = {
    fullname: "IShopLime",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Interface"
};
JsTypes.push(IShopLime);
var ItemType = {
    fullname: "ItemType",
    staticDefinition: {
        Normal: 1,
        Treaure: 2,
        TreasureSoul: 3,
        TreasureChip: 4,
        Equip: 5,
        EquipChip: 6,
        PartnerSoul: 7,
        PartnerChip: 8,
        Cheats: 9,
        Stone: 10
    },
    Kind: "Enum"
};
JsTypes.push(ItemType);
var ItemCell = {
    fullname: "ItemCell",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._ID = 0;
            this._icon = null;
            this._iconAtlas = null;
            this._name = null;
            this._number = 0;
            this._phases = 0;
            this._description = null;
            this._type = 0;
            this._value = 0;
            this._maxStack = 0;
            this._timeLimit = 0;
            this._bSold = false;
            this._bDestoryed = false;
            this._sArtNamePath = null;
            this._color = ColorSign.White;
            this._expSupply = 0;
            System.Object.ctor.call(this);
        },
        ID$$: "System.Int32",
        get_ID: function (){
            return this._ID;
        },
        set_ID: function (value){
            this._ID = value;
        },
        icon$$: "System.String",
        get_icon: function (){
            return this._icon;
        },
        set_icon: function (value){
            this._icon = value;
        },
        iconAtlas$$: "System.String",
        get_iconAtlas: function (){
            return this._iconAtlas;
        },
        set_iconAtlas: function (value){
            this._iconAtlas = value;
        },
        name$$: "System.String",
        get_name: function (){
            return this._name;
        },
        set_name: function (value){
            this._name = value;
        },
        number$$: "System.Int32",
        get_number: function (){
            return this._number;
        },
        set_number: function (value){
            this._number = value;
        },
        phases$$: "System.Int32",
        get_phases: function (){
            return this._phases;
        },
        set_phases: function (value){
            this._phases = value;
        },
        description$$: "System.String",
        get_description: function (){
            return this._description;
        },
        set_description: function (value){
            this._description = value;
        },
        type$$: "System.Int32",
        get_type: function (){
            return this._type;
        },
        set_type: function (value){
            this._type = value;
        },
        value$$: "System.Int32",
        get_value: function (){
            return this._value;
        },
        set_value: function (value){
            this._value = value;
        },
        maxStack$$: "System.Int32",
        get_maxStack: function (){
            return this._maxStack;
        },
        set_maxStack: function (value){
            this._maxStack = value;
        },
        timeLimit$$: "System.Int32",
        get_timeLimit: function (){
            return this._timeLimit;
        },
        set_timeLimit: function (value){
            this._timeLimit = value;
        },
        bSold$$: "System.Boolean",
        get_bSold: function (){
            return this._bSold;
        },
        set_bSold: function (value){
            this._bSold = value;
        },
        bDestoryed$$: "System.Boolean",
        get_bDestoryed: function (){
            return this._bDestoryed;
        },
        set_bDestoryed: function (value){
            this._bDestoryed = value;
        },
        sArtNamePath$$: "System.String",
        get_sArtNamePath: function (){
            return this._sArtNamePath;
        },
        set_sArtNamePath: function (value){
            this._sArtNamePath = value;
        },
        color$$: "ColorSign",
        get_color: function (){
            return this._color;
        },
        set_color: function (value){
            this._color = value;
        },
        expSupply$$: "System.Int32",
        get_expSupply: function (){
            return this._expSupply;
        },
        set_expSupply: function (value){
            this._expSupply = value;
        }
    }
};
JsTypes.push(ItemCell);
var ColorSign = {
    fullname: "ColorSign",
    staticDefinition: {
        White: 1,
        Green: 2,
        Blue: 3,
        Purple: 4,
        Orange: 5,
        Red: 6,
        Black: 7
    },
    Kind: "Enum"
};
JsTypes.push(ColorSign);
var enumMoneyType = {
    fullname: "enumMoneyType",
    staticDefinition: {
        None: 0,
        Gold: 1,
        Diamon: 2,
        iGoldPill: 3,
        iDragonScale: 4,
        iPhoenixFeather: 5,
        iGodSoul: 6,
        iBiasJade: 7,
        iBuddHistrelics: 8,
        iContribution: 9,
        iPrestige: 10
    },
    Kind: "Enum"
};
JsTypes.push(enumMoneyType);
var Lavie$XmlUtils = {
    fullname: "Lavie.XmlUtils",
    baseTypeName: "System.Object",
    staticDefinition: {
        AssignObjectValueFromXml: function (mNode, mData, mDataType){
            var $it9 = mNode.get_Attributes().GetEnumerator();
            while ($it9.MoveNext()){
                var xmlAttribute = $it9.get_Current();
                var fieldName = xmlAttribute.get_Name();
                var value = mNode.get_Attributes().GetNamedItem$$String(fieldName).get_Value().toString();
                var fieldValue = null;
                if (jsimp.Reflection.PropertyTypeIsIntArray(mDataType, fieldName)){
                    fieldValue = Lavie.XmlUtils.ConvertString2IntArray(value);
                }
                else {
                    var fieldType = jsimp.Reflection.GetPropertyType(mDataType, fieldName);
                    if (fieldType == null){
                        continue;
                    }
                    fieldValue = Lavie.XmlUtils.ConvertString2ActualType(fieldType, value);
                }
                jsimp.Reflection.SetPropertyValue(mData, fieldName, fieldValue);
            }
        },
        CreateObjectFromXml$$XmlNode$$Type: function (mNode, type){
            var mData = jsimp.Reflection.CreateInstance$$Type(type);
            Lavie.XmlUtils.AssignObjectValueFromXml(mNode, mData, type);
            return mData;
        },
        CreateObjectFromXml$1$$XmlNode: function (T, mNode){
            var mData = jsimp.Reflection.CreateInstance$1(T);
            Lavie.XmlUtils.AssignObjectValueFromXml(mNode, mData, Typeof(T));
            return mData;
        },
        CreateObjectFromXml$1$$XmlNodeList$$String: function (T, nodeList, subType){
            var list = new System.Collections.Generic.List$1.ctor(T);
            var $it10 = nodeList.GetEnumerator();
            while ($it10.MoveNext()){
                var mNode = $it10.get_Current();
                var mData = jsimp.Reflection.CreateInstance$1(T);
                Lavie.XmlUtils.AssignObjectValueFromXml(mNode, mData, Typeof(T));
                if (subType.length > 0 && mNode.get_HasChildNodes()){
                    var $it11 = mNode.get_ChildNodes().GetEnumerator();
                    while ($it11.MoveNext()){
                        var childNode = $it11.get_Current();
                        var fieldName = Lavie.XmlUtils.NodeValue$1(System.String.ctor, childNode, subType);
                        var fieldType = jsimp.Reflection.GetPropertyType(Typeof(T), fieldName);
                        if (fieldType != null){
                            var fieldValue = Lavie.XmlUtils.CreateObjectFromXml$$XmlNode$$Type(childNode, fieldType);
                            jsimp.Reflection.SetPropertyValue(mData, fieldName, fieldValue);
                        }
                    }
                }
                list.Add(mData);
            }
            return list;
        },
        ConvertString2IntArray: function (value){
            var arr = (value.Split$$Char$Array([","]));
            var ret = new Int32Array(arr.length);
            for (var i = 0; i < arr.length; i++){
                ret[i] = System.Int32.Parse$$String(arr[i]);
            }
            return ret;
        },
        ConvertString2ActualType: function (type, value){
            var ret = null;
            if (type == Typeof(System.Int32.ctor)){
                ret = System.Int32.Parse$$String(value);
            }
            else if (type == Typeof(System.Single.ctor)){
                ret = System.Single.Parse$$String(value);
            }
            else if (type == Typeof(System.Boolean.ctor)){
                ret = (value == "1");
            }
            else if (type.get_IsEnum()){
                ret = System.Int32.Parse$$String(value);
            }
            else if (type == Typeof(System.String.ctor)){
                ret = value;
            }
            else {
            }
            return ret;
        },
        NodeValue$1: function (T, node, nodeName){
            var collection = node.get_Attributes();
            if (collection.get_Count() == 0){
                return Default(T);
            }
            var namedItem = collection.GetNamedItem$$String(nodeName);
            if (namedItem == null){
                return Default(T);
            }
            var typeT = Typeof(T);
            var value = namedItem.get_Value();
            return Cast(Lavie.XmlUtils.ConvertString2ActualType(typeT, value), T);
        },
        Select$1$$XmlNodeList$$String$$String$$String: function (T, xmlNodeList, nodeName, value, attribute){
            var $it12 = xmlNodeList.GetEnumerator();
            while ($it12.MoveNext()){
                var node = $it12.get_Current();
                if (Lavie.XmlUtils.NodeValue$1(T, node, nodeName).toString() == value){
                    return Lavie.XmlUtils.NodeValue$1(T, node, attribute);
                }
            }
            return Default(T);
        },
        Select$1$$XmlNodeList$$String$$T: function (T, xmlNodeList, nodeName, value){
            var $it13 = xmlNodeList.GetEnumerator();
            while ($it13.MoveNext()){
                var node = $it13.get_Current();
                var nodeValue = Lavie.XmlUtils.NodeValue$1(T, node, nodeName);
                if (jsimp.Reflection.SimpleTEquals$1(T, nodeValue, value)){
                    return node;
                }
            }
            return null;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(Lavie$XmlUtils);
var SampleViewer$stScene = {
    fullname: "SampleViewer.stScene",
    baseTypeName: "System.ValueType",
    assemblyName: "SharpKitProj",
    Kind: "Struct",
    definition: {
        ctor$$String$$String: function (a, b){
            this.levelName = null;
            this.showText = null;
            System.ValueType.ctor.call(this);
            this.levelName = a;
            this.showText = (b.length > 0 ? b : a);
        },
        ctor: function (){
            this.levelName = null;
            this.showText = null;
            System.ValueType.ctor.call(this);
        }
    }
};
JsTypes.push(SampleViewer$stScene);
var JSImpTest1$Person = {
    fullname: "JSImpTest1.Person",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.firstName = null;
            this.lastName = null;
            this.age = null;
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(JSImpTest1$Person);
var SerializeStruct$AppleInfo = {
    fullname: "SerializeStruct.AppleInfo",
    baseTypeName: "System.ValueType",
    assemblyName: "SharpKitProj",
    Kind: "Struct",
    definition: {
        ctor: function (){
            this.age = 0;
            this.go = null;
            this.firstName = null;
            this.doYouLoveMe = false;
            System.ValueType.ctor.call(this);
        }
    }
};
JsTypes.push(SerializeStruct$AppleInfo);
var Wheel$Pos = {
    fullname: "Wheel.Pos",
    staticDefinition: {
        LeftFront: 0,
        RightFront: 1,
        LeftBack: 2,
        RightBack: 3
    },
    Kind: "Enum"
};
JsTypes.push(Wheel$Pos);

