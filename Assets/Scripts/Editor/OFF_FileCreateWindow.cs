using UnityEngine;
using UnityEditor;
using Modeling.MeshTools;
using Modeling;

public class OFF_FileCreateWindow : EditorWindow
{
    private string selectedFilePath = "";
    private string meshName = "Special Mesh";
    private string selectedDirectory = "Assets/Meshs";

    [MenuItem("OFF File/From OFF File")]
    public static void ShowWindow()
    {
        // Ouvre la fenêtre
        GetWindow<OFF_FileCreateWindow>("Generate From OFF File");
    }

    private void OnGUI()
    {
        GUILayout.Label("File selection", EditorStyles.boldLabel);

        if (GUILayout.Button("Select a file..."))
        {
            string path = EditorUtility.OpenFilePanel(
                "Select a file",
                "Assets/Import/OFF_Meshes",
                "off"
            );

            if (!string.IsNullOrEmpty(path))
            {
                selectedFilePath = path;
            }
        }
        GUILayout.Label("File selected: " + selectedFilePath, EditorStyles.miniLabel);

        GUILayout.Space(10);

        GUILayout.Label("Mesh parameters", EditorStyles.boldLabel);

        meshName = EditorGUILayout.TextField("Name of the mesh", meshName);

        if (GUILayout.Button("Select a directory..."))
        {
            string path = EditorUtility.OpenFolderPanel(
                "Select a folder",
                "Assets/Meshs",
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

        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            if (selectedFilePath == null)
            {
                Debug.LogError("You have to select a file !");
                return;
            }
            Mesh mesh = OFF_File.ReadFile(selectedFilePath);
            MeshUtils.GenerateMesh(mesh, meshName, selectedDirectory);
        }
    }
}

