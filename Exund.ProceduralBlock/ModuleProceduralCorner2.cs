using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.ProceduralBlocks
{
    public class ModuleProceduralCorner2 : ModuleProcedural
    {
        protected override float MassScaler => !inverted ? 1f/3f : 2f/3f;
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
                        if (y == 0)
                        {
                            aps.Add(new Vector3(x, -0.5f, z));
                        }
                        if (!inverted)
                        {
                            if (x == 0)
                            {
                                var p = new Vector2(y, z);
                                var p0 = Vector2.zero;
                                var p1 = new Vector2(size.y, 0);
                                var p2 = new Vector2(0, size.z);
                                var s = Math.Min(size.y, size.z);
                                if ((y + z + 1) <= s)
                                //if(ProceduralBlocksMod.PointInTriangle(p,p0,p1,p2))
                                    aps.Add(new Vector3(-0.5f, y, z));
                            }
                            if (z == 0)
                            {
                                var p = new Vector2(x, y);
                                var p0 = Vector2.zero;
                                var p1 = new Vector2(size.x, 0);
                                var p2 = new Vector2(0, size.y);
                                var s = Math.Min(size.x, size.y);
                                if ((x + y + 1) <= s)
                                //if (ProceduralBlocksMod.PointInTriangle(p, p0, p1, p2))
                                    aps.Add(new Vector3(x, y, -0.5f));
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
                                var p = new Vector2(y, z);
                                var p0 = Vector2.zero;
                                var p1 = new Vector2(size.y, 0);
                                var p2 = new Vector2(0, size.z);
                                var s = Math.Min(size.y, size.z);
                                if ((y + z + 1) <= s)
                                //if (ProceduralBlocksMod.PointInTriangle(p, p0, p1, p2))
                                    aps.Add(new Vector3(x + 0.5f, y, z));
                            }
                            /*if (y == size.y - 1)
                            {
                                var s = Math.Min(size.x, size.z);
                                if ((x + z + 1) <= s)
                                    aps.Add(new Vector3(x, y + 0.5f, z));
                            }*/
                            if (z == size.z - 1)
                            {
                                var p = new Vector2(x, y);
                                var p0 = Vector2.zero;
                                var p1 = new Vector2(size.x, 0);
                                var p2 = new Vector2(0, size.y);
                                var s = Math.Min(size.x, size.y);
                                if ((x + y + 1) <= s)
                                //if (ProceduralBlocksMod.PointInTriangle(p, p0, p1, p2))
                                    aps.Add(new Vector3(x, y, z + 0.5f));
                            }
                        }
                    }
                }
            }
        }
    }
}