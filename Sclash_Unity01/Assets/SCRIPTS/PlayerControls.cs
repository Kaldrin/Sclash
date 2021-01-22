// GENERATED AUTOMATICALLY FROM 'Assets/Resources/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Duel"",
            ""id"": ""2f848ec9-4cdc-4793-bc23-178c7bf9f959"",
            ""actions"": [
                {
                    ""name"": ""Horizontal"",
                    ""type"": ""Value"",
                    ""id"": ""ecfc9c6c-ee25-42a3-85ae-aa53222a36c4"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""f0f20444-dae2-470f-911b-fef06aa160fa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Parry"",
                    ""type"": ""Button"",
                    ""id"": ""8ecf76c9-9aef-41d2-9509-3e8679e70749"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pommel"",
                    ""type"": ""Button"",
                    ""id"": ""675835d2-57b9-4aa4-a42f-20b6fb465d5f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Value"",
                    ""id"": ""5c56dba8-13ec-4540-aab0-9d81aa5f5195"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6e437161-26cd-4ff9-84bc-5b07f3e806c2"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""12a9a8e2-aa6c-4d7e-8737-29c654c342e2"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""299612e3-a795-41be-9480-4328d6a69f37"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""85ba6e0c-3937-4743-90b8-23f8aa96e966"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f3d22210-f5ed-4eb6-9c66-21fc532fe9d4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9cb2198b-4eb7-4830-aed4-56568fdd4c8f"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""51b84bc9-a8dc-42d4-8b2e-34414625b7f3"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Parry"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df9edd2b-089a-4467-af17-06f89d24b719"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Parry"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c74b938-512e-4078-a99e-0ae3efe2ada6"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Parry"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""84d98823-ab3d-4582-ba96-1f231954f32d"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pommel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cfc7a832-ab08-4203-b0b6-01c7db8c8dd4"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pommel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ba94faf2-b668-49a7-b3c4-4f9c883bdb8c"",
                    ""path"": ""<Gamepad>/rightStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Duel
        m_Duel = asset.FindActionMap("Duel", throwIfNotFound: true);
        m_Duel_Horizontal = m_Duel.FindAction("Horizontal", throwIfNotFound: true);
        m_Duel_Attack = m_Duel.FindAction("Attack", throwIfNotFound: true);
        m_Duel_Parry = m_Duel.FindAction("Parry", throwIfNotFound: true);
        m_Duel_Pommel = m_Duel.FindAction("Pommel", throwIfNotFound: true);
        m_Duel_Dash = m_Duel.FindAction("Dash", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Duel
    private readonly InputActionMap m_Duel;
    private IDuelActions m_DuelActionsCallbackInterface;
    private readonly InputAction m_Duel_Horizontal;
    private readonly InputAction m_Duel_Attack;
    private readonly InputAction m_Duel_Parry;
    private readonly InputAction m_Duel_Pommel;
    private readonly InputAction m_Duel_Dash;
    public struct DuelActions
    {
        private @PlayerControls m_Wrapper;
        public DuelActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Horizontal => m_Wrapper.m_Duel_Horizontal;
        public InputAction @Attack => m_Wrapper.m_Duel_Attack;
        public InputAction @Parry => m_Wrapper.m_Duel_Parry;
        public InputAction @Pommel => m_Wrapper.m_Duel_Pommel;
        public InputAction @Dash => m_Wrapper.m_Duel_Dash;
        public InputActionMap Get() { return m_Wrapper.m_Duel; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DuelActions set) { return set.Get(); }
        public void SetCallbacks(IDuelActions instance)
        {
            if (m_Wrapper.m_DuelActionsCallbackInterface != null)
            {
                @Horizontal.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnHorizontal;
                @Horizontal.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnHorizontal;
                @Horizontal.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnHorizontal;
                @Attack.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnAttack;
                @Parry.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnParry;
                @Parry.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnParry;
                @Parry.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnParry;
                @Pommel.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnPommel;
                @Pommel.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnPommel;
                @Pommel.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnPommel;
                @Dash.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnDash;
            }
            m_Wrapper.m_DuelActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Horizontal.started += instance.OnHorizontal;
                @Horizontal.performed += instance.OnHorizontal;
                @Horizontal.canceled += instance.OnHorizontal;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Parry.started += instance.OnParry;
                @Parry.performed += instance.OnParry;
                @Parry.canceled += instance.OnParry;
                @Pommel.started += instance.OnPommel;
                @Pommel.performed += instance.OnPommel;
                @Pommel.canceled += instance.OnPommel;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
            }
        }
    }
    public DuelActions @Duel => new DuelActions(this);
    public interface IDuelActions
    {
        void OnHorizontal(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnParry(InputAction.CallbackContext context);
        void OnPommel(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
    }
}
