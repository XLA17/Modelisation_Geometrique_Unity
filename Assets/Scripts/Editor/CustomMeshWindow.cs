using UnityEngine;
using UnityEditor;
using Modeling.MeshTools;

public class CustomMeshWindow : EditorWindow
{
    private enum MeshType { Rect, Plane, Cylinder, Sphere, Cone }
    private MeshType selectedMesh = MeshType.Sphere;

    // rect parameters
    private string rectName = "Custom Rect";
    private float rectWidth = 2f;
    private float rectHeight = 1f;

    // plane parameters
    private string planeName = "Custom Plane";
    private float planeWidth = 2f;
    private float planeHeight = 1f;
    private int planeRows = 10;
    private int planeColumns = 10;

    // cylinder parameters
    private string cylinderName = "Custom Cylinder";
    private float cylinderHeight = 2f;
    private float cylinderRadius = 1f;
    private int cylinderMeridians = 16;
    private bool cylinderIsTruncated = false;
    private int cylinderMeridiansTrunc = 3;

    // sphere parameters
    private string sphereName = "Custom Sphere";
    private int sphereParallels = 8;
    private int sphereMeridians = 16;
    private bool sphereIsTruncated = false;
    private int sphereMeridiansTrunc = 3;
    private int sphereParallelsTrunc = 3;

    // cone parameters
    private string coneName = "Custom Cone";
    private float coneHeight = 2f;
    private float coneRadius = 1f;
    private int coneMeridians = 16;
    private bool coneIsTruncated = false;
    private float coneTruncHeight = 1f;
    private int coneMeridiansTrunc = 3;

[MenuItem("Generate/Mesh")]
    public static void ShowWindow()
    {
        // Ouvre la fenêtre
        GetWindow<CustomMeshWindow>("Generate Custom Mesh");
    }

    private void OnGUI()
    {
        selectedMesh = (MeshType)EditorGUILayout.EnumPopup("Mesh Type", selectedMesh);

        switch (selectedMesh)
        {
            case MeshType.Rect:
                DrawRectParameters();
                break;
            case MeshType.Plane:
                DrawPlaneParameters();
                break;
            case MeshType.Cylinder:
                DrawCylinderParameters();
                break;
            case MeshType.Sphere:
                DrawSphereParameters();
                break;
            case MeshType.Cone:
                DrawConeParameters();
                break;
        }
    }

    private void DrawRectParameters()
    {
        GUILayout.Label("Rect parameters", EditorStyles.boldLabel);

        rectName = EditorGUILayout.TextField("Name of the mesh", rectName);
        rectWidth = EditorGUILayout.FloatField("Width", rectWidth);
        rectHeight = EditorGUILayout.FloatField("Height", rectHeight);

        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            Mesh mesh = MeshUtils.CreateRectMesh(rectWidth, rectHeight);
            MeshUtils.GenerateMesh(mesh, rectName);
        }
    }

    private void DrawPlaneParameters()
    {
        GUILayout.Label("Plane parameters", EditorStyles.boldLabel);

        planeName = EditorGUILayout.TextField("Name of the mesh", planeName);
        planeWidth = EditorGUILayout.FloatField("Width", planeWidth);
        planeHeight = EditorGUILayout.FloatField("Height", planeHeight);
        planeRows = EditorGUILayout.IntField("Number of rows", planeRows);
        planeColumns = EditorGUILayout.IntField("Number of columns", planeColumns);

        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            Mesh mesh = MeshUtils.CreatePlaneMesh(planeWidth, planeHeight, planeRows, planeColumns);
            MeshUtils.GenerateMesh(mesh, planeName);
        }
    }

    private void DrawCylinderParameters()
    {
        GUILayout.Label("Cylinder parameters", EditorStyles.boldLabel);

        cylinderName = EditorGUILayout.TextField("Name of the mesh", cylinderName);
        cylinderHeight = EditorGUILayout.FloatField("Height", cylinderHeight);
        cylinderRadius = EditorGUILayout.FloatField("Radius", cylinderRadius);
        cylinderMeridians = EditorGUILayout.IntSlider("Number of meridians", cylinderMeridians, 3, 128);

        cylinderIsTruncated = EditorGUILayout.Toggle("Truncated", cylinderIsTruncated);

        if (cylinderIsTruncated)
        {
            EditorGUI.indentLevel++;
            cylinderMeridiansTrunc = EditorGUILayout.IntSlider("Number of meridians truncated", cylinderMeridiansTrunc, 0, cylinderMeridians - 1);
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            Mesh mesh;
            if (cylinderIsTruncated)
            {
                mesh = MeshUtils.CreateCylinderTruncMesh(cylinderMeridians, cylinderHeight, cylinderRadius, cylinderMeridiansTrunc);
            }
            else
            {
                mesh = MeshUtils.CreateCylinderMesh(cylinderMeridians, cylinderHeight, cylinderRadius);
            }
            MeshUtils.GenerateMesh(mesh, cylinderName);
        }
    }

    private void DrawSphereParameters()
    {
        GUILayout.Label("Sphere parameters", EditorStyles.boldLabel);

        sphereName = EditorGUILayout.TextField("Name of the mesh", sphereName);
        sphereParallels = EditorGUILayout.IntSlider("Number of parallels", sphereParallels, 1, 64);
        sphereMeridians = EditorGUILayout.IntSlider("Number of meridians", sphereMeridians, 3, 128);

        sphereIsTruncated = EditorGUILayout.Toggle("Truncated", sphereIsTruncated);

        if (sphereIsTruncated)
        {
            EditorGUI.indentLevel++;
            sphereParallelsTrunc = EditorGUILayout.IntSlider("Number of parallels truncated", sphereParallelsTrunc, 0, sphereParallels - 1);
            sphereMeridiansTrunc = EditorGUILayout.IntSlider("Number of meridians truncated", sphereMeridiansTrunc, 0, sphereMeridians - 1);
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            Mesh mesh;
            if (sphereIsTruncated)
            {
                mesh = MeshUtils.CreateSphereTruncMesh(sphereParallels, sphereMeridians, sphereParallelsTrunc, sphereMeridiansTrunc);
            }
            else
            {
                mesh = MeshUtils.CreateSphereMesh(sphereParallels, sphereMeridians);
            }
            MeshUtils.GenerateMesh(mesh, sphereName);
        }
    }
 
    private void DrawConeParameters()
    {
        GUILayout.Label("Cylinder parameters", EditorStyles.boldLabel);

        coneName = EditorGUILayout.TextField("Name of the mesh", coneName);
        coneHeight = EditorGUILayout.FloatField("Height", coneHeight);
        coneRadius = EditorGUILayout.FloatField("Radius", coneRadius);
        coneMeridians = EditorGUILayout.IntSlider("Number of meridians", coneMeridians, 3, 128);

        coneIsTruncated = EditorGUILayout.Toggle("Truncated", coneIsTruncated);

        if (coneIsTruncated)
        {
            EditorGUI.indentLevel++;
            coneTruncHeight = EditorGUILayout.Slider("Height", coneTruncHeight, 0.1f, coneHeight);
            coneMeridiansTrunc = EditorGUILayout.IntSlider("Number of meridians truncated", coneMeridiansTrunc, 0, coneMeridians - 1);
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            Mesh mesh;
            if (coneIsTruncated)
            {
                mesh = MeshUtils.CreateConeTruncMesh(coneMeridians, coneHeight, coneRadius, coneTruncHeight, coneMeridiansTrunc);
            } else
            {
                mesh = MeshUtils.CreateConeMesh(coneMeridians, coneHeight, coneRadius);
            }
            MeshUtils.GenerateMesh(mesh, coneName);
        }
    }
}

