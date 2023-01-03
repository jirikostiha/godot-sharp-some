# Godot Sharp Drawing
[![NuGet Downloads](https://img.shields.io/nuget/dt/GodotSharpSome.Drawing2D.svg)](https://www.nuget.org/packages/GodotSharpSome.Drawing2D/)
![GitHub repo size](https://img.shields.io/github/repo-size/jirikostiha/godot-sharp-some)  

Is set of extensions for custom drawing API in Godot engine version 3.3 and higher. It simplifies script drawing.  

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
Includes CanvasItem extensions for drawing plane region and Multiline class extending possibilities of custom drawing API.  
**CanvasItem Extensions**  

**Multiline**  

#video
![examples](./doc/images/examples_full.gif)?

Would you like to know [more](./src/GodotSharpSome.Drawing2D/readme.md)?

## Usage  

_Example 1_ shows how to draw vectors from common origin and then draw sum of vectors.  
```cs
public override void _Draw()
{
    var vectors = new Vector2[] { new(40, 40), new(70, 60), new(80, 120), new(40, -40), new(0, 30) };

    DrawMultiline(
        Multiline.VectorsAbsolutely(new Vector2(100, 100), vectors),
        Color.ColorN("gray"));

    DrawMultiline(
        Multiline.VectorsRelatively(new Vector2(300, 300), vectors),
        Color.ColorN("blue"));
}
```  
\
_Example 2_ shows how to draw more complex line drawings as one multiline.  
```cs
public override void _Draw()
{
    var m = new Multiline()
        .AppendRectangle(new Vector2(0,0), 50, 50, 0)
        .AppendDashDottedLine(new Vector2(-60, 0), new Vector2(60, 0))
        .AppendDashDottedLine(new Vector2(0, 60), new Vector2(0, -60));

    DrawMultiline(m.Points, Color.ColorN("black"));
}
```  
\
_Example 3_ shows drawing of solid geometric object.  
```cs
public override void _Draw()
{
    this.DrawTriangleArea(new Vector2(10, 10), new Vector2(100, 10), new Vector2(10, 100), Color.ColorN("black"));
}
```  
\

Would you like to know [more](./src/usage/)?

## Contributing
bugs  

https://docs.github.com/en/issues/tracking-your-work-with-issues/creating-an-issue

Any contributions are welcome!
[Conventions](./doc/conventions.md)  


## License  
Project is under [MIT](./LICENSE) license.