# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 3D project (Unity 6000.2.2f1) using the Universal Render Pipeline (URP). The project implements a remote-controllable player character system with HTTP API integration for external control via Python clients.

## Core Architecture

### Event-Driven Input System
The project uses a decoupled event-driven architecture for handling input:

- **InputHandler.cs**: Central input processor that supports both keyboard input and HTTP server commands
- **PlayerController.cs**: Subscribes to input events and applies movement/actions to the character
- **HttpServer.cs**: HTTP listener that receives external commands and forwards them to the input system
- **UnityMainThreadDispatcher.cs**: Thread-safe utility for executing HTTP callbacks on Unity's main thread

### Remote Control Integration
The system supports external control through a REST API:
- HTTP endpoint: `POST http://localhost:8080/command`
- Command format: JSON with movement vectors, action flags (running, jumping, rotation)
- **PlayerCommand.cs**: Serializable data structure for command transmission
- **Python client** (`Python/unity_client_example.py`): Example external controller

## Development Commands

### Unity Editor
- Open project through Unity Hub with Unity Editor 6000.2.2f1
- Main scene: `Assets/Scenes/Exp_Scene_1.unity`
- Build using File → Build Settings

### Testing HTTP Integration
1. Start Unity and enter Play mode
2. Run Python client: `python Python/unity_client_example.py`
3. Verify player responds to programmatic commands

### Key Dependencies
- **Input System** (1.14.2) - Modern input handling
- **Universal Render Pipeline** (17.2.0) - Rendering
- **AI Navigation** (2.0.8) - NavMesh pathfinding
- **Visual Scripting** (1.9.7) - Node-based programming

## Architecture Patterns

### Thread Safety
- HTTP server runs on background thread
- `UnityMainThreadDispatcher` ensures Unity API calls execute on main thread
- Event subscription/unsubscription handled in `OnDestroy()` for proper cleanup

### Input Abstraction
- `InputHandler` abstracts input source (keyboard vs HTTP)
- `EnableServerControl` flag switches between input modes
- Events provide loose coupling between input and player systems

### Component Communication
- Static events for system-wide communication (InputHandler → PlayerController)
- Serializable command objects for network communication
- Singleton pattern for thread dispatcher

## Code Conventions

### C# Naming (Project-Specific)
- **Public variables**: PascalCase (`WalkSpeed`, `JumpHeight`, `EnableServerControl`)
- **Private fields**: Underscore prefix + camelCase (`_characterController`, `_isGrounded`, `_hasServerInput`)
- **Events**: "On" prefix + PascalCase (`OnMovementInput`, `OnJumpPressed`)
- **HTTP endpoints**: lowercase with forward slashes (`/command`)

### Event Management
- Always unsubscribe from static events in `OnDestroy()`
- Null-conditional operators for event invocation (`OnMovementInput?.Invoke()`)
- Event cleanup prevents memory leaks between scene loads