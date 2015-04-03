using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using DaikonForge.Tween;

public class TestSlinky : MonoBehaviour
{

	private List<Tween<Vector3>> tweens = new List<Tween<Vector3>>();

	void OnEnable()
	{

		var rings = GameObject.FindObjectsOfType<SpriteRenderer>() as SpriteRenderer[];
		Array.Sort( rings, ( lhs, rhs ) => lhs.sortingOrder.CompareTo( rhs.sortingOrder ) );
		
		for( int i = 0; i < rings.Length; i++ )
		{

			var obj = rings[ i ];

			var tween = obj.TweenPosition()
				.SetDelay( i * 0.02f )
				.SetDuration( 0.5f )
				.SetEasing( TweenEasingFunctions.Spring );

			// If we don't call Play(), then tween.CurrentValue may not contain
			// a valid value when Update detects the first mouse click
			tween.Play();

			tweens.Add( tween );

		}

	}

	void OnGUI()
	{
		var rect = new Rect( 10, 10, 600, 25 );
		GUI.Label( rect, "Click anywhere with the mouse to move rings. SPACE: Slow, ENTER: Fast" );
	}

	void Update()
	{

		if( Input.GetKeyDown( KeyCode.Space ) )
		{
			Debug.Log( "Time scale: SLOW" );
			Time.timeScale = 0.15f;
		}
		else if( Input.GetKeyDown( KeyCode.Return ) )
		{
			Debug.Log( "Time scale: NORMAL" );
			Time.timeScale = 1f;
		}

		if( !Input.GetMouseButtonDown( 0 ) )
			return;

		var plane = new Plane( Vector3.back, Vector3.zero );
		var ray = Camera.main.ScreenPointToRay( Input.mousePosition );

		// Find the 3D world position corresponding to the mouse click position 
		float distance = 0;
		plane.Raycast( ray, out distance );
		var endPosition = ray.GetPoint( distance );

		// Retarget all tweens to arrive at the new position 
		for( int i = 0; i < tweens.Count; i++ )
		{
			var tween = tweens[ i ];
			tween.SetStartValue( tween.CurrentValue ).SetEndValue( endPosition ).Play();
		}

	}

}
