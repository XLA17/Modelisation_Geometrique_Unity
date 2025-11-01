using UnityEditor;
using UnityEngine;
using Modeling.MeshTools;

public class SphereMesh : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField, Range(3, 128)] private int meridians;
    [SerializeField, Range(1, 64)] private int parallels;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private readonly string meshName = "CustomSphere";

    void OnValidate()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.sharedMaterial = material;

        meshFilter.sharedMesh = MeshUtils.CreateSphereMesh(parallels, meridians);
    }

    [ContextMenu("Generate Mesh")]
    void GenerateMesh()
    {
        MeshUtils.GenerateMesh(meshFilter.sharedMesh, meshName);
    }
}
