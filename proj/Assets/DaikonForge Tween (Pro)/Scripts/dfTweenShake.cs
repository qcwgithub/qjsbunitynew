/* Copyright 2014 Daikon Forge */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Provides a shake effect for Vector3-based values (positions)
	/// </summary>
	public class TweenShake : TweenBase, IPoolableObject
	{

		#region Public Properties

		/// <summary>
		/// The base starting value
		/// </summary>
		public Vector3 StartValue;

		/// <summary>
		/// The magnitude of the shake
		/// </summary>
		public float ShakeMagnitude;

		/// <summary>
		/// The duration of the shake
		/// </summary>
		public float ShakeDuration;

		/// <summary>
		/// The speed of the shake
		/// </summary>
		public float ShakeSpeed;

		/// <summary>
		/// Callback for handling result value
		/// </summary>
		public TweenAssignmentCallback<Vector3> Execute;

		#endregion

		#region Callbacks

		/// <summary>
		/// Raised when the shake has finished playing
		/// </summary>
		public TweenCallback ShakeCompleted;

		#endregion

		#region Protected Properties

		protected Vector3 currentValue;

		#endregion

		#region Object pooling

		// Contains the pool of all available Tween<T> instances
		private static List<TweenShake> pool = new List<TweenShake>();

		/// <summary>
		/// Obtain a dfShake instance from the object pool, if one is available. A new 
		/// instance will be returned if there are no instances available in the object 
		/// pool.
		/// </summary>
		public static TweenShake Obtain()
		{

			if( pool.Count > 0 )
			{

				var pooledInstance = pool[ pool.Count - 1 ];
				pool.RemoveAt( pool.Count - 1 );

				return pooledInstance;

			}

			return new TweenShake();

		}

		/// <summary>
		/// Returns the dfShake instance back to the object pool
		/// </summary>
		public override void Release()
		{

			// Make sure that the tween is no longer running
			Stop();

			// Reset all values to defaults
			this.StartValue = Vector3.zero;
			this.currentValue = Vector3.zero;
			this.CurrentTime = 0f;
			this.Delay = 0;

			// Clear all event handlers
			this.ShakeCompleted = null;

			// Clear main callbacks
			this.Execute = null;

			// Add the shake back to the object pool
			pool.Add( this );

		}

		#endregion

		#region Constructors

		public TweenShake()
		{
			this.ShakeSpeed = 10f;
		}

		public TweenShake( Vector3 StartValue, float ShakeMagnitude, float ShakeDuration, float ShakeSpeed, float StartDelay, bool AutoCleanup, TweenAssignmentCallback<Vector3> OnExecute )
		{
			SetStartValue( StartValue )
				.SetShakeMagnitude( ShakeMagnitude )
				.SetDuration( ShakeDuration )
				.SetShakeSpeed( ShakeSpeed )
				.SetDelay( StartDelay )
				.SetAutoCleanup( AutoCleanup )
				.OnExecute( OnExecute );
		}

		#endregion

		#region Property Setter Methods

		/// <summary>
		/// Sets the TimeScaleIndependant property
		/// </summary>
		public TweenShake SetTimeScaleIndependent( bool timeScaleIndependent )
		{
			this.IsTimeScaleIndependent = timeScaleIndependent;
			return this;
		}

		/// <summary>
		/// Sets whether this shake will be automatically pooled when it is finished playing
		/// </summary>
		public TweenShake SetAutoCleanup( bool autoCleanup )
		{
			this.AutoCleanup = autoCleanup;
			return this;
		}

		/// <summary>
		/// Sets the duration of the shake
		/// </summary>
		public TweenShake SetDuration( float duration )
		{

			this.ShakeDuration = duration;

			return this;

		}

		/// <summary>
		/// Sets the start value of the shake
		/// </summary>
		public TweenShake SetStartValue( Vector3 value )
		{

			this.StartValue = value;

			return this;

		}

		/// <summary>
		/// Set the delay period for this shake to pause before executing
		/// </summary>
		public TweenShake SetDelay( float seconds )
		{

			this.Delay = seconds;

			return this;

		}

		/// <summary>
		/// Set the shake magnitude
		/// </summary>
		public TweenShake SetShakeMagnitude( float magnitude )
		{
			this.ShakeMagnitude = magnitude;
			return this;
		}

		/// <summary>
		/// Set the speed of the shake
		/// </summary>
		public TweenShake SetShakeSpeed( float speed )
		{
			this.ShakeSpeed = speed;
			return this;
		}

		#endregion

		#region Callback Setters

		/// <summary>
		/// Set the assignment function for the shake
		/// </summary>
		public TweenShake OnExecute( TweenAssignmentCallback<Vector3> Execute )
		{
			this.Execute = Execute;
			return this;
		}

		/// <summary>
		/// Set a function to be called after the shake has completed
		/// </summary>
		public TweenShake OnComplete( TweenCallback Complete )
		{
			this.ShakeCompleted = Complete;
			return this;
		}

		#endregion

		#region Other public methods

		public override void Update()
		{

			#region Implement Start Delay

			var currentTime = getCurrentTime();

			if( this.State == TweenState.Started )
			{

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

			CurrentTime = Mathf.MoveTowards( CurrentTime, 1f, getDeltaTime() / ShakeDuration );

			float mag = 1f - CurrentTime;
			mag *= ShakeMagnitude;

			// we're going to sample from Mathf.PerlinNoise in order to generate the shake
			float a = Mathf.PerlinNoise( 0.33f, currentTime * ShakeSpeed ) * 2 - 1;
			float b = Mathf.PerlinNoise( 0.66f, currentTime * ShakeSpeed ) * 2 - 1;
			float c = Mathf.PerlinNoise( 1f, currentTime * ShakeSpeed ) * 2 - 1;

			currentValue = StartValue + ( new Vector3( a, b, c ) * mag );

			if( this.Execute != null )
			{
				this.Execute( currentValue );
			}

			if( CurrentTime >= 1f )
			{

				Stop();
				raiseCompleted();

				if( AutoCleanup )
				{
					Release();
				}

			}

		}

		#endregion

	}

}
