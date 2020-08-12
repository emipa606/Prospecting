using System;
using UnityEngine;
using Verse;

namespace Prospecting
{
	// Token: 0x02000026 RID: 38
	public class Settings : ModSettings
	{
		// Token: 0x0600009E RID: 158 RVA: 0x000059AC File Offset: 0x00003BAC
		public void DoWindowContents(Rect canvas)
		{
			float gap = 3f;
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = canvas.width;
			listing_Standard.Begin(canvas);
			listing_Standard.Gap(10f);
			listing_Standard.CheckboxLabeled("Prospecting.AllowManualSound".Translate(), ref this.AllowManualSound, null);
			listing_Standard.Gap(gap);
			listing_Standard.CheckboxLabeled("Prospecting.AllowProspect".Translate(), ref this.AllowProspect, null);
			listing_Standard.Gap(gap);
			checked
			{
				listing_Standard.Label("Prospecting.BaseChance".Translate() + "  " + (int)this.BaseChance, -1f, null);
				this.BaseChance = (float)((int)listing_Standard.Slider((float)((int)this.BaseChance), 25f, 75f));
				listing_Standard.Gap(gap);
				listing_Standard.Label("Prospecting.PrsLumpSizeMin".Translate() + "  " + (int)this.PrsLumpSizeMin, -1f, null);
				this.PrsLumpSizeMin = (float)((int)listing_Standard.Slider((float)((int)this.PrsLumpSizeMin), 50f, 200f));
				listing_Standard.Gap(gap);
				listing_Standard.Label("Prospecting.PrsLumpSizeMax".Translate() + "  " + (int)this.PrsLumpSizeMax, -1f, null);
				this.PrsLumpSizeMax = (float)((int)listing_Standard.Slider((float)((int)this.PrsLumpSizeMax), 50f, 200f));
				listing_Standard.Gap(gap);
				listing_Standard.Label("Prospecting.PrsCommonality".Translate() + "  " + (int)this.PrsCommonality, -1f, null);
				this.PrsCommonality = (float)((int)listing_Standard.Slider((float)((int)this.PrsCommonality), 50f, 200f));
				listing_Standard.Gap(gap);
				listing_Standard.Label("Prospecting.PrsMineYield".Translate() + "  " + (int)this.PrsMineYield, -1f, null);
				this.PrsMineYield = (float)((int)listing_Standard.Slider((float)((int)this.PrsMineYield), 50f, 200f));
				listing_Standard.Gap(gap);
				listing_Standard.Label("Prospecting.PrsDeepLumpSizeMin".Translate() + "  " + (int)this.PrsDeepLumpSizeMin, -1f, null);
				this.PrsDeepLumpSizeMin = (float)((int)listing_Standard.Slider((float)((int)this.PrsDeepLumpSizeMin), 50f, 200f));
				listing_Standard.Gap(gap);
				listing_Standard.Label("Prospecting.PrsDeepLumpSizeMax".Translate() + "  " + (int)this.PrsDeepLumpSizeMax, -1f, null);
				this.PrsDeepLumpSizeMax = (float)((int)listing_Standard.Slider((float)((int)this.PrsDeepLumpSizeMax), 50f, 200f));
				listing_Standard.Gap(gap);
				listing_Standard.Label("Prospecting.PrsDeepCommonality".Translate() + "  " + (int)this.PrsDeepCommonality, -1f, null);
				this.PrsDeepCommonality = (float)((int)listing_Standard.Slider((float)((int)this.PrsDeepCommonality), 50f, 200f));
				listing_Standard.Gap(gap);
				listing_Standard.Label("Prospecting.PrsDeepMineYield".Translate() + "  " + (int)this.PrsDeepMineYield, -1f, null);
				this.PrsDeepMineYield = (float)((int)listing_Standard.Slider((float)((int)this.PrsDeepMineYield), 50f, 200f));
				listing_Standard.Gap(gap);
				Text.Font = GameFont.Tiny;
				listing_Standard.Label("          " + "Prospecting.LoadTip".Translate(), -1f, null);
				Text.Font = GameFont.Small;
				listing_Standard.Gap(gap);
				if (listing_Standard.ButtonTextLabeled("Prospecting.ResetDefaults".Translate(), "Prospecting.Reset".Translate()))
				{
					this.doReset();
				}
				listing_Standard.End();
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005DE0 File Offset: 0x00003FE0
		public void doReset()
		{
			this.AllowManualSound = true;
			this.AllowProspect = true;
			this.BaseChance = 50f;
			this.PrsLumpSizeMin = 100f;
			this.PrsLumpSizeMax = 100f;
			this.PrsCommonality = 100f;
			this.PrsMineYield = 100f;
			this.PrsDeepLumpSizeMin = 100f;
			this.PrsDeepLumpSizeMax = 100f;
			this.PrsDeepCommonality = 100f;
			this.PrsDeepMineYield = 100f;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005E60 File Offset: 0x00004060
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.AllowManualSound, "AllowManualSound", true, false);
			Scribe_Values.Look<bool>(ref this.AllowProspect, "AllowProspect", true, false);
			Scribe_Values.Look<float>(ref this.BaseChance, "BaseChance", 50f, false);
			Scribe_Values.Look<float>(ref this.PrsLumpSizeMin, "PrsLumpSizeMin", 100f, false);
			Scribe_Values.Look<float>(ref this.PrsLumpSizeMax, "PrsLumpSizeMax", 100f, false);
			Scribe_Values.Look<float>(ref this.PrsCommonality, "PrsCommonality", 100f, false);
			Scribe_Values.Look<float>(ref this.PrsMineYield, "PrsMineYield", 100f, false);
			Scribe_Values.Look<float>(ref this.PrsDeepLumpSizeMin, "PrsDeepLumpSizeMin", 100f, false);
			Scribe_Values.Look<float>(ref this.PrsDeepLumpSizeMax, "PrsDeepLumpSizeMax", 100f, false);
			Scribe_Values.Look<float>(ref this.PrsDeepCommonality, "PrsDeepCommonality", 100f, false);
			Scribe_Values.Look<float>(ref this.PrsDeepMineYield, "PrsDeepMineYield", 100f, false);
		}

		// Token: 0x0400003B RID: 59
		public bool AllowManualSound = true;

		// Token: 0x0400003C RID: 60
		public bool AllowProspect = true;

		// Token: 0x0400003D RID: 61
		public float BaseChance = 50f;

		// Token: 0x0400003E RID: 62
		public float PrsLumpSizeMin = 100f;

		// Token: 0x0400003F RID: 63
		public float PrsLumpSizeMax = 100f;

		// Token: 0x04000040 RID: 64
		public float PrsCommonality = 100f;

		// Token: 0x04000041 RID: 65
		public float PrsMineYield = 100f;

		// Token: 0x04000042 RID: 66
		public float PrsDeepLumpSizeMin = 100f;

		// Token: 0x04000043 RID: 67
		public float PrsDeepLumpSizeMax = 100f;

		// Token: 0x04000044 RID: 68
		public float PrsDeepCommonality = 100f;

		// Token: 0x04000045 RID: 69
		public float PrsDeepMineYield = 100f;
	}
}
