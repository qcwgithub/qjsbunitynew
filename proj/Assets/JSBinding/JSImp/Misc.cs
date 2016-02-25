using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharpKit.JavaScript;
using System;

namespace jsimp
{
[JsType(JsMode.Clr, "~/Assets/StreamingAssets/JavaScript/SharpKitGeneratedFiles.javascript")]
    public static class Misc
    {
        [JsMethod(Code = @"return str.replace(str1, str2);")]
        public static string string_Replace(string str, string str1, string str2)
        {
            return str.Replace(str1, str2);
        }
        [JsMethod(Code = @"return str.split(sep);")]
        public static string[] string_Split(string str, string sep)
        {
            return str.Split(sep.ToCharArray());
        }
        [JsMethod(Code = @"var A = str.split(sep);
var j = 0, B = [];
for (var i = 0; i < A.length; i++)
{
    if (A[i].length != 0)
        B[j++] = A[i];
}
return B;")]
        public static string[] string_Split_RemoveEmptyEntries(string str, string sep)
        {
            return str.Split(sep.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
        }
        
        [JsMethod(Code = @"var L = collection.length;
for (var i = 0; i < L; i++)
{
    lst.Add(collection[i]);
}")]
        public static void List_AddRange<T>(List<T> lst, IEnumerable<T> collection)
        {
            lst.AddRange (collection);
        }
        [JsMethod(Code = @"return Math.abs(v);")]
        public static int Abs(this int v)
        {
            return Math.Abs(v);
        }
        [JsMethod(Code = @"return Math.abs(v);")]
        public static float Abs(this float v)
        {
            return Math.Abs(v);
        }
    }
    
}