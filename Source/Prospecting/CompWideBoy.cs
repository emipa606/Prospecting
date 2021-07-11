using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
    // Token: 0x02000008 RID: 8
    public class CompWideBoy : ThingComp
    {
        // Token: 0x0400001B RID: 27
        [NoTranslate] private readonly string BoostTogglePath = "Things/Special/BoostToggle";

        // Token: 0x0400001A RID: 26
        [NoTranslate] private readonly string RockTogglePath = "Things/Special/RockToggle";

        // Token: 0x04000017 RID: 23
        [NoTranslate] private readonly string SpanImageOne = "Things/Special/SpanImageOne";

        // Token: 0x04000019 RID: 25
        [NoTranslate] private readonly string SpanImageThree = "Things/Special/SpanImageThree";

        // Token: 0x04000018 RID: 24
        [NoTranslate] private readonly string SpanImageTwo = "Things/Special/SpanImageTwo";

        // Token: 0x04000014 RID: 20
        public int lastBreakCheck;

        // Token: 0x04000015 RID: 21
        public int lastDriller;

        // Token: 0x04000013 RID: 19
        public int lastSparkTick;

        // Token: 0x04000011 RID: 17
        public bool mineRock;

        // Token: 0x04000016 RID: 22
        public bool MiningResources = true;

        // Token: 0x04000010 RID: 16
        public int span = 49;

        // Token: 0x04000012 RID: 18
        public bool WBBoost;

        // Token: 0x0600001D RID: 29 RVA: 0x00003284 File Offset: 0x00001484
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

        // Token: 0x0600001E RID: 30 RVA: 0x00003316 File Offset: 0x00001516
        public override void CompTick()
        {
            base.CompTick();
            CheckBoost(this, WBBoost);
            CheckResources(this);
        }

        // Token: 0x0600001F RID: 31 RVA: 0x00003334 File Offset: 0x00001534
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

        // Token: 0x06000020 RID: 32 RVA: 0x000033E8 File Offset: 0x000015E8
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

        // Token: 0x06000021 RID: 33 RVA: 0x00003480 File Offset: 0x00001680
        public bool IsDrillActive(Thing WideBoy)
        {
            var compDeep = WideBoy.TryGetComp<CompDeepDrill>();
            return compDeep != null && compDeep.UsedLastTick() && lastDriller > 0;
        }

        // Token: 0x06000022 RID: 34 RVA: 0x000034AC File Offset: 0x000016AC
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

        // Token: 0x06000023 RID: 35 RVA: 0x00003518 File Offset: 0x00001718
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
                            if (thing is not Pawn || thing.thingIDNumber != lastDriller)
                            {
                                continue;
                            }

                            miningSkill = Math.Max(0,
                                Math.Min(20, (thing as Pawn).skills.GetSkill(SkillDefOf.Mining).Level));
                            break;
                        }
                    }
                }
            }

            var breakChance = 20 - (int) (miningSkill / 2f);
            if (ProspectingUtility.Rnd100() >= breakChance || WideBoy.IsBrokenDown())
            {
                return;
            }

            var compBreak = WideBoy.TryGetComp<CompBreakdownable>();
            compBreak?.DoBreakdown();
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00003648 File Offset: 0x00001848
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

        // Token: 0x06000025 RID: 37 RVA: 0x00003658 File Offset: 0x00001858
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

        // Token: 0x06000026 RID: 38 RVA: 0x0000372C File Offset: 0x0000192C
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

        // Token: 0x06000027 RID: 39 RVA: 0x00003784 File Offset: 0x00001984
        public void ToggleMineRock(bool flag, CompWideBoy cwb)
        {
            cwb.mineRock = !flag;
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00003790 File Offset: 0x00001990
        public void ToggleBoost(bool flag, CompWideBoy cwb)
        {
            cwb.WBBoost = !flag;
        }

        // Token: 0x06000029 RID: 41 RVA: 0x0000379C File Offset: 0x0000199C
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

        // Token: 0x0600002A RID: 42 RVA: 0x0000380E File Offset: 0x00001A0E
        public override void PostDraw()
        {
            if (parent != null && parent.Spawned && !MiningResources)
            {
                parent.Map.overlayDrawer.DrawOverlay(parent, OverlayTypes.QuestionMark);
            }
        }

        // Token: 0x0600002B RID: 43 RVA: 0x0000384C File Offset: 0x00001A4C
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
}