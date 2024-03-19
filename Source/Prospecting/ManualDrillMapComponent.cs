using System;
using System.Collections.Generic;
using Verse;

namespace Prospecting;

public class ManualDrillMapComponent : MapComponent
{
    public List<string> drillList = [];

    public ManualDrillMapComponent(Map map) : base(map)
    {
        this.map = map;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref drillList, "drillList", LookMode.Value, []);
    }

    public bool GetValue(IntVec3 cell, out int maxValue)
    {
        var x = cell.x;
        var y = cell.z;
        maxValue = 0;
        var found = false;
        if (drillList.Count <= 0)
        {
            return false;
        }

        foreach (var listing in drillList)
        {
            var num = IntValuePart(listing, 0);
            var chky = IntValuePart(listing, 1);
            if (num != x || chky != y)
            {
                continue;
            }

            maxValue = IntValuePart(listing, 2);
            found = true;
            break;
        }

        return found;
    }

    public void SetValue(IntVec3 cell, int maxValue)
    {
        var x = cell.x;
        var y = cell.z;
        var found = false;
        if (drillList.Count > 0)
        {
            foreach (var value in drillList)
            {
                var chkx = IntValuePart(value, 0);
                var chky = IntValuePart(value, 1);
                if (chkx != x || chky != y)
                {
                    continue;
                }

                found = true;
                break;
            }
        }

        if (found)
        {
            return;
        }

        var strVal = string.Concat(x.ToString(), ",", y.ToString(), ",", maxValue.ToString());
        drillList.Add(strVal);
    }

    public static int IntValuePart(string value, int num)
    {
        char[] divider =
        [
            ','
        ];
        var segments = value.Split(divider);
        try
        {
            return int.Parse(segments[num]);
        }
        catch (FormatException)
        {
            Log.Message("Unable to parse Seg: '" + segments[num] + "'");
        }

        return 0;
    }
}