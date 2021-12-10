using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.AI;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    public Tilemap floorTilemap, wallTilemap;

    [SerializeField]
    private List<TileBase> wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull, 
        wallInnerCornerDownLeft, wallInnerCornerDownRight, 
        wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    [SerializeField]
    private TileBase[] floorTiles;

    public GameObject endObject;
    public GameObject navMesh;

    private GameObject instantiatedEndObj;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        UpdateMesh();

        Vector2Int furthest = new Vector2Int(0, 0);
        float dist = float.MinValue;

        foreach (var tile in floorPositions)
        {
            PaintSingleTile(floorTilemap, floorTiles[UnityEngine.Random.Range(0, floorTiles.Length - 1)], tile);

            if (Vector2.Distance(tile, Vector2Int.zero) > dist)
            {
                furthest = tile;
                dist = Vector2.Distance(tile, Vector2Int.zero);
            }
        }

        DestroyImmediate(GameObject.FindGameObjectWithTag("EndObject"));

        instantiatedEndObj = Instantiate(endObject, (Vector2)furthest, Quaternion.identity);
    }

    public void UpdateMesh()
    {

    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallTop);
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallSideRight);
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallSideLeft);
        }
        else if (WallTypesHelper.wallBottom.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallBottom);
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallFull);
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }       
    }

    public TileBase GetRandomTileFromList(List<TileBase> listOfTiles)
    {
        return listOfTiles[UnityEngine.Random.Range(0, listOfTiles.Count)];
    }

    public void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();       
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);

        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallInnerCornerDownLeft);
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallInnerCornerDownRight);
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallDiagonalCornerDownLeft);
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallDiagonalCornerDownRight);
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallDiagonalCornerUpRight);
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallDiagonalCornerUpLeft);
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallFull);
        }
        else if (WallTypesHelper.wallBottomEightDirections.Contains(typeAsInt))
        {
            tile = GetRandomTileFromList(wallBottom);
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
    }
}
