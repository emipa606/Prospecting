using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting;

public class ProspectingUtility
{
    public static bool IsValidToIndicate(Mineable m, Map map)
    {
        return !m.def.building.isResourceRock;
    }

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

    public static List<string> Exceptions()
    {
        var list = new List<string>();
        list.AddDistinct("CollapsedRocks");
        list.AddDistinct("rxFoamWall");
        list.AddDistinct("rxFoamWallBricks");
        list.AddDistinct("rxCollapsedRoofRocks");
        return list;
    }

    public static int RndBits(int min, int max)
    {
        return Rand.Range(min, max);
    }

    public static int Rnd100()
    {
        return Rand.Range(1, 100);
    }
}