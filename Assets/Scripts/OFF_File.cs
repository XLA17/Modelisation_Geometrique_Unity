using Modeling.MeshTools;
using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
