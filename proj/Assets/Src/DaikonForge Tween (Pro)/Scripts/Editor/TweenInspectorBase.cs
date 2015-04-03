/* Copyright 2013-2014 Daikon Forge */
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using DaikonForge.Tween;
using DaikonForge.Tween.Components;

namespace DaikonForge.Editor
{

	using Object = UnityEngine.Object;
	using UnityEngine;
	using UnityEditor;

	[CanEditMultipleObjects()]
	[CustomEditor( typeof( TweenComponentBase ), true )]
	public class TweenInspectorBase : InspectorBase
	{

		protected bool isEditingMultipleObjects;
		protected TweenComponentBase Target;

		public override void OnInspectorGUI()
		{

			this.Target = target as TweenComponentBase;
			this.isEditingMultipleObjects = serializedObject.isEditingMultipleObjects;

			base.OnInspectorGUI();

			if( !isEditingMultipleObjects && Application.isPlaying )
			{
				showActions();
			}

		}

		protected override bool CanShowField( InspectorField field )
		{

			if( !isEditingMultipleObjects )
			{

				var tween = target as TweenComponentBase;
				var fieldName = field.Member.Name;

				if( fieldName == "loopCount" && tween.LoopType == TweenLoopType.None )
				{
					return false;
				}

				if( fieldName == "assignStartValueBeforeDelay" )
				{
					if( tween.StartDelay <= 0 )
						return false;
				}

				if( fieldName == "startValue" )
				{

					var startTypeProperty = serializedObject.FindProperty( "startValueType" );
					if( startTypeProperty != null )
					{
						if( startTypeProperty.intValue == (int)TweenStartValueType.SyncOnRun )
						{
							return false;
						}
					}

				}

				if( fieldName == "endValue" )
				{

					var endTypeProperty = serializedObject.FindProperty( "endValueType" );
					if( endTypeProperty != null )
					{
						if( endTypeProperty.intValue == (int)TweenEndValueType.SyncOnRun )
						{
							return false;
						}
					}

				}

			}

			return base.CanShowField( field );

		}

		protected void showActions()
		{

			drawSeparator();

			EditorGUILayout.BeginHorizontal();
			{

				GUI.enabled = Target.State == TweenState.Stopped;
				if( GUILayout.Button( "Play" ) )
				{
					Target.Play();
				}

				GUI.enabled = Target.State != TweenState.Stopped;
				if( GUILayout.Button( "Stop" ) )
				{
					Target.Stop();
				}

				GUI.enabled = Target.State == TweenState.Started || Target.State == TweenState.Playing;
				if( GUILayout.Button( "Pause" ) )
				{
					Target.Pause();
				}

				GUI.enabled = Target.State == TweenState.Paused;
				if( GUILayout.Button( "Resume" ) )
				{
					Target.Resume();
				}

			}
			EditorGUILayout.EndHorizontal();

			GUI.enabled = true;

		}

	}

}
