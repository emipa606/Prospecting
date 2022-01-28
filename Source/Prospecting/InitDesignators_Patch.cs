using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace Prospecting;

[HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
public class InitDesignators_Patch
{
    [HarmonyPostfix]
    [HarmonyPriority(0)]
    public static void PostFix(ref List<Designator> ___desList)
    {
        ___desList.Add(new Designator_Prospect());
    }
}