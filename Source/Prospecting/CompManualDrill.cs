using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting;

public class CompManualDrill : ThingComp
{
    private const float WorkPerPortionBase = 12000f;

    private float barrelsAngle;

    private int lastUsedTick = -99999;

    private int maxCount;

    private float portionProgress;

    private float portionYieldPct;

    public bool prospected;

    private float prospectProgress;

    private float prospectYieldPct;

    private float roofFactor = 1f;

    private float windFactor = 1f;

    public bool windOk = true;

    private static float WorkPerPortionCurrentDifficulty => 12000f / Find.Storyteller.difficulty.mineYieldFactor;

    private float ProgressToNextPortionPercent => portionProgress / WorkPerPortionCurrentDifficulty;

    private float ProgressToProspectingPercent => prospectProgress / WorkPerPortionCurrentDifficulty;

    private Building drill => parent as Building;

    public CompProperties_ManualDrill MDProps => (CompProperties_ManualDrill)props;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref maxCount, "maxCount");
        Scribe_Values.Look(ref prospected, "prospected");
        Scribe_Values.Look(ref portionProgress, "portionProgress");
        Scribe_Values.Look(ref prospectProgress, "prospectProgress");
        Scribe_Values.Look(ref portionYieldPct, "portionYieldPct");
        Scribe_Values.Look(ref prospectYieldPct, "prospectYieldPct");
        Scribe_Values.Look(ref lastUsedTick, "lastusedTick");
        Scribe_Values.Look(ref windOk, "windOk", true);
        Scribe_Values.Look(ref roofFactor, "roofFactor", 1f);
        Scribe_Values.Look(ref windFactor, "windFactor", 1f);
        Scribe_Values.Look(ref barrelsAngle, "barrelsAngle");
    }

    public override void CompTick()
    {
        base.CompTick();
        if (!drill.IsHashIntervalTick(240))
        {
            return;
        }

        ManualDrillUtility.GetPerformFactor(drill, out var noWind, out var newroofFactor, out var newwindFactor);
        windOk = !noWind;
        roofFactor = newroofFactor;
        windFactor = newwindFactor;
    }

    public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
    {
        prospected = false;
        maxCount = 0;
        prospectProgress = 0f;
        prospectYieldPct = 0f;
        portionProgress = 0f;
        portionYieldPct = 0f;
        lastUsedTick = -99999;
        windOk = true;
        roofFactor = 1f;
        windFactor = 1f;
    }

    public void ManualDrillWorkDone(Pawn driller, Building building)
    {
        var performFactor =
            ManualDrillUtility.GetPerformFactor(building, out var noWind, out var newroofFactor,
                out var newwindFactor);
        if (!noWind)
        {
            var statValue = driller.GetStatValue(StatDefOf.MiningSpeed) / 2f * performFactor;
            portionProgress += statValue;
            portionYieldPct += statValue * driller.GetStatValue(StatDefOf.MiningYield) /
                               WorkPerPortionCurrentDifficulty;
        }

        lastUsedTick = Find.TickManager.TicksGame;
        if (portionProgress > WorkPerPortionCurrentDifficulty)
        {
            TryManualProducePortion(portionYieldPct, building);
            portionProgress = 0f;
            portionYieldPct = 0f;
        }

        windOk = !noWind;
        roofFactor = newroofFactor;
        windFactor = newwindFactor;
    }

    public void ManualProspectWorkDone(Pawn driller, Building building)
    {
        var performFactor =
            ManualDrillUtility.GetPerformFactor(building, out var noWind, out var newroofFactor,
                out var newwindFactor);
        if (!noWind)
        {
            var statValue = driller.GetStatValue(StatDefOf.MiningSpeed) / 2f * performFactor;
            prospectProgress += statValue;
            prospectYieldPct += statValue * driller.GetStatValue(StatDefOf.MiningYield) /
                                WorkPerPortionCurrentDifficulty;
        }

        lastUsedTick = Find.TickManager.TicksGame;
        if (prospectProgress > WorkPerPortionCurrentDifficulty)
        {
            if (SetMaxCount(building, 9, out var foundDef, out var maxVal))
            {
                if (foundDef != null)
                {
                    TryManualProducePortion(prospectYieldPct, building);
                    string labelone = "Prospecting.AProspector".Translate();
                    string labeltwo = "Prospecting.Unknown".Translate();
                    if (driller.LabelShort != null)
                    {
                        labelone = driller.LabelShort;
                    }

                    if (foundDef.label != null)
                    {
                        labeltwo = foundDef.label;
                    }

                    Messages.Message(
                        "Prospecting.DrillProspect".Translate(labelone) +
                        "Prospecting.ResourceFound".Translate(labeltwo), building, MessageTypeDefOf.PositiveEvent);
                }

                maxCount = maxVal;
            }

            prospectProgress = 0f;
            prospectYieldPct = 0f;
            prospected = true;
        }

        windOk = !noWind;
        roofFactor = newroofFactor;
        windFactor = newwindFactor;
    }

    private bool SetMaxCount(Building building, int numCells, out ThingDef resFound, out int MaxValue)
    {
        resFound = null;
        MaxValue = 0;
        var found = false;
        if (building is { Map: null, Spawned: true })
        {
            return false;
        }

        var drillpoint = building.TrueCenter().ToIntVec3();
        var mapComp = building.Map?.GetComponent<ManualDrillMapComponent>();
        if (mapComp == null)
        {
            return false;
        }

        for (var i = 0; i < numCells; i++)
        {
            var pos = drillpoint + GenRadial.RadialPattern[i];
            var MaxValueAt = 0;
            var MaxExists = mapComp.GetValue(pos, out var maxVal);
            var resFoundAt = building.Map.deepResourceGrid.ThingDefAt(pos);
            if (!MaxExists)
            {
                if (resFoundAt != null)
                {
                    MaxValueAt = building.Map.deepResourceGrid.CountAt(drillpoint);
                }
            }
            else if (resFoundAt != null)
            {
                MaxValueAt = maxVal;
            }

            if (!found && resFoundAt != null)
            {
                resFound = resFoundAt;
                found = true;
            }

            if (MaxValueAt <= 0 || resFoundAt == null)
            {
                continue;
            }

            if (!MaxExists)
            {
                mapComp.SetValue(pos, MaxValueAt);
            }

            MaxValue += MaxValueAt;
        }

        return false;
    }

    private void TryManualProducePortion(float yieldPct, Building building)
    {
        var nextResource = GetNextManualResource(building, out var resDef, out var countPresent, out var cell);
        if (resDef == null)
        {
            return;
        }

        var num = Mathf.Min(countPresent, (int)Mathf.Max(1f, resDef.deepCountPerPortion / 2f));
        if (nextResource)
        {
            var newCount = building.Map.deepResourceGrid.CountAt(cell) - num;
            parent.Map.deepResourceGrid.SetAt(cell, resDef, newCount);
        }

        var stackCount = Mathf.Max(1, GenMath.RoundRandom(num * yieldPct));
        var thing = ThingMaker.MakeThing(resDef);
        thing.stackCount = stackCount;
        GenPlace.TryPlaceThing(thing, parent.InteractionCell, parent.Map, ThingPlaceMode.Near);
        var CMD = building.TryGetComp<CompManualDrill>();
        if (CMD == null)
        {
            return;
        }

        if (!nextResource ||
            ManualDrillUtility.DrillCanGetToCount(building, CMD.MDProps.shallowReach, 9, out _, out _) > 0)
        {
            return;
        }

        if (CMD.MDProps.mineRock &&
            DeepDrillUtility.GetBaseResource(parent.Map, parent.TrueCenter().ToIntVec3()) == null)
        {
            Messages.Message("Prospecting.ManualDrillExhaustedNoFallback".Translate(), parent,
                MessageTypeDefOf.TaskCompletion);
            return;
        }

        Messages.Message(
            "Prospecting.ManualDrillExhausted".Translate(Find.ActiveLanguageWorker.Pluralize(DeepDrillUtility
                .GetBaseResource(parent.Map, parent.TrueCenter().ToIntVec3()).label)), parent,
            MessageTypeDefOf.TaskCompletion);
        building.SetForbidden(true);
    }

    private bool GetNextManualResource(Building building, out ThingDef resDef, out int countPresent,
        out IntVec3 cell)
    {
        return ManualDrillUtility.GetNextManualResource(building, building.TrueCenter().ToIntVec3(), building.Map,
            out resDef, out countPresent, out cell);
    }

    public override string CompInspectStringExtra()
    {
        var progressMsg = "";
        var resMsg = "\n";
        var weatherMsg = "";
        if (drill.Spawned)
        {
            var drillpoint = drill.TrueCenter().ToIntVec3();
            if (prospected)
            {
                var resources = ManualDrillUtility.DrillCanGetToCount(drill, MDProps.shallowReach, 9, out _,
                    out _);
                if (resources > 0)
                {
                    var resDef = drill.Map.deepResourceGrid.ThingDefAt(drillpoint);
                    if (resDef != null)
                    {
                        progressMsg = "ResourceBelow".Translate() + ": " + resDef.LabelCap + "\n" +
                                      "ProgressToNextPortion".Translate() + ": " +
                                      ProgressToNextPortionPercent.ToStringPercent("F0");
                        resMsg += "Prospecting.DeepResources".Translate(resources.ToString(),
                            MDProps.shallowReach.ToStringPercent());
                    }
                }
                else
                {
                    var zero = 0;
                    progressMsg = "DeepDrillNoResources".Translate();
                    resMsg += "Prospecting.DeepResources".Translate(zero.ToString(),
                        MDProps.shallowReach.ToStringPercent());
                }
            }
            else
            {
                progressMsg = "Prospecting.DrillNotProspected".Translate() + "\n" +
                              "Prospecting.ProgressProspect".Translate() + ": " +
                              ProgressToProspectingPercent.ToStringPercent("F0");
                resMsg = "";
            }

            if (!windOk)
            {
                weatherMsg = "\n" + "Prospecting.InsufficientWind".Translate();
            }
            else if (UsedLastTick())
            {
                var roofpct = roofFactor / 1f;
                var windpct = windFactor / 1f;
                weatherMsg = "\n" + "Prospecting.RoofPerform".Translate(roofpct.ToStringPercent("F0")) + ", " +
                             "Prospecting.WindPerform".Translate(windpct.ToStringPercent("F0"));
            }
        }
        else
        {
            progressMsg = "Prospecting.InstallDrill".Translate();
            resMsg = "";
        }

        return progressMsg + resMsg + weatherMsg;
    }

    private bool UsedLastTick()
    {
        return lastUsedTick >= Find.TickManager.TicksGame - 1;
    }

    public static void ResetVals(CompManualDrill cmd)
    {
        cmd.prospected = false;
        cmd.maxCount = 0;
    }

    public override void PostDraw()
    {
        base.PostDraw();
        var rotation = parent.Rotation;
        var pos = parent.TrueCenter();
        pos.y += 0.046875f;
        if (rotation == Rot4.North)
        {
            pos.x += 0f;
            pos.z += 0.5f;
        }
        else if (rotation == Rot4.South)
        {
            pos.x += 0f;
            pos.z += -0.5f;
        }
        else if (rotation == Rot4.East)
        {
            pos.x += 0.5f;
            pos.z += 0f;
        }
        else if (rotation == Rot4.West)
        {
            pos.x += -0.5f;
            pos.z += 0f;
        }

        var s = new Vector3(3f, 0f, 3f);
        var anglePerRate = 4f;
        if (!Find.TickManager.Paused && windOk && UsedLastTick())
        {
            barrelsAngle += windFactor / 1.5f * anglePerRate;
        }

        if (barrelsAngle >= 360f)
        {
            barrelsAngle -= 360f;
        }

        if (barrelsAngle <= -360f)
        {
            barrelsAngle += 360f;
        }

        s.RotatedBy(barrelsAngle);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(pos, Quaternion.AngleAxis(barrelsAngle, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane10, matrix,
            CompManualDrillMats.BarrelTurbineMat, 0);
    }
}