using System.Collections.Generic;
using UnityEngine;

public class Simplification : MonoBehaviour
{
    [SerializeField] private float epsilon;

    private MeshFilter mf;
    private float minX, minY, minZ, maxX, maxY, maxZ;
    private List<Vector3> cubePositionsList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mf = GetComponent<MeshFilter>();

        minX = float.PositiveInfinity;
        minY = float.PositiveInfinity;
        maxX = float.NegativeInfinity;
        maxY = float.NegativeInfinity;

        cubePositionsList = new List<Vector3>();

        InitBox();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("test");
            InitBox();
        }
    }

    public void InitBox()
    {
        //Debug.Log("test");

        foreach (Vector3 vertex in mf.sharedMesh.vertices)
        {
            if (vertex.x < minX) minX = vertex.x;
            else if (vertex.x > maxX) maxX = vertex.x;
            if (vertex.y < minY) minY = vertex.y;
            else if (vertex.y > maxY) maxY = vertex.y;
            if (vertex.z < minZ) minZ = vertex.z;
            else if (vertex.z > maxZ) maxZ = vertex.z;
        }

        for (int i = 0; i < (maxX - minX) / epsilon; i++)
        {
            for (int j = 0; j < (maxY - minY) / epsilon; j++)
            {
                for (int k = 0; k < (maxZ - minZ) / epsilon; k++)
                {
                    cubePositionsList.Add(new Vector3(minX + i * epsilon, minY + j * epsilon, minZ + k * epsilon));
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (cubePositionsList == null)
        {
            return;
        }

        foreach (Vector3 v in cubePositionsList)
        {
            Vector3 A = v;
            Vector3 B = v + new Vector3(epsilon, 0, 0);
            Vector3 C = v + new Vector3(epsilon, epsilon, 0);
            Vector3 D = v + new Vector3(0, epsilon, 0);
            Vector3 E = v + new Vector3(0, 0, epsilon);
            Vector3 F = v + new Vector3(epsilon, 0, epsilon);
            Vector3 G = v + new Vector3(epsilon, epsilon, epsilon);
            Vector3 H = v + new Vector3(0, epsilon, epsilon);

            // Dessiner les 12 arêtes
            Debug.DrawLine(A, B, Color.white);
            Debug.DrawLine(B, C, Color.white);
            Debug.DrawLine(C, D, Color.white);
            Debug.DrawLine(D, A, Color.white);
            Debug.DrawLine(E, F, Color.white);
            Debug.DrawLine(F, G, Color.white);
            Debug.DrawLine(G, H, Color.white);
            Debug.DrawLine(H, E, Color.white);
            Debug.DrawLine(A, E, Color.white);
            Debug.DrawLine(B, F, Color.white);
            Debug.DrawLine(C, G, Color.white);
            Debug.DrawLine(D, H, Color.white);
        }
    }
}
