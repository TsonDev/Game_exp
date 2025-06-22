using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PauseManagement : MonoBehaviour
{
    private VisualElement pauseBackground;
    private Button menubtn;
    private Button continuebtn;
    private Button pausebtn;
    private VisualElement root;
    private Slider volumeSlider;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        menubtn = root.Q<Button>("btn_menu");
        continuebtn = root.Q<Button>("btn_continue");
        pausebtn = root.Q<Button>("btn_pause");
        volumeSlider = root.Q<Slider>("volume_Slider");
        pausebtn.focusable = false;

        pauseBackground = root.Q<VisualElement>("PauseBackground");

        continuebtn.clicked += () =>
        {
            ResumeGame();
        };

        menubtn.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("StartScreen");
        };

        pausebtn.clicked += () =>
        {
            PauseGame();
        };

        // Lúc đầu ẩn Pause menu
        pauseBackground.style.display = DisplayStyle.None;
        // Gán sự kiện khi giá trị Slider thay đổi
        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            float newVolume = Mathf.Clamp01( evt.newValue);
            SetVolume(newVolume);
        });

        // Đặt mặc định ban đầu
        volumeSlider.value = 0.5f; // Ví dụ: 50% volume
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseBackground.style.display == DisplayStyle.None)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        pauseBackground.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        pauseBackground.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }
    private void SetVolume(float volume)
    {
        // Giả sử bạn muốn điều chỉnh volume AudioListener
        AudioListener.volume = volume;
        Debug.Log("Volume set to: " + volume);
    }
}
