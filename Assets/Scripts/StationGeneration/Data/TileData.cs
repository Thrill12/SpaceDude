using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The differem
public enum TileType { empty, floor, wall }

[System.Serializable]
public class TileData : ScriptableObject
{
    public Vector2Int position;
    public TileType type;
    public bool hasObject;

    //For creating tile data from a tile componenet within the editor.
    public void CreateDataFromTile(Tile tile)
    {
        //Get the tiles position.
        position = new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.y);
        //Get the tiles type.
        type = tile.type;
    }

    //For updating tiles when generating the crafts.
    public void UpdateTile(TileType tileType, bool hasObjectOnTile)
    {
        type = tileType;
        hasObject = hasObjectOnTile;
    }
}
