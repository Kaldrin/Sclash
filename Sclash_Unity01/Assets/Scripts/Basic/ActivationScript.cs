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

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void Switch()
    {
        gameObject.SetActive(!isActiveAndEnabled);
    }
}
