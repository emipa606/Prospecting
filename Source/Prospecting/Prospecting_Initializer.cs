using System;
using System.Collections.Generic;
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
			LongEventHandler.QueueLongEvent(new Action(Prospecting_Initializer.Setup), "LibraryStartup", false, null, true);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000206C File Offset: 0x0000026C
		public static void Setup()
		{
			Prospecting_Initializer.ApplyDeepDrillChances();
			List<ThingDef> allDefs = DefDatabase<ThingDef>.AllDefsListForReading;
			checked
			{
				if (allDefs.Count > 0)
				{
					int mineCount = 0;
					int changed = 0;
					int deepmineCount = 0;
					int deepchanged = 0;
					foreach (ThingDef thing in allDefs)
					{
						bool hasDeepChanged = false;
						if (thing != null)
						{
							float deepCommonality = thing.deepCommonality;
							if (thing.deepCommonality > 0f)
							{
								deepmineCount++;
								float MineDeepCommon = thing.deepCommonality;
								float newMineDeepCommon = unchecked(MineDeepCommon * (Controller.Settings.PrsDeepCommonality / 100f));
								if (newMineDeepCommon != MineDeepCommon)
								{
									thing.deepCommonality = newMineDeepCommon;
									hasDeepChanged = true;
								}
								if (thing != null)
								{
									int deepCountPerPortion = thing.deepCountPerPortion;
									int MineDeepYield = thing.deepCountPerPortion;
									int newMineDeepYield = Math.Max(1, (int)(unchecked((float)MineDeepYield * (Controller.Settings.PrsDeepMineYield / 100f))));
									newMineDeepYield = Math.Min(thing.stackLimit, newMineDeepYield);
									if (newMineDeepYield != MineDeepYield)
									{
										thing.deepCountPerPortion = newMineDeepYield;
										hasDeepChanged = true;
									}
								}
								if (thing != null)
								{
									IntRange deepLumpSizeRange = thing.deepLumpSizeRange;
									int MineDeepMin = thing.deepLumpSizeRange.min;
									int MineDeepMax = thing.deepLumpSizeRange.max;
									int newMineDeepMin = Math.Max(0, (int)(unchecked((float)MineDeepMin * (Controller.Settings.PrsDeepLumpSizeMin / 100f))));
									int newMineDeepMax = Math.Max(MineDeepMin, (int)(unchecked((float)MineDeepMax * (Controller.Settings.PrsDeepLumpSizeMax / 100f))));
									if (newMineDeepMin != MineDeepMin || newMineDeepMax != MineDeepMax)
									{
										thing.deepLumpSizeRange.min = newMineDeepMin;
										thing.deepLumpSizeRange.max = newMineDeepMax;
										hasDeepChanged = true;
									}
								}
							}
						}
						if (hasDeepChanged)
						{
							deepchanged++;
						}
						if (thing.mineable && ((thing != null) ? thing.building : null) != null && thing.building.isResourceRock)
						{
							mineCount++;
							bool hasChanged = false;
							if (thing != null)
							{
								BuildingProperties building = thing.building;
								IntRange? intRange = (building != null) ? new IntRange?(building.mineableScatterLumpSizeRange) : null;
								if (intRange != null)
								{
									int MineMin = thing.building.mineableScatterLumpSizeRange.min;
									int MineMax = thing.building.mineableScatterLumpSizeRange.max;
									int newMineMin = Math.Max(1, (int)(unchecked((float)MineMin * (Controller.Settings.PrsLumpSizeMin / 100f))));
									int newMineMax = Math.Max(MineMin, (int)(unchecked((float)MineMax * (Controller.Settings.PrsLumpSizeMax / 100f))));
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
								BuildingProperties building2 = thing.building;
								bool flag;
								if (building2 == null)
								{
									flag = false;
								}
								else
								{
									float mineableScatterCommonality = building2.mineableScatterCommonality;
									flag = true;
								}
								if (flag)
								{
									float MineCommon = thing.building.mineableScatterCommonality;
									float newMineCommon = unchecked(MineCommon * (Controller.Settings.PrsCommonality / 100f));
									if (newMineCommon != MineCommon)
									{
										thing.building.mineableScatterCommonality = newMineCommon;
										hasChanged = true;
									}
								}
							}
							if (thing != null)
							{
								BuildingProperties building3 = thing.building;
								bool flag2;
								if (building3 == null)
								{
									flag2 = false;
								}
								else
								{
									int mineableYield = building3.mineableYield;
									flag2 = true;
								}
								if (flag2)
								{
									int MineYield = thing.building.mineableYield;
									BuildingProperties building4 = thing.building;
									ThingDef MineThing = (building4 != null) ? building4.mineableThing : null;
									int MaxStack = 1;
									if (MineThing != null)
									{
										MaxStack = MineThing.stackLimit;
									}
									int newMineYield = Math.Max(1, (int)(unchecked((float)MineYield * (Controller.Settings.PrsMineYield / 100f))));
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
					}
					string deepMsg = "Prospecting.FeedbackDeepNoChg".Translate(deepmineCount.ToString());
					if (deepchanged > 0)
					{
						deepMsg = "Prospecting.FeedbackDeep".Translate(deepmineCount.ToString(), deepchanged.ToString());
					}
					Log.Message(deepMsg, false);
					string Msg = "Prospecting.FeedbackNoChg".Translate(mineCount.ToString());
					if (changed > 0)
					{
						Msg = "Prospecting.Feedback".Translate(mineCount.ToString(), changed.ToString());
					}
					Log.Message(Msg, false);
				}
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000024C4 File Offset: 0x000006C4
		public static void ApplyDeepDrillChances()
		{
			List<DifficultyDef> list = DefDatabase<DifficultyDef>.AllDefsListForReading;
			if (list.Count > 0)
			{
				foreach (DifficultyDef def in list)
				{
					if (def.deepDrillInfestationChanceFactor <= 0f)
					{
						def.deepDrillInfestationChanceFactor = 0.01f;
					}
				}
			}
		}
	}
}
