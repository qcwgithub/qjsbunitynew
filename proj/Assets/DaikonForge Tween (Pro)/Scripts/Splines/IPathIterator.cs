/* Copyright 2014 Daikon Forge */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !FREE_VERSION

namespace DaikonForge.Tween
{

	public interface IPathIterator
	{
		Vector3 GetPosition( float time );
		Vector3 GetTangent( float time );
	}

}

#endif
