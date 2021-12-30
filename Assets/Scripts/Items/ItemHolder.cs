using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public BaseItem itemHeld;
    private TMP_Text itemNameText;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        itemHeld = ScriptableObject.Instantiate(itemHeld);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = itemHeld.itemRarity.rarityColor;
        itemNameText = GetComponentInChildren<TMP_Text>();
        itemNameText.text = itemHeld.itemName;
        if (itemHeld.itemIcon != null)
        {
            spriteRenderer.sprite = itemHeld.itemIcon;
        }

        if (typeof(BaseEquippable).IsAssignableFrom(itemHeld.GetType()))
        {
            BaseEquippable equippable = (BaseEquippable)itemHeld;
            equippable.isEquipped = false;
        }
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider == gameObject.GetComponent<Collider2D>())
        {
            itemNameText.gameObject.SetActive(true);
        }
        else
        {
            itemNameText.gameObject.SetActive(false);
        }
    }

    public void ClickedOn(GameObject player)
    {
        if (player.GetComponent<Inventory>().items.Count < 30)
        {
            player.GetComponent<Inventory>().AddItemToInventory(itemHeld);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory full");
        }
    }
}
