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

	public abstract class InspectorBase : Editor
	{

		public override void OnInspectorGUI()
		{
			editTaggedFields();
			OnCustomInspectorGUI();
		}

		#region Overridable methods provided for descendant classes

		protected virtual void OnCustomInspectorGUI()
		{
			// Stub, intended to be overridden by subclasses
		}

		#endregion

		#region Private utility methods

		protected void editTaggedFields()
		{

			var fields = target
				.GetType()
				.GetMembers( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
				.Where( x => x.IsDefined( typeof( InspectorAttribute ), true ) )
				.Select( x => new InspectorField()
				{
					EditorTarget = serializedObject,
					Member = x,
					Attribute = (InspectorAttribute)x.GetCustomAttributes( typeof( InspectorAttribute ), true ).FirstOrDefault()
				} )
				.ToList();

			var groups = InspectorGroup.GroupFields( fields );
			orderGroups( groups );

			for( int i = 0; i < groups.Count; i++ )
			{

				var group = groups[ i ];
				if( CanShowGroup( group ) )
				{

					using( BeginGroup( group.Name ) )
					{

						OnBeforeGroupInspectorGUI();

						for( int f = 0; f < group.Fields.Count; f++ )
						{
							var field = group.Fields[ f ];
							if( CanShowField( field ) )
							{
								field.OnInspectorGUI( serializedObject );
							}
						}

						OnAfterGroupInspectorGUI();

					}

				}

			}

			serializedObject.ApplyModifiedProperties();

		}

		protected virtual void OnAfterGroupInspectorGUI()
		{
			// Stub, intended to be overridden
		}

		protected virtual void OnBeforeGroupInspectorGUI()
		{
			// Stub, intended to be overridden
		}

		protected virtual bool CanShowGroup( InspectorGroup group )
		{
			return true;
		}

		protected virtual bool CanShowField( InspectorField field )
		{
			return true;
		}

		protected void orderGroups( List<InspectorGroup> groups )
		{

			var type = target.GetType();
			var attribute = (InspectorGroupOrderAttribute)type.GetCustomAttributes( typeof( InspectorGroupOrderAttribute ), true ).FirstOrDefault();

			if( attribute == null )
				return;

			var order = attribute.Groups;

			groups.Sort( ( lhs, rhs ) =>
			{

				var lhsIndex = order.IndexOf( lhs.Name );
				if( lhsIndex == -1 )
					lhsIndex = int.MaxValue;

				var rhsIndex = order.IndexOf( rhs.Name );
				if( rhsIndex == -1 )
					rhsIndex = int.MaxValue;

				if( lhsIndex == rhsIndex )
					return lhs.Name.CompareTo( rhs.Name );

				return lhsIndex.CompareTo( rhsIndex );


			} );

		}

		protected static void drawSeparator()
		{

			GUILayout.Space( 12f );

			if( Event.current.type == EventType.Repaint )
			{

				Texture2D tex = EditorGUIUtility.whiteTexture;

				Rect rect = GUILayoutUtility.GetLastRect();

				var savedColor = GUI.color;
				GUI.color = new Color( 0f, 0f, 0f, 0.25f );

				GUI.DrawTexture( new Rect( 0f, rect.yMin + 6f, Screen.width, 4f ), tex );
				GUI.DrawTexture( new Rect( 0f, rect.yMin + 6f, Screen.width, 1f ), tex );
				GUI.DrawTexture( new Rect( 0f, rect.yMin + 9f, Screen.width, 1f ), tex );

				GUI.color = savedColor;

			}

		}

		protected static GroupHeader BeginGroup( string label )
		{
			return BeginGroup( label, EditorGUIUtility.labelWidth );
		}

		protected static GroupHeader BeginGroup( string label, float labelWidth )
		{
			return new GroupHeader( label, labelWidth );
		}

		#endregion 

		#region Nested classes

		protected class InspectorGroup
		{

			public List<InspectorField> Fields = new List<InspectorField>();

			public string Name;

			public static List<InspectorGroup> GroupFields( List<InspectorField> fields )
			{

				var groups = new Dictionary<string, InspectorGroup>();

				for( int i = 0; i < fields.Count; i++ )
				{

					var field = fields[ i ];

					InspectorGroup group = null;
					if( !groups.TryGetValue( field.Group, out group ) )
					{
						group = groups[ field.Group ] = new InspectorGroup() { Name = field.Group };
					}

					group.Fields.Add( field );

				}

				var list = groups.Values.ToList();

				for( int i = 0; i < list.Count; i++ )
				{
					list[ i ].SortFields();
				}

				return list;

			}

			public void SortFields()
			{
				Fields.Sort( ( lhs, rhs ) =>
				{
					if( lhs.Order == rhs.Order )
						return lhs.Label.CompareTo( rhs.Label );
					return lhs.Order.CompareTo( rhs.Order );
				} );
			}

		}

		protected class InspectorField
		{

			public MemberInfo Member;

			public InspectorAttribute Attribute;

			public SerializedObject EditorTarget;

			public string Group { get { return Attribute.Group; } }
			public int Order { get { return Attribute.Order; } }
			public string Tooltip { get { return Attribute.Tooltip; } }

			public string Label
			{
				get
				{
					var label = Attribute.Label ?? Attribute.BackingField ?? Member.Name;
					if( !label.Contains( " " ) )
						return ObjectNames.NicifyVariableName( label );
					else
						return label;
				}
			}

			public void OnInspectorGUI( SerializedObject target )
			{

				var property = GetSerializedProperty( target );
				if( property == null )
				{
					EditorGUILayout.HelpBox( "Cannot inspect field: " + Member.Name, MessageType.Warning );
					return;
				}

				var content = new GUIContent( Label );
				if( !string.IsNullOrEmpty( this.Tooltip ) )
				{
					content.tooltip = this.Tooltip;
				}

				EditorGUILayout.PropertyField( property, content, true );

			}

			public SerializedProperty GetSerializedProperty( SerializedObject target )
			{
				return target.FindProperty( Attribute.BackingField ?? Member.Name );
			}

		}

		protected class GroupHeader : IDisposable
		{

			private float savedLabelWidth = 0;

			public GroupHeader( string label )
				: this( label, 100 )
			{
			}

			public GroupHeader( string label, float labelWidth )
			{

				savedLabelWidth = EditorGUIUtility.labelWidth;

				GUILayout.Label( label, "HeaderLabel" );

				EditorGUIUtility.labelWidth = labelWidth;

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space( 10 );
				EditorGUILayout.BeginVertical();

			}

			#region IDisposable Members

			public void Dispose()
			{
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = savedLabelWidth;
			}

			#endregion

		}

		#endregion

	}

}
