using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Back Armour")]
public class BaseBackArmour : BaseArmour
{
    public Sprite showableSprite;

    public BaseBackArmour()
    {
        itemSlot = "Back";
    }
}
