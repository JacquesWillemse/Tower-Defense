using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    public GameObject TileCrater;
    public GameObject TileStriaght;
    public GameObject TileIntersection;
    public GameObject TileTower;
    public GameObject TileElbowRight;
    public GameObject TileFlat;

    GameObject tileGenerated;

    TileUnique tileUnique;

    public CollapseTest collapseTest;

    public void GenerateMesh()
    {
        for (int i = 0; i < collapseTest.gridSize; i++)
        {
            tileGenerated = null;
            for (int j = 0; j < collapseTest.gridSize; j++)
            {
                tileGenerated = null;
                Vector3 location = new Vector3(-j * 2f, 0f, -i * 2f);
                TileType selectedTile = collapseTest.grid[i, j].possibleTiles[0];
                tileUnique = selectedTile.tileUnique;


                switch (tileUnique)
                {
                    case TileUnique.terrain:
                        tileGenerated = TileFlat;
                        break;
                    case TileUnique.crater:
                        tileGenerated = TileCrater;
                        break;
                    case TileUnique.roadStraight:
                        tileGenerated = TileStriaght;
                        break;
                    case TileUnique.roadCorner:
                        tileGenerated = TileElbowRight;
                        break;
                    case TileUnique.roadIntersection:
                        tileGenerated = TileIntersection;
                        break;
                    case TileUnique.tower:
                        tileGenerated = TileTower;
                        break;
                    default:

                        break;
                }

                if (tileGenerated != null)
                {
                    Quaternion tileRotation = Rotation(selectedTile.rotation);
                    Instantiate(tileGenerated, transform.TransformPoint(location), transform.rotation * tileRotation);
                }
            }
        }
    }

    public Quaternion Rotation(int rotationValue)
    {
        Quaternion prefabRotation = Quaternion.Euler(-90f, 180f, 90f);
        Quaternion gridRotation = Quaternion.Euler(0f, 90f + rotationValue, 0f);

        return gridRotation * prefabRotation;
    }
}
