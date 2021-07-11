using Verse;

namespace Prospecting
{
    // Token: 0x02000007 RID: 7
    public class CompProperties_ManualDrill : CompProperties
    {
        // Token: 0x0400000F RID: 15
        public bool mineRock;

        // Token: 0x0400000E RID: 14
        public float shallowReach = 0.25f;

        // Token: 0x0600001C RID: 28 RVA: 0x0000325F File Offset: 0x0000145F
        public CompProperties_ManualDrill()
        {
            compClass = typeof(CompManualDrill);
        }
    }
}