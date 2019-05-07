using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
