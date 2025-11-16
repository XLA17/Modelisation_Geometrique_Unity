using UnityEngine;
using UnityEditor;
using Modeling.MeshTools;
using Modeling;

public class OFF_FileSaveWindow : EditorWindow
{
    private string selectedFilePath = "";
    private string offFileName = "New off file";
    private bool removeTriangles = false;
    private int trianglesToRemoveCount = 0;
    private string selectedDirectory = "Assets/OffFiles";

    [MenuItem("OFF File/Save OFF File")]
    public static void ShowWindow()
    {
        // Ouvre la fenêtre
        GetWindow<OFF_FileSaveWindow>("Save OFF File");
    }

    private void OnGUI()
    {
        GUILayout.Label("Mesh selection", EditorStyles.boldLabel);

        if (GUILayout.Button("Select a mesh..."))
        {
            string path = EditorUtility.OpenFilePanel(
                "Select a mesh",
                "Assets/Meshs",
                "asset"
            );

            if (!string.IsNullOrEmpty(path))
            {
                if (!path.StartsWith(Application.dataPath))
                {
                    Debug.LogError("The selected folder must be in the folder Assets !");
                    return;
                }

                // conversion in relative path
                selectedFilePath = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        GUILayout.Label("Mesh selected: " + selectedFilePath, EditorStyles.miniLabel);

        GUILayout.Space(10);

        GUILayout.Label("File export parameters", EditorStyles.boldLabel);

        offFileName = EditorGUILayout.TextField("Name of the file", offFileName);
        if (GUILayout.Button("Select a directory..."))
        {
            string path = EditorUtility.OpenFolderPanel(
                "Select a folder",
                "Assets/OffFiles",
                ""
            );

            if (!string.IsNullOrEmpty(path))
            {
                if (!path.StartsWith(Application.dataPath))
                {
                    Debug.LogError("The selected folder must be in the folder Assets !");
                    return;
                }

                // conversion in relative path
                selectedDirectory = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        GUILayout.Label("Directory selected: " + selectedDirectory, EditorStyles.miniLabel);

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

            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(selectedFilePath);
            Mesh meshTrunc =MeshUtils.RemoveTriangles(mesh, trianglesToRemoveCount);
            OFF_File.WriteFile(offFileName, meshTrunc, selectedDirectory);
        }
    }
}

