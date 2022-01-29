using UnityEngine;

namespace WaterRipples
{
    public static class MeshBuilder
    {
        public static MeshFilter CreatePlane(this GameObject _target, Vector2Int _gridSize)
        {
            MeshFilter meshFilter = _target.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = _target.AddComponent<MeshFilter>();
            }

            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = new Vector3[(_gridSize.x + 1) * (_gridSize.y + 1)];
            Vector3[] normals = new Vector3[vertices.Length];
            int[] triangles = new int[_gridSize.x * _gridSize.y * 6];
            Vector2[] uvs = new Vector2[vertices.Length];

            int index = 0;
            float zStep = 1f / _gridSize.y;
            float xStep = 1f / _gridSize.x;
            for (int j = 0; j <= _gridSize.y; j++)
            {
                float z = j * zStep;
                for (int i = 0; i <= _gridSize.x; i++)
                {
                    float x = i * xStep;
                    vertices[index] = new Vector3(x, 0, z);
                    normals[index] = new Vector3(0, 1, 0);
                    uvs[index] = new Vector2(x, z);
                    index++;
                }
            }

            int offset = 0;
            index = 0;
            for (int i = 0; i < _gridSize.y; i++)
            {
                for (int j = 0; j < _gridSize.x; j++)
                {
                    triangles[index++] = offset;
                    triangles[index++] = offset + _gridSize.x + 1;
                    triangles[index++] = offset + 1;
                    triangles[index++] = offset + 1;
                    triangles[index++] = offset + _gridSize.x + 1;
                    triangles[index++] = offset + _gridSize.x + 2;
                    offset++;
                }
                offset++;
            }

            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, submesh: 0);
            mesh.SetUVs(0, uvs);
            return meshFilter;
        }
    }
}