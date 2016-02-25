/*
*
* JSRepresentedObject
* to represent C# object whose type is unknown to JS
* currently, it is only used to represent C# delegate objects, which only exists in C#, and are only available in C#
*
*/

if (typeof(JsTypes) == "undefined")
    var JsTypes = [];

JsTypes.push({
    definition: {},
    staticDefinition: {},
    fields: {},
    staticFields: {},
    assemblyName: "",
    Kind: "Class",
    fullname: "JSRepresentedObject"
});


/*
* Before Compile:
* Set MonoBehaviour.ctor to empty function
*/
// UnityEngine.MonoBehaviour.ctor = function () {}

/*
* Compile Now !!
*/

var printError = function () {};
var print = function () {};
(function () {
    for (var i = 0; i < JsTypes.length; i++) {
        if (JsTypes[i].fullname == "UnityEngine.Debug") {
            printError = JsTypes[i].staticDefinition.LogError$$Object;
            print = JsTypes[i].staticDefinition.Log$$Object;
            break;
        }
    }
}());

/*
* Sort JsTypes before Compile()
* if we have 2 types: A.B.C and A.B
* A.B will be in front of A.B.C after sort
*/
JsTypes.sort(function (a, b) {
    return (a.fullname < b.fullname ? -1 : 1);
});

/*

var str = "";
for (var i = 0; i < JsTypes.length; i++) {
    str += JsTypes[i].fullname + "\n";
}
print(str);
    
*/

try
{
    Compile();
}
catch (ex) 
{
    //if (ex.message)
    //    printError("JS Error! Error: " + ex.message + "\n\nStack: \n" + ex.stack);
    //else
        printError("JS Error! Error: " + ex + "\n\nStack: \n" + ex.stack);
}

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

function jsb_IsInheritanceRel(baseClassName, subClassName)
{
    var arr = subClassName.split(".");
    var obj = window;
    arr.forEach(function (a) {
        if (obj)
            obj = obj[a];
    });
    
    if (obj == undefined || obj === this)
        return false;

    while (true) {
        if (obj.baseType != undefined) {
            if (obj.baseType.fullname == baseClassName)
                return true;

            if (obj.interfaceNames !== undefined) {
                for (var i in obj.interfaceNames) {
                    if (obj.interfaceNames[i] == baseClassName) {
                        return true;
                    }
                }
            }
            obj = obj.baseType;
        }
        else break;
    }
    return false;
}