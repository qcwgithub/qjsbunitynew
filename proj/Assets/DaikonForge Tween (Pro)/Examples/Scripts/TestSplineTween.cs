using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

using DaikonForge.Tween;


#if !FREE_VERSION
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/DaikonForge Tween (Pro)/Examples/Scripts/TestSplineTween.javascript")]
public class TestSplineTween : MonoBehaviour
{

	public SplineObject Spline;

	public float Duration = 5f;
	public EasingType Easing = EasingType.Linear;
	public TweenLoopType Loop = TweenLoopType.Loop;


    public Transform target;

	void Start()
	{

	    if (Spline == null)
	    {

	       GameObject obj=  new GameObject("path");
	        obj.transform.position = transform.position;
            Spline = obj.AddComponent<SplineObject>();
	       

	        Spline.AddNode(transform.position);

	        Vector3 mid = (target.position - transform.position)/2+transform.position;
          //  float angle = Mathf.Acos(Vector3.Dot(transform.position.normalized, target.position.normalized)) * Mathf.Rad2Deg;
	        var len = (target.position - transform.position).magnitude;

       
          Vector3 ddd = Quaternion.Euler(0, 0, 90) * (target.position - mid).normalized * Mathf.Clamp(len / 2f, 1, 5);




            Spline.AddNode(mid + ddd);
	        Spline.AddNode(target.position);
	    }


	   var tp=	this.TweenPath( Spline.Spline )
			.SetDuration( Duration )
			.SetEasing( TweenEasingFunctions.GetFunction( Easing ) )
			.SetLoopType( Loop )
			.SetTimeScaleIndependent( true )
			.Play();
        tp.OnCompleted(onTweenComplete);
        

	}

    private void onTweenComplete(TweenBase sender)
    {
        Debug.Log("finish");
    }

    
}

#endif
