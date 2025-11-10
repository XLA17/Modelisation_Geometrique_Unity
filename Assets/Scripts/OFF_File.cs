using Modeling.MeshTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Modeling
{
    public static class OFF_File
    {
        public static Mesh ReadFile(string filename)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();

            using (StreamReader reader = new(filename))
            {
                string off = reader.ReadLine();
                if (off != "OFF")
                {
                    throw new Exception("Not a .off file");
                }

                string[] header = reader.ReadLine().Split(" ");

                for (int i = 0; i < int.Parse(header[0]); i++)
                {
                    String[] values = reader.ReadLine().Replace(".", ",").Split(' ');
                    Vector3 v = new(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                    MeshUtils.AddVertex(vertices, v);
                }

                for (int i = 0; i < int.Parse(header[1]); i++)
                {
                    String[] values = reader.ReadLine().Split(' ');
                    MeshUtils.AddTriangle(triangles, int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                }
            }

            return MeshUtils.CreateSpecialMesh(vertices, triangles);
        }

        public static void WriteFile(string filename, Mesh mesh)
        {
            string folderPath = "Assets/off";
            string assetPath = $"{folderPath}/{filename}.off";

            // Check and create folder if needed
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string parent = "Assets";
                string subFolder = "off";
                AssetDatabase.CreateFolder(parent, subFolder);
            }

            //// Confirmation overlay if already exists
            //if (AssetDatabase.LoadAssetAtPath<Mesh>(assetPath) != null)
            //{
            //    bool confirm = EditorUtility.DisplayDialog(
            //        "Overwrite existing off file?",
            //        $"A file named '{filename}' already exists at:\n{assetPath}\n\nDo you want to replace it?",
            //        "Yes, overwrite",
            //        "Cancel"
            //    );

            //    if (!confirm)
            //    {
            //        Debug.Log("File generation canceled by user.");
            //        return;
            //    }

            //    //AssetDatabase.DeleteAsset(assetPath);
            //}

            //AssetDatabase.CreateAsset(mesh, assetPath);
            //AssetDatabase.SaveAssets();

            using (StreamWriter sw = new StreamWriter(assetPath + ".off"))
            {
                sw.WriteLine("OFF");
                sw.WriteLine(mesh.vertices.Count() + " " + mesh.triangles.Count() + " " + mesh.normals.Count());

                foreach (Vector3 v in mesh.vertices)
                {
                    sw.WriteLine(v.x + " " + v.y + " " + v.z);
                }

                foreach (int t in mesh.triangles)
                {
                    sw.WriteLine(t);
                }

                foreach (Vector3 n in mesh.normals)
                {
                    sw.WriteLine(n.x + " " + n.y + " " + n.z);
                }
            }

            AssetDatabase.Refresh();

            Debug.Log($"File created : {assetPath}");
        }
    }
}
