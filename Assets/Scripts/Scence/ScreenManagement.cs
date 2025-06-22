using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManagement : MonoBehaviour
{
    public List<GameObject> requiredEnemies; // Danh sách quái cần tiêu diệt
    private Animator doorAnimator;
    private bool isDoorOpen = false;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isDoorOpen && AreRequiredEnemiesDefeated())
        {
            OpenDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDoorOpen && collision.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }

    private bool AreRequiredEnemiesDefeated()
    {
        // Kiểm tra xem toàn bộ quái trong danh sách đã bị tiêu diệt (null) chưa
        foreach (GameObject enemy in requiredEnemies)
        {
            if (enemy != null) return false;
        }
        return true;
    }

    private void OpenDoor()
    {
        isDoorOpen = true;
        doorAnimator.SetTrigger("Open");
    }

    private void LoadNextLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string[] parts = currentSceneName.Split(' ');

        if (parts.Length < 2)
        {
            Debug.LogError("Tên scene không đúng định dạng (ví dụ: 'Level 1')");
            return;
        }

        if (int.TryParse(parts[1], out int currentLevel))
        {
            int nextLevel = currentLevel + 1;
            string nextSceneName = parts[0] + " " + nextLevel;

            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save();

            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Không thể phân tích số màn từ tên scene.");
        }
    }
}
