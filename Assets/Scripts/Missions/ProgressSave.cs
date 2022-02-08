using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressSave
{
    public QuestsSavedSO questsSaved;
    public Dictionary<NPC, DialogueGraph> npcStates = new Dictionary<NPC, DialogueGraph>();
}
