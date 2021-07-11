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
        // Token: 0x04000025 RID: 37
        public const int BaseTicksBetweenPickHits = 80;

        // Token: 0x04000026 RID: 38
        private const int BaseDamagePerPickHit_NaturalRock = 1;

        // Token: 0x04000027 RID: 39
        private const int BaseDamagePerPickHit_NotNaturalRock = 1;

        // Token: 0x04000028 RID: 40
        private const float MinProspectSpeedFactorForNPCs = 0.6f;

        // Token: 0x04000029 RID: 41
        private readonly DesignationDef designation = ProspectDef.Prospect;

        // Token: 0x04000023 RID: 35
        private readonly int NumResultTicks = 960;

        // Token: 0x04000024 RID: 36
        private Effecter effecter;

        // Token: 0x04000022 RID: 34
        private int ticksTillResult = 960;

        // Token: 0x04000021 RID: 33
        private int ticksToPickHit = -1000;

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000061 RID: 97 RVA: 0x000043D8 File Offset: 0x000025D8
        private Thing ProspectTarget => job.GetTarget(TargetIndex.A).Thing;

        // Token: 0x06000062 RID: 98 RVA: 0x000043FC File Offset: 0x000025FC
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var pawn1 = pawn;
            LocalTargetInfo target = ProspectTarget;
            var job1 = job;
            return pawn1.Reserve(target, job1, 1, -1, null, errorOnFailed);
        }

        // Token: 0x06000063 RID: 99 RVA: 0x0000442F File Offset: 0x0000262F
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnCellMissingDesignation(TargetIndex.A, designation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            var prospect = new Toil();
            prospect.tickAction = delegate
            {
                var actor = prospect.actor;
                var prospectTarget = ProspectTarget;
                if (ticksToPickHit < -80)
                {
                    ResetTicksToPickHit();
                }

                if (actor.skills != null && (prospectTarget.Faction != actor.Faction || actor.Faction == null))
                {
                    actor.skills.Learn(SkillDefOf.Mining, 0.07f);
                }

                ticksToPickHit--;
                ticksTillResult--;
                if (ticksTillResult <= 0)
                {
                    ProspectResults.CheckProspectResult(actor, prospectTarget.Position);
                    ProspectResults.RemoveProspectDesig(actor.Map, prospectTarget.Position);
                    EndJobWith(JobCondition.Succeeded);
                }

                if (ticksToPickHit > 0)
                {
                    return;
                }

                var position = prospectTarget.Position;
                if (effecter == null)
                {
                    effecter = EffecterDefOf.Mine.Spawn();
                }

                effecter.Trigger(actor, prospectTarget);
                var num = 1;
                if (!(prospectTarget is Mineable mineable) || prospectTarget.HitPoints > num)
                {
                    var mining = DamageDefOf.Mining;
                    float amount = num;
                    var actor2 = prospect.actor;
                    var dinfo = new DamageInfo(mining, amount, 0f, -1f, actor2);
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
                    if (pawn.Faction != Faction.OfPlayer)
                    {
                        var thingList = position.GetThingList(Map);
                        foreach (var thing in thingList)
                        {
                            thing.SetForbidden(true, false);
                        }
                    }

                    if (pawn.Faction == Faction.OfPlayer &&
                        MineStrikeManager.MineableIsVeryValuable(prospectTarget.def))
                    {
                        TaleRecorder.RecordTale(TaleDefOf.MinedValuable, pawn,
                            prospectTarget.def.building.mineableThing);
                    }

                    if (pawn.Faction == Faction.OfPlayer &&
                        MineStrikeManager.MineableIsValuable(prospectTarget.def) && !pawn.Map.IsPlayerHome)
                    {
                        TaleRecorder.RecordTale(TaleDefOf.CaravanRemoteMining, pawn,
                            prospectTarget.def.building.mineableThing);
                    }

                    ReadyForNextToil();
                    return;
                }

                ResetTicksToPickHit();
            };
            prospect.defaultCompleteMode = ToilCompleteMode.Never;
            prospect.WithProgressBar(TargetIndex.A, () => 1f - ((float) ticksTillResult / (float) NumResultTicks));
            prospect.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            prospect.activeSkill = () => SkillDefOf.Mining;
            yield return prospect;
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00004440 File Offset: 0x00002640
        private void ResetTicksToPickHit()
        {
            var num = pawn.GetStatValue(StatDefOf.MiningSpeed);
            if (num < 0.6f && pawn.Faction != Faction.OfPlayer)
            {
                num = 0.6f;
            }

            ticksToPickHit = (int) Math.Round(80f / num);
        }

        // Token: 0x06000065 RID: 101 RVA: 0x00004493 File Offset: 0x00002693
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToPickHit, "ticksToPickHit");
            Scribe_Values.Look(ref ticksTillResult, "ticksTillResult");
        }
    }
}