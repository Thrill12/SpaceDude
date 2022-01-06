using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MultipleInventoryView : MultiSOObserver
{
    public Camera uiCam;
    public GameObject itemPrefab;
    public GameObject gridPrefab;

    public WeaponsHolder weaponsHolder;
    public PlayerEntity player;

    public List<InventoryWithGrid> inventories;

    public List<GameObject> itemViews = new List<GameObject>();

    private GameObject movingObject;
    private StoredItem movingItem;
    private IntPair movingItemOrigPos;

    private void Start()
    {
        //Setting weapons holder if necessary, for equipping weapons
        foreach (var item in inventories)
        {
            if (typeof(PlayerInventory).IsAssignableFrom(item.inventory.GetType()))
            {
                PlayerInventory playerInv = (PlayerInventory)item.inventory;
                playerInv.weaponHolder = weaponsHolder;
                playerInv.playerEntity = player;
            }            
        }            
    }

    //Function ran whenever a thing in the observables changes, whether that is an item being added or removed.
    public override void Notify()
    {
        foreach (var item in inventories)
        {
            DrawInventory(item);
        }
    }

    protected override void OnEnable()
    {
        foreach (var item in inventories)
        {
            item.inventory.items = new List<StoredItem>();
        }

        base.OnEnable();

        DrawAll();
    }

    //Draws everything nice and neatly
    private void DrawAll()
    {
        itemViews.RemoveAll(x => x == null);

        foreach (var item in inventories)
        {
            CalculateSlotDimensions(item);
            DrawInventory(item);
        }        
    }

    private void DrawInventory(InventoryWithGrid invWithGrid)
    {
        CleanupGrid(invWithGrid);
        DrawGrid(invWithGrid);
        DrawItems(invWithGrid);      
    }

    private void DrawItems(InventoryWithGrid invWithGrid)
    {
        foreach (var item in invWithGrid.inventory.items)
        {
            DrawItem(item, invWithGrid);
        }
    }

    //This calculates the modifier for below
    private void CalculateSlotDimensions(InventoryWithGrid inventoryWithGrid)
    {
        float gridWidth = inventoryWithGrid.grid.rect.width;
        float gridHeight = inventoryWithGrid.grid.rect.height;

        inventoryWithGrid.unitSlot = new FloatPair(gridHeight / inventoryWithGrid.inventory.size.x, 
            gridWidth / inventoryWithGrid.inventory.size.y);
    }

    //This gets a multiplier to handle correct sizing of grid
    private FloatPair GetSlotPosition(int row, int col, InventoryWithGrid invWithGrid)
    {
        return new FloatPair(row * -invWithGrid.unitSlot.x, col * invWithGrid.unitSlot.y);
    }

    //Needs to figure out the position in the grid, eg. 0,0 or 5,2
    private void PositionInGrid(GameObject obj, IntPair position, IntPair size, InventoryWithGrid invWithGrid)
    {
        RectTransform transform = obj.GetComponent<RectTransform>();
        FloatPair gridPosition = GetSlotPosition(position.x, position.y, invWithGrid);
        transform.sizeDelta = new Vector2(invWithGrid.unitSlot.y * size.y, invWithGrid.unitSlot.x * size.x);
        transform.localPosition = new Vector3(gridPosition.y, gridPosition.x, 0f);
    }

    //Will draw the item into an itemView object, with the icon and any events it needs on hte button.
    private void DrawItem(StoredItem storedItem, InventoryWithGrid invWithGrid)
    {
        BaseItem item = storedItem.item;

        GameObject itemView = Instantiate(itemPrefab, invWithGrid.grid);

        PositionInGrid(itemView, storedItem.position, item.itemGridSize, invWithGrid);

        Image img = itemView.transform.Find("Icon").GetComponent<Image>();
        img.sprite = storedItem.item.itemIcon;

        storedItem.inventory = invWithGrid.inventory;

        itemView.GetComponent<ItemViewHolder>().item = storedItem;
        itemViews.Add(itemView);

        //This will handle any interaction between user and item, currently only clicking to move.
        Button btn = itemView.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (movingItem == null)
            {
                MoveItem(itemView, storedItem);
            }
        });        
    }

    //Starts to move the item selected to the cursor
    private void MoveItem(GameObject gridObj, StoredItem item)
    {
        movingObject = gridObj;
        movingItem = item;
        movingItemOrigPos = item.position;

        StartCoroutine(ItemMouseFollow());
    }

    private void RemoveItem(StoredItem item, InventoryWithGrid invWithGrid)
    {
        invWithGrid.inventory.RemoveItem(item);
    }

    //Destroys the grid objects
    private void CleanupGrid(InventoryWithGrid invWithGrid)
    {
        for (int i = 0; i < invWithGrid.grid.childCount; i++)
        {
            Destroy(invWithGrid.grid.GetChild(i).gameObject);
        }
    }

    IEnumerator ItemMouseFollow()
    {
        InventoryWithGrid invWithGrid = new InventoryWithGrid();

        //Keeps the object moving with the cursor
        while (!Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;          

            movingObject.transform.position = new Vector3(mousePosition.x - 0.1f, mousePosition.y + 0.1f, movingObject.transform.position.z);
            movingObject.transform.SetSiblingIndex(movingObject.transform.parent.childCount -1);

            foreach (var item in inventories)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(item.grid, movingObject.transform.position))
                {
                    invWithGrid = item;
                    break;
                }
            }            
            
            yield return null;
        }

        //This handles the swapping of inventories if an item is passed from a grid onto another, and also will equip the item if it enters a slot
        if(invWithGrid.inventory != movingItem.inventory)
        {         
            if (invWithGrid.inventory.AddItem(movingItem.item))
            {
                Inventory previousInventory = movingItem.inventory;
                movingItem.inventory.RemoveItem(movingItem);
                movingItem.inventory = invWithGrid.inventory;

                //Handling equipping weapons
                if (typeof(BaseWeapon).IsAssignableFrom(movingItem.item.GetType()))
                {
                    if (movingItem.inventory.inventoryName.ToLower().Contains("weapon"))
                    {
                        BaseWeapon weap = movingItem.item as BaseWeapon;

                        if(movingItem.inventory.inventoryName == weap.itemSlot)
                        {
                            weaponsHolder.EquipWeapon(weap);
                            weaponsHolder.SwapWeapons();
                            weap.hostEntity = player;
                            weap.OnEquip(weap.hostEntity);
                        }
                        else
                        {
                            movingItem.inventory = previousInventory;
                            RepositionMovingObject(invWithGrid);
                        }
                    }
                    else if (previousInventory.inventoryName.ToLower().Contains("weapon"))
                    {
                        BaseWeapon weap = movingItem.item as BaseWeapon;
                        weaponsHolder.UnequipWeapon(weap);
                        weaponsHolder.SwapWeapons();
                        weap.hostEntity = player;
                        weap.OnUnequip();
                    }
                }   
                //Here will handle equipping other types of weapons too
            }
        }

        //Handling swapping items, maybe the biggest pain in my butt since mayes
        if(invWithGrid.inventory.items.Count > 0)
        {
            StoredItem potentialItem = GetItemAtMouse(invWithGrid);

            if (potentialItem != null)
            {
                Debug.Log(potentialItem);

                SwapItemPositions(potentialItem);
            }
        }                   

        RepositionMovingObject(invWithGrid);

        DrawAll();
    }

    //Hella weird function that most of the time i'm not fully sure what it does but it works so hey.
    //Placement is still a bit wonky, but it works well enough for now
    private void SwapItemPositions(StoredItem itemToSwap)
    {
        if (movingItem.inventory.IsPositionValid(itemToSwap.item, movingItemOrigPos.x, movingItemOrigPos.y, movingItem))
        {
            if (itemToSwap.inventory.IsPositionValid(movingItem.item, itemToSwap.position.x, itemToSwap.position.y, itemToSwap))
            {
                if (movingItem.inventory != itemToSwap.inventory)
                {
                    Inventory temp = movingItem.inventory;
                    movingItem.inventory = itemToSwap.inventory;
                    itemToSwap.inventory = temp;
                }

                IntPair tempPos = movingItemOrigPos;
                movingItem.position = itemToSwap.position;
                itemToSwap.position = tempPos;
            }
            else
            {

            }
        }
        else
        {

        }           

        DrawAll();
    }

    //This will move the object to its correct slot in the grid, if possible, if not it will move back to original spot
    private void RepositionMovingObject(InventoryWithGrid invWithGrid)
    {
        CalculateSlotDimensions(invWithGrid);
        if (movingObject != null)
        {         
            if (!invWithGrid.inventory.MoveItem(movingItem, GetRoundedPosition(movingObject.transform.localPosition, invWithGrid)))
            {
                invWithGrid.inventory.MoveItem(movingItem, movingItemOrigPos);
            }

            movingObject = null;
            movingItem = null;
        }
    }

    //Gets rounded position for the grid placement
    private IntPair GetRoundedPosition(Vector2 floatPosition, InventoryWithGrid invWithGrid)
    {
        CalculateSlotDimensions(invWithGrid);

        int row = (int)(floatPosition.y / invWithGrid.unitSlot.x) * -1;
        int col = (int)(floatPosition.x / invWithGrid.unitSlot.y);

        IntPair res = new IntPair(row, col);

        return res;
    }

    //Gets the item under the mouse excluding the currently selected one to know which one to swap with. V V weird
    private StoredItem GetItemAtMouse(InventoryWithGrid invWithGrid)
    {
        foreach (var item in itemViews)
        {
            if(item != null)
            {
                if (item!= movingObject)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(item.GetComponent<RectTransform>(), Input.mousePosition))
                    {
                        return item.GetComponent<ItemViewHolder>().item;
                    }
                }                
            }
        }

        return null;
    }

    //Draws the whole grid, based on the size of the rect and of the inventory in the grid
    private void DrawGrid(InventoryWithGrid invWithGrid)
    {
        GameObject gridCell;
        IntPair unitPair = new IntPair(1, 1);

        for (int i = 0; i < invWithGrid.inventory.size.x; i++)
        {
            for (int j = 0; j < invWithGrid.inventory.size.y; j++)
            {
                gridCell = Instantiate(gridPrefab, invWithGrid.grid);
                PositionInGrid(gridCell, new IntPair(i, j), unitPair, invWithGrid);
            }
        }
    }
}

//Small helper class for multi-inventory transfers
[System.Serializable]
public class InventoryWithGrid
{
    public Inventory inventory;
    public RectTransform grid;
    [HideInInspector]
    public FloatPair unitSlot;
}
