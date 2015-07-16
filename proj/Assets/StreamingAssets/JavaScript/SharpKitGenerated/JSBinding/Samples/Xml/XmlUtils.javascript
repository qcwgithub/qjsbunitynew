if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Lavie$XmlUtils = {
    fullname: "Lavie.XmlUtils",
    baseTypeName: "System.Object",
    staticDefinition: {
        ConvertType$$XmlNode$$Type: function (mNode, target){
            var mData = System.Activator.CreateInstance$$Type(target);
            var t = mData.GetType();
            var fields = t.GetFields();
            for (var $i15 = 0,$l15 = fields.length,fieldInfo = fields[$i15]; $i15 < $l15; $i15++, fieldInfo = fields[$i15]){
                var fieldName = fieldInfo.get_Name();
                var value = mNode.get_Attributes().GetNamedItem$$String(fieldName).get_Value().toString();
                var lastValue = value;
                var fType = t.GetField$$String(fieldName).get_FieldType();
                lastValue = Lavie.XmlUtils.Convert(fType, value, lastValue);
                t.GetField$$String(fieldName).SetValue$$Object$$Object(mData, lastValue);
            }
            return mData;
        },
        ConvertType$1$$XmlNode: function (T, mNode){
            var mData = System.Activator.CreateInstance$1(T);
            var t = mData.GetType();
            var $it15 = mNode.get_Attributes().GetEnumerator();
            while ($it15.MoveNext()){
                var xmlAttribute = $it15.get_Current();
                var fieldName = xmlAttribute.get_Name();
                var value = mNode.get_Attributes().GetNamedItem$$String(fieldName).get_Value().toString();
                var lastValue = value;
                if (System.Reflection.FieldInfo.op_Equality$$FieldInfo$$FieldInfo(t.GetField$$String(fieldName), null)){
                    continue;
                }
                var fType = t.GetField$$String(fieldName).get_FieldType();
                lastValue = Lavie.XmlUtils.Convert(fType, value, lastValue);
                t.GetField$$String(fieldName).SetValue$$Object$$Object(mData, lastValue);
            }
            return mData;
        },
        ConvertType$1$$XmlNodeList: function (T, nodeList){
            var list = new BetterList$1.ctor(T);
            var t = Typeof(T);
            var $it16 = nodeList.GetEnumerator();
            while ($it16.MoveNext()){
                var mNode = $it16.get_Current();
                var mData = System.Activator.CreateInstance$1(T);
                var $it17 = mNode.get_Attributes().GetEnumerator();
                while ($it17.MoveNext()){
                    var xmlAttribute = $it17.get_Current();
                    var fieldName = xmlAttribute.get_Name();
                    var value = mNode.get_Attributes().GetNamedItem$$String(fieldName).get_Value().toString();
                    var lastValue = value;
                    if (System.Reflection.FieldInfo.op_Equality$$FieldInfo$$FieldInfo(t.GetField$$String(fieldName), null)){
                        throw $CreateException(new System.Exception.ctor$$String(System.String.Format$$String$$Object$$Object("{0} have no filed {1}", t.get_Name(), fieldName)), new Error());
                    }
                    var fType = t.GetField$$String(fieldName).get_FieldType();
                    lastValue = Lavie.XmlUtils.Convert(fType, value, lastValue);
                    t.GetField$$String(fieldName).SetValue$$Object$$Object(mData, lastValue);
                }
                list.Add(mData);
            }
            return list;
        },
        ConvertType$1$$XmlNodeList$$String: function (T, nodeList, subType){
            var list = new BetterList$1.ctor(T);
            var t = Typeof(T);
            var $it18 = nodeList.GetEnumerator();
            while ($it18.MoveNext()){
                var mNode = $it18.get_Current();
                var mData = System.Activator.CreateInstance$1(T);
                var $it19 = mNode.get_Attributes().GetEnumerator();
                while ($it19.MoveNext()){
                    var xmlAttribute = $it19.get_Current();
                    var fieldName = xmlAttribute.get_Name();
                    var value = mNode.get_Attributes().GetNamedItem$$String(fieldName).get_Value().toString();
                    var lastValue = value;
                    if (System.Reflection.FieldInfo.op_Equality$$FieldInfo$$FieldInfo(t.GetField$$String(fieldName), null)){
                        throw $CreateException(new System.Exception.ctor$$String(System.String.Format$$String$$Object$$Object("{0} have no filed {1}", t.get_Name(), fieldName)), new Error());
                    }
                    var fType = t.GetField$$String(fieldName).get_FieldType();
                    lastValue = Lavie.XmlUtils.Convert(fType, value, lastValue);
                    t.GetField$$String(fieldName).SetValue$$Object$$Object(mData, lastValue);
                }
                if (mNode.get_HasChildNodes()){
                    var $it20 = mNode.get_ChildNodes().GetEnumerator();
                    while ($it20.MoveNext()){
                        var childNode = $it20.get_Current();
                        var tSubType = Lavie.XmlUtils.NodeValue$1(System.String.ctor, childNode, subType);
                        if (tSubType == null || System.Reflection.FieldInfo.op_Equality$$FieldInfo$$FieldInfo(t.GetField$$String(tSubType), null)){
                        }
                        else {
                            var fType = t.GetField$$String(tSubType).get_FieldType();
                            var fieldValue = Lavie.XmlUtils.ConvertType$$XmlNode$$Type(childNode, fType);
                            t.GetField$$String(tSubType).SetValue$$Object$$Object(mData, fieldValue);
                        }
                    }
                }
                list.Add(mData);
            }
            return list;
        },
        Convert: function (fType, value, lastValue){
            if (fType == Typeof(System.Int32.ctor)){
                var n = System.Int32.Parse$$String(value.toString());
                lastValue = n;
            }
            else if (fType == Typeof(System.Single.ctor)){
                var m = System.Single.Parse$$String(value.toString());
                lastValue = m;
            }
            else if (fType == Typeof(System.Boolean.ctor)){
                lastValue = (value == "1");
            }
            else if (fType.get_IsEnum()){
                var mInt;
                if ((function (){
                    var $1 = {
                        Value: mInt
                    };
                    var $res = System.Int32.TryParse$$String$$Int32(value.toString(), $1);
                    mInt = $1.Value;
                    return $res;
                })()){
                    lastValue = (System.Int32.Parse$$String(value.toString()));
                }
                else {
                    lastValue = (System.Enum.Parse$$Type$$String(fType, value.toString()));
                }
            }
            else if (fType == Typeof(System.String.ctor)){
                lastValue = lastValue.toString();
            }
            else if (fType == Typeof(Int32Array)){
                var value2 = (lastValue.toString().Split$$Char$Array(","));
                var value1 = new Int32Array(value2.length);
                for (var i = 0; i < value2.length; i++){
                    value1[i] = System.Int32.Parse$$String(value2[i]);
                }
                lastValue = value1;
            }
            else {
                throw $CreateException(new System.Exception.ctor$$String(System.String.Format$$String$$Object("value{0} is not defined!", lastValue)), new Error());
            }
            return lastValue;
        },
        NodeValue$1: function (T, node, nodeName){
            if (node.get_Attributes().get_Count() == 0){
                return Default(T);
            }
            if (node.get_Attributes().GetNamedItem$$String(nodeName) == null){
                return Default(T);
            }
            var value = node.get_Attributes().GetNamedItem$$String(nodeName).get_Value();
            if (Typeof(T) == Typeof(System.Int32.ctor)){
                var n = System.Int32.Parse$$String(value.toString());
                return Cast((n), T);
            }
            if (Typeof(T) == Typeof(System.Single.ctor)){
                var m = System.Single.Parse$$String(value.toString());
                return Cast((m), T);
            }
            if (Typeof(T) == Typeof(System.Boolean.ctor)){
                return Cast((value == "1"), T);
            }
            if (Typeof(T) == Typeof(System.Enum.ctor)){
                var mInt;
                if ((function (){
                    var $1 = {
                        Value: mInt
                    };
                    var $res = System.Int32.TryParse$$String$$Int32(value.toString(), $1);
                    mInt = $1.Value;
                    return $res;
                })()){
                    return Cast((System.Int32.Parse$$String(value.toString())), T);
                }
                else {
                    var tt = Typeof(T);
                    return Cast((System.Enum.Parse$$Type$$String(tt, value.toString())), T);
                }
            }
            return Cast(value, T);
        },
        Select$1$$XmlNodeList$$String$$String$$String: function (T, xmlNodeList, nodeName, value, attribute){
            var $it21 = xmlNodeList.GetEnumerator();
            while ($it21.MoveNext()){
                var node = $it21.get_Current();
                if (Lavie.XmlUtils.NodeValue$1(T, node, nodeName).toString() == value){
                    return Lavie.XmlUtils.NodeValue$1(T, node, attribute);
                }
            }
            return Default(T);
        },
        Select$1$$XmlNodeList$$String$$T: function (T, xmlNodeList, nodeName, value){
            var $it22 = xmlNodeList.GetEnumerator();
            while ($it22.MoveNext()){
                var node = $it22.get_Current();
                if (Lavie.XmlUtils.NodeValue$1(T, node, nodeName).Equals$$Object(value)){
                    return node;
                }
            }
            return null;
        }
    },
    assemblyName: "SharpKitProj2010",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(Lavie$XmlUtils);

