using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting;

public class WorkGiver_ManualDrillMine : WorkGiver_Scanner
{
    public ThingDef ManualDrillDef = ProspectDef.PrsManualDrill;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ManualDrillDef);

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ManualDrillDef);
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        var allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
        foreach (var building in allBuildingsColonist)
        {
            if (building.def != ManualDrillDef || !building.Spawned ||
                building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) != null)
            {
                continue;
            }

            var CMD = building.TryGetComp<CompManualDrill>();
            if (CMD is { prospected: true, windOk: true })
            {
                return false;
            }
        }

        return true;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t.Faction != pawn.Faction)
        {
            return false;
        }

        if (!(t is Building building))
        {
            return false;
        }

        if (building.IsForbidden(pawn))
        {
            return false;
        }

        LocalTargetInfo target = building;
        if (!pawn.CanReserve(target, 1, -1, null, forced))
        {
            return false;
        }

        var compManual = building.TryGetComp<CompManualDrill>();
        return compManual is { prospected: true, windOk: true } && ManualDrillUtility.DrillCanGetToCount(building,
                   compManual.MDProps.shallowReach, 9, out _,
                   out _) > 0 && building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) ==
               null &&
               !building.IsBurning();
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(ProspectDef.ManualDrillMine, t, 1500, true);
    }
}