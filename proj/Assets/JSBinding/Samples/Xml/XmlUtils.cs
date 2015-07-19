using SharpKit.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Lavie;

namespace Lavie
{
    ///


    /// xml tool
    /// 全自动分析xml.序列化成类。
    /// 
    /// need ngui betterlist
    /// @author layola
    /// 
    /// 
    ///

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/XmlUtils.javascript")]
    public static class XmlUtils
    {
        ///
        /// 翻译一个类 赋值为对应的一个xml的节点属性的值
        /// 
        public static object ConvertType(this XmlNode mNode, Type target)
        {
            var mData = Activator.CreateInstance(target);
            var t = mData.GetType();
            var fields = t.GetFields();

            foreach (FieldInfo fieldInfo in fields)
            {
                var fieldName = fieldInfo.Name; //xmlAttribute.Name;

                string value = mNode.Attributes.GetNamedItem(fieldName).Value.ToString();

                object lastValue = value;

                Type fType = t.GetField(fieldName).FieldType;


                lastValue = Convert(fType, value, lastValue);


                t.GetField(fieldName).SetValue(mData, lastValue);
            }


            return mData;
        }

        /// <summary>
        /// 翻译一个xmlnode 为对应的一个实体类T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mNode"></param>
        /// <returns></returns>
        public static T ConvertType<T>(this XmlNode mNode)
        {
            var mData = Activator.CreateInstance<T>();
            var t = mData.GetType();
            foreach (XmlAttribute xmlAttribute in mNode.Attributes)
            {
                var fieldName = xmlAttribute.Name;


                string value = mNode.Attributes.GetNamedItem(fieldName).Value.ToString();

                object lastValue = value;

                if (t.GetField(fieldName) == null)
                {
                    //throw new Exception(String.Format("{0} {1} is not defined", t.ToString(), fieldName));
                    //  return default(T);

                    continue;
                }
                Type fType = t.GetField(fieldName).FieldType;


                lastValue = Convert(fType, value, lastValue);


                t.GetField(fieldName).SetValue(mData, lastValue);
            }


            return (T)mData;
        }

        /// <summary>
        /// 自动转换xml到实体类；列表。
        /// </summary>
        /// <param name="t"></param>
        /// <param name="nodeList"></param>
        /// <returns></returns>
//         public static BetterList<T> ConvertType<T>(this XmlNodeList nodeList)
//         {
//             var list = new BetterList<T>();
// 
//             var t = typeof(T);
//             foreach (XmlNode mNode in nodeList)
//             {
//                 var mData = Activator.CreateInstance<T>();
// 
//                 #region 分析<item Id="3"..>
// 
//                 foreach (XmlAttribute xmlAttribute in mNode.Attributes)
//                 {
//                     var fieldName = xmlAttribute.Name;
// 
// 
//                     string value = mNode.Attributes.GetNamedItem(fieldName).Value.ToString();
// 
//                     object lastValue = value;
// 
//                     if (t.GetField(fieldName) == null)
//                     {
//                         throw new Exception(String.Format("{0} have no filed {1}", t.Name, fieldName));
//                     }
//                     Type fType = t.GetField(fieldName).FieldType;
// 
// 
//                     lastValue = Convert(fType, value, lastValue);
// 
// 
//                     t.GetField(fieldName).SetValue(mData, lastValue);
//                 }
// 
//                 #endregion
// 
//                 /*      if (mNode.HasChildNodes)
//             {
//                 foreach (XmlNode childNode in mNode.ChildNodes)
//                 {
// 
// 
//                 }
//             }*/
//                 list.Add(mData);
//             }
// 
// 
//             return list;
//         }


        /// <summary>
        /// 自动转换xml到实体类；列表。 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="subType"> 子节点下这里包括类</param>
        /// <returns></returns>
//         public static BetterList<T> ConvertType<T>(this XmlNodeList nodeList, string subType)
//         {
//             var list = new BetterList<T>();
// 
//             var t = typeof(T);
//             foreach (XmlNode mNode in nodeList)
//             {
//                 var mData = Activator.CreateInstance<T>();
// 
//                 #region 分析<item Id="3"..>
// 
//                 foreach (XmlAttribute xmlAttribute in mNode.Attributes)
//                 {
//                     var fieldName = xmlAttribute.Name;
//                     string value = xmlAttribute.Value.ToString();
// 
//                     object lastValue = value;
// 
//                     FieldInfo fieldInfo = t.GetField(fieldName);
//                     if (fieldInfo == null)
//                     {
//                         throw new Exception(String.Format("{0} have no field {1}", t.Name, fieldName));
//                     }
//                     Type fieldType = fieldInfo.FieldType;
// 
//                     lastValue = Convert(fieldType, value, lastValue);
//                     t.GetField(fieldName).SetValue(mData, lastValue);
//                 }
// 
//                 #endregion
// 
//                 #region <item Id="3"..> 分析这里面的内容 <limit> </limit>  </item>
// 
//                 if (mNode.HasChildNodes)
//                 {
//                     foreach (XmlNode childNode in mNode.ChildNodes)
//                     {
//                         string tSubType = childNode.NodeValue<string>(subType);
//                         if (tSubType == null || t.GetField(tSubType) == null)
//                         {
//                             //没有这个节点
//                         }
//                         else
//                         {
//                             //找出对应的属性类，转化赋值给这个属性，最后赋值给类
//                             Type fType = t.GetField(tSubType).FieldType;
//                             var fieldValue = childNode.ConvertType(fType);
// 
// 
//                             t.GetField(tSubType).SetValue(mData, fieldValue);
//                         }
//                     }
//                 }
// 
//                 #endregion
// 
//                 list.Add(mData);
//             }
// 
// 
//             return list;
//         }


        /// <summary>
        /// 转换模板。
        /// </summary>
        /// <param name="fType"></param>
        /// <param name="value"></param>
        /// <param name="lastValue"></param>
        /// <returns></returns>
        private static object Convert(Type fType, string value, object lastValue)
        {
            if (fType == typeof(int))
            {
                int n = int.Parse(value.ToString());
                lastValue = n;
            }
            else if (fType == typeof(float))
            {
                float m = float.Parse(value.ToString());
                lastValue = m;
            }
            else if (fType == typeof(bool))
            {
                lastValue = (value == "1");
            }
            else if (fType.IsEnum)
            {
                int mInt;
                if (int.TryParse(value.ToString(), out mInt))
                {
                    lastValue = (int.Parse(value.ToString()));
                }
                else
                {
                    lastValue = (Enum.Parse(fType, value.ToString()));
                }
            }
            else if (fType == typeof(string))
            {
                lastValue = lastValue.ToString();
            }
            else if (fType == typeof(int[]))
            {
                string[] value2 = (string[])(lastValue.ToString().Split(','));

                int[] value1 = new int[value2.Length];
                for (int i = 0; i < value2.Length; i++)
                {
                    value1[i] = int.Parse(value2[i]);
                }

                lastValue = value1;
            }
            else
            {
                throw new Exception(String.Format("value{0} is not defined!", lastValue));
            }
            return lastValue;
        }

        /// <summary>
        /// 查找xml 属性值，
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static T NodeValue<T>(this XmlNode node, string nodeName)
        {
            XmlAttributeCollection collection = node.Attributes;

            if (collection.Count == 0)
            {
                // throw  new Exception(String.Format("node {0} have no atrributes {1}",node.Name,nodeName));
                return default(T);
            }

            XmlNode namedItem = collection.GetNamedItem(nodeName);
            if (namedItem == null)
            {
                return default(T);
            }

            Type typeT = typeof(T);

            object value = namedItem.Value;
            if (typeT == typeof(int))
            {
                int n = int.Parse(value.ToString());
                return (T)((object)n);
            }
            else if (typeT == typeof(float))
            {
                float m = float.Parse(value.ToString());
                return (T)((object)m);
            }
            else if (typeT == typeof(bool))
            {
                return (T)((object)(value == "1"));
            }
            else if (jsimp.Reflection.TypeIsEnum(typeT))
            {
                int mInt;
                if (int.TryParse(value.ToString(), out mInt))
                {
                    return (T)((object)(int.Parse(value.ToString())));
                }
                else
                {
                    var tt = typeof(T);
                    return (T)((object)(Enum.Parse(tt, value.ToString())));
                }
            }
            return (T)value;
        }

        /// <summary>
        /// 通过属性的值来查找另外一个属性的值。
        /// </summary>
        /// <param name="xmlNodeList"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static T Select<T>(this XmlNodeList xmlNodeList, string nodeName, string value, string attribute)
        {
            foreach (XmlNode node in xmlNodeList)
            {
                if (node.NodeValue<T>(nodeName).ToString() == value)
                {
                    return node.NodeValue<T>(attribute);
                }
            }
            return default(T);
        }

        /// <summary>
        /// 通过属性的值来查找对应的xmlNode.
        /// </summary>
        /// <param name="xmlNodeList"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static XmlNode Select<T>(this XmlNodeList xmlNodeList, string nodeName, T value)
        {
            foreach (XmlNode node in xmlNodeList)
            {
                T nodeValue = node.NodeValue<T>(nodeName);
                if (jsimp.Reflection.SimpleTEquals(nodeValue, value))
                {
                    return node;
                }
            }
            return null;
        }
    }
}