using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.ProceduralBlocks
{
    class ProceduralEditor : MonoBehaviour
    {
        private int ID = 7787;
        private bool visible = false;
        private Rect win;

        private ModuleProcedural module;
        private bool isAttached = true;
        private int x, y, z;

        private void Update()
        {
            if (!Singleton.Manager<ManPointer>.inst.DraggingItem && Input.GetMouseButtonDown(1))
            {
                win = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - 200f, 200f, 200f);
                try
                {
                    var b = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    isAttached = b.IsAttached;
                    module = b.GetComponent<ModuleProcedural>();
                    x = module.Size.x;
                    y = module.Size.y;
                    z = module.Size.z;
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                    module = null;
                }
                visible = module;
            }
        }

        private void OnGUI()
        {
            if (!visible || !module || isAttached) return;

            try
            {
                win = GUI.Window(ID, win, new GUI.WindowFunction(DoWindow), "Procedural Editor");
                module.Size = new IntVector3(x, y, z);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DoWindow(int id)
        {
            GUILayout.Label("X");
            int.TryParse(GUILayout.TextField(x.ToString()), out x);
            GUILayout.Label("Y");
            int.TryParse(GUILayout.TextField(y.ToString()), out y);
            GUILayout.Label("Z");
            int.TryParse(GUILayout.TextField(z.ToString()), out z);
            if (GUILayout.Button("Close"))
            {
                visible = false;
                module = null;
            }
            GUI.DragWindow();
        }
    }
}
