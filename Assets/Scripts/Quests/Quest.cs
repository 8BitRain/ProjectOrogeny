using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public int id;
    public GameObject giveQuestText;

    private bool _questStarted = false;
    private bool _questCompleted = false;

    void Start()
    {
        HideGiveQuestText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowGiveQuestText()
    {
        giveQuestText.SetActive(true);
    }

    void HideGiveQuestText()
    {
        giveQuestText.SetActive(false);
    }

    void ShowQuestCompleteText()
    {

    }

    void HideQuestCompleteText()
    {

    }

    public void SetQuestStarted()
    {
        _questStarted = true;
        ShowGiveQuestText();
    }

    public bool GetQuestStarted()
    {
        return _questStarted;
    }

    public void SetQuestComplete()
    {
        _questCompleted = true;
        HideGiveQuestText();
    }

    public bool GetQuestComplete()
    {
        return _questCompleted;
    }

    public int GetQuestID()
    {
        return id;
    }
}
