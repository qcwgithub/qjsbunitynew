/* Copyright 2013-2014 Daikon Forge */
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using DaikonForge.Tween;

#if !FREE_VERSION

namespace DaikonForge.Tween.Components
{

	/// <summary>
	/// Used to animate the Transform.eulerAngles property of any component
	/// </summary>
	[AddComponentMenu( "Daikon Forge/Tween/Object Rotation" )]
	public class TweenObjectRotation : TweenComponent<Vector3>
	{

		#region Protected serialized members

		[SerializeField]
		protected bool useLocalRotation = false;

		[SerializeField]
		protected bool useShortestPath = true;

		#endregion

		#region Private runtime variables

		private TweenEasingCallback easingFunc;

		#endregion

		#region Public properties

		/// <summary>
		/// If set to TRUE, the Transform.localEulerAngles property will be animated instead
		/// of the Transform.eulerAngles property.
		/// </summary>
		public bool UseLocalRotation
		{
			get { return this.useLocalRotation; }
			set
			{
				this.useLocalRotation = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					Play();
				}
			}
		}

		/// <summary>
		/// If set to TRUE, the animation between the start rotation and the end 
		/// rotation will take place along the shortest path.
		/// </summary>
		public bool UseShortestPath
		{
			get { return this.useShortestPath; }
			set
			{
				this.useShortestPath = value;
				if( State != TweenState.Stopped )
				{
					Stop();
					Play();
				}
			}
		}

		#endregion

		#region Private utility methods

		protected override void configureTween()
		{

			if( this.tween == null )
			{

				this.easingFunc = TweenEasingFunctions.GetFunction( this.easingType );

				this.tween = (Tween<Vector3>)
					transform.TweenRotation( this.useShortestPath, this.useLocalRotation )
					.SetEasing( this.modifyEasing )
					.OnStarted( ( x ) => { onStarted(); } )
					.OnStopped( ( x ) => { onStopped(); } )
					.OnPaused( ( x ) => { onPaused(); } )
					.OnResumed( ( x ) => { onResumed(); } )
					.OnLoopCompleted( ( x ) => { onLoopCompleted(); } )
					.OnCompleted( ( x ) => { onCompleted(); } );

			}

			var currentValue = useLocalRotation ? transform.localEulerAngles : transform.eulerAngles;

			var actualStartValue = this.startValue;
			if( this.startValueType == TweenStartValueType.SyncOnRun )
			{
				actualStartValue = currentValue;
			}

			var actualEndValue = this.endValue;
			if( this.endValueType == TweenEndValueType.SyncOnRun )
				actualEndValue = currentValue;
			else if( this.endValueType == TweenEndValueType.Relative )
				actualEndValue += actualStartValue;

			this.tween
				.SetStartValue( actualStartValue )
				.SetEndValue( actualEndValue )
				.SetDelay( this.startDelay, this.assignStartValueBeforeDelay )
				.SetDuration( this.duration )
				.SetLoopType( this.LoopType )
				.SetLoopCount( this.loopCount )
				.SetPlayDirection( this.playDirection );

		}

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
