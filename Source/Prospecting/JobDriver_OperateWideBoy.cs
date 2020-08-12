using System;
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
			Pawn pawn = this.pawn;
			LocalTargetInfo targetA = this.job.targetA;
			Job job = this.job;
			return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004337 File Offset: 0x00002537
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.A);
			this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Uninstall);
			this.FailOn(delegate()
			{
				CompDeepDrill compDeepDrill = this.job.targetA.Thing.TryGetComp<CompDeepDrill>();
				CompWideBoy compWideBoy = this.job.targetA.Thing.TryGetComp<CompWideBoy>();
				if (compDeepDrill != null && compWideBoy != null)
				{
					bool value = compDeepDrill.ValuableResourcesPresent();
					bool baseRock = false;
					Thing thing = this.job.targetA.Thing;
					if (((thing != null) ? thing.Map : null) != null)
					{
						baseRock = (DeepDrillUtility.GetBaseResource(this.job.targetA.Thing.Map, this.job.targetA.Thing.TrueCenter().ToIntVec3()) != null);
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
					CompPowerTrader powerComp = this.job.targetA.Thing.TryGetComp<CompPowerTrader>();
					return powerComp == null || !powerComp.PowerOn;
				}
				return true;
			});
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			Toil work = new Toil();
			work.tickAction = delegate()
			{
				Pawn actor = work.actor;
				((Building)actor.CurJob.targetA.Thing).GetComp<CompDeepDrill>().DrillWorkDone(actor);
				actor.skills.Learn(SkillDefOf.Mining, 0.07f, false);
			};
			work.defaultCompleteMode = ToilCompleteMode.Never;
			work.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
			work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			work.activeSkill = (() => SkillDefOf.Mining);
			yield return work;
			yield break;
		}
	}
}
