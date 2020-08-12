using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Prospecting
{
	// Token: 0x0200001F RID: 31
	public class ProspectBelt : Apparel
	{
		// Token: 0x0600007A RID: 122 RVA: 0x00004C66 File Offset: 0x00002E66
		public override IEnumerable<Gizmo> GetWornGizmos()
		{
			Pawn wearer = base.Wearer;
			if (base.Wearer != null)
			{
				if (Find.Selector.SingleSelectedThing == base.Wearer)
				{
					string text = "Prospecting.BeltUse".Translate();
					string desc = "Prospecting.BeltDesc".Translate();
					yield return new Command_Action
					{
						defaultLabel = text,
						defaultDesc = desc,
						icon = ContentFinder<Texture2D>.Get(this.BeltIcon, true),
						hotKey = KeyBindingDefOf.Misc1,
						action = delegate()
						{
							this.DoBeltSelect(base.Wearer);
						}
					};
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004C78 File Offset: 0x00002E78
		public void DoBeltSelect(Pawn p)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			string text = "Prospecting.BeltDoNothing".Translate();
			list.Add(new FloatMenuOption(text, delegate()
			{
				this.PrsUseBelt(p, false);
			}, MenuOptionPriority.Default, null, null, 29f, null, null));
			text = "Prospecting.BeltUseBelt".Translate(this.def.label.CapitalizeFirst());
			list.Add(new FloatMenuOption(text, delegate()
			{
				this.PrsUseBelt(p, true);
			}, MenuOptionPriority.Default, null, null, 29f, (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, this.def), null));
			Find.WindowStack.Add(new FloatMenu(list));
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004D34 File Offset: 0x00002F34
		public void PrsUseBelt(Pawn p, bool Using)
		{
			if (Using && p != null)
			{
				if (this.def.defName == "PrsProspectBelt")
				{
					string Reason;
					bool Passed;
					ProspectBelt.ChkPrsBelt(p, out Reason, out Passed);
					if (Passed)
					{
						ProspectBelt.TakeProspectBeltJob(p);
						return;
					}
					Messages.Message("Prospecting.BeltReasonPrefix".Translate(Reason), p, MessageTypeDefOf.NeutralEvent, false);
					return;
				}
				else
				{
					Log.Message(string.Concat(new string[]
					{
						"ERR: Belt Worn item def not found for ",
						this.def.label.CapitalizeFirst(),
						" : (",
						this.def.defName,
						")"
					}), false);
				}
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004DEC File Offset: 0x00002FEC
		public static void ChkPrsBelt(Pawn p, out string Reason, out bool Passed)
		{
			Reason = null;
			if (!p.RaceProps.Humanlike)
			{
				Passed = false;
				Reason = "Prospecting.NotHumanLike".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
				return;
			}
			if (p.skills.GetSkill(SkillDefOf.Mining).Level < 5)
			{
				Passed = false;
				Reason = "Prospecting.LackSkill".Translate((p != null) ? p.LabelShort.CapitalizeFirst() : null);
				return;
			}
			Passed = true;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004E7C File Offset: 0x0000307C
		public static void TakeProspectBeltJob(Pawn p)
		{
			if (p.CurJob != null)
			{
				if (p != null)
				{
					Pawn_JobTracker jobs = p.jobs;
					if (jobs != null)
					{
						jobs.ClearQueuedJobs(true);
					}
				}
				p.jobs.EndCurrentJob(JobCondition.InterruptForced, false, true);
			}
			Job newJob = new Job(ProspectDef.ProspectBelt, p.Position);
			newJob.expiryInterval = 720;
			p.jobs.TryTakeOrderedJob(newJob, JobTag.Misc);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004EE4 File Offset: 0x000030E4
		public static void DoPrsProspectBelt(Pawn p)
		{
			if (p != null && ((p != null) ? p.Map : null) != null && p.Spawned && !p.Downed)
			{
				string pname = "Prospecting.AProspector".Translate().CapitalizeFirst();
				if (p != null && p.LabelShort != null)
				{
					pname = p.LabelShort;
				}
				string FoundMsg = "Prospecting.BeltNotFoundRes".Translate(pname);
				MessageTypeDef msgTypeDef = MessageTypeDefOf.NeutralEvent;
				float radius;
				IntVec3 prospect;
				ThingDef prospectDef;
				if (ProspectingGenDeep.ProspectSuccess(p, out radius) && ProspectingGenDeep.DoProspectFind(p, p.Map, radius, out prospect, out prospectDef) && prospect != IntVec3.Zero && prospectDef != null)
				{
					ThingDef BPdef = DefDatabase<ThingDef>.GetNamed("PrsProspectMarker", true);
					if (BPdef != null)
					{
						Thing thing = ThingMaker.MakeThing(BPdef, null);
						if (thing != null)
						{
							Thing newThing = GenSpawn.Spawn(thing, prospect, p.Map, Rot4.South, WipeMode.Vanish, false);
							if (newThing != null)
							{
								newThing.SetFactionDirect(Faction.OfPlayer);
							}
						}
					}
					string label = "Prospecting.BeltFoundLabel".Translate();
					string text = "Prospecting.BeltFoundRes".Translate(pname, prospectDef.label);
					Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, new LookTargets(prospect, p.Map), null, null, null, null);
					return;
				}
				Messages.Message(FoundMsg, p, msgTypeDef, false);
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005060 File Offset: 0x00003260
		public static bool IsWearingProspectBelt(Pawn p)
		{
			if (p.apparel.WornApparelCount > 0)
			{
				List<Apparel> apparels = p.apparel.WornApparel;
				if (apparels.Count > 0)
				{
					using (List<Apparel>.Enumerator enumerator = apparels.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current is ProspectBelt)
							{
								return true;
							}
						}
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x0400002D RID: 45
		[NoTranslate]
		private string BeltIcon = "Things/Special/PrsProspectBelt/PrsProspectBelt";
	}
}
