using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting;

[HarmonyPatch(typeof(Mineable), "TrySpawnYield", typeof(Map), typeof(bool), typeof(Pawn))]
public class TrySpawnYield_PostPatch
{
    [HarmonyPostfix]
    public static void PostFix(ref Mineable __instance, Map map, Pawn pawn)
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