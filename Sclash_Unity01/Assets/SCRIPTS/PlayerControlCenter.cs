using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlCenter : MonoBehaviour
{
    PlayerControls controls;

    float move;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Duel.Horizontal.performed += ctx => Move(ctx.ReadValue<float>());
        controls.Duel.Horizontal.canceled += ctx => Move(0);
    }

    private void OnEnable()
    {
        controls.Duel.Enable();
    }

    private void OnDisable()
    {
        controls.Duel.Disable();
    }

    private void Move(float move)
    {
    }
}