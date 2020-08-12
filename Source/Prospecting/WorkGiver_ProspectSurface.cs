using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
	// Token: 0x02000029 RID: 41
	public class WorkGiver_ProspectSurface : WorkGiver_Scanner
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000AE RID: 174 RVA: 0x000062D2 File Offset: 0x000044D2
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000062D5 File Offset: 0x000044D5
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000062D8 File Offset: 0x000044D8
		public static void ResetStaticData()
		{
			WorkGiver_ProspectSurface.NoPathTrans = "NoPath".Translate();
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000062EE File Offset: 0x000044EE
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			foreach (Designation des in pawn.Map.designationManager.SpawnedDesignationsOfDef(this.prospectDesig))
			{
				bool mayBeAccessible = false;
				for (int i = 0; i < 8; i++)
				{
					IntVec3 c = des.target.Cell + GenAdj.AdjacentCells[i];
					if (c.InBounds(pawn.Map) && c.Walkable(pawn.Map))
					{
						mayBeAccessible = true;
						break;
					}
				}
				if (mayBeAccessible)
				{
					Mineable j = des.target.Cell.GetFirstMineable(pawn.Map);
					if (j != null)
					{
						yield return j;
					}
				}
			}
			IEnumerator<Designation> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00006308 File Offset: 0x00004508
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (!t.def.mineable)
			{
				return null;
			}
			if (pawn.Map.designationManager.DesignationAt(t.Position, this.prospectDesig) == null)
			{
				return null;
			}
			LocalTargetInfo target = t;
			if (!pawn.CanReserve(target, 1, -1, null, forced))
			{
				return null;
			}
			bool flag = false;
			for (int i = 0; i < 8; i++)
			{
				IntVec3 intVec = t.Position + GenAdj.AdjacentCells[i];
				if (intVec.InBounds(pawn.Map) && intVec.Standable(pawn.Map) && ReachabilityImmediate.CanReachImmediate(intVec, t, pawn.Map, PathEndMode.ClosestTouch, pawn))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int j = 0; j < 8; j++)
				{
					IntVec3 intVec2 = t.Position + GenAdj.AdjacentCells[j];
					if (intVec2.InBounds(t.Map) && ReachabilityImmediate.CanReachImmediate(intVec2, t, pawn.Map, PathEndMode.Touch, pawn) && intVec2.Walkable(t.Map) && !intVec2.Standable(t.Map))
					{
						Thing thing = null;
						List<Thing> thingList = intVec2.GetThingList(t.Map);
						for (int k = 0; k < thingList.Count; k++)
						{
							if (thingList[k].def.designateHaulable && thingList[k].def.passability == Traversability.PassThroughOnly)
							{
								thing = thingList[k];
								break;
							}
						}
						if (thing != null)
						{
							Job job = HaulAIUtility.HaulAsideJobFor(pawn, thing);
							if (job != null)
							{
								return job;
							}
							JobFailReason.Is(WorkGiver_ProspectSurface.NoPathTrans, null);
							return null;
						}
					}
				}
				JobFailReason.Is(WorkGiver_ProspectSurface.NoPathTrans, null);
				return null;
			}
			return new Job(this.ProspectJob, t, 20000, true);
		}

		// Token: 0x04000048 RID: 72
		private static string NoPathTrans;

		// Token: 0x04000049 RID: 73
		private const int MiningJobTicks = 20000;

		// Token: 0x0400004A RID: 74
		public DesignationDef prospectDesig = ProspectDef.Prospect;

		// Token: 0x0400004B RID: 75
		public JobDef ProspectJob = ProspectDef.ProspectSurface;
	}
}
