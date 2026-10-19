using UnityEngine;
using Verse;

namespace Prospecting;

[StaticConstructorOnStartup]
public static class CompManualDrillMats
{
    internal static readonly Material BarrelTurbineMat =
        MaterialPool.MatFrom("Things/Building/PrsManualDrill/PrsDrillWindBit");
}