_jstype = undefined;
for (var i = 0; i < JsTypes.length; i++) {
    if (JsTypes[i].fullname == "UnityEngine.WaitForSeconds") {
        _jstype = JsTypes[i];
        break;
    }
}

if (_jstype) {

    _jstype.definition.ctor = function(a0) { 
        this.$totalTime = a0;
        this.$elapsedTime = 0;
        this.$finished = false;
    }

    _jstype.definition.get_finished = function(elapsed) { 
        if (!this.$finished) {
            this.$elapsedTime += elapsed;
            if (this.$elapsedTime >= this.$totalTime) {
                this.$finished = true;
            }        
        }
        return this.$finished;
    }
}