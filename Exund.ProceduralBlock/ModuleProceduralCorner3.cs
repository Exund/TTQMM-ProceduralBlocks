using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.ProceduralBlocks
{
    public class ModuleProceduralCorner3 : ModuleProcedural
    {
        protected override float MassScaler => !inverted ? 1f/6f : 5f/6f;
        protected override void GenerateCellsAPs()
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
                        if (!inverted)
                        {
                            if (x == 0)
                            {
                                if(ProceduralBlocksMod.PointInRectangleTriangle(y + 0.5f, z + 0.5f, size.y, size.z))
                                    aps.Add(new Vector3(-0.5f, y, z));
                            }
                            if (y == 0)
                            {
                                if (ProceduralBlocksMod.PointInRectangleTriangle(x + 0.5f, z + 0.5f, size.x, size.z))
                                    aps.Add(new Vector3(x, -0.5f, z));
                            }
                            if (z == 0)
                            {
                                if (ProceduralBlocksMod.PointInRectangleTriangle(x + 0.5f, y + 0.5f, size.x, size.y))
                                    aps.Add(new Vector3(x, y, -0.5f));
                            }
                        }
                        else
                        {
                            if (x == 0)
                            {
                                aps.Add(new Vector3(-0.5f, y, z));
                            }
                            if (y == 0)
                            {
                                aps.Add(new Vector3(x, -0.5f, z));
                            }
                            if (z == 0)
                            {
                                aps.Add(new Vector3(x, y, -0.5f));
                            }

                            if (x == size.x - 1)
                            {
                                if (ProceduralBlocksMod.PointInRectangleTriangle(y + 0.5f, z + 0.5f, size.y, size.z))
                                    aps.Add(new Vector3(x + 0.5f, y, z));
                            }
                            if (y == size.y - 1)
                            {
                                if (ProceduralBlocksMod.PointInRectangleTriangle(x + 0.5f, z + 0.5f, size.x, size.z))
                                    aps.Add(new Vector3(x, y + 0.5f, z));
                            }
                            if (z == size.z - 1)
                            {
                                if (ProceduralBlocksMod.PointInRectangleTriangle(x + 0.5f, y + 0.5f, size.x, size.y))
                                    aps.Add(new Vector3(x, y, z + 0.5f));
                            }
                        }
                    }
                }
            }
        }
    }
}