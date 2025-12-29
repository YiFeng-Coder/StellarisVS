using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stellaris
{
    public class Projectile_Railgun : Projectile_Explosive
    {
        private IntVec3 lastCheckedCell = IntVec3.Invalid;

        protected override void Tick()
        {
            base.Tick(); // 执行原版移动逻辑

            if (this.Destroyed) return;

            // 优化：只有当子弹所在的格子发生变化时，才检测碰撞
            // 原版 Tick 每秒运行 60 次，但子弹越过一个格子可能需要多次 Tick
            if (this.Position != lastCheckedCell)
            {
                lastCheckedCell = this.Position;
                CheckCollision();
            }
        }

        private void CheckCollision()
        {
            // 获取当前格子的所有物体
            List<Thing> thingList = this.Position.GetThingList(this.Map);
            // 倒序遍历通常比正序安全（如果列表在遍历中被修改），且略快
            for (int i = thingList.Count - 1; i >= 0; i--)
            {
                Thing t = thingList[i];
                // 检测：是建筑，不是发射者
                if (t.Map != launcher.Map &&t.def.category == ThingCategory.Building && t != this.launcher)
                {
                    this.Impact(t);
                    return;
                }
            }
        }
    }
}
