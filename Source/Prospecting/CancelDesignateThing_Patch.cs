using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting;

[HarmonyPatch(typeof(Designator_Cancel), "DesignateThing")]
public class CancelDesignateThing_Patch
{
    [HarmonyPostfix]
    [HarmonyPriority(0)]
    public static void PostFix(Thing t)
    {
        if (!t.def.mineable)
        {
            return;
        }

        var desig = ProspectDef.Prospect;
        var designation = t.Map.designationManager.DesignationAt(t.Position, desig);
        if (designation != null)
        {
            t.Map.designationManager.RemoveDesignation(designation);
        }
    }
}