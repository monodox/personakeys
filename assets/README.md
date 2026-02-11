# PersonaKeys Assets

This directory contains icons and visual assets for the PersonaKeys plugin.

## Required Icons

### Plugin Icon
- **File**: `icon.png`
- **Size**: 256x256px (recommended)
- **Format**: PNG with transparency
- **Usage**: Main plugin icon shown in Logitech Options+ / Loupedeck software

### Action Icons
Each action should have an icon to display on the device button.

Recommended icons:
- `debugger.png` - Bug/magnifying glass icon
- `refactor.png` - Wrench/tools icon
- `explainer.png` - Light bulb/question mark icon
- `codegen.png` - Code brackets/sparkles icon
- `temperature.png` - Thermometer/slider icon
- `settings.png` - Gear/cog icon

## Icon Guidelines (Logitech SDK)

### Dimensions
- **Standard**: 80x80px minimum
- **High DPI**: 160x160px or 256x256px recommended

### Format
- PNG with alpha transparency
- SVG supported for vector icons

### Design Principles
- Simple, recognizable shapes
- High contrast for visibility on device
- Monochrome or limited color palette
- Clear at small sizes

### Color Modes
Consider both light and dark backgrounds:
- Light mode icon (dark foreground)
- Dark mode icon (light foreground)

## Action Symbol Templates

Use the Logitech Icon Editor or templates:
- Download from: https://developer.logi.com/actions-sdk/icons

## Creating Icons

### Tools
- **Adobe Illustrator** - Vector design
- **Figma** - Free, web-based
- **Inkscape** - Free, open-source
- **Affinity Designer** - One-time purchase

### Quick Start
1. Create 256x256px canvas
2. Design simple, bold icon
3. Export as PNG with transparency
4. Place in `assets/` directory
5. Reference in action code: `Icon = "assets/debugger.png"`

## Icon Usage in Code

```csharp
[PluginAction]
public class DebuggerAction : BasePersonaAction
{
    public override string Name => "AI Debugger";
    public override string Icon => "assets/debugger.png";
}
```

## Placeholder Note

This project currently uses default icons. Replace with custom icons for production release.
