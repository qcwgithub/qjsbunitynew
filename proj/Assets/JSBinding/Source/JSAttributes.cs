using UnityEngine;
using System.Collections;
using System;

/*
 * 用于指定脚本要使用的 JSComponent 名
 * 
 * 例如
 * JSComponent(Name = "JSComponent_EventTrigger")
 */
[AttributeUsage(AttributeTargets.Class)]
public class JSComponentAttribute : Attribute
{
    public string Name { get; set; }
}
