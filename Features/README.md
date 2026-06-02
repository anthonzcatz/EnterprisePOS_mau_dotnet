# Features

This directory contains feature-based modules for the EnterprisePOS application.

Each feature is a self-contained module with its own:
- Models (domain entities)
- DTOs (data transfer objects)
- Repositories (data access)
- Services (business logic)
- Validators (input validation)
- ViewModels (MVVM view models)
- Views (UI pages)

## Current Features

- **POS** - Point of Sale module for handling transactions, cart management, and product selection

## Adding a New Feature

1. Create a new folder under `Features/` (e.g., `Features/Products/`)
2. Create the standard subfolders: `Models/`, `DTOs/`, `Repositories/`, `Services/`, `Validators/`, `ViewModels/`, `Views/`
3. Add a README.md in the feature folder describing its purpose
4. Register services in `MauiProgram.cs`
5. Add navigation routes in `AppShell.xaml`

## Benefits

- **Scalability**: Easy to add new features without cluttering existing code
- **Maintainability**: All related files for a feature are in one place
- **Team Collaboration**: Different developers can work on different features independently
- **Easy Refactoring**: Moving or removing features is straightforward
