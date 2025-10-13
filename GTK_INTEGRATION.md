# GTK4 Integration for Planos

This document describes the GTK4 integration that has been added to the Planos CAD application.

## What was added

### 1. GTK Sharp Package
- Added `GtkSharp` version 3.24.24.95 to the project dependencies
- This provides GTK3 bindings for .NET (GTK4 bindings for .NET are still in development)

### 2. MainWindow.cs
- Created a new `MainWindow` class that extends `Gtk.Window`
- Provides a simple GUI interface with:
  - Welcome label
  - Three buttons for drawing different entities (Line, Circle, Polygon)
  - Scrollable text view for output
  - Window close handling

### 3. Program.cs Updates
- Added GTK initialization with `Application.Init()`
- Creates and displays the main window after console operations
- Runs the GTK main loop with `Application.Run()`

## Features

The GTK window provides:
- **Draw Line Button**: Calls the existing `cadLine` entity creation
- **Draw Circle Button**: Calls the existing `cadCircle` entity creation  
- **Draw Polygon Button**: Attempts to create a polygon entity (will show error if not implemented)
- **Console Integration**: All drawing operations are logged to both the GTK window and console

## Running the Application

```bash
dotnet run
```

The application will:
1. Execute the original console-based CAD operations
2. Launch a GTK window with the graphical interface
3. Allow interaction with both console and GUI modes

## Technical Notes

- Uses GTK3 (not GTK4) due to .NET binding availability
- The GUI integrates with your existing entity system (`cadLine`, `cadCircle`, etc.)
- Window operations are thread-safe and properly handle application shutdown
- All existing functionality remains intact - GTK is an additional interface

## Dependencies

- .NET 8.0
- GTK3 (installed via GtkSharp NuGet package)
- Your existing CAD entity system (IEntity interface, cadLine, cadCircle classes)

The integration is designed to be non-intrusive to your existing codebase while providing a modern graphical interface for the CAD operations.