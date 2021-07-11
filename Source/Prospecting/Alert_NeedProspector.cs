using System.Linq;
using RimWorld;
using Verse;

namespace Prospecting
{
    // Token: 0x02000003 RID: 3
    public class Alert_NeedProspector : Alert
    {
        // Token: 0x06000004 RID: 4 RVA: 0x00002534 File Offset: 0x00000734
        public Alert_NeedProspector()
        {
            defaultLabel = "Prospector.NeedProspector".Translate();
            defaultExplanation = "Prospector.NeedProspectorDesc".Translate();
            defaultPriority = AlertPriority.High;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002570 File Offset: 0x00000770
        public override AlertReport GetReport()
        {
            var maps = Find.Maps;
            foreach (var map in maps)
            {
                if (!map.IsPlayerHome)
                {
                    continue;
                }

                var designation = (from d in map.designationManager.allDesignations
                    where d.def == ProspectDef.Prospect
                    select d).FirstOrDefault();
                if (designation == null)
                {
                    continue;
                }

                var needProspector = false;
                foreach (var item in map.mapPawns.FreeColonistsSpawned)
                {
                    if (item.Downed || item.workSettings == null ||
                        item.workSettings.GetPriority(ProspectDef.Prospecting) <= 0)
                    {
                        continue;
                    }

                    needProspector = true;
                    break;
                }

                if (!needProspector)
                {
                    return AlertReport.CulpritIs(designation.target.Thing);
                }
            }

            return false;
        }
    }
}