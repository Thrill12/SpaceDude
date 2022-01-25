using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Crafted Resource")]
public class CraftedResource : BaseItem
{
    [Header("Crafted Resource")]

    public List<BaseItem> ingredients = new List<BaseItem>();
}
