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
        protected static FieldInfo FilledCellsGravityScaleFactors;
        protected static FieldInfo m_LastFilledCellsGravityScaleFactors;
        protected static PropertyInfo ConnectedBlocksByAP;
        protected static FieldInfo m_BlockCellBounds;
        protected static MethodInfo CalculateDefaultPhysicsConstants;
        protected static FieldInfo m_PopulateTechBuffer;
        protected static FieldInfo bufferLength;
        protected static MethodInfo GetValue;
        protected static MethodInfo InitAPFilledCells;
        protected static FieldInfo m_DefaultInertiaTensor;

        protected static FieldInfo SpawnContext_block;
        protected static FieldInfo SpawnContext_blockSpec;

        protected MeshRenderer meshRenderer;
        protected MeshFilter meshFilter;
        protected MeshCollider meshCollider;

        protected bool deserializing = false;
        protected List<IntVector3> cells = new List<IntVector3> { IntVector3.zero };
        protected List<Vector3> aps = new List<Vector3>();
        protected IntVector3 size = IntVector3.one;
        protected float originalMaxHealth;
        protected virtual float MassScaler => 1f;
        protected virtual float HealthScaler => MassScaler;
        protected string texture = "";
        protected float originalMass = 0;

        protected Vector3[] originalVertices;
        private bool spawned = false;

        protected Dictionary<Face, bool> faces = new Dictionary<Face, bool>() {
            {Face.Top, true },
            {Face.Bottom, true },
            {Face.Left, true },
            {Face.Right, true },
            {Face.Front, true },
            {Face.Back, true }
        };

        internal bool inverted = false;

        static ModuleProcedural()
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var t = typeof(TankBlock);
            FilledCellsGravityScaleFactors = t.GetField("FilledCellsGravityScaleFactors", flags);
            m_LastFilledCellsGravityScaleFactors = t.GetField("m_LastFilledCellsGravityScaleFactors", flags);
            ConnectedBlocksByAP = t.GetProperty("ConnectedBlocksByAP");
            m_BlockCellBounds = t.GetField("m_BlockCellBounds", flags);//.First(f => f.Name.Contains("m_BlockCellBounds"));
            CalculateDefaultPhysicsConstants = t.GetMethod("CalculateDefaultPhysicsConstants", flags);
            InitAPFilledCells = t.GetMethod("InitAPFilledCells", flags);
            m_DefaultInertiaTensor = t.GetField("m_DefaultInertiaTensor", flags);
            m_PopulateTechBuffer = typeof(ManSpawn).GetField("m_PopulateTechBuffer", flags);

            t = m_PopulateTechBuffer.FieldType;
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
                m_LastFilledCellsGravityScaleFactors.SetValue(base.block, g);
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
                if (!faces[Face.Front]) value.RemoveAll(v => v.z == size.z - 0.5f);
                if (!faces[Face.Back]) value.RemoveAll(v => v.z == -0.5f);
                if (!faces[Face.Right]) value.RemoveAll(v => v.x == size.x - 0.5f);
                if (!faces[Face.Left]) value.RemoveAll(v => v.x == -0.5f);

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
                if (base.block.IsAttached) return;

                value.x = Mathf.Clamp(value.x, 1, 64);
                value.y = Mathf.Clamp(value.y, 1, 64);
                value.z = Mathf.Clamp(value.z, 1, 64);

                if (value == size) return;

                size = value;
                GenerateCellsAPs();
                GenerateMesh();
                GenerateProperties();
                Cells = cells;
                APs = aps;

                CalculateDefaultPhysicsConstants.Invoke(base.block, null);
                m_DefaultInertiaTensor.SetValue(base.block, base.block.CurrentInertiaTensor);
                base.block.rbody.inertiaTensor = base.block.CurrentInertiaTensor;
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

        public Dictionary<Face, bool> Faces
        {
            get
            {
                return new Dictionary<Face, bool>(faces);
            }
            set
            {
                if (base.block.IsAttached) return;
                var changed = false;
                foreach (var kv in value)
                {
                    if(faces[kv.Key] != kv.Value)
                    {
                        faces[kv.Key] = kv.Value;
                        changed = true;
                    }
                }

                if (changed)
                {
                    GenerateCellsAPs();
                    GenerateMesh();
                    APs = aps;
                }
            }
        }

        public void EnableFace(Face face, bool enabled)
        {
            var old = faces[face];
            if(old != enabled)
            {
                faces[face] = enabled;
                GenerateMesh();
            }
        }

        public virtual void BeforeBlockAdded(IntVector3 localPos)
        {
            if (spawned || !base.block) return;
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
                    
                    if (base.block == bblock)
                    { 
                        this.faces = data.faces ?? this.faces;
                        this.Size = data.size;
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
                                              
                        if (x == 0 && faces[Face.Left])
                        {
                            aps.Add(new Vector3(-0.5f, y, z));
                        }
                        if (x == size.x - 1 && faces[Face.Right])
                        {
                            aps.Add(new Vector3(x + 0.5f, y, z));
                        }
                        if (y == 0 && faces[Face.Bottom])
                        {
                            aps.Add(new Vector3(x, -0.5f, z));
                        }
                        if (y == size.y - 1 && faces[Face.Top])
                        {
                            aps.Add(new Vector3(x, y + 0.5f, z));
                        }
                        if (z == 0 && faces[Face.Back])
                        {
                            aps.Add(new Vector3(x, y, -0.5f));
                        }
                        if (z == size.z - 1 && faces[Face.Front])
                        {
                            aps.Add(new Vector3(x, y, z + 0.5f));
                        }                      
                    }
                }
            }
        }

        protected virtual void GenerateMesh()
        {
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
            try
            {
                var mass = originalMass * this.size.x * this.size.y * this.size.z * this.MassScaler;
                base.block.ChangeMass(mass);
                base.block.m_DefaultMass = mass;
                base.block.rbody.mass = mass;

                var healthScale = this.size.x * this.size.y * this.size.z * this.HealthScaler;

                var maxHealth = originalMaxHealth * healthScale;
                base.block.damage.maxHealth = (int)maxHealth;

                var damageable = base.block.visible.damageable;
                var healed = damageable.IsAtFullHealth;
                damageable.SetMaxHealth(maxHealth);
                if (healed) damageable.InitHealth(-1337f);
            }
            catch { }
        }

        private void Start()
        {
            meshRenderer = base.block.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(mr => mr.material.name.Contains("ProceduralMaterial"));
            meshFilter = base.block.GetComponentsInChildren<MeshFilter>().FirstOrDefault(mf => mf.sharedMesh.name == "ProceduralMesh");
            meshCollider = base.block.GetComponentsInChildren<MeshCollider>().FirstOrDefault(mf => mf.sharedMesh.name == "ProceduralMesh");
        }

        private void OnSpawn()
        {
            if (originalMaxHealth == 0f) originalMaxHealth = base.block.damage.maxHealth;
            this.Texture = "";
        }

        protected virtual void OnPool()
        {
            if(originalMass == 0) originalMass = base.block.m_DefaultMass;
            base.block.serializeEvent.Subscribe(this.OnSerialize);
            base.block.serializeTextEvent.Subscribe(this.OnSerialize);
            GenerateMesh();
        }

        protected virtual void OnRecycle()
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
        public new class SerialData : Module.SerialData<ModuleProcedural.SerialData>
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
