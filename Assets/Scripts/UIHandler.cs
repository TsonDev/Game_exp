using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    private VisualElement m_Healthbar;
    private VisualElement m_Manabar;
    private VisualElement m_Stackbar;
    public static UIHandler instance { get; private set; }


    // Awake is called when the script instance is being loaded (in this situation, when the game scene loads)
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        m_Manabar = uiDocument.rootVisualElement.Q<VisualElement>("ManaBar");
        m_Stackbar = uiDocument.rootVisualElement.Q<VisualElement>("StackSkillBar");
        SetHealthValue(1.0f);
        SetManaValue(1.0f);
        SetStackValue(1.0f);
    }




    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);


    }
    public void SetManaValue(float percentage)
    {
        m_Manabar.style.width = Length.Percent(100 * percentage);


    }
    public void SetStackValue(float percentage)
    {
        m_Stackbar.style.width = Length.Percent(100 * percentage);


    }

}
