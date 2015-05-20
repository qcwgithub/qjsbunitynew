using UnityEngine;
using System.Collections;

using SharpKit.JavaScript;
[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/2DPlatformer/Scripts/Score.javascript")]
public class Score : MonoBehaviour
{
	public int score = 0;					// The player's score.


	private PlayerControl playerControl;	// Reference to the player control script.
	private int previousScore = 0;			// The score in the previous frame.


	void Awake ()
	{
		// Setting up the reference.
		playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
	}


	void Update ()
	{
		// Set the score text.
		guiText.text = "Score: " + score;

		// If the score has changed...
		if(previousScore != score)
        {
            // ... play a taunt.
            //playerControl.StartCoroutine(playerControl.Taunt());
            playerControl.PreTaunt();
        }

		// Set the previous score to this frame's score.
		previousScore = score;
	}

}
