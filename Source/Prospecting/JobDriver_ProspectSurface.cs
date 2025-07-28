using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting;

public class JobDriver_ProspectSurface : JobDriver
{
    public const int BaseTicksBetweenPickHits = 80;

    private const int BaseDamagePerPickHit_NaturalRock = 1;

    private const int BaseDamagePerPickHit_NotNaturalRock = 1;

    private const float MinProspectSpeedFactorForNPCs = 0.6f;

    private readonly DesignationDef designation = ProspectDef.Prospect;

    private readonly int NumResultTicks = 960;

    private Effecter effecter;

    private int ticksTillResult = 960;

    private int ticksToPickHit = -1000;

    private Thing ProspectTarget => job.GetTarget(TargetIndex.A).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var pawn1 = pawn;
        LocalTargetInfo target = ProspectTarget;
        var job1 = job;
        return pawn1.Reserve(target, job1, 1, -1, null, errorOnFailed);
    }

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
            effecter ??= EffecterDefOf.Mine.Spawn();

            effecter.Trigger(actor, prospectTarget);
            var num = 1;
            if (prospectTarget is not Mineable mineable || prospectTarget.HitPoints > num)
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
        prospect.WithProgressBar(TargetIndex.A, () => 1f - (ticksTillResult / (float)NumResultTicks));
        prospect.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        prospect.activeSkill = () => SkillDefOf.Mining;
        yield return prospect;
    }

    private void ResetTicksToPickHit()
    {
        var num = pawn.GetStatValue(StatDefOf.MiningSpeed);
        if (num < 0.6f && pawn.Faction != Faction.OfPlayer)
        {
            num = 0.6f;
        }

        ticksToPickHit = (int)Math.Round(80f / num);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksToPickHit, "ticksToPickHit");
        Scribe_Values.Look(ref ticksTillResult, "ticksTillResult");
    }
}