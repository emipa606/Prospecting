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
			int skill = Math.Min(20, p.skills.GetSkill(SkillDefOf.Mining).Level);
			radius = Mathf.Lerp(50f, 100f, Math.Min(0f, (20f - (float)skill) / 20f));
			int chance = Math.Max(1, (int)((float)skill * (Controller.Settings.BaseChance / 100f)));
			return ProspectingUtility.Rnd100() <= chance;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00005190 File Offset: 0x00003390
		internal static bool DoProspectFind(Pawn worker, Map map, float radius, out IntVec3 prospect, out ThingDef thingDef)
		{
			thingDef = null;
			prospect = IntVec3.Zero;
			IntVec3 result;
			if (!CellFinderLoose.TryFindRandomNotEdgeCellWith(5, (IntVec3 x) => ProspectingGenDeep.CanScatterAt(x, map) && x != worker.Position && (float)(x - worker.Position).LengthHorizontalSquared <= radius * radius, map, out result))
			{
				return false;
			}
			thingDef = ProspectingGenDeep.ChooseLumpThingDef();
			if (thingDef.IsMetal)
			{
				prospect = result;
				int numCells = Mathf.CeilToInt((float)thingDef.deepLumpSizeRange.RandomInRange);
				foreach (IntVec3 item in GridShapeMaker.IrregularLump(result, map, numCells))
				{
					if (ProspectingGenDeep.CanScatterAt(item, map) && !item.InNoBuildEdgeArea(map))
					{
						map.deepResourceGrid.SetAt(item, thingDef, thingDef.deepCountPerCell);
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005294 File Offset: 0x00003494
		private static bool CanScatterAt(IntVec3 pos, Map map)
		{
			TerrainDef terrainDef = map.terrainGrid.TerrainAt(CellIndicesUtility.CellToIndex(pos, map.Size.x));
			return (terrainDef == null || !terrainDef.IsWater || terrainDef.passability != Traversability.Impassable) && !map.deepResourceGrid.GetCellBool(CellIndicesUtility.CellToIndex(pos, map.Size.x));
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000052F3 File Offset: 0x000034F3
		private static ThingDef ChooseLumpThingDef()
		{
			return DefDatabase<ThingDef>.AllDefs.RandomElementByWeight((ThingDef def) => def.deepCommonality);
		}
	}
}
