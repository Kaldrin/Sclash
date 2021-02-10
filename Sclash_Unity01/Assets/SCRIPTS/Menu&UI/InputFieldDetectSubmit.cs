using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;



// This script is to put on an Input Field, to detect when a key is pressed and send to it the EndEdit event
[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_InputField))]
public class InputFieldDetectSubmit : MonoBehaviour
{
    [SerializeField] TMP_InputField tmpInputField = null;






    // Update is called once per frame
    void Update()                                                                                           // UPDATE
    {
        if (enabled && isActiveAndEnabled)
        {
            if (Input.GetButtonDown("Submit"))
            {
                // Check if Input Field is assigned
                if (tmpInputField == null)
                    if (GetComponent<TMP_InputField>())
                        tmpInputField = GetComponent<TMP_InputField>();

                Debug.Log("Submit");
                //tmpInputField.ProcessEvent()
                try
                {
                    tmpInputField.onEndEdit.Invoke("myNewText");
                }
                catch
                {

                }
            }

        }
    }










    // EDITOR
    private void OnDrawGizmosSelected()                                                                         // ON DRAW GIZMOS
    {
        if (tmpInputField == null)
            if (GetComponent<TMP_InputField>())
                tmpInputField = GetComponent<TMP_InputField>();
    }
}
