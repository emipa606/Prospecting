using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
	// Token: 0x0200000F RID: 15
	[HarmonyPatch(typeof(CompDeepDrill), "DrillWorkDone")]
	public class DrillWorkDone_PrePatch
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00003E58 File Offset: 0x00002058
		[HarmonyPrefix]
		[HarmonyPriority(800)]
		public static bool PreFix(ref CompDeepDrill __instance, ref CompPowerTrader ___powerComp, ref float ___portionProgress, ref float ___portionYieldPct, ref int ___lastUsedTick, Pawn driller)
		{
			if (driller.CurJob != null)
			{
				Job wbJob = driller.CurJob;
				if (driller.CurJob.def.defName == "OperateWideBoy")
				{
					float powerFactor = 1f;
					if (wbJob.targetA.HasThing)
					{
						float basePower = ___powerComp.Props.basePowerConsumption;
						if (basePower > 0f)
						{
							powerFactor = Math.Max(0f, -1f * (___powerComp.PowerOutput / basePower));
						}
					}
					float statValue = driller.GetStatValue(StatDefOf.MiningSpeed, true) * powerFactor;
					___portionProgress += statValue;
					___portionYieldPct += statValue * driller.GetStatValue(StatDefOf.MiningYield, true) / (12000f / Find.Storyteller.difficulty.mineYieldFactor);
					___lastUsedTick = Find.TickManager.TicksGame;
					if (wbJob.targetA.HasThing)
					{
						CompWideBoy compWideBoy = wbJob.targetA.Thing.TryGetComp<CompWideBoy>();
						if (compWideBoy != null)
						{
							compWideBoy.lastDriller = driller.thingIDNumber;
						}
					}
					if (___portionProgress > 12000f / Find.Storyteller.difficulty.mineYieldFactor)
					{
						AccessTools.Method(typeof(CompDeepDrill), "TryProducePortion", new Type[]
						{
							typeof(float)
						}, null).Invoke(__instance, new object[]
						{
							___portionYieldPct
						});
						___portionProgress = 0f;
						___portionYieldPct = 0f;
					}
					return false;
				}
			}
			return true;
		}
	}
}
