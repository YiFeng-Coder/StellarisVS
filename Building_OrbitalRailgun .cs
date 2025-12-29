using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using static UnityEngine.GraphicsBuffer;

namespace Stellaris
{
    public class Building_OrbitalRailgun : Building_TurretGun
    {
        private int cooldownTicks = 0;
        private const int MaxCooldown = 60000;
        private const float ScatterRadius = 5.9f;
        private CompShipPowerPlant power;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            power = GetComp<CompShipPowerPlant>();
        }

        protected override void Tick()
        {
            // base.Tick() 实际上在 Building 中大多是空的，除非有 Comp，这里调用是为了保险
            base.Tick();
            if (cooldownTicks > 0) cooldownTicks--;
        }

        // GetGizmos 是 UI 帧调用，不需要过度优化，保持逻辑清晰即可
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos()) yield return g;
            if (Prefs.DevMode)
            {
                yield return new Command_Action 
                {
                    defaultLabel = "reset colddown",
                    defaultDesc = "reset colddown",
                    action = delegate { cooldownTicks = 0; }
                };
            }

            if (Faction == Faction.OfPlayer && (power == null || power.PowerOn))
            {
                Command_Action fireCmd = new Command_Action
                {
                    defaultLabel = "StellarisChooseAttackTargetLabel".Translate(),
                    defaultDesc = "StellarisChooseAttackTargetDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Designators/Hunt", true),
                    action = () =>
                    {
                        PlanetTile tile = Tile;
                        CameraJumper.TryJump(CameraJumper.GetWorldTarget(new GlobalTargetInfo(tile)), CameraJumper.MovementMode.Pan);
                        Find.WorldSelector.ClearSelection();
                        Find.WorldTargeter.BeginTargeting(
                             (target) => ChoseWorldTarget(target),
                             true, CompLaunchable.TargeterMouseAttachment, true, delegate
                             {
                             }, null, null, new PlanetTile?(tile), true);
                    }
                };

                if (cooldownTicks > 0)
                    fireCmd.Disable("StellarisColddown".Translate((cooldownTicks / 2500f)) + "h");

                yield return fireCmd;
            }
        }

        private bool ChoseWorldTarget(GlobalTargetInfo target)
        {
            if (!target.IsValid || target.Tile == this.Map.Tile) return false;
            LaunchProjectileStrike(target);
            return true;
        }

        private void LaunchProjectileStrike(GlobalTargetInfo target)
        {
            // 避免 LINQ 带来的 GC Alloc (虽然在事件触发时无所谓，但这是优化版)
            Map targetMap = null;
            List<Map> maps = Current.Game.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].Tile == target.Tile)
                {
                    targetMap = maps[i];
                    break;
                }
            }

            if (targetMap != null)
            {
                CameraJumper.TryJump(targetMap.Center,targetMap);
                Find.Targeter.BeginTargeting(
                    GetTargetingParameters(),
                    (LocalTargetInfo cell) => FireAtCell(cell.Cell, targetMap),
                    null, null, null
                );

            }
        }

        private void FireAtCell(IntVec3 cell, Map targetMap)
        {
            cooldownTicks = MaxCooldown;
            IntVec3 hitCell = CellFinder.RandomClosewalkCellNear(cell, targetMap, (int)ScatterRadius, null);
            if (!hitCell.IsValid) hitCell = cell;

            IntVec3 edgeCell;
            if (!CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => !c.Roofed(targetMap), targetMap, 1f, out edgeCell))
                edgeCell = CellFinder.RandomEdgeCell(targetMap);

            ThingDef projectileDef = StellarisDefOf.StellarisRailgunShell;
            Projectile projectile = (Projectile)GenSpawn.Spawn(projectileDef, edgeCell, targetMap);

            projectile.Launch(this, edgeCell.ToVector3(), new LocalTargetInfo(hitCell), new LocalTargetInfo(hitCell), ProjectileHitFlags.All);

            CameraJumper.TryJump(new GlobalTargetInfo(edgeCell, targetMap));
        }

        private TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetLocations = true,
                canTargetBuildings = true,
                canTargetPawns = true,
                validator = (TargetInfo x) => !x.Cell.Fogged(x.Map) // 只能瞄准看得见的地方
            };
        }
    }
}
