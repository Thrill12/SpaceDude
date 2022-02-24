using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestsSavedSO
{
    public List<Quest> questsCompleted = new List<Quest>();
    public List<Quest> questsActive = new List<Quest>();
}
