using System.Collections.Generic;
using UnityEngine;

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

            bool allIn = true;
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
                        if (!intersection)
                        {
                            OctreeNode node = new(cubeCenter, cubeWidth, cubeScale, -1, spheres, hollow, intersection);
                            nodes.Add(node);
                            break;
                        }
                    }
                    else
                    {
                        if (intersection)
                        {
                            allIn = false;
                            break;
                        }
                    }
                    continue;
                }
                else if (minDist < spheres[j].radius)
                {
                    if (!intersection)
                    {
                        OctreeNode node = new(cubeCenter, cubeWidth, cubeScale, depth - 1, spheres, hollow, intersection);
                        nodes.Add(node);
                        break;
                    }
                    continue;
                }

                // outside of the sphere
                if (intersection)
                {
                    allIn = false;
                    break;
                }
            }

            if (intersection && allIn)
            {
                OctreeNode node = new(cubeCenter, cubeWidth, cubeScale, depth - 1, spheres, hollow, intersection);
                nodes.Add(node);
            }
        }

        return nodes;
    }
}