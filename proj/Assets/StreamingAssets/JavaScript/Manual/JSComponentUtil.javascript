// 这个是手写的
// 反正是用于做一些事情！
// 看一下这里面包含的函数


if (typeof(JsTypes) == "undefined")
    var JsTypes = [];

if (typeof(window) == 'undefined')
    var window = this;
	
var JSComponentUtil = {
    fullname: "JSComponentUtil",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        // 用于判断2个类是不是继承关系
		IsInheritanceRel: function (baseClassName, subClassName)
        {
            //print(baseClassName + "/" + subClassName);
            var arr = subClassName.split(".");
            var obj = window;
            arr.forEach(function (a) {
                if (obj)
                    obj = obj[a];
            });

            if (obj != undefined && obj !== this) {
                while (true) {
                    if (obj.baseType != undefined) {
                        if (obj.baseType.fullname == baseClassName)
                            return true;

                        if (obj.interfaceNames !== undefined) {
                            for (var i in obj.interfaceNames) {
                                //print(baseClassName + " Y " + subClassName);
                                if (obj.interfaceNames[i] == baseClassName) {
                                    return true;
                                }
                            }
                        }
                        obj = obj.baseType;
                    }
                    else {
                        break;
                    }
                }
            }
            //print(baseClassName + " X " + subClassName);
            return false;
		}
    }
};
JsTypes.push(JSComponentUtil);

