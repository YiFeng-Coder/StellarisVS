using RimWorld;
using Stellaris.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stellaris
{
    public class ShipMapComp : MapComponent
    {
        public Action landingAction;
        public bool isMovingRegion = false;
        public List<CompShipPowerPlant> shipPowerPlants = new   List<CompShipPowerPlant>();
        public List<CompShipControl> consoles = new List<CompShipControl>();
        public List<CompShipThruster> thrusters = new List<CompShipThruster>();
        public List<CompRefuelable> fuelTanks = new List<CompRefuelable>();

        public ShipRegion cachedShipRegion;
        public bool isLanding = false;
        /*
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref shipPowerPlants, "shipPowerPlants",LookMode.Reference);
            Scribe_Collections.Look(ref consoles, "consoles", LookMode.Reference);
            Scribe_Collections.Look(ref thrusters, "thrusters", LookMode.Reference);
            Scribe_Collections.Look(ref fuelTanks, "fuelTanks", LookMode.Reference);
        }
        */
        public override void MapComponentOnGUI()
        {
            base.MapComponentOnGUI();
            if (isLanding && cachedShipRegion != null)
            {
                if (Find.CurrentMap == null)
                {
                    return;
                }
                if (isMovingRegion)
                {
                    MoveShipRegion(cachedShipRegion);
                }
                int num = 2;
                int num2 = (num + 1) * 8;
                Vector2 buttonSize = new Vector2(150f, 38f);
                Rect rect = new Rect((float)UI.screenWidth / 2f - (float)num * buttonSize.x / 2f - (float)num2 / 2f, (float)UI.screenHeight - (buttonSize.y + 8f) + -50f, (float)num * buttonSize.x + (float)num2, buttonSize.y + 16f);
                Widgets.DrawWindowBackground(rect);
                float num3 = rect.x + 8f;
                if (Widgets.ButtonText(new Rect(num3, rect.y + 8f, buttonSize.x, buttonSize.y), "StellarisMoveShipRegion".Translate(), true, true, true, null))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera(null);
                    isMovingRegion = true;
                }
                num3 += buttonSize.x + 2f + 8f;
                if ((Widgets.ButtonText(new Rect(num3, rect.y + 8f, buttonSize.x, buttonSize.y), "StellarisPlaceShip".Translate(), true, true, true, null)) || KeyBindingDefOf.Accept.KeyDownEvent)
                {
                    SoundDefOf.Click.PlayOneShotOnCamera(null);
                    foreach (var cell in cachedShipRegion.allCells)
                    {
                        List<Thing> things = map.thingGrid.ThingsListAtFast(cell);
                        if (!things.Empty())
                        {
                            foreach (var item in things)
                            {
                                item.Destroy();
                            }
                        }
                    }
                    landingAction();
                    isLanding = false;
                }
            }
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();

            // 每一帧尝试绘制
            if (isLanding && cachedShipRegion != null)
            {
                CellHighlighter.DrawHelpers(cachedShipRegion.allCells);
            }
        }
        private void MoveShipRegion(ShipRegion copyRegion)
        {
            IntVec3 mouseCell = UI.MouseCell();
            // 检查该单元格是否在地图内
            if (!mouseCell.InBounds(Find.CurrentMap))
            {
                mouseCell = IntVec3.Invalid;
            }
            copyRegion.MoveToCenter(mouseCell);
            if (Event.current != null && Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    isMovingRegion = false;
                }
                Event.current.Use();
            }

        }
        public ShipMapComp(Map map) : base(map)
        {
        }
    }
}
