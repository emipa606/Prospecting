using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Prospecting
{
	// Token: 0x02000003 RID: 3
	public class Alert_NeedProspector : Alert
	{
		// Token: 0x06000004 RID: 4 RVA: 0x00002534 File Offset: 0x00000734
		public Alert_NeedProspector()
		{
			this.defaultLabel = "Prospector.NeedProspector".Translate();
			this.defaultExplanation = "Prospector.NeedProspectorDesc".Translate();
			this.defaultPriority = AlertPriority.High;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002570 File Offset: 0x00000770
		public override AlertReport GetReport()
		{
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				Map map = maps[i];
				if (map.IsPlayerHome)
				{
					Designation designation = (from d in map.designationManager.allDesignations
					where d.def == ProspectDef.Prospect
					select d).FirstOrDefault<Designation>();
					if (designation != null)
					{
						bool flag = false;
						foreach (Pawn item in map.mapPawns.FreeColonistsSpawned)
						{
							if (!item.Downed && item.workSettings != null && item.workSettings.GetPriority(ProspectDef.Prospecting) > 0)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return AlertReport.CulpritIs(designation.target.Thing);
						}
					}
				}
			}
			return false;
		}
	}
}
