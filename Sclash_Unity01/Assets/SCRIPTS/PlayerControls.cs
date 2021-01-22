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
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AnyKey"",
                    ""type"": ""Button"",
                    ""id"": ""037904d4-99ff-4089-ad00-cc2017560d3e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""7bb3c27c-d4d9-40a4-9735-044e185edc04"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Score"",
                    ""type"": ""Button"",
                    ""id"": ""46a42fcd-33e5-40cf-ba8b-5b65eba8418f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""4d5ec2e8-df09-4527-a6db-f06ad59617e2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sneath/Draw"",
                    ""type"": ""Button"",
                    ""id"": ""006318a6-baf0-44e8-9680-be652cf9ee67"",
                    ""expectedControlType"": ""Button"",
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
                    ""name"": """",
                    ""id"": ""c8ef2457-3436-4f16-b7dd-035f62e89a86"",
                    ""path"": ""<Gamepad>/rightShoulder"",
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
                    ""path"": ""<Keyboard>/a"",
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
                    ""id"": ""f0bbb5c5-7c3e-4896-92d0-6767fb396c56"",
                    ""path"": ""<Gamepad>/leftShoulder"",
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
                    ""processors"": ""Normalize(max=1)"",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a484beca-5b10-479e-9b1c-0d0b12707edd"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": ""MultiTap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""KeyboardInstant"",
                    ""id"": ""6472ec87-093c-4bbd-abb3-82212120a6e7"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""14671b5a-9527-42a6-9a9f-e08616a07746"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""ce6b4e95-aedc-46ff-9f16-19e24552645d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""22b19f4c-b87d-4f28-ad47-f5364aa6d9a4"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": ""MultiTap"",
                    ""processors"": ""Invert"",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e93ade03-26c8-4b37-b25b-4787bf76c9a4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": ""MultiTap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dea78637-8341-432b-b85e-307d78ad5c03"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""45c9d6e7-3a29-4107-88eb-3ce5bad4f453"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b07ce19d-167c-45c6-b53b-449c94ad7d5a"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b738576-5178-4000-b66e-d2aa0f14797f"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Score"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f2ed9614-c876-4020-adc7-83f7884616fa"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Score"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""65499eed-9553-4286-98ec-0213172e06d2"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""856cbb2f-6d25-4a58-955e-69b1f8f7b9de"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9ed8ceb-8066-4279-b783-311db6d26b02"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sneath/Draw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7473336c-6a01-40e5-9d14-7378a22ae29a"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sneath/Draw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""4742204d-bb3a-436f-bec2-290462f0558c"",
            ""actions"": [
                {
                    ""name"": ""Vertical"",
                    ""type"": ""Value"",
                    ""id"": ""72fe0c93-0409-4915-8a5c-b15a1f94b2e9"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Horizontal"",
                    ""type"": ""Value"",
                    ""id"": ""f51d8d5a-4f5d-46fb-8bdf-51c106824203"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""fac4db56-8fd7-4991-aa85-5533d7d43de0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""bc81dab4-18c9-4221-848d-e1d343c9ca4d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""bddede42-e2b7-4089-8c5c-2d3316dca9ad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""afc3e1ba-40c0-4a9e-a65e-1cb5e129bc4c"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""d295e7f5-9d74-46d4-962a-6c3f03914b28"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""d2f73d21-5f4b-4b78-85e0-a2e41c51dfa5"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""aa638084-43e1-4995-ac9c-082ddcb0b1dc"",
                    ""path"": ""<Gamepad>/leftStick/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4909dbc0-d170-48b6-a926-d3613424d229"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""993d8d06-52c3-42c9-87e8-7885cd1fbcb2"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a4709f4-e466-4076-a3ac-eb4b9df3101e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""f70c746b-d126-44ac-ac58-5af289eaddaa"",
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
                    ""id"": ""b926130e-5525-478c-851e-7dd659b36b49"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""aa6cc4cf-76a2-47af-a0ec-d733279e1326"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c66dc263-fb9c-459e-9912-c86bfb6baa19"",
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
                    ""id"": ""ce0d98e2-6fd8-4649-a637-e0b2383ec49d"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6c39f1f-6ee1-4b8a-b5cc-5b44815f081b"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3a3e1994-f3a3-4f80-9769-363eabb60ef6"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7d2d7cc0-c80e-4699-93cb-5e3f8bc63c55"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
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
        m_Duel_AnyKey = m_Duel.FindAction("AnyKey", throwIfNotFound: true);
        m_Duel_Jump = m_Duel.FindAction("Jump", throwIfNotFound: true);
        m_Duel_Score = m_Duel.FindAction("Score", throwIfNotFound: true);
        m_Duel_Pause = m_Duel.FindAction("Pause", throwIfNotFound: true);
        m_Duel_SneathDraw = m_Duel.FindAction("Sneath/Draw", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Vertical = m_Menu.FindAction("Vertical", throwIfNotFound: true);
        m_Menu_Horizontal = m_Menu.FindAction("Horizontal", throwIfNotFound: true);
        m_Menu_Submit = m_Menu.FindAction("Submit", throwIfNotFound: true);
        m_Menu_Back = m_Menu.FindAction("Back", throwIfNotFound: true);
        m_Menu_Cancel = m_Menu.FindAction("Cancel", throwIfNotFound: true);
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
    private readonly InputAction m_Duel_AnyKey;
    private readonly InputAction m_Duel_Jump;
    private readonly InputAction m_Duel_Score;
    private readonly InputAction m_Duel_Pause;
    private readonly InputAction m_Duel_SneathDraw;
    public struct DuelActions
    {
        private @PlayerControls m_Wrapper;
        public DuelActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Horizontal => m_Wrapper.m_Duel_Horizontal;
        public InputAction @Attack => m_Wrapper.m_Duel_Attack;
        public InputAction @Parry => m_Wrapper.m_Duel_Parry;
        public InputAction @Pommel => m_Wrapper.m_Duel_Pommel;
        public InputAction @Dash => m_Wrapper.m_Duel_Dash;
        public InputAction @AnyKey => m_Wrapper.m_Duel_AnyKey;
        public InputAction @Jump => m_Wrapper.m_Duel_Jump;
        public InputAction @Score => m_Wrapper.m_Duel_Score;
        public InputAction @Pause => m_Wrapper.m_Duel_Pause;
        public InputAction @SneathDraw => m_Wrapper.m_Duel_SneathDraw;
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
                @AnyKey.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnAnyKey;
                @AnyKey.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnAnyKey;
                @AnyKey.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnAnyKey;
                @Jump.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnJump;
                @Score.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnScore;
                @Score.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnScore;
                @Score.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnScore;
                @Pause.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnPause;
                @SneathDraw.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnSneathDraw;
                @SneathDraw.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnSneathDraw;
                @SneathDraw.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnSneathDraw;
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
                @AnyKey.started += instance.OnAnyKey;
                @AnyKey.performed += instance.OnAnyKey;
                @AnyKey.canceled += instance.OnAnyKey;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Score.started += instance.OnScore;
                @Score.performed += instance.OnScore;
                @Score.canceled += instance.OnScore;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @SneathDraw.started += instance.OnSneathDraw;
                @SneathDraw.performed += instance.OnSneathDraw;
                @SneathDraw.canceled += instance.OnSneathDraw;
            }
        }
    }
    public DuelActions @Duel => new DuelActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Vertical;
    private readonly InputAction m_Menu_Horizontal;
    private readonly InputAction m_Menu_Submit;
    private readonly InputAction m_Menu_Back;
    private readonly InputAction m_Menu_Cancel;
    public struct MenuActions
    {
        private @PlayerControls m_Wrapper;
        public MenuActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Vertical => m_Wrapper.m_Menu_Vertical;
        public InputAction @Horizontal => m_Wrapper.m_Menu_Horizontal;
        public InputAction @Submit => m_Wrapper.m_Menu_Submit;
        public InputAction @Back => m_Wrapper.m_Menu_Back;
        public InputAction @Cancel => m_Wrapper.m_Menu_Cancel;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Vertical.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnVertical;
                @Vertical.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnVertical;
                @Vertical.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnVertical;
                @Horizontal.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnHorizontal;
                @Horizontal.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnHorizontal;
                @Horizontal.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnHorizontal;
                @Submit.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Submit.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Submit.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Back.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Cancel.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Vertical.started += instance.OnVertical;
                @Vertical.performed += instance.OnVertical;
                @Vertical.canceled += instance.OnVertical;
                @Horizontal.started += instance.OnHorizontal;
                @Horizontal.performed += instance.OnHorizontal;
                @Horizontal.canceled += instance.OnHorizontal;
                @Submit.started += instance.OnSubmit;
                @Submit.performed += instance.OnSubmit;
                @Submit.canceled += instance.OnSubmit;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);
    public interface IDuelActions
    {
        void OnHorizontal(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnParry(InputAction.CallbackContext context);
        void OnPommel(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnAnyKey(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnScore(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnSneathDraw(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnVertical(InputAction.CallbackContext context);
        void OnHorizontal(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
    }
}
