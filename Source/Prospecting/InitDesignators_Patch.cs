using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace Prospecting
{
	// Token: 0x02000012 RID: 18
	[HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
	public class InitDesignators_Patch
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00004095 File Offset: 0x00002295
		[HarmonyPostfix]
		[HarmonyPriority(0)]
		public static void PostFix(ref List<Designator> ___desList)
		{
			___desList.Add(new Designator_Prospect());
		}
	}
}
