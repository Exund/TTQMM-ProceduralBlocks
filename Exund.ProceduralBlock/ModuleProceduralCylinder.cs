using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.ProceduralBlocks
{
    public class ModuleProceduralCylinder : ModuleProcedural
    {
        protected override float MassScaler => (float)Math.PI;
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

                        if(y == 0 || y == size.y - 1)
                        {
                            if(ProceduralBlocksMod.PointInEllipse(x + 0.5f, z + 0.5f, size.x * 0.5f, size.z * 0.5f))
                            {
                                if(y == 0) aps.Add(new Vector3(x, -0.5f, z));
                                if(y == size.y - 1) aps.Add(new Vector3(x, y + 0.5f, z));
                            }
                        }
                    }
                }
            }
        }
    }
}