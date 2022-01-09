using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestDisplayHolder : MonoBehaviour
{
    public Quest questHeld;

    private bool hasMovedToBottom;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TMP_Text>().text = questHeld.QuestName;

        StartCoroutine(SpawnGoals());
    }

    IEnumerator SpawnGoals()
    {
        foreach (var item in questHeld.Goals)
        {
            GameObject goal = Instantiate(PrefabManager.instance.questGoalDisplay, transform);
            goal.GetComponent<QuestGoalDisplayHolder>().goalHeld = item;
            LeanTween.scaleY(goal, 1.1f, 0.1f);
            LeanTween.scaleY(goal, 1, 1f).setDelay(0.1f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        if (questHeld.Completed && !hasMovedToBottom)
        {
            transform.SetAsLastSibling();
        }
    }
}
