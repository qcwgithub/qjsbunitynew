/* Copyright 2014 Daikon Forge */
using UnityEngine;
using System.Collections;

#if !FREE_VERSION

namespace DaikonForge.Tween
{

	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using DaikonForge.Editor;

	[ExecuteInEditMode]
	public class SplineNode : MonoBehaviour
	{

		public void OnDestroy()
		{

			if( Application.isPlaying || transform.parent == null )
				return;

			var parent = transform.parent.GetComponent<SplineObject>();
			if( parent == null )
				return;

			parent.ControlPoints.Remove( this.transform );

		}

#if UNITY_EDITOR

		public void OnDrawGizmos()
		{

			var savedColor = Gizmos.color;

			var active = UnityEditor.Selection.gameObjects;
			var isSelected = false;

			for( int i = 0; i < active.Length; i++ )
			{

				if( ( this.gameObject == active[ i ] ) || this.transform.IsChildOf( active[ i ].transform ) )
				{
					isSelected = true;
					break;
				}

			}

			Gizmos.color = isSelected ? Color.red : Color.white;
			Gizmos.DrawSphere( transform.position, 0.2f );
			Gizmos.color = savedColor;

		}

#endif

	}

}

#endif
