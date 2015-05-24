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
	/// Used to animate the opacity of any component
	/// </summary>
	[AddComponentMenu( "Daikon Forge/Tween/Object Alpha" )]
	public class TweenObjectAlpha : TweenComponent<float>
	{

		#region Protected serialized fields 

		[SerializeField]
		protected Component target;

		#endregion 

		#region Private runtime variables

		private TweenEasingCallback easingFunc;

		#endregion

		#region Public properties 

		/// <summary>
		/// Provides information about the target property being tweened
		/// </summary>
		public Component Target
		{
			get { return this.target; }
			set
			{
				this.target = value;
				Stop();
			}
		}

		#endregion 

		#region Private utility methods

		protected override void validateTweenConfiguration()
		{
			
			if( target == null )
			{
				throw new InvalidOperationException( "The Target cannot be NULL" );
			}

			if( startValue < 0 || startValue > 1 )
				throw new InvalidOperationException( "The Start Value must be between 0 and 1" );

			if( endValue < 0 || endValue > 1 )
				throw new InvalidOperationException( "The End Value must be between 0 and 1" );

			base.validateTweenConfiguration();

		}

		protected override void configureTween()
		{

			if( target == null )
			{

				target = gameObject.renderer;

				if( target == null )
				{

					if( this.tween != null )
					{
						tween.Stop();
						tween.Release();
						tween = null;
					}

					return;

				}

			}

			if( this.tween == null )
			{

				this.easingFunc = TweenEasingFunctions.GetFunction( this.easingType );

				this.tween = (Tween<float>)
					Target.TweenAlpha()
					.SetEasing( this.modifyEasing )
					.OnStarted( ( x ) => { onStarted(); } )
					.OnStopped( ( x ) => { onStopped(); } )
					.OnPaused( ( x ) => { onPaused(); } )
					.OnResumed( ( x ) => { onResumed(); } )
					.OnLoopCompleted( ( x ) => { onLoopCompleted(); } )
					.OnCompleted( ( x ) => { onCompleted(); } );

			}

			var currentValue = tween.CurrentValue;

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
