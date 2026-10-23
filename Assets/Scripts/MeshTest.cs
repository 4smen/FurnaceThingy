using UnityEngine;

public class MeshTest : MonoBehaviour
{
    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        int p = 0;
        while (p < vertices.Length)
        {
            vertices[p] += new Vector3(0, Random.Range(-0.3F, 0.3F), 0);
            p++;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
