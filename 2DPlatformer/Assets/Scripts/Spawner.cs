using UnityEngine;
using System.Collections;

using SharpKit.JavaScript;

[JsType(JsMode.Clr, "Spawner.javascript")]
public class Spawner : MonoBehaviour
{
	public float spawnTime = 5f;		// The amount of time between each spawn.
	public float spawnDelay = 3f;		// The amount of time before spawning starts.
	public GameObject[] enemies;		// Array of enemy prefabs.


	void Start ()
	{
		// Start calling the Spawn function repeatedly after a delay .
		// InvokeRepeating("Spawn", spawnDelay, spawnTime);
        _spawnTime = spawnTime;
        _spawnDelay = spawnDelay;
	}

    float _spawnTime;
    float _spawnDelay;

    void Update()
    {
        if (_spawnDelay > 0)
        {
            _spawnDelay -= Time.deltaTime;
            return;
        }
        if (_spawnTime > 0)
        {
            _spawnTime -= Time.deltaTime;
            if (_spawnTime <= 0)
            {
                _spawnTime = spawnTime;
                Spawn();
            }
        
        }
    }

	void Spawn ()
	{
		// Instantiate a random enemy.
		int enemyIndex = Random.Range(0, enemies.Length);
		Instantiate(enemies[enemyIndex], transform.position, transform.rotation);

		// Play the spawning effect from all of the particle systems.
		foreach(ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
		{
			p.Play();
		}
	}
}
