using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Nuterra.BlockInjector;

namespace Exund.ProceduralBlocks
{
    public class ProceduralBlocksMod
    {
        private static GameObject _holder;
        public static void Load()
        {
            /*
             * Need to add harmony patch
             * Before : BlockManager.AddBlock
             * 
             * Check if block has ModuleProcedural
             * Call ModuleProcedural.BeforeBlockAdded
             * in BeforeBlockAdded generate Cells and APs
             */


            _holder = new GameObject();
            _holder.AddComponent<ProceduralEditor>();
            UnityEngine.Object.DontDestroyOnLoad(_holder);

            var t = new Texture2D(1, 1);
            t.SetPixel(0, 0, Color.white);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material.color = Color.white;
            cube.GetComponent<MeshRenderer>().material.mainTexture = t;
            var m = cube.GetComponent<MeshFilter>().sharedMesh;
            m.name = "ProceduralMesh";
            var procedural_block = new BlockPrefabBuilder()
                .SetBlockID(7001, "fd49964942512e91ec41")
                .SetName("Procedural Block")
                .SetDescription("A block that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(m, m, true, cube.GetComponent<MeshRenderer>().material)
                .SetSize(IntVector3.one, BlockPrefabBuilder.AttachmentPoints.All)
                .AddComponent<ModuleProcedural>();
            procedural_block.RegisterLater();


            var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.GetComponent<MeshRenderer>().material.color = Color.white;
            cylinder.GetComponent<MeshRenderer>().material.mainTexture = t;
            var m2 = cylinder.GetComponent<MeshFilter>().sharedMesh;
            m2.name = "ProceduralMesh";
            var vertices = (Vector3[])m2.vertices.Clone();
            for (int i = 0; i < vertices.Length; i++)
            {
                var y = vertices[i].y * 0.5f;
                vertices[i] = new Vector3(vertices[i].x, y, vertices[i].z);
            }
            m2.vertices = vertices;
            m2.RecalculateBounds();

            var procedural_cylinder = new BlockPrefabBuilder()
                .SetBlockID(7002, "8ccfce9497e1ace52d5b")
                .SetName("Procedural Cylinder")
                .SetDescription("A cylinder that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(m2, m2, true, cylinder.GetComponent<MeshRenderer>().material)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralCylinder>();
            procedural_cylinder.TankBlock.attachPoints = new Vector3[] { new Vector3(0, -0.5f, 0), new Vector3(0, 0.5f, 0) };
            procedural_cylinder.RegisterLater();


            var m3 = new Mesh();
            vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f)
            };
            var uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(0,0);
            }
            var triangles = new int[]
            {
                0,1,2,
                1,3,2,
                0,4,1,
                4,5,1,
                0,2,4,
                1,5,3,
                2,5,4,
                2,3,5
            };
            m3.vertices = vertices;
            m3.uv = uvs;
            m3.triangles = triangles;
            m3.RecalculateBounds();
            m3.RecalculateNormals();
            m3.RecalculateTangents();
            m3.name = "ProceduralMesh";
            var procedural_half = new BlockPrefabBuilder()
                .SetBlockID(7003, "89c854222b7088b24b72")
                .SetName("Procedural Half Block")
                .SetDescription("A half block that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(m3, m3, true, cube.GetComponent<MeshRenderer>().material)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralHalfBlock>();
            procedural_half.TankBlock.attachPoints = new Vector3[] { new Vector3(0, -0.5f, 0), new Vector3(-0.5f, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(0, 0, 0.5f) };
            procedural_half.RegisterLater();

            var m4 = new Mesh();
            vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f)
            };
            uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(0, 0);
            }
            triangles = new int[]
            {
                0,1,2,
                1,3,2,
                0,4,1,
                0,2,4,
                1,4,3,
                2,3,4
            };
            m4.vertices = vertices;
            m4.uv = uvs;
            m4.triangles = triangles;
            m4.RecalculateBounds();
            m4.RecalculateNormals();
            m4.RecalculateTangents();
            m4.name = "ProceduralMesh";
            var procedural_corner_2 = new BlockPrefabBuilder()
                .SetBlockID(7004, "3302c74153a768f81be1")
                .SetName("Procedural Corner (2-way)")
                .SetDescription("A corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(m4, m4, true, cube.GetComponent<MeshRenderer>().material)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralCorner2>();
            procedural_corner_2.TankBlock.attachPoints = new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, -0.5f) };
            procedural_corner_2.RegisterLater();

            var m5 = new Mesh();
            vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                //new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f)
            };
            uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }
            triangles = new int[]
            {
                0,1,2,
                0,3,1,
                0,2,3,
                1,3,2                
            };
            m5.vertices = vertices;
            m5.uv = uvs;
            m5.triangles = triangles;
            m5.RecalculateBounds();
            m5.RecalculateNormals();
            m5.RecalculateTangents();
            m5.name = "ProceduralMesh";
            var procedural_corner_3 = new BlockPrefabBuilder()
                .SetBlockID(7005, "5397ca4583307f7d37fe")
                .SetName("Procedural Corner (3-way)")
                .SetDescription("A corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(m5, m5, true, cube.GetComponent<MeshRenderer>().material)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralCorner3>();
            procedural_corner_3.TankBlock.attachPoints = new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, -0.5f) };
            procedural_corner_3.RegisterLater();
        }

        public static bool PointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            var A = 1 / 2 * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
            var sign = A < 0 ? -1 : 1;
            var s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y) * sign;
            var t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y) * sign;

            return s > 0 && t > 0 && (s + t) < 2 * A * sign;
        }
    }
}
