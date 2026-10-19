# GitHub Copilot Instructions for the RimWorld Mod: Prospecting (Continued)

## Mod Overview and Purpose

**Mod Name:** Prospecting (Continued)

**Author:** pelador

**PackageId:** `Mlie.Prospecting`

**Description:** The "Prospecting (Continued)" mod enhances the mining experience in RimWorld by introducing new mechanics and tools. It provides an early indication of ore presence, improves mining resource allocation, and adds novel equipment like the Wide Boy deep drill and the Manual Drill for various tech levels.

## Key Features and Systems

- **Prospecting Activity:** A new work activity under the mining job where pawns can perform geological surveys on rocks to reveal nearby ores and report findings to the player.

- **Early Ore Detection:** When mining, if the rock is near an ore (not stone), it might yield minimal amounts of that ore, slightly influenced by the pawn’s mining skill.

- **Mod Options:** Configuration settings for mining yields, resource lump sizes, and ore commonality. These are effective only on initial game load and map generation.

- **Wide Boy Deep Drill:** An enhanced deep drill with customizable drilling width. It can be configured to mine stone and has a boost mode, increasing power usage and mining speed, with a breakdown risk mitigated by pawn skill.

- **Manual Drill:** A medieval-era wind-assisted deep drill for shallow resource extraction. It requires prospecting before drilling and has an associated research requirement. It includes a mod option to mute its sound.

- **Prospector's Belt:** An accessory that helps identify deep resources, marking them on the map, and provides a small mining bonus.

## Coding Patterns and Conventions

- **C# Files:** The mod uses a structured approach with C# classes for components (`CompManualDrillMats`), controllers (`Controller`), utility functions (`DeepDrillUtility_GetNextResource`), and designators (`Designator_Cancel_CanDesignateThing`).

- **XML Integration:** XML definitions are extensively used to configure jobs, items, research projects, and more in predefined directories, such as `Defs_Jobs_ManualDrill.xml`, `Defs_ManualDrill.xml`, and `Defs_ProspectBelt.xml`.

## Harmony Patching

This mod utilizes Harmony, a library for patching methods at runtime. Key patches include prefixes and postfixes to modify behavior without altering original game code:

- **Harmony Patches:** Utilized for:

  - Modifying mining behavior with defined prefixes (`DeepDrillUtility_GetNextResource`).
  - Adjusting designations (`Designator_Cancel_CanDesignateThing`).

Ensure compatibility with other mods by carefully applying patches and considering potential conflicts.

## Suggestions for Copilot

- **XML Suggestions:** When creating new items or job definitions, cross-reference existing XML files in the mod for consistency. Copilot can assist with autocompleting common tags and attributes.

- **C# Development:** Leverage Copilot to autocomplete method signatures and suggest appropriate data handling within methods, especially for iterative tasks like modifying resource allocations.

- **Harmony Patching:** Use Copilot to suggest patches based on existing prefixes and postfixes, aiding in identifying the right methods to patch.

- **Code Style Consistency:** Follow existing code style found in the mod for method naming, class organization, and comments to maintain consistency across development.

## Additional Notes

- **Compatibility:** Generally compatible with other mining mods but may experience cumulative effects or conflicts if other mods adjust similar values.

- **Translation and Graphics Credits:**
  - Japanese translation by Proxyer
  - Manual drill graphic by jabbamonkey
  - Prospector's belt graphic by Kasmex Forever

  Licensed under CC BY-NC-SA 4.0.

- **Troubleshooting:** Use dedicated Discord channels for error reporting and avoid using discussion threads. Log issues using the Log Uploader and employ RimSort for mod sorting.

| Tags: resources, exploration |

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

