using RimWorld;
using RimWorld.Planet;
using Stellaris.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using Verse;
using Verse.Sound;

namespace Stellaris
{
    public class Dialog_Land : Window
    {
        public bool isMovingRegion = false; 
        public ShipRegion copyRegion;
        private HashSet<IntVec3> area;
        private Map sourceMap;
        private Map targetMap;
        private ShipMapComp shipMapComp;
        private Vector2 initialSize;

        public Dialog_Land(Vector2 initialSize)
        {
            this.initialSize = initialSize;
        }

        public override Vector2 InitialSize => initialSize;

        public Dialog_Land(ShipRegion copyRegion,HashSet<IntVec3> area, Map sourceMap, Map targetMap, IWindowDrawing customWindowDrawing = null) : base(customWindowDrawing)
        {
            this.copyRegion = copyRegion;
            this.area = area;
            this.sourceMap = sourceMap;
            this.targetMap = targetMap;
            int num = 1;
            int num2 = (num + 1) * 8;
            Vector2 buttonSize = new Vector2(150f, 38f);
            initialSize = new Vector2((float)num * buttonSize.x + (float)num2, buttonSize.y + 16f);
            windowRect = new Rect((float)UI.screenWidth / 2f - (float)num * buttonSize.x / 2f - (float)num2 / 2f, (float)UI.screenHeight - (buttonSize.y + 8f) + -50f, (float)num * buttonSize.x + (float)num2, buttonSize.y + 16f);
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Find.CurrentMap == null)
            {
                return;
            }
            if (shipMapComp == null)
            {
                shipMapComp = Find.CurrentMap.GetComponent<ShipMapComp>();
            }

            shipMapComp.cachedShipRegion = copyRegion;
            if (isMovingRegion)
            {
                MoveShipRegion();
            }
            shipMapComp.isLanding = true;
            int num = 1;
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
                MapObjectTransfer.TransferObjectsFromArea(area,sourceMap,targetMap,copyRegion.allCells.First());
                shipMapComp.isLanding = false;
            }
        }
        public void MoveShipRegion()
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
                    isMovingRegion =false;
                }
                Event.current.Use();
            }

        }
    }
}
