using UnityEngine;

[System.Serializable]
public class TileType
{
    public TileParentGroup tileGroup;
    public TileUnique tileUnique;
    public int rotation;

    public int north;
    public int east;
    public int south;
    public int west;
}
