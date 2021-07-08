using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// Reusable asset
// Bastien BERNAND

/// <summary>
/// Used to enable / disable another object with this one, acting like a state constraint
/// </summary>

// UNITY 2020
public class SendEnabledState : MonoBehaviour
{
    [SerializeField] string nameOfTheObjectToSendTo = "AmaterasuHair(Clone)";
    [SerializeField] public GameObject inheritingObject = null;



    void Start()
    {
        TryAndFindObject(nameOfTheObjectToSendTo);
    }

    private void OnEnable()
    {
        TryAndFindObject(nameOfTheObjectToSendTo);
        if (inheritingObject && !inheritingObject.activeInHierarchy)
            inheritingObject.SetActive(true);
    }

    private void OnDisable()
    {
        TryAndFindObject(nameOfTheObjectToSendTo);
        if (inheritingObject && inheritingObject.activeInHierarchy)
            inheritingObject.SetActive(false);
    }



    GameObject TryAndFindObject(string nameOfTheObject)
    {
        GameObject objectToFind = null;

        try
        {
            if (inheritingObject == null)
                objectToFind = GameObject.Find(nameOfTheObject);
            else
                objectToFind = inheritingObject;
        }
        catch
        {
            objectToFind = inheritingObject;
        }


        return objectToFind;
    }
}
