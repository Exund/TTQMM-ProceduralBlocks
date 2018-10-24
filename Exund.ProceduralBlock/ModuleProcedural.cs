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
        protected bool deserializing = false;
        protected List<IntVector3> cells = new List<IntVector3> { IntVector3.zero };
        protected List<Vector3> aps = new List<Vector3>();
        protected IntVector3 size = IntVector3.one;
        protected virtual float MassScaler => 1f;
        protected virtual float HealthScaler => MassScaler;

        protected Vector3[] originalVertices;

        protected static PropertyInfo ConnectedBlocksByAP;
        protected static FieldInfo m_BlockCellBounds;
        protected static MethodInfo CalculateDefaultPhysicsConstants;
        protected static FieldInfo s_BlockSerializationBuffer;
        protected static FieldInfo bufferLength;
        protected static MethodInfo GetValue;

        protected static FieldInfo SpawnContext_block;
        protected static FieldInfo SpawnContext_blockSpec;

        static ModuleProcedural()
        {
            ConnectedBlocksByAP = typeof(TankBlock).GetProperty("ConnectedBlocksByAP");
            m_BlockCellBounds = typeof(TankBlock).GetField("m_BlockCellBounds", BindingFlags.Instance | BindingFlags.NonPublic);//.First(f => f.Name.Contains("m_BlockCellBounds"));
            CalculateDefaultPhysicsConstants = typeof(TankBlock).GetMethod("CalculateDefaultPhysicsConstants", BindingFlags.Instance | BindingFlags.NonPublic);
            s_BlockSerializationBuffer = typeof(ManSpawn).GetField("s_BlockSerializationBuffer", BindingFlags.NonPublic | BindingFlags.Static);
            var t = s_BlockSerializationBuffer.FieldType;
            bufferLength = t.GetField("Length", BindingFlags.Public | BindingFlags.Instance);
            GetValue = t.GetMethod("GetValue", new Type[] { typeof(int) });

            t = typeof(ManSpawn).GetNestedType("SpawnContext", BindingFlags.NonPublic);
            SpawnContext_block = t.GetField("block");
            SpawnContext_blockSpec = t.GetField("blockSpec");
        }

        public List<IntVector3> Cells
        {
            get
            {
                var c = new List<IntVector3>();
                foreach (var cell in cells)
                {
                    c.Add(new IntVector3(cell.x, cell.y, cell.z));
                }
                return c;
            }
            set
            {
                if (base.block.IsAttached) return;
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
            get
            {
                var a = new List<Vector3>();
                foreach (var ap in aps)
                {
                    a.Add(new Vector3(ap.x, ap.y, ap.z));
                }
                return a;
            }
            set
            {
                if (base.block.IsAttached) return;
                aps = value;
                base.block.attachPoints = aps.ToArray();
                
                ConnectedBlocksByAP.SetValue(base.block, new TankBlock[aps.Count], null);
            }
        }

        public IntVector3 Size
        {
            get
            {
                return new IntVector3(size);
            }
            set
            {
                if (value == size) return;
                if (value.x < 1) value.x = 1;
                if (value.y < 1) value.y = 1;
                if (value.z < 1) value.z = 1;
                size = value;
                GenerateCellsAPs();
                GenerateMesh();
                GenerateProperties();
                Cells = cells;
                APs = aps;
            }
        }

        public void BeforeBlockAdded(IntVector3 localPos)
        {
            var serializationBuffer = (Array)s_BlockSerializationBuffer.GetValue(null);
            try
            {
                for (int i = 0; i < serializationBuffer.Length; i++)
                {
                    object sblock = serializationBuffer.GetValue(i);
                    var block = (TankBlock)SpawnContext_block.GetValue(sblock);
                    var blockSpec = (TankPreset.BlockSpec)SpawnContext_blockSpec.GetValue(sblock);

                    if (blockSpec.saveState.Count == 0) continue;
                    var data = Module.SerialData<ModuleProcedural.SerialData>.Retrieve(blockSpec.saveState);

                    if (base.block == block)
                    {
                        this.Size = data.size;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        protected virtual void GenerateProperties()
        {
            base.block.ChangeMass(base.block.m_DefaultMass * this.size.x * this.size.y * this.size.z * this.MassScaler);
            base.block.GetComponent<ModuleDamage>().maxHealth *= (int)(this.size.x * this.size.y * this.size.z * this.HealthScaler);
        }   

        private void OnSpawn()
        {
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
                    size = this.Size,
                    position = base.block.cachedLocalPosition
                };
                serialData.Store(blockSpec.saveState);
            }
            else
            {
                ModuleProcedural.SerialData serialData2 = Module.SerialData<ModuleProcedural.SerialData>.Retrieve(blockSpec.saveState);
                if (serialData2 != null)
                {
                    this.Size = serialData2.size;
                }
            }
        }

        [Serializable]
        private new class SerialData : Module.SerialData<ModuleProcedural.SerialData>
        {
            public IntVector3 size;
            public IntVector3 position;
        }
    }
}
