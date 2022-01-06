using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Inventory"), System.Serializable]
public class Inventory : ObservableSO
{
    public string inventoryName;
    public IntPair size;
    public List<StoredItem> items;    

    public bool IsPositionValid(BaseItem item, int row, int col, StoredItem ignoreWith = null)
    {
        return InBounds(item.itemGridSize, row, col) && !IsColliding(item.itemGridSize, row, col, ignoreWith);
    }

    //Finds the next valid position for the item in the grid
    public IntPair FindValidPosition(BaseItem item)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if(IsPositionValid(item, i, j))
                {
                    return new IntPair(i, j);
                }
            }
        }

        return null;
    }

    public bool MoveItem(StoredItem toMove, IntPair newPos)
    {
        if (IsPositionValid(toMove.item, newPos.x, newPos.y, toMove))
        {
            toMove.position = newPos;
            Notify();
            return true;
        }
        else
        {
            return false;
        }
    }


    public virtual bool AddItem(BaseItem item)
    {
        int totalSize = item.itemGridSize.x * item.itemGridSize.y;
        if(FreeSlotsCount() >= totalSize)
        {
            IntPair position = FindValidPosition(item);
            if(position != null)
            {
                items.Add(new StoredItem(item, position));
                Notify();
                return true;
            }
        }

        return false;
    }

    //Checks if item to be moved of the size in that position fits into the box
    public bool InBounds(IntPair itemSize, int row, int col)
    {
        return row >= 0 && row <= size.x && 
            row + itemSize.x <= size.x && 
            col >= 0 && col <= size.y &&
            col + itemSize.y <= size.y;
    }

    public void RemoveItem(StoredItem item)
    {
        items.Remove(item);
        Notify();
    }

    //Counts total number of tiles available to skip past a load of other checks later on
    private int FreeSlotsCount()
    {
        int occupied = 0;
        foreach (var item in items)
        {
            occupied += item.item.itemGridSize.x * item.item.itemGridSize.y;
        }

        return size.x * size.y - occupied;
    }

    //Checking if an item is colliding with another
    public bool IsColliding(IntPair itemSize, int row, int col, StoredItem ignoreWith = null)
    {
        foreach (var item in items)
        {
            if(ABBintersectsABB(item.position.y, item.position.x, item.item.itemGridSize.y, 
                item.item.itemGridSize.x, col, row, itemSize.y, itemSize.x) && 
                item != ignoreWith)
            {
                return true;
            }
        }

        return false;
    }

    private bool ABBintersectsABB(int ax, int ay, float aw, float ah, int bx, int by, float bw, float bh)
    {
        //Box to box collision test apparently, have no clue how this works but yeah :) - basically comparing the corners
        return (ax < bx + bw &&
            ax + aw > bx &&
            ay < by + bh &&
            ah + ay > by);
    }
}
