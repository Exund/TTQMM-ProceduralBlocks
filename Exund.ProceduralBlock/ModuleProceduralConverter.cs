using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.ProceduralBlocks
{
    class ModuleProceduralConverter : ModuleProcedural
    {
        public float sox = 0;
        public float soz = 0;
        public float ssx = 0.5f;
        public float ssz = 0.5f;
        public float rox = 0;
        public float roz = 0;
        protected override float MassScaler => (float)Math.PI;
        protected override void GenerateCellsAPs()
        {
            cells = new List<IntVector3>();
            aps = new List<Vector3>();
            //var vc = 0.5f * ((Vector3)size - Vector3.one);
            var center = ((Vector3)size - Vector3.one) * 0.5f;
            var rx = (size.x + sox) * ssx + rox;
            var rz = (size.z + soz) * ssz + roz;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        cells.Add(new IntVector3(x, y, z));

                        if (y == 0)
                        {
                            if (Math.Pow(x - center.x, 2) / Math.Pow(rx, 2) + Math.Pow(z - center.z, 2) / Math.Pow(rz, 2) <= 1)
                            {
                                aps.Add(new Vector3(x, -0.5f, z));
                            }
                        }

                        //if(size.x < 3 || size.z < 4)
                        //{
                        //    if(y == 0) aps.Add(new Vector3(x, -0.5f, z));
                        //    if(y == size.y - 1) aps.Add(new Vector3(x, y + 0.5f, z));
                        //    continue;
                        //}
                        //var lu = (float)Math.Pow((double)x - vc.x, 2);
                        //var ru = (float)Math.Pow((double)z - vc.z, 2);
                        //var ld = 0f;
                        //var rd = 0f;
                        ///*if(size.x > size.z)
                        //{*/
                        //    ld = (float)Math.Pow((double)vc.x, 2);
                        //    rd = (float)Math.Pow((double)vc.z, 2);
                        ///*}
                        //else
                        //{
                        //    ld = (float)Math.Pow((double)vc.z, 2);
                        //    rd = (float)Math.Pow((double)vc.x, 2);
                        //}*/
                        //if (y == 0)
                        //{
                        //    if((lu / ld + ru / rd) <= 1.5f)
                        //        aps.Add(new Vector3(x, -0.5f, z));
                        //}
                        if (y == size.y - 1)
                        {
                            aps.Add(new Vector3(x, y + 0.5f, z));
                        }
                    }
                }
            }
        }
    }
}