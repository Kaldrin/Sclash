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
                    ""name"": ""Vertical"",
                    ""type"": ""Value"",
                    ""id"": ""eeaff253-a1b1-4a0e-affd-149ab803cb59"",
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
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""Gamepad Scheme"",
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
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""15997951-8237-4e59-950d-1a72fd28155e"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ArrowScheme"",
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
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard2"",
                    ""id"": ""b2136e9f-f637-4582-beb9-f66bd30e63bb"",
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
                    ""id"": ""f17209b6-677b-4e1f-b66a-168d4613b1b2"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ArrowScheme"",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e4df448e-fe5a-4b01-8a1b-6168d35502dc"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ArrowScheme"",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""51b84bc9-a8dc-42d4-8b2e-34414625b7f3"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""Gamepad Scheme"",
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
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""Parry"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71ffb6ff-f024-4b30-9b3c-978fedf1706b"",
                    ""path"": ""<Keyboard>/numpad2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ArrowScheme"",
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
                    ""groups"": ""Gamepad Scheme"",
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
                    ""groups"": ""WASDScheme"",
                    ""action"": ""Pommel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2420896c-acda-41c3-87c3-996de17cd0b4"",
                    ""path"": ""<Keyboard>/numpad3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ArrowScheme"",
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
                    ""groups"": ""Gamepad Scheme"",
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
                    ""groups"": ""Gamepad Scheme"",
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
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""WASDScheme"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""811c52e0-847d-4720-8da3-729ec49a8705"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": ""MultiTap"",
                    ""processors"": """",
                    ""groups"": ""ArrowScheme"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c0414a64-a599-4023-aabf-1bc0a3d51452"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": ""MultiTap"",
                    ""processors"": ""Invert"",
                    ""groups"": ""ArrowScheme"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""GamepadTrigger"",
                    ""id"": ""62d43948-ba25-453c-bccd-133a09c8f787"",
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
                    ""id"": ""edb353da-3b50-44cb-b756-87ee77a8c27b"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""71fa1314-0a86-41e6-b713-d2b246d77fce"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4da3f7c9-2e96-460f-9bcc-3c1faf9b4b86"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""AnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c143d405-92dd-4708-aaeb-e466315d3ac7"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""AnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2745cb92-4216-4df7-bf99-e3305e74bff1"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""AnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e8536f8-89eb-417f-abbf-c46701c7f2e9"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""AnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6be34400-bded-4c8e-b861-6b057aee0c6c"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""AnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""81b5bae6-9f4c-4e59-8bd3-e8f194576fc2"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""AnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6527ca6-89e3-4fe8-8dd7-bb408f498942"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""AnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""17e9691c-b6d9-46c1-8949-7c58dbbcfe9c"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
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
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b8bae4a-0ff6-4782-9674-477aa24bb5c0"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ArrowScheme"",
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
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""Score"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9ed8ceb-8066-4279-b783-311db6d26b02"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
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
                    ""groups"": ""WASDScheme"",
                    ""action"": ""Sneath/Draw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""11b40eb5-ffd5-49b4-84ef-56482589e4bb"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""afc0599f-79bd-4100-93a0-03faf5d4cbf5"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""WASDScheme"",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""7d9c1728-0c0f-4448-a1bc-d764a0917c15"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""WASDScheme"",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""76d13b6a-0818-4dc1-a75b-8307556d9665"",
                    ""path"": ""<Gamepad>/leftStick/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard2"",
                    ""id"": ""7814088d-3ae8-415a-85aa-08d82cf674a3"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""7c40f8ae-35d8-48b7-9825-ecefcb59b0f6"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ArrowScheme"",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""23aa2b73-b764-491f-84f2-4998456ed793"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ArrowScheme"",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""65499eed-9553-4286-98ec-0213172e06d2"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""WASDScheme"",
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
                    ""groups"": ""Gamepad Scheme"",
                    ""action"": ""Pause"",
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
                },
                {
                    ""name"": ""Navigate"",
                    ""type"": ""Value"",
                    ""id"": ""7edbba51-8092-4010-9d90-e4e3b53f6fe3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": ""SlowTap""
                },
                {
                    ""name"": ""Menu triggers"",
                    ""type"": ""Button"",
                    ""id"": ""0de4fb2d-1f81-4cf5-9ebb-2d31d0f719a6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Menu secondary"",
                    ""type"": ""Button"",
                    ""id"": ""8df4e28f-e17b-46f2-adf6-0f6634a1d006"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
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
                    ""id"": ""bc180a8d-9dbf-4f9e-a2ce-231ee9cc53a0"",
                    ""path"": ""<Keyboard>/backspace"",
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
                },
                {
                    ""name"": ""Gamepad"",
                    ""id"": ""9d102bd4-d0ee-4933-b916-d39b43fdfc5f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6c71f267-0c15-46ab-acac-224234c0cd67"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""40ddc249-7d2a-4f2e-9c1c-07ced63082d2"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""06188670-1684-4c51-b625-a09a6a510689"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""55de03f7-ca5e-48bb-a02b-4e89e5c96c39"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a2abbbf7-823e-4e3b-9ada-7c21c14965ca"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5001efde-4473-410b-8513-f57f95ff4d65"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a8363636-4eba-4987-b7ab-d9932f616235"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""99e00cff-a75e-4d18-a2f4-ad34b358290b"",
                    ""path"": ""<Gamepad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""eb077d26-ce61-4852-a6a2-4a918d10a3fa"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Joystick"",
                    ""id"": ""186cd17f-2589-4049-8558-b3b9b4b19f1b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""41218666-d46a-481e-a531-5577f2ef8672"",
                    ""path"": ""<Joystick>/stick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""37ba8960-9db5-42c1-b056-ea42a349743c"",
                    ""path"": ""<Joystick>/stick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0bd7e4c2-3941-4542-8f6d-c526f3c7ed6f"",
                    ""path"": ""<Joystick>/stick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f86973b2-2e70-4d70-bfa6-63214d76388c"",
                    ""path"": ""<Joystick>/stick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""b161bcad-5728-416e-8a35-faf32590edf9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""bc73c6b1-425c-4e0e-86a3-db7cc545951e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""cfad1a1c-cfdd-4ea2-9b97-f7aa9fbe34ba"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d4ceba66-23af-49e4-baec-0bd201be96a2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0fb12511-4b1f-41eb-a468-d18f126a13df"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""18d15289-ab43-4749-9c58-7146ab522295"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0043da4b-3a1a-4071-bdf6-0cbdbe501ec9"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5c6fea21-07cb-467d-ad8f-ed346313ce0e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""fdac83ce-21d6-4a17-bce1-663d827fb536"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""1476be94-701b-42a1-a947-036ed94497e8"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu triggers"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""9883aca6-a726-45ad-a7ee-29864f9a362c"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu triggers"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""9647a2e7-9954-4c06-be11-e63f637fef7c"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu triggers"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f831129e-941c-476c-bc08-233766a8e152"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu secondary"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""WASDScheme"",
            ""bindingGroup"": ""WASDScheme"",
            ""devices"": []
        },
        {
            ""name"": ""ArrowScheme"",
            ""bindingGroup"": ""ArrowScheme"",
            ""devices"": []
        },
        {
            ""name"": ""Gamepad Scheme"",
            ""bindingGroup"": ""Gamepad Scheme"",
            ""devices"": []
        }
    ]
}");
        // Duel
        m_Duel = asset.FindActionMap("Duel", throwIfNotFound: true);
        m_Duel_Horizontal = m_Duel.FindAction("Horizontal", throwIfNotFound: true);
        m_Duel_Vertical = m_Duel.FindAction("Vertical", throwIfNotFound: true);
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
        m_Menu_Submit = m_Menu.FindAction("Submit", throwIfNotFound: true);
        m_Menu_Back = m_Menu.FindAction("Back", throwIfNotFound: true);
        m_Menu_Cancel = m_Menu.FindAction("Cancel", throwIfNotFound: true);
        m_Menu_Navigate = m_Menu.FindAction("Navigate", throwIfNotFound: true);
        m_Menu_Menutriggers = m_Menu.FindAction("Menu triggers", throwIfNotFound: true);
        m_Menu_Menusecondary = m_Menu.FindAction("Menu secondary", throwIfNotFound: true);
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
    private readonly InputAction m_Duel_Vertical;
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
        public InputAction @Vertical => m_Wrapper.m_Duel_Vertical;
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
                @Vertical.started -= m_Wrapper.m_DuelActionsCallbackInterface.OnVertical;
                @Vertical.performed -= m_Wrapper.m_DuelActionsCallbackInterface.OnVertical;
                @Vertical.canceled -= m_Wrapper.m_DuelActionsCallbackInterface.OnVertical;
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
                @Vertical.started += instance.OnVertical;
                @Vertical.performed += instance.OnVertical;
                @Vertical.canceled += instance.OnVertical;
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
    private readonly InputAction m_Menu_Submit;
    private readonly InputAction m_Menu_Back;
    private readonly InputAction m_Menu_Cancel;
    private readonly InputAction m_Menu_Navigate;
    private readonly InputAction m_Menu_Menutriggers;
    private readonly InputAction m_Menu_Menusecondary;
    public struct MenuActions
    {
        private @PlayerControls m_Wrapper;
        public MenuActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Submit => m_Wrapper.m_Menu_Submit;
        public InputAction @Back => m_Wrapper.m_Menu_Back;
        public InputAction @Cancel => m_Wrapper.m_Menu_Cancel;
        public InputAction @Navigate => m_Wrapper.m_Menu_Navigate;
        public InputAction @Menutriggers => m_Wrapper.m_Menu_Menutriggers;
        public InputAction @Menusecondary => m_Wrapper.m_Menu_Menusecondary;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Submit.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Submit.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Submit.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Back.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Cancel.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnCancel;
                @Navigate.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnNavigate;
                @Navigate.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnNavigate;
                @Navigate.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnNavigate;
                @Menutriggers.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnMenutriggers;
                @Menutriggers.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnMenutriggers;
                @Menutriggers.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnMenutriggers;
                @Menusecondary.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnMenusecondary;
                @Menusecondary.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnMenusecondary;
                @Menusecondary.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnMenusecondary;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Submit.started += instance.OnSubmit;
                @Submit.performed += instance.OnSubmit;
                @Submit.canceled += instance.OnSubmit;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
                @Navigate.started += instance.OnNavigate;
                @Navigate.performed += instance.OnNavigate;
                @Navigate.canceled += instance.OnNavigate;
                @Menutriggers.started += instance.OnMenutriggers;
                @Menutriggers.performed += instance.OnMenutriggers;
                @Menutriggers.canceled += instance.OnMenutriggers;
                @Menusecondary.started += instance.OnMenusecondary;
                @Menusecondary.performed += instance.OnMenusecondary;
                @Menusecondary.canceled += instance.OnMenusecondary;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);
    private int m_WASDSchemeSchemeIndex = -1;
    public InputControlScheme WASDSchemeScheme
    {
        get
        {
            if (m_WASDSchemeSchemeIndex == -1) m_WASDSchemeSchemeIndex = asset.FindControlSchemeIndex("WASDScheme");
            return asset.controlSchemes[m_WASDSchemeSchemeIndex];
        }
    }
    private int m_ArrowSchemeSchemeIndex = -1;
    public InputControlScheme ArrowSchemeScheme
    {
        get
        {
            if (m_ArrowSchemeSchemeIndex == -1) m_ArrowSchemeSchemeIndex = asset.FindControlSchemeIndex("ArrowScheme");
            return asset.controlSchemes[m_ArrowSchemeSchemeIndex];
        }
    }
    private int m_GamepadSchemeSchemeIndex = -1;
    public InputControlScheme GamepadSchemeScheme
    {
        get
        {
            if (m_GamepadSchemeSchemeIndex == -1) m_GamepadSchemeSchemeIndex = asset.FindControlSchemeIndex("Gamepad Scheme");
            return asset.controlSchemes[m_GamepadSchemeSchemeIndex];
        }
    }
    public interface IDuelActions
    {
        void OnHorizontal(InputAction.CallbackContext context);
        void OnVertical(InputAction.CallbackContext context);
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
        void OnSubmit(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnNavigate(InputAction.CallbackContext context);
        void OnMenutriggers(InputAction.CallbackContext context);
        void OnMenusecondary(InputAction.CallbackContext context);
    }
}
