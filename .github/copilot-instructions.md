# GitHub Copilot Instructions for RimWorld Modding Project

Welcome to the coding environment for our RimWorld mod! This guide provides an overview of the mod, key features, coding patterns, XML integration, Harmony patching, and tips for using GitHub Copilot effectively in this project.

## Mod Overview and Purpose

This mod introduces manual drilling and prospecting mechanics to RimWorld, allowing players to engage in resource extraction through enhanced gameplay features. The objective is to provide a deeper, immersive mining experience by integrating new systems and tools that enhance resource management and production in the game.

## Key Features and Systems

- **Manual Drilling**: Introduces buildings like the `Building_ManualDrill` for extracting resources underground manually. Players can utilize the `CompManualDrill` component to manage and optimize the resource yield.
- **Prospecting**: Enables the use of prospecting belts and markers to search and designate resource-rich areas. Key classes include `ProspectBelt` and `ProspectMarker`.
- **WideBoy Device**: A unique asset allowing larger-scale drilling operations that offers increased efficiency and potential resource yield. Managed by the `CompWideBoy` class.
- **Alerts and WorkGivers**: Custom alerts and work givers (e.g., `Alert_NeedProspector`, `WorkGiver_ManualDrillMine`) assist in managing tasks and optimizing workforce management.
- **Settings and Customization**: Mod settings are handled through the `Settings` class, providing players with configurable gameplay elements.

## Coding Patterns and Conventions

- **Coding Standards**: We adhere to C# standards and best practices, utilizing clear naming conventions, structured methods, and efficient class design. Pay attention to class inheritance and interfaces to understand logical structuring.
- **Class Design**: Each class serves a specific purpose and is organized around core functionalities (e.g., `Building_ManualDrill` for building logic, `JobDriver_ManualDrillProspect` for job handling).
- **Method Optimization**: Use helper methods within classes to perform specific tasks. For instance, methods within `CompManualDrill` like `ManualDrillWorkDone` manage specific actions regarding manual drilling progress and resource management.

## XML Integration

- **Parsing Errors**: Be mindful of XML formatting and structure to avoid parsing errors commonly found in mod projects. Ensure all elements are correctly nested and attributes are properly closed.
- **XML Files**: These files define game objects, resources, and other mod components. Ensure changes align with the requirements of the RimWorld modding framework to maintain compatibility.

## Harmony Patching

- **Purpose**: Harmony patches are employed to alter or extend existing game methods without modifying the core code directly. This mod includes patches like `CancelCanDesignateThing_Patch` and `DrillWorkDone_PrePatch`.
- **Implementation**: Harmony patches are used for integrating custom behaviors into the base game code, such as overriding designation logic or modifying job drivers. Ensure patches are crafted thoughtfully to prevent conflicts.
- **File Structure**: Maintain separation of patch logic across different files to aid organization and readability. For instance, different patches are contained within their respective files (e.g., `TrySpawnYield_PostPatch.cs`).

## Suggestions for Copilot

1. **Code Completion**: Use Copilot to suggest method implementations based on existing patterns observed within the codebase, especially for frequently used components.
2. **Example Usage**: Leverage complete examples from similar classes (e.g., `CompManualDrill`) to understand how resources and variables should be handled.
3. **XML Structure Suggestions**: When writing XML, collaborate with Copilot to suggest properly nested elements and syntax structures.
4. **Patching Logic**: Utilize Copilot to generate basic Harmony patching templates, which you can refine by inserting specific logic relevant to your mod's objectives.

By understanding and applying these guidelines, you can effectively utilize GitHub Copilot to streamline mod development, maximize productivity, and maintain high coding standards. Enjoy contributing to the enrichment of the RimWorld gaming experience!
