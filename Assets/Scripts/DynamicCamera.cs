using UnityEngine;
using Cinemachine;

public class DynamicCamera : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public CinemachineVirtualCamera virtualCamera;

    public float minFOV = 30f;  // Closest zoom
    public float maxFOV = 60f;  // Furthest zoom
    public float zoomSpeed = 5f;

    public float minDistance = 2f;  // Minimum distance for max zoom-in
    public float maxDistance = 10f; // Maximum distance for max zoom-out

    public float followSpeed = 5f; // Camera movement speed
    public float yOffset = 5f; // Fixed camera height
    public float lockedXRotation = 45f; // Keep the X rotation fixed

    private void LateUpdate()
    {
        if (player1 == null || player2 == null || virtualCamera == null)
        {
            Debug.LogWarning("DynamicCamera: Players or Virtual Camera are not assigned!");
            return;
        }

        // Calculate midpoint between players
        Vector3 targetPosition = (player1.position + player2.position) / 2f;
        targetPosition.y = yOffset; // Keep camera at a fixed height

        // Move camera smoothly
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Calculate distance between players
        float playerDistance = Vector3.Distance(player1.position, player2.position);

        // Map player distance to FOV range
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, Mathf.InverseLerp(minDistance, maxDistance, playerDistance));

        // Smoothly adjust the camera FOV
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, targetFOV, zoomSpeed * Time.deltaTime);

        // Lock X rotation to 45° while keeping Y/Z the same
        transform.rotation = Quaternion.Euler(lockedXRotation, transform.rotation.eulerAngles.y, 0f);
    }
}


