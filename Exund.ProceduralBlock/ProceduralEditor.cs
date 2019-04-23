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

		private void Update()
        {
            if (!Singleton.Manager<ManPointer>.inst.DraggingItem && Input.GetMouseButtonDown(1))
            {
                win = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - 500f, 400f, 500f);
				hasColor = false;
                try
                {
                    var b = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    module = b.GetComponent<ModuleProcedural>();
					hasColor = b.GetComponent<Exund.ColorBlock.ModuleColor>();
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
                win = GUI.Window(ID, win, new GUI.WindowFunction(DoWindow), "Procedural Editor");
				if(x * y * z < 256) module.Size = new IntVector3(x, y, z);
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
				GUILayout.Label("Y");
				int.TryParse(GUILayout.TextField(y.ToString()), out y);
				GUILayout.Label("Z");
				int.TryParse(GUILayout.TextField(z.ToString()), out z);

				for (int i = 0; i < module.faces.Count; i++)
				{
					var face = module.faces.Keys.ElementAt(i);
					module.faces[face] = GUILayout.Toggle(module.faces[face], face.ToString());
				}
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
						var c = module.block.GetComponent<Exund.ColorBlock.ModuleColor>();
						c.Color = Color.white;
						c.active = module.Texture == "";
					}
				}
			}
			GUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
            {
                visible = false;
                module = null;
            }
            GUI.DragWindow();
        }
    }
}
