using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting;

[HarmonyPatch(typeof(DeepDrillUtility), nameof(DeepDrillUtility.GetNextResource), [
    typeof(IntVec3),
    typeof(Map),
    typeof(ThingDef),
    typeof(int),
    typeof(IntVec3)
], [
    ArgumentType.Normal,
    ArgumentType.Normal,
    ArgumentType.Out,
    ArgumentType.Out,
    ArgumentType.Out
])]
public class DeepDrillUtility_GetNextResource
{
    [HarmonyPriority(800)]
    public static bool Prefix(ref bool __result, IntVec3 p, Map map, out ThingDef resDef, out int countPresent,
        out IntVec3 cell)
    {
        if (ProspectingWideBoy.IsWideBoyAt(map, p, out var wideboy))
        {
            var spanWB = ProspectingWideBoy.GetWideBoySpan(wideboy);
            for (var i = 0; i < spanWB; i++)
            {
                var intVec = ProspectingWideBoy.GetDDCell(p, i);
                if (!intVec.InBounds(map))
                {
                    continue;
                }

                var thingDef = map.deepResourceGrid.ThingDefAt(intVec);
                if (thingDef == null)
                {
                    continue;
                }

                resDef = thingDef;
                countPresent = map.deepResourceGrid.CountAt(intVec);
                cell = intVec;
                __result = true;
                return false;
            }

            resDef = DeepDrillUtility.GetBaseResource(map, p);
            countPresent = int.MaxValue;
            cell = p;
            __result = false;
            return false;
        }

        resDef = DeepDrillUtility.GetBaseResource(map, p);
        countPresent = int.MaxValue;
        cell = p;
        __result = false;
        return true;
    }
}