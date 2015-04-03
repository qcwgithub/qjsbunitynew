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
	[AddComponentMenu( "Daikon Forge/Tween/Move Along Path" )]
	[InspectorGroupOrder( "General", "Path", "Animation", "Looping" )]
	public class TweenObjectPath : TweenComponentBase
	{

		#region Protected serialized fields

		[SerializeField]
		[Inspector( "Animation", 0, Label = "Duration", Tooltip = "How long the Tween should take to complete the animation" )]
		protected float duration = 1f;

		[SerializeField]
		[Inspector( "Path", 0, Label = "Path", Tooltip = "The path for the object to follow" )]
		protected SplineObject path;

		[SerializeField]
		[Inspector( "Animation", 1, Label = "Orient To Path", Tooltip = "If set to TRUE, will orient the object to face the direction of the path" )]
		protected bool orientToPath = true;

		[SerializeField]
		protected TweenDirection playDirection = TweenDirection.Forward;

		#endregion

		#region Private runtime variables

		protected Tween<float> tween;

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
		/// Gets or sets a reference to the SplineObject that defines the path for the object to follow
		/// </summary>
		public SplineObject Path
		{
			get { return this.path; }
			set
			{
				cleanup();
				this.path = value;
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

		/// <summary>
		/// Gets or sets the direction that the tween will play in (forward or reverse)
		/// </summary>
		public TweenDirection PlayDirection
		{
			get { return this.playDirection; }
			set
			{
				this.playDirection = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					Play();
				}
			}
		}

		/// <summary>
		/// If set to TRUE, the target object will be oriented to follow the direction of the path
		/// </summary>
		public bool OrientToPath
		{
			get { return this.orientToPath; }
			set 
			{
				this.orientToPath = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					tween.Release();
					tween = null;
					Play();
				}
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

			if( Path == null )
			{
				throw new InvalidOperationException( "The Path property cannot be NULL" );
			}

		}

		protected void configureTween()
		{

			Path.CalculateSpline();

			if( this.tween == null )
			{

				this.tween = (Tween<float>)
					transform.TweenPath( Path.Spline, this.orientToPath )
					.OnStarted( ( x ) => { onStarted(); } )
					.OnStopped( ( x ) => { onStopped(); } )
					.OnPaused( ( x ) => { onPaused(); } )
					.OnResumed( ( x ) => { onResumed(); } )
					.OnLoopCompleted( ( x ) => { onLoopCompleted(); } )
					.OnCompleted( ( x ) => { onCompleted(); } );

			}

			Path.CalculateSpline();

			this.tween
				.SetDelay( this.startDelay )
				.SetDuration( this.duration )
				.SetLoopType( this.loopType )
				.SetLoopCount( this.loopCount )
				.SetPlayDirection( this.playDirection );

		}

		#endregion

	}

}

#endif
