using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace Exund.ColorBlock
{
    public class ImageToTech : MonoBehaviour
    {
        private int ID = 7785;
        private bool visible = false;
        private Rect win = new Rect(10f, 10f, 300f, 500f);
        private string path = "";
        private Vector2 scrollPos = Vector2.zero;

        private bool useFlesh = true;
        private bool useModBlock = true;

        private List<Temp> blocks = new List<Temp>();
        private Tank tech;

        private Dictionary<Color, BlockTypes> color_blocks = new Dictionary<Color, BlockTypes>
        {
            { Color.black, BlockTypes.SPEColourBlock01_Black_111 },
            { new Color32(0x17,0x17,0x17,0xFF), BlockTypes.SPEColourBlock02_Grey_111 },
            { Color.white, BlockTypes.SPEColourBlock03_White_111 },
            { new Color32(0xD3,0x2C,0x2B,0xFF), BlockTypes.SPEColourBlock14_Red_111 },
            { new Color32(0x2B,0xD1,0x2B,0xFF), BlockTypes.SPEColourBlock09_Green111 },
            { new Color32(0x2B,0x2C,0xD3,0xFF), BlockTypes.SPEColourBlock05_DarkBlue_111 },
            { new Color32(0xD3,0x2C,0xD0,0xFF), BlockTypes.SPEColourBlock16_Pink_111 },
            { new Color32(0xD0,0xD1,0x2B,0xFF), BlockTypes.SPEColourBlock11_Yellow_111 },
            { new Color32(0x2B,0xD1,0xD3,0xFF), BlockTypes.SPEColourBlock07_LightBlue_111 },
            { new Color32(0x7E,0x2C,0xD0,0xFF), BlockTypes.SPEColourBlock04_Purple_111 },
            { new Color32(0xD0,0x2C,0x7E,0xFF), BlockTypes.SPEColourBlock15_Magenta_111 },
            { new Color32(0xD0,0x7E,0x2B,0xFF), BlockTypes.SPEColourBlock12_Orange_111 },
            { new Color32(0x2B,0x7E,0xD3,0xFF), BlockTypes.SPEColourBlock06_Blue_111 },
            { new Color32(0x2B,0xD1,0x7E,0xFF), BlockTypes.SPEColourBlock08_LightGreen_111 },
            { new Color32(0x7E,0xD1,0x2B,0xFF), BlockTypes.SPEColourBlock10_YellowGreen_111 },
            { new Color32(0x68,0x3F,0x15,0xFF), BlockTypes.SPEColourBlock13_Brown_111 },
            { new Color32(0x26,0x69,0x1E,0xFF), BlockTypes.SPEColourBlock17_Camping_Green_111 },
            { new Color32(0x5A,0x0A,0xB5,0xFF), BlockTypes.SPEColourBlock18_Indigo_111 },
            { new Color32(0xF1,0xA0,0xAD,0xFF), BlockTypes.SPEColourBlock19_Light_Pink_111 },
            { new Color32(0xF9,0xD8,0xC6,0xFF), BlockTypes.SPEColourBlock20_Flesh_01_111 },
            { new Color32(0xD8,0xB0,0x94,0xFF), BlockTypes.SPEColourBlock21_Flesh_02_111 },
            { new Color32(0xD6,0xA6,0x73,0xFF), BlockTypes.SPEColourBlock22_Flesh_03_111 },
            { new Color32(0x8C,0x4B,0x26,0xFF), BlockTypes.SPEColourBlock23_Flesh_04_111 },
            { new Color32(0x44,0x22,0x0A,0xFF), BlockTypes.SPEColourBlock24_Flesh_05_111 }
        };

        

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(ProceduralBlocks.ProceduralBlocksMod.img2TechKeycode))// 
            {
                visible = true;
            }
        }

        private void OnGUI()
        {
            if (!visible) return;

            try
            {
                win = GUI.Window(ID, win, new GUI.WindowFunction(DoWindow), "TechArts");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DoWindow(int id)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var image in Directory.GetFiles(Path.GetFullPath(ProceduralBlocks.ProceduralBlocksMod.TechArtFolder), "*.png"))
            {
                if (GUILayout.Button(Path.GetFileNameWithoutExtension(image), new GUIStyle(GUI.skin.button) { richText = true, alignment = TextAnchor.MiddleLeft }))
                {
                    path = image;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.TextField(path);
           
            useModBlock = GUILayout.Toggle(useModBlock, "Use mod block");
            if(!useModBlock) useFlesh = GUILayout.Toggle(useFlesh, "Use flesh blocks");
            if (GUILayout.Button("Load"))
            {
                try
                {
                    Texture2D image = new Texture2D(0, 0);
                    image.LoadImage(File.ReadAllBytes(path));

                    if (image.width <= 64 && image.height <= 64)
                    {
                        blocks = new List<Temp>();
                        Vector3 position = Singleton.playerTank.trans.position;
                        Quaternion rotation = Singleton.playerTank.trans.rotation;

                        TankBlock root = Singleton.Manager<ManSpawn>.inst.SpawnBlock(BlockTypes.GSOCockpit_111, Vector3.zero, rotation);
                        root.trans.localPosition = Vector3.zero;

                        var spawnParams = new ManSpawn.TankSpawnParams()
                        {
                            forceSpawn = true,
                            grounded = true,
                            position = position,
                            rotation = rotation,
                            placement = ManSpawn.TankSpawnParams.Placement.PlacedAtPosition,
                            techData = new TechData()
                            {
                                Name = Path.GetFileNameWithoutExtension(path),
                                m_CreationData = new TechData.CreationData(),
                                m_BlockSpecs = new List<TankPreset.BlockSpec>()
                            }
                        };

						var spec = new TankPreset.BlockSpec();
						spec.InitFromBlockState(root, false);

						spawnParams.techData.m_BlockSpecs.Add(spec);
                        
                        for (int x = 0; x < image.width; x++)
                        {
                            for (int y = 0; y < image.height; y++)
                            {
                                if (x == 0 && y == 0) continue; 
                                var c = image.GetPixel(x, y);
                                var type = (BlockTypes)7000;
                                if (!useModBlock)
                                {
                                    var sw = float.MaxValue;
                                    var cc = Color.white;
                                    foreach (var color in color_blocks.Keys.ToList())
                                    {
                                        if (!useFlesh && color_blocks[color].ToString().Contains("Flesh")) continue;
                                        var w = (float)Math.Sqrt(Math.Pow(c.r - color.r, 2) + Math.Pow(c.g - color.g, 2) + Math.Pow(c.b - color.b, 2));
                                        if (w < sw)
                                        {
                                            sw = w;
                                            cc = color;
                                        }
                                    }
                                    type = color_blocks[cc];
                                }
                                
                                TankBlock b = Singleton.Manager<ManSpawn>.inst.SpawnBlock(type, new Vector3(x,y,0), rotation);
                                b.trans.localPosition = new Vector3(x, y, 0);
                                if (useModBlock) b.GetComponent<ModuleColor>().Color = c;
								spec = new TankPreset.BlockSpec();
								spec.InitFromBlockState(b, false);
                                spawnParams.techData.m_BlockSpecs.Add(spec);
                                b.visible.RemoveFromGame();
                            }
                        }
                        tech = ManSpawn.inst.SpawnUnmanagedTank(spawnParams);
                        Tank playerTank = Singleton.playerTank;
                        Singleton.Manager<ManTechs>.inst.SetPlayerTankLocally(null, true);
                        playerTank.visible.RemoveFromGame();

                        Singleton.Manager<ManTechs>.inst.SetPlayerTankLocally(tech, true);
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
                visible = false;
            }
            if (GUILayout.Button("Cancel")) visible = false;
            GUI.DragWindow();
        }

        private struct Temp
        {
            public BlockTypes type;
            public IntVector3 position;
        }
    } 
}
