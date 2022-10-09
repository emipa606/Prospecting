using System.Linq;
using RimWorld;
using Verse;

namespace Prospecting;

public class Alert_NeedProspector : Alert
{
    public Alert_NeedProspector()
    {
        defaultLabel = "Prospector.NeedProspector".Translate();
        defaultExplanation = "Prospector.NeedProspectorDesc".Translate();
        defaultPriority = AlertPriority.High;
    }

    public override AlertReport GetReport()
    {
        var maps = Find.Maps;
        foreach (var map in maps)
        {
            if (!map.IsPlayerHome)
            {
                continue;
            }

            var designation = (from d in map.designationManager.AllDesignations
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