using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
    // Token: 0x02000017 RID: 23
    public class JobDriver_OperateWideBoy : JobDriver
    {
        // Token: 0x0600005A RID: 90 RVA: 0x00004304 File Offset: 0x00002504
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var pawn1 = pawn;
            var targetA = job.targetA;
            var job1 = job;
            return pawn1.Reserve(targetA, job1, 1, -1, null, errorOnFailed);
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00004337 File Offset: 0x00002537
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Uninstall);
            this.FailOn(delegate
            {
                var compDeepDrill = job.targetA.Thing.TryGetComp<CompDeepDrill>();
                var compWideBoy = job.targetA.Thing.TryGetComp<CompWideBoy>();
                if (compDeepDrill == null || compWideBoy == null)
                {
                    return true;
                }

                var value = compDeepDrill.ValuableResourcesPresent();
                var baseRock = false;
                var thing = job.targetA.Thing;
                if (thing?.Map != null)
                {
                    baseRock = DeepDrillUtility.GetBaseResource(job.targetA.Thing.Map,
                        job.targetA.Thing.TrueCenter().ToIntVec3()) != null;
                }

                if (compWideBoy.mineRock)
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

                var powerComp = job.targetA.Thing.TryGetComp<CompPowerTrader>();
                return powerComp == null || !powerComp.PowerOn;
            });
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            var work = new Toil();
            work.tickAction = delegate
            {
                var actor = work.actor;
                ((Building) actor.CurJob.targetA.Thing).GetComp<CompDeepDrill>().DrillWorkDone(actor);
                actor.skills.Learn(SkillDefOf.Mining, 0.07f);
            };
            work.defaultCompleteMode = ToilCompleteMode.Never;
            work.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
            work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            work.activeSkill = () => SkillDefOf.Mining;
            yield return work;
        }
    }
}