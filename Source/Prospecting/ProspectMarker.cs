using System.Text;
using Verse;

namespace Prospecting
{
    // Token: 0x02000024 RID: 36
    public class ProspectMarker : Building
    {
        // Token: 0x04000039 RID: 57
        private const int NumExpireTicks = 60000;

        // Token: 0x0400003A RID: 58
        private int TicksTillExpire;

        // Token: 0x06000095 RID: 149 RVA: 0x00005621 File Offset: 0x00003821
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref TicksTillExpire, "TicksTillExpire", 60000);
        }

        // Token: 0x06000096 RID: 150 RVA: 0x0000563F File Offset: 0x0000383F
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                TicksTillExpire = 60000;
            }
        }

        // Token: 0x06000097 RID: 151 RVA: 0x00005658 File Offset: 0x00003858
        public override void Tick()
        {
            base.Tick();
            var interval = 240;
            if (!this.IsHashIntervalTick(interval))
            {
                return;
            }

            TicksTillExpire -= interval;
            if (TicksTillExpire <= 0)
            {
                Destroy();
            }
        }

        // Token: 0x06000098 RID: 152 RVA: 0x00005698 File Offset: 0x00003898

        // Token: 0x06000099 RID: 153 RVA: 0x000056A0 File Offset: 0x000038A0
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
}