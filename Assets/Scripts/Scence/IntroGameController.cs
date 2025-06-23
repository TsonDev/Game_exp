using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroGameController : MonoBehaviour
{
    public TextMeshProUGUI introText;
    public Image blackPanel;
    public float fadeSpeed = 1f;
    public float typingSpeed = 0.05f;
    public string nameScreen;

    [TextArea(2, 5)]
    public List<string> storyLines = new List<string>(); // 👈 Có thể nhập từ Inspector

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        blackPanel.color = Color.black;
        yield return StartCoroutine(Fade(false)); // Mở màn hình từ đen

        foreach (string line in storyLines)
        {
            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSeconds(1.5f); // Dừng sau khi viết xong 1 câu
        }

        yield return StartCoroutine(Fade(true)); // Kết thúc fade to black
        SceneManager.LoadScene(nameScreen);
    }

    IEnumerator TypeText(string line)
    {
        introText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            introText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator Fade(bool fadeToBlack)
    {
        float alpha = fadeToBlack ? 0 : 1;
        float target = fadeToBlack ? 1 : 0;

        while (Mathf.Abs(alpha - target) > 0.01f)
        {
            alpha = Mathf.MoveTowards(alpha, target, fadeSpeed * Time.deltaTime);
            blackPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
