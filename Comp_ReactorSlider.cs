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
    public class Comp_ReactorSlider : ThingComp
    {
        public CompRefuelable fuel;
        public CompPowerDynamicPlant powerTrader;
        public CompProperties_ReactorSlider Props => (CompProperties_ReactorSlider)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            fuel = parent.GetComp<CompRefuelable>();
            powerTrader = parent.GetComp<CompPowerDynamicPlant>();
            UpdatePowerAndFuel();
        }

        public float SliderRate { get => sliderRate; set { sliderRate = value;UpdatePowerAndFuel(); } }

        private float sliderRate = 100.0f;
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var item in base.CompGetGizmosExtra())
            {
                yield return item;
            } 
            yield return new Command_Action()
            {
                defaultLabel = "StellarisReactorSliderLabel".Translate(),
                defaultDesc = "StellarisReactorSliderDesc".Translate(),
                action = delegate 
                {
                    Find.WindowStack.Add(new Dialog_Slider(x => "StellarisReactorCurrentRate".Translate() + x + @"%", 0, 300,
                        x => SliderRate = x, (int)SliderRate, 1f
                        ));
                },
                icon = ContentFinder<Texture2D>.Get("UI/Ship/StellarisReactorSlider"),
            };
        }

        public void UpdatePowerAndFuel()
        {
            powerTrader.PowerOutput = 100f * SliderRate;
            powerTrader.powerConsumption = -100f * SliderRate;
            fuel.Props.fuelConsumptionRate = 10f * (float)Math.Pow(SliderRate / 100f,1.5d); 
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref sliderRate, "sliderRate");
        }
    }
}
