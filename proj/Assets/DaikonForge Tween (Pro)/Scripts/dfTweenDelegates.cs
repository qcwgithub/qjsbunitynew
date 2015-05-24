/* Copyright 2014 Daikon Forge */
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Defines the signature for tween callback methods
	/// </summary>
	/// <param name="sender">The Tween instance that is executing the callback method</param>
	public delegate void TweenCallback( TweenBase sender );

	/// <summary>
	/// Defines the signature for tween value sync methods 
	/// </summary>
	public delegate T TweenSyncCallback<T>();

	/// <summary>
	/// Defines the signature for the method that will be used to implement the easing function for a Tween
	/// </summary>
	public delegate float TweenEasingCallback( float time );

	/// <summary>
	/// Defines the signature for the method that will be used to assign the current tween value
	/// to the target object.
	/// </summary>
	public delegate void TweenAssignmentCallback<T>( T currentValue );

}

