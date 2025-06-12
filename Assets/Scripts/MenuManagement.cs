using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManagement : MonoBehaviour
{
    public GameObject logo;
    public GameObject uiGameStart;

    private VisualElement confirmExitElement;
    private Button Startbtn;
    private Button Exitbtn;
    private Button Mapbtn;
    private Button yesExitBtn;
    private Button noExitBtn;

    void Start()
    {
        logo.SetActive(true);
        uiGameStart.SetActive(false);

        Invoke("ShowGameStartUI", 3f);
    }

    void ShowGameStartUI()
    {
        logo.SetActive(false);
        uiGameStart.SetActive(true);
    }

    private void OnEnable()
    {
        var root = uiGameStart.GetComponent<UIDocument>().rootVisualElement;

        // Lấy các nút chính
        Startbtn = root.Q<Button>("btn_start");
        Exitbtn = root.Q<Button>("btn_exit");
        Mapbtn = root.Q<Button>("btn_map");

        Startbtn.clicked += OnStartClicked;
        Mapbtn.clicked += OnMapClicked;
        Exitbtn.clicked += OnExitClicked;

        // Lấy ConfirmExit
        confirmExitElement = root.Q<VisualElement>("ConfirmExit");

        // Lấy các nút Yes/No bên trong ConfirmExit
        var btnContainer = confirmExitElement.Q<VisualElement>("btn");
        yesExitBtn = btnContainer.Q<Button>("btn_yes");
        noExitBtn = btnContainer.Q<Button>("btn_no");

        yesExitBtn.clicked -= ConfirmExit;
        noExitBtn.clicked -= CancelExit;

        yesExitBtn.clicked += ConfirmExit;
        noExitBtn.clicked += CancelExit;

        // Ẩn ConfirmExit lúc đầu
        confirmExitElement.style.display = DisplayStyle.None;
    }

    void OnStartClicked()
    {
        Debug.Log("Start Clicked!");
        SceneManager.LoadScene("Level 1");
    }

    void OnMapClicked()
    {
        Debug.Log("Map Clicked!");
        SceneManager.LoadScene("Menu");
    }

    void OnExitClicked()
    {
        Debug.Log("Exit Clicked!");

        // Ẩn UI Game Start buttons, hiện ConfirmExit
        // (Bạn có thể chọn ẩn chỉ các button hoặc toàn bộ UI Game Start tùy ý)
        confirmExitElement.style.display = DisplayStyle.Flex;
    }

    void ConfirmExit()
    {
        Application.Quit();
        Debug.Log("Quit Confirmed.");
    }

    void CancelExit()
    {
        confirmExitElement.style.display = DisplayStyle.None;
        Debug.Log("Cancel Exit.");
    }
}
