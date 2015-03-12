var mTrans = undefined
var rotateVar = undefined

function Update()
{
    // rotate gameobject
	//if (UnityEngine.Object.op_Equality(this.mTrans, null)){
		this.mTrans = this.get_transform();
		this.rotateVar = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(0.5, 0.5, 0);
	//}
	this.mTrans.Rotate$$Vector3(this.rotateVar);
}
