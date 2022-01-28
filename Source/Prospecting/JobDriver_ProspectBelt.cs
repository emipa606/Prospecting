using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Prospecting;

public class JobDriver_ProspectBelt : JobDriver
{
    private float progressProspect;

    private int remainingTicks;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref remainingTicks, "remainingTicks");
        Scribe_Values.Look(ref progressProspect, "progressProspect");
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var pawn1 = pawn;
        var targetA = job.targetA;
        var job1 = job;
        remainingTicks = job1.expiryInterval;
        return pawn1.Reserve(targetA, job1, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOn(() => !ProspectBelt.IsWearingProspectBelt(GetActor()));
        var work = new Toil { initAction = delegate { pawn.pather.StopDead(); } };
        work.tickAction = delegate
        {
            work.actor.skills.Learn(SkillDefOf.Mining, 0.07f);
            remainingTicks--;
            progressProspect = Mathf.Lerp(1f, 0f, remainingTicks / (float)job.expiryInterval);
        };
        work.defaultCompleteMode = ToilCompleteMode.Never;
        work.WithProgressBar(TargetIndex.A,
            () => ((JobDriver_ProspectBelt)work.actor.jobs.curDriver).progressProspect);
        work.WithEffect(EffecterDefOf.ConstructDirt, TargetIndex.A);
        work.activeSkill = () => SkillDefOf.Mining;
        work.AddFinishAction(delegate { ProspectBelt.DoPrsProspectBelt(GetActor()); });
        yield return work;
    }
}