using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressSave
{
    public QuestsSavedSO questsSaved;
    public NPCStatesSave npcStates;
    public InventorySave inventorySave;
}

[System.Serializable]
public class NPCStatesSave
{
    public List<NPC> npcList = new List<NPC>();
}

[System.Serializable]
public class InventorySave
{
    public BaseItem[] playerInventoryItems;
    public BaseItem[] shipInventoryItems;
    public BaseItem[] itemsEquipped;
}
