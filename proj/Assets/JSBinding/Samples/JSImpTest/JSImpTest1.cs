using SharpKit.JavaScript;
using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/JSImpTest/JSImpTest1.javascript")]
public class JSImpTest1 : MonoBehaviour 
{

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/JSImpTest/Person.javascript")]
    public class Person
    {
        public string firstName;
        public string lastName;
        public string age;
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("firstName", "De Hua");
            dict.Add("lastName", "Liu");
            dict.Add("age", "55");
            Person person = XmlParser.ComvertType<Person>(dict);
            Debug.Log(new StringBuilder().AppendFormat("{0} {1}, {2}", person.lastName, person.firstName, person.age));
        }
	}
}
