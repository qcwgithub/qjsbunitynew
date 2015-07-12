_jstype = undefined;
for (var i = 0; i < JsTypes.length; i++) {
    if (JsTypes[i].fullname == "UnityEngine.MonoBehaviour") {
        _jstype = JsTypes[i];
        break;
    }
}

if (_jstype) {

    _jstype.definition.StartCoroutine$$String$$Object = function(a0/*String*/, a1/*Object*/) { 
        
        return CS.Call(4, 174, 6, false, this, a0, a1); 
    }
    _jstype.definition.StartCoroutine$$String = function(a0/*String*/) { 
        if (this[a0]) 
        {
            var fiber = this[a0].call(this);
            return this.$AddCoroutine(fiber);
        }
    }
    _jstype.definition.StartCoroutine$$IEnumerator = function(a0/*IEnumerator*/) { 
        return this.$AddCoroutine(a0);
    }
    _jstype.definition.StartCoroutine_Auto = function(a0/*IEnumerator*/) { 
        
        return CS.Call(4, 174, 9, false, this, a0); 
    }
    _jstype.definition.StopAllCoroutines = function() { 
        
        return CS.Call(4, 174, 10, false, this); 
    }
    _jstype.definition.StopCoroutine$$Coroutine = function(a0/*Coroutine*/) { 
        
        return CS.Call(4, 174, 11, false, this, a0); 
    }
    _jstype.definition.StopCoroutine$$String = function(a0/*String*/) { 
        
        return CS.Call(4, 174, 12, false, this, a0); 
    }

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
    _jstype.definition.$AddCoroutine = function (fiber) {
        var coroutineNode = {
            $__CN: true,  // mark this is a coroutine node
            prev: undefined,
            next: undefined,
            fiber: fiber,
            finished: false,

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
    }

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
    }

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
    }

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
    }
}