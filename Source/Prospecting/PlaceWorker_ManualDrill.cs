using Verse;

namespace Prospecting;

public class PlaceWorker_ManualDrill : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thing = null)
    {
        if (!loc.Walkable(map))
        {
            return false;
        }

        if (!(loc + ((ThingDef)checkingDef).interactionCellOffset).Walkable(map))
        {
            return false;
        }

        for (var i = 0; i < 9; i++)
        {
            if ((loc + GenRadial.RadialPattern[i]).Roofed(map))
            {
                return false;
            }
        }

        return true;
    }
}