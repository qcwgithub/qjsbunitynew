/* Copyright 2014 Daikon Forge */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !FREE_VERSION

namespace DaikonForge.Tween
{

	/// <summary>
	/// Represents a spline curve
	/// </summary>
	public class Spline : IPathIterator, IEnumerable<Spline.ControlPoint>
	{

		/// <summary>
		/// Method used to interpolate control points
		/// </summary>
		public ISplineInterpolator SplineMethod = new CatmullRomSpline();

		/// <summary>
		/// List of spline control points
		/// </summary>
		public List<ControlPoint> ControlPoints = new List<ControlPoint>();

		/// <summary>
		/// Returns the estimated total length of the spline
		/// </summary>
		public float Length
		{
			get { return this.length; }
		}

		/// <summary>
		/// Whether spline wraps around from the end back to the beginning
		/// </summary>
		public bool Wrap = false;

		#region Private runtime variables 

		private float length;

		#endregion 

		/// <summary>
		/// Return a point along the spline at the given normalized length (0-1)
		/// </summary>
		/// <param name="time">The time (normalized to between 0 and 1) along the spline for which to return the position</param>
		public Vector3 GetPosition( float time )
		{

			// Adjust time to be within 0-1
			time = Mathf.Abs( time ) % 1f;

			float seg = 1f / (float)ControlPoints.Count;

			if( !Wrap )
			{
				time *= ( seg * ( ControlPoints.Count - 1 ) );
			}

			int idx = Mathf.FloorToInt( time * ControlPoints.Count );

			ControlPoint a = getNode( idx - 1 );
			ControlPoint b = getNode( idx );
			ControlPoint c = getNode( idx + 1 );
			ControlPoint d = getNode( idx + 2 );

			float tmin = seg * idx;

			time -= tmin;
			time /= seg;

			return SplineMethod.Evaluate( a.Position, b.Position, c.Position, d.Position, time );

		}

		/// <summary>
		/// Adjusts the given time value (normalized from 0 to 1) to return a value which, when 
		/// used to iterate the spline, will result in movement at a roughly constant speed even 
		/// if the spline's control points are not evenly spaced.
		/// </summary>
		/// <param name="time">The time (normalized to between 0 and 1) along the spline for which to return the adjusted value</param>
		public float AdjustTimeToConstant( float time )
		{

			if( time < 0 || time > 1 )
				throw new System.ArgumentException( "The length parameter must be a value between 0 and 1 (inclusive)" );

			var containingSegmentIndex = getParameterIndex( time );
			var segmentCount = ControlPoints.Count + ( this.Wrap ? 0 : -1 );
			var segmentLength = ( 1f / segmentCount );
			var segmentNode = ControlPoints[ containingSegmentIndex ];
			var containingSegmentLength = segmentNode.Length / this.length;
			var lerp = ( time - segmentNode.Time ) / containingSegmentLength;
			var adjustedTime = containingSegmentIndex * segmentLength + lerp * segmentLength;

			return adjustedTime;

		}

		/// <summary>
		/// Can be used to interpolate between the orientations of the spline's control points
		/// at any specific time.
		/// </summary>
		/// <param name="time">The time (normalized to between 0 and 1) along the spline for which to return the orientation</param>
		public Vector3 GetOrientation( float time )
		{

			var index = ControlPoints.Count - ( Wrap ? 1 : 2 );
			while( ControlPoints[ index ].Time > time )
				index -= 1;

			var nextIndex = ( index == ControlPoints.Count - 1 ) ? 0 : index + 1;

			var segmentNode = ControlPoints[ index ];
			var nextNode = ControlPoints[ nextIndex ];

			var containingSegmentLength = segmentNode.Length / this.length;
			var lerp = ( time - segmentNode.Time ) / containingSegmentLength;

			return Vector3.Lerp( segmentNode.Orientation, nextNode.Orientation, lerp );

		}

		/// <summary>
		/// Gets the forward vector at a particular normalized time along the spline
		/// </summary>
		/// <param name="time">The normalized time along the spline to obtain the forward vector for</param>
		public Vector3 GetTangent( float time )
		{
			return GetTangent( time, 0.01f );
		}

		/// <summary>
		/// Gets the forward vector at a particular normalized time along the spline
		/// </summary>
		/// <param name="time">The normalized time along the spline to obtain the forward vector for</param>
		/// <param name="lookAhead">The distance (normalized) beyond t to look ahead to obtain the forward vector</param>
		public Vector3 GetTangent( float time, float lookAhead )
		{

			if( !Wrap && time > 1f - lookAhead )
				time = 1f - lookAhead;

			Vector3 t0 = GetPosition( time );
			Vector3 t1 = GetPosition( time + lookAhead );
			Vector3 dir = t1 - t0;

			dir.Normalize();

			return dir;

		}

		/// <summary>
		/// Add a new node to the spline
		/// </summary>
		public Spline AddNode( ControlPoint node )
		{
			this.ControlPoints.Add( node );
			return this;
		}

		/// <summary>
		/// Called after adding all control points, this function will calculate the estimated
		/// length of the spline, as well as the estimated length of each segment within the 
		/// spline. Each element in the ControlPoints collection will have its Length property 
		/// assigned (this is necessary if you wish to use AdjustTimeToConstant() to implement
		/// constant-time iteration of the spline).
		/// </summary>
		public void ComputeSpline()
		{

			this.length = 0;
			for( int i = 0; i < ControlPoints.Count; i++ )
			{
				ControlPoints[ i ].Time = 0;
				ControlPoints[ i ].Length = 0;
			}

			if( ControlPoints.Count < 2 )
				return;

			var increments = 16;
			var count = ControlPoints.Count + ( this.Wrap ? 0 : -1 );
			var segmentTime = 1f / count;
			float step = segmentTime / increments;

			for( int idx = 0; idx < count; idx++ )
			{

				var baseTime = idx * segmentTime;
				var last = ControlPoints[ idx ].Position;

				for( int i = 1; i < increments; i++ )
				{

					var point = GetPosition( baseTime + i * step );

					ControlPoints[ idx ].Length += Vector3.Distance( last, point );

					last = point;

				}

			}

			this.length = 0;
			for( int i = 0; i < ControlPoints.Count; i++ )
			{
				this.length += ControlPoints[ i ].Length;
			}

			var accum = 0f;
			for( int i = 0; i < count; i++ )
			{
				ControlPoints[ i ].Time = accum / this.length;
				accum += ControlPoints[ i ].Length;
			}

			if( !Wrap )
			{
				ControlPoints[ ControlPoints.Count - 1 ].Time = 1f;
			}

		}

		#region Private utility functions 

		private int getParameterIndex( float time )
		{

			var index = 0;
			for( int i = 1; i < ControlPoints.Count; i++ )
			{

				var testNode = ControlPoints[ i ];
				if( testNode.Length == 0 || testNode.Time > time )
					break;

				index = i;

			}

			return index;

		}

		private ControlPoint getNode( int nodeIndex )
		{

			if( Wrap )
			{
				if( nodeIndex < 0 )
					nodeIndex += ControlPoints.Count;
				if( nodeIndex >= ControlPoints.Count )
					nodeIndex -= ControlPoints.Count;
			}
			else
			{
				nodeIndex = Mathf.Clamp( nodeIndex, 0, ControlPoints.Count - 1 );
			}

			return ControlPoints[ nodeIndex ];

		}

		#endregion 

		#region Nested classes 

		/// <summary>
		/// Represents a control point in a spline
		/// </summary>
		public class ControlPoint
		{

			public Vector3 Position;
			public Vector3 Orientation;
			public float Length;
			public float Time;

			public ControlPoint( Vector3 Position, Vector3 Tangent )
			{
              
				this.Position = Position;
				this.Orientation = Tangent;
				this.Length = 0;
			}

		}

		#endregion 

		#region IEnumerable<ControlPoint> Members

		private IEnumerator<Spline.ControlPoint> GetCustomEnumerator()
		{

			int index = 0;
			while( index < ControlPoints.Count )
			{
				yield return ControlPoints[ index++ ];
			}

		}

		public IEnumerator<Spline.ControlPoint> GetEnumerator()
		{
			return GetCustomEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetCustomEnumerator();
		}

		#endregion
	}

}

#endif
