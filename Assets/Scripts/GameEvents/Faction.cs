using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Faction")]
public class Faction : ScriptableObject
{
    public string factionName;
    public Sprite factionIcon;

    private int xpBacklog;
    public int factionCurrentXP;
    public int factionXPToNextLevel;
    public int factionLevel;

    public void AddXP(int xp)
    {
        Debug.Log("Adding " + xp + " to " + factionName);

        //Backlog of xp in case we receive more xp than required for next item level
        xpBacklog += xp;

        //Manages situation when we get more than enough xp to level up at least once, will keep leveling up.
        //using the XP it received.
        while (factionCurrentXP + xpBacklog >= factionXPToNextLevel)
        {
            int differenceToNextLevel = factionXPToNextLevel - factionCurrentXP;
            xpBacklog -= differenceToNextLevel;
            LevelUp();
            factionCurrentXP = 0;
        }

        factionCurrentXP += xpBacklog;
        xpBacklog = 0;
    }

    //That weird constant was chosen because it made req. XP for level 50 to be 500k, which was kinda perfect ;)
    public void LevelUp()
    {
        factionLevel++;
        factionXPToNextLevel = Mathf.RoundToInt(1000 * Mathf.Pow(1.69897001f, factionLevel - 1));

        OnLevelUp();
    }

    public virtual void OnLevelUp()
    {
        
    }
}
