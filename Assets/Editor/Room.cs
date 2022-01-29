using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Room : MonoBehaviour
{
    [Tooltip("The width of the room, from (0,0), from the bottom left."), Range(0, 20)]
    public int width;
    [Tooltip("The height of the room, from (0,0), from the bottom left."), Range(0, 20)]
    public int height;

    //For use in editor - generates room data from the tiles child obejcts of a room object for generating ships and stations with.
    public RoomData GenerateRoomData(string dir)
    {
        //Check room width  and height values are valid.
        if (width == 0 || height == 0)
        {
            Debug.LogError("Room width/height is 0 - this is invalid - please update the room width/height before generating room data.");
        }

        //Create the room data to be populated.
        RoomData roomData = ScriptableObject.CreateInstance<RoomData>(); 

        //Set the width and height values of the room data.
        roomData.roomHeight = height;
        roomData.roomWidth = width;

        //Make a list of tile data which will be populated with the tiles from the room object.
        List<TileData> tileData = new List<TileData>();
        //Get allt he Tile components in child objects which will be the tiles [floor and wall] that make up the room.
        Tile[] tiles = GetComponentsInChildren<Tile>();

        Debug.Log(tiles.Length);

        int counter = 0;

        //Loop over the tiles, create tile data foreach of the tiles and add them to the list of tile data.
        foreach (Tile tile in tiles)
        {
            TileData data = ScriptableObject.CreateInstance<TileData>();
            data.CreateDataFromTile(tile);
            tileData.Add(data);

            AssetDatabase.CreateAsset(data, dir + "Tile" + counter + ".asset");

            counter++;
        }

        //Set the room's tile data to the list of tile data created.
        roomData.roomTileData = tileData;

        //return the populated room data.
        return roomData;
    }
}
