using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Helmet Armour")]
public class BaseHelmet : BaseArmour
{
    public Sprite showableSprite;
    [SerializeField, HideInInspector]
    public string showableSpritePath;

    public BaseHelmet()
    {
        itemSlot = "Helmet";
    }
}
