using System;
using Verse;

namespace Prospecting
{
	// Token: 0x02000004 RID: 4
	public class Building_ManualDrill : Building
	{
		// Token: 0x06000006 RID: 6 RVA: 0x0000267C File Offset: 0x0000087C
		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			Map map = base.Map;
			base.DeSpawn(mode);
			CompManualDrill CMD = this.TryGetComp<CompManualDrill>();
			if (CMD != null)
			{
				CompManualDrill.ResetVals(CMD);
			}
		}
	}
}
