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
	/// Used to animate the Transform.Position property of any component
	/// </summary>
	[AddComponentMenu( "Daikon Forge/Tween/Object Position" )]
	public class TweenObjectPosition : TweenComponent<Vector3>
	{

		#region Protected serialized members 

		[SerializeField]
		protected bool useLocalPosition = false;

		#endregion

		#region Private runtime variables

		private TweenEasingCallback easingFunc;

		#endregion 

		#region Public properties 

		/// <summary>
		/// If set to TRUE, the Transform.localPosition property will be animated instead
		/// of the Transform.position property.
		/// </summary>
		public bool UseLocalPosition
		{
			get { return this.useLocalPosition; }
			set
			{
				this.useLocalPosition = value;
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
					transform.TweenPosition( this.useLocalPosition )
					.SetEasing( this.modifyEasing )
					.OnStarted( ( x ) => { onStarted(); } )
					.OnStopped( ( x ) => { onStopped(); } )
					.OnPaused( ( x ) => { onPaused(); } )
					.OnResumed( ( x ) => { onResumed(); } )
					.OnLoopCompleted( ( x ) => { onLoopCompleted(); } )
					.OnCompleted( ( x ) => { onCompleted(); } );

			}

			var currentValue = ( this.useLocalPosition ) ? transform.localPosition : transform.position;

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
