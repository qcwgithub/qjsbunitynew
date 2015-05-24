/* Copyright 2013-2014 Daikon Forge */
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace DaikonForge.Editor
{

	using UnityEngine;
	using UnityEditor;

	using DaikonForge.Tween;
	using DaikonForge.Tween.Components;

	[CanEditMultipleObjects()]
	[CustomEditor( typeof( TweenVector2Property ), true )]
	public class TweenVector2PropertyInspector : TweenPropertyBaseInspector
	{
		protected override System.Type getPropertyType()
		{
			return typeof( Vector2 );
		}
	}

}
