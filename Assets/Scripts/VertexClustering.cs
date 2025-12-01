using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class VertexClustering : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridDepth;
    [SerializeField] private float epsilon;


    private List<GameObject> grid;
    private Vector3 boundsSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //bounds = GetComponent<MeshFilter>().mesh.bounds;
        //boundsSize = bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(transform.position, bounds.size);
    }
}
