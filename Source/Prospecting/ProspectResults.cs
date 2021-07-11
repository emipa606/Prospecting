using System;
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
            var map = prospector.Map;
            var mining = prospector.skills.GetSkill(SkillDefOf.Mining).Level;
            var radius = Math.Max(3, 1 + (int) (mining / 4f));
            var oreFound = false;
            Thing focus = null;
            ThingDef resource = null;
            var cells = GenRadial.RadialCellsAround(targetCell, radius, true).ToList();
            if (cells.Count <= 0)
            {
                return;
            }

            foreach (var cell in cells)
            {
                if (!cell.InBounds(map))
                {
                    continue;
                }

                Thing mineable = cell.GetFirstMineable(map);
                if (mineable == null || !cell.Fogged(map))
                {
                    continue;
                }

                map.fogGrid.Unfog(cell);
                if (oreFound || !mineable.def.building.isResourceRock)
                {
                    continue;
                }

                oreFound = true;
                focus = mineable;
                var def = mineable.def;
                ThingDef thingDef;
                if (def == null)
                {
                    thingDef = null;
                }
                else
                {
                    var building = def.building;
                    thingDef = building?.mineableThing;
                }

                resource = thingDef;
            }

            if (!oreFound)
            {
                return;
            }

            string pname = "Prospecting.AProspector".Translate();
            if (prospector.LabelShort != null)
            {
                pname = prospector.LabelShort;
            }

            string oreFoundMsg = "Prospecting.OreFound".Translate(pname);
            if (resource != null)
            {
                oreFoundMsg += "Prospecting.ResourceFound".Translate(resource.label ?? "unknown");
            }

            oreFoundMsg += ".";
            Messages.Message(oreFoundMsg, focus, MessageTypeDefOf.PositiveEvent);
        }

        // Token: 0x0600009C RID: 156 RVA: 0x00005984 File Offset: 0x00003B84
        internal static void RemoveProspectDesig(Map map, IntVec3 targetCell)
        {
            var ProspectDesig = ProspectDef.Prospect;
            map.designationManager.TryRemoveDesignation(targetCell, ProspectDesig);
        }
    }
}