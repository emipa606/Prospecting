using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting;

public class ProspectingGenDeep
{
    internal static bool ProspectSuccess(Pawn p, out float radius)
    {
        var skill = Math.Min(20, p.skills.GetSkill(SkillDefOf.Mining).Level);
        radius = Mathf.Lerp(50f, 100f, Math.Min(0f, (20f - skill) / 20f));
        var chance = Math.Max(1, (int)(skill * (Controller.Settings.BaseChance / 100f)));
        return ProspectingUtility.Rnd100() <= chance;
    }

    internal static bool DoProspectFind(Pawn worker, Map map, float radius, out IntVec3 prospect,
        out ThingDef thingDef)
    {
        thingDef = null;
        prospect = IntVec3.Zero;
        if (!CellFinderLoose.TryFindRandomNotEdgeCellWith(5,
                x => CanScatterAt(x, map) && x != worker.Position &&
                     (x - worker.Position).LengthHorizontalSquared <= radius * radius, map, out var result))
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

    private static bool CanScatterAt(IntVec3 pos, Map map)
    {
        var terrainDef = map.terrainGrid.TerrainAt(CellIndicesUtility.CellToIndex(pos, map.Size.x));
        return terrainDef is not { IsWater: true, passability: Traversability.Impassable } &&
               !map.deepResourceGrid.GetCellBool(CellIndicesUtility.CellToIndex(pos, map.Size.x));
    }

    private static ThingDef ChooseLumpThingDef()
    {
        return DefDatabase<ThingDef>.AllDefs.RandomElementByWeight(def => def.deepCommonality);
    }
}