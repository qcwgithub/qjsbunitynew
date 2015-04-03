/* Copyright 2014 Daikon Forge */
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Describes the minimum interface that any tween or tween-like object
	/// must implement.
	/// </summary>
	public abstract class TweenBase : ITweenUpdatable, IPoolableObject
	{

		#region Public fields

		/// <summary>
		/// The name of the tween (optional)
		/// </summary>
		public string Name;

		/// <summary>
		/// The current normalized (0-1) time of the tween
		/// </summary>
		public float CurrentTime;

		/// <summary>
		/// Returns the real time (in seconds) since the tween was started
		/// </summary>
		public float ElapsedTime
		{
			get { return getCurrentTime() - this.startTime; }
		}

		/// <summary>
		/// The duration of the tween in seconds
		/// </summary>
		public float Duration;

		/// <summary>
		/// The amount of time (in seconds) that the tween will delay after Play() has been called
		/// </summary>
		public float Delay;

		/// <summary>
		/// Tween loop behavior
		/// </summary>
		public TweenLoopType LoopType = TweenLoopType.None;

		/// <summary>
		/// If the LoopType property is any value other than TweenLoopType.None, this property
		/// controls how many times the tween will loop. If this property is set to 0, the 
		/// tween will loop forever.
		/// </summary>
		public int LoopCount = 0;

		/// <summary>
		/// A function which provides easing
		/// </summary>
		public TweenEasingCallback Easing;

		/// <summary>
		/// Indicates whether tween will be automatically released to the object pool when it is finished playing
		/// </summary>
		public bool AutoCleanup;

		/// <summary>
		/// Indicates whether the tween will operate independant of the Time.timeScale value.
		/// A Tween that is time scale-independant will not be affected by the Time.timeScale
		/// setting and will always run at a constant speed.
		/// </summary>
		public bool IsTimeScaleIndependent;

		/// <summary>
		/// Returns the current state of the object
		/// </summary>
		public TweenState State { get; protected set; }

		#endregion

		#region Public Callbacks

		/// <summary>
		/// Raised when the tween animation has started playing 
		/// </summary>
		public TweenCallback TweenStarted;

		/// <summary>
		/// Raised when the tween animation has stopped playing before completion
		/// </summary>
		public TweenCallback TweenStopped;

		/// <summary>
		/// Raised when the tween animation has been paused
		/// </summary>
		public TweenCallback TweenPaused;

		/// <summary>
		/// Raised when the tween animation has been resumed after having been paused
		/// </summary>
		public TweenCallback TweenResumed;

		/// <summary>
		/// Raised when the tween animation has successfully completed
		/// </summary>
		public TweenCallback TweenCompleted;

		/// <summary>
		/// Raised when the tween animation has successfully completed a loop
		/// </summary>
		public TweenCallback TweenLoopCompleted;

		#endregion

		#region Protected runtime variables

		protected float startTime;
		protected bool registered = false;

		#endregion

		#region Tween Actions

		/// <summary>
		/// Play the tween
		/// </summary>
		public virtual TweenBase Play()
		{

			this.State = TweenState.Started;
			this.CurrentTime = 0f;
			this.startTime = getCurrentTime();

			registerWithTweenManager();
			raiseStarted();

			return this;

		}

		/// <summary>
		/// Pause the tween, if it is playing
		/// </summary>
		public virtual TweenBase Pause()
		{

			if( this.State != TweenState.Playing && this.State != TweenState.Started )
			{
				return this;
			}

			this.State = TweenState.Paused;
			raisePaused();

			return this;

		}

		/// <summary>
		/// Resumes a paused tween
		/// </summary>
		public virtual TweenBase Resume()
		{

			if( this.State != TweenState.Paused )
				return this;

			this.State = TweenState.Playing;
			raiseResumed();

			return this;

		}

		/// <summary>
		/// Stop the tween
		/// </summary>
		public virtual TweenBase Stop()
		{

			if( this.State == TweenState.Stopped )
				return this;

			unregisterWithTweenManager();

			this.State = TweenState.Stopped;
			raiseStopped();

			return this;

		}

		/// <summary>
		/// Rewind the tween to the beginning
		/// </summary>
		public virtual TweenBase Rewind()
		{

			this.CurrentTime = 0f;
			this.startTime = getCurrentTime();

			return this;

		}

		/// <summary>
		/// Fast forward the tween to the end
		/// </summary>
		public virtual TweenBase FastForward()
		{

			this.CurrentTime = 1f;

			return this;

		}

		/// <summary>
		/// Can be used from within a coroutine to wait until the tween has completed
		/// </summary>
		public virtual IEnumerator WaitForCompletion()
		{

			do
			{
				yield return null;
			}
			while( State != TweenState.Stopped );

		}

		/// <summary>
		/// Chain a tween to be played when this one is done
		/// </summary>
		public virtual TweenBase Chain( TweenBase tween )
		{
			return Chain( tween, null );
		}

		/// <summary>
		/// Chain a function to be called and tween to be played when this one is done
		/// </summary>
		public virtual TweenBase Chain( TweenBase tween, System.Action initFunction )
		{

			if( tween == null )
				throw new System.ArgumentNullException( "tween" );

			var completedCallback = this.TweenCompleted;
			this.TweenCompleted = ( sender ) =>
			{

				if( completedCallback != null )
				{
					completedCallback( sender );
				}

				if( initFunction != null )
				{
					initFunction();
				}

				tween.Play();

			};

			return tween;

		}

		/// <summary>
		/// Calculates the length of the animation. Used by tween containers for which the 
		/// duration of an animation is always the sum of the duration of child animations.
		/// </summary>
		/// <returns></returns>
		internal virtual float CalculateTotalDuration()
		{
			
			var total = this.Delay + this.Duration;

			if( this.LoopCount > 0 )
				total *= LoopCount;
			else if( this.LoopType != TweenLoopType.None )
				total = float.PositiveInfinity;

			return total;

		}

		#endregion

		#region Property setters 

		/// <summary>
		/// Sets whether or not animation speed is dependent upon the value of Time.timeScale
		/// </summary>
		public virtual TweenBase SetIsTimeScaleIndependent( bool isTimeScaleIndependent )
		{
			this.IsTimeScaleIndependent = isTimeScaleIndependent;
			return this;
		}

		#endregion 

		#region Pure virtual functions

		/// <summary>
		/// Called by the tween engine to cause the tween to update
		/// </summary>
		public abstract void Update();

		#endregion

		#region Protected functions

		/// <summary>
		/// Resets the tween's values to their defaults so that the tween may be re-used
		/// </summary>
		protected virtual void Reset()
		{

			// Reset all fields to default values 
			this.Easing = TweenEasingFunctions.Linear;
			this.LoopType = TweenLoopType.None;
			this.CurrentTime = 0f;
			this.Delay = 0;
			this.AutoCleanup = false;
			this.IsTimeScaleIndependent = false;
			this.startTime = 0;

			// Clear all event handlers
			this.TweenLoopCompleted = null;
			this.TweenCompleted = null;
			this.TweenPaused = null;
			this.TweenResumed = null;
			this.TweenStarted = null;
			this.TweenStopped = null;

		}

		/// <summary>
		/// Registers this tween with the active TweenManager instance
		/// </summary>
		protected void registerWithTweenManager()
		{
			if( !registered )
			{
				TweenManager.Instance.RegisterTween( this );
				registered = true;
			}

		}

		/// <summary>
		/// Unregisters this tween with the active TweenManager instance
		/// </summary>
		protected void unregisterWithTweenManager()
		{
			if( registered )
			{
				TweenManager.Instance.UnregisterTween( this );
				registered = false;
			}

		}

		/// <summary>
		/// Returns the elapsed time since the tween was started
		/// </summary>
		protected float getTimeElapsed()
		{

			if( this.State == TweenState.Playing || this.State == TweenState.Started )
			{
				var elapsed = Mathf.Min( getCurrentTime() - startTime, Duration );
				return elapsed;
			}

			return 0;

		}

		/// <summary>
		/// Returns the current time, based on whether the TimeScaleIndependent property
		/// has been set.
		/// </summary>
		protected float getCurrentTime()
		{
			if( IsTimeScaleIndependent )
				return TweenManager.Instance.realTimeSinceStartup;
			else
				return Time.time;
		}

		/// <summary>
		/// Returns either the adjusted delta time or the absolute delta time, depending on 
		/// the value of the TimeScaleIndependent property
		/// </summary>
		protected float getDeltaTime()
		{
			if( IsTimeScaleIndependent )
				return TweenManager.realDeltaTime;
			else
				return Time.deltaTime;
		}

		#endregion

		#region Callback setters

		/// <summary>
		/// Set the delegate to be called when this tween completes a loop
		/// </summary>
		public TweenBase OnLoopCompleted( TweenCallback function )
		{
			this.TweenLoopCompleted = function;
			return this;
		}

		/// <summary>
		/// Set the delegate to be called when this tween completes
		/// </summary>
		public TweenBase OnCompleted( TweenCallback function )
		{
			this.TweenCompleted = function;
			return this;
		}

		/// <summary>
		/// Set the delegate to be called when this tween is paused
		/// </summary>
		public TweenBase OnPaused( TweenCallback function )
		{
			this.TweenPaused = function;
			return this;
		}

		/// <summary>
		/// Set the delegate to be called when this tween is resumed
		/// </summary>
		public TweenBase OnResumed( TweenCallback function )
		{
			this.TweenResumed = function;
			return this;
		}

		/// <summary>
		/// Set the delegate to be called when this tween is started 
		/// </summary>
		public TweenBase OnStarted( TweenCallback function )
		{
			this.TweenStarted = function;
			return this;
		}

		/// <summary>
		/// Set the delegate to be called when this tween is stopped 
		/// </summary>
		public TweenBase OnStopped( TweenCallback function )
		{
			this.TweenStopped = function;
			return this;
		}

		#endregion

		#region Public helper methods

		public virtual TweenBase Wait( float seconds )
		{
			return Chain( new TweenWait( seconds ) );
		}

		#endregion

		#region Event signalers

		protected virtual void raisePaused()
		{
			if( TweenPaused != null )
			{
				TweenPaused( this );
			}
		}

		protected virtual void raiseResumed()
		{
			if( TweenResumed != null )
			{
				TweenResumed( this );
			}
		}

		protected virtual void raiseStarted()
		{
			if( TweenStarted != null )
			{
				TweenStarted( this );
			}
		}

		protected virtual void raiseStopped()
		{
			if( TweenStopped != null )
			{
				TweenStopped( this );
			}
		}

		protected virtual void raiseCompleted()
		{
			if( TweenCompleted != null )
			{
				TweenCompleted( this );
			}
		}

		protected virtual void raiseLoopCompleted()
		{
			if( TweenLoopCompleted != null )
			{
				TweenLoopCompleted( this );
			}
		}

		#endregion

		#region System.Object overrides

		public override string ToString()
		{
			return !string.IsNullOrEmpty( Name ) ? Name : base.ToString();
		}

		#endregion 

		#region IPoolableObject Members

		/// <summary>
		/// Overridden in descendant classes to release the object back to the associated object pool.
		/// </summary>
		public abstract void Release();

		#endregion

	}

}
