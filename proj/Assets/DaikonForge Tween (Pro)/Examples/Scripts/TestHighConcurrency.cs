using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

using DaikonForge.Tween;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/DaikonForge Tween (Pro)/Examples/Scripts/TestHighConcurrency.javascript")]
public class TestHighConcurrency : MonoBehaviour
{

	public float BounceRange = 9f;

	private Tween<Vector3> tweenRotation;
	private Tween<Vector3> tweenPosition;
	private Tween<Vector3> tweenPunchScale;

	void OnEnable()
	{

		tweenPosition =
			this.TweenPosition()
			.SetStartValue( transform.position )
			.SetEndValue( transform.position - ( Vector3.up * BounceRange ) )
			.SetDuration( 1f )
			.SetEasing( TweenEasingFunctions.EaseInOutBack )
			.SetDelay( Random.Range( 0.1f, 0.5f ) );
            
      
          

		tweenRotation =
			this.TweenRotation( false )
			.SetEndValue( new Vector3( 0, 360, 0 ) )
			.SetDuration( 0.25f )
			.SetDelay( Random.Range( 0f, 0.5f ) );

		tweenPunchScale =
			this.TweenScale()
			.SetStartValue( transform.localScale )
			.SetEndValue( transform.localScale * 2f )
			.SetDelay( Random.Range( 0.25f, 0.75f ) )
			.SetEasing( TweenEasingFunctions.Punch );

		tweenPosition
			.Play()
			.Chain( tweenRotation )
			.Wait( 0.5f )
			.Chain( tweenPunchScale )
			.Wait( 0.5f )
			.Chain( tweenPosition, () =>
			{
				tweenPosition
					.SetLoopType( TweenLoopType.Pingpong )
					.SetDelay( 1.33f )
					.SetDuration( 1.25f )
					.ReversePlayDirection()
					.Play();
			} );

	}

}