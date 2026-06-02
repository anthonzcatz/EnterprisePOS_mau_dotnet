-- EnterprisePOS Database Schema
-- MySQL 8.0+ Compatible
-- Naming Convention: snake_case for tables and columns
-- Index Naming: idx_table_column or idx_table_unique_columns
-- Foreign Key Naming: fk_table_column_referenced_table_column

-- Create Database
CREATE DATABASE IF NOT EXISTS `enterprisepos_db` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `enterprisepos_db`;

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ============================================
-- USERS & AUTHENTICATION
-- ============================================

CREATE TABLE `roles` (
  `role_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `description` VARCHAR(255),
  `is_system` BOOLEAN DEFAULT FALSE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`role_id`),
  UNIQUE KEY `idx_roles_name_unique` (`name`),
  KEY `idx_roles_is_system` (`is_system`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='User roles for access control';

CREATE TABLE `permissions` (
  `permission_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `description` VARCHAR(255),
  `module` VARCHAR(50),
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`permission_id`),
  UNIQUE KEY `idx_permissions_name_unique` (`name`),
  KEY `idx_permissions_module` (`module`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='System permissions for access control';

CREATE TABLE `role_permissions` (
  `role_id` INT UNSIGNED NOT NULL,
  `permission_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`role_id`, `permission_id`),
  KEY `idx_role_permissions_role_id` (`role_id`),
  KEY `idx_role_permissions_permission_id` (`permission_id`),
  CONSTRAINT `fk_role_permissions_role_id_roles_role_id` 
    FOREIGN KEY (`role_id`) REFERENCES `roles`(`role_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_role_permissions_permission_id_permissions_permission_id` 
    FOREIGN KEY (`permission_id`) REFERENCES `permissions`(`permission_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Mapping between roles and permissions';

CREATE TABLE `users` (
  `user_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(50) NOT NULL,
  `email` VARCHAR(100) NOT NULL,
  `password_hash` VARCHAR(255) NOT NULL,
  `full_name` VARCHAR(100),
  `role_id` INT UNSIGNED,
  `branch_id` INT UNSIGNED,
  `image_url` VARCHAR(255),
  `is_active` BOOLEAN DEFAULT TRUE,
  `last_login_at` TIMESTAMP NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `idx_users_username_unique` (`username`),
  UNIQUE KEY `idx_users_email_unique` (`email`),
  KEY `idx_users_role_id` (`role_id`),
  KEY `idx_users_branch_id` (`branch_id`),
  KEY `idx_users_is_active` (`is_active`),
  CONSTRAINT `fk_users_role_id_roles_role_id` 
    FOREIGN KEY (`role_id`) REFERENCES `roles`(`role_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_users_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='User accounts with authentication and role assignments';

-- ============================================
-- ORGANIZATION
-- ============================================

CREATE TABLE `branches` (
  `branch_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `code` VARCHAR(20) NOT NULL,
  `address` TEXT,
  `phone` VARCHAR(20),
  `email` VARCHAR(100),
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`branch_id`),
  UNIQUE KEY `idx_branches_code_unique` (`code`),
  KEY `idx_branches_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Business branches/stores';

CREATE TABLE `pos_terminals` (
  `terminal_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `name` VARCHAR(50) NOT NULL,
  `code` VARCHAR(20) NOT NULL,
  `location` VARCHAR(100),
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`terminal_id`),
  UNIQUE KEY `idx_pos_terminals_code_unique` (`code`),
  KEY `idx_pos_terminals_branch_id` (`branch_id`),
  KEY `idx_pos_terminals_is_active` (`is_active`),
  CONSTRAINT `fk_pos_terminals_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='POS terminals per branch';

-- ============================================
-- CUSTOMERS
-- ============================================

CREATE TABLE `customers` (
  `customer_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(20) NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  `phone` VARCHAR(20),
  `email` VARCHAR(100),
  `address` TEXT,
  `loyalty_points` INT DEFAULT 0,
  `credit_limit` DECIMAL(10,2) DEFAULT 0.00,
  `current_balance` DECIMAL(10,2) DEFAULT 0.00,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`customer_id`),
  UNIQUE KEY `idx_customers_code_unique` (`code`),
  KEY `idx_customers_name` (`name`),
  KEY `idx_customers_phone` (`phone`),
  KEY `idx_customers_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Customer information with loyalty and credit tracking';

-- ============================================
-- PRODUCTS
-- ============================================

CREATE TABLE `product_categories` (
  `category_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `parent_id` INT UNSIGNED,
  `description` VARCHAR(255),
  `sort_order` INT DEFAULT 0,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`category_id`),
  KEY `idx_product_categories_parent_id` (`parent_id`),
  KEY `idx_product_categories_is_active` (`is_active`),
  KEY `idx_product_categories_sort_order` (`sort_order`),
  CONSTRAINT `fk_product_categories_parent_id_product_categories_category_id` 
    FOREIGN KEY (`parent_id`) REFERENCES `product_categories`(`category_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Product categories with hierarchical support';

CREATE TABLE `units` (
  `unit_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(20) NOT NULL,
  `abbreviation` VARCHAR(10) NOT NULL,
  `base_unit_id` INT UNSIGNED,
  `conversion_factor` DECIMAL(10,4) DEFAULT 1.0000,
  `is_base` BOOLEAN DEFAULT FALSE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`unit_id`),
  UNIQUE KEY `idx_units_abbreviation_unique` (`abbreviation`),
  KEY `idx_units_base_unit_id` (`base_unit_id`),
  CONSTRAINT `fk_units_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Measurement units for products with conversion support';

CREATE TABLE `unit_conversions` (
  `conversion_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `from_unit_id` INT UNSIGNED NOT NULL,
  `to_unit_id` INT UNSIGNED NOT NULL,
  `conversion_factor` DECIMAL(10,4) NOT NULL,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`conversion_id`),
  UNIQUE KEY `idx_unit_conversions_from_to_unique` (`from_unit_id`, `to_unit_id`),
  KEY `idx_unit_conversions_from_unit_id` (`from_unit_id`),
  KEY `idx_unit_conversions_to_unit_id` (`to_unit_id`),
  KEY `idx_unit_conversions_is_active` (`is_active`),
  CONSTRAINT `fk_unit_conversions_from_unit_id_units_unit_id` 
    FOREIGN KEY (`from_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_unit_conversions_to_unit_id_units_unit_id` 
    FOREIGN KEY (`to_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Unit conversion factors for complex relationships';

CREATE TABLE `products` (
  `product_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(20) NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  `description` TEXT,
  `category_id` INT UNSIGNED,
  `unit_id` INT UNSIGNED NOT NULL,
  `cost_price` DECIMAL(10,2) NOT NULL,
  `selling_price` DECIMAL(10,2) NOT NULL,
  `image_url` VARCHAR(255),
  `is_active` BOOLEAN DEFAULT TRUE,
  `is_taxable` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`product_id`),
  UNIQUE KEY `idx_products_code_unique` (`code`),
  KEY `idx_products_name` (`name`),
  KEY `idx_products_category_id` (`category_id`),
  KEY `idx_products_unit_id` (`unit_id`),
  KEY `idx_products_is_active` (`is_active`),
  KEY `idx_products_is_taxable` (`is_taxable`),
  CONSTRAINT `fk_products_category_id_product_categories_category_id` 
    FOREIGN KEY (`category_id`) REFERENCES `product_categories`(`category_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_products_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Product catalog with pricing and tax settings';

CREATE TABLE `product_variants` (
  `variant_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `product_id` INT UNSIGNED NOT NULL,
  `name` VARCHAR(50) NOT NULL,
  `sku` VARCHAR(50),
  `unit_id` INT UNSIGNED,
  `conversion_factor` DECIMAL(10,4) DEFAULT 1.0000,
  `price_adjustment` DECIMAL(10,2) DEFAULT 0.00,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`variant_id`),
  KEY `idx_product_variants_product_id` (`product_id`),
  KEY `idx_product_variants_sku` (`sku`),
  KEY `idx_product_variants_unit_id` (`unit_id`),
  KEY `idx_product_variants_is_active` (`is_active`),
  CONSTRAINT `fk_product_variants_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_product_variants_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Product variants (size, color, etc.) with unit conversion support';

CREATE TABLE `product_addons` (
  `addon_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `unit_id` INT UNSIGNED,
  `default_quantity` DECIMAL(10,4) DEFAULT 1.0000,
  `price` DECIMAL(10,2) NOT NULL,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`addon_id`),
  KEY `idx_product_addons_name` (`name`),
  KEY `idx_product_addons_unit_id` (`unit_id`),
  KEY `idx_product_addons_is_active` (`is_active`),
  CONSTRAINT `fk_product_addons_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Product add-ons/modifiers with unit support';

CREATE TABLE `product_addon_groups` (
  `addon_group_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `product_id` INT UNSIGNED NOT NULL,
  `addon_id` INT UNSIGNED NOT NULL,
  `is_required` BOOLEAN DEFAULT FALSE,
  `max_quantity` INT DEFAULT 1,
  PRIMARY KEY (`addon_group_id`),
  KEY `idx_product_addon_groups_product_id` (`product_id`),
  KEY `idx_product_addon_groups_addon_id` (`addon_id`),
  CONSTRAINT `fk_product_addon_groups_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_product_addon_groups_addon_id_product_addons_addon_id` 
    FOREIGN KEY (`addon_id`) REFERENCES `product_addons`(`addon_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Mapping of add-ons to products with quantity rules';

CREATE TABLE `recipes` (
  `recipe_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `product_id` INT UNSIGNED NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`recipe_id`),
  KEY `idx_recipes_product_id` (`product_id`),
  KEY `idx_recipes_is_active` (`is_active`),
  CONSTRAINT `fk_recipes_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Recipes for products with ingredient consumption tracking';

CREATE TABLE `recipe_ingredients` (
  `recipe_ingredient_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `recipe_id` INT UNSIGNED NOT NULL,
  `ingredient_product_id` INT UNSIGNED NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity` DECIMAL(10,4) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,4) NOT NULL,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`recipe_ingredient_id`),
  KEY `idx_recipe_ingredients_recipe_id` (`recipe_id`),
  KEY `idx_recipe_ingredients_ingredient_product_id` (`ingredient_product_id`),
  KEY `idx_recipe_ingredients_unit_id` (`unit_id`),
  KEY `idx_recipe_ingredients_base_unit_id` (`base_unit_id`),
  CONSTRAINT `fk_recipe_ingredients_recipe_id_recipes_recipe_id` 
    FOREIGN KEY (`recipe_id`) REFERENCES `recipes`(`recipe_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_recipe_ingredients_ingredient_product_id_products_product_id` 
    FOREIGN KEY (`ingredient_product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_recipe_ingredients_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_recipe_ingredients_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Ingredients in recipes with unit conversion support';

CREATE TABLE `production` (
  `production_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `recipe_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity_produced` DECIMAL(10,2) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity_produced` DECIMAL(10,2) NOT NULL,
  `status` ENUM('PENDING', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED') DEFAULT 'PENDING',
  `production_date` DATETIME NOT NULL,
  `notes` TEXT,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`production_id`),
  KEY `idx_production_recipe_id` (`recipe_id`),
  KEY `idx_production_branch_id` (`branch_id`),
  KEY `idx_production_product_id` (`product_id`),
  KEY `idx_production_status` (`status`),
  KEY `idx_production_production_date` (`production_date`),
  KEY `idx_production_created_by` (`created_by`),
  CONSTRAINT `fk_production_recipe_id_recipes_recipe_id` 
    FOREIGN KEY (`recipe_id`) REFERENCES `recipes`(`recipe_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_production_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_production_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_production_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_production_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_production_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Production records for manufacturing products from ingredients';

CREATE TABLE `production_ingredients` (
  `production_ingredient_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `production_id` INT UNSIGNED NOT NULL,
  `ingredient_product_id` INT UNSIGNED NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity_used` DECIMAL(10,4) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity_used` DECIMAL(10,4) NOT NULL,
  PRIMARY KEY (`production_ingredient_id`),
  KEY `idx_production_ingredients_production_id` (`production_id`),
  KEY `idx_production_ingredients_ingredient_product_id` (`ingredient_product_id`),
  KEY `idx_production_ingredients_unit_id` (`unit_id`),
  KEY `idx_production_ingredients_base_unit_id` (`base_unit_id`),
  CONSTRAINT `fk_production_ingredients_production_id_production_production_id` 
    FOREIGN KEY (`production_id`) REFERENCES `production`(`production_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_production_ingredients_ingredient_product_id_products_product_id` 
    FOREIGN KEY (`ingredient_product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_production_ingredients_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_production_ingredients_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Actual ingredient usage in production with unit conversion';

-- ============================================
-- INVENTORY
-- ============================================

CREATE TABLE `product_stocks` (
  `stock_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `product_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  `display_unit_id` INT UNSIGNED NOT NULL,
  `display_quantity` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  `reorder_level` DECIMAL(10,2) DEFAULT 10.00,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`stock_id`),
  UNIQUE KEY `idx_product_stocks_product_branch_unique` (`product_id`, `branch_id`),
  KEY `idx_product_stocks_product_id` (`product_id`),
  KEY `idx_product_stocks_branch_id` (`branch_id`),
  KEY `idx_product_stocks_base_unit_id` (`base_unit_id`),
  KEY `idx_product_stocks_display_unit_id` (`display_unit_id`),
  CONSTRAINT `fk_product_stocks_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_product_stocks_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_product_stocks_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_product_stocks_display_unit_id_units_unit_id` 
    FOREIGN KEY (`display_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Inventory levels per product and branch with unit conversion support';

CREATE TABLE `stock_movements` (
  `movement_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `product_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `movement_type` ENUM('IN', 'OUT', 'TRANSFER', 'ADJUSTMENT') NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity` DECIMAL(10,2) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,2) NOT NULL,
  `reference_type` VARCHAR(50),
  `reference_id` INT UNSIGNED,
  `notes` TEXT,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`movement_id`),
  KEY `idx_stock_movements_product_id` (`product_id`),
  KEY `idx_stock_movements_branch_id` (`branch_id`),
  KEY `idx_stock_movements_movement_type` (`movement_type`),
  KEY `idx_stock_movements_unit_id` (`unit_id`),
  KEY `idx_stock_movements_created_at` (`created_at`),
  KEY `idx_stock_movements_reference` (`reference_type`, `reference_id`),
  CONSTRAINT `fk_stock_movements_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_stock_movements_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_stock_movements_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_stock_movements_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_stock_movements_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Stock movement history with unit conversion tracking';

CREATE TABLE `suppliers` (
  `supplier_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `contact_person` VARCHAR(100),
  `phone` VARCHAR(20),
  `email` VARCHAR(100),
  `address` TEXT,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`supplier_id`),
  KEY `idx_suppliers_name` (`name`),
  KEY `idx_suppliers_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Supplier information for purchase orders';

CREATE TABLE `purchase_orders` (
  `purchase_order_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `supplier_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `order_number` VARCHAR(50) NOT NULL,
  `order_date` DATE NOT NULL,
  `expected_date` DATE,
  `status` ENUM('PENDING', 'RECEIVED', 'CANCELLED') DEFAULT 'PENDING',
  `total_amount` DECIMAL(12,2) DEFAULT 0.00,
  `notes` TEXT,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`purchase_order_id`),
  UNIQUE KEY `idx_purchase_orders_order_number_unique` (`order_number`),
  KEY `idx_purchase_orders_supplier_id` (`supplier_id`),
  KEY `idx_purchase_orders_branch_id` (`branch_id`),
  KEY `idx_purchase_orders_status` (`status`),
  KEY `idx_purchase_orders_order_date` (`order_date`),
  CONSTRAINT `fk_purchase_orders_supplier_id_suppliers_supplier_id` 
    FOREIGN KEY (`supplier_id`) REFERENCES `suppliers`(`supplier_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_purchase_orders_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_purchase_orders_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Purchase orders from suppliers';

CREATE TABLE `purchase_order_items` (
  `purchase_order_item_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `purchase_order_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity` DECIMAL(10,2) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,2) NOT NULL,
  `unit_price` DECIMAL(10,2) NOT NULL,
  `total_price` DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (`purchase_order_item_id`),
  KEY `idx_purchase_order_items_purchase_order_id` (`purchase_order_id`),
  KEY `idx_purchase_order_items_product_id` (`product_id`),
  KEY `idx_purchase_order_items_unit_id` (`unit_id`),
  KEY `idx_purchase_order_items_base_unit_id` (`base_unit_id`),
  CONSTRAINT `fk_purchase_order_items_purchase_order_id_purchase_orders_purchase_order_id` 
    FOREIGN KEY (`purchase_order_id`) REFERENCES `purchase_orders`(`purchase_order_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_purchase_order_items_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_purchase_order_items_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_purchase_order_items_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Items in purchase orders with unit conversion support';

-- ============================================
-- SALES
-- ============================================

CREATE TABLE `sales` (
  `sale_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `terminal_id` INT UNSIGNED NOT NULL,
  `customer_id` INT UNSIGNED,
  `sale_number` VARCHAR(50) NOT NULL,
  `sale_date` DATETIME NOT NULL,
  `subtotal` DECIMAL(12,2) NOT NULL,
  `discount_amount` DECIMAL(12,2) DEFAULT 0.00,
  `tax_amount` DECIMAL(12,2) DEFAULT 0.00,
  `total_amount` DECIMAL(12,2) NOT NULL,
  `paid_amount` DECIMAL(12,2) DEFAULT 0.00,
  `change_amount` DECIMAL(12,2) DEFAULT 0.00,
  `status` ENUM('PENDING', 'COMPLETED', 'CANCELLED', 'REFUNDED') DEFAULT 'PENDING',
  `notes` TEXT,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`sale_id`),
  UNIQUE KEY `idx_sales_sale_number_unique` (`sale_number`),
  KEY `idx_sales_branch_id` (`branch_id`),
  KEY `idx_sales_terminal_id` (`terminal_id`),
  KEY `idx_sales_customer_id` (`customer_id`),
  KEY `idx_sales_status` (`status`),
  KEY `idx_sales_sale_date` (`sale_date`),
  KEY `idx_sales_created_by` (`created_by`),
  CONSTRAINT `fk_sales_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sales_terminal_id_pos_terminals_terminal_id` 
    FOREIGN KEY (`terminal_id`) REFERENCES `pos_terminals`(`terminal_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sales_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_sales_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Sales transactions with payment tracking';

CREATE TABLE `sale_items` (
  `sale_item_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sale_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `product_variant_id` INT UNSIGNED,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity` DECIMAL(10,2) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,2) NOT NULL,
  `unit_price` DECIMAL(10,2) NOT NULL,
  `discount_amount` DECIMAL(10,2) DEFAULT 0.00,
  `tax_amount` DECIMAL(10,2) DEFAULT 0.00,
  `total_price` DECIMAL(10,2) NOT NULL,
  `notes` TEXT,
  PRIMARY KEY (`sale_item_id`),
  KEY `idx_sale_items_sale_id` (`sale_id`),
  KEY `idx_sale_items_product_id` (`product_id`),
  KEY `idx_sale_items_product_variant_id` (`product_variant_id`),
  KEY `idx_sale_items_unit_id` (`unit_id`),
  KEY `idx_sale_items_base_unit_id` (`base_unit_id`),
  CONSTRAINT `fk_sale_items_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_sale_items_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sale_items_product_variant_id_product_variants_variant_id` 
    FOREIGN KEY (`product_variant_id`) REFERENCES `product_variants`(`variant_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_sale_items_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sale_items_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Items in sales transactions with unit conversion support';

CREATE TABLE `sale_item_addons` (
  `sale_item_addon_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sale_item_id` INT UNSIGNED NOT NULL,
  `addon_id` INT UNSIGNED NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity` DECIMAL(10,4) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,4) NOT NULL,
  `price` DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (`sale_item_addon_id`),
  KEY `idx_sale_item_addons_sale_item_id` (`sale_item_id`),
  KEY `idx_sale_item_addons_addon_id` (`addon_id`),
  KEY `idx_sale_item_addons_unit_id` (`unit_id`),
  KEY `idx_sale_item_addons_base_unit_id` (`base_unit_id`),
  CONSTRAINT `fk_sale_item_addons_sale_item_id_sale_items_sale_item_id` 
    FOREIGN KEY (`sale_item_id`) REFERENCES `sale_items`(`sale_item_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_sale_item_addons_addon_id_product_addons_addon_id` 
    FOREIGN KEY (`addon_id`) REFERENCES `product_addons`(`addon_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sale_item_addons_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sale_item_addons_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Add-ons for sale items with unit conversion support';

CREATE TABLE `sales_consumption` (
  `consumption_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sale_id` INT UNSIGNED NOT NULL,
  `sale_item_id` INT UNSIGNED NOT NULL,
  `ingredient_product_id` INT UNSIGNED NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity_consumed` DECIMAL(10,4) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity_consumed` DECIMAL(10,4) NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `consumed_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`consumption_id`),
  KEY `idx_sales_consumption_sale_id` (`sale_id`),
  KEY `idx_sales_consumption_sale_item_id` (`sale_item_id`),
  KEY `idx_sales_consumption_ingredient_product_id` (`ingredient_product_id`),
  KEY `idx_sales_consumption_branch_id` (`branch_id`),
  KEY `idx_sales_consumption_consumed_at` (`consumed_at`),
  KEY `idx_sales_consumption_unit_id` (`unit_id`),
  CONSTRAINT `fk_sales_consumption_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_sales_consumption_sale_item_id_sale_items_sale_item_id` 
    FOREIGN KEY (`sale_item_id`) REFERENCES `sale_items`(`sale_item_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_sales_consumption_ingredient_product_id_products_product_id` 
    FOREIGN KEY (`ingredient_product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sales_consumption_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sales_consumption_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sales_consumption_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Automatic ingredient consumption tracking from sales';

-- ============================================
-- PAYMENTS
-- ============================================

CREATE TABLE `payment_methods` (
  `payment_method_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `code` VARCHAR(20) NOT NULL,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`payment_method_id`),
  UNIQUE KEY `idx_payment_methods_code_unique` (`code`),
  KEY `idx_payment_methods_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Payment method types (cash, card, digital wallets, etc.)';

CREATE TABLE `sale_payments` (
  `sale_payment_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sale_id` INT UNSIGNED NOT NULL,
  `payment_method_id` INT UNSIGNED NOT NULL,
  `amount` DECIMAL(12,2) NOT NULL,
  `reference_number` VARCHAR(100),
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`sale_payment_id`),
  KEY `idx_sale_payments_sale_id` (`sale_id`),
  KEY `idx_sale_payments_payment_method_id` (`payment_method_id`),
  KEY `idx_sale_payments_reference_number` (`reference_number`),
  CONSTRAINT `fk_sale_payments_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_sale_payments_payment_method_id_payment_methods_payment_method_id` 
    FOREIGN KEY (`payment_method_id`) REFERENCES `payment_methods`(`payment_method_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Payment records for sales';

-- ============================================
-- RECEIVABLES
-- ============================================

CREATE TABLE `receivables` (
  `receivable_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `customer_id` INT UNSIGNED NOT NULL,
  `sale_id` INT UNSIGNED NOT NULL,
  `amount` DECIMAL(12,2) NOT NULL,
  `balance` DECIMAL(12,2) NOT NULL,
  `due_date` DATE,
  `status` ENUM('PENDING', 'PARTIAL', 'PAID', 'OVERDUE') DEFAULT 'PENDING',
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`receivable_id`),
  KEY `idx_receivables_customer_id` (`customer_id`),
  KEY `idx_receivables_sale_id` (`sale_id`),
  KEY `idx_receivables_status` (`status`),
  KEY `idx_receivables_due_date` (`due_date`),
  CONSTRAINT `fk_receivables_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_receivables_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Customer receivables and credit tracking';

CREATE TABLE `receivable_payments` (
  `receivable_payment_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `receivable_id` INT UNSIGNED NOT NULL,
  `amount` DECIMAL(12,2) NOT NULL,
  `payment_date` DATE NOT NULL,
  `notes` TEXT,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`receivable_payment_id`),
  KEY `idx_receivable_payments_receivable_id` (`receivable_id`),
  KEY `idx_receivable_payments_payment_date` (`payment_date`),
  KEY `idx_receivable_payments_created_by` (`created_by`),
  CONSTRAINT `fk_receivable_payments_receivable_id_receivables_receivable_id` 
    FOREIGN KEY (`receivable_id`) REFERENCES `receivables`(`receivable_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_receivable_payments_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Payments for customer receivables';

-- ============================================
-- DELIVERY
-- ============================================

CREATE TABLE `delivery_addresses` (
  `delivery_address_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `customer_id` INT UNSIGNED NOT NULL,
  `address_label` VARCHAR(50),
  `recipient_name` VARCHAR(100) NOT NULL,
  `phone` VARCHAR(20) NOT NULL,
  `address_line1` VARCHAR(255) NOT NULL,
  `address_line2` VARCHAR(255),
  `city` VARCHAR(100) NOT NULL,
  `state_province` VARCHAR(100),
  `postal_code` VARCHAR(20),
  `country` VARCHAR(100) DEFAULT 'United States',
  `latitude` DECIMAL(10,8),
  `longitude` DECIMAL(11,8),
  `is_default` BOOLEAN DEFAULT FALSE,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`delivery_address_id`),
  KEY `idx_delivery_addresses_customer_id` (`customer_id`),
  KEY `idx_delivery_addresses_is_default` (`is_default`),
  KEY `idx_delivery_addresses_is_active` (`is_active`),
  CONSTRAINT `fk_delivery_addresses_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Customer delivery addresses';

CREATE TABLE `delivery_drivers` (
  `driver_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `user_id` INT UNSIGNED,
  `name` VARCHAR(100) NOT NULL,
  `phone` VARCHAR(20) NOT NULL,
  `license_number` VARCHAR(50),
  `vehicle_type` VARCHAR(50),
  `vehicle_plate` VARCHAR(20),
  `is_available` BOOLEAN DEFAULT TRUE,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`driver_id`),
  KEY `idx_delivery_drivers_branch_id` (`branch_id`),
  KEY `idx_delivery_drivers_user_id` (`user_id`),
  KEY `idx_delivery_drivers_is_available` (`is_available`),
  KEY `idx_delivery_drivers_is_active` (`is_active`),
  CONSTRAINT `fk_delivery_drivers_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_delivery_drivers_user_id_users_user_id` 
    FOREIGN KEY (`user_id`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Delivery driver information';

CREATE TABLE `delivery_orders` (
  `delivery_order_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sale_id` INT UNSIGNED NOT NULL,
  `delivery_address_id` INT UNSIGNED NOT NULL,
  `driver_id` INT UNSIGNED,
  `branch_id` INT UNSIGNED NOT NULL,
  `delivery_type` ENUM('PICKUP', 'DELIVERY', 'THIRD_PARTY') NOT NULL,
  `scheduled_date` DATETIME,
  `pickup_time` DATETIME,
  `delivery_time` DATETIME,
  `estimated_duration` INT,
  `delivery_fee` DECIMAL(10,2) DEFAULT 0.00,
  `distance_km` DECIMAL(10,2),
  `status` ENUM('PENDING', 'ASSIGNED', 'PICKED_UP', 'IN_TRANSIT', 'DELIVERED', 'CANCELLED', 'FAILED') DEFAULT 'PENDING',
  `notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`delivery_order_id`),
  UNIQUE KEY `idx_delivery_orders_sale_id_unique` (`sale_id`),
  KEY `idx_delivery_orders_delivery_address_id` (`delivery_address_id`),
  KEY `idx_delivery_orders_driver_id` (`driver_id`),
  KEY `idx_delivery_orders_branch_id` (`branch_id`),
  KEY `idx_delivery_orders_status` (`status`),
  KEY `idx_delivery_orders_scheduled_date` (`scheduled_date`),
  CONSTRAINT `fk_delivery_orders_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_delivery_orders_delivery_address_id_delivery_addresses_delivery_address_id` 
    FOREIGN KEY (`delivery_address_id`) REFERENCES `delivery_addresses`(`delivery_address_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_delivery_orders_driver_id_delivery_drivers_driver_id` 
    FOREIGN KEY (`driver_id`) REFERENCES `delivery_drivers`(`driver_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_delivery_orders_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Delivery order information';

CREATE TABLE `delivery_tracking` (
  `tracking_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `delivery_order_id` INT UNSIGNED NOT NULL,
  `status` ENUM('PENDING', 'ASSIGNED', 'PICKED_UP', 'IN_TRANSIT', 'DELIVERED', 'CANCELLED', 'FAILED') NOT NULL,
  `latitude` DECIMAL(10,8),
  `longitude` DECIMAL(11,8),
  `notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`tracking_id`),
  KEY `idx_delivery_tracking_delivery_order_id` (`delivery_order_id`),
  KEY `idx_delivery_tracking_status` (`status`),
  KEY `idx_delivery_tracking_created_at` (`created_at`),
  CONSTRAINT `fk_delivery_tracking_delivery_order_id_delivery_orders_delivery_order_id` 
    FOREIGN KEY (`delivery_order_id`) REFERENCES `delivery_orders`(`delivery_order_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Delivery tracking history';

-- ============================================
-- CASH DRAWER
-- ============================================

CREATE TABLE `cash_drawers` (
  `cash_drawer_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `terminal_id` INT UNSIGNED NOT NULL,
  `name` VARCHAR(50) NOT NULL,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`cash_drawer_id`),
  KEY `idx_cash_drawers_branch_id` (`branch_id`),
  KEY `idx_cash_drawers_terminal_id` (`terminal_id`),
  KEY `idx_cash_drawers_is_active` (`is_active`),
  CONSTRAINT `fk_cash_drawers_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_cash_drawers_terminal_id_pos_terminals_terminal_id` 
    FOREIGN KEY (`terminal_id`) REFERENCES `pos_terminals`(`terminal_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Cash drawer assignments per terminal';

CREATE TABLE `cash_drawer_sessions` (
  `session_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `cash_drawer_id` INT UNSIGNED NOT NULL,
  `user_id` INT UNSIGNED NOT NULL,
  `opening_amount` DECIMAL(12,2) DEFAULT 0.00,
  `closing_amount` DECIMAL(12,2),
  `expected_amount` DECIMAL(12,2),
  `variance` DECIMAL(12,2),
  `opened_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `closed_at` TIMESTAMP NULL,
  `notes` TEXT,
  PRIMARY KEY (`session_id`),
  KEY `idx_cash_drawer_sessions_cash_drawer_id` (`cash_drawer_id`),
  KEY `idx_cash_drawer_sessions_user_id` (`user_id`),
  KEY `idx_cash_drawer_sessions_opened_at` (`opened_at`),
  KEY `idx_cash_drawer_sessions_closed_at` (`closed_at`),
  CONSTRAINT `fk_cash_drawer_sessions_cash_drawer_id_cash_drawers_cash_drawer_id` 
    FOREIGN KEY (`cash_drawer_id`) REFERENCES `cash_drawers`(`cash_drawer_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_cash_drawer_sessions_user_id_users_user_id` 
    FOREIGN KEY (`user_id`) REFERENCES `users`(`user_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Cash drawer opening and closing sessions';

-- ============================================
-- DISCOUNTS & TAXES
-- ============================================

CREATE TABLE `discounts` (
  `discount_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `code` VARCHAR(20),
  `type` ENUM('PERCENTAGE', 'FIXED') NOT NULL,
  `value` DECIMAL(10,2) NOT NULL,
  `min_purchase_amount` DECIMAL(12,2) DEFAULT 0.00,
  `is_active` BOOLEAN DEFAULT TRUE,
  `valid_from` DATE,
  `valid_until` DATE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`discount_id`),
  KEY `idx_discounts_code` (`code`),
  KEY `idx_discounts_is_active` (`is_active`),
  KEY `idx_discounts_valid_period` (`valid_from`, `valid_until`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Discount rules and promotions';

CREATE TABLE `taxes` (
  `tax_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `rate` DECIMAL(5,2) NOT NULL,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`tax_id`),
  KEY `idx_taxes_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Tax rates for products and services';

-- ============================================
-- REFUNDS & VOID
-- ============================================

CREATE TABLE `sale_refunds` (
  `refund_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sale_id` INT UNSIGNED NOT NULL,
  `refund_amount` DECIMAL(12,2) NOT NULL,
  `reason` TEXT,
  `refunded_by` INT UNSIGNED,
  `refunded_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`refund_id`),
  KEY `idx_sale_refunds_sale_id` (`sale_id`),
  KEY `idx_sale_refunds_refunded_by` (`refunded_by`),
  KEY `idx_sale_refunds_refunded_at` (`refunded_at`),
  CONSTRAINT `fk_sale_refunds_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sale_refunds_refunded_by_users_user_id` 
    FOREIGN KEY (`refunded_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Sale refund records';

CREATE TABLE `sale_voids` (
  `void_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sale_id` INT UNSIGNED NOT NULL,
  `reason` TEXT,
  `voided_by` INT UNSIGNED,
  `voided_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`void_id`),
  KEY `idx_sale_voids_sale_id` (`sale_id`),
  KEY `idx_sale_voids_voided_by` (`voided_by`),
  KEY `idx_sale_voids_voided_at` (`voided_at`),
  CONSTRAINT `fk_sale_voids_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sale_voids_voided_by_users_user_id` 
    FOREIGN KEY (`voided_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Sale void records';

-- ============================================
-- EXPENSES
-- ============================================

CREATE TABLE `expenses` (
  `expense_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `category` VARCHAR(50) NOT NULL,
  `description` TEXT,
  `amount` DECIMAL(12,2) NOT NULL,
  `expense_date` DATE NOT NULL,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`expense_id`),
  KEY `idx_expenses_branch_id` (`branch_id`),
  KEY `idx_expenses_category` (`category`),
  KEY `idx_expenses_expense_date` (`expense_date`),
  KEY `idx_expenses_created_by` (`created_by`),
  CONSTRAINT `fk_expenses_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_expenses_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Business expense tracking';

-- ============================================
-- NOTIFICATIONS
-- ============================================

CREATE TABLE `notifications` (
  `notification_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT UNSIGNED NOT NULL,
  `title` VARCHAR(100) NOT NULL,
  `message` TEXT,
  `type` VARCHAR(20),
  `is_read` BOOLEAN DEFAULT FALSE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`notification_id`),
  KEY `idx_notifications_user_id` (`user_id`),
  KEY `idx_notifications_is_read` (`is_read`),
  KEY `idx_notifications_created_at` (`created_at`),
  CONSTRAINT `fk_notifications_user_id_users_user_id` 
    FOREIGN KEY (`user_id`) REFERENCES `users`(`user_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='User notifications and alerts';

-- ============================================
-- LOGGING
-- ============================================

CREATE TABLE `audit_logs` (
  `audit_log_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT UNSIGNED,
  `action` VARCHAR(50) NOT NULL,
  `entity_type` VARCHAR(50),
  `entity_id` INT UNSIGNED,
  `old_values` JSON,
  `new_values` JSON,
  `ip_address` VARCHAR(45),
  `user_agent` VARCHAR(255),
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`audit_log_id`),
  KEY `idx_audit_logs_user_id` (`user_id`),
  KEY `idx_audit_logs_entity` (`entity_type`, `entity_id`),
  KEY `idx_audit_logs_action` (`action`),
  KEY `idx_audit_logs_created_at` (`created_at`),
  CONSTRAINT `fk_audit_logs_user_id_users_user_id` 
    FOREIGN KEY (`user_id`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Audit trail for all data changes';

CREATE TABLE `error_logs` (
  `error_log_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT UNSIGNED,
  `error_message` TEXT NOT NULL,
  `stack_trace` TEXT,
  `request_url` VARCHAR(255),
  `request_method` VARCHAR(10),
  `ip_address` VARCHAR(45),
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`error_log_id`),
  KEY `idx_error_logs_user_id` (`user_id`),
  KEY `idx_error_logs_created_at` (`created_at`),
  CONSTRAINT `fk_error_logs_user_id_users_user_id` 
    FOREIGN KEY (`user_id`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Application error logging';

-- ============================================
-- REPORTING & ANALYTICS
-- ============================================

CREATE TABLE `daily_sales_summary` (
  `summary_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `terminal_id` INT UNSIGNED,
  `summary_date` DATE NOT NULL,
  `total_sales` INT DEFAULT 0,
  `gross_sales` DECIMAL(12,2) DEFAULT 0.00,
  `net_sales` DECIMAL(12,2) DEFAULT 0.00,
  `total_discount` DECIMAL(12,2) DEFAULT 0.00,
  `total_tax` DECIMAL(12,2) DEFAULT 0.00,
  `total_refund` DECIMAL(12,2) DEFAULT 0.00,
  `total_void` DECIMAL(12,2) DEFAULT 0.00,
  `cash_sales` DECIMAL(12,2) DEFAULT 0.00,
  `card_sales` DECIMAL(12,2) DEFAULT 0.00,
  `digital_wallet_sales` DECIMAL(12,2) DEFAULT 0.00,
  `bank_transfer_sales` DECIMAL(12,2) DEFAULT 0.00,
  `check_sales` DECIMAL(12,2) DEFAULT 0.00,
  `gift_card_sales` DECIMAL(12,2) DEFAULT 0.00,
  `other_sales` DECIMAL(12,2) DEFAULT 0.00,
  `total_cost` DECIMAL(12,2) DEFAULT 0.00,
  `gross_profit` DECIMAL(12,2) DEFAULT 0.00,
  `net_profit` DECIMAL(12,2) DEFAULT 0.00,
  `customer_count` INT DEFAULT 0,
  `average_transaction` DECIMAL(12,2) DEFAULT 0.00,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`summary_id`),
  UNIQUE KEY `idx_daily_sales_summary_branch_terminal_date_unique` (`branch_id`, `terminal_id`, `summary_date`),
  KEY `idx_daily_sales_summary_branch_id` (`branch_id`),
  KEY `idx_daily_sales_summary_terminal_id` (`terminal_id`),
  KEY `idx_daily_sales_summary_summary_date` (`summary_date`),
  CONSTRAINT `fk_daily_sales_summary_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_daily_sales_summary_terminal_id_pos_terminals_terminal_id` 
    FOREIGN KEY (`terminal_id`) REFERENCES `pos_terminals`(`terminal_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Daily sales summary for reporting';

CREATE TABLE `product_performance` (
  `performance_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `product_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `period_type` ENUM('DAILY', 'WEEKLY', 'MONTHLY', 'YEARLY') NOT NULL,
  `period_start` DATE NOT NULL,
  `period_end` DATE NOT NULL,
  `quantity_sold` DECIMAL(10,2) DEFAULT 0.00,
  `gross_sales` DECIMAL(12,2) DEFAULT 0.00,
  `discount_amount` DECIMAL(12,2) DEFAULT 0.00,
  `net_sales` DECIMAL(12,2) DEFAULT 0.00,
  `total_cost` DECIMAL(12,2) DEFAULT 0.00,
  `gross_profit` DECIMAL(12,2) DEFAULT 0.00,
  `profit_margin` DECIMAL(5,2) DEFAULT 0.00,
  `return_count` INT DEFAULT 0,
  `return_amount` DECIMAL(12,2) DEFAULT 0.00,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`performance_id`),
  UNIQUE KEY `idx_product_performance_product_branch_period_unique` (`product_id`, `branch_id`, `period_type`, `period_start`, `period_end`),
  KEY `idx_product_performance_product_id` (`product_id`),
  KEY `idx_product_performance_branch_id` (`branch_id`),
  KEY `idx_product_performance_period_type` (`period_type`),
  KEY `idx_product_performance_period_start` (`period_start`),
  KEY `idx_product_performance_period_end` (`period_end`),
  CONSTRAINT `fk_product_performance_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_product_performance_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Product performance analytics for profit analysis';

CREATE TABLE `inventory_valuation` (
  `valuation_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `valuation_date` DATE NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity_on_hand` DECIMAL(10,2) DEFAULT 0.00,
  `unit_cost` DECIMAL(10,2) DEFAULT 0.00,
  `total_value` DECIMAL(12,2) DEFAULT 0.00,
  `valuation_method` ENUM('FIFO', 'LIFO', 'WEIGHTED_AVERAGE') DEFAULT 'WEIGHTED_AVERAGE',
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`valuation_id`),
  UNIQUE KEY `idx_inventory_valuation_branch_product_date_unique` (`branch_id`, `product_id`, `valuation_date`),
  KEY `idx_inventory_valuation_branch_id` (`branch_id`),
  KEY `idx_inventory_valuation_product_id` (`product_id`),
  KEY `idx_inventory_valuation_valuation_date` (`valuation_date`),
  KEY `idx_inventory_valuation_unit_id` (`unit_id`),
  CONSTRAINT `fk_inventory_valuation_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_inventory_valuation_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_inventory_valuation_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Inventory valuation for financial reporting';

-- ============================================
-- TAX COMPLIANCE
-- ============================================

CREATE TABLE `tax_compliance_settings` (
  `tax_setting_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `tax_id_number` VARCHAR(20),
  `permit_number` VARCHAR(50),
  `serial_number` VARCHAR(50),
  `accreditation_start_date` DATE,
  `accreditation_end_date` DATE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`tax_setting_id`),
  KEY `idx_tax_compliance_settings_branch_id` (`branch_id`),
  CONSTRAINT `fk_tax_compliance_settings_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Tax compliance settings per branch (configurable for different regions)';

CREATE TABLE `receipt_series` (
  `receipt_series_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `terminal_id` INT UNSIGNED NOT NULL,
  `prefix` VARCHAR(10),
  `start_number` INT NOT NULL,
  `end_number` INT NOT NULL,
  `current_number` INT NOT NULL,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`receipt_series_id`),
  KEY `idx_receipt_series_branch_id` (`branch_id`),
  KEY `idx_receipt_series_terminal_id` (`terminal_id`),
  KEY `idx_receipt_series_is_active` (`is_active`),
  CONSTRAINT `fk_receipt_series_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_receipt_series_terminal_id_pos_terminals_terminal_id` 
    FOREIGN KEY (`terminal_id`) REFERENCES `pos_terminals`(`terminal_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Receipt number series per terminal';

CREATE TABLE `shift_readings` (
  `shift_reading_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `terminal_id` INT UNSIGNED NOT NULL,
  `cash_drawer_session_id` INT UNSIGNED NOT NULL,
  `reading_date` DATETIME NOT NULL,
  `gross_sales` DECIMAL(12,2) NOT NULL,
  `net_sales` DECIMAL(12,2) NOT NULL,
  `discounts` DECIMAL(12,2) DEFAULT 0.00,
  `taxes` DECIMAL(12,2) DEFAULT 0.00,
  `voids` DECIMAL(12,2) DEFAULT 0.00,
  `refunds` DECIMAL(12,2) DEFAULT 0.00,
  `cash_sales` DECIMAL(12,2) DEFAULT 0.00,
  `card_sales` DECIMAL(12,2) DEFAULT 0.00,
  `other_sales` DECIMAL(12,2) DEFAULT 0.00,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`shift_reading_id`),
  KEY `idx_shift_readings_branch_id` (`branch_id`),
  KEY `idx_shift_readings_terminal_id` (`terminal_id`),
  KEY `idx_shift_readings_cash_drawer_session_id` (`cash_drawer_session_id`),
  KEY `idx_shift_readings_reading_date` (`reading_date`),
  CONSTRAINT `fk_shift_readings_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_shift_readings_terminal_id_pos_terminals_terminal_id` 
    FOREIGN KEY (`terminal_id`) REFERENCES `pos_terminals`(`terminal_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_shift_readings_cash_drawer_session_id_cash_drawer_sessions_session_id` 
    FOREIGN KEY (`cash_drawer_session_id`) REFERENCES `cash_drawer_sessions`(`session_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_shift_readings_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='End-of-day shift readings for tax compliance and reporting';

-- ============================================
-- SYSTEM SETTINGS
-- ============================================

CREATE TABLE `system_settings` (
  `setting_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `key` VARCHAR(100) NOT NULL,
  `value` TEXT,
  `description` VARCHAR(255),
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`setting_id`),
  UNIQUE KEY `idx_system_settings_key_unique` (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='System-wide configuration settings';

CREATE TABLE `number_sequences` (
  `sequence_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `entity_type` VARCHAR(50) NOT NULL,
  `prefix` VARCHAR(20),
  `current_number` INT NOT NULL,
  `padding` INT DEFAULT 6,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`sequence_id`),
  UNIQUE KEY `idx_number_sequences_entity_type_unique` (`entity_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Auto-incrementing number sequences for documents';

-- ============================================
-- BRANDING & CUSTOMIZATION
-- ============================================

CREATE TABLE `branding` (
  `branding_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED,
  `company_name` VARCHAR(100),
  `logo_url` VARCHAR(255),
  `favicon_url` VARCHAR(255),
  `primary_color` VARCHAR(7) DEFAULT '#000000',
  `secondary_color` VARCHAR(7) DEFAULT '#FFFFFF',
  `accent_color` VARCHAR(7) DEFAULT '#0000FF',
  `background_color` VARCHAR(7) DEFAULT '#FFFFFF',
  `text_color` VARCHAR(7) DEFAULT '#000000',
  `theme` ENUM('LIGHT', 'DARK', 'CUSTOM') DEFAULT 'LIGHT',
  `custom_css` TEXT,
  `receipt_header` TEXT,
  `receipt_footer` TEXT,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`branding_id`),
  KEY `idx_branding_branch_id` (`branch_id`),
  KEY `idx_branding_is_active` (`is_active`),
  CONSTRAINT `fk_branding_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Company branding and UI customization';

-- ============================================
-- VERSION & MIGRATION TRACKING
-- ============================================

CREATE TABLE `schema_migrations` (
  `migration_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `version` VARCHAR(50) NOT NULL,
  `description` VARCHAR(255),
  `executed_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `execution_time_ms` INT,
  `status` ENUM('SUCCESS', 'FAILED', 'PENDING') DEFAULT 'SUCCESS',
  PRIMARY KEY (`migration_id`),
  UNIQUE KEY `idx_schema_migrations_version_unique` (`version`),
  KEY `idx_schema_migrations_executed_at` (`executed_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Database schema version and migration tracking';

CREATE TABLE `app_versions` (
  `version_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `version_number` VARCHAR(20) NOT NULL,
  `version_name` VARCHAR(100),
  `release_date` DATE,
  `release_notes` TEXT,
  `is_current` BOOLEAN DEFAULT FALSE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`version_id`),
  UNIQUE KEY `idx_app_versions_version_number_unique` (`version_number`),
  KEY `idx_app_versions_is_current` (`is_current`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Application version tracking';

-- ============================================
-- NOTIFICATION & EMAIL TEMPLATES
-- ============================================

CREATE TABLE `notification_templates` (
  `template_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `template_code` VARCHAR(50) NOT NULL,
  `template_name` VARCHAR(100) NOT NULL,
  `template_type` ENUM('EMAIL', 'SMS', 'PUSH', 'IN_APP') NOT NULL,
  `subject` VARCHAR(255),
  `body` TEXT NOT NULL,
  `variables` JSON,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`template_id`),
  UNIQUE KEY `idx_notification_templates_code_unique` (`template_code`),
  KEY `idx_notification_templates_type` (`template_type`),
  KEY `idx_notification_templates_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Notification and email templates';

-- ============================================
-- LICENSE & SUBSCRIPTION (Optional)
-- ============================================

CREATE TABLE `licenses` (
  `license_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `license_key` VARCHAR(100) NOT NULL,
  `license_type` ENUM('TRIAL', 'BASIC', 'PROFESSIONAL', 'ENTERPRISE') NOT NULL,
  `branch_id` INT UNSIGNED,
  `start_date` DATE NOT NULL,
  `end_date` DATE,
  `max_users` INT,
  `max_terminals` INT,
  `max_branches` INT,
  `features_enabled` JSON,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`license_id`),
  UNIQUE KEY `idx_licenses_license_key_unique` (`license_key`),
  KEY `idx_licenses_branch_id` (`branch_id`),
  KEY `idx_licenses_is_active` (`is_active`),
  CONSTRAINT `fk_licenses_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='License and subscription management';

-- ============================================
-- TABLE RESERVATIONS
-- ============================================

CREATE TABLE `tables` (
  `table_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `table_number` VARCHAR(20) NOT NULL,
  `table_name` VARCHAR(50),
  `capacity` INT DEFAULT 4,
  `section` VARCHAR(50),
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`table_id`),
  UNIQUE KEY `idx_tables_branch_table_unique` (`branch_id`, `table_number`),
  KEY `idx_tables_branch_id` (`branch_id`),
  KEY `idx_tables_section` (`section`),
  KEY `idx_tables_is_active` (`is_active`),
  CONSTRAINT `fk_tables_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Table/seat management for dine-in operations';

CREATE TABLE `reservations` (
  `reservation_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `table_id` INT UNSIGNED,
  `customer_id` INT UNSIGNED,
  `customer_name` VARCHAR(100),
  `customer_phone` VARCHAR(20),
  `reservation_date` DATE NOT NULL,
  `reservation_time` TIME NOT NULL,
  `party_size` INT NOT NULL,
  `status` ENUM('PENDING', 'CONFIRMED', 'SEATED', 'COMPLETED', 'CANCELLED', 'NO_SHOW') DEFAULT 'PENDING',
  `special_requests` TEXT,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`reservation_id`),
  KEY `idx_reservations_branch_id` (`branch_id`),
  KEY `idx_reservations_table_id` (`table_id`),
  KEY `idx_reservations_customer_id` (`customer_id`),
  KEY `idx_reservations_reservation_date` (`reservation_date`),
  KEY `idx_reservations_status` (`status`),
  CONSTRAINT `fk_reservations_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_reservations_table_id_tables_table_id` 
    FOREIGN KEY (`table_id`) REFERENCES `tables`(`table_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_reservations_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_reservations_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Table/seat reservations for dine-in operations';

-- ============================================
-- LOYALTY & REWARDS
-- ============================================

CREATE TABLE `loyalty_programs` (
  `program_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `description` TEXT,
  `points_per_currency` DECIMAL(10,4) DEFAULT 1.0000,
  `currency_per_point` DECIMAL(10,4) DEFAULT 0.0100,
  `min_points_redeem` INT DEFAULT 100,
  `max_points_redeem_percent` INT DEFAULT 100,
  `expiry_months` INT,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`program_id`),
  KEY `idx_loyalty_programs_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Loyalty program configuration';

CREATE TABLE `loyalty_tiers` (
  `tier_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `program_id` INT UNSIGNED NOT NULL,
  `name` VARCHAR(50) NOT NULL,
  `min_points` INT NOT NULL,
  `discount_percent` DECIMAL(5,2) DEFAULT 0.00,
  `benefits` JSON,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`tier_id`),
  KEY `idx_loyalty_tiers_program_id` (`program_id`),
  CONSTRAINT `fk_loyalty_tiers_program_id_loyalty_programs_program_id` 
    FOREIGN KEY (`program_id`) REFERENCES `loyalty_programs`(`program_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Loyalty tier levels';

CREATE TABLE `customer_loyalty` (
  `customer_loyalty_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `customer_id` INT UNSIGNED NOT NULL,
  `program_id` INT UNSIGNED NOT NULL,
  `tier_id` INT UNSIGNED,
  `current_points` INT DEFAULT 0,
  `total_earned` INT DEFAULT 0,
  `total_redeemed` INT DEFAULT 0,
  `joined_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`customer_loyalty_id`),
  UNIQUE KEY `idx_customer_loyalty_customer_program_unique` (`customer_id`, `program_id`),
  KEY `idx_customer_loyalty_program_id` (`program_id`),
  KEY `idx_customer_loyalty_tier_id` (`tier_id`),
  CONSTRAINT `fk_customer_loyalty_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_customer_loyalty_program_id_loyalty_programs_program_id` 
    FOREIGN KEY (`program_id`) REFERENCES `loyalty_programs`(`program_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_customer_loyalty_tier_id_loyalty_tiers_tier_id` 
    FOREIGN KEY (`tier_id`) REFERENCES `loyalty_tiers`(`tier_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Customer loyalty membership';

CREATE TABLE `loyalty_transactions` (
  `transaction_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `customer_loyalty_id` INT UNSIGNED NOT NULL,
  `transaction_type` ENUM('EARN', 'REDEEM', 'ADJUST', 'EXPIRE') NOT NULL,
  `points` INT NOT NULL,
  `balance_after` INT NOT NULL,
  `reference_type` VARCHAR(50),
  `reference_id` INT UNSIGNED,
  `notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`transaction_id`),
  KEY `idx_loyalty_transactions_customer_loyalty_id` (`customer_loyalty_id`),
  KEY `idx_loyalty_transactions_transaction_type` (`transaction_type`),
  KEY `idx_loyalty_transactions_reference` (`reference_type`, `reference_id`),
  CONSTRAINT `fk_loyalty_transactions_customer_loyalty_id_customer_loyalty_customer_loyalty_id` 
    FOREIGN KEY (`customer_loyalty_id`) REFERENCES `customer_loyalty`(`customer_loyalty_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Loyalty points transaction history';

-- ============================================
-- GIFT CARDS
-- ============================================

CREATE TABLE `gift_cards` (
  `gift_card_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `card_number` VARCHAR(50) NOT NULL,
  `pin` VARCHAR(20),
  `customer_id` INT UNSIGNED,
  `initial_balance` DECIMAL(12,2) NOT NULL,
  `current_balance` DECIMAL(12,2) NOT NULL,
  `issue_date` DATE NOT NULL,
  `expiry_date` DATE,
  `status` ENUM('ACTIVE', 'EXPIRED', 'VOID', 'USED') DEFAULT 'ACTIVE',
  `notes` TEXT,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`gift_card_id`),
  UNIQUE KEY `idx_gift_cards_card_number_unique` (`card_number`),
  KEY `idx_gift_cards_customer_id` (`customer_id`),
  KEY `idx_gift_cards_status` (`status`),
  KEY `idx_gift_cards_expiry_date` (`expiry_date`),
  CONSTRAINT `fk_gift_cards_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_gift_cards_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Gift card management';

CREATE TABLE `gift_card_transactions` (
  `gift_card_transaction_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `gift_card_id` INT UNSIGNED NOT NULL,
  `transaction_type` ENUM('ISSUE', 'LOAD', 'REDEEM', 'REFUND', 'ADJUST') NOT NULL,
  `amount` DECIMAL(12,2) NOT NULL,
  `balance_after` DECIMAL(12,2) NOT NULL,
  `reference_type` VARCHAR(50),
  `reference_id` INT UNSIGNED,
  `notes` TEXT,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`gift_card_transaction_id`),
  KEY `idx_gift_card_transactions_gift_card_id` (`gift_card_id`),
  KEY `idx_gift_card_transactions_transaction_type` (`transaction_type`),
  KEY `idx_gift_card_transactions_reference` (`reference_type`, `reference_id`),
  CONSTRAINT `fk_gift_card_transactions_gift_card_id_gift_cards_gift_card_id` 
    FOREIGN KEY (`gift_card_id`) REFERENCES `gift_cards`(`gift_card_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_gift_card_transactions_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Gift card transaction history';

-- ============================================
-- STAFF COMMISSIONS
-- ============================================

CREATE TABLE `staff_commissions` (
  `commission_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT UNSIGNED NOT NULL,
  `sale_id` INT UNSIGNED NOT NULL,
  `commission_type` ENUM('PERCENTAGE', 'FIXED') NOT NULL,
  `commission_rate` DECIMAL(5,4) NOT NULL,
  `commission_amount` DECIMAL(10,2) NOT NULL,
  `is_paid` BOOLEAN DEFAULT FALSE,
  `paid_date` DATE,
  `notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`commission_id`),
  KEY `idx_staff_commissions_user_id` (`user_id`),
  KEY `idx_staff_commissions_sale_id` (`sale_id`),
  KEY `idx_staff_commissions_is_paid` (`is_paid`),
  CONSTRAINT `fk_staff_commissions_user_id_users_user_id` 
    FOREIGN KEY (`user_id`) REFERENCES `users`(`user_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_staff_commissions_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Staff commission tracking';

-- ============================================
-- PROMOTIONS & CAMPAIGNS
-- ============================================

CREATE TABLE `promotions` (
  `promotion_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `description` TEXT,
  `promotion_type` ENUM('PERCENTAGE', 'FIXED_AMOUNT', 'BUY_X_GET_Y', 'BOGO', 'BUNDLE') NOT NULL,
  `discount_value` DECIMAL(10,2) NOT NULL,
  `min_purchase_amount` DECIMAL(10,2) DEFAULT 0.00,
  `max_discount_amount` DECIMAL(10,2),
  `buy_quantity` INT,
  `get_quantity` INT,
  `buy_product_id` INT UNSIGNED,
  `get_product_id` INT UNSIGNED,
  `valid_from` DATETIME NOT NULL,
  `valid_until` DATETIME NOT NULL,
  `applicable_days` JSON,
  `customer_tier_id` INT UNSIGNED,
  `usage_limit` INT,
  `usage_count` INT DEFAULT 0,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`promotion_id`),
  KEY `idx_promotions_promotion_type` (`promotion_type`),
  KEY `idx_promotions_valid_period` (`valid_from`, `valid_until`),
  KEY `idx_promotions_is_active` (`is_active`),
  KEY `idx_promotions_buy_product_id` (`buy_product_id`),
  KEY `idx_promotions_get_product_id` (`get_product_id`),
  CONSTRAINT `fk_promotions_buy_product_id_products_product_id` 
    FOREIGN KEY (`buy_product_id`) REFERENCES `products`(`product_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_promotions_get_product_id_products_product_id` 
    FOREIGN KEY (`get_product_id`) REFERENCES `products`(`product_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Promotions and marketing campaigns';

-- ============================================
-- EMPLOYEE SCHEDULES
-- ============================================

CREATE TABLE `employee_schedules` (
  `schedule_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `shift_date` DATE NOT NULL,
  `shift_start` TIME NOT NULL,
  `shift_end` TIME NOT NULL,
  `break_duration` INT DEFAULT 0,
  `status` ENUM('SCHEDULED', 'CHECKED_IN', 'CHECKED_OUT', 'ABSENT', 'LATE') DEFAULT 'SCHEDULED',
  `notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`schedule_id`),
  KEY `idx_employee_schedules_user_id` (`user_id`),
  KEY `idx_employee_schedules_branch_id` (`branch_id`),
  KEY `idx_employee_schedules_shift_date` (`shift_date`),
  KEY `idx_employee_schedules_status` (`status`),
  CONSTRAINT `fk_employee_schedules_user_id_users_user_id` 
    FOREIGN KEY (`user_id`) REFERENCES `users`(`user_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_employee_schedules_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Employee work schedules and shifts';

-- ============================================
-- SERIAL NUMBER TRACKING
-- ============================================

CREATE TABLE `product_serials` (
  `serial_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `product_id` INT UNSIGNED NOT NULL,
  `serial_number` VARCHAR(100) NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `purchase_order_item_id` INT UNSIGNED,
  `sale_item_id` INT UNSIGNED,
  `status` ENUM('IN_STOCK', 'SOLD', 'RETURNED', 'DEFECTIVE', 'LOST') DEFAULT 'IN_STOCK',
  `purchase_date` DATE,
  `sale_date` DATE,
  `warranty_expiry` DATE,
  `notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`serial_id`),
  UNIQUE KEY `idx_product_serials_serial_unique` (`serial_number`),
  KEY `idx_product_serials_product_id` (`product_id`),
  KEY `idx_product_serials_branch_id` (`branch_id`),
  KEY `idx_product_serials_status` (`status`),
  KEY `idx_product_serials_purchase_order_item_id` (`purchase_order_item_id`),
  KEY `idx_product_serials_sale_item_id` (`sale_item_id`),
  CONSTRAINT `fk_product_serials_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_product_serials_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_product_serials_purchase_order_item_id_purchase_order_items_purchase_order_item_id` 
    FOREIGN KEY (`purchase_order_item_id`) REFERENCES `purchase_order_items`(`purchase_order_item_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_product_serials_sale_item_id_sale_items_sale_item_id` 
    FOREIGN KEY (`sale_item_id`) REFERENCES `sale_items`(`sale_item_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Serial number tracking for high-value items';

-- ============================================
-- BATCH/LOT TRACKING
-- ============================================

CREATE TABLE `product_batches` (
  `batch_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `product_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `batch_number` VARCHAR(50) NOT NULL,
  `manufacturing_date` DATE,
  `expiry_date` DATE,
  `quantity` DECIMAL(10,2) NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `purchase_order_item_id` INT UNSIGNED,
  `status` ENUM('ACTIVE', 'EXPIRED', 'CONSUMED', 'RECALLED') DEFAULT 'ACTIVE',
  `notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`batch_id`),
  UNIQUE KEY `idx_product_batches_product_branch_batch_unique` (`product_id`, `branch_id`, `batch_number`),
  KEY `idx_product_batches_product_id` (`product_id`),
  KEY `idx_product_batches_branch_id` (`branch_id`),
  KEY `idx_product_batches_expiry_date` (`expiry_date`),
  KEY `idx_product_batches_status` (`status`),
  KEY `idx_product_batches_unit_id` (`unit_id`),
  CONSTRAINT `fk_product_batches_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_product_batches_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_product_batches_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_product_batches_purchase_order_item_id_purchase_order_items_purchase_order_item_id` 
    FOREIGN KEY (`purchase_order_item_id`) REFERENCES `purchase_order_items`(`purchase_order_item_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Batch/lot tracking with expiry dates';

-- ============================================
-- TRANSFER ORDERS
-- ============================================

CREATE TABLE `transfer_orders` (
  `transfer_order_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `transfer_number` VARCHAR(50) NOT NULL,
  `from_branch_id` INT UNSIGNED NOT NULL,
  `to_branch_id` INT UNSIGNED NOT NULL,
  `transfer_date` DATE NOT NULL,
  `status` ENUM('PENDING', 'APPROVED', 'IN_TRANSIT', 'RECEIVED', 'CANCELLED') DEFAULT 'PENDING',
  `notes` TEXT,
  `created_by` INT UNSIGNED,
  `approved_by` INT UNSIGNED,
  `approved_at` TIMESTAMP NULL,
  `received_by` INT UNSIGNED,
  `received_at` TIMESTAMP NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`transfer_order_id`),
  UNIQUE KEY `idx_transfer_orders_transfer_number_unique` (`transfer_number`),
  KEY `idx_transfer_orders_from_branch_id` (`from_branch_id`),
  KEY `idx_transfer_orders_to_branch_id` (`to_branch_id`),
  KEY `idx_transfer_orders_status` (`status`),
  KEY `idx_transfer_orders_transfer_date` (`transfer_date`),
  CONSTRAINT `fk_transfer_orders_from_branch_id_branches_branch_id` 
    FOREIGN KEY (`from_branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_transfer_orders_to_branch_id_branches_branch_id` 
    FOREIGN KEY (`to_branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_transfer_orders_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_transfer_orders_approved_by_users_user_id` 
    FOREIGN KEY (`approved_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_transfer_orders_received_by_users_user_id` 
    FOREIGN KEY (`received_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Inter-branch inventory transfer orders';

CREATE TABLE `transfer_order_items` (
  `transfer_item_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `transfer_order_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity` DECIMAL(10,2) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,2) NOT NULL,
  `batch_id` INT UNSIGNED,
  `notes` TEXT,
  PRIMARY KEY (`transfer_item_id`),
  KEY `idx_transfer_order_items_transfer_order_id` (`transfer_order_id`),
  KEY `idx_transfer_order_items_product_id` (`product_id`),
  KEY `idx_transfer_order_items_batch_id` (`batch_id`),
  CONSTRAINT `fk_transfer_order_items_transfer_order_id_transfer_orders_transfer_order_id` 
    FOREIGN KEY (`transfer_order_id`) REFERENCES `transfer_orders`(`transfer_order_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_transfer_order_items_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_transfer_order_items_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_transfer_order_items_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_transfer_order_items_batch_id_product_batches_batch_id` 
    FOREIGN KEY (`batch_id`) REFERENCES `product_batches`(`batch_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Items in transfer orders with unit conversion';

-- ============================================
-- TAX EXEMPTIONS
-- ============================================

CREATE TABLE `customer_tax_exemptions` (
  `exemption_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `customer_id` INT UNSIGNED NOT NULL,
  `exemption_type` VARCHAR(50) NOT NULL,
  `certificate_number` VARCHAR(100),
  `issue_date` DATE,
  `expiry_date` DATE,
  `issuing_authority` VARCHAR(100),
  `is_active` BOOLEAN DEFAULT TRUE,
  `notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`exemption_id`),
  KEY `idx_customer_tax_exemptions_customer_id` (`customer_id`),
  KEY `idx_customer_tax_exemptions_exemption_type` (`exemption_type`),
  KEY `idx_customer_tax_exemptions_is_active` (`is_active`),
  KEY `idx_customer_tax_exemptions_expiry_date` (`expiry_date`),
  CONSTRAINT `fk_customer_tax_exemptions_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Customer tax exemption certificates';

-- ============================================
-- CUSTOMER FEEDBACK & RATINGS
-- ============================================

CREATE TABLE `customer_feedback` (
  `feedback_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `customer_id` INT UNSIGNED,
  `sale_id` INT UNSIGNED,
  `branch_id` INT UNSIGNED NOT NULL,
  `rating` INT CHECK (`rating` BETWEEN 1 AND 5),
  `comment` TEXT,
  `feedback_type` ENUM('PRODUCT', 'SERVICE', 'DELIVERY', 'OVERALL') DEFAULT 'OVERALL',
  `is_anonymous` BOOLEAN DEFAULT FALSE,
  `is_approved` BOOLEAN DEFAULT FALSE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`feedback_id`),
  KEY `idx_customer_feedback_customer_id` (`customer_id`),
  KEY `idx_customer_feedback_sale_id` (`sale_id`),
  KEY `idx_customer_feedback_branch_id` (`branch_id`),
  KEY `idx_customer_feedback_rating` (`rating`),
  KEY `idx_customer_feedback_is_approved` (`is_approved`),
  CONSTRAINT `fk_customer_feedback_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_customer_feedback_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_customer_feedback_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Customer feedback and ratings';

-- ============================================
-- PRICE HISTORY
-- ============================================

CREATE TABLE `price_history` (
  `price_history_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `product_id` INT UNSIGNED NOT NULL,
  `product_variant_id` INT UNSIGNED,
  `old_cost_price` DECIMAL(10,2),
  `new_cost_price` DECIMAL(10,2),
  `old_selling_price` DECIMAL(10,2),
  `new_selling_price` DECIMAL(10,2),
  `reason` VARCHAR(255),
  `changed_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`price_history_id`),
  KEY `idx_price_history_product_id` (`product_id`),
  KEY `idx_price_history_product_variant_id` (`product_variant_id`),
  KEY `idx_price_history_created_at` (`created_at`),
  KEY `idx_price_history_changed_by` (`changed_by`),
  CONSTRAINT `fk_price_history_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_price_history_product_variant_id_product_variants_variant_id` 
    FOREIGN KEY (`product_variant_id`) REFERENCES `product_variants`(`variant_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_price_history_changed_by_users_user_id` 
    FOREIGN KEY (`changed_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Product price change history';

-- ============================================
-- CUSTOMER GROUPS
-- ============================================

CREATE TABLE `customer_groups` (
  `group_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `description` TEXT,
  `discount_percent` DECIMAL(5,2) DEFAULT 0.00,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`group_id`),
  KEY `idx_customer_groups_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Customer groups/segments';

CREATE TABLE `customer_group_memberships` (
  `membership_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `customer_id` INT UNSIGNED NOT NULL,
  `group_id` INT UNSIGNED NOT NULL,
  `joined_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`membership_id`),
  UNIQUE KEY `idx_customer_group_memberships_customer_group_unique` (`customer_id`, `group_id`),
  KEY `idx_customer_group_memberships_customer_id` (`customer_id`),
  KEY `idx_customer_group_memberships_group_id` (`group_id`),
  CONSTRAINT `fk_customer_group_memberships_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_customer_group_memberships_group_id_customer_groups_group_id` 
    FOREIGN KEY (`group_id`) REFERENCES `customer_groups`(`group_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Customer group memberships';

-- ============================================
-- QUOTATIONS/ESTIMATES
-- ============================================

CREATE TABLE `quotations` (
  `quotation_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `quotation_number` VARCHAR(50) NOT NULL,
  `customer_id` INT UNSIGNED,
  `branch_id` INT UNSIGNED NOT NULL,
  `valid_until` DATE NOT NULL,
  `subtotal` DECIMAL(12,2) NOT NULL,
  `discount_amount` DECIMAL(12,2) DEFAULT 0.00,
  `tax_amount` DECIMAL(12,2) DEFAULT 0.00,
  `total_amount` DECIMAL(12,2) NOT NULL,
  `notes` TEXT,
  `status` ENUM('DRAFT', 'SENT', 'ACCEPTED', 'REJECTED', 'EXPIRED', 'CONVERTED') DEFAULT 'DRAFT',
  `converted_to_sale_id` INT UNSIGNED,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`quotation_id`),
  UNIQUE KEY `idx_quotations_quotation_number_unique` (`quotation_number`),
  KEY `idx_quotations_customer_id` (`customer_id`),
  KEY `idx_quotations_branch_id` (`branch_id`),
  KEY `idx_quotations_status` (`status`),
  KEY `idx_quotations_valid_until` (`valid_until`),
  CONSTRAINT `fk_quotations_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_quotations_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_quotations_converted_to_sale_id_sales_sale_id` 
    FOREIGN KEY (`converted_to_sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_quotations_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Quotations and estimates';

CREATE TABLE `quotation_items` (
  `quotation_item_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `quotation_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `product_variant_id` INT UNSIGNED,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity` DECIMAL(10,2) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,2) NOT NULL,
  `unit_price` DECIMAL(10,2) NOT NULL,
  `discount_amount` DECIMAL(10,2) DEFAULT 0.00,
  `tax_amount` DECIMAL(10,2) DEFAULT 0.00,
  `total_price` DECIMAL(10,2) NOT NULL,
  `notes` TEXT,
  PRIMARY KEY (`quotation_item_id`),
  KEY `idx_quotation_items_quotation_id` (`quotation_id`),
  KEY `idx_quotation_items_product_id` (`product_id`),
  KEY `idx_quotation_items_product_variant_id` (`product_variant_id`),
  CONSTRAINT `fk_quotation_items_quotation_id_quotations_quotation_id` 
    FOREIGN KEY (`quotation_id`) REFERENCES `quotations`(`quotation_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_quotation_items_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_quotation_items_product_variant_id_product_variants_variant_id` 
    FOREIGN KEY (`product_variant_id`) REFERENCES `product_variants`(`variant_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_quotation_items_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_quotation_items_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Items in quotations with unit conversion';

-- ============================================
-- API INTEGRATIONS
-- ============================================

CREATE TABLE `api_integrations` (
  `integration_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `integration_name` VARCHAR(100) NOT NULL,
  `integration_type` VARCHAR(50) NOT NULL,
  `api_key` VARCHAR(255),
  `api_secret` VARCHAR(255),
  `endpoint_url` VARCHAR(255),
  `webhook_url` VARCHAR(255),
  `config_data` JSON,
  `is_active` BOOLEAN DEFAULT TRUE,
  `last_sync_at` TIMESTAMP NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`integration_id`),
  KEY `idx_api_integrations_integration_type` (`integration_type`),
  KEY `idx_api_integrations_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Third-party API integrations';

CREATE TABLE `api_logs` (
  `api_log_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `integration_id` INT UNSIGNED NOT NULL,
  `request_type` ENUM('GET', 'POST', 'PUT', 'DELETE', 'WEBHOOK') NOT NULL,
  `endpoint` VARCHAR(255),
  `request_payload` JSON,
  `response_payload` JSON,
  `status_code` INT,
  `status` ENUM('SUCCESS', 'FAILED', 'PENDING') NOT NULL,
  `error_message` TEXT,
  `execution_time_ms` INT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`api_log_id`),
  KEY `idx_api_logs_integration_id` (`integration_id`),
  KEY `idx_api_logs_status` (`status`),
  KEY `idx_api_logs_created_at` (`created_at`),
  CONSTRAINT `fk_api_logs_integration_id_api_integrations_integration_id` 
    FOREIGN KEY (`integration_id`) REFERENCES `api_integrations`(`integration_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='API request/response logs';

-- ============================================
-- KITCHEN DISPLAY SYSTEM (KDS)
-- ============================================

CREATE TABLE `kitchen_stations` (
  `station_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `name` VARCHAR(50) NOT NULL,
  `description` VARCHAR(255),
  `display_order` INT DEFAULT 0,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`station_id`),
  KEY `idx_kitchen_stations_branch_id` (`branch_id`),
  KEY `idx_kitchen_stations_display_order` (`display_order`),
  KEY `idx_kitchen_stations_is_active` (`is_active`),
  CONSTRAINT `fk_kitchen_stations_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Kitchen preparation stations (grill, bar, prep, etc.)';

CREATE TABLE `kitchen_station_products` (
  `station_product_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `station_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `preparation_time_minutes` INT DEFAULT 0,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`station_product_id`),
  UNIQUE KEY `idx_kitchen_station_products_station_product_unique` (`station_id`, `product_id`),
  KEY `idx_kitchen_station_products_product_id` (`product_id`),
  CONSTRAINT `fk_kitchen_station_products_station_id_kitchen_stations_station_id` 
    FOREIGN KEY (`station_id`) REFERENCES `kitchen_stations`(`station_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_kitchen_station_products_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Products assigned to kitchen stations';

CREATE TABLE `kitchen_orders` (
  `kitchen_order_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `sale_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `table_id` INT UNSIGNED,
  `order_type` ENUM('DINE_IN', 'TAKEOUT', 'DELIVERY') NOT NULL,
  `status` ENUM('PENDING', 'IN_PROGRESS', 'READY', 'SERVED', 'CANCELLED') DEFAULT 'PENDING',
  `priority` ENUM('NORMAL', 'HIGH', 'URGENT') DEFAULT 'NORMAL',
  `special_instructions` TEXT,
  `started_at` TIMESTAMP NULL,
  `completed_at` TIMESTAMP NULL,
  `served_at` TIMESTAMP NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`kitchen_order_id`),
  UNIQUE KEY `idx_kitchen_orders_sale_id_unique` (`sale_id`),
  KEY `idx_kitchen_orders_branch_id` (`branch_id`),
  KEY `idx_kitchen_orders_table_id` (`table_id`),
  KEY `idx_kitchen_orders_status` (`status`),
  KEY `idx_kitchen_orders_priority` (`priority`),
  KEY `idx_kitchen_orders_created_at` (`created_at`),
  CONSTRAINT `fk_kitchen_orders_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_kitchen_orders_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_kitchen_orders_table_id_tables_table_id` 
    FOREIGN KEY (`table_id`) REFERENCES `tables`(`table_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Kitchen orders for display system';

CREATE TABLE `kitchen_order_items` (
  `kitchen_item_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `kitchen_order_id` INT UNSIGNED NOT NULL,
  `sale_item_id` INT UNSIGNED NOT NULL,
  `station_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `product_name` VARCHAR(100) NOT NULL,
  `quantity` DECIMAL(10,2) NOT NULL,
  `unit_name` VARCHAR(20),
  `special_instructions` TEXT,
  `status` ENUM('PENDING', 'IN_PROGRESS', 'READY', 'SERVED', 'CANCELLED') DEFAULT 'PENDING',
  `started_at` TIMESTAMP NULL,
  `completed_at` TIMESTAMP NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`kitchen_item_id`),
  KEY `idx_kitchen_order_items_kitchen_order_id` (`kitchen_order_id`),
  KEY `idx_kitchen_order_items_sale_item_id` (`sale_item_id`),
  KEY `idx_kitchen_order_items_station_id` (`station_id`),
  KEY `idx_kitchen_order_items_status` (`status`),
  CONSTRAINT `fk_kitchen_order_items_kitchen_order_id_kitchen_orders_kitchen_order_id` 
    FOREIGN KEY (`kitchen_order_id`) REFERENCES `kitchen_orders`(`kitchen_order_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_kitchen_order_items_sale_item_id_sale_items_sale_item_id` 
    FOREIGN KEY (`sale_item_id`) REFERENCES `sale_items`(`sale_item_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_kitchen_order_items_station_id_kitchen_stations_station_id` 
    FOREIGN KEY (`station_id`) REFERENCES `kitchen_stations`(`station_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Individual items in kitchen orders';

CREATE TABLE `kitchen_order_item_addons` (
  `kitchen_addon_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `kitchen_item_id` INT UNSIGNED NOT NULL,
  `sale_item_addon_id` INT UNSIGNED NOT NULL,
  `addon_name` VARCHAR(100) NOT NULL,
  `quantity` DECIMAL(10,2) NOT NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`kitchen_addon_id`),
  KEY `idx_kitchen_order_item_addons_kitchen_item_id` (`kitchen_item_id`),
  KEY `idx_kitchen_order_item_addons_sale_item_addon_id` (`sale_item_addon_id`),
  CONSTRAINT `fk_kitchen_order_item_addons_kitchen_item_id_kitchen_order_items_kitchen_item_id` 
    FOREIGN KEY (`kitchen_item_id`) REFERENCES `kitchen_order_items`(`kitchen_item_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_kitchen_order_item_addons_sale_item_addon_id_sale_item_addons_sale_item_addon_id` 
    FOREIGN KEY (`sale_item_addon_id`) REFERENCES `sale_item_addons`(`sale_item_addon_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Add-ons/modifiers for kitchen order items';

-- ============================================
-- RETURN MANAGEMENT
-- ============================================

CREATE TABLE `return_orders` (
  `return_order_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `return_number` VARCHAR(50) NOT NULL,
  `sale_id` INT UNSIGNED NOT NULL,
  `customer_id` INT UNSIGNED,
  `branch_id` INT UNSIGNED NOT NULL,
  `return_date` DATE NOT NULL,
  `reason` TEXT NOT NULL,
  `status` ENUM('PENDING', 'APPROVED', 'REJECTED', 'PROCESSING', 'COMPLETED', 'CANCELLED') DEFAULT 'PENDING',
  `refund_method` ENUM('CASH', 'CARD', 'STORE_CREDIT', 'BANK_TRANSFER', 'ORIGINAL_PAYMENT'),
  `refund_amount` DECIMAL(12,2) NOT NULL,
  `restock_fee` DECIMAL(10,2) DEFAULT 0.00,
  `approved_by` INT UNSIGNED,
  `approved_at` TIMESTAMP NULL,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`return_order_id`),
  UNIQUE KEY `idx_return_orders_return_number_unique` (`return_number`),
  KEY `idx_return_orders_sale_id` (`sale_id`),
  KEY `idx_return_orders_customer_id` (`customer_id`),
  KEY `idx_return_orders_branch_id` (`branch_id`),
  KEY `idx_return_orders_status` (`status`),
  KEY `idx_return_orders_return_date` (`return_date`),
  CONSTRAINT `fk_return_orders_sale_id_sales_sale_id` 
    FOREIGN KEY (`sale_id`) REFERENCES `sales`(`sale_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_return_orders_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_return_orders_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_return_orders_approved_by_users_user_id` 
    FOREIGN KEY (`approved_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_return_orders_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Customer return orders';

CREATE TABLE `return_order_items` (
  `return_item_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `return_order_id` INT UNSIGNED NOT NULL,
  `sale_item_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `unit_id` INT UNSIGNED NOT NULL,
  `quantity` DECIMAL(10,2) NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,2) NOT NULL,
  `return_reason` TEXT,
  `condition` ENUM('NEW', 'OPENED', 'USED', 'DAMAGED') DEFAULT 'NEW',
  `refund_amount` DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (`return_item_id`),
  KEY `idx_return_order_items_return_order_id` (`return_order_id`),
  KEY `idx_return_order_items_sale_item_id` (`sale_item_id`),
  KEY `idx_return_order_items_product_id` (`product_id`),
  CONSTRAINT `fk_return_order_items_return_order_id_return_orders_return_order_id` 
    FOREIGN KEY (`return_order_id`) REFERENCES `return_orders`(`return_order_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_return_order_items_sale_item_id_sale_items_sale_item_id` 
    FOREIGN KEY (`sale_item_id`) REFERENCES `sale_items`(`sale_item_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_return_order_items_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_return_order_items_unit_id_units_unit_id` 
    FOREIGN KEY (`unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_return_order_items_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Items in return orders';

-- ============================================
-- SUPPLIER PERFORMANCE
-- ============================================

CREATE TABLE `supplier_performance` (
  `performance_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `supplier_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `period_start` DATE NOT NULL,
  `period_end` DATE NOT NULL,
  `total_orders` INT DEFAULT 0,
  `on_time_deliveries` INT DEFAULT 0,
  `late_deliveries` INT DEFAULT 0,
  `on_time_percentage` DECIMAL(5,2) DEFAULT 0.00,
  `total_amount` DECIMAL(12,2) DEFAULT 0.00,
  `average_delivery_days` DECIMAL(5,2),
  `quality_rating` DECIMAL(3,2),
  `notes` TEXT,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`performance_id`),
  UNIQUE KEY `idx_supplier_performance_supplier_branch_period_unique` (`supplier_id`, `branch_id`, `period_start`, `period_end`),
  KEY `idx_supplier_performance_supplier_id` (`supplier_id`),
  KEY `idx_supplier_performance_branch_id` (`branch_id`),
  KEY `idx_supplier_performance_period_start` (`period_start`),
  CONSTRAINT `fk_supplier_performance_supplier_id_suppliers_supplier_id` 
    FOREIGN KEY (`supplier_id`) REFERENCES `suppliers`(`supplier_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_supplier_performance_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Supplier performance metrics';

-- ============================================
-- WAREHOUSE MANAGEMENT
-- ============================================

CREATE TABLE `warehouses` (
  `warehouse_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `branch_id` INT UNSIGNED NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  `code` VARCHAR(20) NOT NULL,
  `address` TEXT,
  `manager_id` INT UNSIGNED,
  `phone` VARCHAR(20),
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`warehouse_id`),
  UNIQUE KEY `idx_warehouses_code_unique` (`code`),
  KEY `idx_warehouses_branch_id` (`branch_id`),
  KEY `idx_warehouses_manager_id` (`manager_id`),
  KEY `idx_warehouses_is_active` (`is_active`),
  CONSTRAINT `fk_warehouses_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_warehouses_manager_id_users_user_id` 
    FOREIGN KEY (`manager_id`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Warehouse/storage locations';

CREATE TABLE `warehouse_stocks` (
  `warehouse_stock_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `warehouse_id` INT UNSIGNED NOT NULL,
  `product_id` INT UNSIGNED NOT NULL,
  `base_unit_id` INT UNSIGNED NOT NULL,
  `base_quantity` DECIMAL(10,2) NOT NULL,
  `display_unit_id` INT UNSIGNED NOT NULL,
  `display_quantity` DECIMAL(10,2) NOT NULL,
  `reorder_level` DECIMAL(10,2) DEFAULT 0.00,
  `bin_location` VARCHAR(50),
  `last_counted_at` TIMESTAMP NULL,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`warehouse_stock_id`),
  UNIQUE KEY `idx_warehouse_stocks_warehouse_product_unique` (`warehouse_id`, `product_id`),
  KEY `idx_warehouse_stocks_product_id` (`product_id`),
  KEY `idx_warehouse_stocks_base_unit_id` (`base_unit_id`),
  KEY `idx_warehouse_stocks_display_unit_id` (`display_unit_id`),
  CONSTRAINT `fk_warehouse_stocks_warehouse_id_warehouses_warehouse_id` 
    FOREIGN KEY (`warehouse_id`) REFERENCES `warehouses`(`warehouse_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_warehouse_stocks_product_id_products_product_id` 
    FOREIGN KEY (`product_id`) REFERENCES `products`(`product_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_warehouse_stocks_base_unit_id_units_unit_id` 
    FOREIGN KEY (`base_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_warehouse_stocks_display_unit_id_units_unit_id` 
    FOREIGN KEY (`display_unit_id`) REFERENCES `units`(`unit_id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Warehouse inventory levels';

-- ============================================
-- MULTI-CURRENCY SUPPORT
-- ============================================

CREATE TABLE `currencies` (
  `currency_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(3) NOT NULL,
  `name` VARCHAR(50) NOT NULL,
  `symbol` VARCHAR(10) NOT NULL,
  `exchange_rate` DECIMAL(12,6) NOT NULL,
  `is_base` BOOLEAN DEFAULT FALSE,
  `is_active` BOOLEAN DEFAULT TRUE,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`currency_id`),
  UNIQUE KEY `idx_currencies_code_unique` (`code`),
  KEY `idx_currencies_is_base` (`is_base`),
  KEY `idx_currencies_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Supported currencies with exchange rates';

-- ============================================
-- APPOINTMENTS & SCHEDULING
-- ============================================

CREATE TABLE `appointments` (
  `appointment_id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `customer_id` INT UNSIGNED NOT NULL,
  `branch_id` INT UNSIGNED NOT NULL,
  `service_product_id` INT UNSIGNED NOT NULL,
  `staff_id` INT UNSIGNED,
  `appointment_date` DATETIME NOT NULL,
  `duration_minutes` INT NOT NULL,
  `status` ENUM('SCHEDULED', 'CONFIRMED', 'IN_PROGRESS', 'COMPLETED', 'CANCELLED', 'NO_SHOW') DEFAULT 'SCHEDULED',
  `notes` TEXT,
  `created_by` INT UNSIGNED,
  `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`appointment_id`),
  KEY `idx_appointments_customer_id` (`customer_id`),
  KEY `idx_appointments_branch_id` (`branch_id`),
  KEY `idx_appointments_service_product_id` (`service_product_id`),
  KEY `idx_appointments_staff_id` (`staff_id`),
  KEY `idx_appointments_appointment_date` (`appointment_date`),
  KEY `idx_appointments_status` (`status`),
  CONSTRAINT `fk_appointments_customer_id_customers_customer_id` 
    FOREIGN KEY (`customer_id`) REFERENCES `customers`(`customer_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_appointments_branch_id_branches_branch_id` 
    FOREIGN KEY (`branch_id`) REFERENCES `branches`(`branch_id`) ON DELETE CASCADE,
  CONSTRAINT `fk_appointments_service_product_id_products_product_id` 
    FOREIGN KEY (`service_product_id`) REFERENCES `products`(`product_id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_appointments_staff_id_users_user_id` 
    FOREIGN KEY (`staff_id`) REFERENCES `users`(`user_id`) ON DELETE SET NULL,
  CONSTRAINT `fk_appointments_created_by_users_user_id` 
    FOREIGN KEY (`created_by`) REFERENCES `users`(`user_id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Service appointments and scheduling';

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================
-- INITIAL DATA
-- ============================================

-- Default Roles
INSERT INTO `roles` (`name`, `description`, `is_system`) VALUES
('Super Admin', 'Full system access', TRUE),
('Manager', 'Branch management access', TRUE),
('Cashier', 'POS operations only', TRUE),
('Inventory Manager', 'Inventory management', TRUE),
('Reporter', 'Reports only', TRUE);

-- Default Permissions
INSERT INTO `permissions` (`name`, `description`, `module`) VALUES
('users.view', 'View users', 'Users'),
('users.create', 'Create users', 'Users'),
('users.edit', 'Edit users', 'Users'),
('users.delete', 'Delete users', 'Users'),
('products.view', 'View products', 'Products'),
('products.create', 'Create products', 'Products'),
('products.edit', 'Edit products', 'Products'),
('products.delete', 'Delete products', 'Products'),
('inventory.view', 'View inventory', 'Inventory'),
('inventory.adjust', 'Adjust inventory', 'Inventory'),
('sales.view', 'View sales', 'Sales'),
('sales.create', 'Create sales', 'Sales'),
('sales.refund', 'Refund sales', 'Sales'),
('reports.view', 'View reports', 'Reports'),
('settings.view', 'View settings', 'Settings'),
('settings.edit', 'Edit settings', 'Settings');

-- Default Payment Methods
INSERT INTO `payment_methods` (`name`, `code`, `is_active`) VALUES
('Cash', 'CASH', TRUE),
('Credit Card', 'CARD', TRUE),
('Debit Card', 'DEBIT', TRUE),
('Digital Wallet', 'WALLET', TRUE),
('Bank Transfer', 'BANK', TRUE),
('Check', 'CHECK', TRUE),
('Gift Card', 'GIFT', TRUE);

-- Default Units
INSERT INTO `units` (`name`, `abbreviation`, `base_unit_id`, `conversion_factor`, `is_base`) VALUES
('Piece', 'pcs', NULL, 1.0000, TRUE),
('Case', 'case', 1, 24.0000, FALSE),
('Box', 'box', 1, 12.0000, FALSE),
('Kilogram', 'kg', NULL, 1.0000, TRUE),
('Gram', 'g', 4, 0.0010, FALSE),
('Bag', 'bag', 4, 50.0000, FALSE),
('Liter', 'L', NULL, 1.0000, TRUE),
('Milliliter', 'mL', 7, 0.0010, FALSE),
('Bottle', 'bottle', 7, 0.3300, FALSE);

-- Sample Unit Conversions (for complex relationships)
INSERT INTO `unit_conversions` (`from_unit_id`, `to_unit_id`, `conversion_factor`, `is_active`) VALUES
-- Piece conversions
(2, 3, 2.0000, TRUE),  -- 1 Case = 2 Boxes
(3, 1, 12.0000, TRUE),  -- 1 Box = 12 Pieces
(2, 1, 24.0000, TRUE),  -- 1 Case = 24 Pieces
-- Weight conversions
(5, 6, 0.0200, TRUE),  -- 1 Gram = 0.02 Bag (50g per bag)
(6, 4, 50.0000, TRUE),  -- 1 Bag = 50 Grams
(5, 4, 0.0010, TRUE),  -- 1 Gram = 0.001 Kilograms
-- Volume conversions
(8, 9, 3.0303, TRUE),  -- 1 mL = 3.0303 Bottles (330mL per bottle)
(9, 7, 0.3300, TRUE),  -- 1 Bottle = 0.330 Liters
(8, 7, 0.0010, TRUE);  -- 1 mL = 0.001 Liters

-- Default System Settings
INSERT INTO `system_settings` (`key`, `value`, `description`) VALUES
('company.name', 'Enterprise POS', 'Company name'),
('company.address', '', 'Company address'),
('company.phone', '', 'Company phone'),
('company.email', '', 'Company email'),
('tax.rate', '12', 'Default tax rate (%)'),
('currency.symbol', '$', 'Currency symbol'),
('currency.code', 'USD', 'Currency code'),
('receipt.header', '', 'Receipt header'),
('receipt.footer', 'Thank you for your purchase!', 'Receipt footer');

-- Default Branding
INSERT INTO `branding` (`branch_id`, `company_name`, `primary_color`, `secondary_color`, `accent_color`, `theme`, `receipt_header`, `receipt_footer`) VALUES
(NULL, 'Enterprise POS', '#007bff', '#6c757d', '#28a745', 'LIGHT', 'ENTERPRISE POS\nThank you for shopping with us!', 'Thank you for your purchase!\nPlease come again.');

-- Initial Schema Migration
INSERT INTO `schema_migrations` (`version`, `description`, `status`) VALUES
('1.0.0', 'Initial schema creation', 'SUCCESS');

-- Initial App Version
INSERT INTO `app_versions` (`version_number`, `version_name`, `release_date`, `release_notes`, `is_current`) VALUES
('1.0.0', 'Initial Release', CURDATE(), 'First release of Enterprise POS with full inventory, sales, and reporting features', TRUE);

-- Default Notification Templates
INSERT INTO `notification_templates` (`template_code`, `template_name`, `template_type`, `subject`, `body`, `variables`) VALUES
('SALE_RECEIPT', 'Sale Receipt', 'EMAIL', 'Your Receipt from {{company_name}}', 'Dear {{customer_name}},\n\nThank you for your purchase. Here are your order details:\n\nOrder #{{order_number}}\nDate: {{order_date}}\n\n{{order_items}}\n\nTotal: {{total_amount}}\n\n{{receipt_footer}}', '["company_name", "customer_name", "order_number", "order_date", "order_items", "total_amount", "receipt_footer"]'),
('LOW_STOCK_ALERT', 'Low Stock Alert', 'IN_APP', NULL, 'Product {{product_name}} is running low on stock. Current quantity: {{current_quantity}}. Reorder level: {{reorder_level}}.', '["product_name", "current_quantity", "reorder_level"]'),
('SALE_COMPLETION', 'Sale Completed', 'PUSH', 'Sale Completed', 'Sale #{{sale_number}} has been completed successfully. Total amount: {{total_amount}}', '["sale_number", "total_amount"]');

-- ============================================
-- PERFORMANCE OPTIMIZATION NOTES
-- ============================================
-- 
-- 1. Index Strategy:
--    - All foreign keys have corresponding indexes for JOIN performance
--    - Unique indexes on code/name fields for uniqueness enforcement
--    - Composite indexes on frequently queried column combinations
--    - Time-based indexes on created_at/updated_at for date range queries
--
-- 2. Data Types:
--    - INT UNSIGNED for IDs (supports up to 4.2 billion records)
--    - BIGINT UNSIGNED for log tables (supports up to 18 quintillion records)
--    - DECIMAL(10,2) for prices (supports up to 99,999,999.99)
--    - DECIMAL(10,4) for unit conversion factors (high precision)
--    - DECIMAL(12,2) for totals (supports up to 999,999,999,999.99)
--    - TIMESTAMP for automatic timezone handling
--    - JSON for flexible audit trail data
--
-- 3. Security:
--    - All user passwords should be hashed (bcrypt/argon2)
--    - Foreign key constraints ensure referential integrity
--    - NOT NULL constraints on critical fields
--    - ENUM for fixed value sets (status, types)
--
-- 4. Scalability:
--    - Schema supports multi-branch operations
--    - Audit logs for compliance and debugging
--    - Soft deletes via is_active flags
--    - Timestamps for change tracking
--
-- 5. Unit Conversion System:
--    - Units table supports hierarchical unit relationships (base_unit_id, conversion_factor)
--    - unit_conversions table for complex multi-level conversions
--    - All inventory, purchase, and sales tables track both display and base units
--    - Recipes and production tables for ingredient consumption tracking
--    - Supports scenarios like: stock in cases, sell in bottles, consume in grams
--
-- 6. Naming Convention:
--    - Primary keys: {table_singular}_id (e.g., product_id, user_id)
--    - Foreign keys: {referenced_table}_{referenced_column} (e.g., product_id, user_id)
--    - Indexes: idx_{table}_{column(s)}_unique (if unique)
--    - Foreign key constraints: fk_{table}_{column}_{referenced_table}_{referenced_column}
--    - All names use snake_case for consistency
--
-- 7. Reporting & Analytics:
--    - sales_consumption table tracks automatic ingredient deduction from sales
--    - daily_sales_summary for daily sales, profit, and payment method breakdown
--    - product_performance for product-level profit analysis and margins
--    - inventory_valuation for financial reporting and asset valuation
--    - Views below provide pre-built queries for common reports
--
-- 8. Business Type Versatility:
--    - Schema supports retail, restaurant, and service-based businesses
--    - Products table can handle physical goods, food items, or services
--    - Recipes/production for food preparation or service bundling
--    - Add-ons/modifiers for customization across all business types
--    - Unit conversion for any measurement needs (weight, volume, pieces, time)
--    - Generic payment methods support various payment ecosystems
--    - Tax compliance settings configurable for different regions
--
-- 9. Developer & Branding Features:
--    - branding table for company-wide UI customization (colors, logos, themes)
--    - schema_migrations table for database version tracking and rollback support
--    - app_versions table for application release management
--    - notification_templates for customizable email/SMS/push notifications
--    - licenses table for subscription and feature-based access control
--    - All branding can be branch-specific for multi-tenant deployments
--
-- 10. Delivery Support:
--    - delivery_addresses table for customer delivery address management
--    - delivery_drivers table for driver information and availability
--    - delivery_orders table linked to sales for delivery tracking
--    - delivery_tracking table for real-time location and status updates
--    - Supports pickup, delivery, and third-party delivery options
--    - GPS coordinates for location tracking
--    - Delivery fee and distance calculation support
--
-- 11. Dine-In Operations:
--    - tables table for table/seat management with capacity and sections
--    - reservations table for table booking with status workflow
--    - Supports walk-in and reservation-based dining
--
-- 12. Loyalty & Rewards:
--    - loyalty_programs table for points-based loyalty configuration
--    - loyalty_tiers table for tiered membership levels
--    - customer_loyalty table for customer membership tracking
--    - loyalty_transactions table for points earn/redeem history
--    - Supports points expiration and tier-based benefits
--
-- 13. Gift Cards:
--    - gift_cards table for gift card issuance and balance tracking
--    - gift_card_transactions table for complete transaction history
--    - Supports issue, load, redeem, refund, and adjust operations
--    - PIN protection and expiry date support
--
-- 14. Staff Commissions:
--    - staff_commissions table for sales-based commission tracking
--    - Supports percentage and fixed amount commission types
--    - Payment status tracking for commission payouts
--
-- 15. Promotions & Campaigns:
--    - promotions table for marketing campaign management
--    - Supports percentage, fixed amount, buy-x-get-y, BOGO, and bundle promotions
--    - Time-based validity with day restrictions
--    - Usage limits and customer tier targeting
--
-- 16. Employee Management:
--    - employee_schedules table for work shifts and attendance tracking
--    - Supports check-in/check-out, break duration, and status tracking
--
-- 17. Advanced Inventory Tracking:
--    - product_serials table for high-value item serial number tracking
--    - product_batches table for batch/lot tracking with expiry dates
--    - Supports warranty tracking and recall management
--
-- 18. Inter-Branch Operations:
--    - transfer_orders table for inventory transfers between branches
--    - transfer_order_items table with unit conversion support
--    - Approval workflow and batch tracking integration
--
-- 19. Tax Compliance:
--    - tax_compliance_settings table for tax ID and permit tracking
--    - Certificate tracking with expiry dates
--    - Support for various exemption types
--    - shift_readings table for end-of-day reporting
--
-- 20. Customer Engagement:
--    - customer_feedback table for ratings and reviews
--    - Supports product, service, delivery, and overall feedback
--    - Anonymous feedback option and approval workflow
--
-- 21. Price Management:
--    - price_history table for tracking price changes
--    - Records both cost and selling price changes
--    - Tracks who made changes and the reason
--
-- 22. Customer Segmentation:
--    - customer_groups table for customer segments
--    - Group-based discount percentages
--    - customer_group_memberships for group assignments
--
-- 23. Quotations & Estimates:
--    - quotations table for price quotes and estimates
--    - quotation_items with unit conversion support
--    - Status workflow and conversion to sales
--
-- 24. Third-Party Integrations:
--    - api_integrations table for external service connections
--    - api_logs for request/response tracking
--    - Webhook support and sync status tracking
--
-- 25. Kitchen Display System (KDS):
--    - kitchen_stations table for preparation areas (grill, bar, prep, etc.)
--    - kitchen_station_products for routing products to specific stations
--    - kitchen_orders for order display linked to sales
--    - kitchen_order_items for individual item status tracking
--    - kitchen_order_item_addons for modifiers in kitchen display
--    - Supports dine-in, takeout, and delivery order types
--    - Priority levels (NORMAL, HIGH, URGENT) for order management
--    - Complete status workflow: PENDING → IN_PROGRESS → READY → SERVED
--
-- 26. Return Management:
--    - return_orders table for customer return processing
--    - return_order_items for individual item returns with condition tracking
--    - Approval workflow and refund method support
--    - Restock fee and condition tracking (NEW, OPENED, USED, DAMAGED)
--
-- 27. Supplier Performance:
--    - supplier_performance table for tracking supplier metrics
--    - On-time delivery percentage and quality ratings
--    - Period-based performance analysis
--
-- 28. Warehouse Management:
--    - warehouses table for storage location management
--    - warehouse_stocks table for warehouse-specific inventory
--    - Bin location tracking and last counted dates
--    - Unit conversion support for warehouse stocks
--
-- 29. Multi-Currency Support:
--    - currencies table for supported currencies
--    - Exchange rate tracking and base currency designation
--    - Support for international operations
--
-- 30. Service Appointments:
--    - appointments table for service-based businesses
--    - Staff assignment and duration tracking
--    - Status workflow for appointment management
--    - Integration with products for service items
--

-- ============================================
-- REPORTING VIEWS
-- ============================================

-- View: Current Stock Levels with Valuation
CREATE OR REPLACE VIEW `v_current_stock` AS
SELECT 
    ps.stock_id,
    b.branch_id,
    b.name AS branch_name,
    p.product_id,
    p.code AS product_code,
    p.name AS product_name,
    pc.name AS category_name,
    ps.base_unit_id,
    u_base.name AS base_unit_name,
    ps.base_quantity,
    ps.display_unit_id,
    u_display.name AS display_unit_name,
    ps.display_quantity,
    p.cost_price,
    (ps.base_quantity * p.cost_price) AS total_value,
    ps.reorder_level,
    CASE 
        WHEN ps.base_quantity <= ps.reorder_level THEN 'LOW STOCK'
        WHEN ps.base_quantity = 0 THEN 'OUT OF STOCK'
        ELSE 'IN STOCK'
    END AS stock_status
FROM product_stocks ps
JOIN branches b ON ps.branch_id = b.branch_id
JOIN products p ON ps.product_id = p.product_id
LEFT JOIN product_categories pc ON p.category_id = pc.category_id
JOIN units u_base ON ps.base_unit_id = u_base.unit_id
JOIN units u_display ON ps.display_unit_id = u_display.unit_id
WHERE b.is_active = TRUE AND p.is_active = TRUE;

-- View: Sales by Product with Profit
CREATE OR REPLACE VIEW `v_sales_by_product` AS
SELECT 
    p.product_id,
    p.code AS product_code,
    p.name AS product_name,
    pc.name AS category_name,
    COUNT(DISTINCT si.sale_item_id) AS total_transactions,
    SUM(si.quantity) AS total_quantity_sold,
    SUM(si.total_price) AS total_sales,
    SUM(si.discount_amount) AS total_discount,
    SUM(si.total_price - si.discount_amount) AS net_sales,
    SUM(si.quantity * p.cost_price) AS total_cost,
    SUM((si.total_price - si.discount_amount) - (si.quantity * p.cost_price)) AS gross_profit,
    CASE 
        WHEN SUM(si.total_price - si.discount_amount) > 0 
        THEN ((SUM((si.total_price - si.discount_amount) - (si.quantity * p.cost_price))) / SUM(si.total_price - si.discount_amount)) * 100 
        ELSE 0 
    END AS profit_margin_percentage
FROM sale_items si
JOIN sales s ON si.sale_id = s.sale_id
JOIN products p ON si.product_id = p.product_id
LEFT JOIN product_categories pc ON p.category_id = pc.category_id
WHERE s.status = 'COMPLETED'
GROUP BY p.product_id, p.code, p.name, pc.name;

-- View: Ingredient Consumption by Product
CREATE OR REPLACE VIEW `v_ingredient_consumption` AS
SELECT 
    sc.ingredient_product_id,
    p.code AS ingredient_code,
    p.name AS ingredient_name,
    u_base.name AS base_unit_name,
    SUM(sc.base_quantity_consumed) AS total_consumed,
    COUNT(DISTINCT sc.sale_id) AS number_of_sales,
    MIN(sc.consumed_at) AS first_consumed,
    MAX(sc.consumed_at) AS last_consumed
FROM sales_consumption sc
JOIN products p ON sc.ingredient_product_id = p.product_id
JOIN units u_base ON sc.base_unit_id = u_base.unit_id
GROUP BY sc.ingredient_product_id, p.code, p.name, u_base.name;

-- View: Daily Sales Summary by Branch
CREATE OR REPLACE VIEW `v_daily_sales_by_branch` AS
SELECT 
    s.branch_id,
    b.name AS branch_name,
    DATE(s.sale_date) AS sale_date,
    COUNT(DISTINCT s.sale_id) AS total_transactions,
    COUNT(DISTINCT s.customer_id) AS unique_customers,
    SUM(s.total_amount) AS gross_sales,
    SUM(s.discount_amount) AS total_discount,
    SUM(s.tax_amount) AS total_tax,
    SUM(s.total_amount - s.discount_amount) AS net_sales,
    SUM(s.paid_amount) AS total_collected,
    SUM(CASE WHEN pm.code = 'CASH' THEN sp.amount ELSE 0 END) AS cash_sales,
    SUM(CASE WHEN pm.code IN ('CARD', 'DEBIT') THEN sp.amount ELSE 0 END) AS card_sales,
    SUM(CASE WHEN pm.code = 'WALLET' THEN sp.amount ELSE 0 END) AS digital_wallet_sales,
    SUM(CASE WHEN pm.code = 'BANK' THEN sp.amount ELSE 0 END) AS bank_sales,
    SUM(CASE WHEN pm.code = 'CHECK' THEN sp.amount ELSE 0 END) AS check_sales,
    SUM(CASE WHEN pm.code = 'GIFT' THEN sp.amount ELSE 0 END) AS gift_card_sales
FROM sales s
JOIN branches b ON s.branch_id = b.branch_id
LEFT JOIN sale_payments sp ON s.sale_id = sp.sale_id
LEFT JOIN payment_methods pm ON sp.payment_method_id = pm.payment_method_id
WHERE s.status = 'COMPLETED'
GROUP BY s.branch_id, b.name, DATE(s.sale_date);

-- View: Low Stock Alert
CREATE OR REPLACE VIEW `v_low_stock_alert` AS
SELECT 
    ps.stock_id,
    b.branch_id,
    b.name AS branch_name,
    p.product_id,
    p.code AS product_code,
    p.name AS product_name,
    ps.base_quantity,
    u.name AS unit_name,
    ps.reorder_level,
    (ps.reorder_level - ps.base_quantity) AS quantity_below_reorder,
    CASE 
        WHEN ps.base_quantity = 0 THEN 'OUT OF STOCK'
        WHEN ps.base_quantity <= ps.reorder_level THEN 'LOW STOCK'
        ELSE 'OK'
    END AS alert_level
FROM product_stocks ps
JOIN branches b ON ps.branch_id = b.branch_id
JOIN products p ON ps.product_id = p.product_id
JOIN units u ON ps.base_unit_id = u.unit_id
WHERE ps.base_quantity <= ps.reorder_level OR ps.base_quantity = 0
ORDER BY ps.base_quantity ASC;

-- View: Profit by Branch
CREATE OR REPLACE VIEW `v_profit_by_branch` AS
SELECT 
    s.branch_id,
    b.name AS branch_name,
    DATE(s.sale_date) AS sale_date,
    COUNT(DISTINCT s.sale_id) AS total_sales,
    SUM(s.total_amount) AS gross_revenue,
    SUM(s.discount_amount) AS total_discounts,
    SUM(s.tax_amount) AS total_tax,
    SUM(s.total_amount - s.discount_amount) AS net_revenue,
    SUM(si.quantity * p.cost_price) AS total_cost_of_goods,
    SUM((s.total_amount - s.discount_amount) - (si.quantity * p.cost_price)) AS gross_profit,
    CASE 
        WHEN SUM(s.total_amount - s.discount_amount) > 0 
        THEN ((SUM((s.total_amount - s.discount_amount) - (si.quantity * p.cost_price))) / SUM(s.total_amount - s.discount_amount)) * 100 
        ELSE 0 
    END AS profit_margin_percentage
FROM sales s
JOIN branches b ON s.branch_id = b.branch_id
JOIN sale_items si ON s.sale_id = si.sale_id
JOIN products p ON si.product_id = p.product_id
WHERE s.status = 'COMPLETED'
GROUP BY s.branch_id, b.name, DATE(s.sale_date);

