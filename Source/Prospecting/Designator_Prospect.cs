using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting;

public class Designator_Prospect : Designator
{
    public readonly DesignationDef MineDesig = DesignationDefOf.Mine;

    public readonly DesignationDef ProspectDesig = ProspectDef.Prospect;

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

    public override int DraggableDimensions => 0;

    public override bool DragDrawMeasurements => false;

    protected override DesignationDef Designation => ProspectDesig;

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
        if (chkcells.Count <= 0)
        {
            return false;
        }

        foreach (var chkcell in chkcells)
        {
            if (!chkcell.InBounds(Map) || !chkcell.Standable(Map) && !chkcell.Walkable(Map))
            {
                continue;
            }

            entry = true;
            break;
        }

        return entry;
    }

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

    public override void DesignateSingleCell(IntVec3 loc)
    {
        Map.designationManager.AddDesignation(new Designation(loc, Designation));
        Map.designationManager.TryRemoveDesignation(loc, DesignationDefOf.SmoothWall);
    }

    public override void DesignateThing(Thing t)
    {
        DesignateSingleCell(t.Position);
    }


    public override void SelectedUpdate()
    {
        GenUI.RenderMouseoverBracket();
    }

    public override void RenderHighlight(List<IntVec3> dragCells)
    {
        DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells);
    }
}