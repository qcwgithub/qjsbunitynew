/* 全局域 */

if (this.print === undefined) {
    this.print = Debug.Log;
    this.printError = Debug.LogError;
    this.printWarning = Debug.LogWarning;
}

//print = app.console.print;

this.pi = {};
this.app = {};
this.pi.net = {}
this.pi.math = {};

// 还不能打印堆栈
if (this.assert === undefined) {
    this.assert = function (b) {
        if (!b) {
            AssertFail();
            Debug.LogError('ASSERT FAIL');
        }
    };
}


/*
* JS 调用 CS 如果出错，打印堆栈！
*/
CS.CSEntry = CS.Call;
CS.Call = function ()
{
    var ret = undefined;
    try
    {
        ret = CS.CSEntry.apply(CS, arguments);
    }
    catch (ex)
    {
        var str = "JS call CS Exception!\n\n";

        str += "----------- 1) CS Stack -----------\n\n";
        str += ex.toString() + "\n\n\n";

        str += "----------- 2) JS Stack -----------\n\n";
        str += ex.stack;

        printWarning(str);
    }
    return ret;
};

/*
* JS 自己出错，打印堆栈！----没用，无法使用
*/

/*
this.jsErrorReporter = function (str) {
    printWarning(str);
    return;

    try {
        throw Error();
    } catch (ex) {
        printWarning("JS Error! Stack:\n\n" + ex.stack);
    }
};
CS.SetErrorReporter(this.jsErrorReporter);
*/

CS.jsFunctionEntry = function ()
{
    var args = Array.prototype.slice.apply(arguments);
    var obj = args[0];
    var fun = args[1];
    var ret = undefined;

    try {
        ret = fun.apply(obj, args.slice(2));
    } catch (ex) {
        var str = "JS Error! Error:\n" + ex + "\n\nStack:\n\n" + ex.stack;
        printWarning(str);
    }
    return ret;
};