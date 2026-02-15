using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialEnumeration : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private int octreeDepth;
    [SerializeField] private List<Sphere> spheres;
    [SerializeField] private bool hollow;
    [SerializeField] private bool intersection;
    [SerializeField, Range(0, 1)] private float coefWidthCube = 1f;

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

public class Octree
{
    public OctreeNode rootNode;

    public Octree(Vector3 center, float width, int depth, List<Sphere> spheres, bool hollow, bool intersection)
    {
        rootNode = new(center, width, width, depth, spheres, hollow, intersection);
    }
}

public class OctreeNode
{
    private readonly List<Sphere> spheres;
    public Vector3 center;
    private readonly float width;
    private readonly bool hollow;
    private readonly bool intersection;
    public float scale;

    public int depth;
    public List<OctreeNode> nodes;

    public OctreeNode(Vector3 center, float width, float scale, int depth, List<Sphere> spheres, bool holow, bool intersection)
    {
        this.center = center;
        this.width = width;
        this.scale = scale;
        this.depth = depth;
        this.spheres = spheres;
        this.hollow = holow;
        this.intersection = intersection;

        nodes = CreateNodes();
    }

    /// <summary>
    /// Get the 8 positions corresponding to the center of the 8 cubes contains in the parent cube.
    /// </summary>
    /// <param name="center">The center of the parent cube.</param>
    /// <param name="width">The width of the parent cube.</param>
    /// <returns>The 8 centers.</returns>
    List<Vector3> GetCentersOf8Cubes()
    {
        List<Vector3> cube = new()
        {
            new Vector3(center.x - width / 4, center.y - width / 4, center.z - width / 4),
            new Vector3(center.x - width / 4, center.y - width / 4, center.z + width / 4),
            new Vector3(center.x - width / 4, center.y + width / 4, center.z - width / 4),
            new Vector3(center.x - width / 4, center.y + width / 4, center.z + width / 4),
            new Vector3(center.x + width / 4, center.y - width / 4, center.z - width / 4),
            new Vector3(center.x + width / 4, center.y + width / 4, center.z - width / 4),
            new Vector3(center.x + width / 4, center.y - width / 4, center.z + width / 4),
            new Vector3(center.x + width / 4, center.y + width / 4, center.z + width / 4)
        };

        return cube;
    }

    /// <summary>
    /// Creates a list of sub-nodes (between 0 and 8) if a sphere pass through this node.
    /// </summary>
    /// <returns>The list of sub-nodes.</returns>
    List<OctreeNode> CreateNodes()
    {
        if (depth <= 0)
        {
            return new();
        }

        List<OctreeNode> nodes = new();

        List<Vector3> cubeCenters = GetCentersOf8Cubes();

        for (int i = 0; i < 8; i++)
        {
            Vector3 cubeCenter = cubeCenters[i];
            float cubeWidth = width / 2;
            float cubeScale = scale / 2;

            for (int j = 0; j < spheres.Count; j++)
            {
                // Closest point on the cube to the sphere
                Vector3 closestPoint = new(
                    Mathf.Clamp(spheres[j].center.x, cubeCenter.x - cubeWidth / 2, cubeCenter.x + cubeWidth / 2),
                    Mathf.Clamp(spheres[j].center.y, cubeCenter.y - cubeWidth / 2, cubeCenter.y + cubeWidth / 2),
                    Mathf.Clamp(spheres[j].center.z, cubeCenter.z - cubeWidth / 2, cubeCenter.z + cubeWidth / 2)
                );

                float minDist = Vector3.Distance(spheres[j].center, closestPoint);
                float maxDist = Vector3.Distance(spheres[j].center, cubeCenter) + (cubeWidth * Mathf.Sqrt(3f) / 2f);

                if (maxDist < spheres[j].radius)
                {
                    // inside the sphere
                    if (!hollow)
                    {
                        OctreeNode node = new(cubeCenter, cubeWidth, cubeScale, -1, spheres, hollow, intersection);
                        nodes.Add(node);
                        break;
                    }
                    continue;
                }
                else if (minDist < spheres[j].radius)
                {
                    // on the sphere
                    OctreeNode node = new(cubeCenter, cubeWidth, cubeScale, depth - 1, spheres, hollow, intersection);
                    nodes.Add(node);
                    break;
                }

                // outside of the sphere
            }
        }

        return nodes;
    }
}