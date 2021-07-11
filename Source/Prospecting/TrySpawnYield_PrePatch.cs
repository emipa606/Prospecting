using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
    // Token: 0x02000013 RID: 19
    [HarmonyPatch(typeof(Mineable), "TrySpawnYield")]
    public class TrySpawnYield_PrePatch
    {
        // Token: 0x06000050 RID: 80 RVA: 0x000040AC File Offset: 0x000022AC
        [HarmonyPrefix]
        [HarmonyPriority(800)]
        public static bool PreFix(ref Mineable __instance, ref float ___yieldPct, Map map, float yieldChance,
            bool moteOnWaste, Pawn pawn)
        {
            if (__instance.def.building.mineableThing == null ||
                !(Rand.Value <= __instance.def.building.mineableDropChance) || !__instance.def.building.isResourceRock)
            {
                return true;
            }

            var num = Mathf.Max(1,
                Mathf.RoundToInt(
                    __instance.def.building.mineableYield * Find.Storyteller.difficulty.mineYieldFactor));
            if (__instance.def.building.mineableYieldWasteable)
            {
                num = Mathf.Max(1, GenMath.RoundRandom(num * ___yieldPct));
            }

            var thing = ThingMaker.MakeThing(__instance.def.building.mineableThing);
            if (num > thing.def.stackLimit)
            {
                num = thing.def.stackLimit;
            }

            thing.stackCount = num;
            GenSpawn.Spawn(thing, __instance.Position, map);
            if ((pawn == null || !pawn.IsColonist) && thing.def.EverHaulable && !thing.def.designateHaulable)
            {
                thing.SetForbidden(true);
            }

            return false;
        }
    }
}