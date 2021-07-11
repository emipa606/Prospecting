using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting
{
    // Token: 0x0200000D RID: 13
    [HarmonyPatch(typeof(Designator_Cancel), "DesignateThing")]
    public class CancelDesignateThing_Patch
    {
        // Token: 0x06000045 RID: 69 RVA: 0x00003D9C File Offset: 0x00001F9C
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
}