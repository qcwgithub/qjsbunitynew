using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using DaikonForge.Tween;

public class TestHighConcurrencyGroup : MonoBehaviour
{

	public GameObject pivot;
	public GameObject spriteTemplate;

	private const int SPRITE_COUNT = 1000;

	private List<TweenBase> tweens = new List<TweenBase>();

	public void Start()
	{
		DebugMessages.Add( "This sample animates the position, opacity, and scale of 1000 sprites simultaneously." );
		DebugMessages.Add( "Due to the high number of simultaneous animations, it may run slowly in the browser or on mobile devices." );
	}

	public void OnEnable()
	{

		if( spriteTemplate == null )
			return;

		var radius = Camera.main.orthographicSize * 0.95f;
		var duration = 0.5f;

		TweenEasingCallback easingFunc = TweenEasingFunctions.EaseOutSine;

		for( int i = 0; i < SPRITE_COUNT; i++ )
		{

			var angle = Random.Range( 0.0f, Mathf.PI * 2f );
			var endPosition = new Vector3
			(
				Mathf.Cos( angle ) * radius,
				Mathf.Sin( angle ) * radius
			);

			var sprite = (GameObject)GameObject.Instantiate( spriteTemplate );
			sprite.hideFlags = HideFlags.HideInHierarchy;

			var animateOpacity = sprite.renderer
				.TweenAlpha()
				.SetDuration( duration )
				.SetEasing( easingFunc )
				.SetStartValue( 0f )
				.SetEndValue( 0.8f );
               
                

			var animateScale = sprite.transform
				.TweenScale()
				.SetDuration( duration )
				.SetEasing( easingFunc ) 
				.SetStartValue( Vector3.one * 0.25f )
				.SetEndValue( Vector3.one );

			var animatePosition = sprite.transform
				.TweenPosition()
				.SetDuration( duration )
				.SetEasing( easingFunc )
				.SetStartValue( Vector3.zero ) 
				.SetEndValue( endPosition );

			var concurrentGroup = new TweenGroup()
				.SetMode( TweenGroupMode.Concurrent )
				.AppendTween( animateOpacity, animateScale, animatePosition );

			var sequentialGroup = new TweenGroup()
				.SetMode( TweenGroupMode.Sequential )
				.AppendDelay( 255f / i )
				.AppendTween( concurrentGroup )
				.SetLoopType( TweenLoopType.Loop );
                

			tweens.Add( sequentialGroup );

			sequentialGroup.Play();

		}

		// There are three animations per sprite plus two tween groups
		DebugMessages.Add( (SPRITE_COUNT * 5) + " tweens created..." );

		if( pivot != null )
		{

			var animatePivot = pivot.transform
				.TweenRotation()
				.SetDuration( 10 )
				.SetStartValue( new Vector3( 0, -45, 0 ) )
				.SetEndValue( new Vector3( 0, 45, 0 ) )
				.SetLoopType( TweenLoopType.Pingpong );

			tweens.Add( animatePivot );

			animatePivot.Play();

		}

	}

	public void OnGUI()
	{

		var rect = new Rect( 10, 30, 200, Screen.height - 100 );
		GUI.Box( rect, GUIContent.none );

		rect.xMin += 5;
		rect.yMin += 5;
		rect.xMax -= 10;

		GUILayout.BeginArea( rect );

		GUILayout.BeginVertical();
		{

			var tweenState = tweens[ 0 ].State;

			GUI.enabled = tweenState == TweenState.Stopped;
			if( GUILayout.Button( "Play" ) )
			{
				DebugMessages.Add( "Playing..." );
				for( int i = 0; i < tweens.Count; i++ )
				{
					tweens[ i ].Rewind();
					tweens[ i ].Play();
				}
			}

			if( tweenState == TweenState.Playing )
			{

				GUI.enabled = true;
				if( GUILayout.Button( "Pause" ) )
				{
					DebugMessages.Add( "Pausing..." );
					TweenManager.Instance.Pause();
				}

			}
			else if( tweenState == TweenState.Paused )
			{

				GUI.enabled = true;
				if( GUILayout.Button( "Resume" ) )
				{
					DebugMessages.Add( "Resuming..." );
					TweenManager.Instance.Resume();
				}

			}
			else
			{
				// Placeholder
				GUI.enabled = false;
				GUILayout.Button( "Pause" );
			}

			GUI.enabled = tweenState != TweenState.Stopped;
			if( GUILayout.Button( "Stop" ) )
			{
				DebugMessages.Add( "Stopped all tweens" );
				TweenManager.Instance.Stop();
			}

			GUILayout.Space( 25 );

			if( tweenState != TweenState.Stopped )
			{

				var timeScaleSetting = tweens[ 0 ].IsTimeScaleIndependent;
				var userSetting = GUILayout.Toggle( timeScaleSetting, "Time scale independent" );
				if( userSetting != timeScaleSetting )
				{
					DebugMessages.Add( "Toggled time scale independence" );
					for( int i = 0; i < tweens.Count; i++ )
					{
						tweens[ i ].SetIsTimeScaleIndependent( userSetting );
					}
				}

				GUILayout.Space( 5 );
				GUILayout.Label( "Time scale: " + Time.timeScale );
				Time.timeScale = GUILayout.HorizontalSlider( Time.timeScale, 0f, 2f );

			}

		}
		GUILayout.EndVertical();

		GUILayout.EndArea();

		GUI.enabled = true;

	}

}
