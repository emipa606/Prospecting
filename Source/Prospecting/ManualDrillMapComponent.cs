using System;
using System.Collections.Generic;
using Verse;

namespace Prospecting
{
    // Token: 0x0200001A RID: 26
    public class ManualDrillMapComponent : MapComponent
    {
        // Token: 0x0400002A RID: 42
        public List<string> drillList = new List<string>();

        // Token: 0x06000067 RID: 103 RVA: 0x000044F3 File Offset: 0x000026F3
        public ManualDrillMapComponent(Map map) : base(map)
        {
            this.map = map;
        }

        // Token: 0x06000068 RID: 104 RVA: 0x0000450E File Offset: 0x0000270E
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref drillList, "drillList", LookMode.Value, Array.Empty<object>());
        }

        // Token: 0x06000069 RID: 105 RVA: 0x0000452C File Offset: 0x0000272C
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

        // Token: 0x0600006A RID: 106 RVA: 0x000045C4 File Offset: 0x000027C4
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

        // Token: 0x0600006B RID: 107 RVA: 0x00004698 File Offset: 0x00002898
        public static int IntValuePart(string value, int num)
        {
            char[] divider =
            {
                ','
            };
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
}