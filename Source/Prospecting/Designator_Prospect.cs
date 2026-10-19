using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Prospecting;

public class Designator_Prospect : Designator
{
    private readonly DesignationDef MineDesig = DesignationDefOf.Mine;

    private readonly DesignationDef ProspectDesig = ProspectDef.Prospect;

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

    public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Mine;

    public override bool DragDrawMeasurements => false;

    protected override DesignationDef Designation => ProspectDesig;

    public override AcceptanceReport CanDesignateCell(IntVec3 loc)
    {
        if (!loc.InBounds(Map))
        {
            return false;
        }

        if (Map.designationManager.DesignationAt(loc, Designation) != null ||
            Map.designationManager.DesignationAt(loc, MineDesig) != null)
        {
            return AcceptanceReport.WasRejected;
        }

        if (loc.Fogged(Map))
        {
            return false;
        }

        var firstMineable = loc.GetFirstMineable(Map);
        if (firstMineable == null)
        {
            return "MessageMustDesignateMineable".Translate();
        }

        var result = CanDesignateThing(firstMineable);
        if (!result.Accepted)
        {
            return result;
        }

        var chkcells = GenAdjFast.AdjacentCellsCardinal(loc);
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

        if (Map.designationManager.DesignationAt(t.Position, Designation) != null ||
            Map.designationManager.DesignationAt(t.Position, MineDesig) != null)
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

    public static bool CloseToDesig(DesignationDef desig, int near, IntVec3 root, Map map)
    {
        var cells = GenRadial.RadialCellsAround(root, near, true).ToList();
        return cells.Count > 0 && cells.Any(cell => map.designationManager.DesignationAt(cell, desig) != null);
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        Map.designationManager.AddDesignation(new Designation(c, Designation));
        Map.designationManager.TryRemoveDesignation(c, DesignationDefOf.SmoothWall);
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