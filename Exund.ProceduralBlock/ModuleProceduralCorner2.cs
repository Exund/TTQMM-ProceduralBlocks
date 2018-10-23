using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.ProceduralBlocks
{
    class ModuleProceduralCorner2 : ModuleProcedural
    {
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

                        if (x == 0)
                        {
                            aps.Add(new Vector3(-0.5f, y, z));
                        }
                        if (y == 0)
                        {
                            var s = Math.Min(size.x, size.z);
                            if ((x + z + 1) <= s)
                                aps.Add(new Vector3(x, -0.5f, z));
                        }
                        if (z == 0)
                        {
                            var s = Math.Min(size.x, size.y);
                            if ((x + y + 1) <= s)
                                aps.Add(new Vector3(x, y, -0.5f));
                        }
                    }
                }
            }
        }
    }
}