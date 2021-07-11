using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
    // Token: 0x02000021 RID: 33
    public class ProspectingGenDeep
    {
        // Token: 0x06000085 RID: 133 RVA: 0x00005114 File Offset: 0x00003314
        internal static bool ProspectSuccess(Pawn p, out float radius)
        {
            var skill = Math.Min(20, p.skills.GetSkill(SkillDefOf.Mining).Level);
            radius = Mathf.Lerp(50f, 100f, Math.Min(0f, (20f - skill) / 20f));
            var chance = Math.Max(1, (int) (skill * (Controller.Settings.BaseChance / 100f)));
            return ProspectingUtility.Rnd100() <= chance;
        }

        // Token: 0x06000086 RID: 134 RVA: 0x00005190 File Offset: 0x00003390
        internal static bool DoProspectFind(Pawn worker, Map map, float radius, out IntVec3 prospect,
            out ThingDef thingDef)
        {
            thingDef = null;
            prospect = IntVec3.Zero;
            if (!CellFinderLoose.TryFindRandomNotEdgeCellWith(5,
                x => CanScatterAt(x, map) && x != worker.Position &&
                     (float) (x - worker.Position).LengthHorizontalSquared <= radius * radius, map, out var result))
            {
                return false;
            }

            thingDef = ChooseLumpThingDef();
            if (!thingDef.IsMetal)
            {
                return false;
            }

            prospect = result;
            var numCells = Mathf.CeilToInt(thingDef.deepLumpSizeRange.RandomInRange);
            foreach (var item in GridShapeMaker.IrregularLump(result, map, numCells))
            {
                if (CanScatterAt(item, map) && !item.InNoBuildEdgeArea(map))
                {
                    map.deepResourceGrid.SetAt(item, thingDef, thingDef.deepCountPerCell);
                }
            }

            return true;
        }

        // Token: 0x06000087 RID: 135 RVA: 0x00005294 File Offset: 0x00003494
        private static bool CanScatterAt(IntVec3 pos, Map map)
        {
            var terrainDef = map.terrainGrid.TerrainAt(CellIndicesUtility.CellToIndex(pos, map.Size.x));
            return (terrainDef == null || !terrainDef.IsWater || terrainDef.passability != Traversability.Impassable) &&
                   !map.deepResourceGrid.GetCellBool(CellIndicesUtility.CellToIndex(pos, map.Size.x));
        }

        // Token: 0x06000088 RID: 136 RVA: 0x000052F3 File Offset: 0x000034F3
        private static ThingDef ChooseLumpThingDef()
        {
            return DefDatabase<ThingDef>.AllDefs.RandomElementByWeight(def => def.deepCommonality);
        }
    }
}