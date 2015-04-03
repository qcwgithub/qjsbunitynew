/* Copyright 2013-2014 Daikon Forge */
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using DaikonForge.Tween;

namespace DaikonForge.Tween.Components
{

	/// <summary>
	/// Specifies how the StartValue property of a tween component will be evaluated
	/// </summary>
	public enum TweenStartValueType : int
	{

		/// <summary>
		/// The value will be treated as an absolute value
		/// </summary>
		Absolute = 0,

		/// <summary>
		/// The value will be synched with the animated object when the tween is started
		/// </summary>
		SyncOnRun

	}

	/// <summary>
	/// Specifies how the EndValue property of a tween component will be evaluated
	/// </summary>
	public enum TweenEndValueType : int
	{

		/// <summary>
		/// The value will be treated as an absolute value
		/// </summary>
		Absolute = 0,

		/// <summary>
		/// The value will be synched with the animated object when the tween is started
		/// </summary>
		SyncOnRun,

		/// <summary>
		/// The value will be treated as being relative to the StartValue property
		/// </summary>
		Relative

	}

}
