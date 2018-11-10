using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.ProceduralBlocks
{
    class ModuleProceduralCylinder : ModuleProcedural
    {
        protected override float MassScaler => (float)Math.PI;
        protected override void GenerateCellsAPs()
        {
            cells = new List<IntVector3>();
            aps = new List<Vector3>();
            var vc = 0.5f * ((Vector3)size - Vector3.one);
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        cells.Add(new IntVector3(x, y, z));

                        if(size.x < 3 || size.z < 4)
                        {
                            if(y == 0) aps.Add(new Vector3(x, -0.5f, z));
                            if(y == size.y - 1) aps.Add(new Vector3(x, y + 0.5f, z));
                            continue;
                        }
                        var lu = (float)Math.Pow((double)x - vc.x, 2);
                        var ru = (float)Math.Pow((double)z - vc.z, 2);
                        var ld = 0f;
                        var rd = 0f;
                        /*if(size.x > size.z)
                        {*/
                            ld = (float)Math.Pow((double)vc.x, 2);
                            rd = (float)Math.Pow((double)vc.z, 2);
                        /*}
                        else
                        {
                            ld = (float)Math.Pow((double)vc.z, 2);
                            rd = (float)Math.Pow((double)vc.x, 2);
                        }*/
                        if (y == 0)
                        {
                            if((lu / ld + ru / rd) <= 1.5f)
                                aps.Add(new Vector3(x, -0.5f, z));
                        }
                        if (y == size.y - 1)
                        {
                            if ((lu / ld + ru / rd) <= 1.5f)
                                aps.Add(new Vector3(x, y + 0.5f, z));
                        }
                    }
                }
            }
        }
    }
}