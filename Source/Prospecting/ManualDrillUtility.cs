﻿using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
    // Token: 0x0200001B RID: 27
    public class ManualDrillUtility
    {
        // Token: 0x0400002B RID: 43
        public const int drillCellsNum = 9;

        // Token: 0x0600006C RID: 108 RVA: 0x000046F4 File Offset: 0x000028F4
        public static int DrillCanGetToCount(Building drill, float shallowFactor, int numCells, out ThingDef resDef,
            out IntVec3 nextCell)
        {
            var resCount = 0;
            resDef = null;
            nextCell = drill.TrueCenter().ToIntVec3();
            shallowFactor = Math.Max(0.1f, Math.Min(0.33f, shallowFactor));
            if (drill == null || !drill.Spawned || drill.Map == null)
            {
                return resCount;
            }

            var map = drill.Map;
            var mapComp = map.GetComponent<ManualDrillMapComponent>();
            var rootpos = drill.TrueCenter().ToIntVec3();
            for (var i = 0; i < 9; i++)
            {
                var pos = rootpos + GenRadial.ManualRadialPattern[i];
                var resDefAt = map.deepResourceGrid.ThingDefAt(pos);
                if (resDefAt == null)
                {
                    continue;
                }

                if (resDef == null && GetShallowResources(drill, shallowFactor, pos) > 0)
                {
                    resDef = resDefAt;
                    nextCell = pos;
                }

                if (!mapComp.GetValue(pos, out var maxValue))
                {
                    continue;
                }

                var deepCountAt = map.deepResourceGrid.CountAt(pos);
                var shallowCount = (int) (maxValue * shallowFactor);
                if (maxValue - shallowCount <= deepCountAt)
                {
                    resCount += deepCountAt - (maxValue - shallowCount);
                }
            }

            return resCount;
        }

        // Token: 0x0600006D RID: 109 RVA: 0x00004810 File Offset: 0x00002A10
        public static bool GetNextManualResource(Building drill, IntVec3 p, Map map, out ThingDef resDef,
            out int countPresentCell, out IntVec3 cell)
        {
            var CMD = drill.TryGetComp<CompManualDrill>();
            if (CMD != null && CMD.prospected &&
                DrillCanGetToCount(drill, CMD.MDProps.shallowReach, 9, out var thingDef, out var nextCell) > 0)
            {
                resDef = thingDef;
                countPresentCell = GetShallowResources(drill, CMD.MDProps.shallowReach, nextCell);
                cell = nextCell;
                return true;
            }

            resDef = DeepDrillUtility.GetBaseResource(map, p);
            countPresentCell = int.MaxValue;
            cell = p;
            return false;
        }

        // Token: 0x0600006E RID: 110 RVA: 0x00004888 File Offset: 0x00002A88
        public static int GetShallowResources(Building drill, float shallowFactor, IntVec3 cell)
        {
            var shallowRes = 0;
            if (drill?.Map == null || !drill.Spawned || !(shallowFactor > 0f) ||
                !drill.Map.GetComponent<ManualDrillMapComponent>().GetValue(cell, out var maxVal))
            {
                return shallowRes;
            }

            var deepCountAt = drill.Map.deepResourceGrid.CountAt(cell);
            var shallowCount = (int) (maxVal * shallowFactor);
            if (maxVal - shallowCount <= deepCountAt)
            {
                shallowRes += deepCountAt - (maxVal - shallowCount);
            }

            return shallowRes;
        }

        // Token: 0x0600006F RID: 111 RVA: 0x000048F4 File Offset: 0x00002AF4
        public static float GetPerformFactor(Building drill, out bool noWind, out float roofFactor,
            out float windFactor)
        {
            var performFactor = 1f;
            roofFactor = 1f;
            windFactor = 1f;
            noWind = false;
            if (drill?.Map == null || !drill.Spawned)
            {
                return performFactor * roofFactor * windFactor;
            }

            var root = drill.TrueCenter().ToIntVec3();
            var openRoof = 0;
            var totalRoof = 0;
            for (var i = 0; i < 9; i++)
            {
                var c = root + GenRadial.RadialPattern[i];
                totalRoof++;
                if (!c.Roofed(drill.Map))
                {
                    openRoof++;
                }
            }

            if (totalRoof > 0)
            {
                roofFactor = Mathf.Lerp(0.5f, 1f, (float) openRoof / totalRoof);
            }

            windFactor = Math.Min(1.5f, drill.Map.windManager.WindSpeed);
            if (windFactor < 0.5f)
            {
                noWind = true;
            }

            return performFactor * roofFactor * windFactor;
        }
    }
}