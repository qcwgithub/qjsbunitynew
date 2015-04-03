/* Copyright 2014 Daikon Forge */
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using DaikonForge.Tween;

namespace DaikonForge.Tween.Interpolation
{

	#region Interpolator registry 

	/// <summary>
	/// Central repository for type interpolators. 
	/// </summary>
	public static class Interpolators
	{

		#region Private static variables 

		private static Dictionary<System.Type, object> registry = new Dictionary<System.Type, object>();

		#endregion 

		#region Constructor

		static Interpolators()
		{
			Register<int>( IntInterpolator.Default );
			Register<float>( FloatInterpolator.Default );
			Register<Rect>( RectInterpolator.Default );
			Register<Color>( ColorInterpolator.Default );
			Register<Color32>( Color32Interpolator.Default );
			Register<Vector2>( Vector2Interpolator.Default );
			Register<Vector3>( Vector3Interpolator.Default );
			Register<Vector4>( Vector4Interpolator.Default );
		}

		#endregion 

		#region Public methods

		/// <summary>
		/// Returns an Interpolator&lt;T&gt; for the specified type &lt;T&gt;, if one has 
		/// been registered.
		/// </summary>
		/// <typeparam name="T">The System.Type for which an interpolator is needed</typeparam>
		public static Interpolator<T> Get<T>()
		{
			return (Interpolator<T>)Get( typeof( T ), true );
		}

		/// <summary>
		/// Returns an Interpolator&lt;T&gt; for the specified type &lt;T&gt;, if one has 
		/// been registered.
		/// </summary>
		/// <param name="type">The System.Type for which an interpolator is needed</param>
		/// <param name="throwOnNotFound">Set to TRUE if you want the exception to throw an error rather than returning NULL if the interpolator cannot be found</param>
		public static object Get( System.Type type, bool throwOnNotFound )
		{

			if( type == null )
				throw new System.ArgumentNullException( "You must provide a System.Type value" );

			object result = null;

			if( !registry.TryGetValue( type, out result ) && throwOnNotFound )
			{
				throw new KeyNotFoundException( string.Format( "There is no default interpolator defined for type '{0}'", type.Name ) );
			}

			return result;

		}

		/// <summary>
		/// Register an Interpolator&lt;T&gt; instance that can be used to interpolate values 
		/// of the specified type 
		/// </summary>
		/// <param name="interpolator">An interpolator for the specified type</param>
		/// <typeparam name="T">The System.Type for which the interpolator will be used</typeparam>
		public static void Register<T>( Interpolator<T> interpolator )
		{
			registry[ typeof( T ) ] = interpolator;
		}

		#endregion 

	}

	#endregion 

	#region Interpolator base class

	/// <summary>
	/// Base class for data type interpolators
	/// </summary>
	/// <typeparam name="T">The System.Type that the interpolator will be used for</typeparam>
	public abstract class Interpolator<T>
	{

		#region Public methods

		public abstract T Add( T lhs, T rhs );
		public abstract T Interpolate( T startValue, T endValue, float time );

		#endregion 

	}

	#endregion 

	#region Default interpolators

	/// <summary>
	/// Used to interpolate integer values
	/// </summary>
	public class IntInterpolator : Interpolator<int>
	{
		protected static IntInterpolator singleton;

		public override int Add( int lhs, int rhs )
		{
			return lhs + rhs;
		}
		public override int Interpolate( int startValue, int endValue, float time )
		{
			return Mathf.RoundToInt( startValue + ( endValue - startValue ) * time );
		}

		public static Interpolator<int> Default
		{
			get { if( singleton == null ) singleton = new IntInterpolator(); return singleton; }
		}
	}

	/// <summary>
	/// Used to interpolate float values
	/// </summary>
	public class FloatInterpolator : Interpolator<float>
	{
		protected static FloatInterpolator singleton;

		public override float Add( float lhs, float rhs )
		{
			return lhs + rhs;
		}
		public override float Interpolate( float startValue, float endValue, float time )
		{
			return startValue + ( endValue - startValue ) * time;
		}
		public static Interpolator<float> Default
		{
			get { if( singleton == null ) singleton = new FloatInterpolator(); return singleton; }
		}
	}

	/// <summary>
	/// Used to interpolate Rect values
	/// </summary>
	public class RectInterpolator : Interpolator<Rect>
	{
		protected static RectInterpolator singleton;

		public override Rect Add( Rect lhs, Rect rhs )
		{
			return new Rect( lhs.xMin + rhs.xMin, lhs.yMin + rhs.yMin, lhs.width + rhs.width, lhs.height + rhs.height );
		}
		public override Rect Interpolate( Rect startValue, Rect endValue, float time )
		{
			
			var xMin = startValue.xMin + ( endValue.xMin - startValue.xMin ) * time;
			var yMin = startValue.yMin + ( endValue.yMin - startValue.yMin ) * time;
			var width = startValue.width + ( endValue.width - startValue.width ) * time;
			var height = startValue.height + ( endValue.height - startValue.height ) * time;

			return new Rect( xMin, yMin, width, height );

		}
		public static Interpolator<Rect> Default
		{
			get { if( singleton == null ) singleton = new RectInterpolator(); return singleton; }
		}
	}

	/// <summary>
	/// Used to interpolate Vector2 values
	/// </summary>
	public class Vector2Interpolator : Interpolator<Vector2>
	{
		protected static Vector2Interpolator singleton;

		public override Vector2 Add( Vector2 lhs, Vector2 rhs )
		{
			return lhs + rhs;
		}
		public override Vector2 Interpolate( Vector2 startValue, Vector2 endValue, float time )
		{
			return startValue + ( endValue - startValue ) * time;
		}
		public static Interpolator<Vector2> Default
		{
			get { if( singleton == null ) singleton = new Vector2Interpolator(); return singleton; }
		}
	}

	/// <summary>
	/// Used to interpolate Vector3 values
	/// </summary>
	public class Vector3Interpolator : Interpolator<Vector3>
	{
		protected static Vector3Interpolator singleton;

		public override Vector3 Add( Vector3 lhs, Vector3 rhs )
		{
			return lhs + rhs;
		}
		public override Vector3 Interpolate( Vector3 startValue, Vector3 endValue, float time )
		{
			return startValue + ( endValue - startValue ) * time;
		}
		public static Interpolator<Vector3> Default
		{
			get { if( singleton == null ) singleton = new Vector3Interpolator(); return singleton; }
		}
	}

	/// <summary>
	/// Used to interpolate Vector3 values that represent Euler rotations (yaw, pitch, roll)
	/// </summary>
	public class EulerInterpolator : Interpolator<Vector3>
	{

		protected static EulerInterpolator singleton;

		public static Interpolator<Vector3> Default
		{
			get { if( singleton == null ) singleton = new EulerInterpolator(); return singleton; }
		}
		
		public override Vector3 Add( Vector3 lhs, Vector3 rhs )
		{
			return lhs + rhs;
		}

		public override Vector3 Interpolate( Vector3 startValue, Vector3 endValue, float time )
		{

			float x = clerp( startValue.x, endValue.x, time );
			float y = clerp( startValue.y, endValue.y, time );
			float z = clerp( startValue.z, endValue.z, time );

			return new Vector3( x, y, z );

		}

		#region Private utility functions

		private static float clerp( float start, float end, float time )
		{

			float min = 0.0f;
			float max = 360.0f;
			float half = Mathf.Abs( ( max - min ) / 2.0f );
			float retval = 0.0f;
			float diff = 0.0f;

			if( ( end - start ) < -half )
			{
				diff = ( ( max - start ) + end ) * time;
				retval = start + diff;
			}
			else if( ( end - start ) > half )
			{
				diff = -( ( max - end ) + start ) * time;
				retval = start + diff;
			}
			else
			{
				retval = start + ( end - start ) * time;
			}

			return retval;

		}

		#endregion

	}

	/// <summary>
	/// Used to interpolate Vector4 values
	/// </summary>
	public class Vector4Interpolator : Interpolator<Vector4>
	{

		protected static Vector4Interpolator singleton;

		public override Vector4 Add( Vector4 lhs, Vector4 rhs )
		{
			return lhs + rhs;
		}

		public override Vector4 Interpolate( Vector4 startValue, Vector4 endValue, float time )
		{
			return startValue + ( endValue * time );
		}

		public static Interpolator<Vector4> Default
		{
			get { if( singleton == null ) singleton = new Vector4Interpolator(); return singleton; }
		}

	}

	/// <summary>
	/// Used to interpolate Color values
	/// </summary>
	public class ColorInterpolator : Interpolator<Color>
	{

		protected static ColorInterpolator singleton;

		public override Color Add( Color lhs, Color rhs )
		{
			return lhs + rhs;
		}

		public override Color Interpolate( Color startValue, Color endValue, float time )
		{
			return Color.Lerp( startValue, endValue, time );
		}

		public static Interpolator<Color> Default
		{
			get { if( singleton == null ) singleton = new ColorInterpolator(); return singleton; }
		}

	}

	/// <summary>
	/// Used to interpolate Color values
	/// </summary>
	public class Color32Interpolator : Interpolator<Color32>
	{

		protected static Color32Interpolator singleton;

		public override Color32 Add( Color32 lhs, Color32 rhs )
		{
			return (Color)lhs + rhs;
		}

		public override Color32 Interpolate( Color32 startValue, Color32 endValue, float time )
		{
			return Color.Lerp( startValue, endValue, time );
		}

		public static Interpolator<Color32> Default
		{
			get { if( singleton == null ) singleton = new Color32Interpolator(); return singleton; }
		}

	}

	#endregion 

}
