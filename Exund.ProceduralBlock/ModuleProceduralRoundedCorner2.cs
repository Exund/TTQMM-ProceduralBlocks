using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.ProceduralBlocks
{
    public class ModuleProceduralRoundedCorner2 : ModuleProcedural
    {
        protected override float MassScaler => !inverted ? 1f/3f : 2f/3f;
        protected override void GenerateCellsAPs()
        {
            cells = new List<IntVector3>();
            aps = new List<Vector3>();

            var center = -Vector3.one * 0.5f;//(Vector3)size+ Vector3.one;// *0.5f;
            var rx = size.x;
            var ry = size.y;
            var rz = size.z;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        cells.Add(new IntVector3(x, y, z));
                        if (y == 0)
                        {
                            aps.Add(new Vector3(x, -0.5f, z));
                        }
                        if (!inverted)
                        {
                            if (x == 0)
                            {
                                if (Math.Pow(z - center.z, 2) / Math.Pow(rz, 2) + Math.Pow(y - center.y, 2) / Math.Pow(ry, 2) <= 1)
                                {
                                    aps.Add(new Vector3(-0.5f, y, z));
                                }
                            }
                            if (z == 0)
                            {
                                if (Math.Pow(y - center.y, 2) / Math.Pow(ry, 2) + Math.Pow(x - center.x, 2) / Math.Pow(rx, 2) <= 1)
                                {
                                    aps.Add(new Vector3(x, y, -0.5f));
                                }
                            }
                        }
                        else
                        {
                            if (x == 0)
                            {
                                aps.Add(new Vector3(-0.5f, y, z));
                            }
                            if (z == 0)
                            {
                                aps.Add(new Vector3(x, y, -0.5f));
                            }

                            if (x == size.x - 1)
                            {
                                if (Math.Pow(z - center.z, 2) / Math.Pow(rz, 2) + Math.Pow(y - center.y, 2) / Math.Pow(ry, 2) <= 1)
                                {
                                    aps.Add(new Vector3(x + 0.5f, y, z));
                                }
                            }
                            if (z == size.z - 1)
                            {
                                if (Math.Pow(y - center.y, 2) / Math.Pow(ry, 2) + Math.Pow(x - center.x, 2) / Math.Pow(rx, 2) <= 1)
                                {
                                    aps.Add(new Vector3(x, y, z + 0.5f));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}