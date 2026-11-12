using System.IO;
using UnityEngine;

public class MeshReverseEngineer : MonoBehaviour
{

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Renderer renderer;
    [SerializeField] string MeshName;

    Texture mainTexture;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    Vector3[] normals;
    Vector2[] uv;
    Vector4[] tangents;
    Color[] colors;

    MeshData data = new MeshData();


    void Start()
    {
        mainTexture = renderer.sharedMaterial.mainTexture;

        mesh = meshFilter.sharedMesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        normals = mesh.normals;
        uv = mesh.uv;
        tangents = mesh.tangents;
        colors = mesh.colors;

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

        data.vertices = vertices;
        data.triangles = triangles;
        data.normals = normals;
        data.tangents = tangents;
        data.colors = colors;
        data.uvs = uv;
        data.mainTexture = mainTexture;

        Debug.Log(vertices);
        Debug.Log(triangles);
        Debug.Log(normals);
        Debug.Log(uv);
        Debug.Log(tangents);
        Debug.Log(colors);
        Debug.Log(mainTexture);

        string jsonString = JsonUtility.ToJson(data, true);
        string filePath = Path.Combine(Application.persistentDataPath, MeshName + ".json");
        File.WriteAllText(filePath, jsonString);
        Debug.Log("JSON saved to: " + filePath);
    }
}