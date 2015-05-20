using UnityEngine;
using System.Collections;

using SharpKit.JavaScript;
[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/2DPlatformer/Scripts/FollowPlayer.javascript")]
public class FollowPlayer : MonoBehaviour
{
	public Vector3 offset;			// The offset at which the Health Bar follows the player.
	
	private Transform player;		// Reference to the player.


	void Awake ()
	{
		// Setting up the reference.
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update ()
	{
		// Set the position to the player's position with the offset.
		transform.position = player.position + offset;
	}
}
