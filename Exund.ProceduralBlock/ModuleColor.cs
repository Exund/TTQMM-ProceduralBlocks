using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace Exund.ColorBlock
{
    public class ModuleColor : Module
    {
        //private static FieldInfo MatRendererLookup;
        private Color color = Color.white;
        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                if (value == this.color) return;
                //if(MatRendererLookup == null) MatRendererLookup = typeof(TankBlock).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField).FirstOrDefault(f => f.Name.Contains("MatRendererLookup"));
                this.color = value;
                try
                {
                    /*var matRenderLookup = (Dictionary<Material, List<Renderer>>)MatRendererLookup.GetValue(base.block);
                    foreach (var renderers in matRenderLookup.Values)
                    {
                        foreach (var renderer in renderers)
                        {
                            var t1 = new Texture2D(1, 1);
                            t1.SetPixel(0, 0, this.color);
                            renderer.material.mainTexture = t1;
                            renderer.material.color = this.color;
                        }
                    }*/
                    var t = new Texture2D(1, 1);
                    t.SetPixel(0, 0, this.color);
                    base.block.GetComponentInChildren<MeshRenderer>().material.mainTexture = t;
                    base.block.GetComponentInChildren<MeshRenderer>().material.color = this.color;
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void OnSpawn()
        {
            this.Color = Color.white;
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
                ModuleColor.SerialData serialData = new ModuleColor.SerialData()
                {
                    color = string.Format("{0},{1},{2}", this.color.r, this.color.g, this.color.b)
                };
                serialData.Store(blockSpec.saveState);
            }
            else
            {
                ModuleColor.SerialData serialData2 = Module.SerialData<ModuleColor.SerialData>.Retrieve(blockSpec.saveState);
                if (serialData2 != null)
                {
                    var c = serialData2.color.Split(',');
                    this.Color = new Color(float.Parse(c[0]), float.Parse(c[1]), float.Parse(c[2]));
                }
            }
        }

        [Serializable]
        private new class SerialData : Module.SerialData<ModuleColor.SerialData>
        {
            public string color;
        }
    }
}
