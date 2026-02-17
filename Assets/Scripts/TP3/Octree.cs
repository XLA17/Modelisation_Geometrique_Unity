using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    public OctreeNode rootNode;

    public Octree(Vector3 center, float width, int depth, List<Sphere> spheres, bool hollow, bool intersection)
    {
        rootNode = new(center, width, width, depth, spheres, hollow, intersection);
    }
}