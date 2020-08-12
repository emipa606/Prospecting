using System;
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
			if (map != null && pawn != null && ProspectingUtility.IsValidToIndicate(__instance, map))
			{
				int mining = 0;
				bool flag;
				if (pawn == null)
				{
					flag = (null != null);
				}
				else
				{
					Pawn_SkillTracker skills = pawn.skills;
					flag = (((skills != null) ? skills.GetSkill(SkillDefOf.Mining) : null) != null);
				}
				if (flag)
				{
					mining = pawn.skills.GetSkill(SkillDefOf.Mining).Level / 4;
				}
				int chance = (int)(Controller.Settings.BaseChance + (float)mining);
				ThingDef def;
				if (ProspectingUtility.Rnd100() <= chance && ProspectingUtility.ProspectCandidate(map, __instance.Position, out def))
				{
					ProspectingUtility.YieldExtra(pawn, def);
				}
			}
		}
	}
}
