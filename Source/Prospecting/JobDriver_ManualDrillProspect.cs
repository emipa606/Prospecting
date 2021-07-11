using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
    // Token: 0x02000016 RID: 22
    public class JobDriver_ManualDrillProspect : JobDriver
    {
        // Token: 0x06000057 RID: 87 RVA: 0x000042B8 File Offset: 0x000024B8
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var pawn1 = pawn;
            var targetA = job.targetA;
            var job1 = job;
            return pawn1.Reserve(targetA, job1, 1, -1, null, errorOnFailed);
        }

        // Token: 0x06000058 RID: 88 RVA: 0x000042EB File Offset: 0x000024EB
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Uninstall);
            this.FailOn(delegate
            {
                var compManualDrill = job.targetA.Thing.TryGetComp<CompManualDrill>();
                return compManualDrill == null || compManualDrill.prospected || !compManualDrill.windOk;
            });
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            var work = new Toil();
            work.tickAction = delegate
            {
                var actor = work.actor;
                var building = (Building) actor.CurJob.targetA.Thing;
                building.GetComp<CompManualDrill>().ManualProspectWorkDone(actor, building);
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
}