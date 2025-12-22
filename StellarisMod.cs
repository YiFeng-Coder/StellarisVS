using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
namespace Stellaris
{
    public class StellarisMod : Mod
    {
        public static StellarisMod Instance;
        public static GalaxyCluster GalaxyCluster;
        public StellarisMod(ModContentPack content) : base(content)
        {
            Instance = this;
            //Initialize();
        }

        private void Initialize()
        {
            // 注册游戏组件 
            Harmony harmony = new Harmony("rimworld.mod.stellaris");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            if (Current.Game.GetComponent<GalaxyComponent>() == null)
            {
                Log.Error("GalaxyComponent is null");
            }
        }
    }
}
