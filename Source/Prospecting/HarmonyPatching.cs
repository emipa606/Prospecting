using System.Reflection;
using HarmonyLib;
using Verse;

namespace Prospecting
{
    // Token: 0x02000011 RID: 17
    [StaticConstructorOnStartup]
    internal static class HarmonyPatching
    {
        // Token: 0x0600004D RID: 77 RVA: 0x0000407F File Offset: 0x0000227F
        static HarmonyPatching()
        {
            new Harmony("com.Pelador.Rimworld.Prospecting").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}