using UnityEngine;
using System.Collections;

public class JSSerializationTest : MonoBehaviour 
{
    public enum TESTENUM
    {
        HELLO = 8,
        WORLD = 1985,
        APPLE = 1380
    }
    public TESTENUM enum1;
    public float floatV;
    public int[] arrIntV;

	// Use this for initialization
	void Start () {
        // test enum
        Debug.Log(enum1.ToString());
        Debug.Log(floatV.ToString());
        //Debug.Log(intV.ToString());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
