using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class TP1 : MonoBehaviour
{
    [SerializeField] private Material material;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();


        Mesh mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshRenderer>().sharedMaterial = material;

        mesh.Clear();

        //createRect(new Vector3(0, 0, 0), new Vector3(1, 1, 1), new Vector3(-1, 0, 1));
        //createCylindre(new Vector3(0, 0, 0), 20, 10, 10);
        createSphere(new Vector3(0, 0, 0), 10, 5, 10);


        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

    }

    int createVertex(Vector3 v)
    {
        if (!vertices.Contains(v))
        {
            vertices.Add(v);
        }
        return vertices.IndexOf(v);
    }

    void createTriangle(int vertex1Index, int vertex2Index, int vertex3Index)
    {
        triangles.Add(vertex1Index);
        triangles.Add(vertex2Index);
        triangles.Add(vertex3Index);
    }

    //void createRect(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3) // pas très bien, enfin pas ce que je veux, marche pas quoi
    //{
    //    Vector3 vd = vertex3 - vertex2;
    //    Vector3 vertex4 = vertex1 + vd;
    //    AddTriangle(vertex1, vertex2, vertex3);
    //    AddTriangle(vertex2, vertex4, vertex3);
    //}

    //void createPlan(Vector3 v1, Vector3 v2, Vector3 v3, int rowCount, int columnCount)
    //{
    //    for (int i = 0; i < rowCount; i++)
    //    {
    //        for (int j = 0; j < columnCount; j++)
    //        {
    //            Vector3 new_o = new Vector3(origin.x + j*width, origin.y + i*height, origin.x);
    //            createRect(new_o, width, height);
    //        }
    //    }
    //}

    void createCylindre(Vector3 center, int height, int rayon, int meridienCount)
    {
        float halfHeight = height / 2;
        int centerUp = createVertex(new Vector3(center.x, halfHeight, center.z));
        int centerDown = createVertex(new Vector3(center.x, -halfHeight, center.z));

        float teta = 0;
        int vUp = createVertex(new Vector3(rayon * Mathf.Cos(teta), halfHeight, rayon * Mathf.Sin(teta)));
        int vDown = createVertex(new Vector3(rayon * Mathf.Cos(teta), -halfHeight, rayon * Mathf.Sin(teta)));


        for (int i = 0; i < meridienCount-1; i++)
        {
            teta += Mathf.PI * 2 / (meridienCount-1);
            int vUp2 = createVertex(new Vector3(rayon * Mathf.Cos(teta), halfHeight, rayon * Mathf.Sin(teta)));
            int vDown2 = createVertex(new Vector3(rayon * Mathf.Cos(teta), -halfHeight, rayon * Mathf.Sin(teta)));

            createTriangle(vDown, vDown2, centerDown);
            createTriangle(centerUp, vUp2, vUp);
            createTriangle(vUp, vDown2, vDown);
            createTriangle(vUp, vUp2, vDown2);

            vUp = vUp2;
            vDown = vDown2;
        }
    }

    void createSphere(Vector3 center, int rayon, int parraleleCount, int meridienCount)
    {
        float height = -rayon;

        for (int i = 0; i < parraleleCount+1; i++)
        {
            float teta = 0;
            int vUp = createVertex(new Vector3(rayon * Mathf.Cos(teta), height, rayon * Mathf.Sin(teta)));
            int vDown = createVertex(new Vector3(rayon * Mathf.Cos(teta), -height, rayon * Mathf.Sin(teta)));

            height += rayon / (float)(parraleleCount + 1);
            float r = Mathf.Sqrt((rayon * rayon) - (height * height));

            for (int j = 0; j < meridienCount - 1; j++)
            {
                teta += Mathf.PI * 2 / (meridienCount - 1);
                int vUp2 = createVertex(new Vector3(r * Mathf.Cos(teta), height, r * Mathf.Sin(teta)));
                int vDown2 = createVertex(new Vector3(r * Mathf.Cos(teta), height, r * Mathf.Sin(teta)));

                Debug.Log("te: " + vUp + " " + vDown + " " + vUp2 + " " + vDown2);

                createTriangle(vUp, vDown2, vDown);
                createTriangle(vUp, vUp2, vDown2);

                vUp = vUp2;
                vDown = vDown2;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
