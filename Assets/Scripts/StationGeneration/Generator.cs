using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 struct PlacedRoom
{
    public Vector2Int position;
    public int roomWidth;
    public int roomHeight;

    public PlacedRoom(RoomData room, Vector2Int pos)
    {
        position = pos;
        roomWidth = room.roomWidth;  
        roomHeight = room.roomHeight;    
    }
}

//Creates the data representations.
public class Generator 
{
    public GeneratorProfile profile;
    public int seed = 0;

    public TileData[,] grid { get; protected set; }
    private List<PlacedRoom> placedRooms;

    public Generator(GeneratorProfile generatorProfile, int generatorSeed)
    {
        //Assigning the arguements for generation.
        seed = generatorSeed;
        profile = generatorProfile;

        //Create the tile grid which the x and y size are the grid size: e.g. 50x50.
        grid = new TileData[profile.gridSize, profile.gridSize];

        for (int x = 0; x < profile.gridSize; x++)
        {
            for (int y = 0; y < profile.gridSize; y++)
            {
                grid[x, y] = ScriptableObject.CreateInstance<TileData>();
            }
        }

        //Create a list of placed room data which will hold the placed rooms - bascially noraml room data minus the tiles. 
        placedRooms = new List<PlacedRoom>();

        //Initialise the random number generator with the seed;
        //allows for the same ship/station to generate from the same seed.
        Random.InitState(seed);
    }

    public Texture2D GenerateWithDebugTexture()
    {
        GenerateRooms();

        Texture2D texture = DebugTexture();

        return texture;
    }

    public void GenerateStation()
    {
        #region Staion Gen Procedure

        //  1) Generate the rooms for the station - a random number of rooms between minRooms and maxRooms. 
        //      a) A random room is selected from the list of possible rooms to spawn. 
        //      b) The room is attmepted to be placed at random locations. 
        //      c) Repeat 1) and 2) until the desired number of rooms are placed.
        //  2) Generate corridoors which connect the rooms. 
        //  3) Create a Diskstra Map 
        //  4) Adding objects and gameplay elements.

        #endregion
    }

    public void GenerateShip()
    {

    }

    #region Placing Rooms

    private void GenerateRooms()
    {
        //Returns a random integer betweeen the min number of rooms and the max number of rooms.
        int numToSpawn = Random.Range(profile.minNumRooms,profile.maxNumRooms + 1);

        //Generate the rooms. Will generate the number of rooms set in numToSpawn.
        for (int i = 0; i < numToSpawn; i++)
        {
            bool placed;

            //Each room is attemtped to be placed five times,
            //if it is not placed then try a different room.
            do
            {
                //Select a random room from the list of rooms in the generator profile.
                RoomData room = profile.rooms[Random.Range(0, profile.rooms.Count)];

                //Attempts to place the room, if fails after 5 attempts, trys a different room.
                placed = PlaceRoom(room);
            } while (!placed);
        }
    }

    private bool PlaceRoom(RoomData room)
    {
        //Attmept to place the room
        for (int i = 0; i < 5; i++)
        {
            //Work out the max possible x and y values which would allow the room to be placed on the grid.
            int maxX = profile.gridSize - room.roomWidth;
            int maxY = profile.gridSize - room.roomHeight;

            //Pick a random grid pos to spawn the room between 0 and the max x/y respectively. 
            int x = Random.Range(0, maxX + 1);
            int y = Random.Range(0, maxY + 1);
            Vector2Int pos = new Vector2Int(x, y);    

            //Check that the position isn't overlapping. If not overlapping, commit the room to the grid.
            if (IsRoomPlacementValid(room, pos))
            {
                //Commit this room to the grid.
                AddRoomToGrid(room, pos);
                Debug.Log("Room placed");
                return true;
            }
        }

        //The room failed to be placed within 5 attempts.
        return false;
    }

    //Checks the room placement - returns true if it is valid, false when invalid.
    private bool IsRoomPlacementValid(RoomData room, Vector2Int pos)
    {
        if (placedRooms.Count > 0)
        {
            //Checks against all other placed rooms if they overlap at all.
            foreach (PlacedRoom placed in placedRooms)
            {
                #region Rectangle Overlap Check

                //Rectangle 1 - the placed room.
                int r1x1 = placed.position.x;
                int r1y1 = placed.position.y + placed.roomHeight;
                int r1x2 = placed.position.x + placed.roomWidth;
                int r1y2 = placed.position.y;

                //Rectange 2 - the room we're checking placement of. 
                int r2x1 = pos.x;
                int r2y1 = pos.y + room.roomHeight;
                int r2x2 = pos.x + room.roomWidth;
                int r2y2 = pos.y;

                //Complete the check. If all conditions are met then there's and overlap.
                if (r1x1 < r2x2 && r1x2 > r2x1 && r1y1 > r2y2 && r1y2 < r2y1)
                {
                    //The rooms overlap and the placement is invalid.
                    return false;
                }
                #endregion
            }
        }

        //The room placement is valid as room doesn't overlap any placed rooms.
        return true;
    }

    //Commits the rooms tiles to the craft's grid.
    private void AddRoomToGrid(RoomData room, Vector2Int pos)
    {
        //Add the room to the list of placed rooms.
        placedRooms.Add(new PlacedRoom(room, pos));

        //Loop over the tiles of the room and set the tiles on the grid.
        foreach(TileData tile in room.roomTileData)
        {
            Debug.Log(tile);
            Debug.Log(pos);

            //Get the craft grid position this tile should be on .
            Vector2Int gridPos = tile.position + pos;
            //Update the grid with the tile.
            grid[gridPos.x, gridPos.y].UpdateTile(tile.type, tile.hasObject);
        }
    }

    #endregion

    #region Generating Corridoors

    private void CorridorFloorWalk()
    {

    }

    private void GenerateCorridorWalls()
    {

    }
    #endregion

    #region Debugging 

    Texture2D DebugTexture()
    {
        Texture2D tex = new Texture2D(profile.gridSize, profile.gridSize, TextureFormat.ARGB32, false);

        for (int x = 0; x < profile.gridSize; x++)
        {
            for (int y = 0; y < profile.gridSize; y++)
            {
                if (grid[x,y] != null)
                {
                    switch (grid[x, y].type)
                    {
                        case TileType.empty:
                            tex.SetPixel(x, y, Color.black);
                            break;
                        case TileType.floor:
                            tex.SetPixel(x, y, Color.white);
                            break;
                        case TileType.wall:
                            tex.SetPixel(x, y, Color.blue);
                            break;
                        default:
                            break;
                    }
                }  
            }
        }
        tex.Apply();
        return tex;
    }

    #endregion
}
