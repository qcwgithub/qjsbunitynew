using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

using DaikonForge.Tween;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/DaikonForge Tween (Pro)/Examples/Scripts/TestShakeObject.javascript")]
public class TestShakeObject : MonoBehaviour
{

	public Transform Camera;

	private TweenShake shake;
	private TweenBase fallTween;

	void Start()
	{

		shake = TweenShake.Obtain()
			.SetStartValue( Camera.position )
			.SetDuration( 1f )
			.SetShakeMagnitude( 0.25f )
			.SetShakeSpeed( 13f )
			.SetTimeScaleIndependent( true )
			.OnExecute( ( result ) => { Camera.position = new Vector3( result.x, result.y, Camera.position.z ); } );

		fallTween = this.TweenPosition()
			.SetStartValue( transform.position + Vector3.up * 5f )
			.SetEasing( TweenEasingFunctions.EaseInExpo )
			.SetDuration( 1f )
			.SetDelay( 0.25f )
			.SetTimeScaleIndependent( true )
			.OnCompleted( ( sender ) => { shake.Play(); } )
			.Play();

	}

	void OnGUI()
	{
		GUILayout.Label( " Press SPACE to restart " );
	}

	void Update()
	{

		if( Input.GetKeyDown( KeyCode.LeftArrow ) )
		{
			Debug.Log( "Time scale: SLOW" );
			Time.timeScale = 0.15f;
		}
		else if( Input.GetKeyDown( KeyCode.RightArrow ) )
		{
			Debug.Log( "Time scale: NORMAL" );
			Time.timeScale = 1f;
		}

		if( Input.GetKeyDown( KeyCode.Space ) )
		{
			fallTween.Stop().Rewind().Play();
		}

	}

}