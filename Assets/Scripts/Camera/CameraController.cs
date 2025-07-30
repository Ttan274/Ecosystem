using System;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        Orbit,
        Follow,
        Fly
    }

    [Header("Main")]
    [SerializeField] private CameraMode mode;
    [SerializeField] private KeyCode switchKey;

    [Header("Orbit")]
    [SerializeField] private Transform terrain;
    [SerializeField] private Vector3 rotAxis;
    [SerializeField] private float rotSpeed;

    [Header("Fly")]
    [SerializeField] private float speed = 100f;
    [SerializeField] private float speedMultiplier = 250f;
    [SerializeField] private float maxSpeed = 1000f;
    [SerializeField] private float sensivity = 0.25f;
    [SerializeField] private float mouseSpeed = 0.1f;

    private Vector3 lastMousePos = new Vector3(255, 255, 255);
    private float totalMultiplied = 1f;
    private Transform focusPoint;
    private bool camActive = false;
    public static UnityAction<string> OnCamModeChange;

    private void Update()
    {
        if (!camActive) return;
        SwitchCameraMode();
        UpdateCameraMode();
    }

    #region CameraMode Methods
    private void UpdateFlyBehaviour()
    {
        MouseInput();

        Vector3 inputAxes = GetBaseInput();
        if(Input.GetKey(KeyCode.LeftShift))
        {
            totalMultiplied += Time.unscaledDeltaTime;
            inputAxes = inputAxes * totalMultiplied * speedMultiplier;
            inputAxes.x = Mathf.Clamp(inputAxes.x, -maxSpeed, maxSpeed);
            inputAxes.y = Mathf.Clamp(inputAxes.y, -maxSpeed, maxSpeed);
            inputAxes.z = Mathf.Clamp(inputAxes.z, -maxSpeed, maxSpeed);
        }
        else
        {
            totalMultiplied = Mathf.Clamp(totalMultiplied * 0.5f, 1f, 1000f);
            inputAxes = inputAxes * speed;
        }

        inputAxes = inputAxes * Time.unscaledDeltaTime;
        Vector3 newPos = transform.position;
        /*
        if(Input.GetKey(KeyCode.Space))
        {
            transform.Translate(inputAxes);
            newPos.x = transform.position.x;
            newPos.z = transform.position.z;
            transform.position = newPos;
        }
        else
        {*/
        transform.Translate(inputAxes);
        //}
    }

    private void UpdateFollowCamera()
    {
        transform.position = focusPoint.position + new Vector3(0f, 5f, 5f);
        transform.LookAt(focusPoint.position);
    }

    private void UpdateOrbitBehaviour()
    {
        if(terrain != null)
            focusPoint = terrain;
        transform.LookAt(focusPoint.position);
        transform.RotateAround(focusPoint.position, rotAxis, rotSpeed * Time.unscaledDeltaTime);
    }
    #endregion

    #region Input Methods
    private void SwitchCameraMode()
    {
        if (Input.GetKeyUp(switchKey))
        {
            switch (mode)
            {
                case CameraMode.Orbit:
                    mode = CameraMode.Fly;
                    OnCamModeChange?.Invoke(mode.ToString());
                    CursorBehaviour(true);
                    break;
                case CameraMode.Fly:
                    mode = CameraMode.Orbit;
                    OnCamModeChange?.Invoke(mode.ToString());
                    CursorBehaviour(true);
                    break;
                default:
                    Debug.LogWarning("Mode is not defined");
                    break;
            }
        }
    }

    private Vector3 GetBaseInput()
    {
        Vector3 vel = new Vector3();
        if (Input.GetKey(KeyCode.W))
            vel += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S))
            vel += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.A))
            vel += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D))
            vel += new Vector3(1, 0, 0);
        return vel;
    }

    private void MouseInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            float rotationX = -delta.y * sensivity * mouseSpeed * Time.deltaTime;
            float rotationY = delta.x * sensivity * mouseSpeed * Time.deltaTime;

            transform.eulerAngles += new Vector3(rotationX, rotationY, 0f);

            lastMousePos = Input.mousePosition;
        }
    }

    #endregion 

    #region Utility Methods
    public void ChangeCamActiveness(bool x) => camActive = x;

    private void UpdateCameraMode()
    {
        switch (mode)
        {
            case CameraMode.Orbit:
                UpdateOrbitBehaviour();
                break;
            case CameraMode.Follow:
                UpdateFollowCamera();
                break;
            case CameraMode.Fly:
                UpdateFlyBehaviour();
                break;
            default:
                Debug.LogWarning("Mode is not defined");
                break;
        }
    }

    public void StartFollowing(Transform t)
    {
        SetFocusPoint(t);
        mode = CameraMode.Follow;
    }

    public void SetFocusPoint(Transform t)
    {
        if (terrain == null)
        {
            terrain = t;
            camActive = true;
        }

        focusPoint = t;
    }

    private void CursorBehaviour(bool status) => Cursor.visible = status;
    #endregion
}
