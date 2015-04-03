/* Copyright 2013-2014 Daikon Forge */
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using DaikonForge.Tween;

namespace DaikonForge.Tween.Components
{

	using DaikonForge.Editor;

	/// <summary>
	/// Defines the core API for tweening components
	/// </summary>
	public abstract class TweenPlayableComponent : MonoBehaviour
	{

		#region Events

#pragma warning disable 0067

		/// <summary>
		/// Raised when the tween animation has started playing 
		/// </summary>
		public event TweenComponentNotification TweenStarted;

		/// <summary>
		/// Raised when the tween animation has stopped playing before completion
		/// </summary>
		public event TweenComponentNotification TweenStopped;

		/// <summary>
		/// Raised when the tween animation has been paused
		/// </summary>
		public event TweenComponentNotification TweenPaused;

		/// <summary>
		/// Raised when the tween animation has been resumed after having been paused
		/// </summary>
		public event TweenComponentNotification TweenResumed;

		/// <summary>
		/// Raised when the tween animation has successfully completed a single loop
		/// </summary>
		public event TweenComponentNotification TweenLoopCompleted;

		/// <summary>
		/// Raised when the tween animation has successfully completed
		/// </summary>
		public event TweenComponentNotification TweenCompleted;

#pragma warning restore 0067

		#endregion

		#region Protected serialized fields 

		[SerializeField]
		protected bool autoRun = false;

		#endregion 

		#region Public properties

		/// <summary>
		/// Gets or sets the user-defined name of the Tween, which is 
		/// useful to the developer at design time when there are 
		/// multiple tweens on a single GameObject
		/// </summary>
		public virtual string TweenName { get; set; }

		/// <summary>
		/// Returns the current state of the tween
		/// </summary>
		public abstract TweenState State { get; }

		/// <summary>
		/// Returns the TweenBase instance that performs the actual animation
		/// </summary>
		public abstract TweenBase BaseTween { get; }

		/// <summary>
		/// If set to TRUE, the tween will automatically run on startup
		/// </summary>
		[Inspector( "General", 1, BackingField = "autoRun", Tooltip = "If set to TRUE, this Tween will automatically play when the scene starts" )]
		public bool AutoRun
		{
			get { return this.autoRun; }
			set { this.autoRun = value; }
		}

		#endregion 

		#region Play functions

		/// <summary>
		/// Starts the tween animation
		/// </summary>
		public abstract void Play();

		/// <summary>
		/// Stops the tween animation
		/// </summary>
		public abstract void Stop();

		/// <summary>
		/// Rewinds the tween animation to the beginning
		/// </summary>
		public abstract void Rewind();

		/// <summary>
		/// Fast forwards the tween animation to the beginning
		/// </summary>
		public abstract void FastForward();

		/// <summary>
		/// Pause the tween animation (if the tween is currently playing)
		/// </summary>
		public abstract void Pause();

		/// <summary>
		/// Resume the tween animation (if the tween is in a paused state)
		/// </summary>
		public abstract void Resume();

		#endregion 

		#region Monobehaviour Events

		/// <summary>
		/// Called by Unity to initialize variables before the game starts or when
		/// the component is instantiated
		/// </summary>
		public virtual void Awake()
		{
			// Stub
		}

		/// <summary>
		/// Called by Unity on the frame when a script is enabled just before any of the 
		/// Update methods is called the first time.
		/// </summary>
		public virtual void Start()
		{
			// Stub
		}

		/// <summary>
		/// Called by Unity when the component becomes enabled and active.
		/// </summary>
		public virtual void OnEnable()
		{
			// Stub
		}

		/// <summary>
		/// Called by Unity when the component becomes disabled or inactive.
		/// </summary>
		public virtual void OnDisable()
		{
			// Stub
		}

		/// <summary>
		/// Called by Unity when the component will be destroyed.
		/// </summary>
		public virtual void OnDestroy()
		{
			// Stub
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Enables the tween animation 
		/// </summary>
		public virtual void Enable()
		{
			this.enabled = true;
		}

		/// <summary>
		/// Disables the tween animation 
		/// </summary>
		public virtual void Disable()
		{
			this.enabled = false;
		}

		/// <summary>
		/// Can be used from within a coroutine to wait until the tween operation has completed
		/// </summary>
		public virtual IEnumerator WaitForCompletion()
		{

			while( State != TweenState.Stopped )
			{
				yield return null;
			}

		}

		#endregion

		#region Event-raising functions (used by derived classes)

		/// <summary>
		/// Used by derived classes to raise the TweenPaused event
		/// </summary>
		protected virtual void onPaused()
		{
			if( TweenPaused != null )
				TweenPaused( this );
		}

		/// <summary>
		/// Used by derived classes to raise the TweenResumed event
		/// </summary>
		protected virtual void onResumed()
		{
			if( TweenResumed != null )
				TweenResumed( this );
		}

		/// <summary>
		/// Used by derived classes to raise the TweenStarted event
		/// </summary>
		protected virtual void onStarted()
		{
			if( TweenStarted != null )
				TweenStarted( this );
		}

		/// <summary>
		/// Used by derived classes to raise the TweenStopped event
		/// </summary>
		protected virtual void onStopped()
		{
			if( TweenStopped != null )
				TweenStopped( this );
		}

		/// <summary>
		/// Used by derived classes to raise the TweenLoopCompleted event
		/// </summary>
		protected virtual void onLoopCompleted()
		{
			if( TweenLoopCompleted != null )
				TweenLoopCompleted( this );
		}

		/// <summary>
		/// Used by derived classes to raise the TweenComplete event
		/// </summary>
		protected virtual void onCompleted()
		{
			if( TweenCompleted != null )
				TweenCompleted( this );
		}

		#endregion

		#region System.Object overrides

		/// <summary>
		/// Returns a formatted string summarizing this object's state
		/// </summary>
		public override string ToString()
		{
			return this.TweenName + " - " + base.ToString();
		}

		#endregion

	}

	/// <summary>
	/// Abstract class that defines the core API of a tweening component
	/// </summary>
	[Serializable]
	public abstract class TweenComponentBase : TweenPlayableComponent
	{

		#region Protected serialized members 

		[SerializeField]
		[Inspector( "General", Order = -1, Label = "Name", Tooltip = "For your convenience, you may specify a name for this Tween" )]
		protected string tweenName;

		[SerializeField]
		[Inspector( "Animation", Order = 0, Label = "Delay", Tooltip = "The amount of time in seconds to delay before starting the animation" )]
		protected float startDelay = 0f;

		[SerializeField]
		[Inspector( "Animation", Order = 1, Label = "Assign Start First", Tooltip = "If set, the StartValue will be assigned to the target before the delay (if any) is performed" )]
		protected bool assignStartValueBeforeDelay = true;

		[SerializeField]
		[Inspector( "Looping", Order = 1, Label = "Type", Tooltip = "Specify whether the animation will loop at the end" )]
		protected TweenLoopType loopType = TweenLoopType.None;

		[SerializeField]
		[Inspector( "Looping", Order = 1, Label = "Count", Tooltip = "If set to 0, the animation will loop forever" )]
		protected int loopCount = 0;

		#endregion 

		private static bool IsLoopCountVisible( object target )
		{
			return true;
		}

		#region Private instance variables

		protected bool wasAutoStarted = false;

		#endregion

		#region Public properties

		/// <summary>
		/// The amount of time in seconds after Play() is called before the tween will start animating
		/// </summary>
		public float StartDelay
		{
			get { return this.startDelay; }
			set { this.startDelay = value; }
		}

		/// <summary>
		/// If set to TRUE, the StartValue will be assigned to the target <i>before</i> any 
		/// delay (assigned in the StartDelay property) takes place.
		/// </summary>
		public bool AssignStartValueBeforeDelay
		{
			get { return this.assignStartValueBeforeDelay; }
			set { this.assignStartValueBeforeDelay = value; }
		}

		/// <summary>
		/// Gets or sets a value that controls how the Tween operation is looped
		/// </summary>
		public TweenLoopType LoopType
		{
			get { return this.loopType; }
			set
			{
				this.loopType = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					Play();
				}
			}
		}

		/// <summary>
		/// If LoopType is set to any value other than TweenLoopType.None, this value will
		/// be used to specify how many times the tween will loop.
		/// </summary>
		public int LoopCount
		{
			get { return this.loopCount; }
			set
			{
				this.loopCount = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					Play();
				}
			}
		}

		/// <summary>
		/// Returns TRUE if the Tween is currently playing
		/// </summary>
		public bool IsPlaying
		{
			get { return this.enabled && ( State == TweenState.Started || State == TweenState.Playing ); }
		}

		/// <summary>
		/// Indicates whether the tween is paused
		/// </summary>
		public bool IsPaused
		{
			get { return State == TweenState.Paused; }
		}

		#endregion

		#region Monobehaviour Events

		/// <summary>
		/// Called by Unity on the frame when a script is enabled just before any of the 
		/// Update methods is called the first time.
		/// </summary>
		public override void Start()
		{

			base.Start();

			if( autoRun && !wasAutoStarted )
			{
				wasAutoStarted = true;
				Play();
			}

		}

		/// <summary>
		/// Called by Unity when the component becomes enabled and active
		/// </summary>
		public override void OnEnable()
		{

			base.OnEnable();

			if( autoRun && !wasAutoStarted )
			{
				wasAutoStarted = true;
				Play();
			}

		}

		/// <summary>
		/// Called by Unity when the component becomes disabled or inactive.
		/// </summary>
		public override void OnDisable()
		{

			base.OnDisable();
			
			if( IsPlaying )
			{
				Stop();
			}

			wasAutoStarted = false;

		}

		#endregion

		#region System.Object overrides

		/// <summary>
		/// Returns a formatted string summarizing this object's state
		/// </summary>
		public override string ToString()
		{
			return string.Format( "{0}.{1} '{2}'", gameObject.name, this.GetType().Name, this.tweenName );
		}

		#endregion

	}

	/// <summary>
	/// Type-safe component used for animating common properties of game objects
	/// </summary>
	[InspectorGroupOrder( "General", "Animation", "Looping", "Values" )]
	public abstract class TweenComponent<T> : TweenComponentBase
	{

		#region Protected serialized variables 

		[SerializeField]
		[Inspector( "Animation", Order = 4, Tooltip = "How long the Tween should take to complete the animation" )]
		protected float duration = 1f;

		[SerializeField]
		[Inspector( "Animation", Order = 2, Tooltip = "The type of easing, if any, to apply to the animation" )]
		protected EasingType easingType;

		[SerializeField]
		[Inspector( "Animation", Order = 3, Label = "Curve", Tooltip = "An animation curve can be used to modify the animation timeline" )]
		protected AnimationCurve animCurve = new AnimationCurve( new Keyframe( 0f, 0f, 0f, 1f ), new Keyframe( 1f, 1f, 1f, 0f ) );

		[SerializeField]
		[Inspector( "Animation", Order = 5, Label = "Direction" )]
		protected TweenDirection playDirection = TweenDirection.Forward;

		[SerializeField]
		[Inspector( "Values", Order = 0 )]
		protected TweenStartValueType startValueType = TweenStartValueType.Absolute;

		[SerializeField]
		[Inspector( "Values", Order = 1 )]
		protected T startValue;

		[SerializeField]
		[Inspector( "Values", Order = 2 )]
		protected TweenEndValueType endValueType = TweenEndValueType.Absolute;

		[SerializeField]
		[Inspector( "Values", Order = 3 )]
		protected T endValue;

		#endregion 

		#region Private runtime variables

		protected Tween<T> tween;

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
		/// A user-editable AnimationCurve that will be applied to the Tween operation
		/// </summary>
		public AnimationCurve AnimationCurve
		{
			get { return this.animCurve; }
			set { this.animCurve = value; }
		}

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
		/// Represents the starting value for the tween
		/// </summary>
		public T StartValue
		{
			get { return this.startValue; }
			set
			{
				this.startValue = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					Play();
				}
			}
		}

		/// <summary>
		/// Specifies how the StartValue property of a tween component will be evaluated
		/// </summary>
		public TweenStartValueType StartValueType
		{
			get { return this.startValueType; }
			set
			{
				this.startValueType = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					Play();
				}
			}
		}

		/// <summary>
		/// Represents the ending value for the tween
		/// </summary>
		public T EndValue
		{
			get { return this.endValue; }
			set
			{
				this.endValue = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					Play();
				}
			}
		}

		/// <summary>
		/// Specifies how the EndValue property of a tween component will be evaluated
		/// </summary>
		public TweenEndValueType EndValueType
		{
			get { return this.endValueType; }
			set
			{
				this.endValueType = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					Play();
				}
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

#if UNITY_EDITOR
		public void OnValidate()
		{
			this.duration = Mathf.Max( 0, this.duration );
			this.startDelay = Mathf.Max( 0, this.startDelay );
		}
#endif

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

		#region Protected utility methods

		/// <summary>
		/// This function should perform all necessary cleanup. It is expected that the [this.tween]
		/// reference will be NULL after this function completes.
		/// </summary>
		protected virtual void cleanup()
		{

			if( tween != null )
			{
				tween.Stop();
				tween.Release();
				tween = null;
			}

		}

		/// <summary>
		/// This function should validate the current tween configuration. The tween should either 
		/// be in a vaild state where the Play() function may be called, or should raise an exception
		/// otherwise.
		/// </summary>
		protected virtual void validateTweenConfiguration()
		{

			this.loopCount = Mathf.Max( 0, this.loopCount );

			if( tween == null )
			{
				throw new InvalidOperationException( "The tween has not been properly configured" );
			}

		}

		/// <summary>
		/// Called on derived classes to allow the class-specific internal tween configuration to be updated.
		/// </summary>
		protected abstract void configureTween();

		#endregion 

	}

}
