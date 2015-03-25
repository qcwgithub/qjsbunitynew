using UnityEngine;
using System.Collections;
using System.Text;

public class QiucwAttribute : MonoBehaviour {

    public int age;
    public string name;
    public enum SEX { MALE, FEMALE}
    public SEX sex;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Print() {
        Debug.Log("QiucwAttribute: " + age.ToString() + " " + name.ToString() + " " + sex.ToString());
    }
}
