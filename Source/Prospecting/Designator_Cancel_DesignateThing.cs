using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting;

[HarmonyPatch(typeof(Designator_Cancel), nameof(Designator_Cancel.DesignateThing))]
public class Designator_Cancel_DesignateThing
{
    [HarmonyPriority(0)]
    public static void Postfix(Thing t)
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