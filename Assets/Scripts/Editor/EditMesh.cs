using UnityEngine;
using UnityEditor;
using Modeling.MeshTools;
using Modeling;

public class EditMesh : EditorWindow
{
    private string pathName;
    private int triangleToRemoveCount;

    [MenuItem("Generate/Edit mesh")]
    public static void ShowWindow()
    {
        // Ouvre la fenêtre
        GetWindow<EditMesh>("Remove a certain number of triangles of a mesh");
    }

    private void OnGUI()
    {
        GUILayout.Label("Edit mesh parameters", EditorStyles.boldLabel);

        pathName = EditorGUILayout.TextField("Path of the file", pathName);
        triangleToRemoveCount = EditorGUILayout.IntField("Number of triangles to remove", triangleToRemoveCount);

        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            if (pathName != null)
            {
                Mesh mesh = OFF_File.ReadFile(pathName);
                MeshUtils.RemoveTriangles(mesh, triangleToRemoveCount);
            }
        }
    }
}

