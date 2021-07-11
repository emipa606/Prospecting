using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting
{
    // Token: 0x0200000B RID: 11
    public class Designator_Prospect : Designator
    {
        // Token: 0x0400001E RID: 30
        public DesignationDef MineDesig = DesignationDefOf.Mine;

        // Token: 0x0400001D RID: 29
        public DesignationDef ProspectDesig = ProspectDef.Prospect;

        // Token: 0x0600003A RID: 58 RVA: 0x000039B8 File Offset: 0x00001BB8
        public Designator_Prospect()
        {
            defaultLabel = "Prospecting.DesignatorProspect".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/Prospecting");
            defaultDesc = "Prospecting.DesignatorProspectDesc".Translate();
            useMouseIcon = true;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            soundSucceeded = SoundDefOf.Designate_Mine;
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000037 RID: 55 RVA: 0x000039AA File Offset: 0x00001BAA
        public override int DraggableDimensions => 0;

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000038 RID: 56 RVA: 0x000039AD File Offset: 0x00001BAD
        public override bool DragDrawMeasurements => false;

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000039 RID: 57 RVA: 0x000039B0 File Offset: 0x00001BB0
        protected override DesignationDef Designation => ProspectDesig;

        // Token: 0x0600003B RID: 59 RVA: 0x00003A44 File Offset: 0x00001C44
        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(Map))
            {
                return false;
            }

            if (Map.designationManager.DesignationAt(c, Designation) != null)
            {
                return AcceptanceReport.WasRejected;
            }

            if (Map.designationManager.DesignationAt(c, MineDesig) != null)
            {
                return AcceptanceReport.WasRejected;
            }

            if (c.Fogged(Map))
            {
                return false;
            }

            var firstMineable = c.GetFirstMineable(Map);
            if (firstMineable == null)
            {
                return "MessageMustDesignateMineable".Translate();
            }

            var result = CanDesignateThing(firstMineable);
            if (!result.Accepted)
            {
                return result;
            }

            var chkcells = GenAdjFast.AdjacentCellsCardinal(c);
            var entry = false;
            if (chkcells.Count > 0)
            {
                foreach (var chkcell in chkcells)
                {
                    if (!chkcell.InBounds(Map) || !chkcell.Standable(Map) && !chkcell.Walkable(Map))
                    {
                        continue;
                    }

                    entry = true;
                    break;
                }
            }

            if (!entry)
            {
                return false;
            }

            return AcceptanceReport.WasAccepted;
        }

        // Token: 0x0600003C RID: 60 RVA: 0x00003B7C File Offset: 0x00001D7C
        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            if (!t.def.mineable)
            {
                return false;
            }

            if (Map.designationManager.DesignationAt(t.Position, Designation) != null)
            {
                return AcceptanceReport.WasRejected;
            }

            if (Map.designationManager.DesignationAt(t.Position, MineDesig) != null)
            {
                return AcceptanceReport.WasRejected;
            }

            var chkcells = GenAdjFast.AdjacentCellsCardinal(t.Position);
            var entry = false;
            if (chkcells.Count > 0)
            {
                foreach (var chkcell in chkcells)
                {
                    if (!chkcell.InBounds(Map) || !chkcell.Standable(Map) && !chkcell.Walkable(Map))
                    {
                        continue;
                    }

                    entry = true;
                    break;
                }
            }

            if (!entry)
            {
                return false;
            }

            return true;
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00003C74 File Offset: 0x00001E74
        public bool CloseToDesig(DesignationDef desig, int near, IntVec3 root, Map map)
        {
            var cells = GenRadial.RadialCellsAround(root, near, true).ToList();
            if (cells.Count <= 0)
            {
                return false;
            }

            foreach (var cell in cells)
            {
                if (map.designationManager.DesignationAt(cell, desig) != null)
                {
                    return true;
                }
            }

            return false;
        }

        // Token: 0x0600003E RID: 62 RVA: 0x00003CEC File Offset: 0x00001EEC
        public override void DesignateSingleCell(IntVec3 loc)
        {
            Map.designationManager.AddDesignation(new Designation(loc, Designation));
            Map.designationManager.TryRemoveDesignation(loc, DesignationDefOf.SmoothWall);
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00003D25 File Offset: 0x00001F25
        public override void DesignateThing(Thing t)
        {
            DesignateSingleCell(t.Position);
        }

        // Token: 0x06000040 RID: 64 RVA: 0x00003D33 File Offset: 0x00001F33

        // Token: 0x06000041 RID: 65 RVA: 0x00003D3B File Offset: 0x00001F3B
        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
        }

        // Token: 0x06000042 RID: 66 RVA: 0x00003D42 File Offset: 0x00001F42
        public override void RenderHighlight(List<IntVec3> dragCells)
        {
            DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells);
        }
    }
}