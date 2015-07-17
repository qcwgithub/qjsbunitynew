using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/PerformanceTest1.javascript")]
public class PerformanceTest1 : MonoBehaviour {

    Transform mTransform;
	void Start () {
        mTransform = transform;
	}
    void Test0()
    {
        var N = 10000000;

        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        var g = 0;
        var f = 0;
        for (var i = 0; i < N; i++)
        {
            g += 1;
            f += 1;
        }
        sw.Stop();
//         Debug.Log("loop count: " + N.ToString());
//         Debug.Log("calc result: " + (f + g).ToString());
        Debug.Log("test0 time: " + sw.ElapsedMilliseconds + " ms");
    }

    private void Test1()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Vector3 m;
        for (int i = 0; i < 2000; i++)
        {
            m = mTransform.position;

            mTransform.position = m;
        }
        Debug.Log("test1 time: " + sw.ElapsedMilliseconds + " ms");
    }

    private void Test2()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Vector3 m = mTransform.position;
        for (int i = 0; i < 2000; i++)
        {
            m = Vector3.Normalize(m);
        }
        Debug.Log("test2 time: " + sw.ElapsedMilliseconds + " ms");
    }

    private void Test3()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Vector3 m = mTransform.position;
        for (int i = 0; i < 2000; i++)
        {
            m.Normalize();
        }
        Debug.Log("test3 time: " + sw.ElapsedMilliseconds + " ms");
    }

    private void Test4()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Vector3 m = mTransform.position;
        for (int i = 0; i < 2000; i++)
        {
            mTransform.position = m;
        }
        Debug.Log("test4 time: " + sw.ElapsedMilliseconds + " ms");
    }

    private void Test5()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Vector3 m = Vector3.one;
        for (int i = 0; i < 2000; i++)
        {
            m = new Vector3(i, i, i);
        }
        Debug.Log("test5 time: " + sw.ElapsedMilliseconds + " ms");
    }

    void Test6()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        for (var i = 0; i < 50000; i++)
        {
            var go = new GameObject("init");
            GameObject.DestroyImmediate(go);
        }
        
        Debug.Log("test6 time: " + sw.ElapsedMilliseconds + " ms");
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Test0();
            Test1();
            Test2();
            Test3();
            Test4();
            Test5();
            Test6();
        }
	}
}
