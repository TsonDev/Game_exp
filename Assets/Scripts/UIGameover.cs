using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIGameover : MonoBehaviour
{
    private VisualElement gameOverPanel;
    private Button retryButton;
    
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        gameOverPanel = root.Q<VisualElement>("GameOverPanel");
        retryButton = root.Q<Button>("PlayAgain"); // Đặt name="retry-button" trong UI Builder
     

        retryButton.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        };

        HideGameOver();
    }
    public void ShowGameOver()
    {
        
        gameOverPanel.style.display = DisplayStyle.Flex;
        

    }

    public void HideGameOver()
    {
        
        gameOverPanel.style.display = DisplayStyle.None;
    }
}
