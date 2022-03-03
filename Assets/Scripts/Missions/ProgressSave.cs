using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressSave
{
    public QuestsSavedSO questsSaved;
    public NPCStatesSave npcStates;
    public InventorySave inventorySave;

    public Vector3 playerShipLocation = new Vector3();
    public Vector3 playerShipRotation = new Vector3(0, 0, 270);
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
