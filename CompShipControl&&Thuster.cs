using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Stellaris
{
    // 舰船控制组件
    public class CompShipControl : ThingComp
    {
        public ShipMapComp shipMapComp;
        public bool hasDriver = false;  
        public CompProperties_ShipControl Props => (CompProperties_ShipControl)props;

        public CompShipPowerPlant powerTrader;

        private bool shipActive = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerTrader = parent.GetComp<CompShipPowerPlant>();
            shipMapComp = parent.Map.components.Find(x => x is ShipMapComp) as ShipMapComp;
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            foreach (var item in base.CompFloatMenuOptions(selPawn))
            {
                yield return item;
            }
            AcceptanceReport acceptanceReport = this.CanUseNow();
            if (!acceptanceReport.Accepted)
            {
                yield return new FloatMenuOption("CannotChooseNavigator".Translate() + ": " + acceptanceReport.Reason.CapitalizeFirst(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
            }
            else if (selPawn.skills == null || selPawn.skills.GetSkill(SkillDefOf.Intellectual).TotallyDisabled)
            {
                yield return new FloatMenuOption("CannotChooseNavigator".Translate() + ": " + "IncapableOfCapacity".Translate(SkillDefOf.Intellectual.label).CapitalizeFirst(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
            }
            else if (!selPawn.CanReach(this.parent, PathEndMode.InteractionCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
            {
                yield return new FloatMenuOption("CannotChooseNavigator".Translate() + ": " + "NoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
            }
            else if (this.ValidateNavigator(selPawn))
            {
                yield return new FloatMenuOption("StellarisPilotShip".Translate(), delegate ()
                {
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(StellarisDefOf.StellarisShipConsoleJob, parent));
                });
            }
            yield break;
        }
        private AcceptanceReport ValidateNavigator(LocalTargetInfo target)
        {
            Pawn pawn = target.Thing as Pawn;
            if (pawn == null)
            {
                return false;
            }
            if (!pawn.IsColonistPlayerControlled)
            {
                return false;
            }
            if (pawn.Downed || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
            {
                return "Incapable".Translate();
            }
            if (pawn.skills == null || pawn.skills.GetSkill(SkillDefOf.Intellectual).TotallyDisabled)
            {
                return "Incapable".Translate();
            }
            return true;
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
                yield return g;
            if (!shipActive && parent.Faction == Faction.OfPlayer && !(parent.Map.Parent is WorldShip))
            {
                var launch = new Command_Action
                {
                    defaultLabel = "StellarisLaunchShipLabel".Translate(),
                    defaultDesc = "StellarisLaunchShipDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip"),
                    action = LaunchShip
                };
                launch.disabledReason = " ";
                bool flag = false;
                if (!powerTrader.PowerOn)
                {
                    launch.disabledReason += "StellarisPowerOff".Translate();
                    flag = true;
                }
                if (!hasDriver)
                {
                    launch.disabledReason += "StellarisNoDriver".Translate();
                    flag = true;
                }
                if (flag)
                {
                    launch.Disable(launch.disabledReason);
                }
                yield return launch;
                if (Prefs.DevMode)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "强制启动舰船",
                        defaultDesc = "将舰船发射到太空",
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip"),
                        action = ForceLaunchShip
                    };
                }
            }
            if (parent.Map.Parent is WorldShip)
            {
                var landShip = new Command_Action
                {
                    defaultLabel = "StellarisLandShipLabel".Translate(),
                    defaultDesc = "StellarisLandShipDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip"),

                    action = delegate
                    {
                        ShipUtility.LandShip(WorldShip.playerShip, parent);
                    }
                };
                yield return landShip;
            }


            // dev mode
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                { 
                    defaultDesc = "show ship power plant registered",
                    defaultLabel = "show ship power plant registered",
                    action = delegate 
                    {
                        foreach (var item in shipMapComp.shipPowerPlants)
                        {
                            Log.Message(item.parent.Label + ": " + item.parent.Position);
                        }
                    }
                };
            }
        }
        private bool CanUseNow()
        {
            return powerTrader.PowerOn && !hasDriver;
        }
        private void LaunchShip()
        {
            float thrustPowerNeeded = ShipUtility.CalculateShipRegion(parent.Position,parent.Map).allCells.Count;
            if (!HasEnoughThrustPower(thrustPowerNeeded))
            {
                Messages.Message("StellarisNoEnoughThrustPower".Translate(), MessageTypeDefOf.NegativeEvent, false);
            }
            if (HasEnoughFuel(thrustPowerNeeded))
            {
                if (ShipUtility.TryLaunchShip(parent.Map, this.parent.Position))
                {
                    TryConsumeFuelToThrust(thrustPowerNeeded);
                }
            }
            else
            {
                Messages.Message("StellarisNoEnoughFuel".Translate(), MessageTypeDefOf.NegativeEvent, false);
            }
        }
        private void ForceLaunchShip()
        {
            ShipUtility.TryLaunchShip(parent.Map, this.parent.Position,true);
        }

        public bool HasEnoughThrustPower(float thrustPowerNeeded)
        {
            float maxThrust = 0;                   
            if (!shipMapComp.thrusters.Empty())
            {
                foreach (var thrust in shipMapComp.thrusters)
                {
                    maxThrust += thrust.Props.thrustPower;
                }
                if (maxThrust < thrustPowerNeeded)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        public bool HasEnoughFuel(float thrustPowerNeeded)
        {
            if (Prefs.DevMode)
            {
                //Log.Message("thrustPowerNeeded"+thrustPowerNeeded);
            }
            float fuelToCost = 0;
            float averageThrustPowerNeeded = 0;
            float allFuel = 0;
            if (!shipMapComp.thrusters.Empty())
            {
                averageThrustPowerNeeded = thrustPowerNeeded / shipMapComp.thrusters.Count;
                foreach (var thrust in shipMapComp.thrusters)
                {
                    fuelToCost += averageThrustPowerNeeded * thrust.Props.fuelCostPerThrustPower;
                }
                foreach (var item in shipMapComp.fuelTanks)
                {
                    allFuel += item.Fuel;
                }
                if (allFuel < fuelToCost)
                {
                    //Log.Message("All fuels: " + allFuel);
                    //Log.Message("Fuels to cost: " + fuelToCost);
                    return false;
                }
                return true;
            }
            return false;
        }
        public bool TryConsumeFuelToThrust(float thrustPowerNeeded)
        {
            float maxThrust = 0;
            float fuelToCost = 0;
            float averageThrustPowerNeeded = 0;
            if (!shipMapComp.thrusters.Empty())
            {
                averageThrustPowerNeeded = thrustPowerNeeded / shipMapComp.thrusters.Count;
                foreach (var thrust in shipMapComp.thrusters)
                {
                    fuelToCost += averageThrustPowerNeeded * thrust.Props.fuelCostPerThrustPower;
                    maxThrust += thrust.Props.thrustPower;
                }
                if (maxThrust < thrustPowerNeeded)
                {
                    return false;
                }
                ConsumeFuelDirectly(fuelToCost);
                return true;
            }
            return false;
        }
        public void ConsumeFuelDirectly(float amount)
        {
            float hasFuelAmount = 0;
            foreach (var item in shipMapComp.fuelTanks)
            {
                hasFuelAmount += item.Fuel;
            }
            if (hasFuelAmount < amount)
            {
                foreach (var item in shipMapComp.fuelTanks)
                {
                    item.ConsumeFuel(item.Fuel);
                }
            }
            else
            {
                foreach (var item in shipMapComp.fuelTanks)
                {
                    if (amount > item.Fuel)
                    {
                        amount -= item.Fuel;
                        item.ConsumeFuel(item.Fuel);
                    }
                    else
                    {
                        item.ConsumeFuel(amount);
                        amount = 0;
                    }
                }
            }
        }
    }

    public class CompProperties_ShipControl : CompProperties
    {
        public CompProperties_ShipControl()
        {
            compClass = typeof(CompShipControl);
        }
    }

    // 推进器组件
    public class CompShipThruster : ThingComp
    {
        public CompProperties_ShipThruster Props => (CompProperties_ShipThruster)props;
    }

    public class CompProperties_ShipThruster : CompProperties
    {
        public CompProperties_ShipThruster()
        {
            compClass = typeof(CompShipThruster);
        }

        public float thrustPower = 1f;
        public float fuelCostPerThrustPower = 1f;
    }


    // 世界对象 - 太空中的舰船


}