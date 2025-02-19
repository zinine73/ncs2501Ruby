using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float displayTime = 4.0f;
    public GameObject dialogBox;
    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private TextMeshProUGUI shadowText;
    private float timerDisplay;
    private int countRobotLeft;
    void Start()
    {
        timerDisplay = -1.0f;
        countRobotLeft = GameObject.FindGameObjectsWithTag("ENEMY").Length;
        SetDisplayText();
        dialogBox.SetActive(false);   
    }

    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
            }
        }   
    }

    public void DisplayDialog()
    {
        timerDisplay = displayTime;
        dialogBox.SetActive(true);
    }

    public bool NoticeRobotFixed()
    {
        countRobotLeft--;
        bool isCompleted = (countRobotLeft <= 0);
        // if (countRobotLeft <= 0)
        // {
        //     isCompleted = true;
        // }
        // else
        // {
        //     isCompleted = false;
        // }
        SetDisplayText(isCompleted);
        return isCompleted; 
    }

    private void SetDisplayText(bool isCompleted = false)
    {
        if (isCompleted)
        {
            questText.text = shadowText.text =
                $"Good job!\nYou made it!";
        }
        else
        {
            questText.text = shadowText.text =
                $"Help!\nFix the Robots!\nLeft : {countRobotLeft}";
        }
    }
}
