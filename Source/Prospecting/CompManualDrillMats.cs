using UnityEngine;
using Verse;

namespace Prospecting;

[StaticConstructorOnStartup]
public class CompManualDrillMats
{
    internal static readonly Material BarrelTurbineMat =
        MaterialPool.MatFrom("Things/Building/PrsManualDrill/PrsDrillWindBit");
}