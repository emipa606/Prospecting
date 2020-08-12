using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting
{
	// Token: 0x0200000C RID: 12
	[HarmonyPatch(typeof(Designator_Cancel), "CanDesignateThing")]
	public class CancelCanDesignateThing_Patch
	{
		// Token: 0x06000043 RID: 67 RVA: 0x00003D4C File Offset: 0x00001F4C
		[HarmonyPostfix]
		[HarmonyPriority(0)]
		public static void PostFix(ref AcceptanceReport __result, Thing t)
		{
			DesignationDef desig = ProspectDef.Prospect;
			if (t.def.mineable && t.Map.designationManager.DesignationAt(t.Position, desig) != null)
			{
				__result = true;
			}
		}
	}
}
