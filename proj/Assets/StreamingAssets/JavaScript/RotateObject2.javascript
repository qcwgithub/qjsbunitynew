var mTrans = undefined
var rotateVar = undefined
var fAccum = 0
function Update()
{
    // here
    // 'this' means a GameObject

    // rotate gameobject
    if (mTrans === undefined)
    {
        mTrans = transform;
        rotateVar = new Vector3(0.5, 0.5, 0);
    }
    mTrans.Rotate(rotateVar);

    // destroy this GameObject after 10 seconds
    fAccum += Time.deltaTime
    if (fAccum > 10)
    {
        // JavaScript has 'Object' as key word
        // so UnityEngine.Object is renamed to 'UnityObject'
        UnityObject.Destroy(this);
    }
}