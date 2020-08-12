using System;
using UnityEngine;
using Verse;

namespace Prospecting
{
	// Token: 0x0200000A RID: 10
	public class Controller : Mod
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00003978 File Offset: 0x00001B78
		public override string SettingsCategory()
		{
			return "Prospecting.Name".Translate();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003989 File Offset: 0x00001B89
		public override void DoSettingsWindowContents(Rect canvas)
		{
			Controller.Settings.DoWindowContents(canvas);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003996 File Offset: 0x00001B96
		public Controller(ModContentPack content) : base(content)
		{
			Controller.Settings = base.GetSettings<Settings>();
		}

		// Token: 0x0400001C RID: 28
		public static Settings Settings;
	}
}
