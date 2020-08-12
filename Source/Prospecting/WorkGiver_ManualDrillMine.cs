using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
	// Token: 0x02000027 RID: 39
	public class WorkGiver_ManualDrillMine : WorkGiver_Scanner
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00005FE4 File Offset: 0x000041E4
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(this.ManualDrillDef);
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005FF1 File Offset: 0x000041F1
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(this.ManualDrillDef).Cast<Thing>();
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006010 File Offset: 0x00004210
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
			for (int i = 0; i < allBuildingsColonist.Count; i++)
			{
				Building building = allBuildingsColonist[i];
				if (building.def == this.ManualDrillDef && building.Spawned && building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null)
				{
					CompManualDrill CMD = building.TryGetComp<CompManualDrill>();
					if (CMD != null && CMD.prospected && CMD.windOk)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00006094 File Offset: 0x00004294
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t.Faction != pawn.Faction)
			{
				return false;
			}
			Building building = t as Building;
			if (building == null)
			{
				return false;
			}
			if (building.IsForbidden(pawn))
			{
				return false;
			}
			LocalTargetInfo target = building;
			if (!pawn.CanReserve(target, 1, -1, null, forced))
			{
				return false;
			}
			CompManualDrill compManual = building.TryGetComp<CompManualDrill>();
			ThingDef def;
			IntVec3 nextCell;
			return compManual != null && compManual.prospected && compManual.windOk && ManualDrillUtility.DrillCanGetToCount(building, compManual.MDProps.shallowReach, 9, out def, out nextCell) > 0 && building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null && !building.IsBurning();
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x0000613F File Offset: 0x0000433F
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(ProspectDef.ManualDrillMine, t, 1500, true);
		}

		// Token: 0x04000046 RID: 70
		public ThingDef ManualDrillDef = ProspectDef.PrsManualDrill;
	}
}
