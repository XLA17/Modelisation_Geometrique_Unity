using Modeling.MeshTools;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3 minPosition;
    public List<Vector3> allVertices;
    public Dictionary<Vector3, int> vertexAndWeight;
    public Vector3? representativePoint;

    public Cell(Vector3 minPosition)
    {
        this.minPosition = minPosition;
        representativePoint = null;
        allVertices = new();
        vertexAndWeight = new Dictionary<Vector3, int>();
    }

    public void CalculateRepresentativePoint()
    {
        Vector3 vTot = Vector3.zero;
        int weightTot = 0;

        foreach (KeyValuePair<Vector3, int> entry in vertexAndWeight)
        {
            vTot += entry.Key;
            weightTot += entry.Value;
        }

        representativePoint = vTot / weightTot;
    }
}

public class Simplification : MonoBehaviour
{
    [SerializeField] private float epsilon;
    [SerializeField] private bool debugBox;
    [SerializeField] private bool debugRepresentativePoint;

    private MeshFilter mf;
    private float minX, minY, minZ, maxX, maxY, maxZ;
    private float meshXLength, meshYLength, meshZLength;
    private List<Cell> cubePositionsList;

    private int test = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mf = GetComponent<MeshFilter>();

        minX = float.PositiveInfinity;
        minY = float.PositiveInfinity;
        maxX = float.NegativeInfinity;
        maxY = float.NegativeInfinity;

        cubePositionsList = new();

        Execute();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("test");
            Execute();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("new mesh");
            Debug.Log("traingles count : " + mf.sharedMesh.triangles.Length);
            NewMesh();
            Debug.Log("llllll : " + test);
        }
    }

    public void Execute()
    {
        foreach (Vector3 vertex in mf.sharedMesh.vertices)
        {
            if (vertex.x < minX) minX = vertex.x;
            else if (vertex.x > maxX) maxX = vertex.x;
            if (vertex.y < minY) minY = vertex.y;
            else if (vertex.y > maxY) maxY = vertex.y;
            if (vertex.z < minZ) minZ = vertex.z;
            else if (vertex.z > maxZ) maxZ = vertex.z;
        }

        meshXLength = (maxX - minX);
        int cubesXCount = (int)(meshXLength / epsilon);
        float minXBox = (cubesXCount * epsilon - meshXLength) / 2 + transform.position.x;
        meshYLength = (maxY - minY);
        int cubesYCount = (int)(meshYLength / epsilon);
        float minYBox = (cubesYCount * epsilon - meshYLength) / 2 + transform.position.y;
        meshZLength = (maxZ - minZ);
        int cubesZCount = (int)(meshZLength / epsilon);
        float minZBox = (cubesZCount * epsilon - meshZLength) / 2 + transform.position.z;

        List<Vector3> meshVerticesCopy = new(mf.sharedMesh.vertices);


        for (int i = 0; i < cubesXCount + 1; i++)
        {
            for (int j = 0; j < cubesYCount + 1; j++)
            {
                for (int k = 0; k < cubesZCount + 1; k++)
                {
                    float x = minX + minXBox + i * epsilon;
                    float y = minY + minYBox + j * epsilon;
                    float z = minZ + minZBox + k * epsilon;
                    Cell cell = new(new Vector3(x, y, z));

                    for (int vi = 0; vi < meshVerticesCopy.Count; vi++)
                    {
                        Vector3 v = meshVerticesCopy[vi];
                        if (v.x >= x && v.x <= x + epsilon &&
                            v.y >= y && v.y <= y + epsilon &&
                            v.z >= z && v.z <= z + epsilon)
                        {
                            // calculate weight with sharedMesh.triangles
                            cell.allVertices.Add(v);
                            cell.vertexAndWeight.Add(v, 1);
                            //meshVerticesCopy.RemoveAt(vi);
                        }
                    }

                    cubePositionsList.Add(cell);
                }
            }
        }

        foreach (Cell c in cubePositionsList)
        {
            c.CalculateRepresentativePoint();
        }
    }

    private void NewMesh()
    {
        List<Vector3> newVertices = new();
        List<int> newTriangles = new();

        for (int i = 0; i < mf.sharedMesh.triangles.Length; i += 3)
        {
            Vector3 v1 = mf.sharedMesh.vertices[mf.sharedMesh.triangles[i]];
            Vector3 v2 = mf.sharedMesh.vertices[mf.sharedMesh.triangles[i + 1]];
            Vector3 v3 = mf.sharedMesh.vertices[mf.sharedMesh.triangles[i + 2]];

            Cell c1 = GetCell(v1);
            Cell c2 = GetCell(v2);
            Cell c3 = GetCell(v3);

            if (c1 == null || c2 == null || c3 == null) continue;
            test++;
            if (c1 == c2 || c2 == c3 || c3 == c1) continue;

            int v1i = MeshUtils.AddVertex(newVertices, c1.representativePoint.Value);
            int v2i = MeshUtils.AddVertex(newVertices, c2.representativePoint.Value);
            int v3i = MeshUtils.AddVertex(newVertices, c3.representativePoint.Value);

            MeshUtils.AddTriangle(newTriangles, v1i, v2i, v3i);
        }

        mf.sharedMesh = MeshUtils.CreateSpecialMesh(newVertices, newTriangles);
    }

    private Cell GetCell(Vector3 v)
    {
        foreach (Cell c in cubePositionsList)
        {
            if (c.allVertices.Contains(v)) return c;
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (cubePositionsList == null)
        {
            return;
        }

        foreach (Cell c in cubePositionsList)
        {
            Vector3 v = c.minPosition;

            if (debugBox)
            {
                Vector3 center = new Vector3(v.x + transform.position.x + epsilon / 2, v.y + transform.position.y + epsilon / 2, v.z + transform.position.z + epsilon / 2);
                Gizmos.DrawWireCube(center, Vector3.one * epsilon);
            }

            if (debugRepresentativePoint)
            {
                Gizmos.DrawSphere(c.representativePoint.Value + transform.position, 0.05f);
            }
        }
    }
}

//using System.Collections.Generic;
//using UnityEngine;

//public class Simplification : MonoBehaviour
//{
//    [SerializeField] private float epsilon;

//    private MeshFilter mf;
//    private float minX, minY, minZ, maxX, maxY, maxZ;
//    private List<Vector3> cubePositionsList;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        mf = GetComponent<MeshFilter>();

//        minX = float.PositiveInfinity;
//        minY = float.PositiveInfinity;
//        maxX = float.NegativeInfinity;
//        maxY = float.NegativeInfinity;

//        cubePositionsList = new List<Vector3>();

//        InitBox();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.V))
//        {
//            Debug.Log("test");
//            InitBox();
//        }
//    }

//    public void InitBox()
//    {
//        //Debug.Log("test");

//        foreach (Vector3 vertex in mf.sharedMesh.vertices)
//        {
//            if (vertex.x < minX) minX = vertex.x;
//            else if (vertex.x > maxX) maxX = vertex.x;
//            if (vertex.y < minY) minY = vertex.y;
//            else if (vertex.y > maxY) maxY = vertex.y;
//            if (vertex.z < minZ) minZ = vertex.z;
//            else if (vertex.z > maxZ) maxZ = vertex.z;
//        }

//        for (int i = 0; i < (maxX - minX) / epsilon; i++)
//        {
//            for (int j = 0; j < (maxY - minY) / epsilon; j++)
//            {
//                for (int k = 0; k < (maxZ - minZ) / epsilon; k++)
//                {
//                    cubePositionsList.Add(new Vector3(minX + i * epsilon, minY + j * epsilon, minZ + k * epsilon));
//                }
//            }
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        if (cubePositionsList == null)
//        {
//            return;
//        }

//        foreach (Vector3 v in cubePositionsList)
//        {
//            Vector3 A = v;
//            Vector3 B = v + new Vector3(epsilon, 0, 0);
//            Vector3 C = v + new Vector3(epsilon, epsilon, 0);
//            Vector3 D = v + new Vector3(0, epsilon, 0);
//            Vector3 E = v + new Vector3(0, 0, epsilon);
//            Vector3 F = v + new Vector3(epsilon, 0, epsilon);
//            Vector3 G = v + new Vector3(epsilon, epsilon, epsilon);
//            Vector3 H = v + new Vector3(0, epsilon, epsilon);

//            // Dessiner les 12 aretes
//            Debug.DrawLine(A, B, Color.white);
//            Debug.DrawLine(B, C, Color.white);
//            Debug.DrawLine(C, D, Color.white);
//            Debug.DrawLine(D, A, Color.white);
//            Debug.DrawLine(E, F, Color.white);
//            Debug.DrawLine(F, G, Color.white);
//            Debug.DrawLine(G, H, Color.white);
//            Debug.DrawLine(H, E, Color.white);
//            Debug.DrawLine(A, E, Color.white);
//            Debug.DrawLine(B, F, Color.white);
//            Debug.DrawLine(C, G, Color.white);
//            Debug.DrawLine(D, H, Color.white);
//        }
//    }
//}