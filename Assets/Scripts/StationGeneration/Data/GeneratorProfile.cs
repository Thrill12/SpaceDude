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
    [Tooltip("The first item should be the default - any other items should thus be variations of the default. Used when generating corridoors.")]
    public List<TileData> floorTile;
    [Tooltip("The first item should be the default - any other items should thus be variations of the default. Used when generating corridoors.")]
    public List<TileData> wallTiles;
    [Tooltip("The start room of the station; likely the player's entrance, if they don't improvise one of course. Used by generation to base locked doors and station progression.")]
    public RoomData startRoom;
    [Tooltip("The rooms - and their variations - which the generator will use to generate stations with.")]
    public List<RoomData> rooms = new List<RoomData>();
}
