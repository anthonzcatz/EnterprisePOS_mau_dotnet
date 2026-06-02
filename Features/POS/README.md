# POS Feature

Point of Sale module for handling transactions, cart management, and product selection.

## Structure

- **Models/** - Domain entities (Product, CartItem, ProductCategory)
- **DTOs/** - Data transfer objects for API communication (ProductDto, CartDto)
- **Repositories/** - Data access layer (ProductRepository, CartRepository)
- **Services/** - Business logic (MockPosService - to be refactored)
- **Validators/** - Input validation (ProductValidator)
- **ViewModels/** - MVVM view models (POSViewModel)
- **Views/** - UI pages (POSPage)

## Key Components

### POSViewModel
- Manages cart state and operations
- Handles product filtering and search
- Manages UI state (sidebar, mobile cart, checkout modal)
- Commands for cart operations (Add, Remove, Update quantity)

### POSPage
- Responsive layout for desktop and mobile
- Desktop: Sidebar + Product grid + Cart panel
- Mobile: Header + Product grid + Cart overlay + Checkout modal

## Future Enhancements

- Refactor MockPosService to use repositories
- Add real API integration
- Add SignalR for real-time updates
- Add offline support
- Add barcode scanning
