/* Copyright 2014 Daikon Forge */
using UnityEngine;

#if !FREE_VERSION

namespace DaikonForge.Tween
{

	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using DaikonForge.Editor;

	/// <summary>
	/// A spline object which can be used to calculate smooth paths
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu( "Daikon Forge/Tween/Spline Path" )]
	[InspectorGroupOrder( "General", "Path", "Control Points" )]
	public class SplineObject : MonoBehaviour
	{

		#region Public fields 

		/// <summary>
		/// The internal spline curve
		/// </summary>
		public Spline Spline;

		/// <summary>
		/// If set to TRUE, the spline will "wrap" at the ends
		/// </summary>
		[Inspector( "Path", Order = 0, Tooltip = "If set to TRUE, the end of the path will wrap around to the beginning" )]
		public bool Wrap = false;

		/// <summary>
		/// List of spline points
		/// </summary>
		[Inspector( "Control Points", Order = 1, Tooltip = "Contains the list of Transforms that represent the control points of the path's curve" )]
		public List<Transform> ControlPoints = new List<Transform>();

		#endregion 

		#region Monobehaviour events 

		public void Awake()
		{

			if( ControlPoints.Count == 0 )
			{
				var nodes = transform.GetComponentsInChildren<SplineNode>();
				ControlPoints.AddRange( nodes.Select( x => x.transform ).ToList() );
			}

			CalculateSpline();

		}

		#endregion 

		#region Public methods

		/// <summary>
		/// Return a point along the spline at the given normalized length (0-1)
		/// </summary>
		public Vector3 Evaluate( float time )
		{
			return Spline.GetPosition( time );
		}

		/// <summary>
		/// Returns the time (normalized to between 0 and 1, inclusive) along the spline that
		/// corresponds to the the control point at the indicated index
		/// </summary>
		/// <param name="nodeIndex">The index of one of the spline's control points</param>
		public float GetTimeAtNode( int nodeIndex )
		{

			CalculateSpline();

			var node = Spline.ControlPoints[ nodeIndex ];
			return node.Time;

		}

		/// <summary>
		/// Adds a new SplineNode to the list of control points and returns a reference to the new instance
		/// </summary>
		public SplineNode AddNode()
		{

			CalculateSpline();

			var newPosition = this.transform.position;
			
			if( Spline.ControlPoints.Count > 2 )
			{
				var lastPosition = Spline.ControlPoints.Last().Position;
				var direction = Spline.GetTangent( 0.95f );
				newPosition = lastPosition + direction;
			}

			return AddNode( newPosition );

		}

		/// <summary>
		/// Adds a new SplineNode to the list of control points and returns a reference to the new instance
		/// </summary>
		public SplineNode AddNode( Vector3 position )
		{

			var go = new GameObject() { name = "SplineNode" + Spline.ControlPoints.Count };
			var component = go.AddComponent<SplineNode>();

			go.transform.parent = this.transform;
			go.transform.position = position;

			ControlPoints.Add( go.transform );

			CalculateSpline();

			return component;

		}

		/// <summary>
		/// Recalculates the spline data from the lsit of control points
		/// </summary>
		public void CalculateSpline()
		{

			var index = 0;
			while( index < ControlPoints.Count )
			{

				if( ControlPoints[ index ] == null )
					ControlPoints.RemoveAt( index );
				else
					index += 1;

			}

			if( Spline == null )
			{
				Spline = new Spline();
			}

			Spline.Wrap = this.Wrap;

			Spline.ControlPoints.Clear();

			for( int i = 0; i < ControlPoints.Count; i++ )
			{
				
				var node = ControlPoints[ i ];
				if( node == null )
					continue;

				Spline.ControlPoints.Add( new Spline.ControlPoint( node.position, node.forward ) );

			}

			Spline.ComputeSpline();

		}

		/// <summary>
		/// Returns an axis-aligned bounding box containing all of the spline's control points
		/// </summary>
		public Bounds GetBounds()
		{

			var min = Vector3.one * float.MaxValue;
			var max = Vector3.one * float.MinValue;

			var validNodeCount = 0;

			for( int i = 0; i < ControlPoints.Count; i++ )
			{

				if( ControlPoints[ i ] == null )
					continue;

				validNodeCount += 1;

				var pos = ControlPoints[ i ].transform.position;
				min = Vector3.Min( min, pos );
				max = Vector3.Max( max, pos );

			}

			if( validNodeCount == 0 )
			{
				return new Bounds( this.transform.position, Vector3.zero );
			}

			var size = ( max - min );
			return new Bounds( min + size * 0.5f, size );

		}

		#endregion 

#if UNITY_EDITOR

		public void OnDrawGizmos()
		{

			CalculateSpline();

			var splinePoints = Spline.ControlPoints;

			if( splinePoints == null || splinePoints.Count < 2 )
				return;

			var savedColor = Gizmos.color;

			#region Render regular intervals along the spline

			//Gizmos.color = Color.green;

			//var newNodeCount = Spline.Length * 6;
			//var dist = 1f / newNodeCount;

			//for( int i = 0; i < newNodeCount; i++ )
			//{
			//	var time = Spline.AdjustTimeToConstant( i * dist );
			//	var point = Evaluate( time );
			//	Gizmos.DrawSphere( point, 0.1f );
			//}

			#endregion

			Gizmos.color = Color.white;

			var increments = 16;
			var count = splinePoints.Count + ( Spline.Wrap ? 0 : -1 );
			var segmentTime = 1f / count;
			float step = segmentTime / increments;

			var last = splinePoints[ 0 ].Position;
			for( int idx = 0; idx < count; idx++ )
			{

				var baseTime = idx * segmentTime;

				for( int i = 0; i < increments; i++ )
				{

					var point = Evaluate( baseTime + i * step );

					//Gizmos.DrawWireCube( point, Vector3.one * 0.05f );

					Gizmos.DrawLine( last, point );
					last = point;

				}

				//Gizmos.color = Color.red;
				//Gizmos.DrawSphere( splinePoints[ idx ].Position, 0.2f );
				Gizmos.color = Color.white;

			}

			var final = splinePoints[ Spline.Wrap ? 0 : splinePoints.Count - 1 ].Position;
			Gizmos.DrawLine( last, final );

			#region Draw indicator at center of spline 

			var center = GetBounds().center;

			Gizmos.color = Color.yellow;
			var size = 0.5f;
			Gizmos.DrawLine( center + Vector3.left * size, center + Vector3.right * size );
			Gizmos.DrawLine( center + Vector3.forward * size, center + Vector3.back * size );
			Gizmos.DrawLine( center + Vector3.up * size, center + Vector3.down * size );

			#endregion 

			Gizmos.color = savedColor;

		}

#endif

	}

}

#endif
