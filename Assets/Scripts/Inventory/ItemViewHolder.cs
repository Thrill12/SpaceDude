using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemViewHolder : MonoBehaviour
{
    public StoredItem item;

    [Space(5)]

    public Sprite commonBorder;
    public Sprite rareBorder;
    public Sprite royalBorder;
    public Sprite ascendedBorder;

    private void Update()
    {
        if(item != null)
        {
            if(item.item.itemRarity.rarityName == "Common")
            {
                GetComponentInChildren<Image>().sprite = commonBorder;
            }
            else if(item.item.itemRarity.rarityName == "Rare")
            {
                GetComponentInChildren<Image>().sprite = rareBorder;
            }
            else if (item.item.itemRarity.rarityName == "Royal")
            {
                GetComponentInChildren<Image>().sprite = royalBorder;
            }
            else if (item.item.itemRarity.rarityName == "Ascended")
            {
                GetComponentInChildren<Image>().sprite = ascendedBorder;
            }
        }
    }
}
