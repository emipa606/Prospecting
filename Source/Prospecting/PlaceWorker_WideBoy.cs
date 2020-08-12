using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
	// Token: 0x0200001E RID: 30
	public class PlaceWorker_WideBoy : PlaceWorker
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00004B7B File Offset: 0x00002D7B
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			if (DeepDrillUtility.GetNextResource(loc, map) == null)
			{
				return "MustPlaceOnDrillable".Translate();
			}
			return true;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00004BA0 File Offset: 0x00002DA0
		public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
		{
			Map currentMap = Find.CurrentMap;
			List<Building> allBuildingsColonist = currentMap.listerBuildings.allBuildingsColonist;
			for (int i = 0; i < allBuildingsColonist.Count; i++)
			{
				Building thingy = allBuildingsColonist[i];
				if (thingy.TryGetComp<CompDeepScanner>() != null)
				{
					CompPowerTrader compPowerTrader = thingy.TryGetComp<CompPowerTrader>();
					if (compPowerTrader == null || compPowerTrader.PowerOn)
					{
						currentMap.deepResourceGrid.MarkForDraw();
					}
				}
			}
			int span = 49;
			List<IntVec3> cells = new List<IntVec3>();
			for (int j = 0; j < span; j++)
			{
				if (currentMap != null)
				{
					IntVec3 tempCell = center + GenRadial.ManualRadialPattern[j];
					if (tempCell.InBounds(currentMap))
					{
						cells.Add(tempCell);
					}
				}
			}
			if (cells.Count > 0)
			{
				GenDraw.DrawFieldEdges(cells, Color.white);
			}
		}
	}
}
