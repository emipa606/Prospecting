using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Prospecting
{
	// Token: 0x02000025 RID: 37
	public class ProspectResults
	{
		// Token: 0x0600009B RID: 155 RVA: 0x000057DC File Offset: 0x000039DC
		internal static void CheckProspectResult(Pawn prospector, IntVec3 targetCell)
		{
			Map map = prospector.Map;
			int mining = prospector.skills.GetSkill(SkillDefOf.Mining).Level;
			int radius = Math.Max(3, 1 + (int)((float)mining / 4f));
			bool oreFound = false;
			Thing focus = null;
			ThingDef resource = null;
			List<IntVec3> cells = GenRadial.RadialCellsAround(targetCell, (float)radius, true).ToList<IntVec3>();
			if (cells.Count > 0)
			{
				foreach (IntVec3 cell in cells)
				{
					if (cell.InBounds(map))
					{
						Thing mineable = cell.GetFirstMineable(map);
						if (mineable != null && cell.Fogged(map))
						{
							map.fogGrid.Unfog(cell);
							if (!oreFound && mineable.def.building.isResourceRock)
							{
								oreFound = true;
								focus = mineable;
								ThingDef def = mineable.def;
								ThingDef thingDef;
								if (def == null)
								{
									thingDef = null;
								}
								else
								{
									BuildingProperties building = def.building;
									thingDef = ((building != null) ? building.mineableThing : null);
								}
								resource = thingDef;
							}
						}
					}
				}
				if (oreFound)
				{
					string pname = "Prospecting.AProspector".Translate();
					if (prospector.LabelShort != null)
					{
						pname = prospector.LabelShort;
					}
					string oreFoundMsg = "Prospecting.OreFound".Translate(pname);
					if (resource != null)
					{
						oreFoundMsg += "Prospecting.ResourceFound".Translate((resource != null && resource.label != null) ? resource.label : "unknown");
					}
					oreFoundMsg += ".";
					Messages.Message(oreFoundMsg, focus, MessageTypeDefOf.PositiveEvent, true);
				}
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005984 File Offset: 0x00003B84
		internal static void RemoveProspectDesig(Map map, IntVec3 targetCell)
		{
			DesignationDef ProspectDesig = ProspectDef.Prospect;
			map.designationManager.TryRemoveDesignation(targetCell, ProspectDesig);
		}
	}
}
