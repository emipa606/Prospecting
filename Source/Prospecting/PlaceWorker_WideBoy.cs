using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting;

public class PlaceWorker_WideBoy : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
        Thing thingToIgnore = null, Thing thing = null)
    {
        if (DeepDrillUtility.GetNextResource(loc, map) == null)
        {
            return "MustPlaceOnDrillable".Translate();
        }

        return true;
    }

    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
    {
        var currentMap = Find.CurrentMap;
        var allBuildingsColonist = currentMap.listerBuildings.allBuildingsColonist;
        foreach (var thingy in allBuildingsColonist)
        {
            if (thingy.TryGetComp<CompDeepScanner>() == null)
            {
                continue;
            }

            var compPowerTrader = thingy.TryGetComp<CompPowerTrader>();
            if (compPowerTrader == null || compPowerTrader.PowerOn)
            {
                currentMap.deepResourceGrid.MarkForDraw();
            }
        }

        var span = 49;
        var cells = new List<IntVec3>();
        for (var j = 0; j < span; j++)
        {
            if (currentMap == null)
            {
                continue;
            }

            var tempCell = center + GenRadial.ManualRadialPattern[j];
            if (tempCell.InBounds(currentMap))
            {
                cells.Add(tempCell);
            }
        }

        if (cells.Count > 0)
        {
            GenDraw.DrawFieldEdges(cells, Color.white);
        }
    }
}