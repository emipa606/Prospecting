using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Prospecting
{
	// Token: 0x02000028 RID: 40
	public class WorkGiver_ManualDrillProspect : WorkGiver_Scanner
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000616A File Offset: 0x0000436A
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(this.ManualDrillDef);
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006177 File Offset: 0x00004377
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(this.ManualDrillDef).Cast<Thing>();
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006194 File Offset: 0x00004394
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
			for (int i = 0; i < allBuildingsColonist.Count; i++)
			{
				Building building = allBuildingsColonist[i];
				if (building.def == this.ManualDrillDef && building.Spawned && building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null)
				{
					CompManualDrill CMD = building.TryGetComp<CompManualDrill>();
					if (CMD != null && !CMD.prospected && CMD.windOk)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006218 File Offset: 0x00004418
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
			return compManual != null && !compManual.prospected && compManual.windOk && building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null && !building.IsBurning();
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000062A7 File Offset: 0x000044A7
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(ProspectDef.ManualDrillProspect, t, 1500, true);
		}

		// Token: 0x04000047 RID: 71
		public ThingDef ManualDrillDef = ProspectDef.PrsManualDrill;
	}
}
