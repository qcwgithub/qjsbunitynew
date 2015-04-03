/* Copyright 2014 Daikon Forge */
using UnityEngine;
using System.Collections;

#if !FREE_VERSION

namespace DaikonForge.Tween
{

	/// <summary>
	/// Provides an implementation of Catmull-Rom splines
	/// </summary>
	public class CatmullRomSpline : ISplineInterpolator
	{

		public Vector3 Evaluate( Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t )
		{

			return 0.5f * ( ( 2 * b ) +
				( -a + c ) * t +
				( 2 * a - 5 * b + 4 * c - d ) * t * t +
				( -a + 3 * b - 3 * c + d ) * t * t * t );

		}

	}

}

#endif
