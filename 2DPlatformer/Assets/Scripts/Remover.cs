using UnityEngine;
using System.Collections;

using SharpKit.JavaScript;

[JsType(JsMode.Clr, "Remover.javascript")]
public class Remover : MonoBehaviour
{
	public GameObject splash;


	void OnTriggerEnter2D(Collider2D col)
	{
		// If the player hits the trigger...
		if(col.gameObject.tag == "Player")
		{
			// .. stop the camera tracking the player
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().enabled = false;

			// .. stop the Health Bar following the player
			if(GameObject.FindGameObjectWithTag("HealthBar").activeSelf)
			{
				GameObject.FindGameObjectWithTag("HealthBar").SetActive(false);
			}

			// ... instantiate the splash where the player falls in.
			Instantiate(splash, col.transform.position, transform.rotation);
			// ... destroy the player.
			Destroy (col.gameObject);
			// ... reload the level.
			// StartCoroutine("ReloadGame");
            Pre_ReloadGame();
		}
		else
		{
			// ... instantiate the splash where the enemy falls in.
			Instantiate(splash, col.transform.position, transform.rotation);

			// Destroy the enemy.
			Destroy (col.gameObject);	
		}
	}

    void Update()
    {
        if (_reload > 0)
        {
            _reload -= Time.deltaTime;
            if (_reload <= 0)
            {
                ReloadGame();
            }
        }
    }

    float _reload = 0;
    void Pre_ReloadGame()
    {
        _reload = 2f;
    }

	void ReloadGame()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
}
