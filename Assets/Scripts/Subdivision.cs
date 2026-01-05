using Modeling.MeshTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Subdivision : MonoBehaviour
{
    [SerializeField] private GameObject[] mesh2D;
    [SerializeField][Range(1, 10)] private int P;

    private List<Vector3> newMesh2D;
    private MeshFilter meshFilter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = Loop(meshFilter.mesh);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        //for(int i = 0; i < mesh2D.Length; i++)
        //{
        //    var p1 = mesh2D[i];
        //    var p2 = i == mesh2D.Length-1 ? mesh2D[0] : mesh2D[i + 1];
        //    Gizmos.DrawSphere(p1.transform.position, 0.1f);
        //    Gizmos.DrawLine(p1.transform.position, p2.transform.position);
        //}

        //Gizmos.color = Color.blue;
        //for (int i = 0; i < newMesh2D.Count; i++)
        //{
        //    var p1 = newMesh2D[i];
        //    var p2 = i == newMesh2D.Count - 1 ? newMesh2D[0] : newMesh2D[i + 1];
        //    Gizmos.DrawSphere(p1, 0.1f);
        //    Gizmos.DrawLine(p1, p2);
        //}
    }

    List<Vector3> Chaikin(List<Vector3> mesh)
    {
        List<Vector3> positions = new();

        for (int i = 0; i < mesh.Count; i++)
        {
            Vector3 p1 = mesh[i];
            Vector3 p2 = i == mesh.Count - 1 ? mesh[0] : mesh[i + 1];

            float dist = Vector3.Distance(p1, p2);
            Vector3 dir = p2 - p1;

            positions.Add(p1 + dir * 0.25f);
            positions.Add(p1 + dir * 0.75f);
        }

        return positions;
    }

    private void OnValidate()
    {
        List<Vector3> positionsMesh2D = new();

        foreach (GameObject o in mesh2D)
        {
            positionsMesh2D.Add(o.transform.position);
        }

        newMesh2D = Chaikin(positionsMesh2D);

        for (int i = 0; i < P - 1; i++)
        {
            newMesh2D = Chaikin(newMesh2D);
        }
    }

    Mesh Loop(Mesh mesh)
    {
        List<Vector3> newVertices = new();
        List<int> newTriangles = new();

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 v1 = mesh.vertices[mesh.triangles[i]];
            Vector3 v2 = mesh.vertices[mesh.triangles[i+1]];
            Vector3 v3 = mesh.vertices[mesh.triangles[i+2]];

            Vector3 v4 = v1 + (v2 - v1) / 2;
            Vector3 v5 = v2 + (v3 - v2) / 2;
            Vector3 v6 = v3 + (v1 - v3) / 2;

            int indexV1 = MeshUtils.AddVertex(newVertices, v1);
            int indexV2 = MeshUtils.AddVertex(newVertices, v2);
            int indexV3 = MeshUtils.AddVertex(newVertices, v3);
            int indexV4 = MeshUtils.AddVertex(newVertices, v4);
            int indexV5 = MeshUtils.AddVertex(newVertices, v5);
            int indexV6 = MeshUtils.AddVertex(newVertices, v6);

            MeshUtils.AddTriangle(newTriangles, indexV1, indexV4, indexV6);
            MeshUtils.AddTriangle(newTriangles, indexV4, indexV2, indexV5);
            MeshUtils.AddTriangle(newTriangles, indexV6, indexV5, indexV3);
            MeshUtils.AddTriangle(newTriangles, indexV4, indexV5, indexV6);
            //if (newTriangles[i] == -1) Debug.Log("1-----------1");
            //if (newTriangles[i + 1] == -1) Debug.Log("2-----------1");
            //if (newTriangles[i + 2] == -1) Debug.Log("3-----------1");
        }

        List<Vector3> newVertices2 = new(newVertices);
        for (int i = 0; i < newVertices.Count; i++)
        {
            List<int> neighborsIndex = GetNeighborVertices(newTriangles, i);

            //float a = Mathf.Pow(3 + 2 * Mathf.Cos(2*Mathf.PI / neighborsIndex.Count), 2) / 32 - 0.25f;
            //float b = (1 - a) / neighborsIndex.Count;

            Vector3 sum = newVertices[i];

            foreach (var neighborIndex in neighborsIndex)
            {
                Debug.Log(neighborIndex);
                sum += newVertices[neighborIndex];
            }

            Vector3 average = sum / (neighborsIndex.Count + 1);

            newVertices2[i] = average;
        }

        return MeshUtils.CreateSpecialMesh(newVertices2, newTriangles);
    }

    List<int> GetNeighborVertices(List<int> triangles, int vertexIndex)
    {
        static int AddVertex(List<int> neighborsIndex, int neighborIndex)
        {
            if (!neighborsIndex.Contains(neighborIndex))
            {
                neighborsIndex.Add(neighborIndex);
            }
            return neighborsIndex.IndexOf(neighborIndex);
        }

        List<int> neighborsIndex = new();

        for (int i = 0; i < triangles.Count; i+=3)
        {
            if (triangles[i] == vertexIndex || triangles[i+1] == vertexIndex || triangles[i+2] == vertexIndex)
            {
                
                AddVertex(neighborsIndex, triangles[i]);
                AddVertex(neighborsIndex, triangles[i+1]);
                AddVertex(neighborsIndex, triangles[i+2]);
            }
        }

        neighborsIndex.Remove(vertexIndex);

        return neighborsIndex;
    }
}
