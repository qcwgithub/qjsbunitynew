using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

using DaikonForge.Tween;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/DaikonForge Tween (Pro)/Examples/Scripts/TestEasingFunctions.javascript")]
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