using UnityEngine;
using System.Collections;

using DaikonForge.Tween;

public class TestEasingFunctions : MonoBehaviour
{

	public EasingType EaseType;

	void Start()
	{

		TweenEasingCallback func = TweenEasingFunctions.GetFunction( this.EaseType );

		transform.TweenPosition()
			.SetEndValue( transform.position + ( Vector3.right * 9f ) )
			.SetDelay( 0.5f, false )
			.SetDuration( 1.33f )
			.SetEasing( func )
			.SetLoopType( TweenLoopType.Loop )
			.Play();

	}

}