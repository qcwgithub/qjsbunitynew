/* Copyright 2014 Daikon Forge */
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DaikonForge.Tween.Interpolation;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Helper class to configure tweens that can be used to animate any named property of a target object
	/// </summary>
	public class TweenNamedProperty<T>
	{

		#region Public methods 

		/// <summary>
		/// <para>Returns a Tween&lt;T&gt; object that is configured to animate the named property of the target object.</para>
		/// <b>For example, to tween the "timeScale" property of a custom class, you could use the following:</b><pre><code>
		/// var tween = TweenProperty&lt;float&gt;
		///		.Obtain( target, "timeScale" )
		///		.SetStartValue( 0f )
		///		.SetEndValue( 1f )
		///		.SetDuration( 0.5f );
		/// </code></pre>
		/// </summary>
		/// <param name="target">The object to be animated</param>
		/// <param name="propertyName">The name of the property to be animated</param>
		public static Tween<T> Obtain( object target, string propertyName )
		{
			return Obtain( target, propertyName, Interpolators.Get<T>() );
		}

		/// <summary>
		/// <para>Returns a Tween&lt;T&gt; object that is configured to animate the named property of the target object.</para>
		/// <b>For example, to tween the "timeScale" property of a custom class, you could use the following:</b><pre><code>
		/// var tween = TweenProperty&lt;float&gt;
		///		.Obtain( target, "timeScale", Interpolators.Get&lt;float&gt;() )
		///		.SetStartValue( 0f )
		///		.SetEndValue( 1f )
		///		.SetDuration( 0.5f );
		/// </code></pre>
		/// </summary>
		/// <param name="target">The object to be animated</param>
		/// <param name="propertyName">The name of the property to be animated</param>
		/// <param name="interpolator">The evaluator that will be used to interpolate values</param>
		public static Tween<T> Obtain( object target, string propertyName, Interpolator<T> interpolator )
		{

			if( target == null )
				throw new ArgumentException( "Target object cannot be NULL" );

			var type = target.GetType();
			var member = getMember( type, propertyName );

			if( member == null )
				throw new ArgumentException( string.Format( "Failed to find property {0}.{1}", type.Name, propertyName ) );

			#region Validate field/property type 

			var invalidType = false;
			if( member is FieldInfo )
			{
				if( ( (FieldInfo)member ).FieldType != typeof( T ) )
				{
					invalidType = true;
				}
			}
			else if( ((PropertyInfo)member).PropertyType != typeof( T ) ) 
			{
				invalidType = true;
			}

			if( invalidType )
				throw new InvalidCastException( string.Format( "{0}.{1} cannot be cast to type {2}", type.Name, member.Name, typeof( T ).Name ) );

			#endregion 

			var currentValue = get( target, type, member );

			var tween = Tween<T>.Obtain()
				.SetStartValue( currentValue )
				.SetEndValue( currentValue )
				.SetInterpolator( interpolator )
				.OnExecute( set( target, type, member ) );

			return tween;

		}

		public static T GetCurrentValue( object target, string propertyName )
		{

			var type = target.GetType();
			var member = getMember( type, propertyName );

			if( member == null )
				throw new ArgumentException( string.Format( "Failed to find property {0}.{1}", type.Name, propertyName ) );

			return get( target, type, member );

		}

		#endregion 

		#region Private utility methods 

		private static MethodInfo getGetMethod( PropertyInfo property )
		{
#if ( !UNITY_EDITOR && UNITY_METRO )
			return property.GetMethod;
#else
			return property.GetGetMethod();
#endif
		}

		private static MethodInfo getSetMethod( PropertyInfo property )
		{
#if ( !UNITY_EDITOR && UNITY_METRO )
			return property.SetMethod;
#else
			return property.GetSetMethod();
#endif
		}

		private static MemberInfo getMember( Type type, string propertyName )
		{
#if ( !UNITY_EDITOR && UNITY_METRO )
			var typeInfo = type.GetTypeInfo();
			return typeInfo.DeclaredMembers.FirstOrDefault( x => ( (x is FieldInfo) || (x is PropertyInfo) ) && x.Name == propertyName );
#else
			return type.GetMember( propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).FirstOrDefault();
#endif
		}

		private static T get( object target, System.Type type, MemberInfo member )
		{

			if( member is PropertyInfo )
			{
				
				var property = (PropertyInfo)member;

				var getter = getGetMethod( property );
				if( getter == null )
				{
					throw new ArgumentException( string.Format( "Property {0}.{1} cannot be read", type.Name, member.Name ) );
				}

				return (T)getter.Invoke( target, null );

			}

			if( member is FieldInfo )
			{
				var field = (FieldInfo)member;
				return (T)field.GetValue( target );
			}

			throw new ArgumentException( string.Format( "Failed to find property {0}.{1}", type.Name, member.Name ) );

		}

		private static TweenAssignmentCallback<T> set( object target, System.Type type, MemberInfo member )
		{

			if( member is PropertyInfo )
				return setProperty( target, type, (PropertyInfo)member );
			else if( member is FieldInfo )
				return setField( target, type, (FieldInfo)member );

			throw new ArgumentException( string.Format( "Failed to find property {0}.{1}", type.Name, member.Name ) );

		}

		private static TweenAssignmentCallback<T> setField( object target, System.Type type, FieldInfo field )
		{

			if( field.IsLiteral )
				throw new ArgumentException( string.Format( "Field {0}.{1} cannot be assigned to", type.Name, field.Name ) );

			return ( result ) =>
			{
				field.SetValue( target, result );
			};

		}

		private static TweenAssignmentCallback<T> setProperty( object target, System.Type type, PropertyInfo property )
		{

			var setter = getSetMethod( property );
			if( setter == null )
				throw new ArgumentException( string.Format( "Property {0}.{1} cannot be assigned to", type.Name, property.Name ) );

			var paramArray = new object[ 1 ];

			return ( result ) =>
			{
				paramArray[ 0 ] = result;
				setter.Invoke( target, paramArray );
			};

		}

		#endregion 

	}

}
