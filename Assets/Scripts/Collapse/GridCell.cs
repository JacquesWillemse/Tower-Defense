using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridCell
{
    public List<TileType> possibleTiles = new List<TileType>();
    public bool collapsed;
    public int gridX;
    public int gridY;

}
