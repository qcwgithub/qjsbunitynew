using UnityEngine;
using System.Collections;

public class RotateObject2 : MonoBehaviour {

	Transform mTrans;
    Vector3 rotateVar;
    float fAccum = 0;
    void Update()
    {
        // here
        // 'this' means a GameObject

        // rotate gameobject
        if (mTrans == null)
        {
            mTrans = transform;
            rotateVar = new Vector3(0.5f, 0.5f, 0f);
        }
        mTrans.Rotate(rotateVar);

        // destroy this GameObject after 10 seconds
        fAccum += Time.deltaTime;
        if (fAccum > 10)
        {
            // JavaScript has 'Object' as key word
            // so UnityEngine.Object is renamed to 'UnityObject'
            Object.Destroy(this);
        }
    }
}
