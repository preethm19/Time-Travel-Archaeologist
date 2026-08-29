using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Distance")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 8f;

    [Header("Camera Height")]
    [SerializeField] private float height = 1.5f;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 0.15f;

    [Header("Vertical Limits")]
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothTime = 0.05f;

    private float yaw = 0f;
    private float pitch = 15f;

    private Vector3 currentVelocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        HandleMouseInput();
        HandleZoom();
        UpdateCameraPosition();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * mouseSensitivity;
        pitch -= mouseDelta.y * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void HandleZoom()
    {
        if (Mouse.current == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        distance -= scroll * 0.01f;

        distance = Mathf.Clamp(
            distance,
            minDistance,
            maxDistance
        );
    }

    private void UpdateCameraPosition()
    {
        Vector3 targetPosition =
            target.position + Vector3.up * height;

        Quaternion rotation =
            Quaternion.Euler(pitch, yaw, 0f);

        Vector3 desiredPosition =
            targetPosition - rotation * Vector3.forward * distance;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            positionSmoothTime
        );

        transform.LookAt(targetPosition);
    }
}