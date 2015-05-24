/* Copyright 2014 Daikon Forge */
using UnityEngine;
using System.Collections;

#if !FREE_VERSION

namespace DaikonForge.Tween
{

	/// <summary>
	/// Interface to provide spline implementation
	/// </summary>
	public interface ISplineInterpolator
	{
		/// <summary>
		/// Interpolate between spline nodes A and B
		/// </summary>
		Vector3 Evaluate( Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t );
	}

}

#endif
