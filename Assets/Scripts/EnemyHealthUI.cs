using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthUI : MonoBehaviour
{
    public Image healthBarFill; // Thanh màu đỏ
    public Canvas canvas;       // Canvas chứa thanh máu

    private Coroutine hideCoroutine;

    public void SetHealth(float current, float max)
    {
        Debug.Log("SetHealth được gọi!");
        float percent = current / max;
        healthBarFill.fillAmount = percent;

        Show();

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterSeconds(2f));
    }

    void Show()
    {
        canvas.enabled = true;
    }

    IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canvas.enabled = false;
    }

    void Start()
    {
        canvas.enabled = true; // Ban đầu ẩn thanh máu
    }
}
