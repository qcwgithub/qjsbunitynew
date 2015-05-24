using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

using DaikonForge.Tween;
using DaikonForge.Tween.Interpolation;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/DaikonForge Tween (Pro)/Examples/Scripts/MuchTweenSoWow.javascript")]
public class MuchTweenSoWow : MonoBehaviour
{

	public Transform doge;
	public TextMesh[] text;
	public TextMesh logo;

	void Start()
	{

		// Using a TweenTimeline allows us to specify the exact time that the
		// tweens should start. It is somewhat more flexible than using TweenGroup,
		// especially when you need both sequential and concurrent behavior.
		var timeline = new TweenTimeline();

		// NOTE: This tween could be more efficiently done with Transform.TweenScale(),
		// but we wanted to show an example of using Reflection to tween any 
		// property by name.
		var dogeScale = doge.transform
			.TweenProperty<Vector3>( "localScale" )
			.SetStartValue( Vector3.zero )
			.SetDuration( 0.5f )
			.SetDelay( 0.5f )
			.SetInterpolator( EulerInterpolator.Default )
			.SetEasing( TweenEasingFunctions.Spring );

		timeline.Add( 0, dogeScale );

		for( int i = 0; i < text.Length; i++ )
		{

			// Need to start with the TextMesh invisible (it is visible during design time for convenience)
			text[ i ].color = new Color( 1, 1, 1, 0 );

			var alphaTween = text[ i ].TweenAlpha()
				.SetAutoCleanup( true )
				.SetDuration( 0.33f )
				.SetStartValue( 0f )
				.SetEndValue( 1f );

			var rotTween = text[ i ].TweenRotation()
				.SetAutoCleanup( true )
				.SetEndValue( Vector3.zero )
				.SetDuration( 0.5f )
				.SetEasing( TweenEasingFunctions.Spring );

			// Note that we can add as many tweens as we like at the same 
			// time in the timeline. They will all run concurrently when 
			// that time is reached.
			timeline.Add( 1.5f + 0.75f * i, alphaTween, rotTween );

		}

		var cameraSlide = this.TweenPosition()
			.SetEndValue( transform.position + new Vector3( 0f, -1f, 0f ) )
			.SetDuration( 0.5f )
			.SetEasing( TweenEasingFunctions.EaseInOutQuad );

		var logoSlide = logo.TweenPosition()
			.SetStartValue( logo.transform.position - ( Vector3.up * 5 ) )
			.SetDuration( 1f )
			.SetEasing( TweenEasingFunctions.Bounce );

		var logoAlphaTween = logo.TweenAlpha()
				.SetStartValue( 0f )
				.SetEndValue( 1f )
				.SetDuration( 0.75f );

		// Need to start with the logo invisible (it is visible during design time for convenience)
		logo.color = new Color( 1, 1, 1, 0 );

		timeline.Add( 3.75f, cameraSlide );
		timeline.Add( 4f, logoSlide, logoAlphaTween );

		timeline.Play();

	}

}