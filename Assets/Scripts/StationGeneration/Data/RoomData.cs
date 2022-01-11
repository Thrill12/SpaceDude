using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Generation/Create Room Data")]
public class RoomData : ScriptableObject
{
    [Tooltip("The width of the room, from (0,0), from the bottom left."), Range(0,20)]
    public int roomWidth;
    [Tooltip("The height of the room, from (0,0), from the bottom left."), Range(0, 20)]
    public int roomHeight;
    [Tooltip("The list of tiles that form that room.")]
    public List<TileData> roomTileData;
}
