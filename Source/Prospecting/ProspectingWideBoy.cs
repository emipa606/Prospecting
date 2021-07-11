using Verse;

namespace Prospecting
{
    // Token: 0x02000023 RID: 35
    public static class ProspectingWideBoy
    {
        // Token: 0x06000092 RID: 146 RVA: 0x000055CE File Offset: 0x000037CE
        public static IntVec3 GetDDCell(IntVec3 p, int i)
        {
            return p + GenRadial.ManualRadialPattern[i];
        }

        // Token: 0x06000093 RID: 147 RVA: 0x000055E1 File Offset: 0x000037E1
        public static bool IsWideBoyAt(Map map, IntVec3 p, out Thing wideboy)
        {
            wideboy = null;
            if (map == null)
            {
                return false;
            }

            wideboy = p.GetFirstThingWithComp<CompWideBoy>(map);
            if (wideboy != null)
            {
                return true;
            }

            return false;
        }

        // Token: 0x06000094 RID: 148 RVA: 0x000055FC File Offset: 0x000037FC
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
}