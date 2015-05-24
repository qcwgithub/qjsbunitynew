/* Copyright 2014 Daikon Forge */
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Provides the ability to assign a specific time at which any number of tweens
	/// should start playing. This allows for precise control over the sequence
	/// of events. TweenTimeline can act as a scheduler for any tween, tween group, or
	/// tween timeline.
	/// </summary>
	public class TweenTimeline : TweenBase, IEnumerable<TweenBase>
	{

		#region Private runtime variables 

		private List<Entry> tweenList = new List<Entry>();
		private List<Entry> pending = new List<Entry>();
		private List<Entry> triggered = new List<Entry>();

		#endregion 

		#region Object pooling

		// Contains the pool of all available Timeline instances. Note that the list is of
		// type List<object> due to limitations of Unity/Mono on Mac/iOS
		private static List<object> pool = new List<object>();

		/// <summary>
		/// Obtain a Timeline instance from the object pool, if one is available. A new 
		/// instance will be returned if there are no instances available in the object 
		/// pool.
		/// </summary>
		public static TweenTimeline Obtain()
		{

			if( pool.Count > 0 )
			{

				var pooledInstance = (TweenTimeline)pool[ pool.Count - 1 ];
				pool.RemoveAt( pool.Count - 1 );

				return pooledInstance;

			}

			return new TweenTimeline();

		}

		/// <summary>
		/// Returns the Tween instance back to the object pool
		/// </summary>
		public override void Release()
		{

			Stop();
			Reset();

			pool.Add( this );

		}

		#endregion

		#region Public methods  

		/// <summary>
		/// Adds a new TweenBase instance to the timeline at the specified time
		/// </summary>
		/// <param name="time">The time at which to play the tween</param>
		/// <param name="tweens">The tween (or tweens) to be played</param>
		public TweenTimeline Add( float time, params TweenBase[] tweens )
		{
			
			for( int i = 0; i < tweens.Length; i++ )
			{

				var tween = tweens[ i ];

				// Although Duration is not used by this class, keep track 
				// of the total duration for informational purposes.
				this.Duration = Mathf.Max( this.Delay + this.Duration, time + tween.Delay + tween.Duration + this.Delay );

				this.tweenList.Add( new Entry() { Time = time, Tween = tween } );

			}

			return this;

		}

		#endregion 

		#region Tween Actions

		/// <summary>
		/// Play the timeline
		/// </summary>
		public override TweenBase Play()
		{

			pending.AddRange( tweenList );
			pending.Sort();

			triggered.Clear();

			// Delay is not directly used. If it was set by the user, then adjust 
			// the start times of each tween to compensate.
			if( Delay > 0 )
			{
				
				for( int i = 0; i < pending.Count; i++ )
				{
					pending[ i ] = new Entry()
					{
						Time = pending[ i ].Time + this.Delay,
						Tween = pending[ i ].Tween
					};
				}

			}

			this.State = TweenState.Playing;
			this.CurrentTime = 0f;
			this.startTime = getCurrentTime();

			registerWithTweenManager();
			raiseStarted();

			return this;

		}

		/// <summary>
		/// Stop the timeline
		/// </summary>
		public override TweenBase Stop()
		{

			if( this.State == TweenState.Stopped )
				return this;

			for( int i = 0; i < tweenList.Count; i++ )
			{
				var item = tweenList[ i ];
				item.Tween.Stop();
			}

			this.pending.Clear();
			this.triggered.Clear();

			return base.Stop();

		}

		public override TweenBase Pause()
		{

			if( this.State != TweenState.Playing && this.State != TweenState.Started )
			{
				return this;
			}

			for( int i = 0; i < triggered.Count; i++ )
			{
				triggered[ i ].Tween.Pause();
			}

			return base.Pause();

		}

		public override TweenBase Resume()
		{

			if( this.State != TweenState.Paused )
			{
				return this;
			}

			for( int i = 0; i < triggered.Count; i++ )
			{
				triggered[ i ].Tween.Resume();
			}

			return base.Resume();

		}

		#endregion 

		#region Property set functions (method-chaining syntax)

		/// <summary>
		/// Sets whether the tween group cleans up children and pools itself when finished
		/// </summary>
		public TweenTimeline SetAutoCleanup( bool autoCleanup )
		{

			this.AutoCleanup = autoCleanup;

			for( int i = 0; i < tweenList.Count; i++ )
			{

				var tween = tweenList[ i ].Tween;

				if( tween is TweenGroup )
					( (TweenGroup)tween ).SetAutoCleanup( autoCleanup );
				else if( tween is TweenTimeline )
					( (TweenTimeline)tween ).SetAutoCleanup( autoCleanup );
				else
					tween.AutoCleanup = autoCleanup;

			}

			return this;

		}

		/// <summary>
		/// Sets whether or not all tweens in this group are dependent on the value of Time.timeScale
		/// for animation speed.
		/// </summary>
		/// <param name="isTimeScaleIndependent"></param>
		/// <returns></returns>
		public override TweenBase SetIsTimeScaleIndependent( bool isTimeScaleIndependent )
		{

			for( int i = 0; i < tweenList.Count; i++ )
			{
				var tween = tweenList[ i ].Tween;
				tween.SetIsTimeScaleIndependent( isTimeScaleIndependent );
			}

			return base.SetIsTimeScaleIndependent( isTimeScaleIndependent );

		}

		/// <summary>
		/// Set the type of looping (if any) to be used. This function can accept the following
		/// values: TweenLoopType.None, TweenLoopType.Loop
		/// </summary>
		public TweenTimeline SetLoopType( TweenLoopType loopType )
		{

			if( loopType != TweenLoopType.None && loopType != TweenLoopType.Loop )
				throw new System.ArgumentException( "LoopType may only be one of the following values: TweenLoopType.None, TweenLoopType.Loop" );

			this.LoopType = loopType;

			return this;

		}

		/// <summary>
		/// Set how many times the tween loops before stopping. If set to 0, the tween
		/// will loop forever.
		/// </summary>
		public TweenTimeline SetLoopCount( int loopCount )
		{
			this.LoopCount = loopCount;
			return this;
		}

		#endregion

		#region Other base class overrides

		internal override float CalculateTotalDuration()
		{

			var total = 0f;

			for( int i = 0; i < tweenList.Count; i++ )
			{

				var item = tweenList[ i ];
				if( item.Tween == null )
					continue;

				total = Mathf.Max( total, item.Time + item.Tween.CalculateTotalDuration() );

			}

			if( this.LoopCount > 0 )
				total *= LoopCount;
			else if( this.LoopType != TweenLoopType.None )
				total = float.PositiveInfinity;

			return this.Delay + total;

		}

		protected override void Reset()
		{
			
			tweenList.Clear();
			pending.Clear();
			triggered.Clear();

			base.Reset();

		}

		public override void Update()
		{

			if( this.State != TweenState.Started && this.State != TweenState.Playing )
				return;

			var elapsed = getCurrentTime() - startTime;

			while( pending.Count > 0 )
			{

				var item = pending[ 0 ];

				// Because the pending list is sorted by time, we can stop looking 
				// for tweens to trigger when we encounter one whose start time is
				// in the future, since all others will also be in the future.
				if( item.Time > elapsed )
					break;

				// Move the tween from the pending list to the active list
				pending.RemoveAt( 0 );
				triggered.Add( item );

				// Start the tween
				item.Tween.Play();

			}

			// If there are no more pending tweens, then either loop or notify any 
			// observers that the timeline is completed.
			if( allTweensComplete() )
			{

				if( LoopType == TweenLoopType.Loop && --LoopCount != 0 )
				{

					raiseLoopCompleted();

					Rewind();
					Play();

				}
				else
				{

					Stop();
					raiseCompleted();

					if( AutoCleanup )
					{
						Release();
					}

				}

				return;

			}

		}

		/// <summary>
		/// Returns true when all child tweens have finished
		/// </summary>
		private bool allTweensComplete()
		{

			if( pending.Count > 0 )
				return false;

			for( int i = 0; i < triggered.Count; i++ )
			{
				if( triggered[ i ].Tween.State != TweenState.Stopped )
					return false;
			}

			return true;

		}

		#endregion

		#region IEnumerable<TweenBase> Members

		public IEnumerator<TweenBase> GetEnumerator()
		{
			return enumerateTweens();
		}

		private IEnumerator<TweenBase> enumerateTweens()
		{

			var index = 0;
			while( index < tweenList.Count )
			{
				yield return tweenList[ index++ ].Tween;
			}

		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return enumerateTweens();
		}

		#endregion

		#region Nested classes

		private struct Entry : IComparable<Entry>
		{

			public float Time;
			public TweenBase Tween;

			#region IComparable<Event> Members

			public int CompareTo( Entry other )
			{
				return this.Time.CompareTo( other.Time );
			}

			#endregion

		}

		#endregion 
	
	}

}
