using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    

    private Button levelbtn1;
    private Button levelbtn2;
    private Button levelbtn3;
    private Button levelbtn4;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        levelbtn1 = root.Q<Button>("level1_btn");
        levelbtn2 = root.Q<Button>("level2_btn");
        levelbtn3 = root.Q<Button>("level3_btn");
        levelbtn4 = root.Q<Button>("level4_btn");

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); // mặc định mở level 1

        SetupLevelButton(levelbtn1, 1, unlockedLevel);
        SetupLevelButton(levelbtn2, 2, unlockedLevel);
        SetupLevelButton(levelbtn3, 3, unlockedLevel);
        SetupLevelButton(levelbtn4, 4, unlockedLevel);
    }

    private void SetupLevelButton(Button btn, int levelIndex, int unlockedLevel)
    {
        if (levelIndex <= unlockedLevel)
        {
            btn.SetEnabled(true); // mở nút
            btn.clicked += () => LoadLevel(levelIndex);
        }
        else
        {
            btn.SetEnabled(false); // khóa nút
        }
    }

    private void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene("Level " + levelIndex);
    }
}
