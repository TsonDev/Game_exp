using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("Thông tin NPC")]
    public string npcName;
    public Sprite avatar;

    [Header("Dữ liệu thoại")]
    public TextAsset dialogueFile;
    private string[] dialogue;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject contButton;
    public GameObject pressEText;

    [Header("UI bổ sung")]
    public TextMeshProUGUI npcNameText;
    public Image npcAvatar;

    [Header("Tùy chỉnh")]
    public float wordSpeed = 0.05f;

    private int index = 0;
    private bool playerIsClose = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        if (dialogueFile != null)
        {
            dialogue = dialogueFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] NPC chưa có file thoại.");
            dialogue = new string[0];
        }

        zeroText();

        if (pressEText != null)
            pressEText.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            if (dialoguePanel.activeInHierarchy)
            {
                zeroText();
            }
            else
            {
                dialoguePanel.SetActive(true);

                // Gán UI ảnh + tên từ NPC này
                if (npcNameText != null)
                    npcNameText.text = npcName;

                if (npcAvatar != null)
                    npcAvatar.sprite = avatar;

                if (pressEText != null)
                    pressEText.SetActive(false);

                // Gán đúng sự kiện "Tiếp" cho NPC này
                Button btn = contButton.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(NextLine);

                typingCoroutine = StartCoroutine(Typing());
            }
        }

        if (dialogue.Length > 0 && index < dialogue.Length && dialogueText.text == dialogue[index])
        {
            contButton.SetActive(true);
        }
    }

    IEnumerator Typing()
    {
        dialogueText.text = "";

        if (dialogue == null || dialogue.Length == 0 || index >= dialogue.Length)
            yield break;

        foreach (char c in dialogue[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        contButton.SetActive(true);

        // Nếu đang gõ dở thì hiện nốt luôn
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = dialogue[index];
            return;
        }

        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            typingCoroutine = StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    public void zeroText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueText != null)
            dialogueText.text = "";

        index = 0;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (playerIsClose && pressEText != null)
            pressEText.SetActive(true);

        if (contButton != null)
        {
            contButton.SetActive(false);

            var btn = contButton.GetComponent<Button>();
            if (btn != null)
                btn.onClick.RemoveAllListeners();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;

            if (!dialoguePanel.activeInHierarchy && pressEText != null)
            {
                pressEText.SetActive(true);
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
