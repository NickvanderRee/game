using UnityEngine;

[ExecuteInEditMode]
public class SphereDrawer : MonoBehaviour
{
    public float sphereRadius = 1.0f;
    public Color sphereColor = Color.white;
    public Material sphereMaterial; // Assign the material in the Inspector

    private Mesh sphereMesh;

void OnDrawGizmos()
{
    if (sphereMesh != null && sphereMaterial != null)
    {
        // Set up the transformation matrix
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw the mesh with the assigned material and color
        Graphics.DrawMesh(sphereMesh, transform.position, Quaternion.identity, sphereMaterial, 0);

        // Restore the original transformation matrix
        Gizmos.matrix = oldMatrix;
    }
}

void CreateSphereMesh()
{
    // Create a new sphere mesh
    sphereMesh = new Mesh();

    // Create vertices and triangles
    int resolution = 20;
    Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
    Vector3[] normals = new Vector3[vertices.Length];
    int[] triangles = new int[6 * resolution * resolution];

    float radius = 1.0f;
    int vertexIndex = 0;
    int triangleIndex = 0;

    // Generate vertices and normals
    for (int i = 0; i <= resolution; i++)
    {
        for (int j = 0; j <= resolution; j++)
        {
            float theta = (float)i / resolution * Mathf.PI;
            float phi = (float)j / resolution * Mathf.PI * 2;

            float x = Mathf.Sin(theta) * Mathf.Cos(phi);
            float y = Mathf.Cos(theta);
            float z = Mathf.Sin(theta) * Mathf.Sin(phi);

            vertices[vertexIndex] = new Vector3(x, y, z) * radius;
            normals[vertexIndex] = vertices[vertexIndex].normalized;

            vertexIndex++;
        }
    }

    // Generate triangles
    for (int i = 0; i < resolution; i++)
    {
        for (int j = 0; j < resolution; j++)
        {
            int index = i * (resolution + 1) + j;

            triangles[triangleIndex++] = index;
            triangles[triangleIndex++] = index + resolution + 1;
            triangles[triangleIndex++] = index + resolution + 2;

            triangles[triangleIndex++] = index;
            triangles[triangleIndex++] = index + resolution + 2;
            triangles[triangleIndex++] = index + 1;
        }
    }

    // Assign vertices, normals, and triangles to the mesh
    sphereMesh.vertices = vertices;
    sphereMesh.normals = normals;
    sphereMesh.triangles = triangles;
}
}