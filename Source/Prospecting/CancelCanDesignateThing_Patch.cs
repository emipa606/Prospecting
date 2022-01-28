using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting;

[HarmonyPatch(typeof(Designator_Cancel), "CanDesignateThing")]
public class CancelCanDesignateThing_Patch
{
    [HarmonyPostfix]
    [HarmonyPriority(0)]
    public static void PostFix(ref AcceptanceReport __result, Thing t)
    {
        var desig = ProspectDef.Prospect;
        if (t.def.mineable && t.Map.designationManager.DesignationAt(t.Position, desig) != null)
        {
            __result = true;
        }
    }
}