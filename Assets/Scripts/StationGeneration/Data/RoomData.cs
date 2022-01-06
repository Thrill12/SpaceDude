using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Generation/Create Room Data")]
public class RoomData : ScriptableObject
{
    [Tooltip("The width of the room, from (0,0), from the bottom left.")]
    public int totalWidth;
    [Tooltip("The height of the room, from (0,0), from the bottom left.")]
    public int totalHeight;
    [Tooltip("The list of tiles that form that room.")]
    public List<TileData> roomTileData;
}
