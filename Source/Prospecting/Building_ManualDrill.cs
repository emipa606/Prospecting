using Verse;

namespace Prospecting;

public class Building_ManualDrill : Building
{
    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        var unused = Map;
        base.DeSpawn(mode);
        var CMD = this.TryGetComp<CompManualDrill>();
        if (CMD != null)
        {
            CompManualDrill.ResetVals(CMD);
        }
    }
}