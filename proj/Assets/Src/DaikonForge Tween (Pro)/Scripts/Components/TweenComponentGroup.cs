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
	[AddComponentMenu( "Daikon Forge/Tween/Group" )]
	[InspectorGroupOrder( "General", "Animation", "Looping", "Tweens" )]
	public class TweenComponentGroup : TweenComponentBase
	{

		#region Protected serialized fields 

		[SerializeField]
		[Inspector( "General", 1, Label = "Mode" )]
		protected TweenGroupMode groupMode = TweenGroupMode.Sequential;

		[SerializeField]
		[Inspector( "Tweens", 0, Label = "Tweens" )]
		protected List<TweenPlayableComponent> tweens = new List<TweenPlayableComponent>();

		#endregion

		#region Private runtime variables

		protected TweenGroup group;

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
				return group;
			}
		}

		/// <summary>
		/// Returns the list of tween components in the group
		/// </summary>
		public List<TweenPlayableComponent> Tweens
		{
			get { return this.tweens; }
		}

		/// <summary>
		/// Gets or sets the TweenGroupMode (Sequential or Concurrent) that will be used when
		/// playing the constituent tweens
		/// </summary>
		public TweenGroupMode GroupMode
		{
			get { return this.groupMode; }
			set
			{
				if( value != this.groupMode )
				{
					this.groupMode = value;
					Stop();
				}
			}
		}

		/// <summary>
		/// Returns the current state of the tween
		/// </summary>
		public override TweenState State
		{
			get
			{
				if( group == null )
					return TweenState.Stopped;
				else
					return group.State;
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

			group.Play();

		}

		public override void Stop()
		{

			if( IsPlaying )
			{
				validateTweenConfiguration();
				group.Stop();
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
				group.Pause();
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
				group.Resume();
			}
		}

		public override void Rewind()
		{
			validateTweenConfiguration();
			group.Rewind();
		}

		public override void FastForward()
		{
			validateTweenConfiguration();
			group.FastForward();
		}

		#endregion

		#region Private utility methods

		/// <summary>
		/// This function should perform all necessary cleanup. It is expected that the [this.tween]
		/// reference will be NULL after this function completes.
		/// </summary>
		protected void cleanup()
		{

			if( group != null )
			{
				group.Stop();
				group.Release();
				group = null;
			}

		}

		protected void validateTweenConfiguration()
		{

			this.loopCount = Mathf.Max( 0, this.loopCount );

			if( loopType != TweenLoopType.None && loopType != TweenLoopType.Loop )
				throw new System.ArgumentException( "LoopType may only be one of the following values: TweenLoopType.None, TweenLoopType.Loop" );

		}

		protected void configureTween()
		{

			if( this.group == null )
			{

				this.group = (TweenGroup)
					new TweenGroup()
					.OnStarted( ( x ) => { onStarted(); } )
					.OnStopped( ( x ) => { onStopped(); } )
					.OnPaused( ( x ) => { onPaused(); } )
					.OnResumed( ( x ) => { onResumed(); } )
					.OnLoopCompleted( ( x ) => { onLoopCompleted(); } )
					.OnCompleted( ( x ) => { onCompleted(); } );

			}

			group
				.ClearTweens()
				.SetMode( this.groupMode )
				.SetDelay( this.startDelay )
				.SetLoopType( this.loopType )
				.SetLoopCount( this.loopCount );

			for( int i = 0; i < tweens.Count; i++ )
			{

				var tween = tweens[ i ];
				if( tween != null )
				{
					
					tween.AutoRun = false;
					var baseTween = tween.BaseTween;

					if( baseTween == null )
						Debug.LogError( "Base tween not set", tween );
					else
						group.AppendTween( baseTween );

				}

			}

		}

		#endregion

	}

}

#endif
