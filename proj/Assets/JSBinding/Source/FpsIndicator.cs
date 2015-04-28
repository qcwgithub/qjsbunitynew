using UnityEngine;
using System.Collections;

public class FpsIndicator : MonoBehaviour {

    int frameCount = 0;
    float dt = 0f;
    float fps = 0f;
    float updateRate = 4f;  // 4 updates per sec.
    public float y = 20f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1f / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1f / updateRate;
        }
    }
    void OnGUI()
    {
        //GUI.TextArea("FPS: " + fps.ToString());
        GUI.TextArea(new Rect(0, y, 100, 20), "FPS: " + fps.ToString());
    }
}
