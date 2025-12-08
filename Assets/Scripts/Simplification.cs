using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3 minPosition;
    public Dictionary<Vector3, int> vertexAndWeight;
    public Vector3? representativePoint;

    public Cell(Vector3 minPosition)
    {
        this.minPosition = minPosition;
        representativePoint = null;
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
    private List<Cell> cubePositionsList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mf = GetComponent<MeshFilter>();

        minX = float.PositiveInfinity;
        minY = float.PositiveInfinity;
        maxX = float.NegativeInfinity;
        maxY = float.NegativeInfinity;

        cubePositionsList = new();

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

        float meshXLength = (maxX - minX);
        int cubesXCount = (int)(meshXLength / epsilon);
        float minXBox = (cubesXCount * epsilon - meshXLength) / 2;
        float meshYLength = (maxY - minY);
        int cubesYCount = (int)(meshYLength / epsilon);
        float minYBox = (cubesYCount * epsilon - meshYLength) / 2;
        float meshZLength = (maxZ - minZ);
        int cubesZCount = (int)(meshZLength / epsilon);
        float minZBox = (cubesZCount * epsilon - meshZLength) / 2;

        List<Vector3> meshVerticesCopy = new(mf.sharedMesh.vertices);


        for (int i = 0; i < cubesXCount+1; i++)
        {
            for (int j = 0; j < cubesYCount+1; j++)
            {
                for (int k = 0; k < cubesZCount+1; k++)
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
                            cell.vertexAndWeight.Add(v, 1);
                            meshVerticesCopy.RemoveAt(vi);
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

    //public void CalculateNewVertices()
    //{
    //    if (cubePositionsList == null)
    //    {
    //        return;
    //    }

    //    foreach (Vector3 v in cubePositionsList)
    //    {

    //    }
    //}

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
                Vector3 center = new Vector3(v.x + epsilon / 2, v.y + epsilon / 2, v.z + epsilon / 2);
                Gizmos.DrawWireCube(center, Vector3.one * epsilon);
            }

            if (debugRepresentativePoint)
            {
                Gizmos.DrawSphere(c.representativePoint.Value, 0.05f);
            }
        }
    }
}
