if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var SetParticleSortingLayer = {
    fullname: "SetParticleSortingLayer",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.sortingLayerName = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            this.get_particleSystem().get_renderer().set_sortingLayerName(this.sortingLayerName);
        }
    }
};
JsTypes.push(SetParticleSortingLayer);

