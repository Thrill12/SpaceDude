using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

 public struct PlacedRoom
{
    public Vector2Int position;
    public int roomWidth;
    public int roomHeight;
    public List<Vector2Int> entrances;

    public PlacedRoom(RoomData room, Vector2Int pos)
    {
        position = pos;
        roomWidth = room.roomWidth;  
        roomHeight = room.roomHeight;    
        entrances = room.entrances;
    }
}

struct Corridor
{
    public PlacedRoom room1;
    public PlacedRoom room2;
    public List<Vector2Int> xtilesPos;
    public List<Vector2Int> ytilesPos;
}

//Creates the data representations.
public class Generator 
{
    public GeneratorProfile profile;
    public int seed = 0;

    public TileData[,] grid { get; protected set; }
    private List<PlacedRoom> placedRooms;

    TriangulationGraph tGraph;

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

    public Texture2D GenerateForDebugging()
    {
        List<Vector2Int> rooms = new List<Vector2Int>();
        tGraph = new TriangulationGraph();

        GenerateRooms();

        List<TriangulationGraph.Edge> graph = tGraph.DelenauyTriangulation(RoomPositions(), profile.gridSize);

        List<TriangulationGraph.Edge> mstGraph = tGraph.MinimumSpanningTree(graph);

        //Add corridors.
        int[,] mstTable = tGraph.GraphAdjacencyTable(mstGraph, tGraph.rooms, false);
        GenerateCorridors(mstTable);

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

    #region Grid Utilities

    TileData GetTile(int x, int y)
    {
        TileData t = null ;

        if (x < profile.gridSize && y < profile.gridSize)
        {
            //The tile attmepted to be accessed is within our grid so find it.
            t = grid[x, y];
        }

        return t;
    }

    //Returns true if successful.
    bool SetTile(int x, int y, TileData tile)
    {
        bool flag = false;

        if (x < profile.gridSize && y < profile.gridSize)
        {
            //The tile attmepted to be accessed is within our grid so set it.
            grid[x, y].UpdateTile(tile.type, false);
            flag = true;
        }

        return flag;
    }

    #endregion

    #region Placing Rooms

    public void GenerateRooms()
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
            int maxX = profile.gridSize - 1 - room.roomWidth;
            int maxY = profile.gridSize - 1 - room.roomHeight;

            //Pick a random grid pos to spawn the room between 0 and the max x/y respectively. 
            int x = Random.Range(0, maxX + 1);
            int y = Random.Range(0, maxY + 1);
            Vector2Int pos = new Vector2Int(x, y);    

            //Check that the position isn't overlapping. If not overlapping, commit the room to the grid.
            if (IsRoomPlacementValid(room, pos))
            {
                //Commit this room to the grid.
                AddRoomToGrid(room, pos);
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
            //Get the craft grid position this tile should be on .
            Vector2Int gridPos = tile.position + pos;
            //Update the grid with the tile.
            grid[gridPos.x, gridPos.y].UpdateTile(tile.type, tile.hasObject);
        }
    }

    #endregion

    #region Generating Corridoors

    public void GenerateCorridors(int[,] mstTable)
    {
        List<Corridor> corridors = CalculateCorridoorsPaths(mstTable);
        PlaceCorridors(corridors);
    }

    //Calculate the corridor paths which need tracing and corridor rooms generated on..
    private List<Corridor> CalculateCorridoorsPaths(int[,] mstTable)
    {
        List<Corridor> corridors = new List<Corridor>();

        //Make a list of corridors which need tracing.
        for (int from = 0; from < placedRooms.Count; from++)
        {
            for (int to = 0; to < placedRooms.Count; to++)
            {
                //Is there a connection here?
                if (mstTable[from,to] >= 1)
                {
                    //Remove this connection from the table since we'll be sorting this edge now.
                    mstTable[from, to] = 0;
                    mstTable[to, from] = 0;

                    //For the 'from' room, get the point the corridor should aim for.
                    Vector2Int fromCorridorPos = RoomCoridoorConnection(from);
                    //For the 'to' room, get the point the corridor should aim for.
                    Vector2Int toCorridorPos = RoomCoridoorConnection(to);

                    //Create the corridor that needs tracing and add to the list of coridors we are making.
                    Corridor corridor = new Corridor
                    {
                        room1 = placedRooms[from],
                        room2 = placedRooms[to],
                        xtilesPos = new List<Vector2Int>(),
                        ytilesPos = new List<Vector2Int>()
                    };

                    //Work out if we are doing x or y first walk. 
                    if (Random.Range(0,2) == 1)
                    {
                        //Completes the initial outline of the corridor for this edge.
                        XWalk(fromCorridorPos, toCorridorPos, ref corridor);

                        if (corridor.xtilesPos.Count > 0)
                        {
                            fromCorridorPos = corridor.xtilesPos[corridor.xtilesPos.Count - 1];
                        }

                        YWalk(fromCorridorPos,toCorridorPos, ref corridor);
                    }
                    else
                    {
                        //Completes the initial outline of the corridor for this edge.
                        YWalk(fromCorridorPos, toCorridorPos, ref corridor);

                        if (corridor.ytilesPos.Count > 0)
                        {
                            fromCorridorPos = corridor.ytilesPos[corridor.ytilesPos.Count - 1];
                        }

                        XWalk(fromCorridorPos, toCorridorPos, ref corridor);
                    }

                    corridors.Add(corridor);
                }
            }
        } 

        //Return the completed list of corridors.
        return corridors;
    }

    void XWalk(Vector2Int fromCorridorPos, Vector2Int toCorridorPos, ref Corridor corridor)
    {
        //Do x first. //On the X, get the from pos to the to pos yk. 
        if (fromCorridorPos.x != toCorridorPos.x)
        {
            //Work out if to pos is to the left or to the right...
            if (fromCorridorPos.x > toCorridorPos.x)
            {
                //The 'to' room is on the left.
                //Keep 'walking left' until we match with the target 'to' x.
                //Calculate the difference between the current x pos and the target x pos.
                int steps = Mathf.Abs(fromCorridorPos.x - toCorridorPos.x);

                for (int i = 0; i <= steps; i++)
                {
                    //Walk left.
                    int x = fromCorridorPos.x - i;
                    int y = fromCorridorPos.y;

                    corridor.xtilesPos.Add(new Vector2Int(x, y));
                }
            }
            else
            {
                //The 'to' romm is on the right.
                //Keep 'walking right' until we match with the target 'to' x.
                int steps = Mathf.Abs(fromCorridorPos.x - toCorridorPos.x);

                for (int i = 0; i < steps; i++)
                {
                    //Walk left.
                    int x = fromCorridorPos.x + i;
                    int y = fromCorridorPos.y;

                    corridor.xtilesPos.Add(new Vector2Int(x, y));
                }

            }
        }
    }

    void YWalk(Vector2Int fromCorridorPos, Vector2Int toCorridorPos, ref Corridor corridor)
    {
        //On the Y, get the from pos to the to pos yk. 
        if (fromCorridorPos.y != toCorridorPos.y)
        {
            //Work out if 'to pos' is up or down...
            if (fromCorridorPos.y > toCorridorPos.y)
            {
                //The 'to' room is below.
                //Keep 'walking down' until we match with the target 'to' y.
                int steps = Mathf.Abs(fromCorridorPos.y - toCorridorPos.y);

                for (int i = 0; i < steps; i++)
                {
                    //Walk left.
                    int x = fromCorridorPos.x;
                    int y = fromCorridorPos.y - i;

                    corridor.ytilesPos.Add(new Vector2Int(x, y));
                }
            }
            else
            {
                //The 'to' romm is above.
                //Keep 'walking up' until we match with the target 'to' y.
                int steps = Mathf.Abs(fromCorridorPos.y - toCorridorPos.y);

                for (int i = 0; i < steps; i++)
                {
                    //Walk left.
                    int x = fromCorridorPos.x;
                    int y = fromCorridorPos. y + i;

                    corridor.ytilesPos.Add(new Vector2Int(x, y));
                }

            }
        }
    }

    void PlaceCorridors(List<Corridor> corridors)
    {
        foreach(Corridor corridor in corridors)
        {
            Debug.Log("Corridors");

            //Trace all the x-walk tiles.
            foreach (Vector2Int pos in corridor.xtilesPos)
            {
                //The tile data to use for this pos.
                TileData data = profile.floorTile[Random.Range(0, profile.floorTile.Count)];

                //Set the tile of the position on the grid discovered drawing the base path of the corridor. 
                TraceCorridorTile(pos, data);
            }

            //Trace all the y-walk tiles.
            foreach (Vector2Int pos in corridor.ytilesPos)
            {
                TileData data = profile.floorTile[Random.Range(0, profile.floorTile.Count)];

                //Set the tile of the position on the grid discovered drawing the base path of the corridor. 
                TraceCorridorTile(pos, data);
            }
        }
    }

    void TraceCorridorTile(Vector2Int position, TileData data)
    {
        //Set the tile of the position on the grid discovered drawing the base path of the corridor. 
        SetTile(position.x, position.y, data);

        //Now trace around that position to make the corridor the desired width. 
        //Trace the positions NESW.

        //Set tile at the position on the acutal path.
        if (position.x + 1 < profile.gridSize - 1 && position.y + 1 < profile.gridSize - 1)
        {
            SetTile(position.x, position.y, data);
        }

        Vector2Int tracePos;

        //Try going each direction for the width of a corridor ( - 1 since we already set 1 tile wide to follow the path).

        //Start with up 1.
        for (int i = 1; i <= profile.corridorWidth-1; i++)
        {
            tracePos = new Vector2Int(position.x, position.y + i);

            if (tracePos.x + 1 < profile.gridSize - 1 && tracePos.y + 1 < profile.gridSize - 1)
            {
                SetTile(tracePos.x, tracePos.y, profile.floorTile[0]);
            }
        }

        //Then with down 1.
        for (int i = 1; i <= profile.corridorWidth - 1; i++)
        {
            tracePos = new Vector2Int(position.x, position.y - i);

            if (tracePos.x + 1 < profile.gridSize - 1 && tracePos.y + 1 < profile.gridSize - 1)
            {
                SetTile(tracePos.x, tracePos.y, profile.floorTile[0]);
            }
        }

        //Then with left 1.
        for (int i = 1; i <= profile.corridorWidth - 1; i++)
        {
            tracePos = new Vector2Int(position.x - i, position.y);

            if (tracePos.x + 1 < profile.gridSize - 1 && tracePos.y + 1 < profile.gridSize - 1)
            {
                SetTile(tracePos.x, tracePos.y, profile.floorTile[0]);
            }
        }

        //Then with right 1.
        for (int i = 1; i <= profile.corridorWidth - 1; i++)
        {
            tracePos = new Vector2Int(position.x + i, position.y);

            if (tracePos.x + 1 < profile.gridSize - 1 && tracePos.y + 1 < profile.gridSize - 1)
            {
                SetTile(tracePos.x, tracePos.y, profile.floorTile[0]);
            }
        }
    }

    private Vector2Int RoomCoridoorConnection(int fromIndex)
    {
        Vector2Int fromCorridorPos;
        //Decide if we are using the room centre or a specific target.
        if (placedRooms[fromIndex].entrances.Count > 0)
        {
            //Pick a target entrance if there is more than one entrance.
            if (placedRooms[fromIndex].entrances.Count > 1)
            {
                //Pick a random index.
                int index = Random.Range(0, placedRooms[fromIndex].entrances.Count);
                //Calcuate the true grid relative position.
                fromCorridorPos = placedRooms[fromIndex].entrances[index] + placedRooms[fromIndex].position;
            }
            else
            {
                //Calcuate the true grid relative position.
                fromCorridorPos = placedRooms[fromIndex].entrances[0] + placedRooms[fromIndex].position;
            }
        }
        else
        {
            //Work out where the room centre is, make that the corridor target.
            Vector2Int roomCentre = placedRooms[fromIndex].position + new Vector2Int(Mathf.Abs(placedRooms[fromIndex].roomWidth / 2), Mathf.Abs(placedRooms[fromIndex].roomHeight / 2));

            //Set the room centre as the corridor target.
            fromCorridorPos = roomCentre;
        }

        return fromCorridorPos;
    }
    #endregion

    #region Gameplay Mechanics

    //Will identify branches in the minimum spanning tree which can be locked.
    void FindAndLockRooms()
    {
        //Start by finding which room is our start room, this will be a room which is most near the outer edges of the grid.

        int startRoomIndex = FindStartRoom();

        //Any room can be lockable...
        //but weighting should make branched rooms least likely to be locked as this quickly locks large portions of a stations and makes key placements less interesting and exploration more linear.
        //Locked rooms effectively create subtrees which we can use to hide keys in.

        //Identify which rooms to lock then lock them.
    }

    //Will for a given room place a locked door, and find a suitable place to put a key earlier in the tree based on a given start room.
    void PlaceLockKey(PlacedRoom room, int startRoomIndex)
    {

    }

    //Picks a starting rooom which is furthest from the grid centre (so we know it is accessible with the ship).
    int FindStartRoom()
    {
        int index = 0;
        int highestDist = 0;

        //Find room distance from the centre of the grid, we want the most outside.
        foreach (PlacedRoom room in placedRooms)
        {
            int dist = (int)Vector2.Distance(room.position, new Vector2(profile.gridSize / 2, profile.gridSize / 2));

            if (dist > highestDist) 
            { 
                highestDist = dist;
                index = placedRooms.IndexOf(room);
            } 
        }

        return index;
    }

    #endregion

    #region Outputs 

    //Generates a debug texture from a generated grid.
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

    //Returns a list of nodes (rooms).
    public List<Vector2Int> RoomPositions()
    {
        List<Vector2Int> pos = new List<Vector2Int>();

        foreach (PlacedRoom r in placedRooms)
        {
            pos.Add(r.position);
        }

        return pos;
    }

    #endregion
}
