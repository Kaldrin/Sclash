using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;





// BASTIEN BERNAND
// Reusable asset

// REQUIREMENTS
// InputSystem package

/// <summary>
/// This script allows to control a slider with inputs from controllers and stuff
/// </summary>

// UNITY 2020.3
[RequireComponent(typeof(Scrollbar))]
public class ControlScrollbar : MonoBehaviour
{
    [SerializeField] Scrollbar scrollbarToControl = null;
    [SerializeField] public PlayerControls playerControls = null;
    [SerializeField] float controlSpeedMultiplier = 0.004f;





    private void OnEnable()                                                                                                                                 // ON ENABLE
    {
        playerControls = GameManager.Instance.Controls;
        GetMissingCOmponents();
    }


    void Update()                                                                                                                                               // UPDATE
    {
        Debug.Log(playerControls.UI.Navigate.ReadValue<Vector2>());
        scrollbarToControl.value += playerControls.UI.Navigate.ReadValue<Vector2>().y * controlSpeedMultiplier;
    }






    void GetMissingCOmponents()                                                                                                                             // GET MISSING COMPONENTS
    {
        if (scrollbarToControl == null && GetComponent<Scrollbar>())
            scrollbarToControl = GetComponent<Scrollbar>();
    }



    // EDITOR
    private void OnDrawGizmosSelected()                                                                                                                         // ON DRAW GIZMOS SELECTED
    {
        GetMissingCOmponents();
    }
}
