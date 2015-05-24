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

	public interface ITweenPropertyBase
	{
		GameObject Target { get; set; }
		string ComponentType { get; set; }
		string MemberName { get; set; }
	}

	/// <summary>
	/// Defines the core API for tweening components that use reflection to 
	/// tween a named property on a component
	/// </summary>
	[InspectorGroupOrder( "General", "Animation", "Looping", "Property", "Values" )]
	public class TweenPropertyComponent<T> : TweenComponent<T>, ITweenPropertyBase
	{

		#region Serialized fields 

		[SerializeField]
		[Inspector( "Property", Label = "Target", Order = 0 )]
		protected GameObject target;

		[SerializeField]
		protected string componentType;

		[SerializeField]
		[Inspector( "Property", Label = "Field", Order = 1 )]
		protected string memberName;

		#endregion 

		#region Private runtime variables

		private Component component;
		private TweenEasingCallback easingFunc;

		#endregion

		#region Public properties 

		/// <summary>
		/// Gets or sets a reference to the GameObject containing the component whose 
		/// property is to be animated
		/// </summary>
		public GameObject Target
		{
			get { return this.target; }
			set
			{
				if( value != this.target )
				{
					this.target = value;
					if( State != TweenState.Stopped )
					{
						Stop();
						Play();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the name of the System.Type representing the target component
		/// </summary>
		public string ComponentType
		{
			get { return this.componentType; }
			set
			{
				if( value != this.componentType )
				{
					this.componentType = value;
					if( State != TweenState.Stopped )
					{
						Stop();
						Play();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the name of the field or property that will be animated
		/// </summary>
		public string MemberName
		{
			get { return this.memberName; }
			set
			{
				if( value != this.memberName )
				{
					this.memberName = value;
					if( State != TweenState.Stopped )
					{
						Stop();
						Play();
					}
				}
			}
		}

		#endregion 

		#region Monobehaviour events 

		public override void OnEnable()
		{

			if( target == null )
			{
				target = this.gameObject;
			}

			base.OnEnable();

		}

		#endregion 

		#region Base class overrides

		protected override void validateTweenConfiguration()
		{
			
			base.validateTweenConfiguration();

			if( this.target == null || string.IsNullOrEmpty( componentType ) || string.IsNullOrEmpty( memberName ) )
				return;

			var component = target.GetComponent( this.componentType );
			if( component == null )
				throw new NullReferenceException( string.Format( "Object {0} does not contain a {1} component", target.name, componentType ) );

			var interpolator = DaikonForge.Tween.Interpolation.Interpolators.Get<T>();
			if( interpolator == null )
				throw new KeyNotFoundException( string.Format( "There is no default interpolator defined for type '{0}'", typeof( T ).Name ) );

		}

		protected override void configureTween()
		{

			if( this.tween == null )
			{

				if( this.target == null || string.IsNullOrEmpty( componentType ) || string.IsNullOrEmpty( memberName ) )
					return;

				this.component = target.GetComponent( this.componentType );
				if( component == null )
					return;

				this.easingFunc = TweenEasingFunctions.GetFunction( this.easingType );

				this.tween = (Tween<T>)
					component.TweenProperty<T>( memberName )
					.SetEasing( this.modifyEasing )
					.OnStarted( ( x ) => { onStarted(); } )
					.OnStopped( ( x ) => { onStopped(); } )
					.OnPaused( ( x ) => { onPaused(); } )
					.OnResumed( ( x ) => { onResumed(); } )
					.OnLoopCompleted( ( x ) => { onLoopCompleted(); } )
					.OnCompleted( ( x ) => { onCompleted(); } );

			}

			var currentValue = TweenNamedProperty<T>.GetCurrentValue( component, memberName );
			var interpolator = DaikonForge.Tween.Interpolation.Interpolators.Get<T>();

			var actualStartValue = this.startValue;
			if( this.startValueType == TweenStartValueType.SyncOnRun )
			{
				actualStartValue = currentValue;
			}

			var actualEndValue = this.endValue;
			if( this.endValueType == TweenEndValueType.SyncOnRun )
				actualEndValue = currentValue;
			else if( this.endValueType == TweenEndValueType.Relative )
				actualEndValue = interpolator.Add( actualEndValue, actualStartValue );

			this.tween
				.SetStartValue( actualStartValue )
				.SetEndValue( actualEndValue )
				.SetDelay( this.startDelay, this.assignStartValueBeforeDelay )
				.SetDuration( this.duration )
				.SetLoopType( this.LoopType )
				.SetLoopCount( this.loopCount )
				.SetPlayDirection( this.playDirection );

		}

		#endregion 

		#region Private utility functions 

		private float modifyEasing( float time )
		{

			if( this.animCurve != null )
			{
				time = animCurve.Evaluate( time );
			}

			return this.easingFunc( time );

		}

		#endregion 

	}

}

#endif
