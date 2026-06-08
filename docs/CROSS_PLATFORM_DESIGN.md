# Cross-Platform Design Guide

## Current Platform Support

The project is configured for cross-platform support:
- **Android** (API 21+ / Android 5.0+)
- **iOS** (15.0+)
- **MacCatalyst** (15.0+)
- **Windows** (10.0.17763.0+)

## Responsive Breakpoints

Current breakpoints defined in `Helpers/LayoutBreakpoints.cs`:

```csharp
MobileMax = 599              // Phone / narrow
LargeTabletLandscapeMin = 768 // Landscape tablet
TabletMax = 899              // Tablet / small laptop
DesktopMin = 900             // Desktop
```

## Platform-Specific Requirements

### Android
- **Safe Areas**: Status bar, navigation bar, notch/cutout
- **Touch Targets**: Minimum 48x48dp (7.5mm)
- **Material Design Guidelines**: Follow Material 3 standards
- **Navigation**: Bottom navigation or drawer navigation
- **Keyboard**: Handle soft keyboard appearance/disappearance

### iOS
- **Safe Areas**: Status bar, home indicator, notch (Dynamic Island)
- **Touch Targets**: Minimum 44x44pt
- **Human Interface Guidelines**: Follow Apple HIG standards
- **Navigation**: Tab bar or navigation stack
- **Gestures**: Swipe back, pull to refresh

### Windows
- **Safe Areas**: Title bar, taskbar
- **Touch Targets**: Minimum 44x44px
- **Fluent Design**: Follow Fluent Design System
- **Navigation**: Sidebar or top navigation
- **Keyboard**: Handle on-screen keyboard

## Platform-Specific Handlers

### Overview

Platform-specific handlers allow you to customize the native rendering of MAUI controls on each platform. This is the recommended approach for platform-specific styling that cannot be achieved through XAML or resource dictionaries.

### Handler Pattern

#### 1. Create Platform-Specific Handler

Create a handler file in the appropriate platform folder:

**Windows Handler Example** (`Platforms/Windows/EntryHandler.cs`):
```csharp
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace EnterprisePOS.Platforms.Windows;

public class EntryHandlerCustom : EntryHandler
{
    protected override void ConnectHandler(TextBox platformView)
    {
        base.ConnectHandler(platformView);
        
        // Apply platform-specific styling
        platformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
        platformView.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        platformView.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 209, 213, 219));
        platformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
        platformView.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8);
        platformView.Padding = new Microsoft.UI.Xaml.Thickness(12, 8, 12, 8);
        
        // Maintain styling on focus/hover
        platformView.GotFocus += (s, e) =>
        {
            platformView.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 209, 213, 219));
            platformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
        };
    }
}
```

**Android Handler Example** (`Platforms/Android/EntryHandler.cs`):
```csharp
using Microsoft.Maui.Handlers;
using Android.Widget;
using Android.Content.Res;

namespace EnterprisePOS.Platforms.Android;

public class EntryHandlerCustom : EntryHandler
{
    protected override void ConnectHandler(EditText platformView)
    {
        base.ConnectHandler(platformView);
        
        // Apply Android-specific styling
        platformView.Background = null; // Remove default underline
        platformView.SetPadding(24, 16, 24, 16);
    }
}
```

#### 2. Register Handler Globally

Register the handler in `MauiProgram.cs` using platform-specific compilation directives:

```csharp
builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    })
    .ConfigureMauiHandlers(handlers =>
    {
#if WINDOWS
        handlers.AddHandler<Microsoft.Maui.Controls.Entry, EnterprisePOS.Platforms.Windows.EntryHandlerCustom>();
        handlers.AddHandler<Microsoft.Maui.Controls.Picker, EnterprisePOS.Platforms.Windows.PickerHandlerCustom>();
#elif ANDROID
        handlers.AddHandler<Microsoft.Maui.Controls.Entry, EnterprisePOS.Platforms.Android.EntryHandlerCustom>();
#elif IOS
        handlers.AddHandler<Microsoft.Maui.Controls.Entry, EnterprisePOS.Platforms.iOS.EntryHandlerCustom>();
#elif MACCATALYST
        handlers.AddHandler<Microsoft.Maui.Controls.Entry, EnterprisePOS.Platforms.MacCatalyst.EntryHandlerCustom>();
#endif
    });
```

### Current Global Handlers

The project currently has the following global handlers registered:

#### Windows Handlers
- **EntryHandlerCustom** - Customizes Entry controls with:
  - White background
  - Black text
  - Gray border (1px)
  - Rounded corners (8px)
  - Consistent padding
  - Border persistence on focus/hover

- **PickerHandlerCustom** - Customizes Picker controls with:
  - White background
  - Black text
  - Gray border (1px)
  - Rounded corners (8px)
  - Light theme for dropdown popup
  - Delayed styling via `Loaded` event to prevent data binding issues

### Adding New Handlers

#### Step-by-Step Process

1. **Create Handler File**
   - Navigate to `Platforms/[Platform]/` folder
   - Create new file: `[Control]Handler.cs`
   - Inherit from appropriate base handler (e.g., `EntryHandler`, `PickerHandler`)

2. **Implement ConnectHandler**
   - Override `ConnectHandler` method
   - Access native platform view via `platformView` parameter
   - Apply platform-specific styling
   - Handle events if needed (focus, hover, etc.)

3. **Register in MauiProgram.cs**
   - Add handler registration in `ConfigureMauiHandlers`
   - Wrap in appropriate `#if [PLATFORM]` directive
   - Use full namespace for handler class

4. **Test on Target Platform**
   - Build and run on target platform
   - Verify styling applies correctly
   - Test control functionality (data binding, events, etc.)

### Handler Best Practices

#### DO
- Use handlers for platform-specific native rendering
- Apply styling that cannot be achieved through XAML
- Handle platform-specific events (focus, hover, etc.)
- Use platform-specific compilation directives
- Test on actual target platform devices

#### DON'T
- Don't use handlers for cross-platform styling (use Styles.xaml instead)
- Don't break data binding with handler modifications
- Don't apply styling that conflicts with platform guidelines
- Don't forget to unregister handlers if replacing default behavior
- Don't assume handler timing - use `Loaded` event for delayed styling

### Common Control Handlers

#### Entry/Editor
- Windows: `TextBox` styling
- Android: `EditText` styling
- iOS: `UITextField` styling
- MacCatalyst: `NSTextField` styling

#### Picker
- Windows: `ComboBox` styling
- Android: `Spinner` styling
- iOS: `UISPickerView` styling
- MacCatalyst: `NSPopUpButton` styling

#### Button
- Windows: `Button` styling
- Android: `AppCompatButton` styling
- iOS: `UIButton` styling
- MacCatalyst: `NSButton` styling

### Platform-Specific Considerations

#### Windows
- Use `Microsoft.UI.Xaml` namespace for WinUI 3 controls
- Use `Loaded` event for delayed styling to avoid data binding issues
- Set `RequestedTheme` to enforce light/dark mode
- Handle ComboBox popup styling separately from control styling

#### Android
- Use `Android.Widget` namespace for native controls
- Consider Material Design guidelines
- Handle soft keyboard appearance
- Use `AppCompat` widgets for consistent styling across Android versions

#### iOS
- Use `UIKit` namespace for native controls
- Follow Human Interface Guidelines
- Handle safe areas and notches
- Use semantic colors for dark mode support

#### MacCatalyst
- Use `AppKit` namespace for native controls
- Follow macOS design guidelines
- Handle window chrome and title bar
- Consider touch bar support if applicable

## Design Tokens

### Spacing
- **Extra Small**: 4px
- **Small**: 8px
- **Medium**: 16px
- **Large**: 24px
- **Extra Large**: 32px

### Touch Targets
- **Minimum**: 44x44 (iOS/Windows), 48x48 (Android)
- **Recommended**: 48x48 for all platforms

### Border Radius
- **Small**: 4px
- **Medium**: 8px
- **Large**: 16px
- **Extra Large**: 24px

### Typography
- **Headline**: 32px / Bold
- **Title**: 24px / SemiBold
- **Subtitle**: 18px / Medium
- **Body**: 16px / Regular
- **Caption**: 14px / Regular
- **Small**: 12px / Regular

## Safe Area Implementation

### MAUI Safe Area Support
```xml
<ContentPage 
    xmlns:ios="clr-namespace:Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;assembly=Microsoft.Maui.Controls"
    ios:Page.UseSafeArea="True">
```

### Custom Safe Area Padding
```csharp
// In code-behind or view model
var safeInsets = On<iOS>().SafeAreaInsets();
```

## Responsive Layout Strategy

### Mobile (< 600px)
- Single column layout
- Bottom navigation
- Full-width cards
- Collapsible menus
- Touch-optimized controls

### Tablet (600px - 899px)
- Two-column layout
- Sidebar navigation (collapsible)
- Grid-based cards
- Split views for detail pages

### Desktop (900px+)
- Three-column layout
- Fixed sidebar
- Dense data grids
- Hover states
- Keyboard shortcuts

## Platform-Specific Adaptations

### Android
- Use Material Design components
- Ripple effects on touch
- Bottom sheet for actions
- Snackbar for notifications

### iOS
- Use iOS-style controls
- Smooth animations
- Context menus
- Native alerts

### Windows
- Use Fluent Design
- Acrylic materials
- Window chrome
- Command bar

## Testing Strategy

### Screen Sizes to Test
- **Small Phone**: 360x640 (Android), 375x667 (iPhone SE)
- **Large Phone**: 414x896 (iPhone XR), 412x915 (Pixel 4)
- **Tablet Portrait**: 768x1024 (iPad), 800x1280 (Android tablet)
- **Tablet Landscape**: 1024x768 (iPad), 1280x800 (Android tablet)
- **Desktop**: 1366x768, 1920x1080, 2560x1440

### Orientation Changes
- Portrait to landscape
- Landscape to portrait
- Foldable devices

### Safe Area Scenarios
- Notch/cutout
- Dynamic Island
- Status bar height variations
- Navigation bar variations

## Implementation Checklist

- [ ] Add safe area handling to all pages
- [ ] Ensure minimum touch targets (44x44 iOS, 48x48 Android)
- [ ] Test on actual devices (emulators not enough)
- [ ] Add platform-specific visual effects
- [ ] Implement platform-specific navigation patterns
- [ ] Add keyboard handling for mobile
- [ ] Test orientation changes
- [ ] Add accessibility support (screen readers)
- [ ] Test dark mode on all platforms
- [ ] Add platform-specific permissions handling

## Resources

- [MAUI Documentation](https://docs.microsoft.com/maui)
- [Material Design 3](https://m3.material.io)
- [Apple Human Interface Guidelines](https://developer.apple.com/design/human-interface-guidelines)
- [Fluent Design](https://fluent2.microsoft.design)

---

# Offline-First Architecture

## Overview

EnterprisePOS is designed with an **offline-first architecture** that ensures continuous operation even without internet connectivity, with automatic synchronization when online.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      Client Application                      │
│  (WPF / Web / Mobile)                                        │
└────────────┬────────────────────────────────────────────────┘
             │
             ├───┬──────────────────────────────────────┐
             │   │                                      │
             ▼   ▼                                      ▼
    ┌─────────────┐                         ┌─────────────────┐
    │ Local Data  │                         │  Sync Queue     │
    │ (SQLite)    │                         │  (Pending Ops)  │
    └─────────────┘                         └─────────────────┘
             │                                          │
             │                                          │
             ▼                                          ▼
    ┌─────────────────────────────────────────────────────────────┐
│              Connection Status Detector                         │
│           (Online/Offline Detection)                           │
└─────────────────────────────────────────────────────────────┘
             │
             │ (When Online)
             ▼
    ┌─────────────────────────────────────────────────────────────┐
│                  Sync Service                                   │
│           (Bidirectional Synchronization)                        │
└─────────────────────────────────────────────────────────────┘
             │
             ▼
    ┌─────────────────────────────────────────────────────────────┐
│              ASP.NET Core Web API                               │
│              (MySQL Database)                                    │
└─────────────────────────────────────────────────────────────┘
```

## Data Flow

### Online Mode
1. Client reads/writes to local SQLite database
2. Changes are immediately synced to server
3. Server validates and stores in MySQL
4. Client receives confirmation
5. Sync queue is cleared

### Offline Mode
1. Client reads/writes to local SQLite database
2. Changes are queued in sync queue
3. Application continues to function normally
4. No server communication

### Reconnection
1. Connection status detector detects online status
2. Sync service processes queued operations
3. Bidirectional sync with server
4. Conflict resolution if needed
5. Local data updated with server changes

## Technology Stack

### Cross-Platform Backend
- **ASP.NET Core 8+** - Cross-platform Web API
- **Entity Framework Core** - ORM with multi-database support
- **MySQL** - Production database (online)
- **SQLite** - Local database (offline)

### Synchronization
- **SQLite** - Local offline storage
- **Sync Queue** - Track pending operations
- **Conflict Resolution** - Handle data conflicts
- **Connection Detection** - Monitor network status

## Offline Storage Strategy

### Local Database (SQLite)
- Same schema as production MySQL database
- Stores all critical data locally
- Enables full offline functionality
- Lightweight and fast

### Data Categories

#### Critical Data (Must Work Offline)
- Products and inventory
- Customers
- Sales transactions
- Receipts
- User authentication (cached)

#### Sync-Required Data
- New product updates
- Price changes
- Customer data updates
- Sales data upload
- Inventory adjustments

#### Reference Data (Cached)
- Product categories
- Units and conversions
- Payment methods
- Tax rates
- System settings

## Synchronization Strategy

### Sync Queue Structure
```csharp
public class SyncOperation
{
    public long Id { get; set; }
    public string EntityType { get; set; }  // "Product", "Sale", "Customer"
    public string Operation { get; set; }   // "Create", "Update", "Delete"
    public string EntityData { get; set; }   // JSON data
    public DateTime CreatedAt { get; set; }
    public DateTime? SyncedAt { get; set; }
    public string Status { get; set; }      // "Pending", "Synced", "Failed"
    public string ErrorMessage { get; set; }
}
```

### Sync Process

#### Upload (Client → Server)
1. Get pending operations from sync queue
2. Send to server in batches
3. Server validates and processes
4. Update sync queue status
5. Handle failures with retry logic

#### Download (Server → Client)
1. Request data changes since last sync
2. Server returns updated/created records
3. Client updates local database
4. Update last sync timestamp

### Conflict Resolution

#### Conflict Scenarios
1. **Same record modified offline and online**
   - Strategy: Last-write-wins with timestamp
   - Alternative: Manual resolution for critical data

2. **Record deleted offline but modified online**
   - Strategy: Server version wins
   - Mark local as deleted

3. **Record created offline with same ID**
   - Strategy: Generate new ID locally
   - Update references on sync

#### Conflict Handling
```csharp
public enum ConflictResolutionStrategy
{
    ServerWins,        // Always use server version
    ClientWins,        // Always use client version
    LastWriteWins,     // Use most recent timestamp
    ManualResolution   // Require user intervention
}
```

## Connection Detection

### Network Status Monitoring
```csharp
public interface IConnectionService
{
    bool IsOnline { get; }
    event EventHandler<bool> ConnectionChanged;
    Task<bool> CheckConnectionAsync();
}
```

### Fallback Behavior
- **Online**: Direct API calls + sync queue processing
- **Offline**: Local database only + queue operations
- **Unstable**: Retry with exponential backoff

## Repository Pattern

### Dual Repository Implementation
```csharp
public interface IProductRepository
{
    Task<Product> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(int id);
}

public class ProductRepository : IProductRepository
{
    private readonly IConnectionService _connectionService;
    private readonly ILocalProductRepository _localRepo;
    private readonly IRemoteProductRepository _remoteRepo;

    public async Task<Product> GetByIdAsync(int id)
    {
        if (_connectionService.IsOnline)
        {
            // Try remote first, fallback to local
            var product = await _remoteRepo.GetByIdAsync(id);
            if (product != null)
            {
                await _localRepo.UpdateAsync(product);
                return product;
            }
        }
        return await _localRepo.GetByIdAsync(id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        // Always add to local first
        var localProduct = await _localRepo.AddAsync(product);

        if (_connectionService.IsOnline)
        {
            // Sync to server
            await _remoteRepo.AddAsync(product);
        }
        else
        {
            // Queue for sync
            await _syncQueue.AddAsync(new SyncOperation
            {
                EntityType = "Product",
                Operation = "Create",
                EntityData = JsonSerializer.Serialize(product)
            });
        }

        return localProduct;
    }
}
```

## API Design

### RESTful Endpoints
- **GET /api/products** - List products
- **GET /api/products/{id}** - Get product
- **POST /api/products** - Create product
- **PUT /api/products/{id}** - Update product
- **DELETE /api/products/{id}** - Delete product

### Sync Endpoints
- **POST /api/sync/upload** - Upload offline changes
- **GET /api/sync/download** - Download server changes
- **GET /api/sync/status** - Check sync status

### Authentication
- **JWT Bearer Tokens** - Standard cross-platform auth
- **Token Refresh** - Automatic token renewal
- **Offline Token Caching** - Use cached token when offline

## Data Consistency

### Transaction Management
- Local transactions always committed
- Server transactions validated before commit
- Rollback on sync failure

### Data Validation
- Client-side validation (immediate feedback)
- Server-side validation (final check)
- Sync validation (conflict detection)

## Performance Considerations

### Offline Performance
- SQLite is fast for local operations
- Indexes on frequently queried columns
- Lazy loading for large datasets

### Sync Performance
- Batch operations (100 records per batch)
- Differential sync (only changed data)
- Background sync (non-blocking UI)
- Compress data transfer

### Caching Strategy
- Cache reference data locally
- Invalidate cache on sync
- Preload critical data on startup

## Deployment Scenarios

### Single Location (No Internet)
- Local SQLite only
- No sync needed
- Full offline functionality

### Multi-Location (With Internet)
- Local SQLite + MySQL
- Regular sync to central server
- Conflict resolution as needed

### Cloud-Only (Always Online)
- Direct MySQL connection
- No local storage needed
- Real-time updates

## Security Considerations

### Offline Security
- Local database encryption
- Secure token storage
- Biometric authentication (when available)

### Sync Security
- HTTPS for all sync operations
- Certificate pinning
- Data encryption in transit

### Data Privacy
- Sensitive data encryption at rest
- Secure key management
- Compliance with data protection laws

## Testing Strategy

### Offline Testing
- Test all features without network
- Verify data persistence
- Test sync queue functionality

### Sync Testing
- Test conflict scenarios
- Verify data integrity
- Test partial sync failures

### Integration Testing
- Test online/offline transitions
- Verify automatic reconnection
- Test concurrent sync operations

## Migration Path

### Phase 1: Offline-Only
- Implement local SQLite database
- Full offline functionality
- No sync capability

### Phase 2: Add Sync
- Implement sync queue
- Add sync service
- Test sync operations

### Phase 3: Online Integration
- Deploy ASP.NET Core Web API
- Configure MySQL database
- Enable cloud sync

### Phase 4: Optimization
- Performance tuning
- Conflict resolution refinement
- Advanced sync features
