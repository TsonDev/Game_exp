using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManagement : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }
    public void LoadNextLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Tách tên scene, ví dụ: "Level 1" => ["Level", "1"]
        string[] parts = currentSceneName.Split(' ');
        if (parts.Length < 2)
        {
            Debug.LogError("Tên scene không đúng định dạng (ví dụ: 'Level 1')");
            return;
        }

        int currentLevel;
        if (int.TryParse(parts[1], out currentLevel))
        {
            int nextLevel = currentLevel + 1;
            string nextSceneName = parts[0] + " " + nextLevel;

            Debug.Log("Chuyển sang scene: " + nextSceneName);
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
