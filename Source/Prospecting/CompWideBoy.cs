using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting;

public class CompWideBoy : ThingComp
{
    [NoTranslate] private readonly string BoostTogglePath = "Things/Special/BoostToggle";

    [NoTranslate] private readonly string RockTogglePath = "Things/Special/RockToggle";

    [NoTranslate] private readonly string SpanImageOne = "Things/Special/SpanImageOne";

    [NoTranslate] private readonly string SpanImageThree = "Things/Special/SpanImageThree";

    [NoTranslate] private readonly string SpanImageTwo = "Things/Special/SpanImageTwo";

    public int lastBreakCheck;

    public int lastDriller;

    public int lastSparkTick;

    public bool mineRock;

    public bool MiningResources = true;

    public int span = 49;

    public bool WBBoost;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref span, "span", 49);
        Scribe_Values.Look(ref mineRock, "mineRock");
        Scribe_Values.Look(ref WBBoost, "WBBoost");
        Scribe_Values.Look(ref lastSparkTick, "lastSparkTick");
        Scribe_Values.Look(ref lastBreakCheck, "lastBreakCheck");
        Scribe_Values.Look(ref lastDriller, "lastDriller");
        Scribe_Values.Look(ref MiningResources, "MiningResources", true);
    }

    public override void CompTick()
    {
        base.CompTick();
        CheckBoost(this, WBBoost);
        CheckResources(this);
    }

    public void CheckResources(CompWideBoy compWB)
    {
        if (!compWB.parent.IsHashIntervalTick(75))
        {
            return;
        }

        compWB.MiningResources = !NoMiningResources(compWB);
        if (compWB.MiningResources)
        {
            return;
        }

        var compPower = compWB.parent.TryGetComp<CompPowerTrader>();
        if (compPower == null || !compPower.PowerOn)
        {
            return;
        }

        var compFlick = compWB.parent.TryGetComp<CompFlickable>();
        if (compFlick == null || !compFlick.SwitchIsOn)
        {
            return;
        }

        compFlick.SwitchIsOn = false;
        AccessTools.Field(typeof(CompFlickable), "wantSwitchOn").SetValue(compFlick, false);
        Messages.Message("Prospecting.WideBoyZeroLeft".Translate(), parent,
            MessageTypeDefOf.NeutralEvent, false);
    }

    public void CheckBoost(CompWideBoy compWB, bool boost)
    {
        Thing drill = compWB?.parent;
        if (drill == null || !drill.Spawned || drill.Map == null || drill.IsBrokenDown())
        {
            return;
        }

        var compPow = drill.TryGetComp<CompPowerTrader>();
        if (compPow == null || !compPow.PowerOn)
        {
            return;
        }

        var powUse = compPow.Props.basePowerConsumption;
        if (boost)
        {
            powUse *= 1.33f;
        }

        var newPowUse = -1f * powUse;
        if (compPow.PowerOutput != newPowUse)
        {
            compPow.PowerOutput = newPowUse;
        }

        if (!boost || !IsDrillActive(drill))
        {
            return;
        }

        DoSparks(drill);
        CheckBreakdown(drill);
    }

    public bool IsDrillActive(Thing WideBoy)
    {
        var compDeep = WideBoy.TryGetComp<CompDeepDrill>();
        return compDeep != null && compDeep.UsedLastTick() && lastDriller > 0;
    }

    public void DoSparks(Thing WideBoy)
    {
        if (Find.TickManager.TicksGame <= lastSparkTick + ProspectingUtility.RndBits(95, 165))
        {
            return;
        }

        lastSparkTick = Find.TickManager.TicksGame;
        var NumSparks = ProspectingUtility.RndBits(1, 3);
        for (var i = 0; i < NumSparks; i++)
        {
            FleckMaker.ThrowMicroSparks(WideBoy.Position.ToVector3(), WideBoy.Map);
        }
    }

    public void CheckBreakdown(Thing WideBoy)
    {
        if (Find.TickManager.TicksGame <= lastBreakCheck)
        {
            return;
        }

        lastBreakCheck = Find.TickManager.TicksGame + ProspectingUtility.RndBits(7500, 90000);
        var miningSkill = 0;
        if (lastDriller > 0)
        {
            var interactCell = WideBoy.InteractionCell;
            if (WideBoy.Map != null)
            {
                var list = interactCell.GetThingList(WideBoy.Map);
                if (list.Count > 0)
                {
                    foreach (var thing in list)
                    {
                        if (thing is not Pawn pawn || pawn.thingIDNumber != lastDriller)
                        {
                            continue;
                        }

                        miningSkill = Math.Max(0,
                            Math.Min(20, pawn.skills.GetSkill(SkillDefOf.Mining).Level));
                        break;
                    }
                }
            }
        }

        var breakChance = 20 - (int)(miningSkill / 2f);
        if (ProspectingUtility.Rnd100() >= breakChance || WideBoy.IsBrokenDown())
        {
            return;
        }

        var compBreak = WideBoy.TryGetComp<CompBreakdownable>();
        compBreak?.DoBreakdown();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var item in base.CompGetGizmosExtra())
        {
            yield return item;
        }

        var num = span;
        string spanPath;
        if (num != 9)
        {
            spanPath = num != 25 ? SpanImageThree : SpanImageTwo;
        }
        else
        {
            spanPath = SpanImageOne;
        }

        yield return new Command_Action
        {
            action = delegate { SetSpanWB(this); },
            defaultLabel = "Prospecting.SpanLabel".Translate(),
            defaultDesc = "Prospecting.SpanDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get(spanPath)
        };
        yield return new Command_Toggle
        {
            icon = ContentFinder<Texture2D>.Get(RockTogglePath),
            defaultLabel = "Prospecting.RockToggle".Translate(),
            defaultDesc = "Prospecting.RockDesc".Translate(),
            isActive = () => mineRock,
            toggleAction = delegate { ToggleMineRock(mineRock, this); }
        };
        yield return new Command_Toggle
        {
            icon = ContentFinder<Texture2D>.Get(BoostTogglePath),
            defaultLabel = "Prospecting.BoostToggle".Translate(),
            defaultDesc = "Prospecting.BoostDesc".Translate(),
            isActive = () => WBBoost,
            toggleAction = delegate { ToggleBoost(WBBoost, this); }
        };
    }

    public void SetSpanWB(CompWideBoy cwb)
    {
        var list = new List<FloatMenuOption>();
        string text = "Prospecting.SpanDoNothing".Translate();
        list.Add(new FloatMenuOption(text, delegate { SetSpanValue(cwb, 0); }, MenuOptionPriority.Default, null,
            null, 29f));
        for (var i = 0; i < 3; i++)
        {
            var spanWidth = i + 1;
            text = "Prospecting.SpanOption".Translate(spanWidth.ToString());
            list.Add(new FloatMenuOption(text, delegate { SetSpanValue(cwb, spanWidth); },
                MenuOptionPriority.Default, null, null, 29f));
        }

        Find.WindowStack.Add(new FloatMenu(list));
    }

    public void SetSpanValue(CompWideBoy cwb, int spanValue)
    {
        if (spanValue <= 0)
        {
            return;
        }

        if (cwb.parent.TryGetComp<CompForbiddable>() != null)
        {
            cwb.parent.SetForbidden(true);
        }

        var newSpan = 49;
        switch (spanValue)
        {
            case 1:
                newSpan = 9;
                break;
            case 2:
                newSpan = 25;
                break;
            case 3:
                newSpan = 49;
                break;
        }

        cwb.span = newSpan;
    }

    public void ToggleMineRock(bool flag, CompWideBoy cwb)
    {
        cwb.mineRock = !flag;
    }

    public void ToggleBoost(bool flag, CompWideBoy cwb)
    {
        cwb.WBBoost = !flag;
    }

    public override void PostDrawExtraSelectionOverlays()
    {
        var cells = new List<IntVec3>();
        var root = parent.Position;
        for (var i = 0; i < span; i++)
        {
            var currentMap = Find.CurrentMap;
            if (currentMap == null)
            {
                continue;
            }

            var tempCell = root + GenRadial.ManualRadialPattern[i];
            if (tempCell.InBounds(currentMap))
            {
                cells.Add(tempCell);
            }
        }

        if (cells.Count > 0)
        {
            GenDraw.DrawFieldEdges(cells, Color.cyan);
        }
    }

    public override void PostDraw()
    {
        if (parent is { Spawned: true } && !MiningResources)
        {
            parent.Map.overlayDrawer.DrawOverlay(parent, OverlayTypes.QuestionMark);
        }
    }

    public bool NoMiningResources(CompWideBoy compWB)
    {
        var compDeepDrill = compWB?.parent.TryGetComp<CompDeepDrill>();
        if (compDeepDrill == null)
        {
            return true;
        }

        var value = compDeepDrill.ValuableResourcesPresent();
        var baseRock = false;
        var compWbParent = compWB.parent;
        if (compWbParent?.Map != null)
        {
            baseRock = DeepDrillUtility.GetBaseResource(compWB.parent.Map,
                compWB.parent.TrueCenter().ToIntVec3()) != null;
        }

        if (compWB.mineRock)
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
    }
}