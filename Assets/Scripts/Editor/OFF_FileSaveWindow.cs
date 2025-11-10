using UnityEngine;
using UnityEditor;
using Modeling.MeshTools;
using Modeling;

public class OFF_FileSaveWindow : EditorWindow
{
    private string meshName = "mesh";
    private bool removeTriangles = false;
    private int trianglesToRemoveCount = 0;

    [MenuItem("OFF File/Save OFF File")]
    public static void ShowWindow()
    {
        // Ouvre la fenêtre
        GetWindow<OFF_FileSaveWindow>("Save OFF File");
    }

    private void OnGUI()
    {
        GUILayout.Label("Save parameters", EditorStyles.boldLabel);

        meshName = EditorGUILayout.TextField("Name of the mesh", meshName);
        removeTriangles = EditorGUILayout.Toggle("Remove triangles", removeTriangles);

        if (removeTriangles)
        {
            EditorGUI.indentLevel++;
            trianglesToRemoveCount = EditorGUILayout.IntField("Number of triangles to remove", trianglesToRemoveCount);
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Save"))
        {

            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Meshs/" + meshName + ".asset");
            OFF_File.WriteFile(meshName, mesh);
        }
    }
}

