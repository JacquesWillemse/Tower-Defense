using UnityEngine;
using System;

[Serializable]
public class MeshData
{
    public Texture mainTexture;
    public Texture Texture;
    public Mesh mesh;
    public Vector3[] vertices;
    public int[] triangles;

    public Vector3[] normals;
    public Vector2[] uvs;
    public Vector4[] tangents;
    public Color[] colors;
}
