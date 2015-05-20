if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Destroyer = {
    fullname: "Destroyer",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.destroyOnAwake = false;
            this.awakeDestroyDelay = 0;
            this.findChild = false;
            this.namedChild = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
			//UnityEngine.Debug.Log$$Object(this.get_name());
            if (this.destroyOnAwake){
                if (this.findChild){
                    UnityEngine.Object.Destroy$$Object(this.get_transform().Find(this.namedChild).get_gameObject());
                }
                else {
                    UnityEngine.Object.Destroy$$Object$$Single(this.get_gameObject(), this.awakeDestroyDelay);
                }
            }
        },
        DestroyChildGameObject: function (){
            if (UnityEngine.Object.op_Inequality(this.get_transform().Find(this.namedChild).get_gameObject(), null))
                UnityEngine.Object.Destroy$$Object(this.get_transform().Find(this.namedChild).get_gameObject());
        },
        DisableChildGameObject: function (){
            if (this.get_transform().Find(this.namedChild).get_gameObject().get_activeSelf() == true)
                this.get_transform().Find(this.namedChild).get_gameObject().SetActive(false);
        },
        DestroyGameObject: function (){
            UnityEngine.Object.Destroy$$Object(this.get_gameObject());
        }
    }
};
JsTypes.push(Destroyer);

