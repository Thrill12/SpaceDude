using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Chest Armour")]
public class BaseChestArmour : BaseArmour
{
    public Sprite spriteNoWeaponsEquipped;
    [SerializeField, HideInInspector]
    public string spriteNoWeaponsEquippedPath;

    public Sprite spriteSmallWeaponEquipped;
    [SerializeField, HideInInspector]
    public string spriteSmallWeaponEquippedPath;

    public Sprite spriteLargeWeaponEquipped;
    [SerializeField, HideInInspector]
    public string spriteLargeWeaponEquippedPath;

    public BaseChestArmour()
    {
        itemSlot = "Chest";
    }
}
