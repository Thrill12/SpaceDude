using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoSOObserver
{
    public Camera uiCam;
    public RectTransform itemsGrid;
    public Inventory inventory;
    public GameObject itemPrefab;
    public GameObject gridPrefab;

    public WeaponsHolder weaponsHolder;
    public PlayerEntity player;

    private FloatPair unitSlot;

    private GameObject movingObject;
    private StoredItem movingItem;
    private IntPair movingItemOrigPos;

    private void Start()
    {
        if (typeof(PlayerInventory).IsAssignableFrom(inventory.GetType()))
        {
            PlayerInventory playerInv = (PlayerInventory)inventory;
            playerInv.weaponHolder = weaponsHolder;
            playerInv.playerEntity = player;
        }
    }

    public override void Notify()
    {
        DrawInventory();
        Debug.Log("Notify");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CalculateSlotDimensions();
        DrawInventory();
        Debug.Log("OnEnable called");
    }

    private void DrawInventory()
    {
        CleanupGrid();    
        DrawGrid();
        DrawItems();
    }

    private void DrawItems()
    {
        foreach (var item in inventory.items)
        {
            DrawItem(item);
        }

        Debug.Log("DrawItems");
    }

    private void CalculateSlotDimensions()
    {
        float gridWidth = itemsGrid.rect.width;
        float gridHeight = itemsGrid.rect.height;

        unitSlot = new FloatPair(gridHeight / inventory.size.x, gridWidth / inventory.size.y);
    }

    private FloatPair GetSlotPosition(int row, int col)
    {
        return new FloatPair(row * -unitSlot.x, col * unitSlot.y);
    }

    private void PositionInGrid(GameObject obj, IntPair position, IntPair size)
    {
        RectTransform transform = obj.transform as RectTransform;
        FloatPair gridPosition = GetSlotPosition(position.x, position.y);
        transform.sizeDelta = new Vector2(unitSlot.y * size.y, unitSlot.x * size.x);
        transform.localPosition = new Vector3(gridPosition.y, gridPosition.x, 0f);
    }

    private void DrawItem(StoredItem storedItem)
    {
        BaseItem item = storedItem.item;

        GameObject itemView = Instantiate(itemPrefab, itemsGrid);
        PositionInGrid(itemView, storedItem.position, item.itemGridSize);
        Image img = itemView.transform.Find("Icon").GetComponent<Image>();
        img.sprite = storedItem.item.itemIcon;
        Button btn = itemView.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if(movingItem == null)
            {
                MoveItem(itemView, storedItem);
            }
        });

        Debug.Log("Drawn " + storedItem.item.itemName);
    }   

    private void MoveItem(GameObject gridObj, StoredItem item)
    {
        movingObject = gridObj;
        movingItem = item;
        movingItemOrigPos = item.position;

        StartCoroutine(ItemMouseFollow());
    }

    private void RemoveItem(StoredItem item)
    {
        inventory.RemoveItem(item);
    }

    private void CleanupGrid()
    {
        for (int i = 0; i < itemsGrid.childCount; i++)
        {
            Destroy(itemsGrid.GetChild(i).gameObject);
        }
    }

    IEnumerator ItemMouseFollow()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            Debug.Log(Input.mousePosition);
            Vector2 mousePosition = Input.mousePosition;
            Vector2 relativePosition = new Vector2(
                mousePosition.x - itemsGrid.sizeDelta.x,
                itemsGrid.sizeDelta.y + mousePosition.y - itemsGrid.sizeDelta.y
                );
            movingObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, movingObject.transform.position.z);

            yield return null;
        }

        RepositionMovingObject();
    }

    private void RepositionMovingObject()
    {
        if(movingObject != null)
        {
            int row = (int)(movingObject.transform.localPosition.y / unitSlot.x) * -1;
            int col = (int)(movingObject.transform.localPosition.x / unitSlot.y);
            
            if(!inventory.MoveItem(movingItem, new IntPair(row, col)))
            {
                inventory.MoveItem(movingItem, movingItemOrigPos);
            }

            movingObject = null;
            movingItem = null;
        }
    }       

    private void DrawGrid()
    {
        GameObject gridCell;
        IntPair unitPair = new IntPair(1, 1);

        for (int i = 0; i < inventory.size.x; i++)
        {
            for (int j = 0; j < inventory.size.y; j++)
            {
                gridCell = Instantiate(gridPrefab, itemsGrid);
                PositionInGrid(gridCell, new IntPair(i, j), unitPair);
            }
        }
    }
}
