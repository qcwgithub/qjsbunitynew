demo使用方法
------------------------------------------------------------------------------------

运行 UnityJSTestScene 这个场景。可能的错误：

1. InitJSEngine failed. Click menu [Assets -> JSBinding -> Generate JS and CS Bindings]
解决办法：就点一下那个菜单！

2. DllNotFound 错误：
解决办法：拷贝 Plugins/x86/mozjs-28.dll 到 Unity 安装目录下

3. 拷贝了 dll 后还是 DllNotFound
解决办法：请确认你有 VS2012 runtime！那个 dll 是用 VS2012 编译的 release 版

到此你应该可以正常运行，如果没有，你运气太差了！请联系我(QQ 65069822)。


支持的平台：
------------------------------------------------------------------------------------
1. 32位Windows编辑器、32位Windows可执行文件
2. 32位Mac编辑器、32位Mac可执行文件
3. 安卓
4. iOS 32位及64位

使用 Firefox 调试 JS
------------------------------------------------------------------------------------
请看 Firefox调试说明.png！
注意：目前只有 Windows 和 Mac 可以调试，Android 和 iOS 也可以支持，暂时没有


导出类配置：
------------------------------------------------------------------------------------
1. 在 JSBindingSettings 文件中添加你要的枚举，或者类名！
2. 点击菜单 [Assets -> JSBinding -> Generate JS and CS Bindings]
3. 可以在 JS 中使用了


杂项说明（必读）
------------------------------------------------------------------------------------
0. 大部分用法已经在 demo 中有展示了
1. 包含文件 CS.require(fileName, this); 第2个函数可以不填，不填代表global
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
5. 为什么JS代码后缀是 .javascript？因为如果是 .js 的话会被Unity认为是 UnityScript
6. JS的2种用法
    6.1 像 demo 里的 RotateObject.javascript那样，做为 一个 'JSComponent' 使用，里面包含 Awake, Start, Update 等函数。然后在某个 GameObject 身上绑定 JSComponent 组件，脚本名写上 RotateObject.javascript（填相对路径，相对于StreamingAssets/JavaScript/）。这种用法主要用来做为程序的入口
    6.2 不做为 JSComponent 的 JS 文件。你自己通过 CS.require 来包含你的 JS 代码
7. 传递函数指针给C#的用法。
    7.1 通过 property 传递。uiEventListener.onClick = jsOnClick;
    7.2 通过函数参数传递。xxx.SetDeletate(someJSFunction);
    7.3 必须注意！你的JS函数自己要存着！不能被垃圾回收了。
8. 如果传递带有 ref/out 的结构体或类参数？直接传就行了！但是必须先new一个对象出来。例如 
        var hit = new RaycastHit(); //先new一个
        Physics.Raycast(ray, hit);  //传进去，一会取hit就是新的值了

9. 不支持泛型！有带泛型的函数都不能使用
10. 支持函数重载
11. StreamingAssets/JavaScript/Generated/ 下的脚本一开始会全部被加载

...未完待续