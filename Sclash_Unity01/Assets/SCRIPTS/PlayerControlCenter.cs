using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlCenter : MonoBehaviour
{
    PlayerInput m_playerInput;
    [SerializeField]
    int m_playerIndex;
    float m_DashOrientation = 0f;

    PlayerControls controls;
    EventSystemControl UIcontrols;

    Player attachedPlayer;

    private void Awake()
    {
        UIcontrols = new EventSystemControl();
        UIcontrols.UI.Enable();
    }

    private void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_playerIndex = m_playerInput.playerIndex;
        attachedPlayer = GameManager.Instance.playersList[m_playerIndex].GetComponent<Player>();

        UIcontrols.UI.Submit.started += (ctx) => OnSubmit(ctx);
        UIcontrols.UI.Submit.canceled += (ctx) => OnSubmit(ctx);
    }

    private void Update()
    {
        InputManager.Instance.playerInputs[m_playerIndex].pauseUp = false;
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

        OnAnyKey(ctx);
    }

    public void OnParry(InputAction.CallbackContext ctx)
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

    public void OnPommel(InputAction.CallbackContext ctx)
    {
        InputManager.Instance.playerInputs[m_playerIndex].kick = ctx.performed;
        OnAnyKey(ctx);
    }


    public void QuickDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            attachedPlayer.DashInput(ctx.ReadValue<float>(), true);
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            attachedPlayer.DashInput(Mathf.Sign(ctx.ReadValue<float>()), false);
        }
        else if (ctx.canceled)
        {
            attachedPlayer.DashInput(ctx.ReadValue<float>(), false);
        }
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            InputManager.Instance.playerInputs[m_playerIndex].pauseUp = false;
        }

        if (ctx.canceled)
        {
            InputManager.Instance.playerInputs[m_playerIndex].pauseUp = true;
        }
    }

    public void OnScore(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            InputManager.Instance.playerInputs[m_playerIndex].scoreUp = true;
            InputManager.Instance.scoreInput = true;
        }

        if (ctx.canceled)
        {
            InputManager.Instance.playerInputs[m_playerIndex].scoreUp = false;
            InputManager.Instance.scoreInput = false;
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        InputManager.Instance.playerInputs[m_playerIndex].jump = ctx.started;
    }

    public void OnSneath(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            InputManager.Instance.playerInputs[m_playerIndex].battleSneathDraw = true;
        if (ctx.canceled)
            InputManager.Instance.playerInputs[m_playerIndex].battleSneathDraw = false;
    }

    public void OnAnyKey(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.gameState == GameManager.GAMESTATE.paused)
            return;

        if (ctx.started)
        {
            InputManager.Instance.playerInputs[m_playerIndex].anyKey = true;
            InputManager.Instance.playerInputs[m_playerIndex].anyKeyDown = true;
        }
        else if (ctx.canceled)
        {
            InputManager.Instance.playerInputs[m_playerIndex].anyKey = false;
            InputManager.Instance.playerInputs[m_playerIndex].anyKeyDown = false;
        }
    }

    public void OnSubmit(InputAction.CallbackContext ctx)
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

    public void OnRestart(InputAction.CallbackContext ctx)
    {
        Debug.Log("Restart");
    }

    public void OnDeviceLost(PlayerInput input)
    {
        InputManager.Instance.LostDevice(input);
    }

    public void OnDeviceRegained(PlayerInput input)
    {
        InputManager.Instance.RegainedDevice(input);
    }

}