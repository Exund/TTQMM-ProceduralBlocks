using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace Exund.ProceduralBlocks
{
    public class ModuleProcedural : Module
    {
        protected List<IntVector3> cells = new List<IntVector3> { IntVector3.zero };
        protected List<Vector3> aps = new List<Vector3>();
        protected IntVector3 size = IntVector3.one;

        protected Vector3[] originalVertices;

        protected static PropertyInfo ConnectedBlocksByAP;
        protected static FieldInfo m_BlockCellBounds;
        protected static MethodInfo CalculateDefaultPhysicsConstants;

        static ModuleProcedural()
        {
            ConnectedBlocksByAP = typeof(TankBlock).GetProperty("ConnectedBlocksByAP");
            m_BlockCellBounds = typeof(TankBlock).GetField("m_BlockCellBounds", BindingFlags.Instance | BindingFlags.NonPublic);//.First(f => f.Name.Contains("m_BlockCellBounds"));
            CalculateDefaultPhysicsConstants = typeof(TankBlock).GetMethod("CalculateDefaultPhysicsConstants", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public List<IntVector3> Cells
        {
            get => cells;
            set
            {
                cells = value;
                base.block.filledCells = cells.ToArray();
                base.block.filledCellFlags = new byte[cells.Count];
                var bounds = new Bounds(Vector3.zero, Vector3.zero);
                foreach (IntVector3 c in cells)
                {
                    bounds.Encapsulate(c);
                }

                try
                {
                    m_BlockCellBounds.SetValue(base.block, bounds);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        public List<Vector3> APs
        {
            get => aps;
            set
            {
                aps = value;
                base.block.attachPoints = aps.ToArray();
                ConnectedBlocksByAP.SetValue(base.block, new TankBlock[aps.Count], null);
            }
        }

        public IntVector3 Size
        {
            get => size;
            set
            {
                if (value == size) return;
                if (value.x < 1) value.x = 1;
                if (value.y < 1) value.y = 1;
                if (value.z < 1) value.z = 1;
                size = value;
                GenerateCellsAPs();
                GenerateMesh();
            }
        }

        protected virtual void GenerateCellsAPs()
        {
            cells = new List<IntVector3>();
            aps = new List<Vector3>();
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        cells.Add(new IntVector3(x, y, z));
                                              
                        if (x == 0)
                        {
                            aps.Add(new Vector3(-0.5f, y, z));
                        }
                        if (x == size.x - 1)
                        {
                            aps.Add(new Vector3(x + 0.5f, y, z));
                        }
                        if (y == 0)
                        {
                            aps.Add(new Vector3(x, -0.5f, z));
                        }
                        if (y == size.y - 1)
                        {
                            aps.Add(new Vector3(x, y + 0.5f, z));
                        }
                        if (z == 0)
                        {
                            aps.Add(new Vector3(x, y, -0.5f));
                        }
                        if (z == size.z - 1)
                        {
                            aps.Add(new Vector3(x, y, z + 0.5f));
                        }                      
                    }
                }
            }       
        }

        protected virtual void GenerateMesh()
        {
            var meshFilter = base.block.GetComponentsInChildren<MeshFilter>().FirstOrDefault(mf => mf.sharedMesh.name == "ProceduralMesh");
            var meshCollider = base.block.GetComponentsInChildren<MeshCollider>().FirstOrDefault(mf => mf.sharedMesh.name == "ProceduralMesh");
            if (!meshFilter) return;

            var mesh = Instantiate(meshFilter.sharedMesh);
            mesh.name = "ProceduralMesh";

            if (originalVertices == null) originalVertices = mesh.vertices;
            var vertices = (Vector3[])originalVertices.Clone();
            
            for (int i = 0; i < vertices.Length; i++)
            {
                var x = vertices[i].x * size.x + 0.5f * (size.x - 1);
                var y = vertices[i].y * size.y + 0.5f * (size.y - 1);
                var z = vertices[i].z * size.z + 0.5f * (size.z - 1);
                vertices[i] = new Vector3(x, y, z);
            }
            
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
            CalculateDefaultPhysicsConstants.Invoke(base.block, null);
        }

        public void Update()
        {
            if (this.Cells.ToArray() != base.block.filledCells) this.Cells = this.cells;
            if (this.APs.ToArray() != base.block.attachPoints) this.APs = this.aps;
        }

        private void OnSpawn()
        {
            this.Cells = base.block.filledCells.ToList();
            this.APs = base.block.attachPoints.ToList();
            this.Size = this.size;
        }

        private void OnPool()
        {
            base.block.serializeEvent.Subscribe(new Action<bool, TankPreset.BlockSpec>(this.OnSerialize));
            base.block.serializeTextEvent.Subscribe(new Action<bool, TankPreset.BlockSpec>(this.OnSerialize));
        }

        private void OnSerialize(bool saving, TankPreset.BlockSpec blockSpec)
        {
            if (saving)
            {
                ModuleProcedural.SerialData serialData = new ModuleProcedural.SerialData()
                {
                    cells = this.cells,
                    aps = this.aps
                };
                serialData.Store(blockSpec.saveState);
            }
            else
            {
                ModuleProcedural.SerialData serialData2 = Module.SerialData<ModuleProcedural.SerialData>.Retrieve(blockSpec.saveState);
                if (serialData2 != null)
                {
                    this.Cells = serialData2.cells;
                    this.APs = serialData2.aps;
                }
            }
        }

        [Serializable]
        private new class SerialData : Module.SerialData<ModuleProcedural.SerialData>
        {
            public List<IntVector3> cells;
            public List<Vector3> aps;
        }
    }
}
