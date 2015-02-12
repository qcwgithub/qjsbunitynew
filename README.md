demo使用方法
------------------------------------------------------------------------------------

运行 UnityJSTestScene 这个场景。可能的错误：

1：InitJSEngine failed. Click menu [Assets -> JSBinding -> Generate JS and CS Bindings]
解决办法：就点一下那个菜单！

2. DllNotFound 错误：
解决办法：拷贝 Plugins/x86/mozjs-28.dll 到 Unity 安装目录下

3. 拷贝了 dll 后还是 DllNotFound
解决办法：请确认你有 VS2012 runtime！那个 dll 是用 VS2012 编译的 release 版

到此你应该可以正常运行，如果没有，你运气太差了！请联系我(QQ 65069822)。


支持的平台：
------------------------------------------------------------------------------------
32位Windows编辑器、32位Windows可执行文件
32位Mac编辑器、32位Mac可执行文件
安卓
iOS 32位及64位

使用 Firefox 调试 JS
------------------------------------------------------------------------------------
请看 Firefox调试说明.png！
注意：目前只有 Windows 和 Mac 可以调试，Android 和 iOS 也可以支持，暂时没有


导出类配置：
------------------------------------------------------------------------------------
1. 在 JSBindingSettings 文件中添加你要的枚举，或者类名！
2. 点击菜单 [Assets -> JSBinding -> Generate JS and CS Bindings]
3. 可以在 JS 中使用了


杂项说明
------------------------------------------------------------------------------------
1. 包含文件 CS.require
2. 在 js 中调用 +-*/ 操作符
    // js code
    var a = new Vector3(1,0,0)
    var b = new Vector3(0,1,1)
    var c = Vector3.op_Addition(a, b);

    // /  -> op_Division
    // *  -> op_Multiply
    // -  -> op_Subtraction
    // -  -> op_UnaryNegation
    // == -> op_Equality
    // != -> op_Inequality
3. 由于 JS 中已经有名为 Object 的对象，UnityEngine 导到 JS 中变成 UnityObject
4. 任何出错都会在Unity控制台打印出调用堆栈，请注意查看
5. 大部分用法已经在 demo 中有展示了




...未完待续