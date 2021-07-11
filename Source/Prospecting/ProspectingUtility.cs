using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
    // Token: 0x02000022 RID: 34
    public class ProspectingUtility
    {
        // Token: 0x0600008A RID: 138 RVA: 0x00005326 File Offset: 0x00003526
        public static bool IsValidToIndicate(Mineable m, Map map)
        {
            return !m.def.building.isResourceRock;
        }

        // Token: 0x0600008B RID: 139 RVA: 0x00005340 File Offset: 0x00003540
        public static bool ProspectCandidate(Map map, IntVec3 root, out ThingDef prospectDef)
        {
            var candidates = new List<ThingDef>();
            prospectDef = null;
            if (map == null)
            {
                return false;
            }

            var cells = GenRadial.RadialCellsAround(root, 2f, false).ToList();
            if (cells.Count > 0)
            {
                foreach (var cell in cells)
                {
                    if (!cell.InBounds(map))
                    {
                        continue;
                    }

                    Building mineable = cell.GetFirstMineable(map);
                    if (mineable == null || !mineable.def.building.isResourceRock)
                    {
                        continue;
                    }

                    var def = mineable.def;
                    ThingDef thingDef;
                    if (def == null)
                    {
                        thingDef = null;
                    }
                    else
                    {
                        var building = def.building;
                        thingDef = building?.mineableThing;
                    }

                    var mineThingDef = thingDef;
                    if (mineThingDef == null || IsException(mineThingDef))
                    {
                        continue;
                    }

                    if (mineThingDef == ThingDefOf.ComponentIndustrial ||
                        mineThingDef.defName == "ChunkSlagSteel")
                    {
                        candidates.AddDistinct(ThingDefOf.Steel);
                    }
                    else
                    {
                        candidates.AddDistinct(mineThingDef);
                    }
                }
            }

            if (candidates.Count <= 0)
            {
                return false;
            }

            prospectDef = candidates.RandomElement();
            if (prospectDef != null)
            {
                return true;
            }

            return false;
        }

        // Token: 0x0600008C RID: 140 RVA: 0x0000545C File Offset: 0x0000365C
        public static void YieldExtra(Pawn pawn, ThingDef bitsdef)
        {
            var num = Mathf.Max(1, Mathf.RoundToInt(RndBits(1, 3) * Find.Storyteller.difficulty.mineYieldFactor));
            var bitsthing = ThingMaker.MakeThing(bitsdef);
            bitsthing.stackCount = bitsthing.def.stackLimit > 1 ? num : 1;

            GenDrop.TryDropSpawn(bitsthing, pawn.Position, pawn.Map, ThingPlaceMode.Near, out var newbitsthing);
            if (!pawn.IsColonist && newbitsthing.def.EverHaulable &&
                !newbitsthing.def.designateHaulable)
            {
                newbitsthing.SetForbidden(true);
            }
        }

        // Token: 0x0600008D RID: 141 RVA: 0x000054F8 File Offset: 0x000036F8
        public static bool IsException(ThingDef def)
        {
            if (def?.thingCategories == null)
            {
                return def != null && Exceptions().Contains(def.defName);
            }

            using (var enumerator = def.thingCategories.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current?.defName == "StoneChunks")
                    {
                        return true;
                    }
                }
            }

            return Exceptions().Contains(def.defName);
        }

        // Token: 0x0600008E RID: 142 RVA: 0x00005580 File Offset: 0x00003780
        public static List<string> Exceptions()
        {
            var list = new List<string>();
            list.AddDistinct("CollapsedRocks");
            list.AddDistinct("rxFoamWall");
            list.AddDistinct("rxFoamWallBricks");
            list.AddDistinct("rxCollapsedRoofRocks");
            return list;
        }

        // Token: 0x0600008F RID: 143 RVA: 0x000055B3 File Offset: 0x000037B3
        public static int RndBits(int min, int max)
        {
            return Rand.Range(min, max);
        }

        // Token: 0x06000090 RID: 144 RVA: 0x000055BC File Offset: 0x000037BC
        public static int Rnd100()
        {
            return Rand.Range(1, 100);
        }
    }
}