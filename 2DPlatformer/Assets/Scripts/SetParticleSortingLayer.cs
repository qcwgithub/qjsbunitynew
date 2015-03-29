using UnityEngine;
using System.Collections;

using SharpKit.JavaScript;

[JsType(JsMode.Clr, "SetParticleSortingLayer.javascript")]
public class SetParticleSortingLayer : MonoBehaviour
{
	public string sortingLayerName;		// The name of the sorting layer the particles should be set to.


	void Start ()
	{
		// Set the sorting layer of the particle system.
		particleSystem.renderer.sortingLayerName = sortingLayerName;
	}
}
