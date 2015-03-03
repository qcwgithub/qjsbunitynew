var _G = this;
/*
* Make sure namespace object exists
*/
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

/*
* Usage
*/
// List = MakeNS("System.Collections.Generic").List = {};