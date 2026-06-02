# EnterprisePOS Architecture

## Folder Structure

The project uses a **feature-based modular architecture** for scalability and maintainability.

```
EnterprisePOS/
├── Core/                          # Shared infrastructure
│   ├── Models/                   (PosNavItem, DashboardMetric)
│   ├── Validators/               (ValidationResult, IValidator)
│   └── DTOs/                     (UserDto)
│
├── Features/                      # Feature modules
│   ├── POS/                      # Point of Sale module
│   │   ├── Models/               (Product, CartItem, ProductCategory)
│   │   ├── DTOs/                 (ProductDto, CartDto)
│   │   ├── Repositories/         (ProductRepository, CartRepository)
│   │   ├── Services/             (MockPosService)
│   │   ├── Validators/           (ProductValidator)
│   │   ├── ViewModels/           (POSViewModel)
│   │   └── Views/                (POSPage)
│   │
│   ├── Dashboard/                # Dashboard module
│   │   ├── Models/
│   │   ├── DTOs/
│   │   ├── Repositories/
│   │   ├── Services/             (MockDashboardService)
│   │   ├── Validators/
│   │   ├── ViewModels/           (DashboardViewModel)
│   │   └── Views/                (DashboardPage, DashboardHomePage)
│   │
│   ├── Products/                 # Products module
│   │   ├── Models/
│   │   ├── DTOs/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   ├── Validators/
│   │   ├── ViewModels/
│   │   └── Views/                (ProductsPage)
│   │
│   ├── Inventory/                # Inventory module
│   │   ├── Models/
│   │   ├── DTOs/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   ├── Validators/
│   │   ├── ViewModels/
│   │   └── Views/                (InventoryPage)
│   │
│   └── Settings/                 # Settings module
│       ├── Models/
│       ├── DTOs/
│       ├── Repositories/
│       ├── Services/
│       ├── Validators/
│       ├── ViewModels/           (SettingsViewModel)
│       └── Views/                (SettingsPage)
│
├── Interfaces/                   # Base interfaces
├── Repositories/                 # Base repository (InMemoryRepository)
├── Services/                     # Shared services (ThemeService, LoggingService, ShellNavigationService)
├── Components/                   # Reusable UI components
├── Helpers/                      # Utility classes
├── Navigation/                   # Navigation service
├── Themes/                       # Theme management
└── Configurations/                # App configuration
```

## Benefits of This Structure

1. **Easy to find related files** - All files for a feature are in one place
2. **Scalable** - Adding new features doesn't clutter existing folders
3. **Maintainable** - Changes to a feature don't affect other features
4. **Team collaboration** - Different developers can work on different features without conflicts
5. **Easy refactoring** - Moving or removing a feature is straightforward

## Migration from Current Structure

The current structure is organized by type (Models/, Services/, etc.). We are migrating to feature-based organization for better scalability.

**Current → New Mapping:**
- `Models/Product.cs` → `Features/POS/Models/Product.cs`
- `DTOs/ProductDto.cs` → `Features/POS/DTOs/ProductDto.cs`
- `Repositories/ProductRepository.cs` → `Features/POS/Repositories/ProductRepository.cs`
- `Services/MockPosService.cs` → `Features/POS/Services/MockPosService.cs`
- `ViewModels/POSViewModel.cs` → `Features/POS/ViewModels/POSViewModel.cs`
- `Views/POSPage.xaml` → `Features/POS/Views/POSPage.xaml`

## Layer Responsibilities

### Models
- Domain entities representing business objects
- No external dependencies
- Pure data structures

### DTOs
- Data transfer objects for API communication
- Separate from domain models to decouple API contracts
- Include Create/Update variants for different operations

### Repositories
- Data access layer
- Implement CRUD operations
- Handle database/API calls
- Return domain models

### Services
- Business logic layer
- Orchestrate repository calls
- Implement validation
- Handle complex operations

### Validators
- Input validation logic
- Return validation results
- Reusable across services

### ViewModels
- MVVM pattern
- Bind to Views
- Handle UI logic
- Call Services

### Views
- UI definition (XAML)
- No business logic
- Bind to ViewModels

## Dependency Flow

```
View → ViewModel → Service → Repository → Database/API
         ↓              ↓            ↓
      Validator     DTO         Model
```

## Future Considerations

- **API Integration**: Repositories will switch from in-memory to HTTP client
- **Caching**: Add caching layer between Service and Repository
- **SignalR**: Add real-time updates via SignalR in Services
- **Offline Support**: Add offline queue in Repository layer

## Database

For detailed database schema information, see [DATABASE.md](DATABASE.md).
