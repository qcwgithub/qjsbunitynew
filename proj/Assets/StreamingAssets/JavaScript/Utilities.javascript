/*
* Make sure namespace object exists
* Usage:
* List = MakeNS("System.Collections.Generic").List = {};
*/
var _G = this;
function MakeNS(str)
{
    var arr = str.split('.') || [];
    var obj = _G;
    arr.forEach(function(ns) {
        if (obj[ns] === undefined)
            obj[ns] = {};
        obj = obj[ns];
    });
    return obj;
}

function Compile()
{
	JsTypes.forEach(function(t){
        Debug.Log("Compile: " + t.fullname);
	});
}

Compile();

var _Test = {
    print: function() { Debug.Log(this.v); },
	v: 5
}

var _Apple = {
	print: _Test.print,
	v: 9
};

_Test.print()
_Apple.print()