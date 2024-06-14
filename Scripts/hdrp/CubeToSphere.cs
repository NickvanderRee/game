using UnityEngine;

public class CubeToSphere : MonoBehaviour
{
    public float sphereRadius = 1f; // Radius of the sphere
    public int latitudeSegments = 16; // Number of segments along the latitude
    public int longitudeSegments = 16; // Number of segments along the longitude

    MeshFilter meshFilter;
    Mesh mesh;

    void Start()
    {
        // Get the MeshFilter component
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter component not found!");
            return;
        }

        // Create a new mesh
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Generate sphere vertices and triangles
        GenerateSphere();
    }

    void GenerateSphere()
    {
        // Initialize lists to store vertices and triangles
        var vertices = new System.Collections.Generic.List<Vector3>();
        var triangles = new System.Collections.Generic.List<int>();

        // Generate sphere vertices
        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = lat * Mathf.PI / latitudeSegments;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = lon * 2 * Mathf.PI / longitudeSegments;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                Vector3 vertex = new Vector3(cosPhi * sinTheta, sinPhi * sinTheta, cosTheta);
                vertices.Add(vertex * sphereRadius);
            }
        }

        // Generate sphere triangles
        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int current = lat * (longitudeSegments + 1) + lon;
                int next = current + longitudeSegments + 1;

                triangles.Add(current);
                triangles.Add(next + 1);
                triangles.Add(current + 1);

                triangles.Add(current);
                triangles.Add(next);
                triangles.Add(next + 1);
            }
        }

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        // Recalculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}