using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressSave
{
    public QuestsSavedSO questsSaved;
    public NPCStatesSave npcStates;
    public InventorySave inventorySave;
    public DiscoveriesSave discoveriesSave;

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

    public int playerCredits;
}

[System.Serializable]
public class DiscoveriesSave
{
    public List<Location> locationsDiscovered = new List<Location>();
    public List<NPC> npcsDiscovered = new List<NPC>();
    public List<BaseItem> itemsDiscovered = new List<BaseItem>();
}
