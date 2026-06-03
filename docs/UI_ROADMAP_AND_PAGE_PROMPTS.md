# EnterprisePOS UI Roadmap And Page Prompts

## Purpose

Ang document na ito ay UI-first roadmap base sa database schema sa `docs/DATABASE_SCHEMA.sql`.
Target nito ang:

- scalable na module structure
- responsive layout para sa phone, tablet, desktop, at large desktop
- cross-platform compatibility sa Windows, macOS, Android, at iOS
- professional enterprise template na puwedeng palawakin habang dinadagdag ang business logic

## Design Principles

- Gumamit ng adaptive layout sa halip na fixed desktop-only grids.
- Panatilihin ang iisang design system: spacing, typography, border radius, elevation, color states, at empty/loading/error states.
- Ihiwalay ang shell navigation, page templates, cards, filters, tables, forms, drawers, at modals bilang reusable components.
- Bawat page ay dapat may:
  - header
  - stats or summary strip kung kailangan
  - filters/search/actions
  - main content area
  - empty/loading/error state
  - mobile-optimized action pattern
- Desktop:
  - may sidebar + top bar
  - data grids, split views, inspector panels
- Tablet:
  - collapsible sidebar
  - 2-column or stacked content depende sa page
- Mobile:
  - bottom sheet, segmented tabs, card lists, sticky actions
  - iwasan ang sobrang sikip na table; gumamit ng card/list summary + details

## Recommended Global UI Structure

## 1. App Shell

- `Dashboard`
- `POS`
- `Sales`
- `Products`
- `Inventory`
- `Purchasing`
- `Customers`
- `Reservations`
- `Kitchen`
- `Finance`
- `Reports`
- `Administration`
- `Settings`

## 2. Shared Layout Templates

- `MasterDetailPageTemplate`
  - desktop: list + detail panel
  - tablet: list + modal detail
  - mobile: card list + full-screen detail
- `AnalyticsPageTemplate`
  - KPI cards
  - charts
  - filter bar
  - comparison widgets
- `TransactionPageTemplate`
  - summary header
  - item list/grid
  - right-side totals on desktop
  - bottom sticky action on mobile
- `SettingsPageTemplate`
  - left navigation categories on desktop
  - accordion/stack on mobile

## Database To UI Module Mapping

## Core Foundation

- `roles`, `permissions`, `role_permissions`, `users`
  - Admin, User Management, Access Control
- `branches`, `pos_terminals`
  - Organization Setup, Branch and Terminal Setup
- `system_settings`, `number_sequences`, `branding`, `licenses`, `app_versions`, `schema_migrations`
  - System Configuration, Branding, Subscription, Versioning

## Commerce And POS

- `sales`, `sale_items`, `sale_item_addons`, `sale_payments`
  - POS Register, Sales History, Receipt Details, Payment Split UI
- `discounts`, `taxes`, `promotions`, `payment_methods`, `receipt_series`
  - Pricing Rules, Taxes, Promotions, Payment Config, Receipt Config
- `cash_drawers`, `cash_drawer_sessions`, `shift_readings`
  - Cash Management, Shift Open/Close, X/Z Reading

## Product And Catalog

- `product_categories`, `products`, `product_variants`, `product_addons`, `product_addon_groups`
  - Product Catalog, Variant Builder, Add-on Configuration
- `price_history`
  - Price Timeline and Audit

## Production And Kitchen

- `recipes`, `recipe_ingredients`, `production`, `production_ingredients`
  - Recipe Builder, Production Batches
- `kitchen_stations`, `kitchen_station_products`, `kitchen_orders`, `kitchen_order_items`, `kitchen_order_item_addons`
  - Kitchen Display System

## Inventory And Supply Chain

- `units`, `unit_conversions`
  - Units and Conversion Management
- `product_stocks`, `stock_movements`, `inventory_valuation`
  - Inventory Dashboard, Stock Ledger, Stock Adjustments
- `suppliers`, `purchase_orders`, `purchase_order_items`, `supplier_performance`
  - Supplier Management, Purchase Orders, Receiving Analytics
- `product_serials`, `product_batches`
  - Serial/Batch Tracking
- `transfer_orders`, `transfer_order_items`
  - Branch Transfer Workflow
- `warehouses`, `warehouse_stocks`
  - Warehouse Management

## CRM And Customer Services

- `customers`, `customer_groups`, `customer_group_memberships`
  - Customer Directory, Segments
- `delivery_addresses`, `delivery_drivers`, `delivery_orders`, `delivery_tracking`
  - Delivery Module
- `reservations`, `tables`
  - Dine-In Floor and Reservation Management
- `appointments`
  - Service Booking Calendar
- `customer_feedback`
  - Feedback and Ratings
- `customer_tax_exemptions`
  - Tax Exemption Profiles

## Loyalty And Stored Value

- `loyalty_programs`, `loyalty_tiers`, `customer_loyalty`, `loyalty_transactions`
  - Loyalty Configuration and Customer Reward Center
- `gift_cards`, `gift_card_transactions`
  - Gift Card Center

## Finance And Exceptions

- `receivables`, `receivable_payments`
  - Accounts Receivable
- `expenses`
  - Expense Tracking
- `sale_refunds`, `sale_voids`, `return_orders`, `return_order_items`
  - Refund/Void/Return Center
- `staff_commissions`
  - Commission Tracking
- `currencies`
  - Currency Settings

## Monitoring And Integrations

- `notifications`, `notification_templates`
  - Notification Center, Template Builder
- `audit_logs`, `error_logs`
  - Audit Trail and Error Monitoring
- `api_integrations`, `api_logs`
  - Integrations and API Monitoring

## Analytics

- `daily_sales_summary`, `product_performance`, `inventory_valuation`, `supplier_performance`
  - Dashboard, Reporting, BI-style insights

## Required UI Modules And Pages

## Phase 1: Foundation And Shell

- App Shell / Navigation
- Unified Dashboard
- Sign In / Lock Screen
- User Profile
- Notification Center
- Global Search

## Phase 2: POS And Sales

- POS Register
- Hold/Open Tabs
- Order Queue
- Payment Sheet
- Sales History
- Receipt Details
- Refund / Void Center
- Cash Drawer Sessions
- Shift Reading

## Phase 3: Catalog And Inventory

- Products List
- Product Editor
- Categories
- Variants And Add-ons
- Units And Conversions
- Stock Overview
- Stock Movement Ledger
- Warehouse Stocks
- Batch Tracking
- Serial Tracking

## Phase 4: Purchasing And Transfers

- Suppliers
- Purchase Orders
- PO Details / Receiving
- Supplier Performance
- Transfer Orders

## Phase 5: CRM, Delivery, Dine-In

- Customers
- Customer Profile
- Customer Groups
- Loyalty
- Gift Cards
- Delivery Orders
- Delivery Tracking
- Tables / Floor Plan
- Reservations
- Appointments
- Feedback

## Phase 6: Admin And Settings

- Branches
- POS Terminals
- Roles And Permissions
- Tax Settings
- Discounts
- Promotions
- Payment Methods
- Receipt Series
- Branding And Theme
- System Settings
- Number Sequences
- Licenses
- App Versions
- Notification Templates
- API Integrations
- Audit Logs
- Error Logs

## Phase 7: Analytics

- Executive Dashboard
- Sales Analytics
- Product Performance
- Inventory Valuation
- Financial Summary
- Staff Performance

## Recommended Navigation Architecture

## Primary Navigation

- Dashboard
- POS
- Sales
- Catalog
- Inventory
- Purchasing
- Customers
- Operations
- Finance
- Reports
- Admin
- Settings

## Secondary Navigation Groups

- `Catalog`
  - Products
  - Categories
  - Variants
  - Add-ons
  - Units
- `Inventory`
  - Stock Overview
  - Movements
  - Warehouses
  - Batches
  - Serials
  - Transfers
- `Operations`
  - Kitchen
  - Delivery
  - Reservations
  - Tables
  - Appointments
- `Finance`
  - Receivables
  - Expenses
  - Refunds
  - Commissions
- `Admin`
  - Users
  - Roles
  - Branches
  - Terminals
  - Taxes
  - Promotions
  - Branding
  - Licenses
  - Integrations

## Responsive Rules Per Page Type

## Desktop 1280+

- left sidebar visible
- top toolbar with breadcrumbs, quick actions, search, notifications
- content may use 2 or 3 columns
- detailed tables with right-side inspector panel

## Tablet 768-1279

- collapsible sidebar
- stack summary cards to 2 columns
- use segmented tabs for dense pages
- inspector becomes modal or bottom drawer

## Mobile Below 768

- bottom tab or compact sidebar sheet
- single-column layout
- filters inside bottom sheet
- cards instead of dense tables
- sticky primary action button
- use modal steps for complex forms

## UI Gaps Based On Database

Mga kulang pa sa current UI kung ikukumpara sa database:

- wala pang full enterprise shell na naka-group by modules
- kulang ang admin/configuration pages kahit malaki ang settings tables
- wala pang purchasing workflow UI
- wala pang inventory ledger, batch, serial, warehouse, at transfer pages
- wala pang CRM pages tulad ng loyalty, gift cards, customer groups, feedback
- wala pang dine-in at reservation floor management
- wala pang KDS pages
- wala pang receivables at exception center
- wala pang integration, audit, at error monitoring pages
- kulang ang reporting and executive analytics surface

## Standard Page Blueprint

Gamitin ito sa halos lahat ng pages:

1. `Page Header`
   - title
   - subtitle
   - branch selector kung relevant
   - date range/filter pill
   - primary and secondary actions
2. `KPI Strip`
   - 3 to 6 summary cards
3. `Filter Bar`
   - search
   - status
   - date
   - branch
   - tags
4. `Content Body`
   - data grid or card list
   - charts or detail panels
5. `States`
   - loading
   - empty
   - error
   - no permission
6. `Responsive Behavior`
   - desktop split
   - tablet stacked
   - mobile cards + bottom actions

## Master Prompt Template

Use this base prompt kapag gagawa ng bagong page UI:

```text
Create a professional, responsive, cross-platform .NET MAUI page for EnterprisePOS.

Page name: [PAGE NAME]
Business module: [MODULE]
Target users: [USER ROLES]
Database tables involved: [TABLES]
Primary use case: [MAIN GOAL]

Design requirements:
- Enterprise-grade UI
- Clean, premium, modern POS/ERP design
- Responsive for desktop, tablet, and mobile
- Compatible with Windows, macOS, Android, and iOS
- Use adaptive layouts, reusable components, and production-ready spacing
- Include loading, empty, and error states
- Avoid web-only patterns that break on MAUI
- Do not use Windows-only fonts or platform-specific controls unless wrapped safely

Layout requirements:
- Desktop: sidebar-friendly, multi-column where appropriate
- Tablet: stacked or split layout with collapsible panels
- Mobile: single-column, sticky primary action, filter bottom sheet if needed

Content requirements:
- Header with title, subtitle, and actions
- KPI cards if relevant
- Search and filter bar
- Main content area
- Detail panel or modal strategy
- Professional color hierarchy and typography

Output:
- XAML layout
- Suggested ViewModel properties
- Suggested reusable components
- Notes for responsiveness
```

## Complete Prompts Per Key Page

## 1. Dashboard

```text
Create a professional, scalable, responsive Executive Dashboard page for EnterprisePOS in .NET MAUI.

Database basis:
- daily_sales_summary
- product_performance
- inventory_valuation
- supplier_performance
- notifications
- branches
- pos_terminals

Goal:
Show top-level business performance for branch managers and executives.

Include:
- top header with branch selector, terminal selector, date range, refresh action
- KPI cards for gross sales, net sales, transactions, average order value, refunds, voids
- charts for sales trend, payment mix, top products, hourly performance
- alerts panel for low stock, expiring batches, open receivables, pending transfers
- activity feed for latest notifications
- desktop 3-column analytics layout
- tablet 2-column layout
- mobile stacked cards and swipeable chart sections

Style:
- premium enterprise dashboard
- soft surfaces, clear contrast, compact but readable
- professional charts and status chips
```

## 2. POS Register

```text
Create a world-class Point of Sale page for EnterprisePOS in .NET MAUI.

Database basis:
- sales
- sale_items
- sale_item_addons
- sale_payments
- customers
- discounts
- taxes
- promotions
- payment_methods
- pos_terminals
- tables
- kitchen_orders

Goal:
Support dine-in, takeout, and delivery orders with fast cashier workflow.

Include:
- product browsing area with categories, products, variants, and add-ons
- cart panel with item notes, modifiers, quantity controls, taxes, discounts, and promos
- quick customer attach
- order type switcher: dine-in, takeout, delivery
- table selector for dine-in
- payment method selection with split payment support
- hold order/open tab support
- responsive cart behavior:
  - desktop right panel
  - tablet collapsible drawer
  - mobile bottom sheet cart
- polished cashier header with terminal, cashier, branch, and quick actions
- empty cart state and product skeleton loading state

Style:
- premium modern POS
- very fast visual scanning
- touch-friendly buttons
- minimal clutter
```

## 3. Sales History

```text
Create a responsive Sales History and Receipt Explorer page for EnterprisePOS in .NET MAUI.

Database basis:
- sales
- sale_items
- sale_payments
- customers
- sale_refunds
- sale_voids
- branches
- pos_terminals

Goal:
Allow users to review transactions, inspect details, filter by branch/date/status, and open receipt records.

Include:
- KPI summary row
- advanced filters: date range, branch, terminal, cashier, status, payment method
- transaction table on desktop
- transaction cards on mobile
- right-side detail inspector for selected sale
- sections for items, payments, taxes, discounts, refunds, void history
- receipt preview layout
- export and print actions
```

## 4. Products List

```text
Create a scalable Product Catalog Management page for EnterprisePOS in .NET MAUI.

Database basis:
- products
- product_categories
- product_variants
- product_addons
- product_addon_groups
- units
- price_history

Goal:
Manage the full product catalog with fast browsing and editing.

Include:
- category tree
- searchable product grid/list toggle
- product cards with price, stock, status, tax flag, category
- bulk actions
- desktop split view with product list and detail preview
- mobile product cards and full-screen editor navigation
- quick badges for variant-enabled, add-on-enabled, taxable, active/inactive
```

## 5. Product Editor

```text
Create a professional Product Create/Edit page for EnterprisePOS in .NET MAUI.

Database basis:
- products
- product_categories
- units
- product_variants
- product_addons
- product_addon_groups
- recipes

Goal:
Allow managers to create products for retail, food, beverage, or service use cases.

Include tabs or sections:
- General Info
- Pricing
- Inventory Settings
- Variants
- Add-ons
- Recipe Link
- Tax and Discount Rules
- Media and Display

Requirements:
- responsive multi-step form
- desktop 2-column editor
- tablet stacked sections
- mobile stepper or accordion form
- validation states and helper text
```

## 6. Inventory Overview

```text
Create an Inventory Overview page for EnterprisePOS in .NET MAUI.

Database basis:
- product_stocks
- stock_movements
- inventory_valuation
- products
- branches
- units
- product_batches
- product_serials

Goal:
Show stock health, reorder risk, valuation, and recent movements.

Include:
- KPI cards for stock value, low stock count, out of stock count, expiring batches
- inventory table with branch, product, quantities, reorder level, status
- low stock and expiry widgets
- branch and warehouse filters
- movement mini-timeline
- desktop data table
- mobile summary cards
```

## 7. Stock Movement Ledger

```text
Create a Stock Movement Ledger page for EnterprisePOS in .NET MAUI.

Database basis:
- stock_movements
- products
- branches
- units
- users

Goal:
Track all stock adjustments, receipts, transfers, production consumption, and sales consumption.

Include:
- filters for product, branch, movement type, date range, reference type, created by
- ledger table with in/out quantity, unit, base quantity, running context, reference links
- color-coded movement type chips
- detail drawer showing source references
- mobile card timeline layout
```

## 8. Purchase Orders

```text
Create a Purchase Orders management page for EnterprisePOS in .NET MAUI.

Database basis:
- suppliers
- purchase_orders
- purchase_order_items
- products
- branches
- units

Goal:
Manage supplier purchasing from creation to receiving.

Include:
- PO status board
- supplier filter
- desktop master-detail list
- create PO action
- PO detail page with items, totals, expected dates, status history
- receiving-ready visual indicators
- mobile cards with status timeline
```

## 9. Suppliers

```text
Create a Supplier Management page for EnterprisePOS in .NET MAUI.

Database basis:
- suppliers
- supplier_performance
- purchase_orders

Goal:
Manage suppliers and review performance.

Include:
- supplier cards or table
- profile summary with contact details
- performance metrics
- linked purchase orders
- scorecards for on-time delivery, quality, spend, average delivery days
```

## 10. Warehouses

```text
Create a Warehouse Management page for EnterprisePOS in .NET MAUI.

Database basis:
- warehouses
- warehouse_stocks
- branches
- users
- products

Goal:
Manage warehouse locations and stock visibility.

Include:
- warehouse list
- warehouse detail with manager, address, phone, active status
- stock table by bin location
- reorder indicators
- mobile-friendly stock cards
```

## 11. Transfer Orders

```text
Create an Inter-Branch Transfer Orders page for EnterprisePOS in .NET MAUI.

Database basis:
- transfer_orders
- transfer_order_items
- branches
- products
- product_batches
- units
- users

Goal:
Handle transfer request, approval, transit, and receiving workflow.

Include:
- status pipeline
- from/to branch comparison
- item list with quantities, units, batches
- approval and receiving actions
- transfer history timeline
- mobile status cards and step progress
```

## 12. Kitchen Display System

```text
Create a Kitchen Display System page for EnterprisePOS in .NET MAUI.

Database basis:
- kitchen_stations
- kitchen_orders
- kitchen_order_items
- kitchen_order_item_addons
- tables
- sales

Goal:
Display and manage food preparation flow by station.

Include:
- station tabs
- order cards grouped by status
- priority and elapsed time badges
- item-level progress actions
- dine-in table reference and special instructions
- responsive board layout:
  - desktop kanban
  - tablet 2-column queue
  - mobile stacked order cards with station filter
```

## 13. Customers

```text
Create a Customer Management page for EnterprisePOS in .NET MAUI.

Database basis:
- customers
- customer_groups
- customer_group_memberships
- customer_feedback
- customer_tax_exemptions

Goal:
Manage customer records, tags, and service insights.

Include:
- searchable customer directory
- profile snapshot cards
- contact info, notes, tax exemption, customer group tags
- purchase summary stats
- feedback history
- mobile-friendly customer profile layout
```

## 14. Customer Profile

```text
Create a Customer Profile page for EnterprisePOS in .NET MAUI.

Database basis:
- customers
- sales
- receivables
- delivery_addresses
- customer_loyalty
- loyalty_transactions
- gift_cards
- customer_feedback
- reservations
- appointments

Goal:
Show 360-degree customer view.

Include:
- profile header
- tabs for purchases, loyalty, receivables, addresses, bookings, feedback
- quick actions for create sale, reservation, appointment, gift card, payment
- desktop tabbed analytics profile
- mobile segmented stacked sections
```

## 15. Loyalty Center

```text
Create a Loyalty Management page for EnterprisePOS in .NET MAUI.

Database basis:
- loyalty_programs
- loyalty_tiers
- customer_loyalty
- loyalty_transactions

Goal:
Manage loyalty program rules and customer point activity.

Include:
- program cards
- tier configuration table
- customer loyalty enrollment list
- points transaction history
- rule editor panel
```

## 16. Gift Card Center

```text
Create a Gift Card Management page for EnterprisePOS in .NET MAUI.

Database basis:
- gift_cards
- gift_card_transactions
- customers

Goal:
Manage issuing, loading, redeeming, and auditing gift cards.

Include:
- gift card search by card number
- status chips
- balance cards
- transaction ledger
- quick actions for issue, load, adjust, void
```

## 17. Delivery Orders

```text
Create a Delivery Orders page for EnterprisePOS in .NET MAUI.

Database basis:
- delivery_orders
- delivery_tracking
- delivery_drivers
- delivery_addresses
- sales
- customers

Goal:
Manage dispatch and live delivery workflow.

Include:
- dispatch board
- status map/timeline style UI
- driver assignment
- address and customer info
- ETA and proof-of-delivery placeholders
- mobile dispatch-friendly cards
```

## 18. Tables And Reservations

```text
Create a Dine-In Tables and Reservations management page for EnterprisePOS in .NET MAUI.

Database basis:
- tables
- reservations
- customers
- branches

Goal:
Manage floor sections, table occupancy, and reservation scheduling.

Include:
- floor/section filter
- table status cards
- reservation calendar/list
- create reservation form
- seat/confirm/no-show actions
- desktop floor map + side schedule
- mobile table cards + reservation list tabs
```

## 19. Appointments

```text
Create an Appointments and Scheduling page for EnterprisePOS in .NET MAUI.

Database basis:
- appointments
- customers
- branches
- products
- users
- employee_schedules

Goal:
Support service bookings with staff scheduling.

Include:
- calendar view
- staff availability
- appointment status filters
- booking details panel
- mobile agenda list and create booking sheet
```

## 20. Finance Center

```text
Create a Finance Center page for EnterprisePOS in .NET MAUI.

Database basis:
- receivables
- receivable_payments
- expenses
- sale_refunds
- sale_voids
- return_orders
- staff_commissions
- currencies

Goal:
Provide operational finance visibility for branch managers.

Include:
- receivable aging cards
- expenses summary
- refunds/voids monitor
- returns queue
- commission summary
- date and branch filters
```

## 21. Roles And Permissions

```text
Create a Roles and Permissions administration page for EnterprisePOS in .NET MAUI.

Database basis:
- roles
- permissions
- role_permissions
- users

Goal:
Manage access control in a clear and safe admin experience.

Include:
- roles list
- permission matrix
- user count per role
- side-by-side compare roles
- mobile accordion permissions
```

## 22. Branches And Terminals

```text
Create a Branches and POS Terminals administration page for EnterprisePOS in .NET MAUI.

Database basis:
- branches
- pos_terminals
- branding
- tax_compliance_settings
- receipt_series

Goal:
Configure physical store and terminal operations.

Include:
- branch cards
- terminal list per branch
- active/inactive states
- tax and receipt setup preview
- quick actions for activate, configure, and assign
```

## 23. Promotions, Discounts, Taxes

```text
Create a Pricing Rules administration page for EnterprisePOS in .NET MAUI.

Database basis:
- promotions
- discounts
- taxes
- loyalty_tiers
- products

Goal:
Configure pricing rules without confusion.

Include:
- tabs for promotions, discounts, taxes
- rule cards with status and validity period
- condition builders
- affected products/customers preview
- desktop rule builder
- mobile wizard flow
```

## 24. Branding And Theme

```text
Create a Branding and Theme Settings page for EnterprisePOS in .NET MAUI.

Database basis:
- branding
- system_settings

Goal:
Allow branch or company-level visual customization.

Include:
- live preview panel
- logo and color controls
- light/dark/custom theme toggle
- receipt header/footer editor
- responsive theme preview for desktop/tablet/mobile mockups
```

## 25. Audit, Errors, Integrations

```text
Create an Admin Monitoring page for EnterprisePOS in .NET MAUI.

Database basis:
- audit_logs
- error_logs
- api_integrations
- api_logs
- notifications

Goal:
Provide operational visibility and debugging tools.

Include:
- tabs for audit logs, error logs, API integrations, API logs
- searchable event table
- severity chips
- integration health cards
- request/response detail inspector
- mobile event cards
```

## Page Build Order Recommendation

1. Dashboard
2. POS Register
3. Sales History
4. Products List
5. Product Editor
6. Inventory Overview
7. Suppliers
8. Purchase Orders
9. Customers
10. Customer Profile
11. Tables And Reservations
12. Kitchen Display
13. Finance Center
14. Roles And Permissions
15. Branches And Terminals
16. Promotions And Taxes
17. Branding And Theme
18. Monitoring And Integrations

## Suggested Reusable Components

- `AppShellSidebar`
- `TopCommandBar`
- `PageHeader`
- `KpiCard`
- `FilterChipBar`
- `ResponsiveDataGrid`
- `CardListView`
- `DetailInspectorPanel`
- `SectionTabs`
- `StatusBadge`
- `TimelineList`
- `EmptyStateView`
- `ErrorStateView`
- `LoadingSkeletonView`
- `FormSectionCard`
- `SummaryTotalsCard`
- `BottomActionBar`
- `SlideOverDrawer`
- `ConfirmActionSheet`

## Next UI Implementation Sequence

Kapag mag-start na sa actual page building, ito ang pinakamagandang sequence:

1. final design tokens and shell cleanup
2. reusable layout components
3. dashboard template
4. POS refinement
5. list/detail admin templates
6. inventory and sales pages
7. CRM and operations pages
8. admin and settings pages
9. analytics and monitoring pages

