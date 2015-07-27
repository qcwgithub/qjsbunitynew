if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Lavie$XmlUtils = {
    fullname: "Lavie.XmlUtils",
    baseTypeName: "System.Object",
    staticDefinition: {
        AssignObjectValueFromXml: function (mNode, mData, mDataType){
            var $it7 = mNode.get_Attributes().GetEnumerator();
            while ($it7.MoveNext()){
                var xmlAttribute = $it7.get_Current();
                var fieldName = xmlAttribute.get_Name();
                var value = mNode.get_Attributes().GetNamedItem$$String(fieldName).get_Value().toString();
                var fieldValue = null;
                if (jsimp.Reflection.PropertyTypeIsIntArray(mDataType, fieldName)){
                    fieldValue = Lavie.XmlUtils.ConvertString2IntArray(value);
                }
                else {
                    var fieldType = jsimp.Reflection.GetPropertyType(mDataType, fieldName);
                    if (fieldType == null){
                        continue;
                    }
                    fieldValue = Lavie.XmlUtils.ConvertString2ActualType(fieldType, value);
                }
                jsimp.Reflection.SetPropertyValue(mData, fieldName, fieldValue);
            }
        },
        CreateObjectFromXml$$XmlNode$$Type: function (mNode, type){
            var mData = jsimp.Reflection.CreateInstance$$Type(type);
            Lavie.XmlUtils.AssignObjectValueFromXml(mNode, mData, type);
            return mData;
        },
        CreateObjectFromXml$1$$XmlNode: function (T, mNode){
            var mData = jsimp.Reflection.CreateInstance$1(T);
            Lavie.XmlUtils.AssignObjectValueFromXml(mNode, mData, Typeof(T));
            return mData;
        },
        CreateObjectFromXml$1$$XmlNodeList$$String: function (T, nodeList, subType){
            var list = new System.Collections.Generic.List$1.ctor(T);
            var $it8 = nodeList.GetEnumerator();
            while ($it8.MoveNext()){
                var mNode = $it8.get_Current();
                var mData = jsimp.Reflection.CreateInstance$1(T);
                Lavie.XmlUtils.AssignObjectValueFromXml(mNode, mData, Typeof(T));
                if (subType.length > 0 && mNode.get_HasChildNodes()){
                    var $it9 = mNode.get_ChildNodes().GetEnumerator();
                    while ($it9.MoveNext()){
                        var childNode = $it9.get_Current();
                        var fieldName = Lavie.XmlUtils.NodeValue$1(System.String.ctor, childNode, subType);
                        var fieldType = jsimp.Reflection.GetPropertyType(Typeof(T), fieldName);
                        if (fieldType != null){
                            var fieldValue = Lavie.XmlUtils.CreateObjectFromXml$$XmlNode$$Type(childNode, fieldType);
                            jsimp.Reflection.SetPropertyValue(mData, fieldName, fieldValue);
                        }
                    }
                }
                list.Add(mData);
            }
            return list;
        },
        ConvertString2IntArray: function (value){
            var arr = (value.Split$$Char$Array(","));
            var ret = new Int32Array(arr.length);
            for (var i = 0; i < arr.length; i++){
                ret[i] = System.Int32.Parse$$String(arr[i]);
            }
            return ret;
        },
        ConvertString2ActualType: function (type, value){
            var ret = null;
            if (type == Typeof(System.Int32.ctor)){
                ret = System.Int32.Parse$$String(value);
            }
            else if (type == Typeof(System.Single.ctor)){
                ret = System.Single.Parse$$String(value);
            }
            else if (type == Typeof(System.Boolean.ctor)){
                ret = (value == "1");
            }
            else if (type.get_IsEnum()){
                ret = System.Int32.Parse$$String(value);
            }
            else if (type == Typeof(System.String.ctor)){
                ret = value;
            }
            else {
            }
            return ret;
        },
        NodeValue$1: function (T, node, nodeName){
            var collection = node.get_Attributes();
            if (collection.get_Count() == 0){
                return Default(T);
            }
            var namedItem = collection.GetNamedItem$$String(nodeName);
            if (namedItem == null){
                return Default(T);
            }
            var typeT = Typeof(T);
            var value = namedItem.get_Value();
            return Cast(Lavie.XmlUtils.ConvertString2ActualType(typeT, value), T);
        },
        Select$1$$XmlNodeList$$String$$String$$String: function (T, xmlNodeList, nodeName, value, attribute){
            var $it10 = xmlNodeList.GetEnumerator();
            while ($it10.MoveNext()){
                var node = $it10.get_Current();
                if (Lavie.XmlUtils.NodeValue$1(T, node, nodeName).toString() == value){
                    return Lavie.XmlUtils.NodeValue$1(T, node, attribute);
                }
            }
            return Default(T);
        },
        Select$1$$XmlNodeList$$String$$T: function (T, xmlNodeList, nodeName, value){
            var $it11 = xmlNodeList.GetEnumerator();
            while ($it11.MoveNext()){
                var node = $it11.get_Current();
                var nodeValue = Lavie.XmlUtils.NodeValue$1(T, node, nodeName);
                if (jsimp.Reflection.SimpleTEquals$1(T, nodeValue, value)){
                    return node;
                }
            }
            return null;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(Lavie$XmlUtils);

