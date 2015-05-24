/* Copyright 2013-2014 Daikon Forge */
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using DaikonForge.Tween;
using DaikonForge.Editor;

#if !FREE_VERSION

namespace DaikonForge.Tween.Components
{

	/// <summary>
	/// Used to animate a camera to provide a shaking action
	/// </summary>
	[AddComponentMenu( "Daikon Forge/Tween/Camera Shake" )]
	[InspectorGroupOrder( "General", "Animation", "Looping", "Parameters" )]
	public class TweenCameraShake : TweenComponentBase
	{

		#region Serialized fields 

		[SerializeField]
		[Inspector( "Parameters", 0, Label = "Duration" )]
		protected float duration = 1f;

		[SerializeField]
		[Inspector( "Parameters", 0, Label = "Magnitude" )]
		protected float shakeMagnitude = 0.25f;

		[SerializeField]
		[Inspector( "Parameters", 0, Label = "Speed" )]
		protected float shakeSpeed = 13f;

		#endregion 

		#region Public properties 

		/// <summary>
		/// The amount of time in seconds for the tween to operate
		/// </summary>
		public float Duration
		{
			get { return this.duration; }
			set
			{
				this.duration = Mathf.Max( 0f, value );
			}
		}

		/// <summary>
		/// The magnitude of the shake
		/// </summary>
		public float ShakeMagnitude
		{
			get { return this.shakeMagnitude; }
			set
			{
				if( value != this.shakeMagnitude )
				{
					this.shakeMagnitude = value;
					Stop();
				}
			}
		}

		/// <summary>
		/// The speed of the shake
		/// </summary>
		public float ShakeSpeed
		{
			get { return this.shakeSpeed; }
			set
			{
				if( value != this.shakeSpeed )
				{
					this.shakeSpeed = value;
					Stop();
				}
			}
		}

		#endregion 

		#region Private runtime variables

		protected TweenShake tween;

		#endregion

		#region Public properties 

		/// <summary>
		/// Returns the TweenBase instance that performs the actual animation
		/// </summary>
		public override TweenBase BaseTween
		{
			get 
			{
				configureTween();
				return tween; 
			}
		}

		/// <summary>
		/// Returns the current state of the tween
		/// </summary>
		public override TweenState State
		{
			get
			{
				if( tween == null )
					return TweenState.Stopped;
				else
					return tween.State;
			}
		}

		#endregion

		#region Monobehaviour events

		/// <summary>
		/// Called by Unity when the application is about to shut down
		/// </summary>
		public virtual void OnApplicationQuit()
		{
			cleanup();
		}

		public override void OnDisable()
		{
			base.OnDisable();
			cleanup();
		}

		#endregion

		#region Public methods

		public override void Play()
		{

			if( State != TweenState.Stopped )
				Stop();

			configureTween();
			validateTweenConfiguration();

			tween.Play();

		}

		public override void Stop()
		{
			if( IsPlaying )
			{
				validateTweenConfiguration();
				tween.Stop();
			}
		}

		/// <summary>
		/// Pause the tween animation (if the tween is currently playing)
		/// </summary>
		public override void Pause()
		{
			if( IsPlaying )
			{
				validateTweenConfiguration();
				tween.Pause();
			}
		}

		/// <summary>
		/// Resume the tween animation (if the tween is in a paused state)
		/// </summary>
		public override void Resume()
		{
			if( IsPaused )
			{
				validateTweenConfiguration();
				tween.Resume();
			}
		}

		public override void Rewind()
		{
			validateTweenConfiguration();
			tween.Rewind();
		}

		public override void FastForward()
		{
			validateTweenConfiguration();
			tween.FastForward();
		}

		#endregion

		#region Private utility methods

		/// <summary>
		/// This function should perform all necessary cleanup. It is expected that the [this.tween]
		/// reference will be NULL after this function completes.
		/// </summary>
		protected void cleanup()
		{

			if( tween != null )
			{
				tween.Stop();
				tween.Release();
				tween = null;
			}

		}

		protected void validateTweenConfiguration()
		{

			this.loopCount = Mathf.Max( 0, this.loopCount );

			if( gameObject.camera == null )
			{
				throw new InvalidOperationException( "Camera not found" );
			}

		}

		protected void configureTween()
		{

			var camera = gameObject.camera;

			if( this.tween == null )
			{

				this.tween = (TweenShake)
					camera.ShakePosition( true )
					.OnStarted( ( x ) => { onStarted(); } )
					.OnStopped( ( x ) => { onStopped(); } )
					.OnPaused( ( x ) => { onPaused(); } )
					.OnResumed( ( x ) => { onResumed(); } )
					.OnLoopCompleted( ( x ) => { onLoopCompleted(); } )
					.OnCompleted( ( x ) => { onCompleted(); } );

			}

			this.tween
				.SetDelay( this.startDelay )
				.SetDuration( this.Duration )
				.SetShakeMagnitude( this.ShakeMagnitude )
				.SetShakeSpeed( this.ShakeSpeed );

		}

		#endregion

	}

}

#endif
