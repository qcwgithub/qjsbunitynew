/* Copyright 2013-2014 Daikon Forge */
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace DaikonForge.Editor
{

	using Object = UnityEngine.Object;
	using UnityEngine;
	using UnityEditor;

	using DaikonForge.Tween;
	using DaikonForge.Tween.Components;

	[CanEditMultipleObjects()]
	public abstract class TweenPropertyBaseInspector : TweenInspectorBase
	{

		protected override bool CanShowField( InspectorField field )
		{

			if( field.Member.Name != "memberName" )
				return true;

			editMemberName( field );

			return false;

		}

		protected abstract System.Type getPropertyType();

		private void editMemberName( InspectorField field )
		{

			var tween = (ITweenPropertyBase)target;

			var options = getMemberList();
			var currentValue = string.Format( "{0}.{1}", tween.ComponentType, tween.MemberName );
			var currentIndex = options.IndexOf( currentValue );

			var selectedIndex = EditorGUILayout.Popup( "Property", currentIndex, options.ToArray() );
			if( selectedIndex != currentIndex )
				EditorUtility.SetDirty( target );

			if( selectedIndex >= 0 )
			{
				
				var selectedValue = options[ selectedIndex ].Split( '.' );
				tween.ComponentType = selectedValue[ 0 ];
				tween.MemberName = selectedValue[ 1 ];

				field.EditorTarget.FindProperty( "componentType" ).stringValue = tween.ComponentType;
				field.EditorTarget.FindProperty( "memberName" ).stringValue = tween.MemberName;

			}
			else
			{

				tween.ComponentType = "";
				tween.MemberName = "";

				field.EditorTarget.FindProperty( "componentType" ).stringValue = "";
				field.EditorTarget.FindProperty( "memberName" ).stringValue = "";

			}

		}

		private List<string> getMemberList()
		{

			var tween = (ITweenPropertyBase)target;
			var members = new List<string>();

			if( tween.Target == null )
			{
				return members;
			}


			var components = tween.Target.GetComponents<Component>();
			foreach( var component in components )
			{

				if( component is TweenComponentBase )
					continue;

				members.AddRange( getComponentMembers( component.GetType() ) );

			}

			members.Sort();

			return members;

		}

		private List<string> getComponentMembers( System.Type type )
		{

			var propertyType = this.getPropertyType();

			var members = type
				.GetMembers( BindingFlags.Instance | BindingFlags.Public )
				.Where( x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property );

			var list = new List<string>();

			foreach( var member in members )
			{

				System.Type memberType = null;
				if( member is FieldInfo )
				{
					memberType = ( (FieldInfo)member ).FieldType;
				}
				else
				{
					
					var property = (PropertyInfo)member;
					if( !property.CanWrite || property.GetSetMethod() == null )
						continue;

					memberType = property.PropertyType;

				}

				if( !propertyType.IsAssignableFrom( memberType ) )
					continue;

				list.Add( string.Format( "{0}.{1}", type.Name, member.Name ) );

			}

			return list;

		}

	}

}
