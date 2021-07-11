using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
    // Token: 0x0200002A RID: 42
    public class WorkGiver_WideBoy : WorkGiver_DeepDrill
    {
        // Token: 0x0400004C RID: 76
        public ThingDef wideBoyDef = ProspectDef.PrsWideDeepDrill;

        // Token: 0x1700000D RID: 13
        // (get) Token: 0x060000B4 RID: 180 RVA: 0x00006504 File Offset: 0x00004704
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(wideBoyDef);

        // Token: 0x060000B5 RID: 181 RVA: 0x00006511 File Offset: 0x00004711
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(wideBoyDef);
        }

        // Token: 0x060000B6 RID: 182 RVA: 0x00006530 File Offset: 0x00004730
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

        // Token: 0x060000B7 RID: 183 RVA: 0x000065A4 File Offset: 0x000047A4
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
                   building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null &&
                   !building.IsBurning();
        }

        // Token: 0x060000B8 RID: 184 RVA: 0x0000669F File Offset: 0x0000489F
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(ProspectDef.OperateWideBoy, t, 1500, true);
        }
    }
}