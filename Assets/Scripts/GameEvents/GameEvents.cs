using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    private void Awake()
    {
        instance = this;
    }

    //These events should be called whenever an action happens in the game world, could be used eg. for tracking progress in quests.

    #region EntityEvents
    public event Action<BaseEntity> onEntityKilled;
    public void OnEntityKilled(BaseEntity entity)
    {
        if(onEntityKilled != null)
        {
            onEntityKilled(entity);           
        }
    }
    #endregion

    #region LocationEvents
    public event Action<Location> onLocationEntered;
    public void OnLocationEntered(Location location)
    {
        if(onLocationEntered != null)
        {
            onLocationEntered(location);
        }
    }
    #endregion

    #region Miscellaneous Events

    public event Action<NPC> onNPCCommunicate;

    public void OnNPCCommunicate(NPC npc)
    {
        if(onNPCCommunicate != null)
        {
            onNPCCommunicate(npc);
        }
    }
    #endregion
}
