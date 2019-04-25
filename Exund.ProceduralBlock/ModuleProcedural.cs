using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
        protected float originalMaxHealth;
        protected virtual float MassScaler => 1f;
        protected virtual float HealthScaler => MassScaler;
		protected string texture = "";

		protected Vector3[] originalVertices;

        protected static FieldInfo FilledCellsGravityScaleFactors;
        protected static PropertyInfo ConnectedBlocksByAP;
        protected static FieldInfo m_BlockCellBounds;
        protected static MethodInfo CalculateDefaultPhysicsConstants;
        protected static FieldInfo m_PopulateTechBuffer;
        protected static FieldInfo bufferLength;
        protected static MethodInfo GetValue;
		protected static MethodInfo InitAPFilledCells;


		protected static FieldInfo SpawnContext_block;
        protected static FieldInfo SpawnContext_blockSpec;

        private bool spawned = false;

        public Dictionary<Face, bool> faces = new Dictionary<Face, bool>() {
            {Face.Top, true },
            {Face.Bottom, true },
            {Face.Left, true },
            {Face.Right, true },
            {Face.Front, true },
            {Face.Back, true }
        };

        public bool inverted = false;

        static ModuleProcedural()
        {
            FilledCellsGravityScaleFactors = typeof(TankBlock).GetField("FilledCellsGravityScaleFactors", BindingFlags.Instance | BindingFlags.NonPublic);
            ConnectedBlocksByAP = typeof(TankBlock).GetProperty("ConnectedBlocksByAP");
            m_BlockCellBounds = typeof(TankBlock).GetField("m_BlockCellBounds", BindingFlags.Instance | BindingFlags.NonPublic);//.First(f => f.Name.Contains("m_BlockCellBounds"));
            CalculateDefaultPhysicsConstants = typeof(TankBlock).GetMethod("CalculateDefaultPhysicsConstants", BindingFlags.Instance | BindingFlags.NonPublic);
			InitAPFilledCells = typeof(TankBlock).GetMethod("InitAPFilledCells", BindingFlags.Instance | BindingFlags.NonPublic);
			m_PopulateTechBuffer = typeof(ManSpawn).GetField("m_PopulateTechBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
            var t = m_PopulateTechBuffer.FieldType;
            bufferLength = t.GetField("Length", BindingFlags.Public | BindingFlags.Instance);
            GetValue = t.GetMethod("GetValue", new Type[] { typeof(int) });

            t = typeof(ManSpawn).GetNestedType("PopulateTechBlockInfo", BindingFlags.NonPublic);
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
                var g = new float[cells.Count];
                for (int i = 0; i < g.Length; i++)
                {
                    g[i] = 1f;
                }
                FilledCellsGravityScaleFactors.SetValue(base.block, g);
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

                if (!faces[Face.Top]) value.RemoveAll(v => v.y == size.y - 0.5f);
                if (!faces[Face.Bottom]) value.RemoveAll(v => v.y == -0.5f);
                if (!faces[Face.Left]) value.RemoveAll(v => v.z == size.z - 0.5f);
                if (!faces[Face.Right]) value.RemoveAll(v => v.z == -0.5f);
                if (!faces[Face.Front]) value.RemoveAll(v => v.x == size.x - 0.5f);
                if (!faces[Face.Back]) value.RemoveAll(v => v.x == -0.5f);

                aps = value;
                base.block.attachPoints = aps.ToArray();
                
                ConnectedBlocksByAP.SetValue(base.block, new TankBlock[aps.Count], null);
				InitAPFilledCells.Invoke(base.block, new object[0]);
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
                if (value.x * value.y * value.z > 255) return;
                if (value.x < 1) value.x = 1;
                if (value.y < 1) value.y = 1;
                if (value.z < 1) value.z = 1;
                size = value;
                GenerateCellsAPs();
                GenerateMesh();
                GenerateProperties();
                Cells = cells;
                APs = aps;

                CalculateDefaultPhysicsConstants.Invoke(base.block, null);
            }
        }

		public string Texture
		{
			get
			{
				return this.texture;
			}
			set
			{
				var meshRenderer = base.block.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(mr => mr.material.name.Contains("ProceduralMaterial"));
				if (!meshRenderer) return;

				if (value == "" || !value.EndsWith(".png"))
				{
					meshRenderer.material.mainTexture = Texture2D.whiteTexture;
					this.texture = "";
				}
				else
				{
					this.texture = value;
					try
					{
						Texture2D t = new Texture2D(2, 2);
						t.LoadImage(File.ReadAllBytes(Path.Combine(ProceduralBlocksMod.AssetsFolder, "Textures/"+value)));
						meshRenderer.material.mainTexture = t;
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						meshRenderer.material.mainTexture = Texture2D.whiteTexture;
						this.texture = "";
					}
				}
			}
		}

        public void BeforeBlockAdded(IntVector3 localPos)
        {
            if (spawned) return;
            var serializationBuffer = (Array)m_PopulateTechBuffer.GetValue(ManSpawn.inst);
            
            try
            {
                for (int i = 0; i < serializationBuffer.Length; i++)
                {
                    object sblock = serializationBuffer.GetValue(i);
                    var bblock = (TankBlock)SpawnContext_block.GetValue(sblock);
                    var blockSpecn = (TankPreset.BlockSpec?)SpawnContext_blockSpec.GetValue(sblock);
                    if (blockSpecn == null) continue;
                    var blockSpec = (TankPreset.BlockSpec)blockSpecn;
                    if (blockSpec.saveState.Count == 0) continue;
                    var data = Module.SerialData<ModuleProcedural.SerialData>.Retrieve(blockSpec.saveState);
                    
                    if (base.block == bblock )
                    {
                        this.Size = data.size;
                        this.faces = data.faces ?? this.faces;
                        this.inverted = data.inverted;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            spawned = true;
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
        }

        protected virtual void GenerateProperties()
        {
            base.block.ChangeMass(base.block.m_DefaultMass * this.size.x * this.size.y * this.size.z * this.MassScaler);
            var healthScale = this.size.x * this.size.y * this.size.z * this.HealthScaler;

            var maxHealth = originalMaxHealth * healthScale;
            base.block.damage.maxHealth = (int)(maxHealth);

            var damageable = base.block.visible.damageable;
            var healed = damageable.IsAtFullHealth;
            damageable.SetMaxHealth(maxHealth);
            if (healed) damageable.InitHealth(-1337f);
        }   

        private void OnSpawn()
        {
            if (originalMaxHealth == 0f) originalMaxHealth = base.block.damage.maxHealth;
        }

        private void OnPool()
        {
            base.block.serializeEvent.Subscribe(new Action<bool, TankPreset.BlockSpec>(this.OnSerialize));
            base.block.serializeTextEvent.Subscribe(new Action<bool, TankPreset.BlockSpec>(this.OnSerialize));
        }

        private void OnRecycle()
        {
            this.Size = IntVector3.one;
			this.Texture = "";
            spawned = false;
        }

        private void OnSerialize(bool saving, TankPreset.BlockSpec blockSpec)
        {
            if (saving)
            {
				ModuleProcedural.SerialData serialData = new ModuleProcedural.SerialData()
				{
					size = this.Size,
					position = base.block.cachedLocalPosition,
					faces = this.faces,
					inverted = this.inverted,
					texture = this.texture
                };
                serialData.Store(blockSpec.saveState);
            }
            else
            {
                ModuleProcedural.SerialData serialData2 = Module.SerialData<ModuleProcedural.SerialData>.Retrieve(blockSpec.saveState);
                if (serialData2 != null)
                {
                    this.Size = serialData2.size;
					this.Texture = serialData2.texture;
                }
            }
        }

        [Serializable]
        private new class SerialData : Module.SerialData<ModuleProcedural.SerialData>
        {
            public IntVector3 size;
            public IntVector3 position;
            public Dictionary<Face, bool> faces;
            public bool inverted;
			public string texture;
        }

        public enum Face
        {
            Top,
            Bottom,
            Left,
            Right,
            Front,
            Back,
            All
        }
    }
}
