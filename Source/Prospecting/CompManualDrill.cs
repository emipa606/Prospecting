using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
	// Token: 0x02000006 RID: 6
	public class CompManualDrill : ThingComp
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000026C8 File Offset: 0x000008C8
		public static float WorkPerPortionCurrentDifficulty
		{
			get
			{
				return 12000f / Find.Storyteller.difficulty.mineYieldFactor;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000026DF File Offset: 0x000008DF
		public float ProgressToNextPortionPercent
		{
			get
			{
				return this.portionProgress / CompManualDrill.WorkPerPortionCurrentDifficulty;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000026ED File Offset: 0x000008ED
		public float ProgressToProspectingPercent
		{
			get
			{
				return this.prospectProgress / CompManualDrill.WorkPerPortionCurrentDifficulty;
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000026FC File Offset: 0x000008FC
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.maxCount, "maxCount", 0, false);
			Scribe_Values.Look<bool>(ref this.prospected, "prospected", false, false);
			Scribe_Values.Look<float>(ref this.portionProgress, "portionProgress", 0f, false);
			Scribe_Values.Look<float>(ref this.prospectProgress, "prospectProgress", 0f, false);
			Scribe_Values.Look<float>(ref this.portionYieldPct, "portionYieldPct", 0f, false);
			Scribe_Values.Look<float>(ref this.prospectYieldPct, "prospectYieldPct", 0f, false);
			Scribe_Values.Look<int>(ref this.lastUsedTick, "lastusedTick", 0, false);
			Scribe_Values.Look<bool>(ref this.windOk, "windOk", true, false);
			Scribe_Values.Look<float>(ref this.roofFactor, "roofFactor", 1f, false);
			Scribe_Values.Look<float>(ref this.windFactor, "windFactor", 1f, false);
			Scribe_Values.Look<float>(ref this.barrelsAngle, "barrelsAngle", 0f, false);
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000027F1 File Offset: 0x000009F1
		public Building drill
		{
			get
			{
				return this.parent as Building;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000027FE File Offset: 0x000009FE
		public CompProperties_ManualDrill MDProps
		{
			get
			{
				return (CompProperties_ManualDrill)this.props;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000280C File Offset: 0x00000A0C
		public override void CompTick()
		{
			base.CompTick();
			if (this.drill.IsHashIntervalTick(240))
			{
				bool noWind;
				float newroofFactor;
				float newwindFactor;
				ManualDrillUtility.GetPerformFactor(this.drill, out noWind, out newroofFactor, out newwindFactor);
				this.windOk = !noWind;
				this.roofFactor = newroofFactor;
				this.windFactor = newwindFactor;
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000285C File Offset: 0x00000A5C
		public override void PostDeSpawn(Map map)
		{
			this.prospected = false;
			this.maxCount = 0;
			this.prospectProgress = 0f;
			this.prospectYieldPct = 0f;
			this.portionProgress = 0f;
			this.portionYieldPct = 0f;
			this.lastUsedTick = -99999;
			this.windOk = true;
			this.roofFactor = 1f;
			this.windFactor = 1f;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000028CC File Offset: 0x00000ACC
		public void ManualDrillWorkDone(Pawn driller, Building drill)
		{
			bool noWind;
			float newroofFactor;
			float newwindFactor;
			float performFactor = ManualDrillUtility.GetPerformFactor(drill, out noWind, out newroofFactor, out newwindFactor);
			if (!noWind)
			{
				float statValue = driller.GetStatValue(StatDefOf.MiningSpeed, true) / 2f * performFactor;
				this.portionProgress += statValue;
				this.portionYieldPct += statValue * driller.GetStatValue(StatDefOf.MiningYield, true) / CompManualDrill.WorkPerPortionCurrentDifficulty;
			}
			this.lastUsedTick = Find.TickManager.TicksGame;
			if (this.portionProgress > CompManualDrill.WorkPerPortionCurrentDifficulty)
			{
				this.TryManualProducePortion(this.portionYieldPct, drill);
				this.portionProgress = 0f;
				this.portionYieldPct = 0f;
			}
			this.windOk = !noWind;
			this.roofFactor = newroofFactor;
			this.windFactor = newwindFactor;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002990 File Offset: 0x00000B90
		public void ManualProspectWorkDone(Pawn driller, Building drill)
		{
			bool noWind;
			float newroofFactor;
			float newwindFactor;
			float performFactor = ManualDrillUtility.GetPerformFactor(drill, out noWind, out newroofFactor, out newwindFactor);
			if (!noWind)
			{
				float statValue = driller.GetStatValue(StatDefOf.MiningSpeed, true) / 2f * performFactor;
				this.prospectProgress += statValue;
				this.prospectYieldPct += statValue * driller.GetStatValue(StatDefOf.MiningYield, true) / CompManualDrill.WorkPerPortionCurrentDifficulty;
			}
			this.lastUsedTick = Find.TickManager.TicksGame;
			if (this.prospectProgress > CompManualDrill.WorkPerPortionCurrentDifficulty)
			{
				ThingDef foundDef;
				int maxVal;
				if (this.SetMaxCount(drill, 9, out foundDef, out maxVal))
				{
					if (foundDef != null)
					{
						this.TryManualProducePortion(this.prospectYieldPct, drill);
						string labelone = "Prospecting.AProspector".Translate();
						string labeltwo = "Prospecting.Unknown".Translate();
						if (driller.LabelShort != null)
						{
							labelone = driller.LabelShort;
						}
						if (foundDef.label != null)
						{
							labeltwo = foundDef.label;
						}
						Messages.Message("Prospecting.DrillProspect".Translate(labelone) + "Prospecting.ResourceFound".Translate(labeltwo), drill, MessageTypeDefOf.PositiveEvent, true);
					}
					this.maxCount = maxVal;
				}
				this.prospectProgress = 0f;
				this.prospectYieldPct = 0f;
				this.prospected = true;
			}
			this.windOk = !noWind;
			this.roofFactor = newroofFactor;
			this.windFactor = newwindFactor;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002B04 File Offset: 0x00000D04
		private bool SetMaxCount(Building drill, int numCells, out ThingDef resFound, out int MaxValue)
		{
			resFound = null;
			MaxValue = 0;
			bool found = false;
			if (((drill != null) ? drill.Map : null) != null && drill.Spawned)
			{
				IntVec3 drillpoint = drill.TrueCenter().ToIntVec3();
				ManualDrillMapComponent mapComp = drill.Map.GetComponent<ManualDrillMapComponent>();
				if (mapComp != null)
				{
					for (int i = 0; i < numCells; i++)
					{
						IntVec3 pos = drillpoint + GenRadial.RadialPattern[i];
						int MaxValueAt = 0;
						int maxVal;
						bool MaxExists = mapComp.GetValue(pos, out maxVal);
						ThingDef resFoundAt = drill.Map.deepResourceGrid.ThingDefAt(pos);
						if (!MaxExists)
						{
							if (resFoundAt != null)
							{
								MaxValueAt = drill.Map.deepResourceGrid.CountAt(drillpoint);
							}
						}
						else if (resFoundAt != null)
						{
							MaxValueAt = maxVal;
						}
						if (!found && resFoundAt != null)
						{
							resFound = resFoundAt;
							found = true;
						}
						if (MaxValueAt > 0 && resFoundAt != null)
						{
							if (!MaxExists)
							{
								mapComp.SetValue(pos, MaxValueAt);
							}
							MaxValue += MaxValueAt;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002BF0 File Offset: 0x00000DF0
		private void TryManualProducePortion(float yieldPct, Building drill)
		{
			ThingDef resDef;
			int countPresent;
			IntVec3 cell;
			bool nextResource = this.GetNextManualResource(drill, out resDef, out countPresent, out cell);
			if (resDef == null)
			{
				return;
			}
			int num = Mathf.Min(countPresent, (int)Mathf.Max(1f, (float)resDef.deepCountPerPortion / 2f));
			if (nextResource)
			{
				int newCount = drill.Map.deepResourceGrid.CountAt(cell) - num;
				this.parent.Map.deepResourceGrid.SetAt(cell, resDef, newCount);
			}
			int stackCount = Mathf.Max(1, GenMath.RoundRandom((float)num * yieldPct));
			Thing thing = ThingMaker.MakeThing(resDef, null);
			thing.stackCount = stackCount;
			GenPlace.TryPlaceThing(thing, this.parent.InteractionCell, this.parent.Map, ThingPlaceMode.Near, null, null, default(Rot4));
			CompManualDrill CMD = drill.TryGetComp<CompManualDrill>();
			if (CMD == null)
			{
				return;
			}
			ThingDef def;
			IntVec3 cellchk;
			if (!nextResource || ManualDrillUtility.DrillCanGetToCount(drill, CMD.MDProps.shallowReach, 9, out def, out cellchk) > 0)
			{
				return;
			}
			if (CMD.MDProps.mineRock && DeepDrillUtility.GetBaseResource(this.parent.Map, this.parent.TrueCenter().ToIntVec3()) == null)
			{
				Messages.Message("Prospecting.ManualDrillExhaustedNoFallback".Translate(), this.parent, MessageTypeDefOf.TaskCompletion, true);
				return;
			}
			Messages.Message("Prospecting.ManualDrillExhausted".Translate(Find.ActiveLanguageWorker.Pluralize(DeepDrillUtility.GetBaseResource(this.parent.Map, this.parent.TrueCenter().ToIntVec3()).label, -1)), this.parent, MessageTypeDefOf.TaskCompletion, true);
			drill.SetForbidden(true, true);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002D8F File Offset: 0x00000F8F
		private bool GetNextManualResource(Building drill, out ThingDef resDef, out int countPresent, out IntVec3 cell)
		{
			return ManualDrillUtility.GetNextManualResource(drill, drill.TrueCenter().ToIntVec3(), drill.Map, out resDef, out countPresent, out cell);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002DAC File Offset: 0x00000FAC
		public override string CompInspectStringExtra()
		{
			string progressMsg = "";
			string resMsg = "\n";
			string weatherMsg = "";
			if (this.drill.Spawned)
			{
				IntVec3 drillpoint = this.drill.TrueCenter().ToIntVec3();
				if (this.prospected)
				{
					ThingDef thingdef;
					IntVec3 nextCell;
					int resources = ManualDrillUtility.DrillCanGetToCount(this.drill, this.MDProps.shallowReach, 9, out thingdef, out nextCell);
					if (resources > 0)
					{
						ThingDef resDef = this.drill.Map.deepResourceGrid.ThingDefAt(drillpoint);
						if (resDef != null)
						{
							progressMsg = "ResourceBelow".Translate() + ": " + resDef.LabelCap + "\n" + "ProgressToNextPortion".Translate() + ": " + this.ProgressToNextPortionPercent.ToStringPercent("F0");
							resMsg += "Prospecting.DeepResources".Translate(resources.ToString(), this.MDProps.shallowReach.ToStringPercent());
						}
					}
					else
					{
						int zero = 0;
						progressMsg = "DeepDrillNoResources".Translate();
						resMsg += "Prospecting.DeepResources".Translate(zero.ToString(), this.MDProps.shallowReach.ToStringPercent());
					}
				}
				else
				{
					progressMsg = "Prospecting.DrillNotProspected".Translate() + "\n" + "Prospecting.ProgressProspect".Translate() + ": " + this.ProgressToProspectingPercent.ToStringPercent("F0");
					resMsg = "";
				}
				if (!this.windOk)
				{
					weatherMsg = "\n" + "Prospecting.InsufficientWind".Translate();
				}
				else if (this.UsedLastTick())
				{
					float roofpct = this.roofFactor / 1f;
					float windpct = this.windFactor / 1f;
					weatherMsg = "\n" + "Prospecting.RoofPerform".Translate(roofpct.ToStringPercent("F0")) + ", " + "Prospecting.WindPerform".Translate(windpct.ToStringPercent("F0"));
				}
			}
			else
			{
				progressMsg = "Prospecting.InstallDrill".Translate();
				resMsg = "";
			}
			return progressMsg + resMsg + weatherMsg;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00003033 File Offset: 0x00001233
		public bool UsedLastTick()
		{
			return this.lastUsedTick >= Find.TickManager.TicksGame - 1;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000304C File Offset: 0x0000124C
		public static void ResetVals(CompManualDrill cmd)
		{
			cmd.prospected = false;
			cmd.maxCount = 0;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000305C File Offset: 0x0000125C
		public override void PostDraw()
		{
			base.PostDraw();
			Rot4 rotation = this.parent.Rotation;
			Vector3 pos = this.parent.TrueCenter();
			pos.y += 0.046875f;
			if (rotation == Rot4.North)
			{
				pos.x += 0f;
				pos.z += 0.5f;
			}
			else if (rotation == Rot4.South)
			{
				pos.x += 0f;
				pos.z += -0.5f;
			}
			else if (rotation == Rot4.East)
			{
				pos.x += 0.5f;
				pos.z += 0f;
			}
			else if (rotation == Rot4.West)
			{
				pos.x += -0.5f;
				pos.z += 0f;
			}
			bool flag = true;
			Vector3 s = new Vector3(3f, 0f, 3f);
			float anglePerRate = 4f;
			if (!Find.TickManager.Paused && this.windOk && this.UsedLastTick())
			{
				this.barrelsAngle += this.windFactor / 1.5f * anglePerRate;
			}
			if (this.barrelsAngle >= 360f)
			{
				this.barrelsAngle -= 360f;
			}
			if (this.barrelsAngle <= -360f)
			{
				this.barrelsAngle += 360f;
			}
			s.RotatedBy(this.barrelsAngle);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(pos, Quaternion.AngleAxis(this.barrelsAngle, Vector3.up), s);
			Graphics.DrawMesh((!flag) ? MeshPool.plane10Flip : MeshPool.plane10, matrix, CompManualDrillMats.BarrelTurbineMat, 0);
		}

		// Token: 0x04000002 RID: 2
		public bool prospected;

		// Token: 0x04000003 RID: 3
		public int maxCount;

		// Token: 0x04000004 RID: 4
		private float portionProgress;

		// Token: 0x04000005 RID: 5
		private float prospectProgress;

		// Token: 0x04000006 RID: 6
		private float portionYieldPct;

		// Token: 0x04000007 RID: 7
		private float prospectYieldPct;

		// Token: 0x04000008 RID: 8
		private int lastUsedTick = -99999;

		// Token: 0x04000009 RID: 9
		public bool windOk = true;

		// Token: 0x0400000A RID: 10
		public float roofFactor = 1f;

		// Token: 0x0400000B RID: 11
		public float windFactor = 1f;

		// Token: 0x0400000C RID: 12
		public float barrelsAngle;

		// Token: 0x0400000D RID: 13
		private const float WorkPerPortionBase = 12000f;
	}
}
