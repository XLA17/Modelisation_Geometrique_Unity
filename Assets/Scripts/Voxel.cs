using System.Collections.Generic;
using UnityEngine;

public class SphereVoxel : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Vector3 octreeCenter;
    [SerializeField] private float octreeWidth;
    [SerializeField] private int octreeDepth;
    [SerializeField] private List<Sphere> spheres;
    [SerializeField] private bool intersection;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Octree octree = new(octreeCenter, octreeWidth, octreeDepth, spheres, intersection);

        RecursiveInstantiate(octree.rootNode.nodes);
    }

    void RecursiveInstantiate(List<OctreeNode> nodes)
    {
        Debug.Log("RecursiveInstantiate");
        foreach(OctreeNode node in nodes)
        {
            if (node.depth <= 0)
            {
                GameObject cube = Instantiate(cubePrefab, node.center, Quaternion.identity);
                cube.transform.localScale = Vector3.one * node.scale;
                cube.transform.parent = transform;
            }
            else
            {
                RecursiveInstantiate(node.nodes);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
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
    private Vector3 center;
    private float width;
    private int depth;
    private bool intersection;
    public OctreeNode rootNode;
    public List<Sphere> spheres;

    public Octree(Vector3 center, float width, int depth, List<Sphere> spheres, bool intersection)
    {
        this.center = center;
        this.width = width;
        this.depth = depth;
        this.spheres = spheres;
        this.intersection = intersection;

        rootNode = new(center, width, width, depth, spheres, intersection);
        this.intersection = intersection;
    }
}

public class OctreeNode
{
    private List<Sphere> spheres;
    public Vector3 center;
    private float width;
    private bool intersection;
    public float scale;

    public int depth;
    public List<OctreeNode> nodes;

    public OctreeNode(Vector3 center, float width, float scale, int depth, List<Sphere> spheres, bool intersection)
    {
        this.center = center;
        this.width = width;
        this.scale = scale;
        this.depth = depth;
        this.spheres = spheres;
        this.intersection = intersection;

        nodes = CreateNodes();
    }

    List<Vector3> GetCentersOf8Cubes(Vector3 center, float width)
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

    List<OctreeNode> CreateNodes()
    {
        if (depth <= 0)
        {
            return new();
        }

        List<OctreeNode> nodes = new();

        List<Vector3> cubeCenters = GetCentersOf8Cubes(center, width);

        for (int i = 0; i < 8; i++)
        {
            Vector3 cubeCenter = cubeCenters[i];
            float cubeWidth = width / 2;
            float cubeScale = scale / 2;

            bool isIntersection = true;
            bool isUnion = true;
            for (int j = 0; j < spheres.Count; j++)
            {
                float distanceBetweenCenters = Vector3.Distance(spheres[j].center, cubeCenter);

                if (distanceBetweenCenters + Mathf.Sqrt(3f) * cubeWidth / 2 < spheres[j].radius)
                {
                    isUnion = true;
                } else if (distanceBetweenCenters - Mathf.Sqrt(3f) * cubeWidth / 2 > spheres[j].radius)
                {
                    isIntersection = false;
                    if (intersection) break;
                    isUnion = false;
                } else
                {
                    isUnion = true;
                } 
            }

            if ((intersection && isIntersection) || (!intersection && isUnion))
            {
                OctreeNode node = new(cubeCenter, cubeWidth, cubeScale, depth - 1, spheres, intersection);
                nodes.Add(node);
            }
        }

        return nodes;
    }
} 