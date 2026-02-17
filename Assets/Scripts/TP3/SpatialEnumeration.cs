using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Sphere
{
    public Vector3 center;
    public float radius;

    public Sphere(Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }
}


public class SpatialEnumeration : MonoBehaviour
{
    [Header("Cube Information")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField, Range(0, 1)] private float coefWidthCube = 1f;

    [Header("Cube Information")]
    [SerializeField] private List<Sphere> spheres;
    [SerializeField] private int octreeDepth;
    [SerializeField] private bool hollow;
    [SerializeField] private bool intersection;

    private float boundingBoxWidth;
    private Vector3 boundingBoxCenter;
    private bool canReload = true;

    void Start()
    {
        Compute();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R) && canReload)
        {
            Debug.Log("Reload");
            foreach (Transform child in transform) Destroy(child.gameObject);
            StartCoroutine(WaitBeforeComputeAgain());
        }
    }

    IEnumerator WaitBeforeComputeAgain()
    {
        canReload = false;
        yield return new WaitForSeconds(0.5f);
        canReload = true;
        Compute();
    }

    void Compute()
    {
        CalculateBoundingBox();
        Octree octree = new(boundingBoxCenter, boundingBoxWidth, octreeDepth, spheres, hollow, intersection);
        RecursiveInstantiate(octree.rootNode.nodes, coefWidthCube);
    }

    void CalculateBoundingBox()
    {
        Vector3 min = Vector3.one * float.MaxValue;
        Vector3 max = Vector3.one * float.MinValue;

        foreach (var sphere in spheres)
        {
            min = Vector3.Min(min, sphere.center - Vector3.one * sphere.radius);
            max = Vector3.Max(max, sphere.center + Vector3.one * sphere.radius);
        }

        Vector3 size = max - min;
        boundingBoxWidth = Mathf.Max(size.x, size.y, size.z);
        boundingBoxCenter = (max + min) / 2;
    }

    void RecursiveInstantiate(List<OctreeNode> nodes, float coefWidthCube)
    {
        foreach (OctreeNode node in nodes)
        {
            if (node.depth <= 0)
            {
                GameObject cube = Instantiate(cubePrefab, node.center, Quaternion.identity);
                cube.transform.localScale = Vector3.one * node.scale * coefWidthCube;
                cube.transform.parent = transform;
            }
            else
            {
                RecursiveInstantiate(node.nodes, coefWidthCube);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Sphere sphere in spheres)
        {
            Gizmos.DrawWireSphere(sphere.center, sphere.radius);
        }

        Gizmos.DrawWireCube(boundingBoxCenter, boundingBoxWidth * Vector3.one);
    }
}