using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManagement : MonoBehaviour
{
    public GameObject logo;
    public GameObject uiGameStart;
    public GameObject uiConfirmExit;
    private Button Startbtn;
    private Button Exitbtn;
    private Button Mapbtn;
    private Button yesExitBtn;
    private Button noExitBtn;

    void Start()
    {
        // Ban đầu: chỉ hiện logo, ẩn UI chính
        logo.SetActive(true);
        uiGameStart.SetActive(false);
        uiConfirmExit.SetActive(false);
        // Gọi hàm đổi sau 3 giây
        Invoke("ShowGameStartUI", 3f);

    }

    void ShowGameStartUI()
    {
        // Ẩn logo, hiện UI Game Start
        logo.SetActive(false);
        uiGameStart.SetActive(true);
    }
    private void OnEnable()
    {
        var root =uiGameStart.GetComponent<UIDocument>().rootVisualElement;
    
        Startbtn = root.Q<Button>("btn_start");
        Exitbtn = root.Q<Button>("btn_exit");
        Mapbtn = root.Q<Button>("btn_map");
       
        Startbtn.clicked += OnStartClicked;
        Mapbtn.clicked += OnMapClicked;
        Exitbtn.clicked += OnExitClicked;
       

       
    }
    void OnStartClicked()
    {
        Debug.Log("Start Clicked!");
        SceneManager.LoadScene("Level 1"); // Thay bằng tên scene thật
    }

    void OnMapClicked()
    {
        Debug.Log("Map Clicked!");
        SceneManager.LoadScene("Menu"); // Thay bằng tên scene thật
    }

    void OnExitClicked()
    {
        Debug.Log("Exit Clicked!");
        var rootE = uiConfirmExit.GetComponent<UIDocument>().rootVisualElement;
        yesExitBtn = rootE.Q<Button>("btn_yes");
        noExitBtn = rootE.Q<Button>("btn_no");

        uiGameStart.SetActive(false );
        uiConfirmExit.SetActive(true);
        yesExitBtn.clicked += ConfirmExit;
        noExitBtn.clicked += CancelExit;
    }
    void ConfirmExit()
    {
        Application.Quit();
        Debug.Log("Quit Confirmed.");
    }

    void CancelExit()
    {
        uiConfirmExit.SetActive(false);
    }
}
