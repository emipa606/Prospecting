using System.Reflection;
using HarmonyLib;
using Verse;

namespace Prospecting;

[StaticConstructorOnStartup]
internal static class HarmonyPatching
{
    static HarmonyPatching()
    {
        new Harmony("com.Pelador.Rimworld.Prospecting").PatchAll(Assembly.GetExecutingAssembly());
    }
}