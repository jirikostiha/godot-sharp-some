# Godot Sharp Drawing
[![NuGet Downloads](https://img.shields.io/nuget/dt/GodotSharpSome.Drawing2D.svg)](https://www.nuget.org/packages/GodotSharpSome.Drawing2D/)
![GitHub repo size](https://img.shields.io/github/repo-size/jirikostiha/godot-sharp-some)  

Is set of extensions for custom drawing in Godot engine version 3.3 and higher.  

**Note: Godot currently does not support parameters 'width' and 'antialiased' of 'DrawMultiline' method so they have no effect for now.**

## Setup
Add [nuget package](https://www.nuget.org/packages/GodotSharpSome.Drawing2D)
to your project.

Godot project's `.csproj` file should look like this:

```xml
<Project Sdk="Godot.NET.Sdk/3.3.0">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="GodotSharpSome.Drawing2D" Version="0.14.0" />
  </ItemGroup>
</Project>
```

## Features  
[features 1](./doc/images/features2D_1.png "Features")  
[features 2](./doc/images/features2D_2.png "Features")  

**CanvasItem extensions**  
triangle line, triangle area, rectangle line, rectangle area, polygon line, polygon area, circle line, circle area  

## Usage
There are three ways how to use Multiline class.  
1. Using preset shapes   

```csharp
canvas.DrawMultiline(
    Multiline.Cross(new Vector2(100,100), 20),
    Color.ColorN("red"));
```

2. As a builder



3. Static methods


For details and exapmles see [example project](./src/usage/).