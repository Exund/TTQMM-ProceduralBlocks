using System;
using UnityEngine;

namespace Exund.ColorBlock
{
    public class ColorTools : MonoBehaviour
    {
        private int colorID = 7786;
        private int toolsID = 7788;

        private Rect win;
        private Rect win2 = new Rect(0, 100f, 300f, 350f);

        private ModuleColor module;
        private Color blockColor;

        private bool toolsVisible = false;
        private bool selectingColor = false;
        private Color color = Color.white;
        private Texture2D preview = new Texture2D(50, 50);
        private int selection = 0;
        private string[] axis = new string[] { "X", "Y", "Z" };
        private int radius = 1;
        private Rect selectInfo = new Rect(Screen.width / 2 - 50f, Screen.height / 6, 100f, 25f);
        private bool changeKey = false;
        private KeyCode key = KeyCode.Keypad1;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0)) toolsVisible = !toolsVisible;
            if (!toolsVisible) selectingColor = false;

            if (Input.GetMouseButtonDown(0) || Input.GetKey(key))
            {
                try
                {
                    var block = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    IntVector3 blockPosition = block.cachedLocalPosition;
                    var mod = block.GetComponent<ModuleColor>();
                    if (selectingColor)
                    {
                        color = mod.Color;
                        selectingColor = false;
                    } else if (toolsVisible)
                    {
                        if (axis[selection] == "X")
                        {
                            IntVector3 start = blockPosition - new IntVector3(0, radius - 2, radius - 2);
                            IntVector3 end = blockPosition + new IntVector3(0, radius - 1, radius - 1);
                            for (int y = start.y; y < end.y; y++)
                            {
                                for (int z = start.z; z < end.z; z++)
                                {
                                    var current = block.tank.blockman.GetBlockAtPosition(new IntVector3(blockPosition.x, y, z));
                                    var modc = current?.GetComponent<ModuleColor>();
                                    if (modc) modc.Color = color;
                                }
                            }
                        }
                        else if (axis[selection] == "Y")
                        {
                            IntVector3 start = blockPosition - new IntVector3(radius - 2, 0, radius - 2);
                            IntVector3 end = blockPosition + new IntVector3(radius - 1, 0, radius - 1);
                            for (int x = start.x; x < end.x; x++)
                            {
                                for (int z = start.z; z < end.z; z++)
                                {
                                    var current = block.tank.blockman.GetBlockAtPosition(new IntVector3(x, blockPosition.y, z));
                                    var modc = current?.GetComponent<ModuleColor>();
                                    if (modc) modc.Color = color;
                                }
                            }
                        }
                        else if (axis[selection] == "Z")
                        {
                            IntVector3 start = blockPosition - new IntVector3(radius - 2, radius - 2, 0);
                            IntVector3 end = blockPosition + new IntVector3(radius - 1, radius - 1, 0);
                            for (int x = start.x; x < end.x; x++)
                            {
                                for (int y = start.y; y < end.y; y++)
                                {
                                    var current = block.tank.blockman.GetBlockAtPosition(new IntVector3(x, y, blockPosition.z));
                                    var modc = current?.GetComponent<ModuleColor>();
                                    if (modc) modc.Color = color;
                                }
                            }
                        }
                    }
                } catch(Exception e) { }
            }

            if (!Singleton.Manager<ManPointer>.inst.DraggingItem)
            {
                if(Input.GetMouseButtonDown(1))
                {
                    win = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - 200f, 200f, 200f);
                    try
                    {
                        module = Singleton.Manager<ManPointer>.inst.targetVisible.block.GetComponent<ModuleColor>();
                        blockColor = module.Color;
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e);
                        module = null;
                    }
                }
            }
        }

        private void OnGUI()
        {
            if(module)
            {
                try
                {
                    win = GUI.Window(colorID, win, new GUI.WindowFunction(ColorWindow), "Block Color");
                    module.Color = blockColor;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            if(toolsVisible)
            {
                try
                {
                    win2 = GUI.Window(toolsID, win2, new GUI.WindowFunction(ToolsWindow), "Color Tools");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            if(selectingColor)
            {
                GUI.Label(selectInfo, "Click on a color block to select its color", new GUIStyle() { alignment = TextAnchor.MiddleCenter });
            }
        }
        
        private void ColorWindow(int id)
        {
            blockColor = ProceduralBlocks.ProceduralBlocksMod.ColorField(blockColor);

            if(GUILayout.Button("Close"))
            {
                module = null;
            }
            GUI.DragWindow();
        }

        private void ToolsWindow(int id)
        {
            Color[] cs = new Color[preview.width * preview.height];
            for (int i = 0; i < cs.Length; i++) cs[i] = color;
            preview.SetPixels(cs);
            preview.Apply();

            if (changeKey)
            {
                Event current = Event.current;
                if (current.isKey)
                {
                    key = current.keyCode;
                    changeKey = false;
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Brush Color");
            GUILayout.Label(preview);
            GUILayout.EndVertical();
            color = ProceduralBlocks.ProceduralBlocksMod.ColorField(color);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Pick color from block")) selectingColor = true;

            GUILayout.Label("Brush Radius");
            if(int.TryParse(GUILayout.TextField((radius - 1).ToString()), out int o)) radius = o+1;
            if (radius < 2) radius = 2;

            GUILayout.Label("Fixed Axis");
            selection = GUILayout.SelectionGrid(selection, axis, 3);

            GUILayout.Label("Paint Key");
            if(GUILayout.Button(changeKey ? "Press a key" : key.ToString())) changeKey = true;

            if (GUILayout.Button("Close")) toolsVisible = false;
            GUI.DragWindow();
        }
    }
}
