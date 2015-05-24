using SharpKit.JavaScript;
/* Copyright 2014 Daikon Forge */
using UnityEngine;
using System.Collections;

[AddComponentMenu( "Daikon Forge/Examples/General/FPS Counter" )]
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/DaikonForge Tween (Pro)/Examples/Scripts/TestFPS.javascript")]
public class TestFPS : MonoBehaviour
{

	public bool editorOnly = true;

	private const float UPDATE_INTERVAL = 0.2F;

	private string labelText = "00";
	private Color labelColor = Color.white;
	private GUIStyle labelStyle;

	private float accum = 0; 
	private int numFrames = 0; 
	private float timeleft; 
	private float lastFrameTime;

	void Start()
	{
		timeleft = UPDATE_INTERVAL;
	}

	void OnGUI()
	{

		if( editorOnly && !Application.isEditor )
			return;

		if( labelStyle == null )
		{
			labelStyle = new GUIStyle( "label" );
			labelStyle.alignment = TextAnchor.MiddleCenter;
		}

		var savedColor = GUI.color;

		var rect = new Rect( 0, 0, 100, 25 );

		GUI.color = Color.black;
		GUI.Box( rect, GUIContent.none );

		GUI.color = labelColor;
		labelStyle.normal.textColor = labelColor;
		GUI.Label( rect, labelText, labelStyle );

		GUI.color = savedColor;

	}

	void Update()
	{

		var realDeltaTime = Time.realtimeSinceStartup - lastFrameTime;
		lastFrameTime = Time.realtimeSinceStartup;

		if( Time.timeScale <= 0.01f )
		{
			labelColor = Color.yellow;
			labelText = "Pause";
			return;
		}
		//else if( Mathf.Abs( Time.timeScale - 1 ) > 0.1f )
		//{

		//	labelColor = Color.green;

		//	var fps = Mathf.CeilToInt( 1f / realDeltaTime );
		//	labelText = System.String.Format( "{0:F0} FPS", fps );

		//	return;

		//}

		timeleft -= realDeltaTime;
		accum += 1f / realDeltaTime;

		numFrames += 1;

		if( timeleft <= 0.0 )
		{

			var fps = Mathf.CeilToInt( accum / numFrames );
			labelText = System.String.Format( "{0:F0} FPS", fps );

			if( fps < 30 )
				labelColor = Color.yellow;
			else if( fps < 10 )
				labelColor = Color.red;
			else
				labelColor = Color.green;

			timeleft = UPDATE_INTERVAL;
			accum = 0.0F;
			numFrames = 0;

		}

	}

}