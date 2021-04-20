using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;





// HEADER
// For Sclash

// REQUIREMENTS
// InputSystem package
// Player script
// GameManager script (Single instance)
// InputManager script (Single instance)

/// <summary>
/// 
/// </summary>

// VERSION
// Made for Unity 2019.4.14
public class PlayerControlCenter : MonoBehaviour
{
    PlayerInput m_playerInput;
    [SerializeField] int m_playerIndex;
    float m_DashOrientation = 0f;

    PlayerControls controls;
    EventSystemControl UIcontrols;

    [SerializeField] Player attachedPlayer;







    #region FUNCTIONS
    private void Awake()                                                                                                                            // AWAKE
    {
        UIcontrols = new EventSystemControl();
        UIcontrols.UI.Enable();
    }


    private void Start()                                                                                                                            // START
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_playerIndex = m_playerInput.playerIndex;
        if (attachedPlayer == null)
            attachedPlayer = GameManager.Instance.playersList[m_playerIndex].GetComponent<Player>();

        UIcontrols.UI.Submit.started += (ctx) => OnSubmit(ctx);
        UIcontrols.UI.Submit.canceled += (ctx) => OnSubmit(ctx);
    }


    private void Update()                                                                                                                           // UPDATE
    {
        if (isActiveAndEnabled && enabled)
        {
            

            if (attachedPlayer == null)
                if (GameManager.Instance.playersList.Count != 0)
                    attachedPlayer = GameManager.Instance.playersList[m_playerIndex].GetComponent<Player>();

            
        }
    }

    private void LateUpdate()
    {
        if (isActiveAndEnabled && enabled)
            if (InputManager.Instance)
            {
                if (InputManager.Instance.playerInputs[m_playerIndex].anyKeyDown && InputManager.Instance.playerInputs[m_playerIndex].anyKey)
                    InputManager.Instance.playerInputs[m_playerIndex].anyKeyDown = false;

                InputManager.Instance.playerInputs[m_playerIndex].pauseUp = false;
            }
    }






    public void OnHorizontal(InputAction.CallbackContext ctx)                                                                                       // ON HORIZONTAL
    {
        InputManager.Instance.playerInputs[m_playerIndex].horizontal = ctx.ReadValue<float>();
    }


    public void OnVertical(InputAction.CallbackContext ctx)                                                                                         // ON VERTICAL
    {
        InputManager.Instance.playerInputs[m_playerIndex].vertical = ctx.ReadValue<float>();
    }


    public void OnAttack(InputAction.CallbackContext ctx)                                                                                           // ON ATTACK
    {
        if (ctx.started)
        {
            InputManager.Instance.playerInputs[m_playerIndex].attack = true;
            InputManager.Instance.playerInputs[m_playerIndex].attackDown = true;
        }
        else if (ctx.performed)
            InputManager.Instance.playerInputs[m_playerIndex].attackDown = false;
        else if (ctx.canceled)
        {
            InputManager.Instance.playerInputs[m_playerIndex].attack = false;
            InputManager.Instance.playerInputs[m_playerIndex].attackDown = false;
        }

        OnAnyKey(ctx);
    }


    public void OnParry(InputAction.CallbackContext ctx)                                                                                                // ON PARRY
    {
        if (ctx.started)
        {
            InputManager.Instance.playerInputs[m_playerIndex].parry = true;
            InputManager.Instance.playerInputs[m_playerIndex].parryDown = true;
        }
        else if (ctx.canceled)
        {
            InputManager.Instance.playerInputs[m_playerIndex].parry = false;
            InputManager.Instance.playerInputs[m_playerIndex].parryDown = false;
        }

        OnAnyKey(ctx);
    }


    public void OnPommel(InputAction.CallbackContext ctx)                                                                                               // ON POMMEL
    {
        InputManager.Instance.playerInputs[m_playerIndex].kick = ctx.performed;
        OnAnyKey(ctx);
    }


    public void QuickDash(InputAction.CallbackContext ctx)                                                                                               // QUICK DASH
    {
        if (ctx.performed)
            attachedPlayer.DashInput(ctx.ReadValue<float>(), true);

        if (ctx.canceled)
            attachedPlayer.DashInput(0f, true);

        if (InputManager.Instance)
            InputManager.Instance.playerInputs[m_playerIndex].dash = ctx.ReadValue<float>();
    }


    public void OnDash(InputAction.CallbackContext ctx)                                                                                                 // ON DASH
    {
        if (ctx.canceled)
        {
            
            attachedPlayer.DashInput(0f, false);
            return;
        }

        attachedPlayer.DashInput(ctx.ReadValue<float>(), false);


        if (InputManager.Instance)
            InputManager.Instance.playerInputs[m_playerIndex].dash = ctx.ReadValue<float>();
    }


    public void OnPause(InputAction.CallbackContext ctx)                                                                                                // ON PAUSE
    {
        if (ctx.started)
            if (InputManager.Instance)
                InputManager.Instance.playerInputs[m_playerIndex].pauseUp = false;

        if (ctx.canceled)
            if (InputManager.Instance)
                InputManager.Instance.playerInputs[m_playerIndex].pauseUp = true;
    }


    public void OnScore(InputAction.CallbackContext ctx)                                                                                                // ON SCORE
    {
        if (ctx.started)
            if (InputManager.Instance)
            {
                InputManager.Instance.playerInputs[m_playerIndex].score = true;
                InputManager.Instance.playerInputs[m_playerIndex].scoreUp = true;
                InputManager.Instance.scoreInput = true;
            }

        if (ctx.canceled)
            if (InputManager.Instance)
            {
                InputManager.Instance.playerInputs[m_playerIndex].score = false;
                InputManager.Instance.playerInputs[m_playerIndex].scoreUp = false;
                InputManager.Instance.scoreInput = false;
            }
    }


    public void OnJump(InputAction.CallbackContext ctx)                                                                                                     // ON JUMP
    {
        if (InputManager.Instance)
            InputManager.Instance.playerInputs[m_playerIndex].jump = ctx.started;
    }


    public void OnSneath(InputAction.CallbackContext ctx)                                                                                                   // ON SNEATH
    {
        if (ctx.started)
            if (InputManager.Instance)
                InputManager.Instance.playerInputs[m_playerIndex].battleSneathDraw = true;
        if (ctx.canceled)
            if (InputManager.Instance)
                InputManager.Instance.playerInputs[m_playerIndex].battleSneathDraw = false;
    }


    public void OnAnyKey(InputAction.CallbackContext ctx)                                                                                                     // ON ANY KEY
    {
        if (GameManager.Instance.gameState == GameManager.GAMESTATE.paused)
            return;

        if (ctx.started)
        {
            if (InputManager.Instance)
            {
                InputManager.Instance.playerInputs[m_playerIndex].anyKeyDown = true;

                InputManager.Instance.playerInputs[m_playerIndex].anyKey = true;
            }
        }
        else if (ctx.canceled)
            if (InputManager.Instance)
            {
                InputManager.Instance.playerInputs[m_playerIndex].anyKey = false;
                InputManager.Instance.playerInputs[m_playerIndex].anyKeyDown = false;
            }
    }


    public void OnSubmit(InputAction.CallbackContext ctx)                                                                                                       // ON SUBMIT
    {
        if (ctx.started)
        {
            InputManager.Instance.submitInput = true;
            InputManager.Instance.submitInputUp = false;
        }
        if (ctx.canceled)
        {
            InputManager.Instance.submitInput = false;
            InputManager.Instance.submitInputUp = true;
        }
    }


    public void OnRestart(InputAction.CallbackContext ctx)                                                                                                      // ON RESTART
    {
        Debug.Log("Restart");
    }

    public void OnSkip(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            if (InputManager.Instance)
                InputManager.Instance.skip = true;
        if (ctx.canceled)
            if (InputManager.Instance)
                InputManager.Instance.skip = false;
    }

    public void OnDeviceLost(PlayerInput input)                                                                                                                 // ON DEVICE LOST
    {
        if (InputManager.Instance)
            InputManager.Instance.LostDevice(input);
    }


    public void OnDeviceRegained(PlayerInput input)                                                                                                             // ON DEVICE REGAINED
    {
        if (InputManager.Instance)
            InputManager.Instance.RegainedDevice(input);
    }
    
    
    





    void RemoveWarnings()
    {
        m_DashOrientation += m_DashOrientation;
    }
    #endregion
}