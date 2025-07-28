using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting;

[HarmonyPatch(typeof(Mineable), "TrySpawnYield", typeof(Map), typeof(bool), typeof(Pawn))]
public class Mineable_TrySpawnYield
{
    [HarmonyPriority(800)]
    public static bool Prefix(ref Mineable __instance, ref float ___yieldPct, Map map, Pawn pawn)
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
        if (pawn is not { IsColonist: true } && thing.def.EverHaulable && !thing.def.designateHaulable)
        {
            thing.SetForbidden(true);
        }

        return false;
    }

    public static void Postfix(ref Mineable __instance, Map map, Pawn pawn)
    {
        if (map == null || pawn == null || !ProspectingUtility.IsValidToIndicate(__instance, map))
        {
            return;
        }

        var mining = 0;
        var skills = pawn.skills;
        var miningSkill = skills?.GetSkill(SkillDefOf.Mining) != null;

        if (miningSkill)
        {
            mining = pawn.skills.GetSkill(SkillDefOf.Mining).Level / 4;
        }

        var chance = (int)(Controller.Settings.BaseChance + mining);
        if (ProspectingUtility.Rnd100() <= chance &&
            ProspectingUtility.ProspectCandidate(map, __instance.Position, out var def))
        {
            ProspectingUtility.YieldExtra(pawn, def);
        }
    }
}