using Verse;

namespace Prospecting;

public static class ProspectingWideBoy
{
    public static IntVec3 GetDDCell(IntVec3 p, int i)
    {
        return p + GenRadial.ManualRadialPattern[i];
    }

    public static bool IsWideBoyAt(Map map, IntVec3 p, out Thing wideboy)
    {
        wideboy = null;
        if (map == null)
        {
            return false;
        }

        wideboy = p.GetFirstThingWithComp<CompWideBoy>(map);
        return wideboy != null;
    }

    public static int GetWideBoySpan(Thing wideboy)
    {
        var spanValue = 9;
        if (wideboy.TryGetComp<CompWideBoy>() != null)
        {
            spanValue = wideboy.TryGetComp<CompWideBoy>().span;
        }

        return spanValue;
    }
}