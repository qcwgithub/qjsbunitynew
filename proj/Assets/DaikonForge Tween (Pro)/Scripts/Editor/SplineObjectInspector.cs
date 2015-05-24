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
	[CustomEditor( typeof( SplineObject ), true )]
	public class SplineObjectInspector : InspectorBase
	{

		public override void OnInspectorGUI()
		{

			var component = (SplineObject)target;
			ensureNodeComponents( component );

			component.CalculateSpline();

			editTaggedFields();

			using( BeginGroup( "Actions" ) )
			{

				if( GUILayout.Button( "Add Node" ) )
				{

					Undo.RegisterCompleteObjectUndo( component.gameObject, "Add Node" );
					EditorUtility.SetDirty( component );

					var newNode = component.AddNode().gameObject;
					Undo.RegisterCreatedObjectUndo( newNode, "Add Node" );

					Selection.activeGameObject = newNode;

				}

			}

			var spline = component.Spline;
			var controlPoints = spline.ControlPoints;

			using( BeginGroup( "Information" ) )
			{
			
				var message = string.Format( "Length: {0}", spline.Length );
				
				for( int i = 0; i < controlPoints.Count; i++ )
				{
					message += string.Format( "\n\tNode {0} - Time: {1:F2}, Length: {2:F2}", i + 1, controlPoints[ i ].Time, controlPoints[ i ].Length );
				}
				
				EditorGUILayout.HelpBox( message, MessageType.Info );

			}

		}

		private void ensureNodeComponents( SplineObject component )
		{

			var count = component.transform.childCount;
			for( int i = 0; i < count; i++ )
			{

				var point = component.transform.GetChild( i ).gameObject;
				if( point.GetComponent<SplineNode>() == null )
				{
					point.AddComponent<SplineNode>();
				}

			}

		}

	}

}