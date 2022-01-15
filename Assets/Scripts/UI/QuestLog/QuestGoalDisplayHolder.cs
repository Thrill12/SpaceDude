using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestGoalDisplayHolder : MonoBehaviour
{
    public Goal goalHeld;

    private bool hasRunTween = false;

    // This will make sure that once we finish a goal, it updates correctly in the quest log with the tick box
    void Update()
    {
        if (goalHeld.GoalCompleted && !hasRunTween)
        {
            GetComponentInChildren<Image>().enabled = true;
            Color color = GetComponent<TMP_Text>().color;
            hasRunTween = true;
        }

        if (goalHeld.GoalRequiredAmount > 0)
        {
            GetComponent<TMP_Text>().text = "- " + goalHeld.GoalCurrentAmount + "/" + goalHeld.GoalRequiredAmount + " " + goalHeld.GoalQuestLogDisplay;
        }
        else
        {
            GetComponent<TMP_Text>().text = "- " + goalHeld.GoalQuestLogDisplay;
        }
    }
}
