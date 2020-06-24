using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Nuterra.BlockInjector;

namespace Exund.ProceduralBlocks
{
    internal class SlicedMeshJSON
    {
        static readonly string PartSetsPath = Path.Combine(ProceduralBlocksMod.AssetsFolder, "Models/PartSets/");

        static FieldInfo[] SlicedMesh26JSON_Fields = typeof(SlicedMesh26JSON).GetFields().Where(f => f.Name != "name").ToArray();
        static Dictionary<string, FieldInfo> SlicedMesh_Fields = new Dictionary<string, FieldInfo>();

        static SlicedMeshJSON()
        {
            foreach(var f in typeof(ModuleProceduralSlicedMesh.SlicedMesh).GetFields())
            {
                if(f.Name != "name")
                {
                    SlicedMesh_Fields.Add(f.Name, f);
                }
            }
        }

        public static string StripComments(string input)
        {
            input = Regex.Replace(input, @"^\s*//.*$", "", RegexOptions.Multiline);  // removes comments like this
            input = Regex.Replace(input, @"^\s*/\*(\s|\S)*?\*/\s*$", "", RegexOptions.Multiline); /* comments like this */

            return input;
        }

        public static void LoadSets()
        {
            Console.WriteLine("Loading Parts Sets for Procedural Blocks");
            foreach(var file in Directory.GetFiles(PartSetsPath, "*.json", SearchOption.AllDirectories))
            {
                var info = new FileInfo(file);
                Console.WriteLine("JSON found: " + info.Name);
                try
                {
                    var definition = JObject.Parse(StripComments(File.ReadAllText(info.FullName))).ToObject<SlicedMeshDifinitionJSON>();

                    var directory = info.DirectoryName;
                    if(definition.Full != null)
                    {
                        var partSet = (SlicedMesh26JSON)definition.Full;
                        var slicedMesh = new ModuleProceduralSlicedMesh.SlicedMesh()
                        {
                            name = partSet.name
                        };

                        foreach (var f in SlicedMesh26JSON_Fields)
                        {
                            var filename = ((string)f.GetValue(partSet)).Replace("../", "").Replace("..", "");
                            var mesh = GameObjectJSON.MeshFromFile(Path.Combine(directory, filename));
                            SlicedMesh_Fields[f.Name].SetValue(slicedMesh, new ModuleProceduralSlicedMesh.CombineInstanceAlt()
                            {
                                mesh = mesh,
                                transform = ModuleProceduralSlicedMesh.Matrix4x4Alt.identity
                            });
                        }

                        ModuleProceduralSlicedMesh.SlicedMeshes.Add(slicedMesh.name, slicedMesh);
                        Console.WriteLine("Full parts set added: " + slicedMesh.name);
                    }

                    if(definition.Minimal != null)
                    {
                        var partSet = (SlicedMesh3JSON)definition.Minimal;
                        var slicedMesh = new ModuleProceduralSlicedMesh.SlicedMesh3()
                        {
                            name = partSet.name,
                            Corner = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.Corner.Replace("../", "").Replace("..", ""))),
                            Edge = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.Edge.Replace("../", "").Replace("..", ""))),
                            Face = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.Face.Replace("../", "").Replace("..", "")))
                        };

                        ModuleProceduralSlicedMesh.SlicedMeshes.Add(slicedMesh.name, slicedMesh);
                        Console.WriteLine("Minimal parts set added: " + slicedMesh.name);
                    }

                    if (definition.Extended != null)
                    {
                        var partSet = (SlicedMesh5JSON)definition.Extended;
                        var slicedMesh = new ModuleProceduralSlicedMesh.SlicedMesh5()
                        {
                            name = partSet.name,
                            Corner = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.Corner.Replace("../", "").Replace("..", ""))),
                            VerticalEdge = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.VerticalEdge.Replace("../", "").Replace("..", ""))),
                            HorizontalEdge = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.HorizontalEdge.Replace("../", "").Replace("..", ""))),
                            VerticalFace = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.VerticalFace.Replace("../", "").Replace("..", ""))),
                            HorizontalFace = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.HorizontalFace.Replace("../", "").Replace("..", "")))
                        };

                        ModuleProceduralSlicedMesh.SlicedMeshes.Add(slicedMesh.name, slicedMesh);
                        Console.WriteLine("Extended parts set added: " + slicedMesh.name);
                    }

                    if (definition.TopMiddleBottom != null)
                    {
                        var partSet = (SlicedMeshTMBJSON)definition.TopMiddleBottom;
                        var slicedMesh = new ModuleProceduralSlicedMesh.SlicedMeshTMB()
                        {
                            name = partSet.name,
                            CornerBottom = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.CornerBottom.Replace("../", "").Replace("..", ""))),
                            CornerTop = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.CornerTop.Replace("../", "").Replace("..", ""))),
                            EdgeBottom = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.EdgeBottom.Replace("../", "").Replace("..", ""))),
                            EdgeMiddle = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.EdgeMiddle.Replace("../", "").Replace("..", ""))),
                            EdgeTop = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.EdgeTop.Replace("../", "").Replace("..", ""))),
                            FaceBottom = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.FaceBottom.Replace("../", "").Replace("..", ""))),
                            FaceMiddle = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.FaceMiddle.Replace("../", "").Replace("..", ""))),
                            FaceTop = GameObjectJSON.MeshFromFile(Path.Combine(directory, partSet.FaceTop.Replace("../", "").Replace("..", "")))
                        };

                        ModuleProceduralSlicedMesh.SlicedMeshes.Add(slicedMesh.name, slicedMesh);
                        Console.WriteLine("TMB parts set added: " + slicedMesh.name);
                    }
                } 
                catch (Exception e)
                {
                    Console.WriteLine("Parts set registration failed");
                    Console.WriteLine(e.ToString());
                }
            }
        }

        struct SlicedMeshDifinitionJSON
        {
            public SlicedMesh26JSON? Full;
            public SlicedMesh3JSON? Minimal;
            public SlicedMesh5JSON? Extended;
            public SlicedMeshTMBJSON? TopMiddleBottom;
        }

        struct SlicedMesh26JSON
        {
            public string name;

            public string CornerLeftBottomBack;
            public string CornerRightBottomBack;

            public string CornerLeftBottomFront;
            public string CornerRightBottomFront;

            public string CornerLeftTopBack;
            public string CornerRightTopBack;

            public string CornerLeftTopFront;
            public string CornerRightTopFront;


            public string EdgeBottomBack;
            public string EdgeBottomFront;
            public string EdgeLeftBottom;
            public string EdgeRightBottom;

            public string EdgeTopBack;
            public string EdgeTopFront;
            public string EdgeLeftTop;
            public string EdgeRightTop;

            public string EdgeLeftBack;
            public string EdgeRightBack;
            public string EdgeLeftFront;
            public string EdgeRightFront;


            public string FaceBottom;
            public string FaceTop;

            public string FaceBack;
            public string FaceFront;

            public string FaceLeft;
            public string FaceRight;
        }

        struct SlicedMesh3JSON
        {
            public string name;

            public string Corner;
            public string Edge;
            public string Face;
        }

        struct SlicedMesh5JSON
        {
            public string name;

            public string Corner;
            public string VerticalEdge;
            public string HorizontalEdge;
            public string VerticalFace;
            public string HorizontalFace;
        }

        struct SlicedMeshTMBJSON
        {
            public string name;

            public string CornerTop;
            public string CornerBottom;
            public string EdgeTop;
            public string EdgeMiddle;
            public string EdgeBottom;
            public string FaceTop;
            public string FaceMiddle;
            public string FaceBottom;
        }
    }
}
