using UnityEngine;

// This script allows to access more activation functions for an object
public class ActivationScript : MonoBehaviour {


    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void Switch()
    {
        gameObject.SetActive(!isActiveAndEnabled);
    }
}
