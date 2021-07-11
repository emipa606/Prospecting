using UnityEngine;
using Verse;

namespace Prospecting
{
    // Token: 0x02000026 RID: 38
    public class Settings : ModSettings
    {
        // Token: 0x0400003B RID: 59
        public bool AllowManualSound = true;

        // Token: 0x0400003C RID: 60
        public bool AllowProspect = true;

        // Token: 0x0400003D RID: 61
        public float BaseChance = 50f;

        // Token: 0x04000040 RID: 64
        public float PrsCommonality = 100f;

        // Token: 0x04000044 RID: 68
        public float PrsDeepCommonality = 100f;

        // Token: 0x04000043 RID: 67
        public float PrsDeepLumpSizeMax = 100f;

        // Token: 0x04000042 RID: 66
        public float PrsDeepLumpSizeMin = 100f;

        // Token: 0x04000045 RID: 69
        public float PrsDeepMineYield = 100f;

        // Token: 0x0400003F RID: 63
        public float PrsLumpSizeMax = 100f;

        // Token: 0x0400003E RID: 62
        public float PrsLumpSizeMin = 100f;

        // Token: 0x04000041 RID: 65
        public float PrsMineYield = 100f;

        // Token: 0x0600009E RID: 158 RVA: 0x000059AC File Offset: 0x00003BAC
        public void DoWindowContents(Rect canvas)
        {
            var gap = 3f;
            var listing_Standard = new Listing_Standard {ColumnWidth = canvas.width};
            listing_Standard.Begin(canvas);
            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("Prospecting.AllowManualSound".Translate(), ref AllowManualSound);
            listing_Standard.Gap(gap);
            listing_Standard.CheckboxLabeled("Prospecting.AllowProspect".Translate(), ref AllowProspect);
            listing_Standard.Gap(gap);
            checked
            {
                listing_Standard.Label("Prospecting.BaseChance".Translate() + "  " + (int) BaseChance);
                BaseChance = (int) listing_Standard.Slider((int) BaseChance, 25f, 75f);
                listing_Standard.Gap(gap);
                listing_Standard.Label("Prospecting.PrsLumpSizeMin".Translate() + "  " + (int) PrsLumpSizeMin);
                PrsLumpSizeMin = (int) listing_Standard.Slider((int) PrsLumpSizeMin, 50f, 200f);
                listing_Standard.Gap(gap);
                listing_Standard.Label("Prospecting.PrsLumpSizeMax".Translate() + "  " + (int) PrsLumpSizeMax);
                PrsLumpSizeMax = (int) listing_Standard.Slider((int) PrsLumpSizeMax, 50f, 200f);
                listing_Standard.Gap(gap);
                listing_Standard.Label("Prospecting.PrsCommonality".Translate() + "  " + (int) PrsCommonality);
                PrsCommonality = (int) listing_Standard.Slider((int) PrsCommonality, 50f, 200f);
                listing_Standard.Gap(gap);
                listing_Standard.Label("Prospecting.PrsMineYield".Translate() + "  " + (int) PrsMineYield);
                PrsMineYield = (int) listing_Standard.Slider((int) PrsMineYield, 50f, 200f);
                listing_Standard.Gap(gap);
                listing_Standard.Label("Prospecting.PrsDeepLumpSizeMin".Translate() + "  " + (int) PrsDeepLumpSizeMin);
                PrsDeepLumpSizeMin = (int) listing_Standard.Slider((int) PrsDeepLumpSizeMin, 50f, 200f);
                listing_Standard.Gap(gap);
                listing_Standard.Label("Prospecting.PrsDeepLumpSizeMax".Translate() + "  " + (int) PrsDeepLumpSizeMax);
                PrsDeepLumpSizeMax = (int) listing_Standard.Slider((int) PrsDeepLumpSizeMax, 50f, 200f);
                listing_Standard.Gap(gap);
                listing_Standard.Label("Prospecting.PrsDeepCommonality".Translate() + "  " + (int) PrsDeepCommonality);
                PrsDeepCommonality = (int) listing_Standard.Slider((int) PrsDeepCommonality, 50f, 200f);
                listing_Standard.Gap(gap);
                listing_Standard.Label("Prospecting.PrsDeepMineYield".Translate() + "  " + (int) PrsDeepMineYield);
                PrsDeepMineYield = (int) listing_Standard.Slider((int) PrsDeepMineYield, 50f, 200f);
                listing_Standard.Gap(gap);
                Text.Font = GameFont.Tiny;
                listing_Standard.Label("          " + "Prospecting.LoadTip".Translate());
                Text.Font = GameFont.Small;
                listing_Standard.Gap(gap);
                if (listing_Standard.ButtonTextLabeled("Prospecting.ResetDefaults".Translate(),
                    "Prospecting.Reset".Translate()))
                {
                    doReset();
                }

                listing_Standard.End();
            }
        }

        // Token: 0x0600009F RID: 159 RVA: 0x00005DE0 File Offset: 0x00003FE0
        public void doReset()
        {
            AllowManualSound = true;
            AllowProspect = true;
            BaseChance = 50f;
            PrsLumpSizeMin = 100f;
            PrsLumpSizeMax = 100f;
            PrsCommonality = 100f;
            PrsMineYield = 100f;
            PrsDeepLumpSizeMin = 100f;
            PrsDeepLumpSizeMax = 100f;
            PrsDeepCommonality = 100f;
            PrsDeepMineYield = 100f;
        }

        // Token: 0x060000A0 RID: 160 RVA: 0x00005E60 File Offset: 0x00004060
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AllowManualSound, "AllowManualSound", true);
            Scribe_Values.Look(ref AllowProspect, "AllowProspect", true);
            Scribe_Values.Look(ref BaseChance, "BaseChance", 50f);
            Scribe_Values.Look(ref PrsLumpSizeMin, "PrsLumpSizeMin", 100f);
            Scribe_Values.Look(ref PrsLumpSizeMax, "PrsLumpSizeMax", 100f);
            Scribe_Values.Look(ref PrsCommonality, "PrsCommonality", 100f);
            Scribe_Values.Look(ref PrsMineYield, "PrsMineYield", 100f);
            Scribe_Values.Look(ref PrsDeepLumpSizeMin, "PrsDeepLumpSizeMin", 100f);
            Scribe_Values.Look(ref PrsDeepLumpSizeMax, "PrsDeepLumpSizeMax", 100f);
            Scribe_Values.Look(ref PrsDeepCommonality, "PrsDeepCommonality", 100f);
            Scribe_Values.Look(ref PrsDeepMineYield, "PrsDeepMineYield", 100f);
        }
    }
}