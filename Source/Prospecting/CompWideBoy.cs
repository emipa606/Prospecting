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
		// Token: 0x0600001D RID: 29 RVA: 0x00003284 File Offset: 0x00001484
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.span, "span", 49, false);
			Scribe_Values.Look<bool>(ref this.mineRock, "mineRock", false, false);
			Scribe_Values.Look<bool>(ref this.WBBoost, "WBBoost", false, false);
			Scribe_Values.Look<int>(ref this.lastSparkTick, "lastSparkTick", 0, false);
			Scribe_Values.Look<int>(ref this.lastBreakCheck, "lastBreakCheck", 0, false);
			Scribe_Values.Look<int>(ref this.lastDriller, "lastDriller", 0, false);
			Scribe_Values.Look<bool>(ref this.MiningResources, "MiningResources", true, false);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003316 File Offset: 0x00001516
		public override void CompTick()
		{
			base.CompTick();
			this.CheckBoost(this, this.WBBoost);
			this.CheckResources(this);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003334 File Offset: 0x00001534
		public void CheckResources(CompWideBoy compWB)
		{
			if (compWB.parent.IsHashIntervalTick(75))
			{
				compWB.MiningResources = !this.NoMiningResources(compWB);
				if (!compWB.MiningResources)
				{
					CompPowerTrader compPower = compWB.parent.TryGetComp<CompPowerTrader>();
					if (compPower != null && compPower.PowerOn)
					{
						CompFlickable compFlick = compWB.parent.TryGetComp<CompFlickable>();
						if (compFlick != null && compFlick.SwitchIsOn)
						{
							compFlick.SwitchIsOn = false;
							AccessTools.Field(typeof(CompFlickable), "wantSwitchOn").SetValue(compFlick, false);
							Messages.Message("Prospecting.WideBoyZeroLeft".Translate(), this.parent, MessageTypeDefOf.NeutralEvent, false);
						}
					}
				}
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000033E8 File Offset: 0x000015E8
		public void CheckBoost(CompWideBoy compWB, bool boost)
		{
			if (compWB != null)
			{
				Thing drill = compWB.parent;
				if (drill != null && drill.Spawned && ((drill != null) ? drill.Map : null) != null && !drill.IsBrokenDown())
				{
					CompPowerTrader compPow = drill.TryGetComp<CompPowerTrader>();
					if (compPow != null && compPow.PowerOn)
					{
						float powUse = compPow.Props.basePowerConsumption;
						if (boost)
						{
							powUse *= 1.33f;
						}
						float newPowUse = -1f * powUse;
						if (compPow.PowerOutput != newPowUse)
						{
							compPow.PowerOutput = newPowUse;
						}
						if (boost && this.IsDrillActive(drill))
						{
							this.DoSparks(drill);
							this.CheckBreakdown(drill);
						}
					}
				}
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003480 File Offset: 0x00001680
		public bool IsDrillActive(Thing WideBoy)
		{
			CompDeepDrill compDeep = WideBoy.TryGetComp<CompDeepDrill>();
			return compDeep != null && compDeep.UsedLastTick() && this.lastDriller > 0;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000034AC File Offset: 0x000016AC
		public void DoSparks(Thing WideBoy)
		{
			if (Find.TickManager.TicksGame > this.lastSparkTick + ProspectingUtility.RndBits(95, 165))
			{
				this.lastSparkTick = Find.TickManager.TicksGame;
				int NumSparks = ProspectingUtility.RndBits(1, 3);
				for (int i = 0; i < NumSparks; i++)
				{
					MoteMaker.ThrowMicroSparks(WideBoy.Position.ToVector3(), WideBoy.Map);
				}
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003518 File Offset: 0x00001718
		public void CheckBreakdown(Thing WideBoy)
		{
			if (Find.TickManager.TicksGame > this.lastBreakCheck)
			{
				this.lastBreakCheck = Find.TickManager.TicksGame + ProspectingUtility.RndBits(7500, 90000);
				int miningSkill = 0;
				if (this.lastDriller > 0)
				{
					IntVec3 interactCell = WideBoy.InteractionCell;
					if (((WideBoy != null) ? WideBoy.Map : null) != null)
					{
						List<Thing> list = interactCell.GetThingList(WideBoy.Map);
						if (list.Count > 0)
						{
							foreach (Thing thing in list)
							{
								if (thing is Pawn && thing.thingIDNumber == this.lastDriller)
								{
									miningSkill = Math.Max(0, Math.Min(20, (thing as Pawn).skills.GetSkill(SkillDefOf.Mining).Level));
									break;
								}
							}
						}
					}
				}
				int breakChance = 20 - (int)((float)miningSkill / 2f);
				if (ProspectingUtility.Rnd100() < breakChance && !WideBoy.IsBrokenDown())
				{
					CompBreakdownable compBreak = WideBoy.TryGetComp<CompBreakdownable>();
					if (compBreak != null)
					{
						compBreak.DoBreakdown();
					}
				}
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003648 File Offset: 0x00001848
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo item in base.CompGetGizmosExtra())
			{
				yield return item;
			}
			IEnumerator<Gizmo> enumerator = null;
			int num = this.span;
			string spanPath;
			if (num != 9)
			{
				if (num != 25)
				{
					if (num != 49)
					{
						spanPath = this.SpanImageThree;
					}
					else
					{
						spanPath = this.SpanImageThree;
					}
				}
				else
				{
					spanPath = this.SpanImageTwo;
				}
			}
			else
			{
				spanPath = this.SpanImageOne;
			}
			yield return new Command_Action
			{
				action = delegate()
				{
					this.SetSpanWB(this);
				},
				defaultLabel = "Prospecting.SpanLabel".Translate(),
				defaultDesc = "Prospecting.SpanDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(spanPath, true)
			};
			yield return new Command_Toggle
			{
				icon = ContentFinder<Texture2D>.Get(this.RockTogglePath, true),
				defaultLabel = "Prospecting.RockToggle".Translate(),
				defaultDesc = "Prospecting.RockDesc".Translate(),
				isActive = (() => this.mineRock),
				toggleAction = delegate()
				{
					this.ToggleMineRock(this.mineRock, this);
				}
			};
			yield return new Command_Toggle
			{
				icon = ContentFinder<Texture2D>.Get(this.BoostTogglePath, true),
				defaultLabel = "Prospecting.BoostToggle".Translate(),
				defaultDesc = "Prospecting.BoostDesc".Translate(),
				isActive = (() => this.WBBoost),
				toggleAction = delegate()
				{
					this.ToggleBoost(this.WBBoost, this);
				}
			};
			yield break;
			yield break;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003658 File Offset: 0x00001858
		public void SetSpanWB(CompWideBoy cwb)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			string text = "Prospecting.SpanDoNothing".Translate();
			list.Add(new FloatMenuOption(text, delegate()
			{
				this.SetSpanValue(cwb, 0);
			}, MenuOptionPriority.Default, null, null, 29f, null, null));
			for (int i = 0; i < 3; i++)
			{
				int spanWidth = i + 1;
				text = "Prospecting.SpanOption".Translate(spanWidth.ToString());
				list.Add(new FloatMenuOption(text, delegate()
				{
					this.SetSpanValue(cwb, spanWidth);
				}, MenuOptionPriority.Default, null, null, 29f, null, null));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}

		// Token: 0x06000026 RID: 38 RVA: 0x0000372C File Offset: 0x0000192C
		public void SetSpanValue(CompWideBoy cwb, int spanValue)
		{
			if (spanValue > 0)
			{
				if (cwb.parent.TryGetComp<CompForbiddable>() != null)
				{
					cwb.parent.SetForbidden(true, true);
				}
				int newSpan = 49;
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
			List<IntVec3> cells = new List<IntVec3>();
			IntVec3 root = this.parent.Position;
			for (int i = 0; i < this.span; i++)
			{
				Map currentMap = Find.CurrentMap;
				if (currentMap != null)
				{
					IntVec3 tempCell = root + GenRadial.ManualRadialPattern[i];
					if (tempCell.InBounds(currentMap))
					{
						cells.Add(tempCell);
					}
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
			if (this.parent != null && this.parent.Spawned && !this.MiningResources)
			{
				this.parent.Map.overlayDrawer.DrawOverlay(this.parent, OverlayTypes.QuestionMark);
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x0000384C File Offset: 0x00001A4C
		public bool NoMiningResources(CompWideBoy compWB)
		{
			if (compWB == null)
			{
				return true;
			}
			CompDeepDrill compDeepDrill = compWB.parent.TryGetComp<CompDeepDrill>();
			if (compDeepDrill != null)
			{
				bool value = compDeepDrill.ValuableResourcesPresent();
				bool baseRock = false;
				ThingWithComps parent = compWB.parent;
				if (((parent != null) ? parent.Map : null) != null)
				{
					baseRock = (DeepDrillUtility.GetBaseResource(compWB.parent.Map, compWB.parent.TrueCenter().ToIntVec3()) != null);
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
			return true;
		}

		// Token: 0x04000010 RID: 16
		public int span = 49;

		// Token: 0x04000011 RID: 17
		public bool mineRock;

		// Token: 0x04000012 RID: 18
		public bool WBBoost;

		// Token: 0x04000013 RID: 19
		public int lastSparkTick;

		// Token: 0x04000014 RID: 20
		public int lastBreakCheck;

		// Token: 0x04000015 RID: 21
		public int lastDriller;

		// Token: 0x04000016 RID: 22
		public bool MiningResources = true;

		// Token: 0x04000017 RID: 23
		[NoTranslate]
		private string SpanImageOne = "Things/Special/SpanImageOne";

		// Token: 0x04000018 RID: 24
		[NoTranslate]
		private string SpanImageTwo = "Things/Special/SpanImageTwo";

		// Token: 0x04000019 RID: 25
		[NoTranslate]
		private string SpanImageThree = "Things/Special/SpanImageThree";

		// Token: 0x0400001A RID: 26
		[NoTranslate]
		private string RockTogglePath = "Things/Special/RockToggle";

		// Token: 0x0400001B RID: 27
		[NoTranslate]
		private string BoostTogglePath = "Things/Special/BoostToggle";
	}
}
