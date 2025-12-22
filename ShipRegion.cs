using System.Collections.Generic;
using Verse;
using System.Linq;
using System;
namespace Stellaris
{
    public class ShipRegion
    {
        public HashSet<IntVec3> allCells;
        public CellRect boundingRect;
        public IntVec3 centerCell;
        public ShipRegion DeepCopy()
        {
            ShipRegion copyRegion = new ShipRegion();
            List<IntVec3> tempList = new List<IntVec3>();
            allCells.CopyToList(tempList, false);
            copyRegion.allCells = tempList.ToHashSet<IntVec3>();
            copyRegion.CalculateBounds();
            return copyRegion;
        }
        public ShipRegion()
        {
            allCells = new HashSet<IntVec3>();
        }

        public void CalculateBounds()
        {
            if (allCells.Count == 0)
            {
                boundingRect = new CellRect(0, 0, 0, 0);
                centerCell = IntVec3.Zero;
                return;
            }

            int minX = int.MaxValue, maxX = int.MinValue;
            int minZ = int.MaxValue, maxZ = int.MinValue;

            foreach (IntVec3 cell in allCells)
            {
                if (cell.x < minX) minX = cell.x;
                if (cell.x > maxX) maxX = cell.x;
                if (cell.z < minZ) minZ = cell.z;
                if (cell.z > maxZ) maxZ = cell.z;
            }

            boundingRect = new CellRect(minX, minZ, maxX - minX + 1, maxZ - minZ + 1);

            // 计算中心点（使用所有细胞的平均值）
            int totalX = 0, totalZ = 0;
            foreach (IntVec3 cell in allCells)
            {
                totalX += cell.x;
                totalZ += cell.z;
            }

            centerCell = new IntVec3(totalX / allCells.Count, 0, totalZ / allCells.Count);
        }
        public void MoveToCenter(IntVec3 newCenter)
        {
            if (centerCell == newCenter) return;

            // 计算从当前中心到新中心的偏移量
            IntVec3 offset = new IntVec3(
                newCenter.x - centerCell.x,
                newCenter.y - centerCell.y,
                newCenter.z - centerCell.z
            );

            MoveRegionFast(offset);
        }
        public void MoveRegionFast(IntVec3 offset)
        {
            if (offset == IntVec3.Zero || allCells.Count == 0) return;

            // 移动每个单元格
            var newCells = new HashSet<IntVec3>(allCells.Count);
            foreach (var cell in allCells)
            {
                newCells.Add(cell + offset);
            }

            allCells = newCells;

            // 更新边界矩形（直接移动，不重新计算）
            boundingRect = new CellRect(
                boundingRect.minX + offset.x,
                boundingRect.minZ + offset.z,
                boundingRect.Width,
                boundingRect.Height
            );

            // 更新中心点
            centerCell += offset;
        }

    }

}
