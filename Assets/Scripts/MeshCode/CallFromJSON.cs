using System.IO;
using UnityEngine;

public class CallFromJSON : MonoBehaviour
{
    public enum MeshFile
    {
        TileCraterWF,
        TileStraightWF,
        TileIntersectionWF,
        TileRightWF,
        TileFlatWF,
        TileTowerWF
    }


    [SerializeField] MeshFile selectedFile; 
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Renderer renderer;

    MeshData data;
    public Texture mainTexture;

    public Mesh mesh;
    public Vector3[] vertices;
    public int[] triangles;

    public Vector3[] normals;
    public Vector2[] uv;
    public Vector4[] tangents;
    public Color[] colors;


    public void LoadMeshData()
    {

        string filePath = Path.Combine(Application.persistentDataPath, selectedFile.ToString() + ".json");

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);

            data = JsonUtility.FromJson<MeshData>(jsonString);

            vertices = data.vertices;
            triangles = data.triangles;
            normals = data.normals;
            uv = data.uvs;
            tangents = data.tangents;
            colors = data.colors;
            mainTexture = data.mainTexture;

            //Debug.Log("Mesh data loaded from: " + filePath);
        }
        else
        {
            //Debug.Log("JSON file not found: " + filePath);
        }
    }
}
