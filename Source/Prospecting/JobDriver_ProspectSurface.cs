using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
	// Token: 0x02000019 RID: 25
	public class JobDriver_ProspectSurface : JobDriver
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000061 RID: 97 RVA: 0x000043D8 File Offset: 0x000025D8
		private Thing ProspectTarget
		{
			get
			{
				return this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000043FC File Offset: 0x000025FC
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo target = this.ProspectTarget;
			Job job = this.job;
			return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000442F File Offset: 0x0000262F
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnCellMissingDesignation(TargetIndex.A, this.designation);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			Toil prospect = new Toil();
			prospect.tickAction = delegate()
			{
				Pawn actor = prospect.actor;
				Thing prospectTarget = this.ProspectTarget;
				if (this.ticksToPickHit < -80)
				{
					this.ResetTicksToPickHit();
				}
				if (actor.skills != null && (prospectTarget.Faction != actor.Faction || actor.Faction == null))
				{
					actor.skills.Learn(SkillDefOf.Mining, 0.07f, false);
				}
				this.ticksToPickHit--;
				this.ticksTillResult--;
				if (this.ticksTillResult <= 0)
				{
					ProspectResults.CheckProspectResult(actor, prospectTarget.Position);
					ProspectResults.RemoveProspectDesig(actor.Map, prospectTarget.Position);
					this.EndJobWith(JobCondition.Succeeded);
				}
				if (this.ticksToPickHit <= 0)
				{
					IntVec3 position = prospectTarget.Position;
					if (this.effecter == null)
					{
						this.effecter = EffecterDefOf.Mine.Spawn();
					}
					this.effecter.Trigger(actor, prospectTarget);
					int num = (!prospectTarget.def.building.isNaturalRock) ? 1 : 1;
					Mineable mineable = prospectTarget as Mineable;
					if (mineable == null || prospectTarget.HitPoints > num)
					{
						DamageDef mining = DamageDefOf.Mining;
						float amount = (float)num;
						Pawn actor2 = prospect.actor;
						DamageInfo dinfo = new DamageInfo(mining, amount, 0f, -1f, actor2, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
						prospectTarget.TakeDamage(dinfo);
					}
					else
					{
						mineable.Notify_TookMiningDamage(prospectTarget.HitPoints, prospect.actor);
						mineable.HitPoints = 0;
						mineable.DestroyMined(actor);
					}
					if (prospectTarget.Destroyed)
					{
						actor.Map.mineStrikeManager.CheckStruckOre(position, prospectTarget.def, actor);
						actor.records.Increment(RecordDefOf.CellsMined);
						if (this.pawn.Faction != Faction.OfPlayer)
						{
							List<Thing> thingList = position.GetThingList(this.Map);
							for (int i = 0; i < thingList.Count; i++)
							{
								thingList[i].SetForbidden(true, false);
							}
						}
						if (this.pawn.Faction == Faction.OfPlayer && MineStrikeManager.MineableIsVeryValuable(prospectTarget.def))
						{
							TaleRecorder.RecordTale(TaleDefOf.MinedValuable, new object[]
							{
								this.pawn,
								prospectTarget.def.building.mineableThing
							});
						}
						if (this.pawn.Faction == Faction.OfPlayer && MineStrikeManager.MineableIsValuable(prospectTarget.def) && !this.pawn.Map.IsPlayerHome)
						{
							TaleRecorder.RecordTale(TaleDefOf.CaravanRemoteMining, new object[]
							{
								this.pawn,
								prospectTarget.def.building.mineableThing
							});
						}
						this.ReadyForNextToil();
						return;
					}
					this.ResetTicksToPickHit();
				}
			};
			prospect.defaultCompleteMode = ToilCompleteMode.Never;
			prospect.WithProgressBar(TargetIndex.A, () => 1f - (float)this.ticksTillResult / (float)this.NumResultTicks, false, -0.5f);
			prospect.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			prospect.activeSkill = (() => SkillDefOf.Mining);
			yield return prospect;
			yield break;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004440 File Offset: 0x00002640
		private void ResetTicksToPickHit()
		{
			float num = this.pawn.GetStatValue(StatDefOf.MiningSpeed, true);
			if (num < 0.6f && this.pawn.Faction != Faction.OfPlayer)
			{
				num = 0.6f;
			}
			this.ticksToPickHit = (int)Math.Round((double)(80f / num));
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004493 File Offset: 0x00002693
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.ticksToPickHit, "ticksToPickHit", 0, false);
			Scribe_Values.Look<int>(ref this.ticksTillResult, "ticksTillResult", 0, false);
		}

		// Token: 0x04000021 RID: 33
		private int ticksToPickHit = -1000;

		// Token: 0x04000022 RID: 34
		private int ticksTillResult = 960;

		// Token: 0x04000023 RID: 35
		private int NumResultTicks = 960;

		// Token: 0x04000024 RID: 36
		private Effecter effecter;

		// Token: 0x04000025 RID: 37
		public const int BaseTicksBetweenPickHits = 80;

		// Token: 0x04000026 RID: 38
		private const int BaseDamagePerPickHit_NaturalRock = 1;

		// Token: 0x04000027 RID: 39
		private const int BaseDamagePerPickHit_NotNaturalRock = 1;

		// Token: 0x04000028 RID: 40
		private const float MinProspectSpeedFactorForNPCs = 0.6f;

		// Token: 0x04000029 RID: 41
		private DesignationDef designation = ProspectDef.Prospect;
	}
}
