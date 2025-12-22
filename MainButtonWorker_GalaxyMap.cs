using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    public class MainButtonWorker_GalaxyMap : MainButtonWorker
    {
        public override void Activate()
        {
            // 打开星系团视图
            Find.WindowStack.Add(new Window_GalaxyCluster());
        }
    }
}
