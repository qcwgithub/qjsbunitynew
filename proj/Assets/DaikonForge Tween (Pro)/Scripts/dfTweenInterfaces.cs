/* Copyright 2014 Daikon Forge */
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Interface for tween object that need to be updated on a per-frame basis when running
	/// </summary>
	// @private
	public interface ITweenUpdatable
	{
		TweenState State { get; }
		void Update();
	}

	/// <summary>
	/// Interface for objects which can be returned to an object pool when no longer needed
	/// </summary>
	// @private
	public interface IPoolableObject
	{
		void Release();
	}

}
