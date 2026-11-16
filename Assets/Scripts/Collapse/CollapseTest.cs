using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollapseTest : MonoBehaviour
{

    //Script to generate Mesh After the wave function is done
    public WaveFunctionCollapse waveFunctionCollapse;
    //Creates a 2D array of grid cells containg, coordinates, bool if its been collapsed or not, and a list of all possible tiles it can currently be
    public GridCell[,] grid;
    //The current variation of possible tiles
    public TileType terrainTile;
    public TileType verticalRoad;
    public TileType CornerRoad;
    public TileType IntersectionRoad;
    public TileType Tower;
    public TileType Crater;

    //keeps info of initial tiles that have the same ID value for each side
    public TileType OldVerticalRoad;
    public TileType OldCornerRoad;
    public TileType OldCrater;
    public TileType OldTower;

    //this list gets filled with all the possible tiles a cell can be as well as their rotated varients where relevent 
    public List<TileType> allPossibleTiles = new List<TileType>();

    //determines the size of the grid, it will be a square of whatever value is selected
    public int gridSize;

    //Tile ID's of prefabs
    int ROAD_EDGE = 2;
    int TERRAIN_EDGE = 1;

    //rotation values of tiles to change the tile IDs to match the change the rotation made
    int rotationNorth = 0;
    int rotationSouth = 180;
    int rotationEast = 90;
    int rotationWest = 270;

    //collapse posibilities variables
    int lowestNumPos; //Keeps track of the current lowest possibilities of tiles a grid cell can potentially be
    int lowestPosX;   //x Coordinate of lowest possibility tile
    int lowestPosY;   //y Coordinate of lowest possibility tile

    bool contradictionFound = false;
    bool generationComplete = false;

    bool towerPlaced = false;
    int towerRotation;

    public Queue<GridCell> collapseQueue = new Queue<GridCell>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //creates a tile template for Tiles that have the same IDs for every side
        //gives its initial rotation, what parent group it belongs to, the unique tile name, and then assigns the Tile Id's to every side to use in future
        terrainTile = PopulateTile(rotationNorth, TileParentGroup.Terrain, TileUnique.terrain, TERRAIN_EDGE, TERRAIN_EDGE, TERRAIN_EDGE, TERRAIN_EDGE);
        IntersectionRoad = PopulateTile(rotationNorth, TileParentGroup.Road, TileUnique.roadIntersection, ROAD_EDGE, TERRAIN_EDGE, ROAD_EDGE, ROAD_EDGE);
        Tower = PopulateTile(rotationNorth, TileParentGroup.Road, TileUnique.tower, ROAD_EDGE, ROAD_EDGE, ROAD_EDGE, ROAD_EDGE);

        for (int i = 0; i < 4; i++)
        {

            switch (i)
            {
                //creates a tile template for Tiles that have different ID's when rotated
                //gives its initial rotation, what parent group it belongs to, the unique tile name, and then assigns the Tile Id's to every side
                case 0:
                    verticalRoad = PopulateTile(rotationNorth, TileParentGroup.Road, TileUnique.roadStraight, TERRAIN_EDGE, ROAD_EDGE, TERRAIN_EDGE, ROAD_EDGE);
                    OldVerticalRoad = verticalRoad;

                    CornerRoad = PopulateTile(rotationNorth, TileParentGroup.Road, TileUnique.roadCorner, TERRAIN_EDGE, ROAD_EDGE, ROAD_EDGE, TERRAIN_EDGE);
                    OldCornerRoad = CornerRoad;

                    Tower = PopulateTile(rotationNorth, TileParentGroup.Tower, TileUnique.tower, ROAD_EDGE, TERRAIN_EDGE, ROAD_EDGE, ROAD_EDGE);
                    OldTower = Tower;

                    Crater = PopulateTile(rotationNorth, TileParentGroup.Road, TileUnique.crater, TERRAIN_EDGE, ROAD_EDGE, TERRAIN_EDGE, TERRAIN_EDGE);
                    OldCrater = Crater;
                    break;
                case 1:
                    verticalRoad = ReCalculateRotation(OldVerticalRoad, rotationEast);
                    CornerRoad = ReCalculateRotation(OldCornerRoad, rotationEast);
                    Tower = ReCalculateRotation(OldTower, rotationEast);
                    Crater = ReCalculateRotation(OldCrater, rotationEast);
                    break;
                case 2:
                    CornerRoad = ReCalculateRotation(OldCornerRoad, rotationWest);
                    Tower = ReCalculateRotation(OldTower, rotationWest);
                    Crater = ReCalculateRotation(OldCrater, rotationWest);
                    break;
                case 3:
                    CornerRoad = ReCalculateRotation(OldCornerRoad, rotationSouth);
                    Tower = ReCalculateRotation(OldTower, rotationSouth);
                    Crater = ReCalculateRotation(OldCrater, rotationSouth);
                    break;
                default:

                    break;
            }
            if (i < 2)
            {
                //tiles that share 2 ID's where the same ID is opposite each other doesnt have to be added twice as they can be rotated
                //for the opposite axis
                allPossibleTiles.Add(verticalRoad);
            }
            allPossibleTiles.Add(CornerRoad);
            allPossibleTiles.Add(Crater);
            allPossibleTiles.Add(Tower);
        }
        //adds the tile to the list of possible tiles
        allPossibleTiles.Add(terrainTile);
        allPossibleTiles.Add(IntersectionRoad);

        //instantiates a new GridCell class, squared from gridsize
        grid = new GridCell[gridSize, gridSize];
        //Instantiates every cell with a gridcell and provides value for its location in the cell and all possible varients it can be
        // each cell has its own list of possible tiles
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = new GridCell();
                grid[x, y].gridY = y;
                grid[x, y].gridX = x;
                grid[x, y].possibleTiles = new List<TileType>(allPossibleTiles);
            }
        }


        //PlacePOIs();
        CalculateEndPointLocation(5, 5, Direction.north, CraterID.CraterOne);
        CalculateEndPointLocation(5, 5, Direction.north, CraterID.CraterTwo);
        CalculateEndPointLocation(5, 5, Direction.north, CraterID.CraterThree);





        while (!generationComplete && !contradictionFound)
        {
            //Searches through each of the grid entries possibilities to find the list with the least amount of entries,
            //saves a new lowest number, as well as the coordinates for the tile in the array
            if (!FindLowestCollapseValue())
            {
                generationComplete = true;
                break;
            }
            else
            {
                if (grid[lowestPosX, lowestPosY].possibleTiles.Count == 0)
                {
                    contradictionFound = true;
                    break;
                }
                if (!contradictionFound)
                {
                    int randomTileCollapse = UnityEngine.Random.Range(0, grid[lowestPosX, lowestPosY].possibleTiles.Count);
                    TileType chosenTile = grid[lowestPosX, lowestPosY].possibleTiles[randomTileCollapse];
                    grid[lowestPosX, lowestPosY].possibleTiles.Clear();
                    grid[lowestPosX, lowestPosY].possibleTiles.Add(chosenTile);
                    grid[lowestPosX, lowestPosY].collapsed = true;
                    collapseQueue.Enqueue(grid[lowestPosX, lowestPosY]);
                }
            }

            while (collapseQueue.Count > 0)
            {
                GridCell gridCell = collapseQueue.Dequeue();
                //North Tile check
                if (gridCell.gridY > 0)
                {
                    bool removedTile = false;
                    bool compatible = false;
                    for (int a = grid[gridCell.gridX, gridCell.gridY - 1].possibleTiles.Count - 1; a >= 0; a--)
                    {
                        compatible = false;
                        //Check every possible tile in the current cell
                        for (int b = 0; b < gridCell.possibleTiles.Count; b++)
                        {
                            //If one matches the northern tile candidate, set compatible = true
                            if (CheckCompatibility(grid[gridCell.gridX, gridCell.gridY].possibleTiles[b], grid[gridCell.gridX, gridCell.gridY - 1].possibleTiles[a], Direction.north))
                            {
                                compatible = true;
                            }
                        }
                        //remove the northen tile if there are no possible matches
                        if (!compatible)
                        {
                            grid[gridCell.gridX, gridCell.gridY - 1].possibleTiles.Remove(grid[gridCell.gridX, gridCell.gridY - 1].possibleTiles[a]);
                            removedTile = true;
                        }
                    }
                    if (grid[gridCell.gridX, gridCell.gridY - 1].possibleTiles.Count == 0)
                    {
                        Debug.Log("North tile has 0 possibilities");
                        contradictionFound = true;
                        break;
                    }
                    //Adds the northern tile to the queue to be checked if north is a valid tile
                    if (removedTile)
                    {
                        collapseQueue.Enqueue(grid[gridCell.gridX, gridCell.gridY - 1]);
                    }
                }

                //South Tile Check
                if (gridCell.gridY < gridSize - 1)
                {
                    bool removedTile = false;
                    bool compatible = false;
                    for (int a = grid[gridCell.gridX, gridCell.gridY + 1].possibleTiles.Count - 1; a >= 0; a--)
                    {
                        compatible = false;
                        //Check every possible tile in the current cell
                        for (int b = 0; b < gridCell.possibleTiles.Count; b++)
                        {
                            //If one matches the southern tile candidate, set compatible = true
                            if (CheckCompatibility(grid[gridCell.gridX, gridCell.gridY].possibleTiles[b], grid[gridCell.gridX, gridCell.gridY + 1].possibleTiles[a], Direction.south))
                            {
                                compatible = true;
                            }
                        }

                        if (!compatible)
                        {
                            grid[gridCell.gridX, gridCell.gridY + 1].possibleTiles.Remove(grid[gridCell.gridX, gridCell.gridY + 1].possibleTiles[a]);
                            removedTile = true;
                        }
                    }
                    if (grid[gridCell.gridX, gridCell.gridY + 1].possibleTiles.Count == 0)
                    {
                        Debug.Log("South tile has 0 possibilities");
                        contradictionFound = true;
                        break;
                    }
                    //Adds the southers tile to the queue to be checked if south is a valid tile
                    if (removedTile)
                    {
                        collapseQueue.Enqueue(grid[gridCell.gridX, gridCell.gridY + 1]);
                    }
                }

                //west tile check
                if (gridCell.gridX > 0)
                {
                    bool removedTile = false;
                    bool compatible = false;
                    for (int a = grid[gridCell.gridX - 1, gridCell.gridY].possibleTiles.Count - 1; a >= 0; a--)
                    {
                        compatible = false;
                        //Check every possible tile in the current cell
                        for (int b = 0; b < gridCell.possibleTiles.Count; b++)
                        {
                            if (CheckCompatibility(grid[gridCell.gridX, gridCell.gridY].possibleTiles[b], grid[gridCell.gridX - 1, gridCell.gridY].possibleTiles[a], Direction.west))
                            {
                                compatible = true;
                            }
                        }
                        if (!compatible)
                        {
                            grid[gridCell.gridX - 1, gridCell.gridY].possibleTiles.Remove(grid[gridCell.gridX - 1, gridCell.gridY].possibleTiles[a]);
                            removedTile = true;
                        }
                    }
                    if (grid[gridCell.gridX - 1, gridCell.gridY].possibleTiles.Count == 0)
                    {
                        Debug.Log("West tile has 0 possibilities");
                        contradictionFound = true;
                        break;
                    }
                    //Adds the western tile to the queue to be checked if west is a valid tile
                    if (removedTile)
                    {
                        collapseQueue.Enqueue(grid[gridCell.gridX - 1, gridCell.gridY]);
                    }
                }

                //east tile check
                if (gridCell.gridX < gridSize - 1)
                {
                    bool removedTile = false;
                    bool compatible = false;
                    for (int a = grid[gridCell.gridX + 1, gridCell.gridY].possibleTiles.Count - 1; a >= 0; a--)
                    {
                        compatible = false;
                        for (int b = 0; b < gridCell.possibleTiles.Count; b++)
                        {
                            if (CheckCompatibility(grid[gridCell.gridX, gridCell.gridY].possibleTiles[b], grid[gridCell.gridX + 1, gridCell.gridY].possibleTiles[a], Direction.east))
                            {
                                compatible = true;
                            }
                        }
                        if (!compatible)
                        {
                            grid[gridCell.gridX + 1, gridCell.gridY].possibleTiles.Remove(grid[gridCell.gridX + 1, gridCell.gridY].possibleTiles[a]);
                            removedTile = true;
                        }
                    }
                    if (grid[gridCell.gridX + 1, gridCell.gridY].possibleTiles.Count == 0)
                    {
                        Debug.Log("East tile has 0 possibilities");
                        contradictionFound = true;
                        break;
                    }
                    //Adds the eastern tile to the queue to be checked if east is a valid tile
                    if (removedTile)
                    {
                        collapseQueue.Enqueue(grid[gridCell.gridX + 1, gridCell.gridY]);
                    }
                }
            }
        }
        if (generationComplete && !contradictionFound)
        {
            // pass on the gridcell of all collapsed tiles to be generated 
            waveFunctionCollapse.collapseTest = this;
            waveFunctionCollapse.GenerateMesh();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Hardcoded values to populate a type of tile, this will have to be hand fed for each created tile mesh to ensure it lines up
    public TileType PopulateTile(int rotation, TileParentGroup tileParentGroupTerrain, TileUnique uniqueTileTerrain, int north, int east, int south, int west)
    {
        TileType tile = new TileType();
        tile.rotation = rotation;
        tile.tileUnique = uniqueTileTerrain;
        tile.tileGroup = tileParentGroupTerrain;
        tile.north = north;
        tile.east = east;
        tile.south = south;
        tile.west = west;

        return (tile);
    }

    //Rotates the Tile IDs to match the new rotated possition of the tile
    public TileType ReCalculateRotation(TileType tileType, int RotationValue)
    {
        int oldNorth = tileType.north;
        int oldEast = tileType.east;
        int oldSouth = tileType.south;
        int oldWest = tileType.west;

        int newNorth = oldNorth;
        int newEast = oldEast;
        int newSouth = oldSouth;
        int newWest = oldWest;

        int rotation = RotationValue;

        TileType newRotations = new TileType();

        switch (rotation)
        {
            case 0:
                newNorth = oldNorth;
                newEast = oldEast;
                newSouth = oldSouth;
                newWest = oldWest;
                break;
            case 90:
                newNorth = oldWest;
                newEast = oldNorth;
                newSouth = oldEast;
                newWest = oldSouth;
                break;
            case 180:
                newNorth = oldSouth;
                newEast = oldWest;
                newSouth = oldNorth;
                newWest = oldEast;
                break;
            case 270:
                newNorth = oldEast;
                newEast = oldSouth;
                newSouth = oldWest;
                newWest = oldNorth;
                break;

            default:

                break;
        }
        newRotations.north = newNorth;
        newRotations.east = newEast;
        newRotations.south = newSouth;
        newRotations.west = newWest;
        newRotations.tileGroup = tileType.tileGroup;
        newRotations.tileUnique = tileType.tileUnique;
        newRotations.rotation = rotation;

        return (newRotations);
    }

    //checks if a tile shares the same id of a neigbouring tile based on which directed the 2nd tile is from the 1st
    public bool CheckCompatibility(TileType tileOne, TileType tileTwo, Direction direction)
    {
        switch (direction)
        {
            case Direction.north:
                return tileOne.north == tileTwo.south;
            case Direction.south:
                return tileOne.south == tileTwo.north;
            case Direction.east:
                return tileOne.east == tileTwo.west;
            case Direction.west:
                return tileOne.west == tileTwo.east;
            default:
                return false;
        }
    }

    public bool FindLowestCollapseValue()
    {
        lowestNumPos = int.MaxValue;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!grid[x, y].collapsed && grid[x, y].possibleTiles.Count < lowestNumPos)
                {
                    lowestNumPos = grid[x, y].possibleTiles.Count;
                    lowestPosX = x;
                    lowestPosY = y;
                }
            }
        }
        if (lowestNumPos == int.MaxValue)
        {
            return false;
        }
        else
        {
            return true;
        }
    }



    public void FindPOILocation(int row, int column)
    {
        //x,y coordinates for POI 
        int[] POECoordinates = new int[2];

        //gets the size of a 3rd of the grid
        int gridThrird = gridSize / 3;

        if (row == 2 && column == 2)
        {
            int TowerRotationVal = UnityEngine.Random.Range(1, 5);

            POECoordinates[0] = UnityEngine.Random.Range(gridThrird, gridSize - gridThrird);
            POECoordinates[1] = UnityEngine.Random.Range(gridThrird, gridSize - gridThrird);


            switch (TowerRotationVal)
            {
                case 1: //0
                    Tower = ReCalculateRotation(OldTower, rotationNorth);
                    break;
                case 2: // 90
                    Tower = ReCalculateRotation(OldTower, rotationEast);
                    break;
                case 3: // 180
                    Tower = ReCalculateRotation(OldTower, rotationSouth);
                    break;
                case 4: // 270
                    Tower = ReCalculateRotation(OldTower, rotationWest);
                    break;
                default:
                    break;
            }
            grid[POECoordinates[0], POECoordinates[1]].possibleTiles.Clear();
            grid[POECoordinates[0], POECoordinates[1]].possibleTiles.Add(Tower);
            grid[POECoordinates[0], POECoordinates[1]].collapsed = true;
            towerPlaced = true;
            towerRotation = TowerRotationVal;
        }
        else
        {
            switch (row)
            {
                case 1:
                    POECoordinates[0] = UnityEngine.Random.Range(1, gridThrird);
                    break;
                case 2:
                    POECoordinates[0] = UnityEngine.Random.Range(gridThrird + 1, gridSize - gridThrird);
                    break;
                case 3:
                    POECoordinates[0] = UnityEngine.Random.Range(gridSize - gridThrird + 1, gridSize);
                    break;
                default:
                    break;
            }
            switch (column)
            {
                case 1:
                    POECoordinates[1] = UnityEngine.Random.Range(1, gridThrird);
                    break;
                case 2:
                    POECoordinates[1] = UnityEngine.Random.Range(gridThrird + 1, gridSize - gridThrird);
                    break;
                case 3:
                    POECoordinates[1] = UnityEngine.Random.Range(gridSize - gridThrird + 1, gridSize);
                    break;
                default:
                    break;
            }
            grid[POECoordinates[0], POECoordinates[1]].possibleTiles.Clear();
            grid[POECoordinates[0], POECoordinates[1]].possibleTiles.Add(Crater);
            grid[POECoordinates[0], POECoordinates[1]].collapsed = true;
        }
    }

    public void PlacePOIs()
    {
        int regionX = 2; // column
        int regionY = 2; // row

        if (!towerPlaced)
        {
            FindPOILocation(regionY, regionX);
        }
        switch (towerRotation)
        {
            case 1: //0
                //crater One
                FindPOILocation(3, 3);

                //crater Two
                FindPOILocation(1, 3);

                //crater Three
                FindPOILocation(2, 1);
                break;
            case 2: // 90
                //crater One
                FindPOILocation(3, 3);

                //crater Two
                FindPOILocation(3, 1);

                //Crater Three
                FindPOILocation(1, 2);
                break;
            case 3: // 180
                //crater One
                FindPOILocation(3, 1);

                //crater Two
                FindPOILocation(1, 3);

                //Crater Three
                FindPOILocation(2, 1);
                break;
            case 4: //270
                //crater One
                FindPOILocation(1, 1);

                //crater Two
                FindPOILocation(1, 3);

                //Crater Three
                FindPOILocation(3, 2);
                break;
            default:
                break;
        }
    }

    public void CalculateEndPointLocation(int x, int y, Direction towerDirection, CraterID craterId)
    {
        Vector2Int towerConnectionLocation = new Vector2Int(0, 0);
        switch (towerDirection)
        {
            case Direction.north: //0
                switch (craterId)
                {
                    case CraterID.CraterOne:
                        towerConnectionLocation = new Vector2Int(x - 1, y);
                        break;
                    case CraterID.CraterTwo:
                        towerConnectionLocation = new Vector2Int(x + 1, y);
                        break;
                    case CraterID.CraterThree:
                        towerConnectionLocation = new Vector2Int(x, y + 1);
                        break;
                    default:
                        break;
                }
                break;
            case Direction.east: // 90
                switch (craterId)
                {
                    case CraterID.CraterOne:
                        towerConnectionLocation = new Vector2Int(x, y + 1);
                        break;
                    case CraterID.CraterTwo:
                        towerConnectionLocation = new Vector2Int(x, y - 1);
                        break;
                    case CraterID.CraterThree:
                        towerConnectionLocation = new Vector2Int(x - 1, y);
                        break;
                    default:
                        break;
                }
                break;
            case Direction.south: // 180
                switch (craterId)
                {
                    case CraterID.CraterOne:
                        towerConnectionLocation = new Vector2Int(x + 1, y);
                        break;
                    case CraterID.CraterTwo:
                        towerConnectionLocation = new Vector2Int(x - 1, y);
                        break;
                    case CraterID.CraterThree:
                        towerConnectionLocation = new Vector2Int(x, y - 1);
                        break;
                    default:
                        break;
                }
                break;
            case Direction.west: //270
                switch (craterId)
                {
                    case CraterID.CraterOne:
                        towerConnectionLocation = new Vector2Int(x, y + 1);
                        break;
                    case CraterID.CraterTwo:
                        towerConnectionLocation = new Vector2Int(x, y - 1);
                        break;
                    case CraterID.CraterThree:
                        towerConnectionLocation = new Vector2Int(x + 1, y);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        Debug.Log(craterId + " connection location is " + towerConnectionLocation + "if the tower location is " + x + " " + y);
    }
}