using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using Nuterra.BlockInjector;

namespace Exund.ProceduralBlocks
{
    class ModuleProceduralSlicedMesh : ModuleProcedural
    {
        static Mesh AP = GameObjectJSON.MeshFromFile(Path.Combine(ProceduralBlocksMod.AssetsFolder, "Models/PartSets/AP.obj"));

        public static readonly Dictionary<string, SlicedMesh> SlicedMeshes = new Dictionary<string, SlicedMesh>();

        private string slicedMeshName = SlicedMeshes.Keys.First();

        public string SlicedMeshName
        {
            get
            {
                return slicedMeshName;
            }
            set
            {
                if(value != slicedMeshName)
                {
                    slicedMeshName = value;
                    GenerateMesh();
                }
            }
        }

        private static CombineInstance TransformSlice(Vector3 pos, CombineInstanceAlt slice, Quaternion rotation)
        {
            var transform = slice.transform;
            var mat = Matrix4x4.TRS(transform.translation + pos, transform.rotation * rotation, transform.scale);
            return new CombineInstance()
            {
                mesh = slice.mesh,
                transform = mat
            };
        }

        private static CombineInstance TransformSlice(Vector3 pos, CombineInstanceAlt slice)
        {   
            var transform = slice.transform;
            var mat = Matrix4x4.TRS(transform.translation + pos, transform.rotation, transform.scale);
            return new CombineInstance()
            {
                mesh = slice.mesh,
                transform = mat
            };
        }

        protected override void GenerateMesh()
        {
            var meshFilter = base.block.GetComponentsInChildren<MeshFilter>().FirstOrDefault(mf => mf.sharedMesh.name == "ProceduralMesh");
            if (!meshFilter) return;

            var mesh = Instantiate(meshFilter.sharedMesh);
            mesh.name = "ProceduralMesh";
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            List<CombineInstance> combines = new List<CombineInstance>();


            var slicedMesh = SlicedMeshes[slicedMeshName];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        var pos = new Vector3(x, y, z);

                        #region Corners
                        if (x == 0 && y == 0 && z == 0)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.CornerLeftBottomBack));
                        }
                        if (x == size.x - 1 && y == 0 && z == 0)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.CornerRightBottomBack));
                        }
                        if (x == 0 && y == size.y - 1 && z == 0)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.CornerLeftTopBack));
                        }
                        if (x == 0 && y == 0 && z == size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.CornerLeftBottomFront));
                        }
                        if (x == size.x - 1 && y == size.y - 1 && z == 0)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.CornerRightTopBack));
                        }
                        if (x == size.x - 1 && y == 0 && z == size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.CornerRightBottomFront));
                        }
                        if (x == 0 && y == size.y - 1 && z == size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.CornerLeftTopFront));
                        }
                        if (x == size.x - 1 && y == size.y - 1 && z == size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.CornerRightTopFront));
                        }
                        #endregion Corners

                        #region Edges
                        #region Vertical
                        if (x == 0 && y < size.y - 1 && z == 0)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeLeftBack));
                        }
                        if (x == size.x - 1 && y < size.y - 1 && z == 0)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeRightBack));
                        }
                        if (x == 0 && y < size.y - 1 && z == size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeLeftFront));
                        }
                        if (x == size.x - 1 && y < size.y - 1 && z == size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeRightFront));
                        }
                        #endregion Vertical

                        #region Horizontal
                        if (x == 0 && y == 0 && z < size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeLeftBottom));
                        }
                        if (x == size.x - 1 && y == 0 && z < size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeRightBottom));
                        }
                        if (x < size.x - 1 && y == 0 && z == 0)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeBottomBack));
                        }
                        if (x < size.x - 1 && y == 0 && z == size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeBottomFront));
                        }

                        if (x == 0 && y == size.y - 1 && z < size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeLeftTop));
                        }
                        if (x == size.x - 1 && y == size.y - 1 && z < size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeRightTop));
                        }
                        if (x < size.x - 1 && y == size.y - 1 && z == 0)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeTopBack));
                        }
                        if (x < size.x - 1 && y == size.y - 1 && z == size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.EdgeTopFront));
                        }
                        #endregion Horizontal
                        #endregion Edges

                        #region Faces
                        if (x == 0 && y < size.y - 1 && z < size.z - 1)
                        {
                            combines.Add(TransformSlice(pos , slicedMesh.FaceLeft));
                        }
                        if (x == size.x - 1 && y < size.y - 1 && z < size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.FaceRight));
                        }
                        if (z == 0 && x < size.x - 1 && y < size.y - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.FaceBack));
                        }
                        if (z == size.z - 1 && x < size.x - 1 && y < size.y - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.FaceFront));
                        }

                        if (y == 0 && x < size.x - 1 && z < size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.FaceBottom));
                        }
                        if (y == size.y - 1 && x < size.x - 1 && z < size.z - 1)
                        {
                            combines.Add(TransformSlice(pos, slicedMesh.FaceTop));
                        }
                        #endregion Faces

                        #region APs
                        if (x == 0 && faces[ModuleProcedural.Face.Left])
                        {
                            var mat = Matrix4x4.identity;
                            mat.SetTRS(pos, Quaternion.Euler(0, 0, -90f), Vector3.one);

                            combines.Add(new CombineInstance()
                            {
                                mesh = AP,
                                transform = mat
                            });
                        }
                        if (x == size.x - 1 && faces[ModuleProcedural.Face.Right])
                        {
                            var mat = Matrix4x4.identity;
                            mat.SetTRS(pos, Quaternion.Euler(0, 0, -90f), new Vector3(1, -1, 1));

                            combines.Add(new CombineInstance()
                            {
                                mesh = AP,
                                transform = mat
                            });
                        }

                        if (y == 0 && faces[ModuleProcedural.Face.Bottom])
                        {
                            var mat = Matrix4x4.identity;
                            mat.SetTRS(pos, Quaternion.identity, Vector3.one);

                            combines.Add(new CombineInstance()
                            {
                                mesh = AP,
                                transform = mat
                            });
                        }
                        if (y == size.y - 1 && faces[ModuleProcedural.Face.Top])
                        {
                            var mat = Matrix4x4.identity;
                            mat.SetTRS(pos, Quaternion.identity, new Vector3(1, -1, 1));

                            combines.Add(new CombineInstance()
                            {
                                mesh = AP,
                                transform = mat
                            });
                        }

                        if (z == 0 && faces[ModuleProcedural.Face.Back])
                        {
                            var mat = Matrix4x4.identity;
                            mat.SetTRS(pos, Quaternion.Euler(90f, 0, 0), Vector3.one);

                            combines.Add(new CombineInstance()
                            {
                                mesh = AP,
                                transform = mat
                            });
                        }
                        if (z == size.z - 1 && faces[ModuleProcedural.Face.Front])
                        {
                            var mat = Matrix4x4.identity;
                            mat.SetTRS(pos, Quaternion.Euler(90f, 0, 0), new Vector3(1, -1, 1));

                            combines.Add(new CombineInstance()
                            {
                                mesh = AP,
                                transform = mat
                            });
                        }
                        #endregion
                    }
                }
            }

            mesh.Clear();
            mesh.CombineMeshes(combines.ToArray());
            meshFilter.sharedMesh = mesh;

            var collider = base.block.GetComponentInChildren<BoxCollider>();
            collider.size = size;
            collider.center = (collider.size - Vector3.one) / 2f;
        }

        protected override void OnPool()
        {
            base.OnPool();
            base.block.serializeEvent.Subscribe(this.OnSerializeSliced);
            base.block.serializeTextEvent.Subscribe(this.OnSerializeSliced);
        }

        private void OnSerializeSliced(bool saving, TankPreset.BlockSpec blockSpec)
        {
            if (saving)
            {
                var serialData = new ModuleProceduralSlicedMesh.SerialData()
                {
                    slicedMeshName = slicedMeshName
                };
                serialData.Store(blockSpec.saveState);
            }
            else
            {
                var serialData2 = Module.SerialData<ModuleProceduralSlicedMesh.SerialData>.Retrieve(blockSpec.saveState);
                if (serialData2 != null)
                {
                    this.SlicedMeshName = serialData2.slicedMeshName;
                }
            }
        }

        [Serializable]
        public new class SerialData : Module.SerialData<ModuleProceduralSlicedMesh.SerialData>
        {
            public string slicedMeshName;
        }

        public struct Matrix4x4Alt
        {
            public Vector3 translation;
            public Quaternion rotation;
            public Vector3 scale;

            public static Matrix4x4Alt identity
            {
                get 
                {
                    return new Matrix4x4Alt()
                    {
                        translation = Vector3.zero,
                        rotation = Quaternion.identity,
                        scale = Vector3.one
                    };
                }
            }

            public void SetTRS(Vector3 t, Quaternion r, Vector3 s)
            {
                this.translation = t;
                this.rotation = r;
                this.scale = s;
            }

            public static implicit operator Matrix4x4(Matrix4x4Alt m)
            {
                return Matrix4x4.TRS(m.translation, m.rotation, m.scale);
            }
        }

        public struct CombineInstanceAlt
        {
            public Mesh mesh;
            public Matrix4x4Alt transform;
        }

        public class SlicedMesh
        {
            public string name;

            public CombineInstanceAlt CornerLeftBottomBack;
            public CombineInstanceAlt CornerRightBottomBack;

            public CombineInstanceAlt CornerLeftBottomFront;
            public CombineInstanceAlt CornerRightBottomFront;

            public CombineInstanceAlt CornerLeftTopBack;
            public CombineInstanceAlt CornerRightTopBack;

            public CombineInstanceAlt CornerLeftTopFront;
            public CombineInstanceAlt CornerRightTopFront;


            public CombineInstanceAlt EdgeBottomBack;
            public CombineInstanceAlt EdgeBottomFront;
            public CombineInstanceAlt EdgeLeftBottom;
            public CombineInstanceAlt EdgeRightBottom;

            public CombineInstanceAlt EdgeTopBack;
            public CombineInstanceAlt EdgeTopFront;
            public CombineInstanceAlt EdgeLeftTop;
            public CombineInstanceAlt EdgeRightTop;

            public CombineInstanceAlt EdgeLeftBack;
            public CombineInstanceAlt EdgeRightBack;
            public CombineInstanceAlt EdgeLeftFront;
            public CombineInstanceAlt EdgeRightFront;


            public CombineInstanceAlt FaceBottom;
            public CombineInstanceAlt FaceTop;

            public CombineInstanceAlt FaceBack;
            public CombineInstanceAlt FaceFront;

            public CombineInstanceAlt FaceLeft;
            public CombineInstanceAlt FaceRight;
        }

        public class SlicedMesh3 : SlicedMesh
        {
            public Mesh Corner
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    CornerLeftBottomBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, 1, 1));
                    CornerRightBottomBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
                    CornerLeftBottomFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, 1, -1));
                    CornerRightBottomFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };


                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
                    CornerLeftTopBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, -1, 1));
                    CornerRightTopBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, -1));
                    CornerLeftTopFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, -1, -1));
                    CornerRightTopFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh Edge
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), Vector3.one);
                    EdgeBottomBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), new Vector3(-1, 1, 1));

                    EdgeBottomFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(1, 1, 1));
                    EdgeLeftBottom = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(-1, 1, 1));
                    EdgeRightBottom = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };


                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), new Vector3(1, -1, 1));
                    EdgeTopBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), new Vector3(-1, -1, 1));
                    EdgeTopFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(1, -1, 1));
                    EdgeLeftTop = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(-1, -1, 1));
                    EdgeRightTop = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };



                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0), Quaternion.Euler(90, 0, 0), Vector3.one);
                    EdgeLeftBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0), Quaternion.Euler(90, 0, 0), new Vector3(-1, 1, 1));

                    EdgeRightBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0), Quaternion.Euler(-90, 0, 0), Vector3.one);

                    EdgeLeftFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0), Quaternion.Euler(-90, 0, 0), new Vector3(-1, 1, 1));

                    EdgeRightFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh Face
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0.5f, 0), Quaternion.Euler(0, -90f, 0), Vector3.one);
                    FaceBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0.5f, 0), Quaternion.Euler(0, -90f, 0), new Vector3(-1, 1, 1));

                    FaceFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0.5f), Quaternion.identity, Vector3.one);

                    FaceLeft = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0.5f), Quaternion.identity, new Vector3(-1, 1, 1));

                    FaceRight = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0.5f), Quaternion.Euler(0, 0, 90), Vector3.one);
                    FaceBottom = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0.5f), Quaternion.Euler(0, 0, 90), new Vector3(-1, 1, 1));

                    FaceTop = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }
        }

        public class SlicedMesh5 : SlicedMesh
        {
            public Mesh Corner
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    CornerLeftBottomBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                       
                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, 1, 1));
                    CornerRightBottomBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
                    CornerLeftBottomFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, 1, -1));
                    CornerRightBottomFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };


                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
                    CornerLeftTopBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, -1, 1));
                    CornerRightTopBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, -1));
                    CornerLeftTopFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, -1, -1));
                    CornerRightTopFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh VerticalEdge
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.up * 0.5f, Quaternion.identity, Vector3.one);
                    EdgeLeftBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.up * 0.5f, Quaternion.identity, new Vector3(-1, 1, 1));

                    EdgeRightBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.up * 0.5f, Quaternion.identity, new Vector3(1, 1, -1));

                    EdgeLeftFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.up * 0.5f, Quaternion.identity, new Vector3(-1, 1, -1));

                    EdgeRightFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh HorizontalEdge
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), Vector3.one);
                    EdgeBottomBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), new Vector3(-1, 1, 1));

                    EdgeBottomFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(1, 1, 1));
                    EdgeLeftBottom = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(-1, 1, 1));
                    EdgeRightBottom = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };


                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), new Vector3(1, -1, 1));
                    EdgeTopBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), new Vector3(-1, -1, 1));
                    EdgeTopFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(1, -1, 1));
                    EdgeLeftTop = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(-1, -1, 1));
                    EdgeRightTop = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh VerticalFace
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0.5f, 0), Quaternion.Euler(0, -90f, 0), Vector3.one);
                    FaceBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0.5f, 0), Quaternion.Euler(0, -90f, 0), new Vector3(-1, 1, 1));

                    FaceFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0.5f), Quaternion.identity, Vector3.one);

                    FaceLeft = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0.5f), Quaternion.identity, new Vector3(-1, 1, 1));

                    FaceRight = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh HorizontalFace
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0.5f), Quaternion.identity, Vector3.one);
                    FaceBottom = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0.5f), Quaternion.identity, new Vector3(1, -1, 1));

                    FaceTop = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }
        }

        public class SlicedMeshTMB : SlicedMesh
        {
            public Mesh CornerTop
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    CornerLeftTopBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, 1, 1));
                    CornerRightTopBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
                    CornerLeftTopFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, 1, -1));
                    CornerRightTopFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh CornerBottom
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    CornerLeftBottomBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, 1, 1));
                    CornerRightBottomBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
                    CornerLeftBottomFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1, 1, -1));
                    CornerRightBottomFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh EdgeTop
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), Vector3.one);
                    EdgeTopBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), new Vector3(-1, 1, 1));
                    EdgeTopFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, Vector3.one);
                    EdgeLeftTop = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(-1, 1, 1));
                    EdgeRightTop = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh EdgeMiddle
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.up * 0.5f, Quaternion.identity, Vector3.one);
                    EdgeLeftBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.up * 0.5f, Quaternion.identity, new Vector3(-1, 1, 1));

                    EdgeRightBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.up * 0.5f, Quaternion.identity, new Vector3(1, 1, -1));

                    EdgeLeftFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(Vector3.up * 0.5f, Quaternion.identity, new Vector3(-1, 1, -1));

                    EdgeRightFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh EdgeBottom
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), Vector3.one);
                    EdgeBottomBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0), Quaternion.identity * Quaternion.Euler(0, -90, 0), new Vector3(-1, 1, 1));

                    EdgeBottomFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(1, 1, 1));
                    EdgeLeftBottom = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0, 0.5f), Quaternion.identity, new Vector3(-1, 1, 1));
                    EdgeRightBottom = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public new Mesh FaceTop
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0.5f), Quaternion.identity, Vector3.one);

                    base.FaceTop = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public Mesh FaceMiddle
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0.5f, 0), Quaternion.Euler(0, -90f, 0), Vector3.one);
                    FaceBack = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0.5f, 0), Quaternion.Euler(0, -90f, 0), new Vector3(-1, 1, 1));

                    FaceFront = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0.5f), Quaternion.identity, Vector3.one);

                    FaceLeft = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };

                    mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0, 0.5f, 0.5f), Quaternion.identity, new Vector3(-1, 1, 1));

                    FaceRight = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }

            public new Mesh FaceBottom
            {
                set
                {
                    var mat = Matrix4x4Alt.identity;
                    mat.SetTRS(new Vector3(0.5f, 0, 0.5f), Quaternion.identity, Vector3.one);
                    base.FaceBottom = new CombineInstanceAlt()
                    {
                        mesh = value,
                        transform = mat
                    };
                }
            }
        }
    }
}
