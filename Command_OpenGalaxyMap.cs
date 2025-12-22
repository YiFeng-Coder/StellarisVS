using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Stellaris
{
    public class Command_OpenGalaxyMap : Command
    {
        public Command_OpenGalaxyMap()
        {
            defaultLabel = "StellarisGalaxyMapLabel".Translate();
            defaultDesc = "StellarisGalaxyMapDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Icons/MainButtons/GalaxyMap");
            hotKey = KeyBindingDefOf.Misc1; // 或者使用自定义热键
        }
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            Find.WindowStack.Add(new Window_GalaxyCluster());
        }
    }
}
