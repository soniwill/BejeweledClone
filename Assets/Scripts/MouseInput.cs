// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Mouse.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MouseInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MouseInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Mouse"",
    ""maps"": [
        {
            ""name"": ""Mouse"",
            ""id"": ""34fd8c7d-e5f3-40b2-96c5-c0e846da5bb4"",
            ""actions"": [
                {
                    ""name"": ""MouseUp"",
                    ""type"": ""Button"",
                    ""id"": ""58d46066-9871-4e07-be7a-50c3914a54b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseDown"",
                    ""type"": ""Button"",
                    ""id"": ""c56c80f0-c5cc-4c8e-a962-618f1b844538"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""f7fd2137-bf0c-4a40-926d-7cbf43280471"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7b65d989-57f9-406a-8878-e36b41cc422d"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c81a1c2-c5e8-4471-9751-e7f83d60c495"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3d2898d6-962a-4c84-a52a-74f897482618"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_MouseUp = m_Mouse.FindAction("MouseUp", throwIfNotFound: true);
        m_Mouse_MouseDown = m_Mouse.FindAction("MouseDown", throwIfNotFound: true);
        m_Mouse_MousePosition = m_Mouse.FindAction("MousePosition", throwIfNotFound: true);
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

    // Mouse
    private readonly InputActionMap m_Mouse;
    private IMouseActions m_MouseActionsCallbackInterface;
    private readonly InputAction m_Mouse_MouseUp;
    private readonly InputAction m_Mouse_MouseDown;
    private readonly InputAction m_Mouse_MousePosition;
    public struct MouseActions
    {
        private @MouseInput m_Wrapper;
        public MouseActions(@MouseInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseUp => m_Wrapper.m_Mouse_MouseUp;
        public InputAction @MouseDown => m_Wrapper.m_Mouse_MouseDown;
        public InputAction @MousePosition => m_Wrapper.m_Mouse_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterface != null)
            {
                @MouseUp.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseUp;
                @MouseUp.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseUp;
                @MouseUp.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseUp;
                @MouseDown.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseDown;
                @MouseDown.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseDown;
                @MouseDown.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseDown;
                @MousePosition.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_MouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseUp.started += instance.OnMouseUp;
                @MouseUp.performed += instance.OnMouseUp;
                @MouseUp.canceled += instance.OnMouseUp;
                @MouseDown.started += instance.OnMouseDown;
                @MouseDown.performed += instance.OnMouseDown;
                @MouseDown.canceled += instance.OnMouseDown;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public MouseActions @Mouse => new MouseActions(this);
    public interface IMouseActions
    {
        void OnMouseUp(InputAction.CallbackContext context);
        void OnMouseDown(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
}
