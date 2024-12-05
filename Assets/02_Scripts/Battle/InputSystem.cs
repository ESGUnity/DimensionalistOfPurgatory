using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    public Camera PlayerCamera;
    public event Action OnClickedLeft;
    public event Action OnPressedLeft;
    public event Action OnReleasedLeft;

    InputActionAsset inputActions;
    InputAction leftClickAction;
    InputAction rightClickAction;
    InputAction escapeAction;
    Vector3 lastPosition;
    LayerMask entireLayerMask;
    LayerMask astralLayerMask;
    string thisPlayerTag;

    private void Awake()
    {
        thisPlayerTag = gameObject.tag;

        if (thisPlayerTag == "Player")
        {
            entireLayerMask = LayerMask.GetMask("PlayerField");
            astralLayerMask = LayerMask.GetMask("PlayerAstralField");
        }
        else if (thisPlayerTag == "OpponentAI")
        {
            entireLayerMask = LayerMask.GetMask("OpponentField");
            astralLayerMask = LayerMask.GetMask("OpponentAstralField");
        }

        inputActions = GetComponent<PlayerInput>().actions;

        leftClickAction = inputActions.FindAction("LeftClick");
        rightClickAction = inputActions.FindAction("RightClick");
        escapeAction = inputActions.FindAction("Escape");

        leftClickAction.started += (context) => OnClickedLeft?.Invoke();
        leftClickAction.canceled += (context) => OnReleasedLeft?.Invoke();
    }
    private void OnEnable()
    {
        leftClickAction.Enable();
    }
    private void OnDisable()
    {
        leftClickAction.Disable();
    }

    void Update()
    {
        if (leftClickAction.IsPressed())
        {
            OnPressedLeft?.Invoke();
        }
    }

    public Vector3 GetMousePositionOnField()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, entireLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
    public Vector3 GetMousePositionOnAstralField()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, astralLayerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }
    public bool CanPlacement()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, entireLayerMask))
        {
            return true;
        }

        return false;
    }
    public bool CanPlacementAstral()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, astralLayerMask))
        {
            return true;
        }

        return false;
    }
    public float GetViewportPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 viewportPosition = PlayerCamera.ScreenToViewportPoint(mousePos);
        return viewportPosition.y;
    }
    public Vector3 GetMousePositionOnScreen()
    {
        return Input.mousePosition;
    }
}
