using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace Modeling.MeshTools
{
    public class MeshUtils
    {
        public static int AddVertex(List<Vector3> vertices, Vector3 v)
        {
            if (!Contains(vertices, v)) vertices.Add(v);

            return vertices.IndexOf(v);
        }

        public static bool Contains(List<Vector3> vertices, Vector3 v)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (Vector3.Distance(vertices[i], v) < 0.001f) return true;
            }

            return false;
        }

        public static void AddTriangle(List<int> triangles, int vertex1Index, int vertex2Index, int vertex3Index)
        {
            triangles.Add(vertex1Index);
            triangles.Add(vertex2Index);
            triangles.Add(vertex3Index);
        }

        public static List<Vector3> CalculateNormals(List<Vector3> vertices, List<int> triangles)
        {
            List<Vector3> normals = new();
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 normalSum = Vector3.zero;
                int faceCount = 0;

                for (int j = 0; j < triangles.Count; j += 3)
                {
                    if (triangles[j] == i || triangles[j+1] == i || triangles[j+2] == i)
                    {
                        Vector3 v1 = vertices[triangles[j]];
                        Vector3 v2 = vertices[triangles[j+1]];
                        Vector3 v3 = vertices[triangles[j+2]];
                        Vector3 vd1 = v2 - v1;
                        Vector3 vd2 = v3 - v1;
                        Vector3 normalTriangle = Vector3.Cross(vd1, vd2);
                        normalTriangle.Normalize();
                        normalSum += normalTriangle;
                        faceCount++;
                    }
                }
                Vector3 normalVertex = normalSum / faceCount;
                normalVertex.Normalize();
                normals.Add(normalVertex);
            }

            return normals;
        }
        public static Mesh CreateRectMesh(float width, float height)
        {
            Mesh mesh = new();

            List<Vector3> vertices = new();
            List<int> triangles = new();

            float halfWidth = width / 2;
            float halfHeight = height / 2;
            int v1 = AddVertex(vertices, new Vector3(-halfWidth, halfHeight, 0));
            int v2 = AddVertex(vertices, new Vector3(halfWidth, halfHeight, 0));
            int v3 = AddVertex(vertices, new Vector3(halfWidth, -halfHeight, 0));
            int v4 = AddVertex(vertices, new Vector3(-halfWidth, -halfHeight, 0));

            AddTriangle(triangles, v1, v2, v3);
            AddTriangle(triangles, v1, v3, v4);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreatePlaneMesh(float width, float height, int rows, int columns)
        {
            Mesh mesh = new();

            List<Vector3> vertices = new();
            List<int> triangles = new();

            float rectWidth = width / columns;
            float rectHeight = height / rows;

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    float leftX = i * rectWidth - width / 2;
                    float rightX = (i + 1) * rectWidth - width / 2;
                    float upY = (j + 1) * rectHeight - height / 2;
                    float downY = j * rectHeight - height / 2;

                    int v1 = AddVertex(vertices, new Vector3(leftX, upY, 0));
                    int v2 = AddVertex(vertices, new Vector3(rightX, upY, 0));
                    int v3 = AddVertex(vertices, new Vector3(rightX, downY, 0));
                    int v4 = AddVertex(vertices, new Vector3(leftX, downY, 0));

                    AddTriangle(triangles, v1, v2, v3);
                    AddTriangle(triangles, v1, v3, v4);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateCylinderMesh(int meridians, float height, float radius)
        {
            Mesh mesh = new();

            List<Vector3> vertices = new();
            List<int> triangles = new();

            float halfHeight = height / 2;
            int centerUp = AddVertex(vertices, new Vector3(0, halfHeight, 0));
            int centerDown = AddVertex(vertices, new Vector3(0, -halfHeight, 0));

            int[] cylinderVertices = new int[meridians * 2];

            // create vertices

            for (int i = 0; i < meridians; i++)
            {
                float teta = i * Mathf.PI * 2 / meridians;
                int vUp = AddVertex(vertices, new Vector3(radius * Mathf.Cos(teta), halfHeight, radius * Mathf.Sin(teta)));
                int vDown = AddVertex(vertices, new Vector3(radius * Mathf.Cos(teta), -halfHeight, radius * Mathf.Sin(teta)));

                cylinderVertices[i * 2] = vUp;
                cylinderVertices[i * 2 + 1] = vDown;
            }

            // create triangles

            for (int j = 0; j < meridians; j++)
            {
                int next = (j + 1) % meridians;
                AddTriangle(triangles, cylinderVertices[j * 2], cylinderVertices[next * 2], cylinderVertices[j * 2 + 1]);
                AddTriangle(triangles, cylinderVertices[next * 2], cylinderVertices[next * 2 + 1], cylinderVertices[j * 2 + 1]);
                AddTriangle(triangles, centerUp, cylinderVertices[next * 2], cylinderVertices[j * 2]);
                AddTriangle(triangles, centerDown, cylinderVertices[j * 2 + 1], cylinderVertices[next * 2 + 1]);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateCylinderTruncMesh(int meridians, float height, float radius, int meridiansTrunc)
        {
            Mesh mesh = new();

            List<Vector3> vertices = new();
            List<int> triangles = new();

            float halfHeight = height / 2;
            int meridiansAfterTrunc = meridians - meridiansTrunc;
            int centerUp = AddVertex(vertices, new Vector3(0, halfHeight, 0));
            int centerDown = AddVertex(vertices, new Vector3(0, -halfHeight, 0));

            int[] cylinderVertices = new int[meridiansAfterTrunc * 2];

            // create vertices

            for (int i = 0; i < meridiansAfterTrunc; i++)
            {
                float teta = i * Mathf.PI * 2 / meridians;
                int vUp = AddVertex(vertices, new Vector3(radius * Mathf.Cos(teta), halfHeight, radius * Mathf.Sin(teta)));
                int vDown = AddVertex(vertices, new Vector3(radius * Mathf.Cos(teta), -halfHeight, radius * Mathf.Sin(teta)));

                cylinderVertices[i * 2] = vUp;
                cylinderVertices[i * 2 + 1] = vDown;
            }

            // create triangles

            for (int j = 0; j < meridiansAfterTrunc - 1; j++)
            {
                int next = j + 1;
                AddTriangle(triangles, cylinderVertices[j * 2], cylinderVertices[next * 2], cylinderVertices[j * 2 + 1]);
                AddTriangle(triangles, cylinderVertices[next * 2], cylinderVertices[next * 2 + 1], cylinderVertices[j * 2 + 1]);
                AddTriangle(triangles, centerUp, cylinderVertices[next * 2], cylinderVertices[j * 2]);
                AddTriangle(triangles, centerDown, cylinderVertices[j * 2 + 1], cylinderVertices[next * 2 + 1]);
            }
            AddTriangle(triangles, centerUp, cylinderVertices[0], centerDown);
            AddTriangle(triangles, centerDown, cylinderVertices[0], cylinderVertices[1]);
            AddTriangle(triangles, centerUp, centerDown, cylinderVertices[cylinderVertices.Count() - 2]);
            AddTriangle(triangles, centerDown, cylinderVertices[cylinderVertices.Count() - 1], cylinderVertices[cylinderVertices.Count() - 2]);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateSphereMesh(int parallels, int meridians)
        {
            Mesh mesh = new();

            List<Vector3> vertices = new();
            List<int> triangles = new();

            int northHemisphere = AddVertex(vertices, new Vector3(0, 1, 0));
            int southHemisphere = AddVertex(vertices, new Vector3(0, -1, 0));

            int vertecesCount = meridians * parallels;
            int[] sphereVertices = new int[vertecesCount];

            // create vertices

            for (int i = 0; i < parallels; i++)
            {
                float height2 = Mathf.Cos((i + 1) * Mathf.PI / (parallels + 1));
                Debug.Log(height2);
                float new_r = Mathf.Sqrt(1 - Mathf.Pow(height2, 2));
                float angleGap = 2 * Mathf.PI / meridians;
                float teta = 0;

                for (int j = 0; j < meridians; j++)
                {
                    teta += angleGap;
                    int point = AddVertex(vertices, new Vector3(new_r * Mathf.Cos(teta), height2, new_r * Mathf.Sin(teta)));
                    sphereVertices[i * meridians + j] = point;
                }
            }

            // create triangles

            for (int i = 0; i < parallels - 1; i++)
            {
                for (int j = 0; j < meridians; j++)
                {
                    int next = (j + 1) % meridians;

                    AddTriangle(
                        triangles,
                        sphereVertices[i * meridians + j],
                        sphereVertices[i * meridians + next],
                        sphereVertices[(i + 1) * meridians + j]
                    );
                    AddTriangle(
                        triangles,
                        sphereVertices[i * meridians + next],
                        sphereVertices[(i + 1) * meridians + next],
                        sphereVertices[(i + 1) * meridians + j]
                    );
                }
            }

            int lastRowStart = vertecesCount - meridians;
            for (int j = 0; j < meridians; j++)
            {
                int next = (j + 1) % meridians;
                AddTriangle(
                    triangles,
                    northHemisphere,
                    sphereVertices[next],
                    sphereVertices[j]
                );
                AddTriangle(
                    triangles,
                    southHemisphere,
                    sphereVertices[lastRowStart + j],
                    sphereVertices[lastRowStart + next]
                );
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateSphereTruncMesh(int parallels, int meridians, int parallelsTrunc, int meridiansTrunc)
        {
            Mesh mesh = new();

            List<Vector3> vertices = new();
            List<int> triangles = new();

            int parallelsAfterTrunc = parallels - parallelsTrunc;
            int meridiansAfterTrunc = meridians - meridiansTrunc;
            int meridiansPlusCenter = meridiansAfterTrunc + 1;
            int northHemisphere = AddVertex(vertices, new Vector3(0, 1, 0));
            int southHemisphere = AddVertex(vertices, new Vector3(0, Mathf.Cos(parallelsAfterTrunc * Mathf.PI / (parallels + 1)), 0));

            int vertecesCount = meridiansPlusCenter * parallelsAfterTrunc;
            int[] sphereVertices = new int[vertecesCount];

            // create vertices

            for (int i = 0; i < parallelsAfterTrunc; i++)
            {
                float height = Mathf.Cos((i + 1) * Mathf.PI / (parallels + 1));
                float new_r = Mathf.Sqrt(1 - Mathf.Pow(height, 2));
                float angleGap = 2 * Mathf.PI / meridians;
                float teta = 0;

                int center = AddVertex(vertices, new Vector3(0, height, 0));
                sphereVertices[i * meridiansPlusCenter] = center;

                for (int j = 0; j < meridiansAfterTrunc; j++)
                {
                    teta += angleGap;
                    int point = AddVertex(vertices, new Vector3(new_r * Mathf.Cos(teta), height, new_r * Mathf.Sin(teta)));
                    sphereVertices[i * meridiansPlusCenter + 1 + j] = point;
                }
            }

            // create triangles

            for (int i = 0; i < parallelsAfterTrunc - 1; i++)
            {
                for (int j = 0; j < (meridiansAfterTrunc - 1); j++)
                {
                    int next = j + 1;

                    AddTriangle(
                        triangles,
                        sphereVertices[i * meridiansPlusCenter + j + 1],
                        sphereVertices[i * meridiansPlusCenter + next + 1],
                        sphereVertices[(i + 1) * meridiansPlusCenter + j + 1]
                    );
                    AddTriangle(
                        triangles,
                        sphereVertices[i * meridiansPlusCenter + next + 1],
                        sphereVertices[(i + 1) * meridiansPlusCenter + next + 1],
                        sphereVertices[(i + 1) * meridiansPlusCenter + j + 1]
                    );
                }
                AddTriangle(
                    triangles,
                    sphereVertices[i * meridiansPlusCenter],
                    sphereVertices[i * meridiansPlusCenter + 1],
                    sphereVertices[(i + 1) * meridiansPlusCenter]
                );
                AddTriangle(
                    triangles,
                    sphereVertices[i * meridiansPlusCenter + 1],
                    sphereVertices[(i + 1) * meridiansPlusCenter + 1],
                    sphereVertices[(i + 1) * meridiansPlusCenter]
                );
                AddTriangle(
                    triangles,
                    sphereVertices[i * meridiansPlusCenter],
                    sphereVertices[(i + 1) * meridiansPlusCenter],
                    sphereVertices[(i + 1) * meridiansPlusCenter - 1]
                );
                AddTriangle(
                    triangles,
                    sphereVertices[(i + 1) * meridiansPlusCenter - 1],
                    sphereVertices[(i + 1) * meridiansPlusCenter],
                    sphereVertices[(i + 2) * meridiansPlusCenter - 1]
                );
            }

            AddTriangle(
                triangles,
                northHemisphere,
                sphereVertices[1],
                sphereVertices[0]
            );
            AddTriangle(
                triangles,
                northHemisphere,
                sphereVertices[0],
                sphereVertices[meridiansAfterTrunc]
            );

            int lastRowStart = vertecesCount - meridiansAfterTrunc;
            for (int j = 0; j < (meridiansAfterTrunc - 1); j++)
            {
                int next = j + 1;
                AddTriangle(
                    triangles,
                    northHemisphere,
                    sphereVertices[next + 1],
                    sphereVertices[j + 1]
                );
                AddTriangle(
                    triangles,
                    southHemisphere,
                    sphereVertices[lastRowStart + j],
                    sphereVertices[lastRowStart + next]
                );
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateConeMesh(int meridians, float height, float radius)
        {
            Mesh mesh = new();

            List<Vector3> vertices = new();
            List<int> triangles = new();

            int centerUp = AddVertex(vertices, new Vector3(0, height, 0));
            int centerDown = AddVertex(vertices, new Vector3(0, 0, 0));

            int[] coneVertices = new int[meridians];

            // create vertices

            for (int i = 0; i < meridians; i++)
            {
                float teta = i * Mathf.PI * 2 / meridians;
                int v = AddVertex(vertices, new Vector3(radius * Mathf.Cos(teta), 0, radius * Mathf.Sin(teta)));
                coneVertices[i] = v;
            }

            // create triangles

            for (int j = 0; j < meridians; j++)
            {
                int next = (j + 1) % meridians;
                AddTriangle(triangles, centerDown, coneVertices[j], coneVertices[next]);
                AddTriangle(triangles, centerUp, coneVertices[next], coneVertices[j]);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh CreateConeTruncMesh(int meridians, float coneHeight, float radius, float truncHeight, int meridiansTrunc)
        {
            Mesh mesh = new();

            List<Vector3> vertices = new();
            List<int> triangles = new();

            int meridiansAfterTrunc = meridians - meridiansTrunc;
            int centerUp = AddVertex(vertices, new Vector3(0, truncHeight, 0));
            int centerDown = AddVertex(vertices, new Vector3(0, 0, 0));
            float truncRadius = (coneHeight - truncHeight) * radius / coneHeight;

            int[] coneVertices = new int[meridiansAfterTrunc * 2];

            // create vertices
            for (int i = 0; i < meridiansAfterTrunc; i++)
            {
                float teta = i * Mathf.PI * 2 / meridians;
                int v = AddVertex(vertices, new Vector3(radius * Mathf.Cos(teta), 0, radius * Mathf.Sin(teta)));
                int truncV = AddVertex(vertices, new Vector3(truncRadius * Mathf.Cos(teta), truncHeight, truncRadius * Mathf.Sin(teta)));

                coneVertices[i * 2] = truncV;
                coneVertices[i * 2 + 1] = v;
            }

            // create triangles
            for (int j = 0; j < meridiansAfterTrunc - 1; j++)
            {
                int next = j + 1;
                AddTriangle(triangles, coneVertices[j * 2], coneVertices[next * 2], coneVertices[j * 2 + 1]);
                AddTriangle(triangles, coneVertices[next * 2], coneVertices[next * 2 + 1], coneVertices[j * 2 + 1]);
                AddTriangle(triangles, centerUp, coneVertices[next * 2], coneVertices[j * 2]);
                AddTriangle(triangles, centerDown, coneVertices[j * 2 + 1], coneVertices[next * 2 + 1]);
            }
            AddTriangle(triangles, centerUp, coneVertices[0], centerDown);
            AddTriangle(triangles, centerDown, coneVertices[0], coneVertices[1]);
            AddTriangle(triangles, centerUp, centerDown, coneVertices[coneVertices.Count() - 2]);
            AddTriangle(triangles, centerDown, coneVertices[coneVertices.Count() - 1], coneVertices[coneVertices.Count() - 2]);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static void GenerateMesh(Mesh mesh, string name, string folderPath = "Assets/Meshs")
        {
            string assetPath = $"{folderPath}/{name}.asset";

            // Check and create folder if needed
            //CreateFoldersRecursively(folderPath);

            // Confirmation overlay if already exists
            if (AssetDatabase.LoadAssetAtPath<Mesh>(assetPath) != null)
            {
                bool confirm = EditorUtility.DisplayDialog(
                    "Overwrite existing mesh?",
                    $"A mesh named '{name}' already exists at:\n{assetPath}\n\nDo you want to replace it?",
                    "Yes, overwrite",
                    "Cancel"
                );

                if (!confirm)
                {
                    Debug.Log("Mesh generation canceled by user.");
                    return;
                }

                AssetDatabase.DeleteAsset(assetPath);
            }

            AssetDatabase.CreateAsset(mesh, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Mesh sauvegardé : {assetPath}");
        }
        public static void CreateFoldersRecursively(string fullPath)
        {
            // fullPath must be a Unity path, ex : "Assets/Meshs/Objets/Data"
            string[] parts = fullPath.Split('/');

            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];

                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        public static Mesh CreateSpecialMesh(List<Vector3> vertices, List<int> triangles)
        {
            Vector3 sum = Vector3.zero;

            foreach (Vector3 v in vertices)
            {
                sum += v;
            }

            Vector3 center = sum / vertices.Count;

            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] -= center;
            }

            float max = 0f;

            foreach (Vector3 v in vertices)
            {
                if (v.x > max)
                {
                    max = v.x;
                }
                if (v.y > max)
                {
                    max = v.y;
                }
                if (v.z > max)
                {
                    max = v.z;
                }
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] /= max;
            }

            Mesh mesh = new()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                normals = CalculateNormals(vertices, triangles).ToArray()
            };

            //mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh RemoveTriangles(Mesh mesh, int triangleToRemoveCount)
        {
            List<int> newTrianglesList = mesh.triangles.ToList();
            newTrianglesList.RemoveRange(mesh.triangles.Length - (triangleToRemoveCount * 3), triangleToRemoveCount * 3);
            Debug.Log(newTrianglesList);

            Mesh newMesh = new()
            {
                vertices = mesh.vertices.ToArray(),
                triangles = newTrianglesList.ToArray(),
                normals = mesh.normals.ToArray()
            };

            return newMesh;
        }
    }
}
