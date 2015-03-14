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
