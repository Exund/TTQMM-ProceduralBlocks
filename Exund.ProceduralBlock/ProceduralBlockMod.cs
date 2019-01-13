using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Nuterra.BlockInjector;
using Harmony;

namespace Exund.ProceduralBlocks
{
    public class ProceduralBlocksMod
    {
        private static GameObject _holder;
        public static void Load()
        {
            var harmony = HarmonyInstance.Create("exund.prodcedural.blocks");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            _holder = new GameObject();
            _holder.AddComponent<ProceduralEditor>();
            UnityEngine.Object.DontDestroyOnLoad(_holder);

            var t = new Texture2D(1, 1);
            t.SetPixel(0, 0, Color.white);

            Material mat = GameObjectJSON.MaterialFromShader();
            mat.mainTexture = t;

            var cube = GameObjectJSON.MeshFromFile("Assets/Procedural Block.obj");
            cube.name = "ProceduralMesh";
            var procedural_block = new BlockPrefabBuilder()
                .SetBlockID(7001, "fd49964942512e91ec41")
                .SetName("Procedural Block")
                .SetDescription("A block that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(cube, cube, Material:mat)
                .SetSize(IntVector3.one, BlockPrefabBuilder.AttachmentPoints.All)
                .AddComponent<ModuleProcedural>();
            procedural_block.RegisterLater();

            var cylinder = GameObjectJSON.MeshFromFile("Assets/Procedural Cylinder.obj");
            cylinder.name = "ProceduralMesh";
            var procedural_cylinder = new BlockPrefabBuilder()
                .SetBlockID(7002, "8ccfce9497e1ace52d5b")
                .SetName("Procedural Cylinder")
                .SetDescription("A cylinder that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(cylinder, cylinder, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralCylinder>();
            procedural_cylinder.TankBlock.attachPoints = new Vector3[] { new Vector3(0, -0.5f, 0), new Vector3(0, 0.5f, 0) };
            procedural_cylinder.RegisterLater();

            var half = GameObjectJSON.MeshFromFile("Assets/Procedural Half Block.obj");
            half.name = "ProceduralMesh";
            var procedural_half = new BlockPrefabBuilder()
                .SetBlockID(7003, "89c854222b7088b24b72")
                .SetName("Procedural Half Block")
                .SetDescription("A half block that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250/2)
                .SetMass(1)
                .SetModel(half, half, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralHalfBlock>();
            procedural_half.TankBlock.attachPoints = new Vector3[] { new Vector3(0, -0.5f, 0), new Vector3(-0.5f, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(0, 0, 0.5f) };
            procedural_half.RegisterLater();

            var corner2 = GameObjectJSON.MeshFromFile("Assets/Procedural Corner (2-way).obj");
            corner2.name = "ProceduralMesh";
            var procedural_corner_2 = new BlockPrefabBuilder()
                .SetBlockID(7004, "3302c74153a768f81be1")
                .SetName("Procedural Corner (2-way)")
                .SetDescription("A corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(corner2, corner2, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralCorner2>();
            procedural_corner_2.TankBlock.attachPoints = new Vector3[] { new Vector3(0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, 0.5f) };
            procedural_corner_2.RegisterLater();

            var corner3 = GameObjectJSON.MeshFromFile("Assets/Procedural Corner (3-way).obj");
            corner3.name = "ProceduralMesh";
            var procedural_corner_3 = new BlockPrefabBuilder()
                .SetBlockID(7005, "5397ca4583307f7d37fe")
                .SetName("Procedural Corner (3-way)")
                .SetDescription("A corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250/3)
                .SetMass(1)
                .SetModel(corner3, corner3, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralCorner3>();
            procedural_corner_3.TankBlock.attachPoints = new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, -0.5f) };
            procedural_corner_3.RegisterLater();

            var rounded = GameObjectJSON.MeshFromFile("Assets/Procedural Rounded Half Block.obj");
            rounded.name = "ProceduralMesh";
            var procedural_rounded = new BlockPrefabBuilder()
                .SetBlockID(7006, "be4c9e7859e29073aec0")
                .SetName("Procedural Rounded Half Block")
                .SetDescription("A rounded half block that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250 / 2)
                .SetMass(1)
                .SetModel(rounded, rounded, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralRoundedHalfBlock>();
            procedural_rounded.TankBlock.attachPoints = new Vector3[] { new Vector3(0, -0.5f, 0), new Vector3(-0.5f, 0, 0), new Vector3(0, 0, -0.5f), new Vector3(0, 0, 0.5f) };
            procedural_rounded.RegisterLater();

            var sphere = GameObjectJSON.MeshFromFile("Assets/Procedural Sphere.obj");
            sphere.name = "ProceduralMesh";
            var procedural_sphere = new BlockPrefabBuilder()
                .SetBlockID(7007, "88df56b7bc96edd1e288")
                .SetName("Procedural Sphere")
                .SetDescription("A sphere that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(sphere, sphere, true, mat)
                .SetSize(IntVector3.one, BlockPrefabBuilder.AttachmentPoints.All)
                .AddComponent<ModuleProcedural>();
            procedural_sphere.RegisterLater();

            var converter = GameObjectJSON.MeshFromFile("Assets/Procedural Cylinder-Cube Converter.obj");
            converter.name = "ProceduralMesh";
            var procedural_converter = new BlockPrefabBuilder()
                .SetBlockID(7008, "92f1be44b016bb6e7e50")
                .SetName("Procedural Converter")
                .SetDescription("A cylinder that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(converter, converter, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralConverter>();
            procedural_converter.TankBlock.attachPoints = new Vector3[] { new Vector3(0, -0.5f, 0), new Vector3(0, 0.5f, 0) };
            procedural_converter.RegisterLater();

            var incorner2 = GameObjectJSON.MeshFromFile("Assets/Procedural Inside Corner (2-way).obj");
            incorner2.name = "ProceduralMesh";
            var procedural_incorner_2 = new BlockPrefabBuilder()
                .SetBlockID(7009, "d2976201068b86e4b7cc")
                .SetName("Procedural Inside Corner (2-way)")
                .SetDescription("A corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(incorner2, incorner2, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralCorner2>(out ModuleProceduralCorner2 inverted2);
            procedural_incorner_2.TankBlock.attachPoints = new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, -0.5f), new Vector3(0.5f, 0, 0), new Vector3(0, 0, 0.5f) };
            inverted2.inverted = true;
            procedural_incorner_2.RegisterLater();

            var incorner3 = GameObjectJSON.MeshFromFile("Assets/Procedural Inside Corner (3-way).obj");
            incorner3.name = "ProceduralMesh";
            var procedural_incorner_3 = new BlockPrefabBuilder()
                .SetBlockID(7010, "d85e864aad33b54a6028")
                .SetName("Procedural Inside Corner (3-way)")
                .SetDescription("A corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250 / 3)
                .SetMass(1)
                .SetModel(incorner3, incorner3, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralCorner3>(out ModuleProceduralCorner3 inverted3);
            procedural_incorner_3.TankBlock.attachPoints = new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, -0.5f), new Vector3(0.5f, 0, 0), new Vector3(0, 0.5f, 0), new Vector3(0, 0, 0.5f) };
            inverted3.inverted = true;
            procedural_incorner_3.RegisterLater();

            var rounded_corner2 = GameObjectJSON.MeshFromFile("Assets/Procedural Rounded Corner (2-way).obj");
            rounded_corner2.name = "ProceduralMesh";
            var procedural_rounded_corner2 = new BlockPrefabBuilder()
                .SetBlockID(7011, "ab23c00ba9ab31d27e04")
                .SetName("Procedural Rounded Corner (2-way)")
                .SetDescription("A rounded corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(rounded_corner2, rounded_corner2, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralRoundedCorner2>();
            procedural_rounded_corner2.TankBlock.attachPoints = new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, -0.5f) };
            procedural_rounded_corner2.RegisterLater();

            var rounded_corner3 = GameObjectJSON.MeshFromFile("Assets/Procedural Rounded Corner (3-way).obj");
            rounded_corner3.name = "ProceduralMesh";
            var procedural_rounded_corner3 = new BlockPrefabBuilder()
                .SetBlockID(7012, "9630f2a69c42c30c0862")
                .SetName("Procedural Rounded Corner (3-way)")
                .SetDescription("A rounded corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(rounded_corner3, rounded_corner3, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralRoundedCorner3>();
            procedural_rounded_corner3.TankBlock.attachPoints = new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, -0.5f) };
            procedural_rounded_corner3.RegisterLater();

            var rounded_incorner2 = GameObjectJSON.MeshFromFile("Assets/Procedural Inside Rounded Corner (2-way).obj");
            rounded_incorner2.name = "ProceduralMesh";
            var procedural_rounded_incorner2 = new BlockPrefabBuilder()
                .SetBlockID(7013, "2381cf216f226ed2f537")
                .SetName("Procedural Inside Rounded Corner (2-way)")
                .SetDescription("A rounded corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(rounded_incorner2, rounded_incorner2, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralRoundedCorner2>(out ModuleProceduralRoundedCorner2 invertedr2);
            procedural_rounded_incorner2.TankBlock.attachPoints = new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, -0.5f) };
            invertedr2.inverted = true;
            procedural_rounded_incorner2.RegisterLater();

            var rounded_incorner3 = GameObjectJSON.MeshFromFile("Assets/Procedural Inside Rounded Corner (3-way).obj");
            rounded_incorner3.name = "ProceduralMesh";
            var procedural_rounded_incorner3 = new BlockPrefabBuilder()
                .SetBlockID(7014, "eadb1d61f1dc3e7f1fc9")
                .SetName("Procedural Inside Rounded Corner (3-way)")
                .SetDescription("A rounded corner that can change size")
                .SetFaction(FactionSubTypes.EXP)
                .SetCategory(BlockCategories.Standard)
                .SetGrade()
                .SetHP(250)
                .SetMass(1)
                .SetModel(rounded_incorner3, rounded_incorner3, true, mat)
                .SetSize(IntVector3.one)
                .AddComponent<ModuleProceduralRoundedCorner3>(out ModuleProceduralRoundedCorner3 invertedr3);
            procedural_rounded_incorner3.TankBlock.attachPoints = new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0, 0, -0.5f) };
            invertedr3.inverted = true;
            procedural_rounded_incorner3.RegisterLater();
        }

        public static bool PointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            var A = 1 / 2 * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
            var sign = A < 0 ? -1 : 1;
            var s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y) * sign;
            var t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y) * sign;

            return s > 0 && t > 0 && (s + t) < 2 * A * sign;
        }

        internal class Patches
        {
            [HarmonyPatch(typeof(BlockManager), "AddBlock")]
            private static class BlockManagerFix
            {
                private static void Prefix(ref TankBlock block, IntVector3 localPos)
                {
                    var module = block.GetComponent<ModuleProcedural>();
                    if(module)
                    {
                        module.BeforeBlockAdded(localPos);
                    }
                }
            }
        }
    }
}
