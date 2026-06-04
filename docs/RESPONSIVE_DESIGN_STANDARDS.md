# EnterprisePOS Responsive Design Standards

## Overview
This document defines the responsive design standards and UI design system for all components and pages in EnterprisePOS. Follow these guidelines when creating new modules to ensure consistent, adaptive, and visually clean UI across all screen sizes and platforms (Windows, macOS, Android, iOS).

---

## UI Design System (Visual Language)

### Design Philosophy
Inspired by modern POS systems (Loyverse, Changx, PayPoint), our UI follows:
- **Minimalist & clean** — white backgrounds, generous spacing, no visual clutter
- **Black-primary buttons** — dark/black for primary actions, white text
- **Subtle borders** — light gray `#E5E7EB` borders, no heavy shadows
- **One accent color** — indigo `#4F46E5` for charts, links, and data highlights
- **Clear hierarchy** — bold black headings, gray descriptions, muted meta text

### Color Palette

| Token | Hex | Usage |
|-------|-----|-------|
| Primary | `#111111` | Primary buttons, active nav items |
| PrimaryDark | `#000000` | Hover state for primary buttons |
| Accent | `#4F46E5` | Charts, links, data highlights |
| Success | `#10B981` | Completed status, positive trends |
| Warning | `#F59E0B` | Pending status, attention needed |
| Danger | `#EF4444` | Error states, critical alerts |
| Gray900 | `#111827` | Headings, primary text |
| Gray500 | `#6B7280` | Secondary text, descriptions |
| Gray400 | `#9CA3AF` | Muted/meta text |
| Gray200 | `#E5E7EB` | Borders, dividers |
| Gray100 | `#F3F4F6` | Secondary backgrounds, hover states |
| White | `#FFFFFF` | Card backgrounds, sidebar |
| BackgroundSecondary | `#F9FAFB` | Page canvas background |

### Button Styles

```xml
<!-- Primary Button: Black bg, white text, rounded -->
<Button Style="{StaticResource PrimaryButton}" Text="Confirm" />
<!-- Result: bg=#111111, text=white, cornerRadius=12, bold -->

<!-- Outline Button: White bg, gray border, dark text -->
<Button Style="{StaticResource OutlineButton}" Text="Cancel" />
<!-- Result: bg=transparent, border=#E5E7EB, text=#111827, cornerRadius=12 -->

<!-- POS Primary Button: Same black, slightly more rounded -->
<Button Style="{StaticResource PosPrimaryButton}" Text="Add to Order" />
<!-- Result: bg=#111111, text=white, cornerRadius=12 -->
```

### Card Pattern

```xml
<!-- Standard card: white bg, subtle border, 16-18px rounded corners -->
<Border BackgroundColor="{StaticResource White}"
        Stroke="{StaticResource PosBorder}"
        StrokeThickness="1"
        Padding="{OnIdiom Phone=12, Tablet=16, Desktop=18}"
        StrokeShape="RoundRectangle 16">
    <!-- Card content -->
</Border>
```

### Navigation (Sidebar)

- **Desktop**: White background sidebar with border separator
- **Active item**: Black text with bold weight or dark background pill
- **Inactive item**: Gray500 text
- **Section headers**: Gray400 uppercase text, small font
- **Mobile**: Bottom tab navigation

### Typography Hierarchy

| Level | Font Size (Desktop) | Weight | Color |
|-------|-------------------|--------|-------|
| Page Title | 28-32px | Bold | Gray900 |
| Section Title | 17-19px | Bold | Gray900 |
| Card Title | 15-17px | Bold | Gray900 |
| Body | 13-15px | Regular | Gray700 |
| Caption/Meta | 11-13px | Regular | Gray500 |
| Muted | 10-12px | Regular | Gray400 |

### Status Badges

```xml
<!-- Success badge -->
<Border BackgroundColor="{StaticResource SuccessLight}" Padding="8,4" StrokeThickness="0" StrokeShape="RoundRectangle 8">
    <Label Text="Completed" FontSize="12" TextColor="{StaticResource Success}" />
</Border>

<!-- Danger badge -->
<Border BackgroundColor="{StaticResource DangerLight}" Padding="8,4" StrokeThickness="0" StrokeShape="RoundRectangle 8">
    <Label Text="Failed" FontSize="12" TextColor="{StaticResource Danger}" />
</Border>
```

### Platform Compatibility
All UI must render correctly on:
- **Windows** (desktop, mouse/keyboard)
- **macOS** (desktop, trackpad)
- **Android** (phone/tablet, touch)
- **iOS** (phone/tablet, touch)

Use `OnIdiom` and `OnPlatform` where platform-specific adjustments are needed.

---

## Device Breakpoints

MAUI's `OnIdiom` supports three main device types:

| Idiom | Screen Width | Typical Devices |
|-------|-------------|-----------------|
| Phone | < 768px | Mobile phones, small tablets |
| Tablet | 768px - 1024px | Tablets (iPad, Android tablets) |
| Desktop | > 1024px | Laptops, desktops, large displays |

---

## Core Responsive Principles

### 1. Always Use OnIdiom for Sizing
Never hardcode fixed sizes. Use `OnIdiom` for all responsive properties:

```xml
<!-- Font Sizes -->
<Label FontSize="{OnIdiom Phone=12, Tablet=14, Desktop=16}" />

<!-- Padding/Margin -->
<Border Padding="{OnIdiom Phone=12, Tablet=16, Desktop=24}" />

<!-- Width/Height -->
<Grid HeightRequest="{OnIdiom Phone=120, Tablet=148, Desktop=160}" />

<!-- Grid Columns -->
<Grid ColumnDefinitions="{OnIdiom Phone='*,Auto', Tablet='*,*,Auto', Desktop='*,*,*,Auto'}" />
```

### 2. Use FlexLayout for Adaptive Cards
FlexLayout with `Wrap` and `Basis` is ideal for responsive card layouts:

```xml
<FlexLayout Direction="Row" Wrap="Wrap" JustifyContent="SpaceBetween">
    <Border FlexLayout.Basis="{OnIdiom Phone='100%', Tablet='48%', Desktop='31%'}" />
</FlexLayout>
```

### 3. Responsive Grid Spans
For CollectionView grids, use responsive span:

```xml
<CollectionView.ItemsLayout>
    <GridItemsLayout 
        Orientation="Vertical"
        Span="{OnIdiom Phone=1, Tablet=2, Desktop=4}"
        HorizontalItemSpacing="{OnIdiom Phone=8, Tablet=12, Desktop=16}"
        VerticalItemSpacing="{OnIdiom Phone=8, Tablet=12, Desktop=16}" />
</CollectionView.ItemsLayout>
```

---

## Component-Specific Standards

### Buttons

#### Primary Button
```xml
<Button Style="{StaticResource PrimaryButton}" />
```

**Responsive Properties:**
- Phone: FontSize 14, Height 44, Padding 16,10
- Tablet: FontSize 15, Height 48, Padding 18,12
- Desktop: FontSize 16, Height 52, Padding 20,14

#### Secondary/Outline Button
```xml
<Button Style="{StaticResource OutlineButton}" />
```

**Responsive Properties:**
- Phone: FontSize 12, Height 36, Padding 10,6
- Tablet: FontSize 13, Height 40, Padding 12,8
- Desktop: FontSize 14, Height 44, Padding 14,10

#### Icon Button
```xml
<Border Style="{StaticResource IconButton}" 
        WidthRequest="{OnIdiom Phone=32, Tablet=36, Desktop=40}"
        HeightRequest="{OnIdiom Phone=32, Tablet=36, Desktop=40}" />
```

### Typography Scale

| Element | Phone | Tablet | Desktop |
|---------|-------|--------|---------|
| H1 (Page Title) | 24 | 28 | 32 |
| H2 (Section Header) | 20 | 22 | 24 |
| H3 (Card Title) | 16 | 18 | 20 |
| Body Text | 13 | 14 | 15 |
| Caption/Small | 11 | 12 | 13 |
| Tiny | 10 | 11 | 12 |

### Spacing Scale

| Context | Phone | Tablet | Desktop |
|---------|-------|--------|---------|
| XS (2px) | 2 | 2 | 2 |
| SM (4px) | 4 | 4 | 4 |
| MD (8px) | 8 | 8 | 8 |
| LG (12px) | 12 | 12 | 12 |
| XL (16px) | 16 | 16 | 16 |
| 2XL (20px) | 16 | 20 | 24 |
| 3XL (24px) | 20 | 24 | 28 |

### Card Components

#### Product Card
```xml
<Border Padding="{OnIdiom Phone=10, Tablet=12, Desktop=16}">
    <Grid HeightRequest="{OnIdiom Phone=120, Tablet=148, Desktop=160}">
        <!-- Image area -->
    </Grid>
    <Label FontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}" />
    <Label FontSize="{OnIdiom Phone=11, Tablet=12, Desktop=13}" />
</Border>
```

#### Metric Card
```xml
<Border Padding="{OnIdiom Phone=12, Tablet=16, Desktop=20}">
    <Label FontSize="{OnIdiom Phone=10, Tablet=12, Desktop=14}" />
    <Label FontSize="{OnIdiom Phone=20, Tablet=24, Desktop=28}" />
</Border>
```

### Input Fields

#### SearchBar/Entry
```xml
<SearchBar 
    MinimumHeightRequest="{OnIdiom Phone=40, Tablet=44, Desktop=48}"
    FontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}"
    Padding="{OnIdiom Phone=12, Tablet=14, Desktop=16}" />
```

#### DatePicker/TimePicker
```xml
<DatePicker 
    MinimumHeightRequest="{OnIdiom Phone=40, Tablet=44, Desktop=48}"
    FontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}" />
```

#### Picker
```xml
<Picker 
    MinimumHeightRequest="{OnIdiom Phone=40, Tablet=44, Desktop=48}"
    FontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}" />
```

### PageHeader Component

```xml
<components:PageHeader 
    TitleFontSize="{OnIdiom Phone=24, Tablet=28, Desktop=32}"
    DescriptionFontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}"
    Padding="{OnIdiom Phone=16, Tablet=20, Desktop=24}" />
```

### EmptyStateView Component

```xml
<components:EmptyStateView 
    IconSize="{OnIdiom Phone=48, Tablet=64, Desktop=80}"
    TitleFontSize="{OnIdiom Phone=18, Tablet=20, Desktop=24}"
    MessageFontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}"
    Padding="{OnIdiom Phone=24, Tablet=32, Desktop=40}" />
```

---

## Layout Patterns

### Pattern 1: Responsive Header
```xml
<Grid ColumnDefinitions="{OnIdiom Phone='*,Auto', Tablet='*,Auto', Desktop='*,Auto'}">
    <components:PageHeader 
        TitleFontSize="{OnIdiom Phone=24, Tablet=28, Desktop=32}" />
    <Button WidthRequest="{OnIdiom Phone=120, Tablet=140, Desktop=160}" />
</Grid>
```

### Pattern 2: Stats Grid
```xml
<Grid ColumnDefinitions="{OnIdiom Phone='*', Tablet='*,*', Desktop='*,*,*,*'}"
      ColumnSpacing="{OnIdiom Phone=8, Tablet=12, Desktop=16}">
    <components:MetricCardView />
    <components:MetricCardView />
    <components:MetricCardView />
    <components:MetricCardView />
</Grid>
```

### Pattern 3: Two-Column Layout
```xml
<Grid ColumnDefinitions="{OnIdiom Phone='*', Tablet='*,*'}"
      ColumnSpacing="{OnIdiom Phone=0, Tablet=16, Desktop=24}">
    <!-- Left content -->
    <!-- Right content -->
</Grid>
```

### Pattern 4: Sidebar + Content
```xml
<Grid ColumnDefinitions="{OnIdiom Phone='*', Tablet='200,*', Desktop='240,*'}">
    <components:Sidebar IsVisible="{OnIdiom Phone=False, Tablet=True, Desktop=True}" />
    <ContentArea />
</Grid>
```

### Pattern 5: Form Layout
```xml
<Grid ColumnDefinitions="{OnIdiom Phone='*', Tablet='*,*'}"
      RowDefinitions="Auto,Auto,Auto,Auto"
      ColumnSpacing="{OnIdiom Phone=0, Tablet=16, Desktop=24}"
      RowSpacing="{OnIdiom Phone=12, Tablet=16, Desktop=20}">
    <Label Grid.Row="0" Grid.Column="0" Text="Name" />
    <Entry Grid.Row="1" Grid.Column="0" />
    
    <Label Grid.Row="2" Grid.Column="0" Text="Email" />
    <Entry Grid.Row="3" Grid.Column="0" />
    
    <!-- On tablet/desktop, show additional fields in second column -->
    <Label Grid.Row="0" Grid.Column="1" 
           IsVisible="{OnIdiom Phone=False, Tablet=True, Desktop=True}" Text="Phone" />
    <Entry Grid.Row="1" Grid.Column="1" 
           IsVisible="{OnIdiom Phone=False, Tablet=True, Desktop=True}" />
</Grid>
```

### Pattern 6: List Item Layout
```xml
<Grid ColumnDefinitions="Auto,*,Auto"
      ColumnSpacing="{OnIdiom Phone=8, Tablet=12, Desktop=16}"
      Padding="{OnIdiom Phone=12, Tablet=16, Desktop=20}">
    <Border Grid.Column="0" 
            WidthRequest="{OnIdiom Phone=40, Tablet=48, Desktop=56}"
            HeightRequest="{OnIdiom Phone=40, Tablet=48, Desktop=56}" />
    <VerticalStackLayout Grid.Column="1" Spacing="{OnIdiom Phone=2, Tablet=4, Desktop=4}">
        <Label FontSize="{OnIdiom Phone=14, Tablet=15, Desktop=16}" />
        <Label FontSize="{OnIdiom Phone=12, Tablet=13, Desktop=14}" />
    </VerticalStackLayout>
    <Label Grid.Column="2" FontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}" />
</Grid>
```

### Pattern 7: Modal/Dialog
```xml
<Border BackgroundColor="{StaticResource White}"
        Stroke="{StaticResource PosBorder}"
        StrokeThickness="1"
        StrokeShape="RoundRectangle 16"
        Padding="{OnIdiom Phone=16, Tablet=24, Desktop=32}"
        WidthRequest="{OnIdiom Phone='100%', Tablet=500, Desktop=600}"
        HorizontalOptions="Center"
        VerticalOptions="Center">
    <VerticalStackLayout Spacing="{OnIdiom Phone=16, Tablet=20, Desktop=24}">
        <Label FontSize="{OnIdiom Phone=18, Tablet=20, Desktop=24}" />
        <Label FontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}" />
        <Button />
    </VerticalStackLayout>
</Border>
```

---

## Design Tokens Update

Update `DesignTokens.xaml` with responsive values:

```xml
<!-- Responsive Font Sizes -->
<x:Double x:Key="Font_H1_Phone">24</x:Double>
<x:Double x:Key="Font_H1_Tablet">28</x:Double>
<x:Double x:Key="Font_H1_Desktop">32</x:Double>

<!-- Responsive Padding -->
<x:Double x:Key="Pad_Phone">12</x:Double>
<x:Double x:Key="Pad_Tablet">16</x:Double>
<x:Double x:Key="Pad_Desktop">24</x:Double>

<!-- Responsive Spacing -->
<x:Double x:Key="Space_Phone">8</x:Double>
<x:Double x:Key="Space_Tablet">12</x:Double>
<x:Double x:Key="Space_Desktop">16</x:Double>
```

---

## Style Updates Required

### Update PosStyles.xaml

All button and input styles should use OnIdiom:

```xml
<Style x:Key="PosPrimaryButton" TargetType="Button">
    <Setter Property="FontSize" Value="{OnIdiom Phone=14, Tablet=15, Desktop=16}" />
    <Setter Property="MinimumHeightRequest" Value="{OnIdiom Phone=44, Tablet=48, Desktop=52}" />
    <Setter Property="Padding" Value="{OnIdiom Phone='16,10', Tablet='18,12', Desktop='20,14'}" />
</Style>

<Style x:Key="PosSearchBarCompact" TargetType="SearchBar">
    <Setter Property="MinimumHeightRequest" Value="{OnIdiom Phone=36, Tablet=38, Desktop=40}" />
    <Setter Property="FontSize" Value="{OnIdiom Phone=12, Tablet=13, Desktop=14}" />
</Style>
```

---

## Checklist for New Modules

When creating a new page or component:

- [ ] All FontSize properties use OnIdiom
- [ ] All Padding/Margin use OnIdiom
- [ ] All WidthRequest/HeightRequest use OnIdiom
- [ ] Grid ColumnDefinitions use OnIdiom
- [ ] CollectionView Span uses OnIdiom
- [ ] Buttons use responsive styles
- [ ] Input fields use responsive heights
- [ ] Cards use responsive padding
- [ ] Test on Phone, Tablet, Desktop viewports
- [ ] Test in portrait and landscape orientations

---

## Testing Responsiveness

### Windows Simulator
```bash
# Test different screen sizes
# Use Visual Studio's device toolbar to switch between:
# - Mobile (5.5" phone)
# - Tablet (10" tablet)
# - Desktop (1920x1080)
```

### Visual States for Orientation
```csharp
protected override void OnSizeAllocated(double width, double height)
{
    base.OnSizeAllocated(width, height);
    
    if (width != _oldWidth || height != _oldHeight)
    {
        _oldWidth = width;
        _oldHeight = height;
        
        // Handle orientation changes if needed
        // Update layout-specific properties
    }
}
```

---

## Common Mistakes to Avoid

❌ **Don't hardcode sizes:**
```xml
<Label FontSize="14" />  <!-- Wrong -->
<Border Padding="16" />  <!-- Wrong -->
```

✅ **Use OnIdiom:**
```xml
<Label FontSize="{OnIdiom Phone=12, Tablet=14, Desktop=16}" />
<Border Padding="{OnIdiom Phone=12, Tablet=16, Desktop=24}" />
```

❌ **Don't use fixed grid spans:**
```xml
<GridItemsLayout Span="4" />  <!-- Wrong -->
```

✅ **Use responsive spans:**
```xml
<GridItemsLayout Span="{OnIdiom Phone=1, Tablet=2, Desktop=4}" />
```

❌ **Don't ignore small screens:**
```xml
<Grid ColumnDefinitions="*,*,*,*,*">  <!-- Too many for phone -->
```

✅ **Adapt to screen size:**
```xml
<Grid ColumnDefinitions="{OnIdiom Phone='*', Tablet='*,*', Desktop='*,*,*,*,*'}">
```

---

## Examples from Current Codebase

### Good Example: DashboardPage.xaml
```xml
<FlexLayout.Basis="{OnIdiom Phone='100%', Tablet='48%', Desktop='31%'}" />
```

### Needs Improvement: ProductsPage.xaml
```xml
<!-- Current (fixed) -->
<GridItemsLayout Orientation="Vertical" Span="4" />

<!-- Should be (responsive) -->
<GridItemsLayout 
    Orientation="Vertical"
    Span="{OnIdiom Phone=1, Tablet=2, Desktop=4}" />
```

### Needs Improvement: PosProductCardView.xaml
```xml
<!-- Current (fixed) -->
<Grid HeightRequest="148">
<Label FontSize="14">

<!-- Should be (responsive) -->
<Grid HeightRequest="{OnIdiom Phone=120, Tablet=148, Desktop=160}">
<Label FontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}">
```

### Fixed: PageHeader.xaml ✅
```xml
<!-- Now responsive -->
<VerticalStackLayout Spacing="{OnIdiom Phone=2, Tablet=4, Desktop=4}">
    <Label FontSize="{OnIdiom Phone=10, Tablet=11, Desktop=12}" />  <!-- Eyebrow -->
    <Label FontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}" />  <!-- Description -->
    <Label FontSize="{OnIdiom Phone=11, Tablet=12, Desktop=13}" />  <!-- Meta -->
</VerticalStackLayout>
```

### Fixed: EmptyStateView.xaml ✅
```xml
<!-- Now responsive -->
<VerticalStackLayout Padding="{OnIdiom Phone=24, Tablet=32, Desktop=40}" 
                    Spacing="{OnIdiom Phone=8, Tablet=12, Desktop=16}">
    <Label FontSize="{OnIdiom Phone=48, Tablet=64, Desktop=80}" />  <!-- Icon -->
    <Label FontSize="{OnIdiom Phone=16, Tablet=18, Desktop=20}" />  <!-- Title -->
    <Label FontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}" />  <!-- Message -->
</VerticalStackLayout>
```

---

## Implementation Priority

1. **High Priority** - Core components (buttons, inputs, cards)
2. **Medium Priority** - Page layouts (grids, flex layouts)
3. **Low Priority** - Fine-tuning (spacing adjustments)

---

## Additional Components to Update

### LoadingView Component
```xml
<components:LoadingView 
    SpinnerSize="{OnIdiom Phone=32, Tablet=40, Desktop=48}"
    MessageFontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}"
    Padding="{OnIdiom Phone=24, Tablet=32, Desktop=40}" />
```

### ErrorView Component
```xml
<components:ErrorView 
    IconSize="{OnIdiom Phone=48, Tablet=64, Desktop=80}"
    TitleFontSize="{OnIdiom Phone=16, Tablet=18, Desktop=20}"
    MessageFontSize="{OnIdiom Phone=13, Tablet=14, Desktop=15}"
    Padding="{OnIdiom Phone=24, Tablet=32, Desktop=40}" />
```

### Navigation/Shell Responsive Patterns
```xml
<!-- Flyout Width -->
<Shell.FlyoutWidth>
    <OnIdiom x:TypeArguments="x:Double">
        <OnIdiom.Phone>280</OnIdiom.Phone>
        <OnIdiom.Tablet>320</OnIdiom.Tablet>
        <OnIdiom.Desktop>360</OnIdiom.Desktop>
    </OnIdiom>
</Shell.FlyoutWidth>

<!-- TabBar Height -->
<Shell.TabBarHeight>
    <OnIdiom x:TypeArguments="x:Double">
        <OnIdiom.Phone>56</OnIdiom.Phone>
        <OnIdiom.Tablet>60</OnIdiom.Tablet>
        <OnIdiom.Desktop>64</OnIdiom.Desktop>
    </OnIdiom>
</Shell.TabBarHeight>
```

---

## Image and Media Responsiveness

### Image Sizing
```xml
<!-- Responsive image dimensions -->
<Image 
    WidthRequest="{OnIdiom Phone=100, Tablet=120, Desktop=140}"
    HeightRequest="{OnIdiom Phone=100, Tablet=120, Desktop=140}"
    Aspect="AspectFill" />
```

### Avatar/Profile Images
```xml
<Border 
    WidthRequest="{OnIdiom Phone=32, Tablet=40, Desktop=48}"
    HeightRequest="{OnIdiom Phone=32, Tablet=40, Desktop=48}">
    <Border.StrokeShape>
        <RoundRectangle CornerRadius="{OnIdiom Phone=16, Tablet=20, Desktop=24}" />
    </Border.StrokeShape>
</Border>
```

---

## Accessibility Considerations

### Minimum Touch Targets
Ensure touch targets meet accessibility guidelines:
```xml
<Button MinimumHeightRequest="{OnIdiom Phone=44, Tablet=44, Desktop=44}"
        MinimumWidthRequest="{OnIdiom Phone=44, Tablet=44, Desktop=44}" />
```

### Text Scaling
Support system font scaling where possible:
```xml
<Label FontSize="{OnIdiom Phone=14, Tablet=16, Desktop=18}"
       FontFamily="Default" />
```

---

## Performance Best Practices

### Lazy Loading for Large Lists
```xml
<CollectionView ItemsSource="{Binding Items}"
                RemainingItemsThreshold="5"
                RemainingItemsThresholdReachedCommand="{Binding LoadMoreCommand}">
    <!-- Items -->
</CollectionView>
```

### Image Optimization
```xml
<Image Source="{Binding ImageUrl}"
       Aspect="AspectFill"
       CacheDuration="30" />
```

---

## Quick Reference Card

### Common OnIdiom Values

| Property | Phone | Tablet | Desktop |
|----------|-------|--------|---------|
| Padding (small) | 8 | 12 | 16 |
| Padding (medium) | 12 | 16 | 24 |
| Padding (large) | 16 | 24 | 32 |
| Font (tiny) | 10 | 11 | 12 |
| Font (caption) | 11 | 12 | 13 |
| Font (body) | 13 | 14 | 15 |
| Font (h3) | 16 | 18 | 20 |
| Font (h2) | 20 | 22 | 24 |
| Font (h1) | 24 | 28 | 32 |
| Button height | 44 | 48 | 52 |
| Input height | 40 | 44 | 48 |
| Icon size | 32 | 36 | 40 |
| Spacing | 8 | 12 | 16 |

---

## Resources

- [MAUI Responsive UI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/markup-extensions/consume#onidiom-markup-extension)
- [FlexLayout Documentation](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/flexlayout)
- [Grid Layout Documentation](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/grid)

---

**Last Updated:** June 3, 2026
**Version:** 1.0
