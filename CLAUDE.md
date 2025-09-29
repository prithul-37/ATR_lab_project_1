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

## Camera Capture System

### Real-Time Video Streaming
The project includes a camera capture system for streaming video feed to external Python clients:

- **CameraCapture.cs**: Handles real-time frame capture from Unity cameras
- **Camera endpoints**: `GET /camera/frame` (JPEG data) and `GET /camera/info` (metadata)
- **Video format**: JPEG compression at 75% quality for optimal performance
- **Configuration**: Configurable resolution (default 640x480) and frame rate (default 30fps)

### Python Integration
- **Enhanced Python client**: Supports multiple image formats (OpenCV, PIL, raw bytes)
- **Live video display**: Real-time camera feed with OpenCV integration
- **Dependencies**: opencv-python, Pillow, numpy, requests (see `Python/requirements.txt`)

## Development History

### Implementation Timeline
This section documents the development process and key implementation milestones:

1. **Initial Setup and Documentation**
   - Established Unity 6000.2.2f1 project with URP
   - Created comprehensive CLAUDE.md documentation
   - Analyzed existing event-driven input architecture

2. **Control System Enhancement**
   - Added runtime toggle functionality to InputHandler.cs
   - Implemented 'T' key toggle between server control and keyboard control
   - Enhanced with proper state management and debug logging

3. **Python Client Integration**
   - Provided setup instructions for Python client usage
   - Resolved dependency issues (requests module installation)
   - Established working HTTP communication between Unity and Python

4. **Camera Feed Implementation**
   - Created CameraCapture.cs for real-time frame capture
   - Extended HttpServer.cs with camera endpoints (`/camera/frame`, `/camera/info`)
   - Implemented JPEG encoding with configurable quality and resolution
   - Added thread-safe camera operations with proper resource cleanup

5. **Computer Vision Integration**
   - Enhanced Python client with camera feed functionality
   - Added support for multiple image formats (OpenCV, PIL, bytes)
   - Installed computer vision dependencies (opencv-python, Pillow, numpy)
   - Implemented live video display with frame saving capabilities

### Key Technical Decisions

**Event-Driven Architecture**: Chose static events for loose coupling between input handling and player control, enabling easy switching between input sources.

**Thread Safety**: Implemented UnityMainThreadDispatcher pattern to handle HTTP server callbacks safely on Unity's main thread.

**Camera Capture Approach**: Used RenderTexture → Texture2D → JPEG encoding pipeline for efficient video streaming with configurable quality settings.

**HTTP REST API Design**: Structured endpoints for clear separation of concerns (/command for control, /camera/* for video streaming).

**Python Client Architecture**: Created modular UnityController class with separate methods for movement control and camera functionality.

### Current System Capabilities

**Remote Control Features**:
- Full player movement control (WASD, running, jumping, rotation)
- Real-time command processing via HTTP API
- Runtime toggle between keyboard and server control modes
- Complex movement combinations (diagonal movement, running + jumping)

**Camera Streaming Features**:
- Real-time video capture from Unity cameras
- JPEG compression for network efficiency
- Multiple output formats for Python integration
- Live video display with frame saving
- Camera metadata API for client configuration

**Development Workflow**:
- Python dependency management with requirements.txt
- Comprehensive error handling and logging
- Resource cleanup and memory management
- Cross-platform HTTP communication

### Testing Procedures

1. **Unity Setup**: Load main scene (Exp_Scene_1.unity) and enter Play mode
2. **Control Testing**: Use 'T' key to toggle control modes, verify debug output
3. **Python Client**: Run `python Python/unity_client_example.py` for movement testing
4. **Camera Feed**: Run `python Python/unity_client_example.py camera` for video streaming
5. **Integration Testing**: Combine movement commands with camera feed monitoring

All systems are fully implemented and tested with proper error handling and resource management.