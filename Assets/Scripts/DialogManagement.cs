using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManagement : MonoBehaviour
{


[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] lines;
}

}
