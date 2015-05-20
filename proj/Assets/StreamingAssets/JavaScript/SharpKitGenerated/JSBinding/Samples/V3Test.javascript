if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var V3Test = {
    fullname: "V3Test",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj2010",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.elapsed = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
            this.elapsed += UnityEngine.Time.get_deltaTime();
            if (this.elapsed > 1){
                var sb = new System.Text.StringBuilder.ctor();
                this.elapsed = 0;
                var v = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(2, 3, 6);
                var w = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(7, 23, 1);
                var n = v.get_normalized();
                var arr = [n.x, n.y, n.z];
                UnityEngine.Debug.Log$$Object(sb.AppendFormat$$String$$Object$Array("v.normalized = ({0}, {1}, {2})", arr).toString());
                sb.Remove(0, sb.get_Length());
                var cross = UnityEngine.Vector3.Cross(v, w);
                arr = [cross.x, cross.y, cross.z];
                UnityEngine.Debug.Log$$Object(sb.AppendFormat$$String$$Object$Array("Cross(v, w) = ({0}, {1}, {2})", arr).toString());
                UnityEngine.Debug.Log$$Object("v.magnitude = " + v.get_magnitude());
                UnityEngine.Debug.Log$$Object("w.magnitude = " + w.get_magnitude());
                UnityEngine.Debug.Log$$Object("Dot(v, w) = " + UnityEngine.Vector3.Dot(v, w));
                UnityEngine.Debug.Log$$Object("Angle(v, w) = " + UnityEngine.Vector3.Angle(v, w));
                var proj = UnityEngine.Vector3.Project(v, w);
                UnityEngine.Debug.Log$$Object("Project(v,w) = " + proj.toString());
                v.Normalize();
                w.Normalize();
                UnityEngine.Debug.Log$$Object("normalized v = " + v.toString());
                UnityEngine.Debug.Log$$Object("normalized w = " + w.toString());
            }
        }
    }
};
JsTypes.push(V3Test);

