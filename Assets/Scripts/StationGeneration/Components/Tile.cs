using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType type;
    public Sprite tileSprite;
    [Tooltip("Marks the tile as a entrance target.")]
    public bool isEntranceTile;
}
