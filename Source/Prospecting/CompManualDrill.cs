using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
    // Token: 0x02000006 RID: 6
    public class CompManualDrill : ThingComp
    {
        // Token: 0x0400000D RID: 13
        private const float WorkPerPortionBase = 12000f;

        // Token: 0x0400000C RID: 12
        public float barrelsAngle;

        // Token: 0x04000008 RID: 8
        private int lastUsedTick = -99999;

        // Token: 0x04000003 RID: 3
        public int maxCount;

        // Token: 0x04000004 RID: 4
        private float portionProgress;

        // Token: 0x04000006 RID: 6
        private float portionYieldPct;

        // Token: 0x04000002 RID: 2
        public bool prospected;

        // Token: 0x04000005 RID: 5
        private float prospectProgress;

        // Token: 0x04000007 RID: 7
        private float prospectYieldPct;

        // Token: 0x0400000A RID: 10
        public float roofFactor = 1f;

        // Token: 0x0400000B RID: 11
        public float windFactor = 1f;

        // Token: 0x04000009 RID: 9
        public bool windOk = true;

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x0600000A RID: 10 RVA: 0x000026C8 File Offset: 0x000008C8
        public static float WorkPerPortionCurrentDifficulty => 12000f / Find.Storyteller.difficulty.mineYieldFactor;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600000B RID: 11 RVA: 0x000026DF File Offset: 0x000008DF
        public float ProgressToNextPortionPercent => portionProgress / WorkPerPortionCurrentDifficulty;

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600000C RID: 12 RVA: 0x000026ED File Offset: 0x000008ED
        public float ProgressToProspectingPercent => prospectProgress / WorkPerPortionCurrentDifficulty;

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600000E RID: 14 RVA: 0x000027F1 File Offset: 0x000009F1
        public Building drill => parent as Building;

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x0600000F RID: 15 RVA: 0x000027FE File Offset: 0x000009FE
        public CompProperties_ManualDrill MDProps => (CompProperties_ManualDrill) props;

        // Token: 0x0600000D RID: 13 RVA: 0x000026FC File Offset: 0x000008FC
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

        // Token: 0x06000010 RID: 16 RVA: 0x0000280C File Offset: 0x00000A0C
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

        // Token: 0x06000011 RID: 17 RVA: 0x0000285C File Offset: 0x00000A5C
        public override void PostDeSpawn(Map map)
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

        // Token: 0x06000012 RID: 18 RVA: 0x000028CC File Offset: 0x00000ACC
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

        // Token: 0x06000013 RID: 19 RVA: 0x00002990 File Offset: 0x00000B90
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

        // Token: 0x06000014 RID: 20 RVA: 0x00002B04 File Offset: 0x00000D04
        private bool SetMaxCount(Building building, int numCells, out ThingDef resFound, out int MaxValue)
        {
            resFound = null;
            MaxValue = 0;
            var found = false;
            if (building?.Map == null || !building.Spawned)
            {
                return false;
            }

            var drillpoint = building.TrueCenter().ToIntVec3();
            var mapComp = building.Map.GetComponent<ManualDrillMapComponent>();
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

        // Token: 0x06000015 RID: 21 RVA: 0x00002BF0 File Offset: 0x00000DF0
        private void TryManualProducePortion(float yieldPct, Building building)
        {
            var nextResource = GetNextManualResource(building, out var resDef, out var countPresent, out var cell);
            if (resDef == null)
            {
                return;
            }

            var num = Mathf.Min(countPresent, (int) Mathf.Max(1f, resDef.deepCountPerPortion / 2f));
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

        // Token: 0x06000016 RID: 22 RVA: 0x00002D8F File Offset: 0x00000F8F
        private bool GetNextManualResource(Building building, out ThingDef resDef, out int countPresent,
            out IntVec3 cell)
        {
            return ManualDrillUtility.GetNextManualResource(building, building.TrueCenter().ToIntVec3(), building.Map,
                out resDef, out countPresent, out cell);
        }

        // Token: 0x06000017 RID: 23 RVA: 0x00002DAC File Offset: 0x00000FAC
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

        // Token: 0x06000018 RID: 24 RVA: 0x00003033 File Offset: 0x00001233
        public bool UsedLastTick()
        {
            return lastUsedTick >= Find.TickManager.TicksGame - 1;
        }

        // Token: 0x06000019 RID: 25 RVA: 0x0000304C File Offset: 0x0000124C
        public static void ResetVals(CompManualDrill cmd)
        {
            cmd.prospected = false;
            cmd.maxCount = 0;
        }

        // Token: 0x0600001A RID: 26 RVA: 0x0000305C File Offset: 0x0000125C
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
}