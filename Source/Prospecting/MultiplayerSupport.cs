using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace Prospecting;

[StaticConstructorOnStartup]
internal static class MultiplayerSupport
{
    private static readonly Harmony harmony = new Harmony("rimworld.pelador.prospecting.multiplayersupport");

    static MultiplayerSupport()
    {
        if (!MP.enabled)
        {
            return;
        }

        MP.RegisterSyncMethod(typeof(CompWideBoy), "SetSpanWB");
        MP.RegisterSyncMethod(typeof(CompWideBoy), "ToggleMineRock");
        MP.RegisterSyncMethod(typeof(CompWideBoy), "ToggleBoost");
        MP.RegisterSyncMethod(typeof(ProspectBelt), "DoBeltSelect");
        MP.RegisterSyncMethod(typeof(ProspectBelt), "PrsUseBelt");
        MethodInfo[] array =
        {
            AccessTools.Method(typeof(ProspectingUtility), "RndBits"),
            AccessTools.Method(typeof(ProspectingUtility), "Rnd100")
        };
        foreach (var methodInfo in array)
        {
            FixRNG(methodInfo);
        }
    }

    private static void FixRNG(MethodInfo method)
    {
        harmony.Patch(method, new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPre"),
            new HarmonyMethod(typeof(MultiplayerSupport), "FixRNGPos"));
    }

    private static void FixRNGPre()
    {
        Rand.PushState(Find.TickManager.TicksAbs);
    }

    private static void FixRNGPos()
    {
        Rand.PopState();
    }
}