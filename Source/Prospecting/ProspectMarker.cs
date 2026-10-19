using System.Text;
using RimWorld;
using Verse;

namespace Prospecting;

public class ProspectMarker : Building
{
    private int TicksTillExpire;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref TicksTillExpire, "TicksTillExpire", GenDate.TicksPerDay);
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (!respawningAfterLoad)
        {
            TicksTillExpire = GenDate.TicksPerDay;
        }
    }

    protected override void Tick()
    {
        base.Tick();
        if (!this.IsHashIntervalTick(GenTicks.TickRareInterval))
        {
            return;
        }

        TicksTillExpire -= GenTicks.TickRareInterval;
        if (TicksTillExpire <= 0)
        {
            Destroy();
        }
    }


    public override string GetInspectString()
    {
        string returnString = null;
        string resString = null;
        if (Map != null && Spawned)
        {
            var resDef = Map.deepResourceGrid.ThingDefAt(Position);
            if (resDef != null)
            {
                string resLabel = resDef.LabelCap;
                var resCount = Map.deepResourceGrid.CountAt(Position).ToString();
                var resTime = (TicksTillExpire / 2500f).ToString("F1");
                resString = "Prospecting.MarkerDisplay".Translate(resLabel, resCount, resTime);
            }
        }

        if (resString != null)
        {
            returnString = resString;
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetInspectString());
        var text = InspectStringPartsFromComps();
        if (!text.NullOrEmpty())
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.AppendLine();
            }

            stringBuilder.Append(text);
        }

        if (returnString != null)
        {
            returnString += stringBuilder.ToString();
        }
        else
        {
            returnString = stringBuilder.ToString();
        }

        return returnString;
    }
}