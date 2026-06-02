# Database Implementation Roadmap

## Overview
This document outlines the phased implementation of the EnterprisePOS database schema from `DATABASE_SCHEMA.sql` into the local SQLite database with EF Core.

## Phase 1: Core Tables ✅ COMPLETED
**Status:** Completed
**Tables Implemented:**
- ✅ Roles, Permissions, RolePermissions
- ✅ Users
- ✅ Branches, PosTerminals
- ✅ ProductCategories, Units, UnitConversions
- ✅ Products
- ✅ Customers
- ✅ Sales, SaleItems

**Files Created:**
- `Core/Data/Models/Role.cs`
- `Core/Data/Models/Permission.cs`
- `Core/Data/Models/RolePermission.cs`
- `Core/Data/Models/User.cs`
- `Core/Data/Models/Branch.cs`
- `Core/Data/Models/PosTerminal.cs`
- `Core/Data/Models/ProductCategory.cs`
- `Core/Data/Models/Unit.cs`
- `Core/Data/Models/UnitConversion.cs`
- `Core/Data/Models/Product.cs`
- `Core/Data/Models/Customer.cs`
- `Core/Data/Models/Sale.cs`
- `Core/Data/Models/SaleItem.cs`

**Features:**
- All entities inherit from BaseEntity with sync fields
- Navigation properties configured
- Indexes for performance
- Foreign key relationships with proper cascade behaviors
- Soft delete query filters

---

## Phase 2: Inventory Management
**Priority:** High
**Estimated Time:** 2-3 hours

### Tables to Implement:
1. **ProductStocks** - Stock levels per branch
2. **StockMovements** - Stock movement history
3. **Suppliers** - Supplier information
4. **PurchaseOrders** - Purchase orders from suppliers
5. **PurchaseOrderItems** - Items in purchase orders
6. **ProductVariants** - Product variants (size, color, etc.)
7. **ProductAddons** - Product add-ons/modifiers
8. **ProductAddonGroups** - Mapping of add-ons to products
9. **Recipes** - Recipes for products
10. **RecipeIngredients** - Recipe ingredients

### Key Features:
- Unit conversion support for stock movements
- Supplier management
- Purchase order tracking
- Recipe/ingredient consumption tracking
- Product variant support

### Implementation Steps:
1. Create entity models for all tables
2. Add navigation properties
3. Configure relationships in LocalDbContext
4. Add indexes for performance
5. Test CRUD operations

---

## Phase 3: Payments & Financials
**Priority:** High
**Estimated Time:** 1-2 hours

### Tables to Implement:
1. **PaymentMethods** - Payment method configurations
2. **SalePayments** - Payment records for sales
3. **Discounts** - Discount configurations
4. **Taxes** - Tax configurations
5. **SaleDiscounts** - Applied discounts on sales
6. **SaleTaxes** - Applied taxes on sales
7. **Receivables** - Customer receivables
8. **ReceivablePayments** - Receivable payment records

### Key Features:
- Multiple payment methods per sale
- Discount and tax calculations
- Customer credit/receivable tracking
- Payment history

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add calculation logic for discounts/taxes
4. Test payment workflows

---

## Phase 4: Delivery Operations
**Priority:** Medium
**Estimated Time:** 1-2 hours

### Tables to Implement:
1. **DeliveryAddresses** - Customer delivery addresses
2. **DeliveryDrivers** - Delivery driver information
3. **DeliveryOrders** - Delivery order tracking
4. **DeliveryOrderItems** - Items in delivery orders
5. **DeliveryTracking** - Delivery status tracking

### Key Features:
- Multiple delivery addresses per customer
- Driver management
- Real-time delivery tracking
- Delivery status updates

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add status tracking logic
4. Test delivery workflows

---

## Phase 5: Dine-In Operations
**Priority:** Medium
**Estimated Time:** 1-2 hours

### Tables to Implement:
1. **DiningTables** - Restaurant table management
2. **TableSections** - Table sections/zones
3. **Reservations** - Table reservations
4. **ReservationItems** - Items in reservations

### Key Features:
- Table management
- Section organization
- Reservation system
- Table status tracking

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add reservation logic
4. Test dine-in workflows

---

## Phase 6: Kitchen Display System (KDS)
**Priority:** Medium
**Estimated Time:** 1-2 hours

### Tables to Implement:
1. **KitchenStations** - Kitchen station configurations
2. **KitchenOrders** - Kitchen order queue
3. **KitchenOrderItems** - Items in kitchen orders
4. **KitchenOrderItemAddons** - Add-ons for kitchen items

### Key Features:
- Kitchen station management
- Order routing to stations
- Real-time order status
- Add-on support

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add order routing logic
4. Test KDS workflows

---

## Phase 7: Customer Management
**Priority:** Medium
**Estimated Time:** 1-2 hours

### Tables to Implement:
1. **CustomerGroups** - Customer groupings
2. **CustomerGroupMembers** - Group membership
3. **GiftCards** - Gift card management
4. **GiftCardTransactions** - Gift card transactions
5. **CustomerFeedback** - Customer feedback
6. **TaxExemptions** - Customer tax exemptions

### Key Features:
- Customer grouping
- Gift card system
- Feedback collection
- Tax exemption tracking

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add gift card logic
4. Test customer features

---

## Phase 8: Marketing & Promotions
**Priority:** Low
**Estimated Time:** 1-2 hours

### Tables to Implement:
1. **Promotions** - Promotion configurations
2. **PromotionProducts** - Product promotions
3. **NotificationTemplates** - Notification templates
4. **CustomerNotifications** - Customer notifications

### Key Features:
- Promotion management
- Product-specific promotions
- Notification system
- Template management

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add promotion logic
4. Test marketing features

---

## Phase 9: Employee Management
**Priority:** Low
**Estimated Time:** 1-2 hours

### Tables to Implement:
1. **EmployeeSchedules** - Employee schedules
2. **EmployeeCommissions** - Commission tracking
3. **ShiftReadings** - Shift cash readings

### Key Features:
- Schedule management
- Commission calculations
- Shift tracking

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add scheduling logic
4. Test employee features

---

## Phase 10: Advanced Features
**Priority:** Low
**Estimated Time:** 2-3 hours

### Tables to Implement:
1. **Transfers** - Stock transfers between branches
2. **TransferItems** - Items in transfers
3. **Warehouses** - Warehouse management
4. **WarehouseStocks** - Warehouse stock levels
5. **MultiCurrencies** - Currency configurations
6. **ExchangeRates** - Exchange rate tracking
7. **Appointments** - Appointment scheduling
8. **Returns** - Return management
9. **ReturnItems** - Items in returns

### Key Features:
- Multi-branch transfers
- Warehouse management
- Multi-currency support
- Appointment system
- Return processing

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add advanced logic
4. Test features

---

## Phase 11: Developer Tools
**Priority:** Low
**Estimated Time:** 1 hour

### Tables to Implement:
1. **Branding** - App branding settings
2. **VersionTracking** - Version tracking
3. **Licenses** - License management
4. **ApiIntegrations** - API integration configs
5. **ApiIntegrationLogs** - API integration logs

### Key Features:
- Branding customization
- Version control
- License management
- API integration tracking

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add developer tools
4. Test features

---

## Phase 12: Reporting & Analytics
**Priority:** Medium
**Estimated Time:** 2-3 hours

### Tables to Implement:
1. **DailySalesSummary** - Daily sales aggregation
2. **ProductPerformance** - Product performance metrics
3. **InventoryValuation** - Inventory valuation
4. **SalesConsumption** - Sales consumption tracking (already in schema)

### Key Features:
- Daily sales reports
- Product performance analytics
- Inventory valuation
- Consumption tracking

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add reporting logic
4. Test reports

---

## Phase 13: System Configuration
**Priority:** Medium
**Estimated Time:** 1-2 hours

### Tables to Implement:
1. **SystemSettings** - System-wide settings
2. **NumberSequences** - Auto-increment sequences
3. **TaxComplianceSettings** - Tax compliance settings (generic)
4. **ShiftReadings** - Shift readings (already in schema)

### Key Features:
- System configuration
- Number sequence management
- Tax compliance settings
- Shift management

### Implementation Steps:
1. Create entity models
2. Configure relationships
3. Add configuration logic
4. Test system settings

---

## Implementation Guidelines

### Naming Convention
- SQL: `snake_case` for tables and columns
- C#: `PascalCase` for classes and properties
- Navigation properties: Use singular for single, plural for collections

### Relationship Configuration
- **Cascade**: When parent deleted, children also deleted
- **Restrict**: Cannot delete parent if children exist
- **SetNull**: Set foreign key to null when parent deleted

### Index Strategy
- Unique indexes on natural keys (Code, Username, Email)
- Composite indexes on frequently queried combinations
- Foreign key indexes for join performance

### Soft Delete
- All entities inherit from BaseEntity with `IsDeleted` field
- Global query filters automatically exclude deleted records
- To include deleted records, use `IgnoreQueryFilters()`

### Sync Support
- All entities have sync fields: `ServerId`, `LastSyncedAt`, `NeedsSync`
- Sync queue tracks pending operations
- Conflict resolution strategies: server wins, client wins, last-write-wins

### Security
- Sensitive fields marked with comments (Phone, Email, PasswordHash)
- Encryption service available for field-level encryption
- Configuration service for app settings

---

## Testing Strategy

### Unit Tests
- Test CRUD operations for each entity
- Test relationship navigation
- Test soft delete behavior
- Test sync operations

### Integration Tests
- Test complex workflows (sale → payment → stock update)
- Test transaction rollback on errors
- Test concurrent access
- Test database migration

### Performance Tests
- Test query performance with large datasets
- Test index effectiveness
- Test connection pooling
- Test batch operations

---

## Migration Strategy

### EF Core Migrations
- Use EF Core migrations for schema changes
- Create migration for each phase
- Test migrations on fresh database
- Test migration rollback

### Data Seeding
- Seed initial data (roles, permissions, default settings)
- Seed sample data for testing
- Seed configuration values
- Seed default units and categories

---

## Completion Checklist

For each phase:
- [ ] Create all entity models
- [ ] Add navigation properties
- [ ] Configure relationships in LocalDbContext
- [ ] Add indexes for performance
- [ ] Add global query filters for soft delete
- [ ] Update ILocalDbContext interface
- [ ] Build and test compilation
- [ ] Write unit tests
- [ ] Test CRUD operations
- [ ] Document any special considerations

---

## Notes

- Database location: `%LocalAppData%\EnterprisePOS\local.db`
- Configuration location: `%LocalAppData%\EnterprisePOS\config.json`
- Backup location: `%LocalAppData%\EnterprisePOS\backup_*.db`
- Always build after each phase to catch compilation errors early
- Test relationships before moving to next phase
- Keep models simple - add business logic in services, not models
