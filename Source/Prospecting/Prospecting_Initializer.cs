﻿using System;
using RimWorld;
using Verse;

namespace Prospecting
{
    // Token: 0x02000002 RID: 2
    [StaticConstructorOnStartup]
    internal static class Prospecting_Initializer
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        static Prospecting_Initializer()
        {
            LongEventHandler.QueueLongEvent(Setup, "LibraryStartup", false, null);
        }

        // Token: 0x06000002 RID: 2 RVA: 0x0000206C File Offset: 0x0000026C
        public static void Setup()
        {
            ApplyDeepDrillChances();
            var allDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            checked
            {
                if (allDefs.Count <= 0)
                {
                    return;
                }

                var mineCount = 0;
                var changed = 0;
                var deepmineCount = 0;
                var deepchanged = 0;
                foreach (var thing in allDefs)
                {
                    var hasDeepChanged = false;
                    if (thing != null)
                    {
                        var unused = thing.deepCommonality;
                        if (thing.deepCommonality > 0f)
                        {
                            deepmineCount++;
                            var MineDeepCommon = thing.deepCommonality;
                            var newMineDeepCommon =
                                MineDeepCommon * (Controller.Settings.PrsDeepCommonality / 100f);
                            if (newMineDeepCommon != MineDeepCommon)
                            {
                                thing.deepCommonality = newMineDeepCommon;
                                hasDeepChanged = true;
                            }

                            var unused1 = thing.deepCountPerPortion;
                            var MineDeepYield = thing.deepCountPerPortion;
                            var newMineDeepYield = Math.Max(1,
                                (int) (MineDeepYield * (Controller.Settings.PrsDeepMineYield / 100f)));
                            newMineDeepYield = Math.Min(thing.stackLimit, newMineDeepYield);
                            if (newMineDeepYield != MineDeepYield)
                            {
                                thing.deepCountPerPortion = newMineDeepYield;
                                hasDeepChanged = true;
                            }

                            var unused2 = thing.deepLumpSizeRange;
                            var MineDeepMin = thing.deepLumpSizeRange.min;
                            var MineDeepMax = thing.deepLumpSizeRange.max;
                            var newMineDeepMin = Math.Max(0,
                                (int) (MineDeepMin * (Controller.Settings.PrsDeepLumpSizeMin / 100f)));
                            var newMineDeepMax = Math.Max(MineDeepMin,
                                (int) (MineDeepMax * (Controller.Settings.PrsDeepLumpSizeMax / 100f)));
                            if (newMineDeepMin != MineDeepMin || newMineDeepMax != MineDeepMax)
                            {
                                thing.deepLumpSizeRange.min = newMineDeepMin;
                                thing.deepLumpSizeRange.max = newMineDeepMax;
                                hasDeepChanged = true;
                            }
                        }
                    }

                    if (hasDeepChanged)
                    {
                        deepchanged++;
                    }

                    if (thing != null && (!thing.mineable || thing.building == null ||
                                          !thing.building.isResourceRock))
                    {
                        continue;
                    }

                    mineCount++;
                    var hasChanged = false;
                    if (thing != null)
                    {
                        var building = thing.building;
                        var intRange = building != null
                            ? new IntRange?(building.mineableScatterLumpSizeRange)
                            : null;
                        if (intRange != null)
                        {
                            var MineMin = thing.building.mineableScatterLumpSizeRange.min;
                            var MineMax = thing.building.mineableScatterLumpSizeRange.max;
                            var newMineMin = Math.Max(1,
                                (int) (MineMin * (Controller.Settings.PrsLumpSizeMin / 100f)));
                            var newMineMax = Math.Max(MineMin,
                                (int) (MineMax * (Controller.Settings.PrsLumpSizeMax / 100f)));
                            if (newMineMin != MineMin || newMineMax != MineMax)
                            {
                                thing.building.mineableScatterLumpSizeRange.min = newMineMin;
                                thing.building.mineableScatterLumpSizeRange.max = newMineMax;
                                hasChanged = true;
                            }
                        }
                    }

                    if (thing != null)
                    {
                        var building2 = thing.building;
                        bool building;
                        if (building2 == null)
                        {
                            building = false;
                        }
                        else
                        {
                            var unused = building2.mineableScatterCommonality;
                            building = true;
                        }

                        if (building)
                        {
                            var MineCommon = thing.building.mineableScatterCommonality;
                            var newMineCommon = MineCommon * (Controller.Settings.PrsCommonality / 100f);
                            if (newMineCommon != MineCommon)
                            {
                                thing.building.mineableScatterCommonality = newMineCommon;
                                hasChanged = true;
                            }
                        }
                    }

                    if (thing != null)
                    {
                        var building3 = thing.building;
                        bool building;
                        if (building3 == null)
                        {
                            building = false;
                        }
                        else
                        {
                            var unused = building3.mineableYield;
                            building = true;
                        }

                        if (building)
                        {
                            var MineYield = thing.building.mineableYield;
                            var building4 = thing.building;
                            var MineThing = building4?.mineableThing;
                            var MaxStack = 1;
                            if (MineThing != null)
                            {
                                MaxStack = MineThing.stackLimit;
                            }

                            var newMineYield = Math.Max(1,
                                (int) (MineYield * (Controller.Settings.PrsMineYield / 100f)));
                            newMineYield = Math.Min(MaxStack, newMineYield);
                            if (newMineYield != MineYield)
                            {
                                thing.building.mineableYield = newMineYield;
                                hasChanged = true;
                            }
                        }
                    }

                    if (hasChanged)
                    {
                        changed++;
                    }
                }

                string deepMsg = "Prospecting.FeedbackDeepNoChg".Translate(deepmineCount.ToString());
                if (deepchanged > 0)
                {
                    deepMsg = "Prospecting.FeedbackDeep".Translate(deepmineCount.ToString(),
                        deepchanged.ToString());
                }

                Log.Message(deepMsg);
                string Msg = "Prospecting.FeedbackNoChg".Translate(mineCount.ToString());
                if (changed > 0)
                {
                    Msg = "Prospecting.Feedback".Translate(mineCount.ToString(), changed.ToString());
                }

                Log.Message(Msg);
            }
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000024C4 File Offset: 0x000006C4
        public static void ApplyDeepDrillChances()
        {
            var list = DefDatabase<DifficultyDef>.AllDefsListForReading;
            if (list.Count <= 0)
            {
                return;
            }

            foreach (var def in list)
            {
                if (def.deepDrillInfestationChanceFactor <= 0f)
                {
                    def.deepDrillInfestationChanceFactor = 0.01f;
                }
            }
        }
    }
}