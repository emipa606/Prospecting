using RimWorld;
using Verse;

namespace Prospecting;

[DefOf]
public class ProspectDef
{
    public static DesignationDef Prospect;

    public static WorkTypeDef Prospecting;

    public static JobDef ProspectSurface;

    public static JobDef ProspectBelt;

    public static JobDef OperateWideBoy;

    public static JobDef ManualDrillProspect;

    public static JobDef ManualDrillMine;

    public static ThingDef PrsWideDeepDrill;

    public static ThingDef PrsManualDrill;

    public static ThingDef PrsProspectMarker;

    public static EffecterDef PrsManualDrillEff;

    static ProspectDef()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ProspectDef));
    }
}