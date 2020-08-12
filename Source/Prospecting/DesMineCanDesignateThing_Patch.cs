using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting
{
	// Token: 0x0200000E RID: 14
	[HarmonyPatch(typeof(Designator_Mine), "CanDesignateThing")]
	public class DesMineCanDesignateThing_Patch
	{
		// Token: 0x06000047 RID: 71 RVA: 0x00003DF0 File Offset: 0x00001FF0
		[HarmonyPostfix]
		[HarmonyPriority(0)]
		public static void PostFix(ref AcceptanceReport __result, Thing t)
		{
			if (!t.def.mineable)
			{
				__result = false;
			}
			if (((t != null) ? t.Map : null) != null)
			{
				DesignationDef desig = ProspectDef.Prospect;
				if (t.Map.designationManager.DesignationAt(t.Position, desig) != null)
				{
					__result = AcceptanceReport.WasRejected;
				}
			}
		}
	}
}
