using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    private int index;

    public GameObject contButton;
    public GameObject pressEText; // 👉 UI hiển thị "Nhấn E để nói chuyện"

    public float wordSpeed = 0.05f;
    public bool playerIsClose;

    private Coroutine typingCoroutine;

    private void Start()
    {
        zeroText();

        if (pressEText != null)
            pressEText.SetActive(false); // Ẩn chữ "Nhấn E" ban đầu
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && playerIsClose)
        {
            if (dialoguePanel.activeInHierarchy)
            {
                zeroText();
            }
            else
            {
                dialoguePanel.SetActive(true);

                // Ẩn chữ "Nhấn E" khi bắt đầu nói chuyện
                if (pressEText != null)
                    pressEText.SetActive(false);

                typingCoroutine = StartCoroutine(Typing());
            }
        }

        if (dialogueText.text == dialogue[index])
        {
            contButton.SetActive(true);
        }
    }

    IEnumerator Typing()
    {
        dialogueText.text = "";
        foreach (char c in dialogue[index].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        contButton.SetActive(false);

        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);

        // Nếu người chơi vẫn còn gần, hiện lại "Nhấn E"
        if (playerIsClose && pressEText != null)
        {
            pressEText.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;

            // 👉 Khi vào vùng NPC, hiện "Nhấn E"
            if (!dialoguePanel.activeInHierarchy && pressEText != null)
            {
                pressEText.SetActive(true);
                Debug.Log("Hiện: Nhấn E để nói chuyện");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();

            if (pressEText != null)
                pressEText.SetActive(false);
        }
    }
}
