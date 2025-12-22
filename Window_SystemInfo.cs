using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Stellaris
{
    public class Window_SystemInfo : Window
    {
        public StarSystem selectedSystem;
        public Window_SystemInfo(StarSystem selectedSystem ,IWindowDrawing customWindowDrawing = null ) : base(customWindowDrawing)
        {
            this.selectedSystem = selectedSystem;
            draggable = true;
        }

        public override Vector2 InitialSize
        {
            get { return new Vector2(280f, 200f); }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Widgets.DrawMenuSection(inRect);

            Rect innerRect = inRect.ContractedBy(10f);
            float y = innerRect.y;

            // 系统名称
            Widgets.Label(new Rect(innerRect.x, y, innerRect.width, 25f), selectedSystem.name);
            y += 30f;

            // 恒星信息
            if (selectedSystem.star != null)
            {
                Widgets.Label(new Rect(innerRect.x, y, innerRect.width, 25f), "StellarisStar".Translate(selectedSystem.star.name));
                y += 25f;
                Widgets.Label(new Rect(innerRect.x, y, innerRect.width, 25f), "StellarisType".Translate(selectedSystem.star.type));
                y += 25f;
            }

            // 行星数量
            Widgets.Label(new Rect(innerRect.x, y, innerRect.width, 25f), "StellarisPlanetCount".Translate(selectedSystem.planets.Count));
            y += 30f;

            // 探索按钮
            if (Widgets.ButtonText(new Rect(innerRect.x, y, innerRect.width, 30f), "StellarisOpenSystemWindow".Translate()))
            {
                Find.WindowStack.Add(new Window_StarSystem(selectedSystem));
            }
        }
    }
}
