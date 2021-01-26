using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlCenter : MonoBehaviour
{
    PlayerInput m_playerInput;
    [SerializeField]
    int m_playerIndex;

    PlayerControls controls;

    private void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_playerIndex = m_playerInput.playerIndex;
        /*controls = new PlayerControls();
        controls.Enable();

        //Show score
        controls.Duel.Score.performed += ctx => InputManager.Instance.scoreInput = true;
        controls.Duel.Score.canceled += ctx => InputManager.Instance.scoreInput = false;

        //Manage horizontal input            
        controls.Duel.Horizontal.performed += ctx => { InputManager.Instance.playerInputs[m_playerIndex].horizontal = ctx.ReadValue<float>(); };
        controls.Duel.Horizontal.canceled += ctx => { InputManager.Instance.playerInputs[m_playerIndex].horizontal = 0; };

        //Manage vertical input
        controls.Duel.Vertical.performed += ctx => { InputManager.Instance.playerInputs[m_playerIndex].vertical = ctx.ReadValue<float>(); };
        controls.Duel.Vertical.canceled += ctx => { InputManager.Instance.playerInputs[m_playerIndex].vertical = 0; };

        //Manage Attack inputs
        controls.Duel.Attack.started += ctx => { InputManager.Instance.playerInputs[m_playerIndex].attack = true; InputManager.Instance.playerInputs[m_playerIndex].attackDown = true; };
        controls.Duel.Attack.performed += ctx => { InputManager.Instance.playerInputs[m_playerIndex].attackDown = false; };
        controls.Duel.Attack.canceled += ctx => { InputManager.Instance.playerInputs[m_playerIndex].attack = false; InputManager.Instance.playerInputs[m_playerIndex].attackDown = false; };


        //Manage pommel inputs
        controls.Duel.Pommel.started += ctx => { InputManager.Instance.playerInputs[m_playerIndex].kick = true; };
        controls.Duel.Pommel.canceled += ctx => { InputManager.Instance.playerInputs[m_playerIndex].kick = false; };

        //Manage parry inputs
        controls.Duel.Parry.started += ctx => { InputManager.Instance.playerInputs[m_playerIndex].parry = true; InputManager.Instance.playerInputs[m_playerIndex].parryDown = true; };
        controls.Duel.Parry.performed += ctx => { InputManager.Instance.playerInputs[m_playerIndex].parryDown = false; };
        controls.Duel.Parry.canceled += ctx => { InputManager.Instance.playerInputs[m_playerIndex].parry = false; InputManager.Instance.playerInputs[m_playerIndex].parryDown = false; };

        //Manage dash inputs
        float m_DashOrientation = 0f;
        controls.Duel.Dash.started += ctx => { m_DashOrientation = Mathf.Sign(ctx.ReadValue<float>()); };
        controls.Duel.Dash.performed += ctx => { InputManager.Instance.playerInputs[m_playerIndex].dash = m_DashOrientation; };
        controls.Duel.Dash.canceled += ctx => { InputManager.Instance.playerInputs[m_playerIndex].dash = 0; };

        //Manage Jump inputs
        controls.Duel.Jump.started += ctx => { InputManager.Instance.playerInputs[m_playerIndex].jump = true; };
        controls.Duel.Jump.canceled += ctx => { InputManager.Instance.playerInputs[m_playerIndex].jump = false; };*/
    }


    public void OnHorizontal(InputAction.CallbackContext ctx)
    {
        InputManager.Instance.playerInputs[m_playerIndex].horizontal = ctx.ReadValue<float>();
    }

    public void OnVertical(InputAction.CallbackContext ctx)
    {
        InputManager.Instance.playerInputs[m_playerIndex].vertical = ctx.ReadValue<float>();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            InputManager.Instance.playerInputs[m_playerIndex].attack = true;
            InputManager.Instance.playerInputs[m_playerIndex].attackDown = true;
        }
        else if (ctx.performed)
        {
            InputManager.Instance.playerInputs[m_playerIndex].attackDown = false;
        }
        else if (ctx.canceled)
        {
            InputManager.Instance.playerInputs[m_playerIndex].attack = false;
            InputManager.Instance.playerInputs[m_playerIndex].attackDown = false;
        }
    }

    public void OnParry(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            InputManager.Instance.playerInputs[m_playerIndex].parry = true;
            InputManager.Instance.playerInputs[m_playerIndex].parryDown = true;
        }
        else if (ctx.performed)
        {
            InputManager.Instance.playerInputs[m_playerIndex].parryDown = false;
        }
        else if (ctx.canceled)
        {
            InputManager.Instance.playerInputs[m_playerIndex].parry = false;
            InputManager.Instance.playerInputs[m_playerIndex].parryDown = false;
        }
    }

    public void OnPommel(InputAction.CallbackContext ctx)
    {
        InputManager.Instance.playerInputs[m_playerIndex].kick = ctx.performed;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        float m_DashOrientation = 0f;
        if (ctx.started)
            m_DashOrientation = Mathf.Sign(ctx.ReadValue<float>());
        else if (ctx.performed)
            InputManager.Instance.playerInputs[m_playerIndex].dash = m_DashOrientation;
        else if (ctx.canceled)
            InputManager.Instance.playerInputs[m_playerIndex].dash = 0;
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {

    }

    public void OnScore(InputAction.CallbackContext ctx)
    {
        InputManager.Instance.scoreInput = ctx.started;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        InputManager.Instance.playerInputs[m_playerIndex].jump = ctx.started;
    }

    public void OnAnyKey(InputAction.CallbackContext ctx)
    {
        InputManager.Instance.playerInputs[m_playerIndex].anyKey = ctx.started;
    }
}