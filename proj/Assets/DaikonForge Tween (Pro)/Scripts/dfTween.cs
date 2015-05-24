/* Copyright 2014 Daikon Forge */
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DaikonForge.Tween.Interpolation;

namespace DaikonForge.Tween
{

	/// <summary>
	/// A generic tween class which can be used to tween any value
	/// </summary>
	public class Tween<T> : TweenBase
	{

		#region Public fields

		/// <summary>
		/// The starting value of the tween
		/// </summary>
		public T StartValue;

		/// <summary>
		/// The end value of the tween
		/// </summary>
		public T EndValue;

		/// <summary>
		/// A function which interpolates the start and end values
		/// </summary>
		public Interpolator<T> Interpolator;

		/// <summary>
		/// Callback for handling result value
		/// </summary>
		public TweenAssignmentCallback<T> Execute;

		/// <summary>
		/// Returns the current calculated value of the tween
		/// </summary>
		public T CurrentValue { get; protected set; }

		/// <summary>
		/// Returns whether EndValue is relative to StartValue or absolute
		/// </summary>
		public bool EndIsOffset { get; protected set; }

		/// <summary>
		/// Indicates the direction that the tween will play in
		/// </summary>
		public TweenDirection PlayDirection;

		#endregion

		#region Public callbacks 

		/// <summary>
		/// If assigned, this callback will be called when the tween is played and the result
		/// will be applied to StartValue, providing the opportunity to synchronize the start
		/// value on demand.
		/// </summary>
		public TweenSyncCallback<T> TweenSyncStartValue;

		/// <summary>
		/// If assigned, this callback will be called when the tween is played and the result
		/// will be applied to EndValue, providing the opportunity to synchronize the end
		/// value on demand.
		/// </summary>
		public TweenSyncCallback<T> TweenSyncEndValue;

		#endregion 

		#region Protected fields

		protected bool assignStartValueBeforeDelay = true;

		#endregion

		#region Object pooling

		// Contains the pool of all available Tween<T> instances. Note that the list is of
		// type List<object> due to limitations of Unity/Mono on Mac/iOS
		private static List<object> pool = new List<object>();

		/// <summary>
		/// Obtain a Tween instance from the object pool, if one is available. A new 
		/// instance will be returned if there are no instances available in the object 
		/// pool.
		/// </summary>
		public static Tween<T> Obtain()
		{

			if( pool.Count > 0 )
			{

				var pooledInstance = (Tween<T>)pool[ pool.Count - 1 ];
				pool.RemoveAt( pool.Count - 1 );

				return pooledInstance;

			}

			return new Tween<T>();

		}

		/// <summary>
		/// Returns the Tween instance back to the object pool
		/// </summary>
		public override void Release()
		{

			if( !pool.Contains( this ) )
			{

				Stop();
				Reset();

				pool.Add( this );

			}

		}

		#endregion

		#region Constructors

		public Tween()
		{
			// Provided for serialization support. It is better to use the static Obtain() function
			// when obtaining a reference to a tween, and to use Release() when done with that tween.
		}

		#endregion

		#region Property setter methods (method-chaining syntax) 

		/// <summary>
		/// Sets whether the end value is relative to start value
		/// </summary>
		public Tween<T> SetEndRelative( bool relative )
		{
			this.EndIsOffset = relative;
			return this;
		}

		/// <summary>
		/// Set whether this tween will be automatically pooled when it finishes playing
		/// </summary>
		public Tween<T> SetAutoCleanup( bool autoCleanup )
		{
			this.AutoCleanup = autoCleanup;
			return this;
		}

		/// <summary>
		/// Set the direction that the tween will play in
		/// </summary>
		public Tween<T> SetPlayDirection( TweenDirection direction )
		{
			this.PlayDirection = direction;
			return this;
		}

		/// <summary>
		/// Set the easing function of the tween
		/// </summary>
		public Tween<T> SetEasing( TweenEasingCallback easingFunction )
		{
			this.Easing = easingFunction;
			return this;
		}

		/// <summary>
		/// Sets the duration of the tween
		/// </summary>
		public Tween<T> SetDuration( float duration )
		{
			this.Duration = duration;
			return this;
		}

		/// <summary>
		/// Sets the end value of the tween
		/// </summary>
		public Tween<T> SetEndValue( T value )
		{
			this.EndValue = value;
			return this;
		}

		/// <summary>
		/// Sets the start value of the tween
		/// </summary>
		public Tween<T> SetStartValue( T value )
		{
			this.StartValue = this.CurrentValue = value;
			return this;
		}

		/// <summary>
		/// Set the delay period for this tween to pause before executing
		/// </summary>
		/// <param name="seconds">The number of seconds to delay</param>
		public Tween<T> SetDelay( float seconds )
		{
			return SetDelay( seconds, this.assignStartValueBeforeDelay );
		}

		/// <summary>
		/// Set the delay period for this tween to pause before executing
		/// </summary>
		/// <param name="seconds">The number of seconds to delay</param>
		/// <param name="assignStartValueBeforeDelay">If set to TRUE (default), it will assign the starting value before the delay. 
		/// If set to FALSE, no actions at all will be taken until the delay has expired</param>
		public Tween<T> SetDelay( float seconds, bool assignStartValueBeforeDelay )
		{
			this.Delay = seconds;
			this.assignStartValueBeforeDelay = assignStartValueBeforeDelay;
			return this;
		}

		/// <summary>
		/// Set the type of looping (if any) to be used
		/// </summary>
		public Tween<T> SetLoopType( TweenLoopType loopType )
		{
			this.LoopType = loopType;
			return this;
		}

		/// <summary>
		/// Set how many times the tween loops before stopping. If set to 0, the tween
		/// will loop forever.
		/// </summary>
		public Tween<T> SetLoopCount( int loopCount )
		{
			this.LoopCount = loopCount;
			return this;
		}

		/// <summary>
		/// Sets the IsTimeScaleIndependent property
		/// </summary>
		public Tween<T> SetTimeScaleIndependent( bool timeScaleIndependent )
		{
			this.IsTimeScaleIndependent = timeScaleIndependent;
			return this;
		}

		#endregion

		#region Tween Actions

		/// <summary>
		/// Play the tween
		/// </summary>
		public override TweenBase Play()
		{

			if( TweenSyncStartValue != null )
			{
				this.StartValue = TweenSyncStartValue();
			}

			if( TweenSyncEndValue != null )
			{
				this.EndValue = TweenSyncEndValue();
			}

			base.Play();

			ensureInterpolator();
			if( this.assignStartValueBeforeDelay )
			{
				evaluateAtTime( CurrentTime );
			}

			return this;

		}

		/// <summary>
		/// Rewind the tween to the beginning
		/// </summary>
		public override TweenBase Rewind()
		{

			base.Rewind();

			evaluateAtTime( CurrentTime );

			return this;

		}

		/// <summary>
		/// Fast forward the tween to the end
		/// </summary>
		public override TweenBase FastForward()
		{

			base.FastForward();

			evaluateAtTime( CurrentTime );

			return this;

		}

		/// <summary>
		/// Reverses the direction of play
		/// </summary>
		public virtual TweenBase ReversePlayDirection()
		{

			this.PlayDirection = ( this.PlayDirection == TweenDirection.Forward ) ? TweenDirection.Reverse : TweenDirection.Forward;

			return this;

		}

		#endregion

		#region Callback setters

		/// <summary>
		/// Sets the lerp function
		/// </summary>
		public Tween<T> SetInterpolator( Interpolator<T> interpolator )
		{
			this.Interpolator = interpolator;
			return this;
		}

		/// <summary>
		/// Sets the execution callback
		/// </summary>
		public Tween<T> OnExecute( TweenAssignmentCallback<T> function )
		{
			this.Execute = function;
			return this;
		}

		/// <summary>
		/// Sets the callback that will be used to synchronize the start value
		/// </summary>
		public Tween<T> OnSyncStartValue( TweenSyncCallback<T> function )
		{
			this.TweenSyncStartValue = function;
			return this;
		}

		/// <summary>
		/// Sets the callback that will be used to synchronize the end value
		/// </summary>
		public Tween<T> OnSyncEndValue( TweenSyncCallback<T> function )
		{
			this.TweenSyncEndValue = function;
			return this;
		}

		#endregion

		#region Other public methods

		/// <summary>
		/// Updates the tween. Internal use only.
		/// </summary>
		// @private
		public override void Update()
		{

			#region Implement Start Delay

			if( this.State == TweenState.Started )
			{

				var currentTime = getCurrentTime();

				if( currentTime < startTime + Delay )
				{
					return;
				}

				this.startTime = currentTime;
				this.CurrentTime = 0f;
				this.State = TweenState.Playing;

			}
			else if( this.State != TweenState.Playing )
			{
				return;
			}

			#endregion

			var frameDelta = getDeltaTime();
			CurrentTime = Mathf.MoveTowards( CurrentTime, 1f, frameDelta / Duration );

			float time = CurrentTime;

			if( this.Easing != null )
			{
				time = this.Easing( CurrentTime );
			}

			evaluateAtTime( time );

			if( CurrentTime >= 1f )
			{

				if( LoopType == TweenLoopType.Loop && --LoopCount != 0 )
				{

					raiseLoopCompleted();

					if( EndIsOffset )
					{
						StartValue = CurrentValue;
					}

					Rewind();
					Play();

				}
				else if( LoopType == TweenLoopType.Pingpong && --LoopCount != 0 )
				{

					raiseLoopCompleted();

					ReversePlayDirection();
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

			}

		}

		#endregion

		#region Private utility methods

		private bool ensureInterpolator()
		{
			if( this.Interpolator == null )
			{
				this.Interpolator = Interpolators.Get<T>();
			}
			return this.Interpolator != null;
		}

		protected override void Reset()
		{

			base.Reset();

			// Reset all fields to defaults
			this.StartValue = default( T );
			this.EndValue = default( T );
			this.CurrentValue = default( T );
			this.Duration = 1f;
			this.EndIsOffset = false;
			this.PlayDirection = TweenDirection.Forward;

			// Reset runtime variables to default values
			this.LoopCount = -1;
			this.assignStartValueBeforeDelay = true;

			// Clear main callbacks
			this.Interpolator = null;
			this.Execute = null;

		}

		private void evaluateAtTime( float time )
		{

			if( !ensureInterpolator() )
				throw new System.InvalidOperationException( string.Format( "No interpolator for type '{0}' has been assigned", typeof( T ).Name ) );

			var actualEndValue = !EndIsOffset ? EndValue : Interpolator.Add( StartValue, this.EndValue );

			if( this.PlayDirection == TweenDirection.Reverse )
				CurrentValue = this.Interpolator.Interpolate( actualEndValue, StartValue, time );
			else
				CurrentValue = this.Interpolator.Interpolate( StartValue, actualEndValue, time );

			if( this.Execute != null )
			{
				this.Execute( CurrentValue );
			}

		}

		#endregion

	}

}
