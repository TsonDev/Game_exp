using UnityEngine;
using Cinemachine;

public class ManagementLevel : MonoBehaviour
{
    public Transform targetPosition;
    public CinemachineVirtualCamera virtualCam;
    public CinemachineConfiner2D confiner;
    public PolygonCollider2D newConfinerShape; // Hình vùng mới (nếu level2 dùng confiner khác)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Dịch chuyển player
            collision.transform.position = targetPosition.position;

            // Ép camera theo kịp player ngay lập tức
            if (virtualCam != null)
            {
                virtualCam.ForceCameraPosition(targetPosition.position, Quaternion.identity);
            }

            // Nếu có confiner mới cho Level 2
            if (confiner != null && newConfinerShape != null)
            {
                confiner.m_BoundingShape2D = newConfinerShape;
                confiner.InvalidateCache(); // RẤT QUAN TRỌNG để cập nhật lại confiner
            }
        }
    }
}
