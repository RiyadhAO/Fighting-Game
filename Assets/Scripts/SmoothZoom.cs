using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float zoomSpeed = 5f;
    public float minZoom = -15f;
    public float maxZoom = -5f;

    private CinemachineTransposer transposer;

    void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // Scroll Up/Down
        if (scrollInput != 0)
        {
            Vector3 newOffset = transposer.m_FollowOffset;
            newOffset.z = Mathf.Clamp(newOffset.z + scrollInput * zoomSpeed, minZoom, maxZoom);
            transposer.m_FollowOffset = newOffset;
        }
    }
}
