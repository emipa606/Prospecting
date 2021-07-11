using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Prospecting
{
    // Token: 0x02000018 RID: 24
    public class JobDriver_ProspectBelt : JobDriver
    {
        // Token: 0x04000020 RID: 32
        private float progressProspect;

        // Token: 0x0400001F RID: 31
        private int remainingTicks;

        // Token: 0x0600005D RID: 93 RVA: 0x0000434F File Offset: 0x0000254F
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref remainingTicks, "remainingTicks");
            Scribe_Values.Look(ref progressProspect, "progressProspect");
        }

        // Token: 0x0600005E RID: 94 RVA: 0x00004380 File Offset: 0x00002580
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var pawn1 = pawn;
            var targetA = job.targetA;
            var job1 = job;
            remainingTicks = job1.expiryInterval;
            return pawn1.Reserve(targetA, job1, 1, -1, null, errorOnFailed);
        }

        // Token: 0x0600005F RID: 95 RVA: 0x000043BF File Offset: 0x000025BF
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => !ProspectBelt.IsWearingProspectBelt(GetActor()));
            var work = new Toil {initAction = delegate { pawn.pather.StopDead(); }};
            work.tickAction = delegate
            {
                work.actor.skills.Learn(SkillDefOf.Mining, 0.07f);
                remainingTicks--;
                progressProspect = Mathf.Lerp(1f, 0f, remainingTicks / (float) job.expiryInterval);
            };
            work.defaultCompleteMode = ToilCompleteMode.Never;
            work.WithProgressBar(TargetIndex.A,
                () => ((JobDriver_ProspectBelt) work.actor.jobs.curDriver).progressProspect);
            work.WithEffect(EffecterDefOf.ConstructDirt, TargetIndex.A);
            work.activeSkill = () => SkillDefOf.Mining;
            work.AddFinishAction(delegate { ProspectBelt.DoPrsProspectBelt(GetActor()); });
            yield return work;
        }
    }
}