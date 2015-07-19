if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var XmlTest = {
    fullname: "XmlTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var ABC = enumMoneyType.Gold;
            UnityEngine.Debug.Log$$Object(ABC.toString());
            var textAssets = Cast(UnityEngine.Resources.Load$$String("ShopConfig"), UnityEngine.TextAsset.ctor);
            if (UnityEngine.Object.op_Inequality(textAssets, null)){
                UnityEngine.Debug.Log$$Object(textAssets.get_text());
            }
            else {
                UnityEngine.Debug.Log$$Object("unkonw error!");
            }
            var xml = new System.Xml.XmlDocument.ctor();
            xml.LoadXml(textAssets.get_text());
            UnityEngine.Debug.Log$$Object("xmlload");
            var xmlPackets = xml.SelectNodes$$String("root/Packets/Packet");
            var pp = Lavie.XmlUtils.Select$1$$XmlNodeList$$String$$T(System.String.ctor, xmlPackets, "ID", "1");
            UnityEngine.Debug.Log$$Object("pp=" + pp.toString());
            var v1 = Lavie.XmlUtils.NodeValue$1(ItemType.ctor, pp, "ID");
            UnityEngine.Debug.Log$$Object("ID == " + v1.toString());
        }
    }
};
JsTypes.push(XmlTest);

