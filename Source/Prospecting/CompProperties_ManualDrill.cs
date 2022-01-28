using Verse;

namespace Prospecting;

public class CompProperties_ManualDrill : CompProperties
{
    public bool mineRock;

    public float shallowReach = 0.25f;

    public CompProperties_ManualDrill()
    {
        compClass = typeof(CompManualDrill);
    }
}