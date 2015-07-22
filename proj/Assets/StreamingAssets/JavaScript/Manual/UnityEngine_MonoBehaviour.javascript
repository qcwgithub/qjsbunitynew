_jstype = undefined;
for (var i = 0; i < JsTypes.length; i++) {
    if (JsTypes[i].fullname == "UnityEngine.MonoBehaviour") {
        _jstype = JsTypes[i];
        break;
    }
}

if (_jstype) {
    _jstype.definition.CancelInvoke$$String = function(a0/*String*/) {
        this.$RemoveInvokeByName(a0);
    };
    _jstype.definition.CancelInvoke = function() {
        this.$RemoveAllInvokes();
    };
    _jstype.definition.Invoke = function(a0/*String*/, a1/*Single*/) {
        if (this[a0]) {
            var fun = this[a0].bind(this);
            this.$AddInvoke(a0, fun, a1, 0, false);
        }
    };
    _jstype.definition.InvokeRepeating = function(a0/*String*/, a1/*Single*/, a2/*Single*/) {
        if (this[a0]) {
            var fun = this[a0].bind(this);
            this.$AddInvoke(a0, fun, a1, a2, true);
        }
    };
    _jstype.definition.IsInvoking$$String = function(a0/*String*/) {
        return this.$IsInvoking(a0);
    };
    _jstype.definition.IsInvoking = function() {
        return this.$IsInvoking(undefined);
    };
    _jstype.definition.StartCoroutine$$String$$Object = function(a0/*String*/, a1/*Object*/) {
        if (this[a0]) {
            var fiber = this[a0].call(this, a1);
            return this.$AddCoroutine(fiber, a0);
        }
    };
    _jstype.definition.StartCoroutine$$String = function(a0/*String*/) {
        if (this[a0]) {
            var fiber = this[a0].call(this);
            return this.$AddCoroutine(fiber, a0);
        }
    };
    _jstype.definition.StartCoroutine$$IEnumerator = function(a0/*IEnumerator*/) { 
        return this.$AddCoroutine(a0);
    };
    _jstype.definition.StartCoroutine_Auto = function(a0/*IEnumerator*/) {
        return this.$AddCoroutine(a0);
    };
    _jstype.definition.StopAllCoroutines = function() {
        return this.$RemoveAllCoroutines();
    };
    _jstype.definition.StopCoroutine$$IEnumerator = function(a0/*Coroutine*/) {
        return this.$RemoveCoroutineByFiber(a0);
    };
    _jstype.definition.StopCoroutine$$String = function(a0/*String*/) {
        return this.$RemoveCoroutineByName(a0);
    };
    //
    // Invoke Scheduler
    //
    _jstype.definition.$AddInvoke = function (funName, fun, delay, interval, bRepeat) {
        var invokeNode = {
            funName: funName,
            fun: fun,
            delay: delay,
            interval: interval,
            accum: 0,
            bRepeat: bRepeat,

            prev: undefined,
            next: undefined
        };
        if (this.$firstInvoke) {
            invokeNode.next = this.$firstInvoke;
            this.$firstInvoke.prev = invokeNode;
        };
        this.$firstInvoke = invokeNode;
    };
    _jstype.definition.$UpdateAllInvokes = function (elapsed) {
        var invoke = this.$firstInvoke,
            next,
            bCall;
        while (invoke != undefined) {
            next = invoke.next;
            bCall = false;
            if (invoke.delay > 0) {
                invoke.delay -= elapsed;
                if (invoke.delay <= 0) {
                    bCall = true;
                }
            } else {
                invoke.accum += elapsed;
                if (invoke.accum >= invoke.interval) {
                    invoke.accum -= invoke.interval;
                    bCall = true;
                }
            }
            if (bCall) {
                invoke.fun();
                if (!invoke.bRepeat) {
                    this.$RemoveInvoke(invoke);
                }
            }
            invoke = next;
        }
    };
    _jstype.definition.$IsInvoking = function (funName) {
        if (funName == undefined) {
            return (this.$firstInvoke != undefined);
        } else {
            var invoke = this.$firstInvoke,
                next,
                bCall;
            while (invoke != undefined) {
                next = invoke.next;
                if (invoke.funName == funName) {
                    return true;
                }
                invoke = next;
            }
            return false;
        }
    };
    _jstype.definition.$RemoveAllInvokes = function () {
        this.$firstInvoke = undefined;
    };
    _jstype.definition.$RemoveInvokeByName = function (funName) {
        var invoke = this.$firstInvoke,
            next,
            bCall;
        while (invoke != undefined) {
            next = invoke.next;
            if (invoke.funName == funName) {
                this.$RemoveInvoke(invoke);
            }
            invoke = next;
        }
    };
    _jstype.definition.$RemoveInvoke = function (invoke) {
        if (this.$firstInvoke == invoke) {
            this.$firstInvoke = invoke.next;
        }
        else {
            if (invoke.next != undefined) {
                invoke.prev.next = invoke.next;
                invoke.next.prev = invoke.prev;
            }
            else if (invoke.prev) {
                invoke.prev.next = undefined;
            }
        }
        invoke.prev = undefined;
        invoke.next = undefined;
    };

    //
    // Coroutine Scheduler
    // 
    // REFERENCE FROM
    // 
    // Coroutine Scheduler:
    // http://wiki.unity3d.com/index.php/CoroutineScheduler
    //
    // JavaScript yield documents:
    // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/yield
    // 

    // fiber 类似于 C# 的 IEnumerator
    _jstype.definition.$AddCoroutine = function (fiber, name) {
        var coroutineNode = {
            $__CN: true,  // mark this is a coroutine node
            prev: undefined,
            next: undefined,
            fiber: fiber,
            finished: false,
            name: name,

            waitForFrames: 0,          // yield null
            waitForSeconds: undefined, // WaitForSeconds
            www: undefined,            // WWW
            waitForCoroutine: undefined, // Coroutine
        };

        if (this.$first) {
            coroutineNode.next = this.$first;
            this.$first.prev = coroutineNode;
        };

        this.$first = coroutineNode;
        // NOTE
        // return coroutine node itself!
        return coroutineNode;
    };
    // this method is called from LateUpdate
    _jstype.definition.$UpdateAllCoroutines = function (elapsed) {
        // cn is short for Coroutine Node
        var cn = this.$first;
        while (cn != undefined) {
            // store next coroutineNode before it is removed from the list
            var next = cn.next;
            var update = false;

            if (cn.waitForFrames > 0) {
                cn.waitForFrames--;
                if (cn.waitForFrames <= 0) {
                    waitForFrames = 0;
                    this.$UpdateCoroutine(cn);
                }
            }
            else if (cn.waitForSeconds) {
                if (cn.waitForSeconds.get_finished(elapsed)) {
                    cn.waitForSeconds = undefined;
                    this.$UpdateCoroutine(cn);
                }
            }
            else if (cn.www) {
                if (cn.www.get_isDone()) {
                    cn.www = undefined;
                    this.$UpdateCoroutine(cn);
                }
            }
            else if (cn.waitForCoroutine) {
                if (cn.waitForCoroutine.finished == true) {
                    cn.waitForCoroutine = undefined;
                    this.$UpdateCoroutine(cn);
                }  
            }
            else {
                this.$UpdateCoroutine(cn);
            }
            cn = next;
        }
    };
    _jstype.definition.$UpdateCoroutine = function (cn) { // cn is short for Coroutine Node
        var fiber = cn.fiber;
        var obj = fiber.next();
        if (!obj.done) {
            var yieldCommand = obj.value;
            // UnityEngine.Debug.Log$$Object(JSON.stringify(yieldCommand));
            if (yieldCommand == null) {
                cn.waitForFrames = 1;
            }
            else {
                if (yieldCommand instanceof UnityEngine.WaitForSeconds.ctor) {
                    cn.waitForSeconds = yieldCommand;
                } 
                else if (yieldCommand instanceof UnityEngine.WWW.ctor) {
                    cn.www = yieldCommand;
                }
                else if (yieldCommand.$__CN === true/*yieldCommand.toString() == "[object Generator]"*/) {
                    cn.waitForCoroutine = yieldCommand;
                }
                else {
                    throw "Unexpected coroutine yield type: " + yieldCommand.GetType();
                }
            }
        } 
        else {
            // UnityEngine.Debug.Log$$Object("cn.finished = true;");
            cn.finished = true;
            this.$RemoveCoroutine(cn);
        }
    };
    _jstype.definition.$RemoveAllCoroutines = function () {
        this.$first = undefined;
    };
    _jstype.definition.$RemoveCoroutineByFiber = function (fiber) {
        var cn = this.$first;
        while (cn != undefined) {
            // store next coroutineNode before it is removed from the list
            var next = cn.next;
            if (cn.fiber === fiber) {
                this.$RemoveCoroutine(cn);
            }
            cn = next;
        }
    };
    _jstype.definition.$RemoveCoroutineByName = function (name) {
        var cn = this.$first;
        while (cn != undefined) {
            // store next coroutineNode before it is removed from the list
            var next = cn.next;
            if (cn.name == name) {
                this.$RemoveCoroutine(cn);
            }
            cn = next;
        }
    };
    _jstype.definition.$RemoveCoroutine = function (cn) { // cn is short for Coroutine Node
        if (this.$first == cn) {
            this.$first = cn.next;
        }
        else {
            if (cn.next != undefined) {
                cn.prev.next = cn.next;
                cn.next.prev = cn.prev;
            }
            else if (cn.prev) {
                cn.prev.next = undefined;
            }
        }
        cn.prev = undefined;
        cn.next = undefined;
    };
}