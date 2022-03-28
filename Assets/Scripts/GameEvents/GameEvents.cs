using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    //These events should be called whenever an action happens in the game world, could be used eg. for tracking progress in quests.

    #region EntityEvents
    public event Action<BaseEntity, BaseEntity> onEntityKilled;
    public void OnEntityKilled(BaseEntity victim, BaseEntity killer)
    {
        if(onEntityKilled != null)
        {
            onEntityKilled(victim, killer);           
        }
    }

    public event Action<BaseEntity, BaseEntity, float, EffectToCheck> onEntityHit;

    public void OnEntityHit(BaseEntity victim, BaseEntity hitter, float damage, EffectToCheck effectFlags)
    {
        if(onEntityHit != null)
        {
            onEntityHit(victim, hitter, damage, effectFlags);
        }
    }

    public event Action<NPC> onNPCCommunicate;

    public void OnNPCCommunicate(NPC npc)
    {
        if (onNPCCommunicate != null)
        {
            onNPCCommunicate(npc);
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

    public event Action<BaseItem> onItemPickedUp;

    public void OnItemPickedUp(BaseItem item)
    {
        if(item != null)
        {
            onItemPickedUp(item);
        }
    }

    #endregion
}
