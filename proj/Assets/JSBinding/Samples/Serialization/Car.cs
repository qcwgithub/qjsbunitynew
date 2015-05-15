using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

// demonstrate how to refer other scripts, GameObjects, components
// and array

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Serialization/Car.javascript")]
public class Car : MonoBehaviour {

    public Wheel[] wheels; // left-front, right-front
    public GameObject[] goWheels; // left-back, right-back

	// Use this for initialization
	void Start () 
    {
        foreach (var w in wheels)
        {
            if (w != null)
            {
                w.setSpeed(Random.Range(1f, 4f));
            }
        }

        foreach (var go in goWheels)
        {
            if (go != null)
            {
                Wheel w = go.GetComponent<Wheel>();
                if (w != null)
                {
                    w.setSpeed(Random.Range(1f, 4f));
                }
            }
        }
	}
}
