using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastAndDestroy : MonoBehaviour {


    //Wait amount
    public float
       lastDuration;
    float
        spawnTimecode;


    //Parameters
    public bool
        onDestroySpawnObject;

    //Game object to spawn
    public GameObject 
        gameObjectToSpawn;

	// Use this for initialization
	void Start () {
        spawnTimecode = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time >= spawnTimecode + lastDuration)
        {
            if (onDestroySpawnObject)
                Instantiate(gameObjectToSpawn, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
        }
	}
}
