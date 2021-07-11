using Verse;

namespace Prospecting
{
    // Token: 0x0200001D RID: 29
    public class PlaceWorker_ManualDrill : PlaceWorker
    {
        // Token: 0x06000075 RID: 117 RVA: 0x00004B00 File Offset: 0x00002D00
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
            Thing thingToIgnore = null, Thing thing = null)
        {
            if (!loc.Walkable(map))
            {
                return false;
            }

            if (!(loc + ((ThingDef) checkingDef).interactionCellOffset).Walkable(map))
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
}