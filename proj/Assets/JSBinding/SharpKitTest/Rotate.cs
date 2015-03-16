using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
    float speed = 0.1f;
	Transform mTrans;
	Vector3 vec;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		if (mTrans == null) {
			mTrans = this.transform;
			vec = Vector3.forward;
		}
        mTrans.Rotate(vec * speed);
    }

    void OnGUI()
    {
        GUILayout.TextArea("Click the big cube!");
    }

}
