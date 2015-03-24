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
    public GameObject[] arrGameObject;
    public Vector3[] arrVec;

	// Use this for initialization
	void Start () 
    {
        // otuput enum
        Debug.Log(enum1.ToString());

        // output float
        Debug.Log(floatV.ToString());

        // output int[]
        string s = "arrIntV = ";
        for (var i = 0; i < arrIntV.Length; i++) {
            s += arrIntV[i].ToString() + ", ";
        }
        Debug.Log(s);

        // output GameObject[]
        s = "arrGameObject = ";
        for (var i = 0; i < arrGameObject.Length; i++)
        {
            s += arrGameObject[i].name + ", ";
        }
        Debug.Log(s);

        // output Vector3[]
        s = "arrVec = ";
        for (var i = 0; i < arrVec.Length; i++)
        {
            s += arrVec[i].ToString() + ", ";
        }
        Debug.Log(s);
	}

    // Update is called once per frame
    void Update()
    {
	
	}
}
