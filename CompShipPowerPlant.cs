using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace Stellaris
{
    public class CompShipPowerPlant : CompPowerPlant
    {
        public ShipMapComp shipMapComp;
        public CompProperties_Power Props { get => (CompProperties_Power)this.props; }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            shipMapComp = (ShipMapComp)parent.Map.components.Find(x => x is ShipMapComp);
            Register();
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            Unregister();
            base.PostDeSpawn(map, mode);
        }

        public void Register()
        {
            if (this.parent.HasComp<CompShipControl>())
            {
                shipMapComp.consoles.Add(parent.GetComp<CompShipControl>());
            }
            else if(this.parent.HasComp<CompShipThruster>())
            {
                shipMapComp.thrusters.Add(parent.GetComp<CompShipThruster>());
            }
            else if (parent.HasComp<CompRefuelable>())
            {
                shipMapComp.fuelTanks.Add(parent.GetComp<CompRefuelable>());
            }
            shipMapComp.shipPowerPlants.Add(this);
        }
        public void Unregister()
        {
            if (this.parent.HasComp<CompShipControl>())
            {
                shipMapComp.consoles.Remove(parent.GetComp<CompShipControl>());
            }
            else if (this.parent.HasComp<CompShipThruster>())
            {
                shipMapComp.thrusters.Remove(parent.GetComp<CompShipThruster>());
            }
            else if (parent.HasComp<CompRefuelable>())
            {
                shipMapComp.fuelTanks.Remove(parent.GetComp<CompRefuelable>());
            }
            shipMapComp.shipPowerPlants.Remove(this);
        }
    }
}
