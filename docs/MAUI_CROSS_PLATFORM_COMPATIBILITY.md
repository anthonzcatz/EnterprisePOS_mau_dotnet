# MAUI Cross-Platform Compatibility Guide

## Overview

This document provides a reference for which .NET MAUI controls, properties, and features are safe to use across all target platforms (Windows, Android, iOS, macOS). Following this guide prevents runtime crashes caused by unsupported features on specific platforms.

---

## Platform Targets

| Platform | Framework | Runtime |
|----------|-----------|---------|
| Windows | `net10.0-windows10.0.19041.0` | WinUI 3 / Windows App SDK |
| Android | `net10.0-android` | Android API 21+ |
| iOS | `net10.0-ios` | iOS 14+ |
| macOS | `net10.0-maccatalyst` | macOS 11+ |

---

## Safe Controls (Always Compatible)

These controls work reliably on **all platforms** without platform-specific handling:

| Control | Notes |
|---------|-------|
| `Label` | Core text display — fully supported |
| `Button` | Primary action control — fully supported |
| `Entry` | Single-line text input — fully supported |
| `Editor` | Multi-line text input — fully supported |
| `Image` | Image display — fully supported |
| `ScrollView` | Scrollable container — fully supported |
| `StackLayout` | Linear layout — fully supported |
| `Grid` | Grid-based layout — fully supported |
| `FlexLayout` | Flexible box layout — fully supported |
| `CollectionView` | Virtualized list — fully supported |
| `Frame` | Bordered container WITH built-in shadow — **preferred over Border** |
| `ContentPage` | Root page container — fully supported |
| `Border` | Bordered container — **NO shadow support on Windows** |
| `BoxView` | Simple colored box — **NO ToolTip support** |
| `Picker` | Dropdown selection — fully supported |
| `DatePicker` | Date selection — fully supported |
| `Switch` | Toggle control — fully supported |
| `Slider` | Range selection — fully supported |
| `Stepper` | Increment/decrement — fully supported |
| `ProgressBar` | Progress indicator — fully supported |
| `ActivityIndicator` | Loading spinner — fully supported |
| `WebView` | Browser control — fully supported |

---

## Border vs Frame — CRITICAL

### `Border` — Limited Support

The `Border` control has **platform-specific limitations**:

| Feature | Windows | Android | iOS/macOS |
|---------|---------|---------|-----------|
| `Stroke` | Yes | Yes | Yes |
| `StrokeShape` | Yes | Yes | Yes |
| `Padding` | Yes | Yes | Yes |
| `Shadow` property | **NO** | Partial | Partial |
| `VisualStateManager` / `PointerOver` | **NO** | Yes | Yes |

**Recommendation:** Use `Frame` instead of `Border` when you need shadows or hover effects.

### `Frame` — Full Support

`Frame` is the **preferred container** for cards with visual depth:

```xml
<Frame BackgroundColor="{StaticResource White}"
       BorderColor="{StaticResource PosBorder}"
       CornerRadius="18"
       Padding="18"
       HasShadow="True">
    <!-- Content -->
</Frame>
```

| Property | Windows | Android | iOS/macOS |
|----------|---------|---------|-----------|
| `HasShadow` | Yes | Yes | Yes |
| `CornerRadius` | Yes | Yes | Yes |
| `BorderColor` | Yes | Yes | Yes |
| `BackgroundColor` | Yes | Yes | Yes |
| `Padding` | Yes | Yes | Yes |
| `VisualStateManager` | Yes | Yes | Yes |

---

## Shadow Support

### ❌ Do NOT use on `Border`

```xml
<!-- WRONG — Crashes on Windows -->
<Border Shadow="{StaticResource CardShadow}">
```

### ✅ Use on `Frame` instead

```xml
<!-- CORRECT — Works on all platforms -->
<Frame HasShadow="True"
       ShadowColor="{StaticResource Gray300}">
```

### Shadow Alternatives by Platform

| Approach | Windows | Android | iOS | macOS |
|----------|---------|---------|-----|-------|
| `Frame.HasShadow` | Yes | Yes | Yes | Yes |
| `Frame.ShadowColor` | Yes | Yes | Yes | Yes |
| `Border.Shadow` property | **NO** | Partial | Partial | Partial |
| Custom renderer | Complex | Complex | Complex | Complex |

---

## Hover Effects

### ❌ Do NOT use `PointerOver` VisualState on `Border`

```xml
<!-- WRONG — Crashes on Windows -->
<Border>
    <VisualStateManager.VisualStateGroups>
        <VisualStateGroup x:Name="CommonStates">
            <VisualState x:Name="PointerOver">  <!-- Not supported -->
                <Setter Property="Scale" Value="1.01" />
            </VisualState>
        </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>
</Border>
```

### ✅ Safe Hover Alternatives

**Option 1: Use `Frame` + `VisualStateManager`**

```xml
<Frame>
    <VisualStateManager.VisualStateGroups>
        <VisualStateGroup x:Name="CommonStates">
            <VisualState x:Name="Normal">
                <Setter Property="BackgroundColor" Value="White" />
            </VisualState>
            <VisualState x:Name="PointerOver">
                <Setter Property="BackgroundColor" Value="{StaticResource Gray50}" />
            </VisualState>
        </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>
</Frame>
```

**Option 2: Use `PointerGestureRecognizer`** (Recommended — fully cross-platform)

```xml
<Frame BackgroundColor="White">
    <Frame.GestureRecognizers>
        <PointerGestureRecognizer
            PointerEntered="OnPointerEntered"
            PointerExited="OnPointerExited" />
    </Frame.GestureRecognizers>
    <!-- Content -->
</Frame>
```

```csharp
private void OnPointerEntered(object sender, PointerEventArgs e)
{
    var frame = (Frame)sender;
    frame.BackgroundColor = Colors.LightGray;
}

private void OnPointerExited(object sender, PointerEventArgs e)
{
    var frame = (Frame)sender;
    frame.BackgroundColor = Colors.White;
}
```

**Option 3: Use `TapGestureRecognizer` for click feedback**

```xml
<Frame BackgroundColor="White">
    <Frame.GestureRecognizers>
        <TapGestureRecognizer Tapped="OnCardTapped" />
    </Frame.GestureRecognizers>
</Frame>
```

---

## ToolTip Support

### ❌ Do NOT use `ToolTip` on `BoxView`

```xml
<!-- WRONG — Not supported on BoxView -->
<BoxView>
    <BoxView.ToolTip>
        <ToolTip Content="Data value" />
    </BoxView.ToolTip>
</BoxView>
```

### ✅ Safe ToolTip Targets

| Control | Windows | Android | iOS | macOS |
|---------|---------|---------|-----|-------|
| `Button` | Yes | Yes | Yes | Yes |
| `Label` | Yes | Yes | Limited | Limited |
| `Image` | Yes | Yes | Limited | Limited |
| `Entry` | Yes | Yes | Yes | Yes |
| `BoxView` | **NO** | **NO** | **NO** | **NO** |

### ✅ Recommended ToolTip Pattern

Wrap chart bars in a `Border` or `Frame` that supports tooltips:

```xml
<Frame BackgroundColor="{StaticResource ChartBlue}"
       CornerRadius="4"
       HeightRequest="24">
    <Frame.ToolTip>
        <ToolTip Content="Cash: 45% (₱67,500)" />
    </Frame.ToolTip>
</Frame>
```

Or use a `Label` with tooltip:

```xml
<Label Text="45%">
    <Label.ToolTip>
        <ToolTip Content="Cash payments: 45%" />
    </Label.ToolTip>
</Label>
```

---

## Property Compatibility Matrix

### Layout Properties

| Property | Windows | Android | iOS | macOS | Notes |
|----------|---------|---------|-----|-------|-------|
| `Padding` | Yes | Yes | Yes | Yes | Universal |
| `Margin` | Yes | Yes | Yes | Yes | Universal |
| `CornerRadius` | Yes | Yes | Yes | Yes | Use on `Frame` or `Border` |
| `Shadow` (attached) | **NO** | Partial | Partial | Partial | Use `Frame.HasShadow` |
| `ZIndex` | Yes | Yes | Yes | Yes | Layout ordering |
| `Opacity` | Yes | Yes | Yes | Yes | Universal |
| `Scale` | Yes | Yes | Yes | Yes | Universal |
| `Rotation` | Yes | Yes | Yes | Yes | Universal |

### Color Resources

| Resource Type | Safe? | Notes |
|---------------|-------|-------|
| `Color` | Yes | Define in `Colors.xaml` |
| `SolidColorBrush` | Yes | Define for brushes |
| `LinearGradientBrush` | Yes | Works on all platforms |
| `RadialGradientBrush` | Limited | Test on Windows first |

### Markup Extensions

| Extension | Safe? | Example |
|-----------|-------|---------|
| `{StaticResource}` | Yes | Universal |
| `{DynamicResource}` | Yes | Universal |
| `{Binding}` | Yes | Universal |
| `{OnIdiom}` | Yes | Phone/Tablet/Desktop |
| `{OnPlatform}` | Yes | Platform-specific values |
| `{AppThemeBinding}` | Yes | Light/Dark mode |

---

## Testing Checklist

Before committing UI changes, verify on:

- [ ] **Windows** — Build and run WinUI 3 app
- [ ] **Android** — Build APK and test on device/emulator
- [ ] **iOS** — Test on iOS simulator or device
- [ ] **macOS** — Test on Mac Catalyst build

### Quick Validation Commands

```bash
# Windows
 dotnet build -f net10.0-windows10.0.19041.0

# Android
 dotnet build -f net10.0-android

# iOS (requires Mac)
 dotnet build -f net10.0-ios

# macOS (requires Mac)
 dotnet build -f net10.0-maccatalyst
```

---

## Common Pitfalls & Solutions

### Pitfall 1: Using `Border.Shadow`

```xml
<!-- WRONG -->
<Border Shadow="{StaticResource CardShadow}">
```

```xml
<!-- CORRECT -->
<Frame HasShadow="True" ShadowColor="{StaticResource Gray300}">
```

### Pitfall 2: `PointerOver` on `Border`

```xml
<!-- WRONG -->
<Border>
    <VisualStateManager.VisualStateGroups>
        <VisualState x:Name="PointerOver"> <!-- Not supported -->
```

```xml
<!-- CORRECT -->
<Frame>
    <VisualStateManager.VisualStateGroups>
        <VisualState x:Name="PointerOver"> <!-- Supported -->
```

### Pitfall 3: `ToolTip` on `BoxView`

```xml
<!-- WRONG -->
<BoxView>
    <BoxView.ToolTip> <!-- Not supported -->
```

```xml
<!-- CORRECT -->
<Frame BackgroundColor="Blue">
    <Frame.ToolTip> <!-- Supported -->
        <ToolTip Content="Value" />
    </Frame.ToolTip>
</Frame>
```

### Pitfall 4: `OnIdiom` inside `StrokeShape`

```xml
<!-- WRONG -->
<Border StrokeShape="RoundRectangle {OnIdiom Phone=22, Tablet=25, Desktop=28}">
```

```xml
<!-- CORRECT -->
<Border StrokeShape="RoundRectangle 24">
```

### Pitfall 5: `OnIdiom` inside `Margin` or `CornerRadius`

```xml
<!-- WRONG -->
<Border Margin="0,{OnIdiom Phone=6, Tablet=8, Desktop=8},0,0">
```

```xml
<!-- CORRECT -->
<Border Margin="0,8,0,0">
```

---

## Recommended Patterns for EnterprisePOS

### Card Container (Cross-Platform Safe)

```xml
<Frame BackgroundColor="{StaticResource White}"
       BorderColor="{StaticResource PosBorder}"
       CornerRadius="18"
       Padding="18"
       HasShadow="True"
       ShadowColor="{StaticResource Gray300}">
    <!-- Card content -->
</Frame>
```

### Chart Bar with Tooltip (Cross-Platform Safe)

```xml
<Frame BackgroundColor="{StaticResource ChartBlue}"
       CornerRadius="4"
       HeightRequest="120"
       WidthRequest="20">
    <Frame.ToolTip>
        <ToolTip Content="Monday: ₱24,500" />
    </Frame.ToolTip>
</Frame>
```

### Hover Effect (Cross-Platform Safe)

```xml
<Frame BackgroundColor="White"
       x:Name="MyCard">
    <Frame.GestureRecognizers>
        <PointerGestureRecognizer
            PointerEntered="OnHoverIn"
            PointerExited="OnHoverOut" />
    </Frame.GestureRecognizers>
</Frame>
```

```csharp
void OnHoverIn(object sender, PointerEventArgs e) =>
    ((Frame)sender).BackgroundColor = Color.FromArgb("#F8FAFD");

void OnHoverOut(object sender, PointerEventArgs e) =>
    ((Frame)sender).BackgroundColor = Colors.White;
```

---

## References

- [Microsoft MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [MAUI Platform Differences](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/)
- [WinUI 3 Controls Gallery](https://learn.microsoft.com/en-us/windows/apps/design/controls/)
- [Android Material Design](https://m3.material.io/)
- [Apple Human Interface Guidelines](https://developer.apple.com/design/human-interface-guidelines/)

---

**Document Version:** 1.0  
**Last Updated:** 2026-06-06  
**Maintainer:** EnterprisePOS Development Team
