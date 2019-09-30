using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
    private Vector3 focusOffset;
    private Quaternion cameraRotation;

    private const float ClosestAngle = 20f;
    private const float FurthestAngle = 50f;
    private const float ClosestDistanceFromLookAtPosition = 16f;
    private const float FurthestDistanceFromLookAtPosition = 36f;

    private const float CameraPanAcceleration = 0.4f;
    private const float CameraPanDeceleration = 2f;
    private const float CameraPanMaxVelocity = 4f;
    private const float CameraZoomAcceleration = 0.5f;
    private const float CameraZoomDeceleration = 0.125f;

    private float _zoomVelocity = 0f;
    private float _zoomValue = 0.3f;
    private Vector3 _lookAtPosition = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;

    // Update is called once per frame
    void LateUpdate()
    {
        if (GameController.instance.GamePaused)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            // TODO: allow user to click middle mouse and drag
        }

        // update tracked position
        Vector3 cameraMovement = CalculateCameraMotionInput();

        _velocity += cameraMovement * CameraPanAcceleration * Time.deltaTime;
        _velocity = Vector3.ClampMagnitude(_velocity, CameraPanMaxVelocity);

        float decelerationMaxDelta = CameraPanDeceleration * Time.deltaTime;
        if (cameraMovement.x == 0)
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, decelerationMaxDelta);
        }

        if (cameraMovement.z == 0)
        {
            _velocity.z = Mathf.MoveTowards(_velocity.z, 0, decelerationMaxDelta);
        }

        _lookAtPosition += _velocity;

        // update zoom value
        _zoomVelocity += Input.mouseScrollDelta.y * CameraZoomAcceleration * Time.deltaTime;
        if (Input.mouseScrollDelta.y == 0)
        {
            _zoomVelocity = Mathf.MoveTowards(_zoomVelocity, 0, CameraZoomDeceleration * Time.deltaTime);
        }
        _zoomValue = Mathf.Clamp01(_zoomValue + _zoomVelocity);

        // apply updated position and zoom values
        PositionCamera();
    }

    private void PositionCamera()
    {
        float distanceFromCamera = Mathf.Lerp(FurthestDistanceFromLookAtPosition, ClosestDistanceFromLookAtPosition, _zoomValue);
        float angleFromCamera = Mathf.Lerp(FurthestAngle, ClosestAngle, _zoomValue);

        Vector3 offset = new Vector3(0, 0, -distanceFromCamera);
        offset = Quaternion.AngleAxis(angleFromCamera, Vector3.right) * offset;
        transform.position = _lookAtPosition + offset;

        transform.LookAt(_lookAtPosition, Vector3.up);
    }

    private Vector3 CalculateCameraMotionInput()
    {
        Vector3 cameraMovementInput = Vector3.zero;

        if (Input.mousePosition.x >= Screen.width)
        {
            cameraMovementInput.x = 1;
        }
        else if (Input.mousePosition.x <= 0)
        {
            cameraMovementInput.x = -1;
        }

        if (Input.mousePosition.y >= Screen.height)
        {
            cameraMovementInput.z = 1;
        }
        else if (Input.mousePosition.y <= 0)
        {
            cameraMovementInput.z = -1;
        }

        // Debug.Log(Input.mousePosition + " -> cameraMovementInput: " + cameraMovementInput);
        return cameraMovementInput;
    }
}