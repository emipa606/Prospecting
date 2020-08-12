using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
	// Token: 0x02000015 RID: 21
	public class JobDriver_ManualDrillMine : JobDriver
	{
		// Token: 0x06000054 RID: 84 RVA: 0x0000426C File Offset: 0x0000246C
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo targetA = this.job.targetA;
			Job job = this.job;
			return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000429F File Offset: 0x0000249F
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.A);
			this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Uninstall);
			this.FailOn(delegate()
			{
				CompManualDrill compManualDrill = this.job.targetA.Thing.TryGetComp<CompManualDrill>();
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
				ThingDef def;
				IntVec3 nextCell;
				bool value = ManualDrillUtility.DrillCanGetToCount(this.job.targetA.Thing as Building, compManualDrill.MDProps.shallowReach, 9, out def, out nextCell) > 0;
				bool baseRock = false;
				Thing thing = this.job.targetA.Thing;
				if (((thing != null) ? thing.Map : null) != null)
				{
					baseRock = (DeepDrillUtility.GetBaseResource(this.job.targetA.Thing.Map, this.job.targetA.Thing.TrueCenter().ToIntVec3()) != null);
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
			Toil work = new Toil();
			work.tickAction = delegate()
			{
				Pawn actor = work.actor;
				Building building = (Building)actor.CurJob.targetA.Thing;
				building.GetComp<CompManualDrill>().ManualDrillWorkDone(actor, building);
				actor.skills.Learn(SkillDefOf.Mining, 0.07f, false);
			};
			work.defaultCompleteMode = ToilCompleteMode.Never;
			if (Controller.Settings.AllowManualSound)
			{
				work.WithEffect(ProspectDef.PrsManualDrillEff, TargetIndex.A);
			}
			work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			work.activeSkill = (() => SkillDefOf.Mining);
			yield return work;
			yield break;
		}
	}
}
