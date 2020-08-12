using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting
{
	// Token: 0x02000010 RID: 16
	[HarmonyPatch(typeof(DeepDrillUtility), "GetNextResource", new Type[]
	{
		typeof(IntVec3),
		typeof(Map),
		typeof(ThingDef),
		typeof(int),
		typeof(IntVec3)
	}, new ArgumentType[]
	{
		ArgumentType.Normal,
		ArgumentType.Normal,
		ArgumentType.Out,
		ArgumentType.Out,
		ArgumentType.Out
	})]
	public class GetNextResource_PrePatch
	{
		// Token: 0x0600004B RID: 75 RVA: 0x00003FCC File Offset: 0x000021CC
		[HarmonyPrefix]
		[HarmonyPriority(800)]
		public static bool PreFix(ref bool __result, IntVec3 p, Map map, out ThingDef resDef, out int countPresent, out IntVec3 cell)
		{
			Thing wideboy;
			if (ProspectingWideBoy.IsWideBoyAt(map, p, out wideboy))
			{
				int spanWB = ProspectingWideBoy.GetWideBoySpan(wideboy);
				for (int i = 0; i < spanWB; i++)
				{
					IntVec3 intVec = ProspectingWideBoy.GetDDCell(p, i);
					if (intVec.InBounds(map))
					{
						ThingDef thingDef = map.deepResourceGrid.ThingDefAt(intVec);
						if (thingDef != null)
						{
							resDef = thingDef;
							countPresent = map.deepResourceGrid.CountAt(intVec);
							cell = intVec;
							__result = true;
							return false;
						}
					}
				}
				resDef = DeepDrillUtility.GetBaseResource(map, p);
				countPresent = int.MaxValue;
				cell = p;
				__result = false;
				return false;
			}
			resDef = DeepDrillUtility.GetBaseResource(map, p);
			countPresent = int.MaxValue;
			cell = p;
			__result = false;
			return true;
		}
	}
}
