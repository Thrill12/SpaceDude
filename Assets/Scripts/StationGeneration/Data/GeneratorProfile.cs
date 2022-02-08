using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpaceCraftTpye {Station, Ship}

[CreateAssetMenu(menuName = "Generation/Create Generator Profile")]
public class GeneratorProfile : ScriptableObject
{
    [Header("Generation Parameters")]
    [Space(5)]
    //Determine what type of generation needs to be used
    public SpaceCraftTpye craftType;
    [Space(5)]
    //Space for faction that craft belongs too
    [Tooltip("The x and y size of the grid the craft is generated on.")]
    public int gridSize;
    [Space(5)]
    [Tooltip("The minimum number of rooms that must be generated.")]
    public int minNumRooms;
    [Tooltip("The maximum number of rooms that must could be generated.")]
    public int maxNumRooms;
    public int corridorWidth;
    [Tooltip("The first item should be the default - any other items should thus be variations of the default.")]
    public List<TileData> floorTile;
    [Tooltip("The first item should be the default - any other items should thus be variations of the default.")]
    public List<TileData> wallTiles;
    
    //Rooms this profile can use.
    public List<RoomData> rooms = new List<RoomData>();
}
