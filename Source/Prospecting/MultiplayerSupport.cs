using System;
using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace Prospecting
{
	// Token: 0x0200001C RID: 28
	[StaticConstructorOnStartup]
	internal static class MultiplayerSupport
	{
		// Token: 0x06000071 RID: 113 RVA: 0x000049CC File Offset: 0x00002BCC
		static MultiplayerSupport()
		{
			if (!MP.enabled)
			{
				return;
			}
			MP.RegisterSyncMethod(typeof(CompWideBoy), "SetSpanWB", null);
			MP.RegisterSyncMethod(typeof(CompWideBoy), "ToggleMineRock", null);
			MP.RegisterSyncMethod(typeof(CompWideBoy), "ToggleBoost", null);
			MP.RegisterSyncMethod(typeof(ProspectBelt), "DoBeltSelect", null);
			MP.RegisterSyncMethod(typeof(ProspectBelt), "PrsUseBelt", null);
			MethodInfo[] array = new MethodInfo[]
			{
				AccessTools.Method(typeof(ProspectingUtility), "RndBits", null, null),
				AccessTools.Method(typeof(ProspectingUtility), "Rnd100", null, null)
			};
			for (int i = 0; i < array.Length; i++)
			{
				MultiplayerSupport.FixRNG(array[i]);
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004AAD File Offset: 0x00002CAD
		private static void FixRNG(MethodInfo method)
		{
			MultiplayerSupport.harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre", null), new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos", null), null, null);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004AE7 File Offset: 0x00002CE7
		private static void FixRNGPre()
		{
			Rand.PushState(Find.TickManager.TicksAbs);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004AF8 File Offset: 0x00002CF8
		private static void FixRNGPos()
		{
			Rand.PopState();
		}

		// Token: 0x0400002C RID: 44
		private static Harmony harmony = new Harmony("rimworld.pelador.prospecting.multiplayersupport");
	}
}
