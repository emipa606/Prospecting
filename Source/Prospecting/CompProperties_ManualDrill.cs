using Verse;

namespace Prospecting;

public class CompProperties_ManualDrill : CompProperties
{
    public readonly float shallowReach = 0.25f;
    public bool mineRock;

    public CompProperties_ManualDrill()
    {
        compClass = typeof(CompManualDrill);
    }
}