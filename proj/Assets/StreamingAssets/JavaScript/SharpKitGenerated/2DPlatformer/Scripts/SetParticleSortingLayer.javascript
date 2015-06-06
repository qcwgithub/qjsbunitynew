if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var SetParticleSortingLayer = {
    fullname: "SetParticleSortingLayer",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.sortingLayerName = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            this.GetComponent$1(UnityEngine.ParticleSystem.ctor).GetComponent$1(UnityEngine.Renderer.ctor).set_sortingLayerName(this.sortingLayerName);
        }
    }
};
JsTypes.push(SetParticleSortingLayer);

