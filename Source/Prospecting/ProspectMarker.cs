using System;
using System.Text;
using Verse;

namespace Prospecting
{
	// Token: 0x02000024 RID: 36
	public class ProspectMarker : Building
	{
		// Token: 0x06000095 RID: 149 RVA: 0x00005621 File Offset: 0x00003821
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.TicksTillExpire, "TicksTillExpire", 60000, false);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000563F File Offset: 0x0000383F
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				this.TicksTillExpire = 60000;
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005658 File Offset: 0x00003858
		public override void Tick()
		{
			base.Tick();
			int interval = 240;
			if (this.IsHashIntervalTick(interval))
			{
				this.TicksTillExpire -= interval;
				if (this.TicksTillExpire <= 0)
				{
					this.Destroy(DestroyMode.Vanish);
				}
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005698 File Offset: 0x00003898
		public override void TickRare()
		{
			base.TickRare();
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000056A0 File Offset: 0x000038A0
		public override string GetInspectString()
		{
			string returnString = null;
			string resString = null;
			if (((this != null) ? base.Map : null) != null && base.Spawned)
			{
				ThingDef resDef = base.Map.deepResourceGrid.ThingDefAt(base.Position);
				if (resDef != null)
				{
					string resLabel = (resDef.LabelCap != null) ? resDef.LabelCap : "Prospecting.Unknown".Translate();
					string resCount = base.Map.deepResourceGrid.CountAt(base.Position).ToString();
					string resTime = ((float)this.TicksTillExpire / 2500f).ToString("F1");
					resString = "Prospecting.MarkerDisplay".Translate(resLabel, resCount, resTime);
				}
			}
			if (resString != null)
			{
				returnString = resString;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			string text = base.InspectStringPartsFromComps();
			if (!text.NullOrEmpty())
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(text);
			}
			if (returnString != null)
			{
				returnString += stringBuilder.ToString();
			}
			else
			{
				returnString = stringBuilder.ToString();
			}
			return returnString;
		}

		// Token: 0x04000039 RID: 57
		private const int NumExpireTicks = 60000;

		// Token: 0x0400003A RID: 58
		private int TicksTillExpire;
	}
}
