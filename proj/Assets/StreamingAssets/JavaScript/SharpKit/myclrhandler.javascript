
/*
* Before Compile:
* Set MonoBehaviour.ctor to empty function
*/
// UnityEngine.MonoBehaviour.ctor = function () {}

/*
* Compile Now !!
*/
Compile();


// print = UnityEngine.Debug.Log$$Object;

//print(UnityEngine.Vector3.ctor.prototype.x);

/*
for (var v in Rotate)
{
	    print(v)
}*/

function jsb_NewMonoBehaviour(name, nativeObj) 
{
    var jsType = this[name];
    if (jsType && jsType.ctor) {
        var obj = new jsType.ctor();
        obj.__nativeObj = nativeObj;
        return obj;
    }
    return undefined;
}

function jsb_NewObject(name)
{
    var arr = name.split(".");
    var obj = this;
    arr.forEach(function (a) {
        if (obj)
            obj = obj[a];
    });
    if (obj && obj.ctor) {
        var o = {};
        o.__proto__ = obj.ctor.prototype;
        return o;
    }
    return undefined;
}

function jsb_CallObjectCtor(name)
{
    var arr = name.split(".");
    var obj = this;
    arr.forEach(function (a) {
        if (obj)
            obj = obj[a];
    });
    if (obj && obj.ctor) {
        return new obj.ctor();
    }
    return undefined;
}

function jsb_formatParamsArray(preCount, argArray, funArguments)
{
    if (Object.prototype.toString.apply(argArray) === "[object Array]") {
        return argArray;
    } else {
        return Array.prototype.slice.apply(funArguments).slice(preCount);
    }
}