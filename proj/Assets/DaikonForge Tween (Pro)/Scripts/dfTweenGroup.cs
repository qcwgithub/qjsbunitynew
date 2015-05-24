/* Copyright 2014 Daikon Forge */
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DaikonForge.Tween
{

	/// <summary>
	/// A group of tweens which can play concurrently or sequentially
	/// </summary>
	public class TweenGroup : TweenBase, IEnumerable<TweenBase>
	{

		#region Object pooling 

		private static List<TweenGroup> pool = new List<TweenGroup>();

		/// <summary>
		/// Returns a TweenGroup instance from the object pool, if one is available, or 
		/// returns a new instance if there were none in the object pool
		/// </summary>
		public static TweenGroup Obtain()
		{

			if( pool.Count > 0 )
			{

				var instance = pool[ pool.Count - 1 ];
				pool.RemoveAt( pool.Count - 1 );

				return instance;

			}

			return new TweenGroup();

		}

		/// <summary>
		/// Returns the TweenGroup instance to the object pool
		/// </summary>
		public override void Release()
		{

			this.Stop();

			if( !pool.Contains( this ) )
			{
				Reset();
				pool.Add( this );
			}

		}

		#endregion

		#region Public fields 

		/// <summary>
		/// Gets or sets whether the tweens will be played sequentially or concurrently
		/// </summary>
		public TweenGroupMode Mode = TweenGroupMode.Sequential;

		#endregion

		#region Protected runtime variables 

		protected List<TweenBase> tweenList = new List<TweenBase>();

		protected TweenBase currentTween = null;
		protected int currentIndex = 0;

		#endregion

		#region Property set functions (method-chaining syntax)

		/// <summary>
		/// Sets whether the tween group cleans up children and pools itself when finished
		/// </summary>
		public TweenGroup SetAutoCleanup( bool autoCleanup )
		{

			this.AutoCleanup = autoCleanup;

			for( int i = 0; i < tweenList.Count; i++ )
			{

				var tween = tweenList[ i ];

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
				tweenList[ i ].SetIsTimeScaleIndependent( isTimeScaleIndependent );
			}
			return base.SetIsTimeScaleIndependent( isTimeScaleIndependent );
		}

		/// <summary>
		/// Set the sequence type of this tween group
		/// </summary>
		public TweenGroup SetMode( TweenGroupMode mode )
		{
			this.Mode = mode;
			return this;
		}

		/// <summary>
		/// Set the delay period for this group to pause before executing
		/// </summary>
		/// <param name="seconds">The number of seconds to delay</param>
		public TweenGroup SetDelay( float seconds )
		{
			this.Delay = seconds;
			return this;
		}

		/// <summary>
		/// Set the type of looping (if any) to be used. This function can accept the following
		/// values: TweenLoopType.None, TweenLoopType.Loop
		/// </summary>
		public TweenGroup SetLoopType( TweenLoopType loopType )
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
		public TweenGroup SetLoopCount( int loopCount )
		{
			this.LoopCount = loopCount;
			return this;
		}

		#endregion 

		#region Public methods

		/// <summary>
		/// Append any number of TweenBase instances to the group
		/// </summary>
		public TweenGroup AppendTween( params TweenBase[] tweens )
		{

			if( tweens == null || tweens.Length == 0 )
				throw new System.ArgumentException( "You must provide at least one Tween" );

			tweenList.AddRange( tweens );

			return this;

		}

		/// <summary>
		/// Append a delay. NOTE: This will only work as expected when the TweenGroup instance
		/// is operating in Sequential mode. If you wish to have a delay fire after a group of 
		/// tweens has finished playing in Consecutive mode, then you should use the Chain() method.
		/// </summary>
		public TweenGroup AppendDelay( float seconds )
		{
			tweenList.Add( TweenWait.Obtain( seconds ) );
			return this;
		}

		/// <summary>
		/// Removes all TweenBase instances from the list of tweens in this group
		/// </summary>
		public TweenGroup ClearTweens()
		{
			tweenList.Clear();
			return this;
		}

		/// <summary>
		/// Play the tween group
		/// </summary>
		public override TweenBase Play()
		{

			if( LoopType != TweenLoopType.None && LoopType != TweenLoopType.Loop )
				throw new System.ArgumentException( "LoopType may only be one of the following values: TweenLoopType.None, TweenLoopType.Loop" );

			currentTween = null;
			currentIndex = -1;

			base.Play();

			return this;

		}

		/// <summary>
		/// Stop the tween group
		/// </summary>
		public override TweenBase Stop()
		{

			if( State != TweenState.Stopped )
			{

				for( int i = 0; i < tweenList.Count; i++ )
				{
					tweenList[ i ].Stop();
				}

				currentTween = null;
				currentIndex = -1;

			}

			return base.Stop();

		}

		/// <summary>
		/// Pause the tween group
		/// </summary>
		public override TweenBase Pause()
		{

			if( State == TweenState.Playing || State == TweenState.Started )
			{

				if( Mode == TweenGroupMode.Concurrent )
				{
					pauseAllTweens();
				}
				else
				{
					if( currentTween != null )
					{
						currentTween.Pause();
					}
				}

			}

			return base.Pause();

		}

		/// <summary>
		/// Resume the tween group
		/// </summary>
		public override TweenBase Resume()
		{

			if( State == TweenState.Paused )
			{

				if( Mode == TweenGroupMode.Concurrent )
				{
					resumeAllTweens();
				}
				else
				{
					if( currentTween != null )
					{
						currentTween.Resume();
					}
				}

			}

			base.Resume();

			return this;

		}

		public override TweenBase Rewind()
		{

			for( int i = 0; i < tweenList.Count; i++ )
			{
				tweenList[ i ].Rewind();
			}

			currentTween = null;
			currentIndex = -1;

			return base.Rewind();

		}

		/// <summary>
		/// Called internally by the tween manager. There is no need to call this function directly.
		/// </summary>
		// @private
		public override void Update()
		{

			if( tweenList.Count == 0 )
				return;

			#region Implement Start Delay

			if( this.State == TweenState.Started )
			{

				var time = getCurrentTime();

				if( time < startTime + Delay )
				{
					return;
				}

				if( Mode == TweenGroupMode.Concurrent )
				{
					startAllTweens();
				}
				else
				{
					if( !nextTween() )
						return;
				}

				this.startTime = time;
				this.CurrentTime = 0f;
				this.State = TweenState.Playing;

			}
			else if( this.State != TweenState.Playing )
			{
				return;
			}

			#endregion

			#region Concurrent mode 

			if( Mode == TweenGroupMode.Concurrent )
			{

				if( allTweensComplete() )
				{

					if( LoopType == TweenLoopType.Loop && --LoopCount != 0 )
					{

						raiseLoopCompleted();

						// The OnLoopCompleted event handler may have chosen to change the 
						// state of the tween. If so, do not override
						if( this.State == TweenState.Playing )
						{
							Rewind();
							Play();
						}

					}
					else
					{
						onGroupComplete();
					}

				}

				return;

			}

			#endregion 

			if( currentTween.State == TweenState.Stopped )
			{

				var reachedEnd = !nextTween();
				if( reachedEnd )
				{

					Stop();
					raiseCompleted();

					return;

				}

			}

		}

		#endregion

		#region Base class overrides

		protected override void Reset()
		{

			Stop();

			if( AutoCleanup )
			{
				cleanUp();
			}

			base.Reset();

			this.Mode = TweenGroupMode.Sequential;
			this.AutoCleanup = false;

			this.tweenList.Clear();

		}

		internal override float CalculateTotalDuration()
		{

			var total = 0f;

			if( Mode == TweenGroupMode.Sequential )
			{

				for( int i = 0; i < tweenList.Count; i++ )
				{

					var tween = tweenList[ i ];
					if( tween == null )
						continue;

					total += tween.CalculateTotalDuration();

				}

			}
			else
			{

				for( int i = 0; i < tweenList.Count; i++ )
				{

					var tween = tweenList[ i ];
					if( tween == null )
						continue;

					total = Mathf.Max( total, tween.CalculateTotalDuration() );

				}

			}

			if( this.LoopCount > 0 )
				total *= LoopCount;
			else if( this.LoopType != TweenLoopType.None )
				total = float.PositiveInfinity;

			return this.Delay + total;

		}

		#endregion

		#region Private utility functions

		private void onGroupComplete()
		{

			Stop();
			raiseCompleted();

			if( AutoCleanup )
			{
				cleanUp();
			}

		}

		private void startAllTweens()
		{

			for( int i = 0; i < tweenList.Count; i++ )
			{
				var tween = tweenList[ i ];
				if( tween != null )
				{
					tween.Play();
				}
			}

		}

		private void pauseAllTweens()
		{

			for( int i = 0; i < tweenList.Count; i++ )
			{
				var tween = tweenList[ i ];
				if( tween != null )
				{
					tween.Pause();
				}
			}

		}

		private void resumeAllTweens()
		{

			for( int i = 0; i < tweenList.Count; i++ )
			{
				var tween = tweenList[ i ];
				if( tween != null )
				{
					tween.Resume();
				}
			}

		}

		private bool nextTween()
		{

			if( this.Mode == TweenGroupMode.Concurrent )
				return true;

			if( this.State == TweenState.Started )
			{

				currentIndex = 0;
				currentTween = tweenList[ currentIndex ];
				currentTween.Play();

				return true;

			}

			if( currentIndex == tweenList.Count - 1 )
			{
				if( LoopType == TweenLoopType.Loop && --LoopCount != 0 )
				{

					raiseLoopCompleted();

					currentIndex = 0;

					// The OnLoopCompleted event handler may have chosen to change the 
					// state of the tween. If so, do not override
					if( State == TweenState.Stopped )
						return false;

				}
				else
				{
					return false;
				}
			}
			else
			{
				currentIndex += 1;
			}

			currentTween = tweenList[ currentIndex ];
			currentTween.Play();

			return true;

		}

		/// <summary>
		/// Returns true when all child tweens have finished
		/// </summary>
		private bool allTweensComplete()
		{

			if( Mode == TweenGroupMode.Sequential && currentTween != null )
				return currentTween.State == TweenState.Stopped;

			for( int i = 0; i < tweenList.Count; i++ )
			{
				if( tweenList[ i ].State != TweenState.Stopped )
					return false;
			}

			return true;

		}

		private void cleanUp()
		{

			for( int i = 0; i < tweenList.Count; )
			{

				var tween = tweenList[ i ];
				if( tween != null && tween.AutoCleanup )
				{

					if( tween is IPoolableObject )
					{
						( (IPoolableObject)tween ).Release();
					}

					tweenList.RemoveAt( i );

				}
				else
				{
					i += 1;
				}

			}

		}

		#endregion

		#region IEnumerable<TweenBase> Members

		public IEnumerator<TweenBase> GetEnumerator()
		{
			return tweenList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return tweenList.GetEnumerator();
		}

		#endregion

	}

}
