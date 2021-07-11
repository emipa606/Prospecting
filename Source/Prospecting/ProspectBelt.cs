﻿using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Prospecting
{
    // Token: 0x0200001F RID: 31
    public class ProspectBelt : Apparel
    {
        // Token: 0x0400002D RID: 45
        [NoTranslate] private readonly string BeltIcon = "Things/Special/PrsProspectBelt/PrsProspectBelt";

        // Token: 0x0600007A RID: 122 RVA: 0x00004C66 File Offset: 0x00002E66
        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            var unused = Wearer;
            if (Wearer == null)
            {
                yield break;
            }

            if (Find.Selector.SingleSelectedThing != Wearer)
            {
                yield break;
            }

            string text = "Prospecting.BeltUse".Translate();
            string desc = "Prospecting.BeltDesc".Translate();
            yield return new Command_Action
            {
                defaultLabel = text,
                defaultDesc = desc,
                icon = ContentFinder<Texture2D>.Get(BeltIcon),
                hotKey = KeyBindingDefOf.Misc1,
                action = delegate { DoBeltSelect(Wearer); }
            };
        }

        // Token: 0x0600007B RID: 123 RVA: 0x00004C78 File Offset: 0x00002E78
        public void DoBeltSelect(Pawn p)
        {
            var list = new List<FloatMenuOption>();
            string text = "Prospecting.BeltDoNothing".Translate();
            list.Add(new FloatMenuOption(text, delegate { PrsUseBelt(p, false); }, MenuOptionPriority.Default, null,
                null, 29f));
            text = "Prospecting.BeltUseBelt".Translate(def.label.CapitalizeFirst());
            list.Add(new FloatMenuOption(text, delegate { PrsUseBelt(p, true); }, MenuOptionPriority.Default, null,
                null, 29f, rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + ((rect.height - 24f) / 2f), def)));
            Find.WindowStack.Add(new FloatMenu(list));
        }

        // Token: 0x0600007C RID: 124 RVA: 0x00004D34 File Offset: 0x00002F34
        public void PrsUseBelt(Pawn p, bool Using)
        {
            if (!Using || p == null)
            {
                return;
            }

            if (def.defName == "PrsProspectBelt")
            {
                ChkPrsBelt(p, out var Reason, out var Passed);
                if (Passed)
                {
                    TakeProspectBeltJob(p);
                    return;
                }

                Messages.Message("Prospecting.BeltReasonPrefix".Translate(Reason), p, MessageTypeDefOf.NeutralEvent,
                    false);
            }
            else
            {
                Log.Message(
                    string.Concat("ERR: Belt Worn item def not found for ", def.label.CapitalizeFirst(), " : (",
                        def.defName, ")"));
            }
        }

        // Token: 0x0600007D RID: 125 RVA: 0x00004DEC File Offset: 0x00002FEC
        public static void ChkPrsBelt(Pawn p, out string Reason, out bool Passed)
        {
            Reason = null;
            if (!p.RaceProps.Humanlike)
            {
                Passed = false;
                Reason = "Prospecting.NotHumanLike".Translate(p.LabelShort.CapitalizeFirst());
                return;
            }

            if (p.skills.GetSkill(SkillDefOf.Mining).Level < 5)
            {
                Passed = false;
                Reason = "Prospecting.LackSkill".Translate(p.LabelShort.CapitalizeFirst());
                return;
            }

            Passed = true;
        }

        // Token: 0x0600007E RID: 126 RVA: 0x00004E7C File Offset: 0x0000307C
        public static void TakeProspectBeltJob(Pawn p)
        {
            if (p.CurJob != null)
            {
                var jobs = p.jobs;
                jobs?.ClearQueuedJobs();

                p.jobs.EndCurrentJob(JobCondition.InterruptForced, false);
            }

            var newJob = new Job(ProspectDef.ProspectBelt, p.Position) {expiryInterval = 720};
            p.jobs.TryTakeOrderedJob(newJob, JobTag.Misc);
        }

        // Token: 0x0600007F RID: 127 RVA: 0x00004EE4 File Offset: 0x000030E4
        public static void DoPrsProspectBelt(Pawn p)
        {
            if (p?.Map == null || !p.Spawned || p.Downed)
            {
                return;
            }

            string pname = "Prospecting.AProspector".Translate().CapitalizeFirst();
            if (p.LabelShort != null)
            {
                pname = p.LabelShort;
            }

            string FoundMsg = "Prospecting.BeltNotFoundRes".Translate(pname);
            var msgTypeDef = MessageTypeDefOf.NeutralEvent;
            if (ProspectingGenDeep.ProspectSuccess(p, out var radius) &&
                ProspectingGenDeep.DoProspectFind(p, p.Map, radius, out var prospect, out var prospectDef) &&
                prospect != IntVec3.Zero && prospectDef != null)
            {
                var BPdef = DefDatabase<ThingDef>.GetNamed("PrsProspectMarker");
                if (BPdef != null)
                {
                    var thing = ThingMaker.MakeThing(BPdef);
                    if (thing != null)
                    {
                        var newThing = GenSpawn.Spawn(thing, prospect, p.Map, Rot4.South);
                        newThing?.SetFactionDirect(Faction.OfPlayer);
                    }
                }

                string label = "Prospecting.BeltFoundLabel".Translate();
                string text = "Prospecting.BeltFoundRes".Translate(pname, prospectDef.label);
                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent,
                    new LookTargets(prospect, p.Map));
                return;
            }

            Messages.Message(FoundMsg, p, msgTypeDef, false);
        }

        // Token: 0x06000080 RID: 128 RVA: 0x00005060 File Offset: 0x00003260
        public static bool IsWearingProspectBelt(Pawn p)
        {
            if (p.apparel.WornApparelCount <= 0)
            {
                return false;
            }

            var apparels = p.apparel.WornApparel;
            if (apparels.Count <= 0)
            {
                return false;
            }

            using var enumerator = apparels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is ProspectBelt)
                {
                    return true;
                }
            }

            return false;
        }
    }
}