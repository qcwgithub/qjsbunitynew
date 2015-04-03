/* Copyright 2014 Daikon Forge */
using UnityEngine;
using System.Collections;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Provides common Flash-style easing functions
	/// Each function takes in a value T (representing linear normalized time from 0 - 1) and returns a modified T value
	/// 
	/// Except for Linear, Spring, Bounce, and Punch, all other tween functions have Ease In, Ease Out, and Ease In&Out versions
	/// </summary>
	public class TweenEasingFunctions
	{

		public static float Linear( float t )
		{
			return t;
		}

		public static float Spring( float t )
		{
			float time = Mathf.Clamp01( t );
			time = ( Mathf.Sin( time * Mathf.PI * ( 0.2f + 2.5f * time * time * time ) ) * Mathf.Pow( 1f - time, 2.2f ) + time ) * ( 1f + ( 1.2f * ( 1f - time ) ) );
			return time;
		}

		public static float EaseInQuad( float t )
		{
			return t * t;
		}

		public static float EaseOutQuad( float t )
		{
			return -1f * t * ( t - 2 );
		}

		public static float EaseInOutQuad( float t )
		{
			t /= .5f;
			if( t < 1 )
				return 1f / 2 * t * t;
			t--;
			return -1f / 2 * ( t * ( t - 2 ) - 1 );
		}

		public static float EaseInCubic( float t )
		{
			return t * t * t;
		}

		public static float EaseOutCubic( float t )
		{
			t--;
			return t * t * t + 1;
		}

		public static float EaseInOutCubic( float t )
		{
			t /= .5f;
			if( t < 1 )
				return 1f / 2 * t * t * t;
			t -= 2;
			return 1f / 2 * ( t * t * t + 2 );
		}

		public static float EaseInQuart( float t )
		{
			return t * t * t * t;
		}

		public static float EaseOutQuart( float t )
		{
			t--;
			return -1f * ( t * t * t * t - 1 );
		}

		public static float EaseInOutQuart( float t )
		{
			t /= .5f;
			if( t < 1 )
				return 1f / 2 * t * t * t * t;
			t -= 2;
			return -1f / 2 * ( t * t * t * t - 2 );
		}

		public static float EaseInQuint( float t )
		{
			return t * t * t * t * t;
		}

		public static float EaseOutQuint( float t )
		{
			t--;
			return ( t * t * t * t * t + 1 );
		}

		public static float EaseInOutQuint( float t )
		{
			t /= .5f;
			if( t < 1 )
				return 1f / 2 * t * t * t * t * t;
			t -= 2;
			return 1f / 2 * ( t * t * t * t * t + 2 );
		}

		public static float EaseInSine( float t )
		{
			return -1f * Mathf.Cos( t / 1 * ( Mathf.PI / 2 ) ) + 1f;
		}

		public static float EaseOutSine( float t )
		{
			return Mathf.Sin( t / 1 * ( Mathf.PI / 2 ) );
		}

		public static float EaseInOutSine( float t )
		{
			return -1f / 2 * ( Mathf.Cos( Mathf.PI * t / 1 ) - 1 );
		}

		public static float EaseInExpo( float t )
		{
			return Mathf.Pow( 2, 10 * ( t / 1 - 1 ) );
		}

		public static float EaseOutExpo( float t )
		{
			return ( -Mathf.Pow( 2, -10 * t / 1 ) + 1 );
		}

		public static float EaseInOutExpo( float t )
		{
			t /= .5f;
			if( t < 1 )
				return 1f / 2 * Mathf.Pow( 2, 10 * ( t - 1 ) );
			t--;
			return 1f / 2 * ( -Mathf.Pow( 2, -10 * t ) + 2 );
		}

		public static float EaseInCirc( float t )
		{
			return -1f * ( Mathf.Sqrt( 1 - t * t ) - 1 );
		}

		public static float EaseOutCirc( float t )
		{
			t--;
			return Mathf.Sqrt( 1 - t * t );
		}

		public static float EaseInOutCirc( float t )
		{
			t /= .5f;
			if( t < 1 )
				return -1f / 2 * ( Mathf.Sqrt( 1 - t * t ) - 1 );
			t -= 2;
			return 1f / 2 * ( Mathf.Sqrt( 1 - t * t ) + 1 );
		}

		public static float EaseInBack( float t )
		{
			t /= 1;
			float s = 1.70158f;
			return t * t * ( ( s + 1 ) * t - s );
		}

		public static float EaseOutBack( float t )
		{
			float s = 1.70158f;
			t = ( t / 1 ) - 1;
			return ( t * t * ( ( s + 1 ) * t + s ) + 1 );
		}

		public static float EaseInOutBack( float t )
		{
			float s = 1.70158f;
			t /= .5f;
			if( t < 1 )
			{
				s *= ( 1.525f );
				return 1f / 2 * ( t * t * ( ( ( s ) + 1 ) * t - s ) );
			}
			t -= 2;
			s *= ( 1.525f );
			return 1f / 2 * ( t * t * ( ( ( s ) + 1 ) * t + s ) + 2 );
		}

		public static float Bounce( float t )
		{
			t /= 1f;
			if( t < ( 1 / 2.75f ) )
			{
				return ( 7.5625f * t * t );
			}
			else if( t < ( 2 / 2.75f ) )
			{
				t -= ( 1.5f / 2.75f );
				return ( 7.5625f * ( t ) * t + .75f );
			}
			else if( t < ( 2.5 / 2.75 ) )
			{
				t -= ( 2.25f / 2.75f );
				return ( 7.5625f * ( t ) * t + .9375f );
			}
			else
			{
				t -= ( 2.625f / 2.75f );
				return ( 7.5625f * ( t ) * t + .984375f );
			}
		}

		public static float Punch( float t )
		{
			float s = 9;
			if( t == 0 )
			{
				return 0;
			}
			if( t == 1 )
			{
				return 0;
			}
			float period = 1 * 0.3f;
			s = period / ( 2 * Mathf.PI ) * Mathf.Asin( 0 );
			return ( Mathf.Pow( 2, -10 * t ) * Mathf.Sin( ( t * 1 - s ) * ( 2 * Mathf.PI ) / period ) );
		}

		/// <summary>
		/// Returns an <see cref="EasignFunction"/> delegate that implements the 
		/// easing equation defined by the <paramref name="easeType"/> argument.
		/// </summary>
		/// <param name="easeType">The easing equation to be used</param>
		public static TweenEasingCallback GetFunction( EasingType easeType )
		{

			switch( easeType )
			{
				case EasingType.BackEaseIn:
					return EaseInBack;
				case EasingType.BackEaseInOut:
					return EaseInOutBack;
				case EasingType.BackEaseOut:
					return EaseOutBack;
				case EasingType.Bounce:
					return Bounce;
				case EasingType.CircEaseIn:
					return EaseInCirc;
				case EasingType.CircEaseInOut:
					return EaseInOutCirc;
				case EasingType.CircEaseOut:
					return EaseOutCirc;
				case EasingType.CubicEaseIn:
					return EaseInCubic;
				case EasingType.CubicEaseInOut:
					return EaseInOutCubic;
				case EasingType.CubicEaseOut:
					return EaseOutCubic;
				case EasingType.ExpoEaseIn:
					return EaseInExpo;
				case EasingType.ExpoEaseInOut:
					return EaseInOutExpo;
				case EasingType.ExpoEaseOut:
					return EaseOutExpo;
				case EasingType.Linear:
					return Linear;
				case EasingType.QuadEaseIn:
					return EaseInQuad;
				case EasingType.QuadEaseInOut:
					return EaseInOutQuad;
				case EasingType.QuadEaseOut:
					return EaseOutQuad;
				case EasingType.QuartEaseIn:
					return EaseInQuart;
				case EasingType.QuartEaseInOut:
					return EaseInOutQuart;
				case EasingType.QuartEaseOut:
					return EaseOutQuart;
				case EasingType.QuintEaseIn:
					return EaseInQuint;
				case EasingType.QuintEaseInOut:
					return EaseInOutQuint;
				case EasingType.QuintEaseOut:
					return EaseOutQuint;
				case EasingType.SineEaseIn:
					return EaseInSine;
				case EasingType.SineEaseInOut:
					return EaseInOutSine;
				case EasingType.SineEaseOut:
					return EaseOutSine;
				case EasingType.Spring:
					return Spring;
			}

			throw new System.NotImplementedException();

		}

	}

}
