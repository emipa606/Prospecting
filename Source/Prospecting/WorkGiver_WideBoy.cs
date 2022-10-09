using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting;

public class WorkGiver_WideBoy : WorkGiver_DeepDrill
{
    public ThingDef wideBoyDef = ProspectDef.PrsWideDeepDrill;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(wideBoyDef);

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(wideBoyDef);
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        var allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
        foreach (var building in allBuildingsColonist)
        {
            if (building.def != wideBoyDef)
            {
                continue;
            }

            var comp = building.GetComp<CompPowerTrader>();
            if ((comp == null || comp.PowerOn) &&
                building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null)
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

        var compDeepDrill = building.TryGetComp<CompDeepDrill>();
        if (compDeepDrill != null)
        {
            var value = compDeepDrill.ValuableResourcesPresent();
            var baseRock = false;
            if (building.Map != null)
            {
                baseRock = DeepDrillUtility.GetBaseResource(building.Map, building.TrueCenter().ToIntVec3()) !=
                           null;
            }

            var compWideBoy = building.TryGetComp<CompWideBoy>();
            if (compWideBoy != null)
            {
                if (compWideBoy.mineRock)
                {
                    if (building.Map == null)
                    {
                        return false;
                    }

                    if (!value && !baseRock)
                    {
                        return false;
                    }
                }
                else if (!value)
                {
                    return false;
                }
            }
        }

        var powerComp = building.TryGetComp<CompPowerTrader>();
        return (powerComp == null || powerComp.PowerOn) &&
               building.Map?.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null &&
               !building.IsBurning();
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(ProspectDef.OperateWideBoy, t, 1500, true);
    }
}