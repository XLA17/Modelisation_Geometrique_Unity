using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private float radius;
    [SerializeField] private int depth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Recursif(depth, transform.position, radius*2, radius*2);
    }

    // Update is called once per frame
    void Update()
    {
    }

    //List<Vector3> GetCube(Vector3 center, float width)
    //{
    //    List<Vector3> cube = new List<Vector3>();
    //    cube.Add(new Vector3(center.x - width / 2, center.y - width / 2, center.z - width / 2));
    //    cube.Add(new Vector3(center.x - width / 2, center.y - width / 2, center.z + width / 2));
    //    cube.Add(new Vector3(center.x - width / 2, center.y + width / 2, center.z - width / 2));
    //    cube.Add(new Vector3(center.x - width / 2, center.y + width / 2, center.z + width / 2));
    //    cube.Add(new Vector3(center.x + width / 2, center.y - width / 2, center.z - width / 2));
    //    cube.Add(new Vector3(center.x + width / 2, center.y + width / 2, center.z - width / 2));
    //    cube.Add(new Vector3(center.x + width / 2, center.y - width / 2, center.z + width / 2));
    //    cube.Add(new Vector3(center.x + width / 2, center.y + width / 2, center.z + width / 2));

    //    return cube;
    //}

    List<Vector3> GetCentersOf8Cubes(Vector3 center, float width)
    {
        List<Vector3> cube = new List<Vector3>();
        cube.Add(new Vector3(center.x - width / 4, center.y - width / 4, center.z - width / 4));
        cube.Add(new Vector3(center.x - width / 4, center.y - width / 4, center.z + width / 4));
        cube.Add(new Vector3(center.x - width / 4, center.y + width / 4, center.z - width / 4));
        cube.Add(new Vector3(center.x - width / 4, center.y + width / 4, center.z + width / 4));
        cube.Add(new Vector3(center.x + width / 4, center.y - width / 4, center.z - width / 4));
        cube.Add(new Vector3(center.x + width / 4, center.y + width / 4, center.z - width / 4));
        cube.Add(new Vector3(center.x + width / 4, center.y - width / 4, center.z + width / 4));
        cube.Add(new Vector3(center.x + width / 4, center.y + width / 4, center.z + width / 4));

        return cube;
    }

    void Recursif(int maxDepth, Vector3 center, float width, float scale)
    {
        if (maxDepth == 0)
        {
            GameObject cube = Instantiate(cubePrefab, center, Quaternion.identity);
            cube.transform.localScale = Vector3.one * scale;
            cube.transform.parent = transform;
            return;
        }

        List<Vector3> cubeCenters = GetCentersOf8Cubes(center, width);

        for (int i = 0; i < 8; i++)
        {
            Vector3 cubeCenter = cubeCenters[i];
            float distanceBetweenCenters = Vector3.Distance(transform.position, cubeCenter);
            float cubeWidth = width / 2;
            float cubeScale = scale / 2;

            if (distanceBetweenCenters + Mathf.Sqrt(3f) * cubeWidth / 2 < radius)
            {
                GameObject cube = Instantiate(cubePrefab, cubeCenter, Quaternion.identity);
                cube.transform.localScale = Vector3.one * cubeScale;
                cube.transform.parent = transform;
            }
            else if (distanceBetweenCenters - Mathf.Sqrt(3f) * cubeWidth / 2 > radius)
            {
            }
            else
            {
                Debug.Log(distanceBetweenCenters);
                Debug.Log(Mathf.Sqrt(3) * cubeWidth);
                Recursif(maxDepth - 1, cubeCenter, cubeWidth, cubeScale);
            }
        }
    }
}
