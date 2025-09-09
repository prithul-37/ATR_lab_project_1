# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 3D project (Unity 6000.2.2f1) using the Universal Render Pipeline (URP). The project includes:

- Input System for handling player input
- AI Navigation package for pathfinding
- Visual Scripting for node-based programming
- Test Framework for unit testing

## Unity Project Structure

- `Assets/Scenes/` - Unity scene files
  - `SampleScene.unity` - Default Unity sample scene
  - `Exp_Scene_1.unity` - Custom experimental scene
- `Assets/Script/` - C# scripts directory (currently empty)
- `Assets/Material/` - Materials and textures
- `Assets/nappin/` - Additional assets directory
- `Assets/TutorialInfo/` - Unity tutorial readme system
- `ProjectSettings/` - Unity project configuration files
- `Packages/manifest.json` - Unity package dependencies

## Development Commands

### Unity Editor
- Open the project through Unity Hub or directly with Unity Editor 6000.2.2f1
- Build the project using Unity Editor's Build Settings (File → Build Settings)

### Testing
- Run tests through Unity Test Runner (Window → General → Test Runner)
- Tests should be placed in appropriate test directories following Unity conventions

## Key Dependencies

- **Universal Render Pipeline (URP)** - For rendering pipeline
- **Input System** - Modern input handling system
- **AI Navigation** - For NavMesh and pathfinding
- **Visual Scripting** - Node-based visual programming
- **Timeline** - For cinematic sequences and animations

## Code Style and Conventions

### C# Naming Conventions
Follow Unity's C# naming conventions:

- **Public variables**: PascalCase (e.g., `WalkSpeed`, `JumpHeight`, `GroundCheck`)
- **Private variables**: Underscore prefix + camelCase (e.g., `_characterController`, `_isGrounded`, `_currentMovementInput`)
- **Methods**: PascalCase (e.g., `HandleMovementInput`, `ApplyMovement`)
- **Events**: PascalCase with "On" prefix (e.g., `OnMovementInput`, `OnJumpPressed`)

### Architecture Patterns
- **Event-driven input system**: InputHandler publishes events, controllers subscribe
- **Component-based architecture**: Use Unity's component system for modular functionality
- **Proper event cleanup**: Always unsubscribe from events in `OnDestroy()` to prevent memory leaks

## Architecture Notes

This appears to be an early-stage Unity project with minimal custom code. The main structure follows Unity's standard project organization with scenes, assets, and project settings properly configured.

The project uses Unity's newer packages including the Input System (instead of legacy input) and URP (instead of built-in render pipeline), indicating it's set up with modern Unity practices.