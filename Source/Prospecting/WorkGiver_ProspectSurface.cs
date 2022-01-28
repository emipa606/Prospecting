using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting;

public class WorkGiver_ProspectSurface : WorkGiver_Scanner
{
    private const int MiningJobTicks = 20000;

    private static string NoPathTrans;

    public DesignationDef prospectDesig = ProspectDef.Prospect;

    public JobDef ProspectJob = ProspectDef.ProspectSurface;

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public static void ResetStaticData()
    {
        NoPathTrans = "NoPath".Translate();
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        foreach (var des in pawn.Map.designationManager.SpawnedDesignationsOfDef(prospectDesig))
        {
            var mayBeAccessible = false;
            for (var i = 0; i < 8; i++)
            {
                var c = des.target.Cell + GenAdj.AdjacentCells[i];
                if (!c.InBounds(pawn.Map) || !c.Walkable(pawn.Map))
                {
                    continue;
                }

                mayBeAccessible = true;
                break;
            }

            if (!mayBeAccessible)
            {
                continue;
            }

            var j = des.target.Cell.GetFirstMineable(pawn.Map);
            if (j != null)
            {
                yield return j;
            }
        }
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!t.def.mineable)
        {
            return null;
        }

        if (pawn.Map.designationManager.DesignationAt(t.Position, prospectDesig) == null)
        {
            return null;
        }

        LocalTargetInfo target = t;
        if (!pawn.CanReserve(target, 1, -1, null, forced))
        {
            return null;
        }

        var reachable = false;
        for (var i = 0; i < 8; i++)
        {
            var intVec = t.Position + GenAdj.AdjacentCells[i];
            if (!intVec.InBounds(pawn.Map) || !intVec.Standable(pawn.Map) ||
                !ReachabilityImmediate.CanReachImmediate(intVec, t, pawn.Map, PathEndMode.ClosestTouch, pawn))
            {
                continue;
            }

            reachable = true;
            break;
        }

        if (reachable)
        {
            return new Job(ProspectJob, t, 20000, true);
        }

        for (var j = 0; j < 8; j++)
        {
            var intVec2 = t.Position + GenAdj.AdjacentCells[j];
            if (!intVec2.InBounds(t.Map) ||
                !ReachabilityImmediate.CanReachImmediate(intVec2, t, pawn.Map, PathEndMode.Touch, pawn) ||
                !intVec2.Walkable(t.Map) || intVec2.Standable(t.Map))
            {
                continue;
            }

            Thing thing = null;
            var thingList = intVec2.GetThingList(t.Map);
            foreach (var thing1 in thingList)
            {
                if (!thing1.def.designateHaulable || thing1.def.passability != Traversability.PassThroughOnly)
                {
                    continue;
                }

                thing = thing1;
                break;
            }

            if (thing == null)
            {
                continue;
            }

            var job = HaulAIUtility.HaulAsideJobFor(pawn, thing);
            if (job != null)
            {
                return job;
            }

            JobFailReason.Is(NoPathTrans);
            return null;
        }

        JobFailReason.Is(NoPathTrans);
        return null;
    }
}