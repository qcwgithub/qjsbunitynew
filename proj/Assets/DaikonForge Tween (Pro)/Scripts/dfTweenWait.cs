/* Copyright 2014 Daikon Forge */
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Represents a "tween" that waits for some amount of time, which can be useful 
	/// when chained with other tweens or tween groups
	/// </summary>
	public class TweenWait : TweenBase
	{

		#region Object pooling

		private static List<TweenWait> pool = new List<TweenWait>();

		public static TweenWait Obtain( float seconds )
		{

			if( pool.Count > 0 )
			{

				// Obtain an object instance from the pool
				var instance = pool[ pool.Count - 1 ];
				pool.RemoveAt( pool.Count - 1 );

				instance.Delay = seconds;

				return instance;

			}

			return new TweenWait( seconds ) 
			{ 
				AutoCleanup = true 
			};

		}

		public override void Release()
		{

			if( pool.Contains( this ) )
				return;

			Reset();

			pool.Add( this );

		}

		#endregion

		#region Private runtime variables 

		private float elapsed = 0f;

		#endregion 

		#region Constructor

		public TweenWait( float seconds )
		{
			Delay = seconds;
		}

		#endregion

		#region Base class overrides

		public override TweenBase Rewind()
		{
			elapsed = 0;
			return base.Rewind();
		}

		public override TweenBase FastForward()
		{
			elapsed = Delay;
			return base.FastForward();
		}

		public override void Update()
		{

			if( this.State != TweenState.Playing && this.State != TweenState.Started )
				return;

			if( this.State == TweenState.Started )
			{

				elapsed = 0f;
				startTime = getCurrentTime();
				State = TweenState.Playing;

				return;

			}

			elapsed += getDeltaTime();
			if( elapsed >= Delay )
			{
				Stop();
				raiseCompleted();
			}

		}

		#endregion

	}

}
