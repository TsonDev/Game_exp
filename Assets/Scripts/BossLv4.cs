using System.Collections;
using UnityEngine;

public class BossLv4 : MonoBehaviour
{
    private Animator animator;
    private Vector3 originalScale;
    private Vector3 maxScale = new Vector3(50f, 50f, 1f); // Giới hạn độ to tối đa
    private float interval = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;

        // Bắt đầu vòng lặp tấn công
        StartCoroutine(BossAttackRoutine());
    }

    IEnumerator BossAttackRoutine()
    {
        while (true)
        {
            // 1. Phun lửa (BossSkill2)
            animator.Play("BossSkill2");
            yield return new WaitForSeconds(0.5f); // Chờ animation phun lửa (điều chỉnh theo độ dài thực tế)

            // 2. Vẫy đuôi (BossSkill1)
            animator.Play("BossSkill1");
            yield return new WaitForSeconds(0.5f); // Chờ animation vẫy đuôi

            // 3. Tăng kích thước (to lên dần)
            GrowBoss();

            // 4. Chờ đến lần kế tiếp
            yield return new WaitForSeconds(interval);
        }
    }

    void GrowBoss()
    {
        Vector3 currentScale = transform.localScale;
        Vector3 newScale = currentScale * 1.5f;

        // Giới hạn kích thước tối đa
        if (newScale.x > maxScale.x)
            newScale = maxScale;

        // Tính tỉ lệ thay đổi chiều cao
        float heightDiff = newScale.y - currentScale.y;

        // Dời boss lên trên nửa phần tăng thêm để giữ chân ở đúng vị trí
        transform.position += new Vector3(0, heightDiff / 2f, 0);

        // Gán scale mới
        transform.localScale = newScale;
    }

}
