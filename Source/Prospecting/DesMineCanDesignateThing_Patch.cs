using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting;

[HarmonyPatch(typeof(Designator_Mine), "CanDesignateThing")]
public class DesMineCanDesignateThing_Patch
{
    [HarmonyPostfix]
    [HarmonyPriority(0)]
    public static void PostFix(ref AcceptanceReport __result, Thing t)
    {
        if (!t.def.mineable)
        {
            __result = false;
        }

        if (t.Map == null)
        {
            return;
        }

        var desig = ProspectDef.Prospect;
        if (t.Map.designationManager.DesignationAt(t.Position, desig) != null)
        {
            __result = AcceptanceReport.WasRejected;
        }
    }
}