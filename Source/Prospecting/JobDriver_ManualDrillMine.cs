using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting;

public class JobDriver_ManualDrillMine : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var pawn1 = pawn;
        var targetA = job.targetA;
        var job1 = job;
        return pawn1.Reserve(targetA, job1, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Uninstall);
        this.FailOn(delegate
        {
            var compManualDrill = job.targetA.Thing.TryGetComp<CompManualDrill>();
            if (compManualDrill == null)
            {
                return true;
            }

            if (!compManualDrill.prospected)
            {
                return true;
            }

            if (!compManualDrill.windOk)
            {
                return true;
            }

            var value = ManualDrillUtility.DrillCanGetToCount(job.targetA.Thing as Building,
                compManualDrill.MDProps.shallowReach, 9, out _, out _) > 0;
            var baseRock = false;
            var thing = job.targetA.Thing;
            if (thing?.Map != null)
            {
                baseRock = DeepDrillUtility.GetBaseResource(job.targetA.Thing.Map,
                    job.targetA.Thing.TrueCenter().ToIntVec3()) != null;
            }

            if (compManualDrill.MDProps.mineRock)
            {
                if (!value && !baseRock)
                {
                    return true;
                }
            }
            else if (!value)
            {
                return true;
            }

            return false;
        });
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
        var work = new Toil();
        work.tickAction = delegate
        {
            var actor = work.actor;
            var building = (Building)actor.CurJob.targetA.Thing;
            building.GetComp<CompManualDrill>().ManualDrillWorkDone(actor, building);
            actor.skills.Learn(SkillDefOf.Mining, 0.07f);
        };
        work.defaultCompleteMode = ToilCompleteMode.Never;
        if (Controller.Settings.AllowManualSound)
        {
            work.WithEffect(ProspectDef.PrsManualDrillEff, TargetIndex.A);
        }

        work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
        work.activeSkill = () => SkillDefOf.Mining;
        yield return work;
    }
}