using HarmonyLib;
using RimWorld;
using Verse;

namespace Prospecting
{
    // Token: 0x02000014 RID: 20
    [HarmonyPatch(typeof(Mineable), "TrySpawnYield")]
    public class TrySpawnYield_PostPatch
    {
        // Token: 0x06000052 RID: 82 RVA: 0x000041D8 File Offset: 0x000023D8
        [HarmonyPostfix]
        public static void PostFix(ref Mineable __instance, Map map, float yieldChance, bool moteOnWaste, Pawn pawn)
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

            var chance = (int) (Controller.Settings.BaseChance + mining);
            if (ProspectingUtility.Rnd100() <= chance &&
                ProspectingUtility.ProspectCandidate(map, __instance.Position, out var def))
            {
                ProspectingUtility.YieldExtra(pawn, def);
            }
        }
    }
}