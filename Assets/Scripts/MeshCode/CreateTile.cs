using UnityEngine;

public class CreateTile : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Renderer renderer;

    Texture mainTexture;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    Vector3[] normals;
    Vector2[] uv;
    Vector4[] tangents;
    Color[] colors;


    CallFromJSON getMeshInformation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        getMeshInformation = GetComponent<CallFromJSON>();
        getMeshInformation.LoadMeshData();

        mainTexture = getMeshInformation.mainTexture;
        mesh = getMeshInformation.mesh;
        vertices = getMeshInformation.vertices;
        triangles = getMeshInformation.triangles;
        normals = getMeshInformation.normals;
        uv = getMeshInformation.uv;
        colors = getMeshInformation.colors;
        tangents = getMeshInformation.tangents;
        mainTexture = getMeshInformation.mainTexture;

        RenderMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RenderMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.tangents = tangents;
        mesh.colors = colors;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<Renderer>().material.mainTexture = mainTexture;
        mesh.RecalculateBounds();

        //Debug.Log(vertices);
        //Debug.Log(triangles);
        //Debug.Log(normals);
        //Debug.Log(uv);
        //Debug.Log(tangents);
        //Debug.Log(colors);

        //string jsonString = JsonUtility.ToJson(data, true);
        //string filePath = Path.Combine(Application.persistentDataPath, "MeshData.json");
        //File.WriteAllText(filePath, jsonString);
        //Debug.Log("JSON saved to: " + filePath);
    }
}
