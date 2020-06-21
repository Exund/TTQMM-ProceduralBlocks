using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using System.Reflection;

namespace Exund.ProceduralBlocks
{
    class ProceduralEditor : MonoBehaviour
    {
        private int ID = 7787;
        private bool visible = false;
        private Rect win;

        private ModuleProcedural module;
        private int x, y, z;
		private bool hasColor = false;

		private string[] textures = new string[0];
		private Vector2 scrollPos;
        private Vector2 scrollPos2;

        private float height = 500f;

		private void Update()
        {
            if (!Singleton.Manager<ManPointer>.inst.DraggingItem && Input.GetMouseButtonDown(1))
            {
                win = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - height, 400f, height);

				hasColor = false;
                try
                {
                    var b = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    module = b.GetComponent<ModuleProcedural>();
					hasColor = b.GetComponent<ColorBlock.ModuleColor>();
                    x = module.Size.x;
                    y = module.Size.y;
                    z = module.Size.z;
                }
                catch (Exception e)
                {
                    module = null;
                }
                visible = module;
				if(visible)
				{
					textures = (new string[] { "None" }).Concat(Directory.GetFiles(Path.Combine(ProceduralBlocksMod.AssetsFolder, "Textures"), "*.png")).Select(p => Path.GetFileName(p)).ToArray();
				}
            }
        }

        private void OnGUI()
        {
            if (!visible || !module) return;

            try
            {
                win.y += height;
                height = 0f;
                if (!module.block.IsAttached)
                {
                    height += GUI.skin.label.CalcSize(new GUIContent("X")).y * 3;
                    height += GUI.skin.textField.CalcSize(new GUIContent("0")).y * 3;
                    height += GUI.skin.toggle.CalcSize(new GUIContent("T")).y * module.Faces.Count;
                }
                height += GUI.skin.label.CalcSize(new GUIContent(module.Texture)).y;
                height += Math.Min(200f, GUI.skin.button.CalcSize(new GUIContent("G")).y * textures.Length);
                height += GUI.skin.button.CalcSize(new GUIContent("Close")).y;
                if(module is ModuleProceduralSlicedMesh)
                {
                    height += GUI.skin.label.CalcSize(new GUIContent("A")).y;
                    height += Math.Min(200f, GUI.skin.button.CalcSize(new GUIContent("G")).y * ModuleProceduralSlicedMesh.SlicedMeshes.Count);
                } 

                win.y -= height;
                win.height = height;
                win = GUI.Window(ID, win, DoWindow, "Procedural Editor");
				module.Size = new IntVector3(x, y, z);  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DoWindow(int id)
        {
			if (!module.block.IsAttached)
			{
				GUILayout.Label("X");
				int.TryParse(GUILayout.TextField(x.ToString()), out x);
                x = Math.Min(64, x);
				GUILayout.Label("Y");
				int.TryParse(GUILayout.TextField(y.ToString()), out y);
                y = Math.Min(64, y);
                GUILayout.Label("Z");
				int.TryParse(GUILayout.TextField(z.ToString()), out z);
                z = Math.Min(64, z);

                var faces = module.Faces;
                foreach(var kv in module.Faces)
				{
					faces[kv.Key] = GUILayout.Toggle(kv.Value, kv.Key.ToString());
				}
                module.Faces = faces;
			}

			GUILayout.Label(module.Texture);
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(200f)); 
			foreach (var texture in textures)
			{
				if (GUILayout.Button(texture, new GUIStyle(GUI.skin.button) { richText = true, alignment = TextAnchor.MiddleLeft }))
				{
					module.Texture = texture;

					if(hasColor)
					{
						var c = module.block.GetComponent<ColorBlock.ModuleColor>();
						c.Color = Color.white;
						c.active = module.Texture == "";
					}
				}
			}   
			GUILayout.EndScrollView();

            if(module is ModuleProceduralSlicedMesh mpsm)
            {
                GUILayout.Label(mpsm.SlicedMeshName);
                scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.MaxHeight(200f));
                foreach (var k in ModuleProceduralSlicedMesh.SlicedMeshes.Keys)
                {
                    if (GUILayout.Button(k, new GUIStyle(GUI.skin.button) { richText = true, alignment = TextAnchor.MiddleLeft }))
                    {
                        mpsm.SlicedMeshName = k;
                    }
                }
                GUILayout.EndScrollView();
            }
            
            if (GUILayout.Button("Close"))
            {
                visible = false;
                module = null;
            }
            GUI.DragWindow();
        }
    }
}
