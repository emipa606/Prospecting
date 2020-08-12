using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
	// Token: 0x0200001B RID: 27
	public class ManualDrillUtility
	{
		// Token: 0x0600006C RID: 108 RVA: 0x000046F4 File Offset: 0x000028F4
		public static int DrillCanGetToCount(Building drill, float shallowFactor, int numCells, out ThingDef resDef, out IntVec3 nextCell)
		{
			int resCount = 0;
			resDef = null;
			nextCell = drill.TrueCenter().ToIntVec3();
			shallowFactor = Math.Max(0.1f, Math.Min(0.33f, shallowFactor));
			if (drill != null && drill.Spawned && ((drill != null) ? drill.Map : null) != null)
			{
				Map map = drill.Map;
				ManualDrillMapComponent mapComp = map.GetComponent<ManualDrillMapComponent>();
				IntVec3 rootpos = drill.TrueCenter().ToIntVec3();
				for (int i = 0; i < 9; i++)
				{
					IntVec3 pos = rootpos + GenRadial.ManualRadialPattern[i];
					ThingDef resDefAt = map.deepResourceGrid.ThingDefAt(pos);
					if (resDefAt != null)
					{
						if (resDef == null && ManualDrillUtility.GetShallowResources(drill, shallowFactor, pos) > 0)
						{
							resDef = resDefAt;
							if (pos != nextCell)
							{
								nextCell = pos;
							}
						}
						int maxValue;
						if (mapComp.GetValue(pos, out maxValue))
						{
							int deepCountAt = map.deepResourceGrid.CountAt(pos);
							int shallowCount = (int)((float)maxValue * shallowFactor);
							if (maxValue - shallowCount <= deepCountAt)
							{
								resCount += deepCountAt - (maxValue - shallowCount);
							}
						}
					}
				}
			}
			return resCount;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004810 File Offset: 0x00002A10
		public static bool GetNextManualResource(Building drill, IntVec3 p, Map map, out ThingDef resDef, out int countPresentCell, out IntVec3 cell)
		{
			CompManualDrill CMD = drill.TryGetComp<CompManualDrill>();
			ThingDef thingDef;
			IntVec3 nextCell;
			if (CMD != null && CMD.prospected && ManualDrillUtility.DrillCanGetToCount(drill, CMD.MDProps.shallowReach, 9, out thingDef, out nextCell) > 0)
			{
				resDef = thingDef;
				countPresentCell = ManualDrillUtility.GetShallowResources(drill, CMD.MDProps.shallowReach, nextCell);
				cell = nextCell;
				return true;
			}
			resDef = DeepDrillUtility.GetBaseResource(map, p);
			countPresentCell = int.MaxValue;
			cell = p;
			return false;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004888 File Offset: 0x00002A88
		public static int GetShallowResources(Building drill, float shallowFactor, IntVec3 cell)
		{
			int shallowRes = 0;
			int maxVal;
			if (((drill != null) ? drill.Map : null) != null && drill.Spawned && shallowFactor > 0f && drill.Map.GetComponent<ManualDrillMapComponent>().GetValue(cell, out maxVal))
			{
				int deepCountAt = drill.Map.deepResourceGrid.CountAt(cell);
				int shallowCount = (int)((float)maxVal * shallowFactor);
				if (maxVal - shallowCount <= deepCountAt)
				{
					shallowRes += deepCountAt - (maxVal - shallowCount);
				}
			}
			return shallowRes;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000048F4 File Offset: 0x00002AF4
		public static float GetPerformFactor(Building drill, out bool noWind, out float roofFactor, out float windFactor)
		{
			float performFactor = 1f;
			roofFactor = 1f;
			windFactor = 1f;
			noWind = false;
			if (((drill != null) ? drill.Map : null) != null && drill.Spawned)
			{
				IntVec3 root = drill.TrueCenter().ToIntVec3();
				int openRoof = 0;
				int totalRoof = 0;
				for (int i = 0; i < 9; i++)
				{
					IntVec3 c = root + GenRadial.RadialPattern[i];
					totalRoof++;
					if (!c.Roofed(drill.Map))
					{
						openRoof++;
					}
				}
				if (totalRoof > 0)
				{
					roofFactor = Mathf.Lerp(0.5f, 1f, (float)(openRoof / totalRoof));
				}
				windFactor = Math.Min(1.5f, drill.Map.windManager.WindSpeed);
				if (windFactor < 0.5f)
				{
					noWind = true;
				}
			}
			return performFactor * roofFactor * windFactor;
		}

		// Token: 0x0400002B RID: 43
		public const int drillCellsNum = 9;
	}
}
