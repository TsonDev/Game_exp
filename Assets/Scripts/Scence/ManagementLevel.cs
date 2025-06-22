using UnityEngine;
using Cinemachine;

public class ManagementLevel : MonoBehaviour
{
    public Transform targetPosition;
    public CinemachineVirtualCamera virtualCam;
    public CinemachineConfiner2D confiner;
    public PolygonCollider2D targetConfinerShape;
    public Transform player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Disable all virtual cameras
        foreach (var cam in FindObjectsOfType<CinemachineVirtualCamera>())
            cam.gameObject.SetActive(false);

        // Enable current virtual camera
        virtualCam.gameObject.SetActive(true);
        virtualCam.Follow = player;

        // Move player
        player.position = targetPosition.position;

        // Force camera to move immediately
        virtualCam.ForceCameraPosition(targetPosition.position, Quaternion.identity);

        // Update confiner
        if (confiner != null && targetConfinerShape != null)
        {
            confiner.m_BoundingShape2D = targetConfinerShape;
            confiner.InvalidateCache();
        }
    }
}
