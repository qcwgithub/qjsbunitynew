/* Copyright 2014 Daikon Forge */
using System;
using System.Collections;

using UnityEngine;

using DaikonForge.Tween;
using DaikonForge.Tween.Interpolation;

/// <summary>
/// Implements extension methods can be be useful when chaining tween callbacks
/// </summary>
internal static class TweenCallbackExtensions
{

	/// <summary>
	/// Returns a new TweenCallback function that first executes the original callback, then
	/// executes the chained callback.
	/// </summary>
	/// <param name="original">The original callback function to be chained</param>
	/// <param name="callback">The additional callback to be executed</param>
	/// <returns>A new wrapped TweenCallback instance which calls the original method first, followed by the new method</returns>
	internal static TweenCallback Chain( this TweenCallback original, TweenCallback callback )
	{
		return ( tween ) =>
		{
			if( original != null )
				original( tween );
			if( callback != null )
				callback( tween );
		};
	}

}

/// <summary>
/// Implements extension methods that can be used to conveniently tween any named property
/// of an object instance through reflection.
/// </summary>
public static class TweenReflectionExtensions
{

	/// <summary>
	/// Returns a Tween&lt;T&gt; instance that is configured to animate the named property
	/// </summary>
	public static Tween<T> TweenProperty<T>( this object target, string propertyName )
	{
		return TweenNamedProperty<T>.Obtain( target, propertyName );
	}

	/// <summary>
	/// Returns a Tween&lt;T&gt; instance that is configured to animate the named property
	/// </summary>
	public static Tween<T> TweenProperty<T>( this object target, string propertyName, Interpolator<T> interpolator )
	{
		return TweenNamedProperty<T>.Obtain( target, propertyName, interpolator );
	}

}

/// <summary>
/// Implements extension methods that can be used to conveniently tween commonly-animated 
/// properties of a Material instance.
/// </summary>
public static partial class TweenMaterialExtensions
{

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Material.color property
	/// </summary>
	public static Tween<Color> TweenColor( this Material material )
	{

		var tween = Tween<Color>
			.Obtain()
			.SetStartValue( material.color )
			.SetEndValue( material.color )
			.SetDuration( 1f )
			.OnExecute( ( currentValue ) => { material.color = currentValue; } );

		return tween;

	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate the alpha channel of a 
	/// Material.color property. Note that this function relies on the target component
	/// having a renderer whose sharedMaterial property has been assigned. If it has not,
	/// a NullReferenceException will be thrown.
	/// </summary>
	public static Tween<float> TweenAlpha( this Material material )
	{

		var tween = Tween<float>
			.Obtain()
			.SetStartValue( material.color.a )
			.SetEndValue( material.color.a )
			.SetDuration( 1f )
			.OnExecute( ( currentValue ) =>
			{

				var newColor = material.color;
				newColor.a = currentValue;

				material.color = newColor;

			} );

		return tween;

	}

}

public static partial class TweenTextExtensions
{

	/// <summary>
	/// Returns a Tween instance that is configured to animate the alpha channel of a
	/// TextMesh's color property.
	/// </summary>
	/// <param name="text">The TextMesh to animate</param>
	public static Tween<float> TweenAlpha( this GUIText text )
	{

		var tween = Tween<float>
			.Obtain()
			.SetStartValue( text.color.a )
			.SetEndValue( text.color.a )
			.SetDuration( 1f )
			.OnExecute( ( currentValue ) =>
			{

				var newColor = text.color;
				newColor.a = currentValue;

				text.color = newColor;

			} );

		return tween;

	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate the color property of a 
	/// TextMesh instance.
	/// </summary>
	/// <param name="text">The TextMesh to animate</param>
	public static Tween<Color> TweenColor( this GUIText text )
	{

		var tween = Tween<Color>
			.Obtain()
			.SetStartValue( text.color )
			.SetEndValue( text.color )
			.SetDuration( 1f )
			.OnExecute( ( currentValue ) => { text.color = currentValue; } );

		return tween;

	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate the alpha channel of a
	/// TextMesh's color property.
	/// </summary>
	/// <param name="text">The TextMesh to animate</param>
	public static Tween<float> TweenAlpha( this TextMesh text )
	{

		var tween = Tween<float>
			.Obtain()
			.SetStartValue( text.color.a )
			.SetEndValue( text.color.a )
			.SetDuration( 1f )
			.OnExecute( ( currentValue ) =>
			{

				var newColor = text.color;
				newColor.a = currentValue;

				text.color = newColor;

			} );

		return tween;

	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate the color property of a 
	/// TextMesh instance.
	/// </summary>
	/// <param name="text">The TextMesh to animate</param>
	public static Tween<Color> TweenColor( this TextMesh text )
	{

		var tween = Tween<Color>
			.Obtain()
			.SetStartValue( text.color )
			.SetEndValue( text.color )
			.SetDuration( 1f )
			.OnExecute( ( currentValue ) => { text.color = currentValue; } );

		return tween;

	}

}

/// <summary>
/// Implements extension methods that can be used to conveniently tween commonly-animated 
/// properties of a TextMesh instance.
/// </summary>
public static partial class TweenSpriteExtensions
{

	/// <summary>
	/// Returns a Tween instance that is configured to animate the alpha channel of a
	/// SpriteRenderer's color property.
	/// </summary>
	/// <param name="sprite">The SpriteRenderer to animate</param>
	public static Tween<float> TweenAlpha( this SpriteRenderer sprite )
	{

		var tween = Tween<float>
			.Obtain()
			.SetStartValue( sprite.color.a )
			.SetEndValue( sprite.color.a )
			.SetDuration( 1f )
			.OnExecute( ( currentValue ) =>
			{

				var newColor = sprite.color;
				newColor.a = currentValue;

				sprite.color = newColor;

			} );

		return tween;

	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate the color property of a 
	/// SpriteRenderer instance.
	/// </summary>
	/// <param name="sprite">The SpriteRenderer to animate</param>
	public static Tween<Color> TweenColor( this SpriteRenderer sprite )
	{

		var tween = Tween<Color>
			.Obtain()
			.SetStartValue( sprite.color )
			.SetEndValue( sprite.color )
			.SetDuration( 1f )
			.OnExecute( ( currentValue ) => { sprite.color = currentValue; } );

		return tween;

	}

}

/// <summary>
/// Implements extension methods that can be used to conveniently tween commonly-animated 
/// properties of a Component instance.
/// </summary>
public static partial class TweenComponentExtensions
{

	/// <summary>
	/// Returns a Tween instance that is configured to animate the alpha channel of a 
	/// Material.color property. Note that this function relies on the target component
	/// having a renderer whose sharedMaterial property has been assigned. If it has not,
	/// a NullReferenceException will be thrown.
	/// </summary>
	/// <param name="component"></param>
	public static Tween<float> TweenAlpha( this Component component )
	{

		// Special cases for components which have a specific color, renderer, or material property
		if( component is TextMesh )
		{
			return TweenTextExtensions.TweenAlpha( (TextMesh)component );
		}
		else if( component is GUIText )
		{
			return TweenMaterialExtensions.TweenAlpha( ( (GUIText)component ).material );
		}
		else if( component.renderer is SpriteRenderer )
		{
			return TweenSpriteExtensions.TweenAlpha( (SpriteRenderer)component.renderer );
		}

		if( component.renderer == null )
			throw new NullReferenceException( "Component does not have a Renderer assigned" );

		var material = component.renderer.material;
		if( material == null )
			throw new NullReferenceException( "Component does not have a Material assigned" );

		return TweenMaterialExtensions.TweenAlpha( material );

	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Material.color property.
	/// Note that this function relies on the target having a renderer whose material property
	/// has been assigned. If it has not, a NullReferenceException will be thrown.
	/// </summary>
	/// <param name="component">The Monobehavior whose color will be animated.</param>
	public static Tween<Color> TweenColor( this Component component )
	{

		// Special cases for components which have a specific color property
		if( component is TextMesh )
		{
			return TweenTextExtensions.TweenColor( (TextMesh)component );
		}
		else if( component is GUIText )
		{
			return TweenMaterialExtensions.TweenColor( ( (GUIText)component ).material );
		}
		else if( component.renderer is SpriteRenderer )
		{
			return TweenSpriteExtensions.TweenColor( (SpriteRenderer)component.renderer );
		}

		if( component.renderer == null )
			throw new NullReferenceException( "Component does not have a Renderer assigned" );

		var material = component.renderer.material;
		if( material == null )
			throw new NullReferenceException( "Component does not have a Material assigned" );

		return TweenMaterialExtensions.TweenColor( material );

	}

#if !FREE_VERSION
	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform along a spline path
	/// </summary>
	/// <param name="component">The component whose position will be animated along the path</param>
	/// <param name="path">The Spline path to follow</param>
	public static Tween<float> TweenPath( this Component component, IPathIterator path )
	{
		return TweenPath( component, path, true );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform along a spline path
	/// </summary>
	/// <param name="component">The component whose position will be animated along the path</param>
	/// <param name="path">The Spline path to follow</param>
	/// <param name="orientToPath">If set to TRUE, the object will be oriented to match the path's direction</param>
	public static Tween<float> TweenPath( this Component component, IPathIterator path, bool orientToPath )
	{

		if( component == null )
			throw new ArgumentNullException( "component" );

		return TweenTransformExtensions.TweenPath( component.transform, path );

	}
#endif

	#region Shake tweens 

	/// <summary>
	/// Returns a dfTweenShake instance that is configured to shake a transform's position
	/// </summary>
	public static TweenShake ShakePosition( this Component component )
	{
		return component.transform.ShakePosition();
	}

	/// <summary>
	/// Returns a dfTweenShake instance that is configured to shake a transform's position
	/// </summary>
	public static TweenShake ShakePosition( this Component component, bool localPosition )
	{
		return component.transform.ShakePosition( localPosition );
	}

	#endregion 

	#region Scale tweens 

	/// <summary>
	/// Returns a Tween instance that is configured to animate the Transform.localScale property
	/// from the desired start scale to the transform's current scale.
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="startScale">The desired start scale</param>
	public static Tween<Vector3> TweenScaleFrom( this Component component, Vector3 startScale )
	{
		return TweenScale( component ).SetStartValue( startScale );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate the Transform.localScale property
	/// from the current scale to the desired end scale.
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="endScale">The desired end scale</param>
	public static Tween<Vector3> TweenScaleTo( this Component component, Vector3 endScale )
	{
		return TweenTransformExtensions.TweenScale( component.transform ).SetEndValue( endScale );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.localScale property
	/// </summary>
	/// <param name="component">The component to be animated</param>
	public static Tween<Vector3> TweenScale( this Component component )
	{
		return TweenTransformExtensions.TweenScale( component.transform );
	}

	#endregion 

	#region Rotation tweens

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the specified start value to the object's current rotation.
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="startRotation">The desired start rotation</param>
	public static Tween<Vector3> TweenRotateFrom( this Component component, Vector3 startRotation )
	{
		return TweenRotateFrom( component, startRotation, true, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the specified start value to the object's current rotation.
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="startRotation">The desired start rotation</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	public static Tween<Vector3> TweenRotateFrom( this Component component, Vector3 startRotation, bool useShortestPath )
	{
		return TweenRotateFrom( component, startRotation, useShortestPath, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the specified start value to the object's current rotation.
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="startRotation">The desired start rotation</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	/// <param name="useLocalRotation">If set to TRUE, then Transform.localEulerAngles will be animated instead of Transform.eulerAngles</param>
	public static Tween<Vector3> TweenRotateFrom( this Component component, Vector3 startRotation, bool useShortestPath, bool useLocalRotation )
	{
		return TweenRotation( component.transform, useShortestPath, useLocalRotation ).SetStartValue( startRotation );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the current rotation to the specified end rotation.
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="endRotation">The desired end rotation</param>
	public static Tween<Vector3> TweenRotateTo( this Component component, Vector3 endRotation )
	{
		return TweenRotateTo( component, endRotation, true, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the current rotation to the specified end rotation.
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="endRotation">The desired end rotation</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	public static Tween<Vector3> TweenRotateTo( this Component component, Vector3 endRotation, bool useShortestPath )
	{
		return TweenRotateTo( component, endRotation, useShortestPath, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the current rotation to the specified end rotation.
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="endRotation">The desired end rotation</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	/// <param name="useLocalRotation">If set to TRUE, then Transform.localEulerAngles will be animated instead of Transform.eulerAngles</param>
	public static Tween<Vector3> TweenRotateTo( this Component component, Vector3 endRotation, bool useShortestPath, bool useLocalRotation )
	{
		return TweenRotation( component.transform, useShortestPath, useLocalRotation ).SetEndValue( endRotation );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property
	/// </summary>
	/// <param name="component">The component to be animated</param>
	public static Tween<Vector3> TweenRotation( this Component component )
	{
		return TweenTransformExtensions.TweenRotation( component.transform, true, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	public static Tween<Vector3> TweenRotation( this Component component, bool useShortestPath )
	{
		return TweenTransformExtensions.TweenRotation( component.transform, useShortestPath, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	/// <param name="useLocalRotation">If set to TRUE, then Transform.localEulerAngles will be animated instead of Transform.eulerAngles</param>
	public static Tween<Vector3> TweenRotation( this Component component, bool useShortestPath, bool useLocalRotation )
	{
		return TweenTransformExtensions.TweenRotation( component.transform, useShortestPath, useLocalRotation );
	}

	#endregion 

	#region Position tweens

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property 
	/// to move from the specified start position to its current position.
	/// </summary>
	/// <param name="component">The Component to animate</param>
	/// <param name="startPosition">The desired start position</param>
	public static Tween<Vector3> TweenMoveFrom( this Component component, Vector3 startPosition )
	{
		return TweenMoveFrom( component, startPosition, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property 
	/// to move from the specified start position to its current position.
	/// </summary>
	/// <param name="component">The Component to animate</param>
	/// <param name="startPosition">The desired start position</param>
	/// <param name="useLocalPosition">If set to TRUE, the Transform.localPosition property will be animated rather than Transform.position</param>
	public static Tween<Vector3> TweenMoveFrom( this Component component, Vector3 startPosition, bool useLocalPosition )
	{
		return TweenPosition( component, useLocalPosition )
			.SetStartValue( startPosition );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property 
	/// to move from its current position to the specified end position
	/// </summary>
	/// <param name="component">The Component to animate</param>
	/// <param name="endPosition">The desired end position</param>
	public static Tween<Vector3> TweenMoveTo( this Component component, Vector3 endPosition )
	{
		return TweenMoveTo( component, endPosition, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property 
	/// to move from its current position to the specified end position
	/// </summary>
	/// <param name="component">The Component to animate</param>
	/// <param name="endPosition">The desired end position</param>
	/// <param name="useLocalPosition">If set to TRUE, the Transform.localPosition property will be animated rather than Transform.position</param>
	public static Tween<Vector3> TweenMoveTo( this Component component, Vector3 endPosition, bool useLocalPosition )
	{
		return TweenPosition( component, useLocalPosition )
			.SetEndValue( endPosition );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property
	/// </summary>
	/// <param name="component">The component to be animated</param>
	public static Tween<Vector3> TweenPosition( this Component component )
	{
		return TweenTransformExtensions.TweenPosition( component.transform, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property
	/// </summary>
	/// <param name="component">The component to be animated</param>
	/// <param name="useLocalPosition">If set to TRUE, the Transform.localPosition property will be used instead of Transform.position</param>
	public static Tween<Vector3> TweenPosition( this Component component, bool useLocalPosition )
	{
		return TweenTransformExtensions.TweenPosition( component.transform, useLocalPosition );
	}

	#endregion 

}

/// <summary>
/// Implements extension methods that can be used to conveniently tween commonly-animated 
/// properties of a Transform instance.
/// </summary>
public static partial class TweenTransformExtensions
{

	#region Shake tweens 

	/// <summary>
	/// Returns a dfTweenShake instance that is configured to shake a transform's position
	/// </summary>
	public static TweenShake ShakePosition( this Transform transform )
	{
		return transform.ShakePosition( false );
	}

	/// <summary>
	/// Returns a dfTweenShake instance that is configured to shake a transform's position
	/// </summary>
	public static TweenShake ShakePosition( this Transform transform, bool localPosition )
	{
		return TweenShake.Obtain()
			.SetStartValue( localPosition ? transform.localPosition : transform.position )
			.SetShakeMagnitude( 0.1f )
			.SetDuration( 1f )
			.SetShakeSpeed( 10f )
			.OnExecute( ( result ) =>
			{
				if( localPosition )
				{
					transform.localPosition = result;
				}
				else
				{
					transform.position = result;
				}
			} );
	}

	#endregion 

	#region Path tweens 

#if !FREE_VERSION
	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform along a spline path
	/// </summary>
	/// <param name="transform">The Transfrom whose position will be animated along the path</param>
	/// <param name="path">The Spline path to follow</param>
	public static Tween<float> TweenPath( this Transform transform, IPathIterator path )
	{
		return TweenPath( transform, path, true );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform along a spline path
	/// </summary>
	/// <param name="transform">The Transfrom whose position will be animated along the path</param>
	/// <param name="path">The Spline path to follow</param>
	/// <param name="orientToPath">If set to TRUE, the object will be oriented to match the path's direction</param>
	public static Tween<float> TweenPath( this Transform transform, IPathIterator path, bool orientToPath )
	{

		if( transform == null )
			throw new ArgumentNullException( "transform" );

		if( path == null )
			throw new ArgumentNullException( "path" );

		// Declared here so that it can be referenced in the anonymous function assigned in OnExecute()
		Tween<float> tween = null;

		tween = Tween<float>.Obtain()
			.SetStartValue( 0f )
			.SetEndValue( 1f )
			.SetEasing( TweenEasingFunctions.Linear )
			.OnExecute( ( time ) =>
			{

				transform.position = path.GetPosition( time );

				if( orientToPath )
				{
					var direction = path.GetTangent( time );
					if( tween.PlayDirection == TweenDirection.Forward )
						transform.forward = direction;
					else
						transform.forward = -direction;
				}

			} );

		return tween;

	}
#endif

	#endregion 

	#region Scale tweens 

	/// <summary>
	/// Returns a Tween instance that is configured to animate the Transform.localScale property
	/// from the desired start scale to the transform's current scale.
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="startScale">The desired start scale</param>
	public static Tween<Vector3> TweenScaleFrom( this Transform transform, Vector3 startScale )
	{
		return TweenScale( transform ).SetStartValue( startScale );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate the Transform.localScale property
	/// from the current scale to the desired end scale.
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="endScale">The desired end scale</param>
	public static Tween<Vector3> TweenScaleTo( this Transform transform, Vector3 endScale )
	{
		return TweenScale( transform ).SetEndValue( endScale );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.localScale property
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	public static Tween<Vector3> TweenScale( this Transform transform )
	{

		var tween = Tween<Vector3>
			.Obtain()
			.SetStartValue( transform.localScale )
			.SetEndValue( transform.localScale )
			.SetDuration( 1f )
			.OnExecute( ( currentValue ) => { transform.localScale = currentValue; } );

		return tween;

	}

	#endregion

	#region Rotation tweens 

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the specified start value to the object's current rotation.
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="startRotation">The desired start rotation</param>
	public static Tween<Vector3> TweenRotateFrom( this Transform transform, Vector3 startRotation )
	{
		return TweenRotateFrom( transform, startRotation, true, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the specified start value to the object's current rotation.
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="startRotation">The desired start rotation</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	public static Tween<Vector3> TweenRotateFrom( this Transform transform, Vector3 startRotation, bool useShortestPath )
	{
		return TweenRotateFrom( transform, startRotation, useShortestPath, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the specified start value to the object's current rotation.
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="startRotation">The desired start rotation</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	/// <param name="useLocalRotation">If set to TRUE, then Transform.localEulerAngles will be animated instead of Transform.eulerAngles</param>
	public static Tween<Vector3> TweenRotateFrom( this Transform transform, Vector3 startRotation, bool useShortestPath, bool useLocalRotation )
	{
		return TweenRotation( transform, useShortestPath, useLocalRotation ).SetStartValue( startRotation );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the current rotation to the specified end rotation.
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="endRotation">The desired end rotation</param>
	public static Tween<Vector3> TweenRotateTo( this Transform transform, Vector3 endRotation )
	{
		return TweenRotateTo( transform, endRotation, true, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the current rotation to the specified end rotation.
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="endRotation">The desired end rotation</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	public static Tween<Vector3> TweenRotateTo( this Transform transform, Vector3 endRotation, bool useShortestPath )
	{
		return TweenRotateTo( transform, endRotation, useShortestPath, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property to 
	/// animate from the current rotation to the specified end rotation.
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="endRotation">The desired end rotation</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	/// <param name="useLocalRotation">If set to TRUE, then Transform.localEulerAngles will be animated instead of Transform.eulerAngles</param>
	public static Tween<Vector3> TweenRotateTo( this Transform transform, Vector3 endRotation, bool useShortestPath, bool useLocalRotation )
	{
		return TweenRotation( transform, useShortestPath, useLocalRotation ).SetEndValue( endRotation );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	public static Tween<Vector3> TweenRotation( this Transform transform )
	{
		return TweenRotation( transform, true, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	public static Tween<Vector3> TweenRotation( this Transform transform, bool useShortestPath )
	{
		return TweenRotation( transform, useShortestPath, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.eulerAngles property
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="useShortestPath">If set to TRUE, the rotation will follow the shortest path.</param>
	/// <param name="useLocalRotation">If set to TRUE, then Transform.localEulerAngles will be animated instead of Transform.eulerAngles</param>
	public static Tween<Vector3> TweenRotation( this Transform transform, bool useShortestPath, bool useLocalRotation )
	{

		var interpolator = useShortestPath
			? (Interpolator<Vector3>)EulerInterpolator.Default
			: (Interpolator<Vector3>)Vector3Interpolator.Default;

		var startValue = useLocalRotation ? transform.localEulerAngles : transform.eulerAngles;

		TweenAssignmentCallback<Vector3> updateFunc = null;
		if( useLocalRotation )
			updateFunc = ( localValue ) => { transform.localEulerAngles = localValue; };
		else
			updateFunc = ( globalValue ) => { transform.eulerAngles = globalValue; };

		var tween = Tween<Vector3>
			.Obtain()
			.SetStartValue( startValue )
			.SetEndValue( startValue )
			.SetDuration( 1f )
			.SetInterpolator( interpolator )
			.OnExecute( updateFunc );

		return tween;

	}

	#endregion 

	#region Position tweens 

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property 
	/// to move from the specified start position to its current position.
	/// </summary>
	/// <param name="transform">The Transform to animate</param>
	/// <param name="startPosition">The desired start position</param>
	public static Tween<Vector3> TweenMoveFrom( this Transform transform, Vector3 startPosition )
	{
		return TweenMoveFrom( transform, startPosition, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property 
	/// to move from the specified start position to its current position.
	/// </summary>
	/// <param name="transform">The Transform to animate</param>
	/// <param name="startPosition">The desired start position</param>
	/// <param name="useLocalPosition">If set to TRUE, the Transform.localPosition property will be animated rather than Transform.position</param>
	public static Tween<Vector3> TweenMoveFrom( this Transform transform, Vector3 startPosition, bool useLocalPosition )
	{
		return TweenPosition( transform, useLocalPosition )
			.SetStartValue( startPosition );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property 
	/// to move from its current position to the specified end position
	/// </summary>
	/// <param name="transform">The Transform to animate</param>
	/// <param name="endPosition">The desired end position</param>
	public static Tween<Vector3> TweenMoveTo( this Transform transform, Vector3 endPosition )
	{
		return TweenMoveTo( transform, endPosition, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property 
	/// to move from its current position to the specified end position
	/// </summary>
	/// <param name="transform">The Transform to animate</param>
	/// <param name="endPosition">The desired end position</param>
	/// <param name="useLocalPosition">If set to TRUE, the Transform.localPosition property will be animated rather than Transform.position</param>
	public static Tween<Vector3> TweenMoveTo( this Transform transform, Vector3 endPosition, bool useLocalPosition )
	{
		return TweenPosition( transform, useLocalPosition )
			.SetEndValue( endPosition );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	public static Tween<Vector3> TweenPosition( this Transform transform )
	{
		return TweenPosition( transform, false );
	}

	/// <summary>
	/// Returns a Tween instance that is configured to animate a Transform.position property
	/// </summary>
	/// <param name="transform">The transform to be animated</param>
	/// <param name="useLocalPosition">If set to TRUE, the Transform.localPosition property will be used instead of Transform.position</param>
	public static Tween<Vector3> TweenPosition( this Transform transform, bool useLocalPosition )
	{

		var startValue = useLocalPosition ? transform.localPosition : transform.position;

		TweenAssignmentCallback<Vector3> updateFunc = null;
		if( useLocalPosition )
			updateFunc = ( localValue ) => { transform.localPosition = localValue; };
		else
			updateFunc = ( globalValue ) => { transform.position = globalValue; };

		var tween = Tween<Vector3>
			.Obtain()
			.SetStartValue( startValue )
			.SetEndValue( startValue )
			.SetDuration( 1f )
			.OnExecute( updateFunc );

		return tween;

	}

	#endregion 

}

/// <summary>
/// Implements a set of helper functions to create tweens that animate common properties
/// of game objects. This class serves primarily as a helper for UnityScript users, as 
/// a means of simplifying the creation of tweens. C# developers will likely find the 
/// use of this library's extension methods to be clearer and more concise.
/// </summary>
public static partial class Tween
{

	#region Helper methods for SpriteRenderer

	public static Tween<Color> Color( SpriteRenderer renderer )
	{
		return TweenSpriteExtensions.TweenColor( renderer );
	}

	public static Tween<float> Alpha( SpriteRenderer renderer )
	{
		return TweenSpriteExtensions.TweenAlpha( renderer );
	}

	#endregion

	#region Helper methods for Material

	public static Tween<Color> Color( Material material )
	{
		return TweenMaterialExtensions.TweenColor( material );
	}

	public static Tween<float> Alpha( Material material )
	{
		return TweenMaterialExtensions.TweenAlpha( material );
	}

	#endregion

	#region Helper methods for Transform

	#region Position tweens 

	public static Tween<Vector3> Position( Transform transform )
	{
		return Position( transform, false );
	}

	public static Tween<Vector3> Position( Transform transform, bool useLocalPosition )
	{
		return TweenTransformExtensions.TweenPosition( transform, useLocalPosition );
	}

	public static Tween<Vector3> MoveTo( Transform transform, Vector3 endPosition )
	{
		return MoveTo( transform, endPosition, false );
	}

	public static Tween<Vector3> MoveTo( Transform transform, Vector3 endPosition, bool useLocalPosition )
	{
		return TweenTransformExtensions.TweenMoveTo( transform, endPosition, useLocalPosition );
	}

	public static Tween<Vector3> MoveFrom( Transform transform, Vector3 startPosition )
	{
		return TweenTransformExtensions.TweenMoveFrom( transform, startPosition, false );
	}

	public static Tween<Vector3> MoveFrom( Transform transform, Vector3 startPosition, bool useLocalPosition )
	{
		return TweenTransformExtensions.TweenMoveFrom( transform, startPosition, useLocalPosition );
	}

	#endregion 

	#region Rotation tweens 

	public static Tween<Vector3> RotateFrom( Transform transform, Vector3 startRotation )
	{
		return RotateFrom( transform, startRotation, true, false );
	}

	public static Tween<Vector3> RotateFrom( Transform transform, Vector3 startRotation, bool useShortestPath )
	{
		return RotateFrom( transform, startRotation, useShortestPath, false );
	}

	public static Tween<Vector3> RotateFrom( Transform transform, Vector3 startRotation, bool useShortestPath, bool useLocalRotation )
	{
		return TweenTransformExtensions.TweenRotateFrom( transform, startRotation, useShortestPath, useLocalRotation );
	}

	public static Tween<Vector3> RotateTo( Transform transform, Vector3 endRotation )
	{
		return RotateTo( transform, endRotation, true, false );
	}

	public static Tween<Vector3> RotateTo( Transform transform, Vector3 endRotation, bool useShortestPath )
	{
		return RotateTo( transform, endRotation, useShortestPath, false );
	}

	public static Tween<Vector3> RotateTo( Transform transform, Vector3 endRotation, bool useShortestPath, bool useLocalRotation )
	{
		return TweenTransformExtensions.TweenRotateTo( transform, endRotation, useShortestPath, useLocalRotation );
	}

	public static Tween<Vector3> Rotation( Transform transform )
	{
		return TweenTransformExtensions.TweenRotation( transform );
	}

	public static Tween<Vector3> Rotation( Transform transform, bool useShortestPath )
	{
		return Rotation( transform, useShortestPath, false );
	}

	public static Tween<Vector3> Rotation( Transform transform, bool useShortestPath, bool useLocalRotation )
	{
		return TweenTransformExtensions.TweenRotation( transform, useShortestPath, useLocalRotation );
	}

	#endregion 

	#region Scale tweens 

	public static Tween<Vector3> ScaleFrom( Transform transform, Vector3 startScale )
	{
		return Scale( transform ).SetStartValue( startScale );
	}

	public static Tween<Vector3> ScaleTo( Transform transform, Vector3 endScale )
	{
		return Scale( transform ).SetEndValue( endScale );
	}

	public static Tween<Vector3> Scale( Transform transform )
	{
		return TweenTransformExtensions.TweenScale( transform );
	}

	#endregion 

	#region Shake tweens

	public static TweenShake Shake( Transform transform )
	{
		return Shake( transform, false );
	}

	public static TweenShake Shake( Transform transform, bool localPosition )
	{
		return TweenTransformExtensions.ShakePosition( transform, localPosition );
	}

	#endregion 

	#endregion

	#region Helper methods for reflection

	public static Tween<T> NamedProperty<T>( object target, string propertyName )
	{
		return TweenNamedProperty<T>.Obtain( target, propertyName );
	}

	#endregion 

}

