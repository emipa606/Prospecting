using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace Prospecting;

[HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
public class ReverseDesignatorDatabase_InitDesignators
{
    [HarmonyPriority(0)]
    public static void Postfix(ref List<Designator> ___desList)
    {
        ___desList.Add(new Designator_Prospect());
    }
}