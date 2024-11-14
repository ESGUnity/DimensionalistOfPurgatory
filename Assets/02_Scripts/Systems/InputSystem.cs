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
    LayerMask placementLayerMask;

    private static InputSystem instance;
    public static InputSystem Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;

        if (gameObject.tag == "Player")
        {
            placementLayerMask = LayerMask.GetMask("PlayerField");
        }
        else
        {
            placementLayerMask = LayerMask.GetMask("OpponentField");
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
            Debug.Log("±×·¦ ½ÇÇà");
        }
    }

    public Vector3 GetMousePositionOnField()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }
    public bool CanPlacemet()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayerMask))
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
