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

namespace DaikonForge.Editor
{

	using Object = UnityEngine.Object;
	using UnityEngine;
	using UnityEditor;

	[CanEditMultipleObjects()]
	[CustomEditor( typeof( SplineNode ), true )]
	public class SplineNodeInspector : InspectorBase
	{

		private SplineNode node;
		private SplineObject spline;

		public override void OnInspectorGUI()
		{

			if( Application.isPlaying )
				return;

			if( serializedObject.isEditingMultipleObjects )
				return;

			this.node = target as SplineNode;
			if( node.transform.parent == null )
				return;

			this.spline = node.transform.parent.GetComponent<SplineObject>();
			if( spline == null )
				return;

			using( BeginGroup( "Selection" ) )
			{

				EditorGUILayout.BeginHorizontal();
				{

					if( GUILayout.Button( "Spline" ) )
					{
						Selection.activeGameObject = spline.gameObject;
					}

					if( GUILayout.Button( "Prev" ) )
					{
						var index = spline.ControlPoints.IndexOf( node.transform ) - 1;
						if( index < 0 )
							index = spline.ControlPoints.Count - 1;
						Selection.activeGameObject = spline.ControlPoints[ index ].gameObject;
					}

					if( GUILayout.Button( "Next" ) )
					{
						var index = spline.ControlPoints.IndexOf( node.transform ) + 1;
						if( index > spline.ControlPoints.Count - 1 )
							index = 0;
						Selection.activeGameObject = spline.ControlPoints[ index ].gameObject;
					}

				}
				EditorGUILayout.EndHorizontal();

			}

			using( BeginGroup( "Insert New Node" ) )
			{

				EditorGUILayout.BeginHorizontal();
				{

					if( GUILayout.Button( "Before" ) )
					{
						insertNodeBefore();
					}

					if( GUILayout.Button( "After" ) )
					{
						insertNodeAfter();
					}

				}
				EditorGUILayout.EndHorizontal();

			}

			using( BeginGroup( "Other Actions" ) )
			{

				if( GUILayout.Button( "Split Spline" ) )
				{
				}

			}

		}

		private void insertNodeBefore()
		{

			spline.CalculateSpline();

			var index = spline.ControlPoints.IndexOf( node.transform );
			if( index > 0 )
			{

				var myPos = node.transform.position;
				var prevPos = spline.ControlPoints[ index - 1 ].transform.position;
				var newPos = prevPos + ( myPos - prevPos ) * 0.5f;

				var go = addNewNode( newPos );
				spline.ControlPoints.Insert( index, go.transform );

			}
			else
			{

				var direction = -spline.Spline.GetTangent( 0f );
				var go = addNewNode( node.transform.position + direction );
			
				spline.ControlPoints.Insert( 0, go.transform );

			}

			spline.CalculateSpline();

		}


		private void insertNodeAfter()
		{

			spline.CalculateSpline();

			var index = spline.ControlPoints.IndexOf( node.transform );
			if( index < spline.ControlPoints.Count - 1 )
			{

				var myPos = node.transform.position;
				var nextPos = spline.ControlPoints[ index + 1 ].transform.position;
				var newPos = myPos + ( nextPos - myPos ) * 0.5f;

				var go = addNewNode( newPos );
				spline.ControlPoints.Insert( index + 1, go.transform );

			}
			else
			{

				var time = spline.GetTimeAtNode( index );
				var direction = spline.Spline.GetTangent( time );
				var go = addNewNode( node.transform.position + direction );

				spline.ControlPoints.Add( go.transform );

			}

			spline.CalculateSpline();

		}

		private GameObject addNewNode( Vector3 newPos )
		{

			Undo.RegisterCompleteObjectUndo( spline, "Add Node" );
			EditorUtility.SetDirty( spline );

			var go = new GameObject() { name = "SplineNode" + spline.ControlPoints.Count };
			Undo.RegisterCreatedObjectUndo( go, "Insert Spline Node" );

			go.AddComponent<SplineNode>();
			go.transform.position = newPos;
			go.transform.parent = spline.transform;

			Selection.activeGameObject = go;

			return go;

		}

	}

}
