/* Copyright 2014 Daikon Forge */
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Indicates the current state of a Tween
	/// </summary>
	public enum TweenState
	{

		/// <summary>The tween is not currently running</summary>
		Stopped,

		/// <summary>The tween is currently paused</summary>
		Paused,

		/// <summary>The tween is currently running</summary>
		Playing,

		/// <summary>The tween was started, but is waiting for the StartDelay to expire</summary>
		Started

	}

	/// <summary>
	/// Indicates the direction that a tween or animation will play
	/// </summary>
	public enum TweenDirection
	{
		Forward = 0,
		Reverse
	}

	/// <summary>
	/// Indicates the manner in which a Tween will loop
	/// </summary>
	public enum TweenLoopType
	{

		/// <summary>
		/// The tween will stop executing when it reaches the end value
		/// </summary>
		None,

		/// <summary>
		/// The tween will play back from the beginning when it reaches the end value
		/// </summary>
		Loop,

		/// <summary>
		/// The tween will play backwards and forwards
		/// </summary>
		Pingpong

	}

	/// <summary>
	/// Used by the TweenGroup class to determine whether to play the tweens 
	/// sequentially or concurrently
	/// </summary>
	public enum TweenGroupMode
	{

		/// <summary>
		/// Each tween is played one after another in sequence, with each tween
		/// waiting until the previous tween is completed before continuing
		/// </summary>
		Sequential,

		/// <summary>
		/// All tweens are played at the same time
		/// </summary>
		Concurrent

	}

	/// <summary>
	/// The type of easing function (which specifies a the rate of change of a value over time) to use for animating.
	/// </summary>
	public enum EasingType
	{

		/// <summary>
		/// Easing equation function for a simple linear tweening, with no easing.
		/// </summary>
		Linear,

		/// <summary>
		/// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: 
		/// decelerating from zero velocity.
		/// </summary>
		Bounce,

		/// <summary>
		/// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
		/// accelerating from zero velocity.
		/// </summary>
		BackEaseIn,
		/// <summary>
		/// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
		/// decelerating from zero velocity.
		/// </summary>
		BackEaseOut,
		/// <summary>
		/// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
		/// acceleration until halfway, then deceleration.
		/// </summary>
		BackEaseInOut,

		/// <summary>
		/// Easing equation function for a circular (sqrt(1-t^2)) easing in: 
		/// accelerating from zero velocity.
		/// </summary>
		CircEaseIn,
		/// <summary>
		/// Easing equation function for a circular (sqrt(1-t^2)) easing out: 
		/// decelerating from zero velocity.
		/// </summary>
		CircEaseOut,
		/// <summary>
		/// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
		/// acceleration until halfway, then deceleration.
		/// </summary>
		CircEaseInOut,

		/// <summary>
		/// Easing equation function for a cubic (t^3) easing in: 
		/// accelerating from zero velocity.
		/// </summary>
		CubicEaseIn,
		/// <summary>
		/// Easing equation function for a cubic (t^3) easing out: 
		/// decelerating from zero velocity.
		/// </summary>
		CubicEaseOut,
		/// <summary>
		/// Easing equation function for a cubic (t^3) easing in/out: 
		/// acceleration until halfway, then deceleration.
		/// </summary>
		CubicEaseInOut,

		/// <summary>
		/// Easing equation function for an exponential (2^t) easing in: 
		/// accelerating from zero velocity.
		/// </summary>
		ExpoEaseIn,
		/// <summary>
		/// Easing equation function for an exponential (2^t) easing out: 
		/// decelerating from zero velocity.
		/// </summary>
		ExpoEaseOut,
		/// <summary>
		/// Easing equation function for an exponential (2^t) easing in/out: 
		/// acceleration until halfway, then deceleration.
		/// </summary>
		ExpoEaseInOut,

		/// <summary>
		/// Easing equation function for a quadratic (t^2) easing in: 
		/// accelerating from zero velocity.
		/// </summary>
		QuadEaseIn,
		/// <summary>
		/// Easing equation function for a quadratic (t^2) easing out: 
		/// decelerating from zero velocity.
		/// </summary>
		QuadEaseOut,
		/// <summary>
		/// Easing equation function for a quadratic (t^2) easing in/out: 
		/// acceleration until halfway, then deceleration.
		/// </summary>
		QuadEaseInOut,

		/// <summary>
		/// Easing equation function for a quartic (t^4) easing in: 
		/// accelerating from zero velocity.
		/// </summary>
		QuartEaseIn,
		/// <summary>
		/// Easing equation function for a quartic (t^4) easing out: 
		/// decelerating from zero velocity.
		/// </summary>
		QuartEaseOut,
		/// <summary>
		/// Easing equation function for a quartic (t^4) easing in/out: 
		/// acceleration until halfway, then deceleration.
		/// </summary>
		QuartEaseInOut,

		/// <summary>
		/// Easing equation function for a quintic (t^5) easing in: 
		/// accelerating from zero velocity.
		/// </summary>
		QuintEaseIn,
		/// <summary>
		/// Easing equation function for a quintic (t^5) easing out: 
		/// decelerating from zero velocity.
		/// </summary>
		QuintEaseOut,
		/// <summary>
		/// Easing equation function for a quintic (t^5) easing in/out: 
		/// acceleration until halfway, then deceleration.
		/// </summary>
		QuintEaseInOut,

		/// <summary>
		/// Easing equation function for a sinusoidal (sin(t)) easing in: 
		/// accelerating from zero velocity.
		/// </summary>
		SineEaseIn,
		/// <summary>
		/// Easing equation function for a sinusoidal (sin(t)) easing out: 
		/// decelerating from zero velocity.
		/// </summary>
		SineEaseOut,
		/// <summary>
		/// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
		/// acceleration until halfway, then deceleration.
		/// </summary>
		SineEaseInOut,

		Spring

	}

}
