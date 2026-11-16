using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]

public class RoadPath
{
    public List<Vector2Int> roadGridCoordinates;
    public Vector2Int parentCraterLocation;
    public Vector2Int towerConnectionLocation;
    public Direction towerHookupDirection;
    public CraterID craterID;
}
