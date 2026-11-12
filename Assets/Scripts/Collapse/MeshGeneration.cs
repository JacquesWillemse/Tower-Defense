using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeshGeneration : MonoBehaviour
{
    [SerializeField] int xValue = 0;
    [SerializeField] int yValue = 0;
    [SerializeField] int zValue = 0;

    [SerializeField] int numVertices = 3;// num of vertices squared 
    int index = 0;
    Vector3[] vertices;

    bool firstPlane = true;
    bool LeftCorner = true;

    public GameObject testBall;//object for testing
    public GameObject testBall2;//object for testing

    List<int> triangles = new List<int>();


    //test var
    int numC = 0; //value for point c in square
    int numStart = 0;
    int numFace;  //the amount of faces that would be produces in a row based on the amount of vertices



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateVertices();
        GenerateTriangles();
        GenerateVertices();
        GenerateVertices();
    }

    void GenerateVertices()
    {
        vertices = new Vector3[numVertices * numVertices * 3];
        if (firstPlane)
        {
            for (int i = 0; i < numVertices; i++)
            {
                for (int j = 0; j < numVertices; j++)
                {
                    vertices[index] = new Vector3(xValue, yValue, 0);
                    Instantiate(testBall, transform.TransformPoint(vertices[index]), Quaternion.identity);//Adds object at vertices for testing
                    index++;
                    xValue++;
                }
                xValue = 0;
                yValue++;
            }
            firstPlane = false;
            yValue = 0;
        }
        else
        {
            if (LeftCorner)
            {
                xValue = 0;
                LeftCorner = false;
            }
            else
            {
                xValue = numVertices - 1;
            }

            zValue = 1;
            for (int i = 0; i < numVertices; i++)
            {
                for (int j = 0; j < numVertices - 1; j++)
                {
                    vertices[index] = new Vector3(xValue, yValue, zValue);
                    Instantiate(testBall, transform.TransformPoint(vertices[index]), Quaternion.identity);//Adds object at vertices for testing
                    Debug.Log(vertices[index]);
                    index++;
                    zValue++;
                }
                zValue = 1;
                yValue++;
            }
            yValue = 0;
        }
    }

    void GenerateTriangles()
    {
        numFace = numVertices - 1;
        for (int j = 0; j < numFace; j++)
        {
            for (int i = 0; i < numFace; i++)
            {
                numFace = numVertices - 1;
                numC = numStart + numVertices; //moves amount of plane vertices length from point A in index to get point C 

                int a = numStart;
                int b = numStart + 1;
                int c = numC;
                int d = numC + 1;

                triangles.Add(c);
                triangles.Add(b);
                triangles.Add(a);

                triangles.Add(c);
                triangles.Add(d);
                triangles.Add(b);

                numStart++;
            }
            numStart++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
