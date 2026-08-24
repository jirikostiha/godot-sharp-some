# Godot Sharp Some Documentation

Extensions and utilities for custom 2D drawing in the [Godot Engine](https://godotengine.org/) using C# and .NET.

---

## Design Principles

- **Fluid Drawing API:** Extends Godot's `CanvasItem` with intuitive drawing methods for standard and composite 2D geometric shapes.
- **Advanced Multiline Management:** The `Multiline` generator builds vertex coordinate collections supporting styled lines, pen switching, and spatial transformations without boilerplate.
- **High Performance:** Lightweight point generation and batch rendering designed to integrate directly with Godot's 2D rendering pipeline.

---

## Installation

Add the NuGet package to your Godot C# project:

```shell
dotnet add package GodotSharpSome.Drawing2D
```

Your project's `.csproj` should target `net8.0` with `Godot.NET.Sdk`:

```xml
<Project Sdk="Godot.NET.Sdk/4.1.3">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="GodotSharpSome.Drawing2D" Version="0.23.0" />
  </ItemGroup>
</Project>
```

---

## Quick Navigation

- [API Reference](api/index.md) - Complete reference for classes, extension methods, and line types.
- [Features](../features.md) - Detailed breakdown of shapes, drawing capabilities, and visual examples.
- [Conventions](../conventions.md) - Versioning, commit standards, and project organization.
- [GitHub Repository](https://github.com/jirikostiha/godot-sharp-some) - Source code and issue tracker.

---

## Visual Overview

![Dots and Lines](../images/dots_and_lines_animation.gif)
