using UnityEngine;
using UnityEditor;
using Modeling.MeshTools;
using Modeling;

public class OFF_FileCreateWindow : EditorWindow
{
    private string pathName;
    private string meshName = "Special Mesh";

    [MenuItem("OFF File/From OFF File")]
    public static void ShowWindow()
    {
        // Ouvre la fenêtre
        GetWindow<OFF_FileCreateWindow>("Generate From OFF File");
    }

    private void OnGUI()
    {
        GUILayout.Label("Rect parameters", EditorStyles.boldLabel);

        pathName = EditorGUILayout.TextField("Path of the file", pathName);
        meshName = EditorGUILayout.TextField("Name of the mesh", meshName);

        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            if (pathName != null)
            {
                Mesh mesh = OFF_File.ReadFile(pathName);
                MeshUtils.GenerateMesh(mesh, meshName);
            }
        }
    }
}

