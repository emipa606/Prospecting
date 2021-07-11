using RimWorld;
using Verse;

namespace Prospecting
{
    // Token: 0x02000020 RID: 32
    [DefOf]
    public class ProspectDef
    {
        // Token: 0x0400002E RID: 46
        public static DesignationDef Prospect;

        // Token: 0x0400002F RID: 47
        public static WorkTypeDef Prospecting;

        // Token: 0x04000030 RID: 48
        public static JobDef ProspectSurface;

        // Token: 0x04000031 RID: 49
        public static JobDef ProspectBelt;

        // Token: 0x04000032 RID: 50
        public static JobDef OperateWideBoy;

        // Token: 0x04000033 RID: 51
        public static JobDef ManualDrillProspect;

        // Token: 0x04000034 RID: 52
        public static JobDef ManualDrillMine;

        // Token: 0x04000035 RID: 53
        public static ThingDef PrsWideDeepDrill;

        // Token: 0x04000036 RID: 54
        public static ThingDef PrsManualDrill;

        // Token: 0x04000037 RID: 55
        public static ThingDef PrsProspectMarker;

        // Token: 0x04000038 RID: 56
        public static EffecterDef PrsManualDrillEff;

        // Token: 0x06000083 RID: 131 RVA: 0x000050F9 File Offset: 0x000032F9
        static ProspectDef()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ProspectDef));
        }
    }
}