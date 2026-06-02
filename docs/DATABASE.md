# EnterprisePOS Database Schema

## Overview

The database schema is designed to be generic and versatile, supporting multiple business types (retail, restaurant, service) without being tied to specific regions or niches.

## Schema Summary

The complete database schema includes **70+ tables** organized into the following modules:

### Core System
- Users, roles, permissions with RBAC
- Multi-branch support with terminals
- System settings and configuration

### Products & Inventory
- Products with variants and categories
- Unit conversion system (base units, display units, conversion factors)
- Stock management with movements
- Supplier management and purchase orders
- Serial number tracking for high-value items
- Batch/lot tracking with expiry dates

### Sales & Payments
- Sales with items and add-ons/modifiers
- Multiple payment methods (cash, card, digital wallet, bank transfer, check, gift card)
- Discounts and taxes
- Refunds and voids
- Automatic ingredient deduction via recipes

### Delivery Operations
- Customer delivery addresses with GPS coordinates
- Driver management and availability
- Delivery orders with tracking
- Support for pickup, delivery, and third-party options

### Dine-In Operations
- Table/seat management with capacity
- Reservations with status workflow
- Walk-in and reservation-based dining

### Kitchen Display System (KDS)
- Preparation stations (grill, bar, prep, etc.)
- Product routing to stations
- Order display with priority levels
- Individual item status tracking
- Add-on/modifier display

### Customer Management
- Customer profiles and groups
- Loyalty programs with tiers
- Gift card management
- Tax exemption certificates
- Feedback and ratings

### Marketing
- Promotions (percentage, fixed, BOGO, bundle)
- Notification templates (email, SMS, push)
- Customer segmentation

### Employee Management
- Staff schedules and shifts
- Commission tracking
- Attendance management

### Advanced Features
- Inter-branch inventory transfers
- Warehouse management
- Multi-currency support
- Service appointments
- Return management with approval workflow
- Supplier performance tracking
- Quotations and estimates
- API integrations with logging

### Developer Tools
- Branding and UI customization
- Schema version tracking and migrations
- Application version management
- License and subscription management

### Reporting
- Daily sales summary
- Product performance
- Inventory valuation
- Multiple reporting views

## Schema Design Principles

1. **Generic Terminology** - No country-specific or business-specific references
2. **Multi-Business Support** - Works for retail, restaurant, and service businesses
3. **Multi-Region Support** - Tax compliance configurable for different regions
4. **Unit Conversion** - Comprehensive system for weight, volume, pieces, time
5. **Audit Trail** - Created/updated timestamps and user tracking
6. **Soft Deletes** - Is_active flags for data preservation
7. **Foreign Key Constraints** - Referential integrity with appropriate cascade rules
8. **Indexing Strategy** - Optimized for common query patterns

## Database File

The complete SQL schema is available in `docs/DATABASE_SCHEMA.sql`.

## Key Features

### Unit Conversion
- Support for any measurement system with automatic conversion
- Base units and display units with conversion factors
- Complex unit relationships via unit_conversions table

### Recipe Management
- Automatic ingredient deduction from sales
- Production tracking for made-to-order items
- Support for food preparation or service bundling

### Tax Compliance
- Generic settings for any tax authority
- Tax ID and permit tracking
- Shift readings for end-of-day reporting
- Customer tax exemption certificates

### Multi-Currency
- Exchange rate tracking for international operations
- Base currency designation
- Support for multiple currencies

### Warehouse Management
- Bin locations and stock tracking
- Warehouse-specific inventory levels
- Manager assignment and location management

### Serial/Batch Tracking
- Serial number tracking for high-value items
- Batch/lot tracking with expiry dates
- Warranty tracking and recall management

### KDS Integration
- Real-time kitchen order routing
- Preparation station management
- Priority levels (NORMAL, HIGH, URGENT)
- Complete status workflow: PENDING → IN_PROGRESS → READY → SERVED

### Delivery Tracking
- GPS coordinates for location tracking
- Driver availability management
- Support for pickup, delivery, and third-party options
- Delivery fee and distance calculation

### Loyalty System
- Points-based programs with tiers
- Tier-based benefits and discounts
- Points earn/redeem history
- Points expiration support

### Gift Cards
- Full lifecycle management (issue, load, redeem, refund)
- PIN protection and expiry dates
- Transaction history tracking

### Return Processing
- Approval workflow with condition tracking
- Multiple refund methods
- Restock fee support
- Condition tracking (NEW, OPENED, USED, DAMAGED)

## Technology Stack

- **Database**: MySQL/MariaDB
- **Engine**: InnoDB for transaction support
- **Charset**: utf8mb4 for full Unicode support
- **Collation**: utf8mb4_unicode_ci

## Table Modules

### Core Tables
- `roles` - User roles and permissions
- `permissions` - System permissions
- `role_permissions` - Role-permission mapping
- `users` - User accounts
- `branches` - Branch/store locations
- `pos_terminals` - POS terminal devices

### Product & Inventory Tables
- `product_categories` - Product categorization
- `units` - Unit of measurement definitions
- `unit_conversions` - Unit conversion relationships
- `products` - Product catalog
- `product_variants` - Product variants (size, color, etc.)
- `product_addons` - Product add-ons/modifiers
- `product_addon_groups` - Add-on groupings
- `recipes` - Product recipes/ingredients
- `recipe_ingredients` - Recipe ingredient details
- `production` - Production orders
- `production_ingredients` - Production ingredient usage
- `product_stocks` - Inventory stock levels
- `stock_movements` - Stock movement history
- `suppliers` - Supplier information
- `purchase_orders` - Purchase orders
- `purchase_order_items` - Purchase order line items
- `product_serials` - Serial number tracking
- `product_batches` - Batch/lot tracking

### Sales & Payment Tables
- `sales` - Sales transactions
- `sale_items` - Sale line items
- `sale_item_addons` - Sale item add-ons
- `sales_consumption` - Ingredient consumption tracking
- `payment_methods` - Payment method definitions
- `sale_payments` - Sale payment records
- `discounts` - Discount definitions
- `taxes` - Tax definitions
- `sale_refunds` - Sale refunds
- `sale_voids` - Sale voids
- `receivables` - Customer receivables
- `receivable_payments` - Receivable payments

### Delivery Tables
- `delivery_addresses` - Customer delivery addresses
- `delivery_drivers` - Driver information
- `delivery_orders` - Delivery orders
- `delivery_tracking` - Delivery tracking history

### Dine-In Tables
- `tables` - Table/seat management
- `reservations` - Table reservations

### KDS Tables
- `kitchen_stations` - Preparation stations
- `kitchen_station_products` - Product-station mapping
- `kitchen_orders` - Kitchen orders
- `kitchen_order_items` - Kitchen order items
- `kitchen_order_item_addons` - Kitchen item add-ons

### Customer Tables
- `customers` - Customer profiles
- `loyalty_programs` - Loyalty program configuration
- `loyalty_tiers` - Loyalty tier levels
- `customer_loyalty` - Customer loyalty membership
- `loyalty_transactions` - Loyalty point transactions
- `gift_cards` - Gift card management
- `gift_card_transactions` - Gift card transactions
- `customer_tax_exemptions` - Tax exemption certificates
- `customer_feedback` - Customer feedback and ratings
- `customer_groups` - Customer segments
- `customer_group_memberships` - Group memberships

### Marketing Tables
- `promotions` - Marketing campaigns
- `notification_templates` - Notification templates

### Employee Tables
- `employee_schedules` - Staff schedules
- `staff_commissions` - Commission tracking

### Advanced Feature Tables
- `transfer_orders` - Inter-branch transfers
- `transfer_order_items` - Transfer line items
- `warehouses` - Warehouse locations
- `warehouse_stocks` - Warehouse inventory
- `currencies` - Currency definitions
- `appointments` - Service appointments
- `return_orders` - Return orders
- `return_order_items` - Return line items
- `supplier_performance` - Supplier metrics
- `quotations` - Price quotations
- `quotation_items` - Quotation line items
- `api_integrations` - API connections
- `api_logs` - API request logs

### Developer Tool Tables
- `branding` - Company branding
- `schema_migrations` - Database version tracking
- `app_versions` - Application versions
- `licenses` - License management

### Cash Management Tables
- `cash_drawers` - Cash drawer assignments
- `cash_drawer_sessions` - Cash drawer sessions

### Tax Compliance Tables
- `tax_compliance_settings` - Tax compliance settings
- `receipt_series` - Receipt number series
- `shift_readings` - End-of-day shift readings

### System Tables
- `system_settings` - System configuration
- `number_sequences` - Auto-incrementing sequences
- `expenses` - Expense tracking
- `notifications` - System notifications
- `audit_logs` - Audit trail
- `error_logs` - Error logging

### Reporting Tables
- `daily_sales_summary` - Daily sales aggregation
- `product_performance` - Product performance metrics
- `inventory_valuation` - Inventory value tracking

### Price Management Tables
- `price_history` - Price change history

## Sample Data

The schema includes sample data for:
- Default roles (Super Admin, Manager, Cashier, etc.)
- Default permissions
- Default payment methods (Cash, Credit Card, Debit Card, Digital Wallet, Bank Transfer, Check, Gift Card)
- Default units (Piece, Case, Box, Kilogram, Gram, Bag, Liter, Milliliter, Bottle)
- Sample unit conversions
- Default system settings
- Default branding
- Initial schema migration record
- Initial application version
- Default notification templates

## Reporting Views

The schema includes several pre-built views for common reporting:
- `v_current_stock` - Current stock levels with valuation
- `v_low_stock_alert` - Low stock alerts
- `v_daily_sales_by_branch` - Daily sales by branch
- `v_product_performance` - Product performance metrics
- `v_inventory_valuation` - Inventory valuation report

## Performance Optimization

The schema includes performance optimization notes covering:
- Index strategy
- Foreign key constraints
- Time-based queries
- Composite indexes
- Full-text search considerations
- Partitioning recommendations
- Query optimization tips
- Caching strategies
- Business type versatility
- Developer and branding features
- Delivery support
- Dine-in operations
- Loyalty and rewards
- Gift cards
- Staff commissions
- Promotions and campaigns
- Employee management
- Advanced inventory tracking
- Inter-branch operations
- Tax compliance
- Customer engagement
- Price management
- Customer segmentation
- Quotations and estimates
- Third-party integrations
- Kitchen display system
- Return management
- Supplier performance
- Warehouse management
- Multi-currency support
- Service appointments

## Production Deployment

### Cloud Database Providers

For online production deployment, consider these managed database services:

#### AWS (Amazon Web Services)
- **Amazon RDS for MySQL** - Fully managed MySQL service
- **Amazon Aurora MySQL** - Cloud-native MySQL-compatible database
- Features: Automatic backups, read replicas, multi-AZ deployment
- Pricing: ~$0.04-$0.50+ per hour depending on instance size

#### Google Cloud Platform
- **Cloud SQL for MySQL** - Fully managed MySQL database
- Features: Automatic backups, high availability, read replicas
- Pricing: ~$0.04-$0.50+ per hour

#### Microsoft Azure
- **Azure Database for MySQL** - Fully managed MySQL service
- Features: Built-in high availability, automatic backups
- Pricing: ~$0.04-$0.50+ per hour

#### DigitalOcean
- **Managed Databases for MySQL** - Simple, affordable managed MySQL
- Features: Automatic backups, one-click failover
- Pricing: ~$15-$200+ per month

#### PlanetScale
- **Serverless MySQL** - Scalable, forkable MySQL database
- Features: Branching, sharding, edge caching
- Pricing: Free tier available, then $29+ per month

#### Other Options
- **Heroku Postgres** (can use MySQL via add-ons)
- **Railway** - Simple deployment with MySQL
- **Render** - Managed MySQL with easy deployment

### Recommended Configuration for Production

#### Minimum Requirements
- **CPU**: 2 vCPUs
- **RAM**: 4 GB
- **Storage**: 50 GB SSD (with auto-scaling)
- **High Availability**: Multi-AZ or replica setup
- **Backups**: Daily automated backups with 7-30 day retention

#### Recommended for Medium Traffic
- **CPU**: 4 vCPUs
- **RAM**: 8 GB
- **Storage**: 100 GB SSD
- **Read Replicas**: 1-2 for read-heavy workloads
- **Connection Pooling**: Enabled

#### Recommended for High Traffic
- **CPU**: 8+ vCPUs
- **RAM**: 16+ GB
- **Storage**: 200+ GB SSD with auto-scaling
- **Read Replicas**: 3+ for scaling reads
- **Connection Pooling**: Required
- **Caching Layer**: Redis for frequent queries

### Security Considerations

#### Essential Security Measures
1. **SSL/TLS Encryption** - Enforce encrypted connections
2. **VPC/Private Network** - Database in private subnet only
3. **Firewall Rules** - Whitelist application IPs only
4. **Strong Authentication** - Complex passwords, no root access
5. **Least Privilege** - Separate users for read/write operations
6. **Regular Updates** - Keep database engine updated
7. **Audit Logging** - Enable query and access logging

#### Connection Security
```csharp
// Example secure connection string
"Server=db.example.com;Database=enterprisepos_db;User=app_user;Password=xxx;SslMode=Required;AllowPublicKeyRetrieval=True;"
```

### Backup Strategy

#### Backup Types
- **Automated Backups** - Daily full backups
- **Point-in-Time Recovery** - Restore to any point in time
- **Manual Snapshots** - Before major changes
- **Cross-Region Replication** - For disaster recovery

#### Retention Policy
- Daily backups: 7-30 days
- Weekly backups: 4-12 weeks
- Monthly backups: 12 months
- Off-site storage: Required for critical data

### Performance Optimization

#### Database Configuration
- **InnoDB Buffer Pool**: 70-80% of RAM
- **Query Cache**: Enable for read-heavy workloads
- **Connection Pooling**: Use connection pooling in application
- **Index Optimization**: Monitor slow query log
- **Partitioning**: Consider for large tables (sales, stock_movements)

#### Caching Strategy
- **Redis** - Cache frequently accessed data (products, customers)
- **Application Cache** - In-memory cache for configuration
- **CDN** - For static assets only

### Monitoring & Alerting

#### Key Metrics to Monitor
- CPU utilization
- Memory usage
- Disk I/O and storage
- Connection count
- Query performance (slow queries)
- Replication lag (if using replicas)

#### Recommended Tools
- **AWS CloudWatch** / **Azure Monitor** / **GCP Cloud Monitoring**
- **Prometheus + Grafana** - Open-source monitoring
- **Datadog** - Comprehensive monitoring
- **New Relic** - Application and database monitoring

### Scaling Strategy

#### Vertical Scaling
- Upgrade instance size for more CPU/RAM
- Easier to implement
- Limited by max instance size
- Potential downtime during upgrade

#### Horizontal Scaling
- Add read replicas for read operations
- Write operations go to primary
- Better for high traffic
- More complex to implement

#### Database Sharding
- Split data across multiple databases
- For very large datasets
- Complex to implement
- Consider only at scale

### Cost Optimization

#### Cost-Saving Tips
1. **Right-size instances** - Start with what you need, scale up as needed
2. **Reserved instances** - Commit to 1-3 years for discounts (30-50% savings)
3. **Spot instances** - For non-critical workloads (up to 90% savings)
4. **Storage optimization** - Compress backups, delete old logs
5. **Read replicas** - Use only when needed, scale down during off-hours

#### Estimated Monthly Costs (AWS RDS MySQL)
- **Small (db.t3.micro)**: ~$15/month
- **Medium (db.t3.medium)**: ~$30/month
- **Large (db.t3.large)**: ~$60/month
- **Production (db.m5.large)**: ~$120/month
- **High Performance (db.m5.2xlarge)**: ~$240/month

### Deployment Checklist

#### Pre-Deployment
- [ ] Review and optimize schema indexes
- [ ] Set up VPC and security groups
- [ ] Configure firewall rules
- [ ] Create database users with appropriate permissions
- [ ] Test connection from application
- [ ] Set up automated backups
- [ ] Configure monitoring and alerts

#### Deployment
- [ ] Execute schema migration
- [ ] Load initial data (if needed)
- [ ] Test all critical operations
- [ ] Verify backup and restore process
- [ ] Update application connection strings
- [ ] Enable SSL/TLS
- [ ] Run performance benchmarks

#### Post-Deployment
- [ ] Monitor for 24-48 hours
- [ ] Review slow query log
- [ ] Optimize queries as needed
- [ ] Set up regular maintenance windows
- [ ] Document disaster recovery process
- [ ] Train team on monitoring tools

### Migration from Local to Production

#### Steps
1. **Export local database**
   ```bash
   mysqldump -u root -p enterprisepos_db > backup.sql
   ```

2. **Import to production**
   ```bash
   mysql -h production-host -u admin -p enterprisepos_db < backup.sql
   ```

3. **Update application configuration**
   - Change connection string
   - Update credentials
   - Enable SSL

4. **Verify data integrity**
   - Check record counts
   - Test critical queries
   - Verify relationships

### Disaster Recovery Plan

#### Recovery Procedures
1. **Database Failure**
   - Failover to read replica (if available)
   - Restore from latest backup
   - Replay transaction logs if using point-in-time recovery

2. **Data Corruption**
   - Identify corruption time
   - Restore to point before corruption
   - Reapply transactions after corruption point

3. **Complete Outage**
   - Restore from off-site backup
   - Spin up new database instance
   - Update DNS/application configuration

#### Recovery Time Objectives (RTO/RPO)
- **RTO (Recovery Time Objective)**: Target < 1 hour
- **RPO (Recovery Point Objective)**: Target < 15 minutes data loss
