if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var XmlTest = {
    fullname: "XmlTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj2010",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
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
            var daoJunodeList = pp.get_ChildNodes();
            UnityEngine.Debug.Log$$Object("daojulist Count = " + daoJunodeList.get_Count());
            var daoJumdata = Lavie.XmlUtils.ConvertType$1$$XmlNodeList$$String(ShopItemData.ctor, daoJunodeList, "SubType");
            UnityEngine.Debug.Log$$Object("daojudata Count = " + daoJumdata.size);
            var $it14 = daoJumdata.GetEnumerator();
            while ($it14.MoveNext()){
                var shopItemData = $it14.get_Current();
                UnityEngine.Debug.Log$$Object(shopItemData.ID);
            }
        }
    }
};
JsTypes.push(XmlTest);

