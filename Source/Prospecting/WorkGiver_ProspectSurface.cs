using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
    // Token: 0x02000029 RID: 41
    public class WorkGiver_ProspectSurface : WorkGiver_Scanner
    {
        // Token: 0x04000049 RID: 73
        private const int MiningJobTicks = 20000;

        // Token: 0x04000048 RID: 72
        private static string NoPathTrans;

        // Token: 0x0400004A RID: 74
        public DesignationDef prospectDesig = ProspectDef.Prospect;

        // Token: 0x0400004B RID: 75
        public JobDef ProspectJob = ProspectDef.ProspectSurface;

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x060000AE RID: 174 RVA: 0x000062D2 File Offset: 0x000044D2
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        // Token: 0x060000AF RID: 175 RVA: 0x000062D5 File Offset: 0x000044D5
        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        // Token: 0x060000B0 RID: 176 RVA: 0x000062D8 File Offset: 0x000044D8
        public static void ResetStaticData()
        {
            NoPathTrans = "NoPath".Translate();
        }

        // Token: 0x060000B1 RID: 177 RVA: 0x000062EE File Offset: 0x000044EE
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

        // Token: 0x060000B2 RID: 178 RVA: 0x00006308 File Offset: 0x00004508
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
}