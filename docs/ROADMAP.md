# EnterprisePOS Implementation Roadmap

> Enterprise cross-platform POS & business management UI — .NET MAUI, MVVM, responsive Shell.

**Last reviewed:** 2026-05-27

## Status legend

| Mark | Meaning |
|------|---------|
| `[x]` | **Done** — in repo and current build |
| `[~]` | **Partial** — started, not complete or not on all platforms |
| `[ ]` | **Not started** |

---

## Platform support (single codebase)

| Platform | TFM | Build | Responsive UI |
|----------|-----|-------|-----------------|
| Windows desktop | `net10.0-windows10.0.19041.0` | `[x]` | `[x]` POS wide/narrow + Shell flyout |
| Android phone | `net10.0-android` | `[x]` | `[~]` TabBar + stacked POS |
| Android tablet | `net10.0-android` | `[x]` | `[~]` Flyout + POS wide layout |
| iOS | `net10.0-ios` | `[~]` | `[~]` same patterns (verify on device) |
| Mac desktop | `net10.0-maccatalyst` | `[~]` | `[~]` same as Windows (verify) |

**Run Windows:** `.\run.ps1` or `bin\Debug\net10.0-windows10.0.19041.0\win-x64\EnterprisePOS.exe`

---

## Folder structure — OK ba?

| Folder | Status | Notes |
|--------|--------|-------|
| `Views/` | `[x]` | `POSPage`, `DashboardHomePage`, module placeholders |
| `ViewModels/` | `[~]` | POS, Settings, Dashboard VMs |
| `Models/` | `[x]` | Product, Cart, Category, Nav |
| `Services/` | `[~]` | Mock POS/Dashboard, Theme, Shell nav |
| `Repositories/` | `[~]` | `InMemoryRepository` stub |
| `Components/` | `[x]` | POS cards, sidebar, category tabs, cart line |
| `Themes/` | `[~]` | Light/Dark/Tokens in repo — **excluded from build** (WinUI) |
| `Helpers/` | `[x]` | BaseViewModel, ServiceHelper, converters, **LayoutBreakpoints** |
| `Interfaces/` | `[~]` | IPos, IDashboard, INavigation, IApi, IRepository |
| `DTOs/` | `[~]` | Folder + README (contracts next) |
| `Validators/` | `[~]` | Folder + README |
| `Configurations/` | `[x]` | `AppSettings.cs` (API/SignalR URLs, mock flag) |
| `Navigation/` | `[x]` | `Routes.cs` |
| `Resources/` | `[x]` | MAUI **Assets** equivalent: Images, Fonts, Styles |
| `docs/` | `[x]` | This roadmap |

**Hindi kailangan hiwalay na `Assets/`** sa MAUI — gamitin `Resources/` (Images, Fonts, Raw, Styles). OK ang structure mo kung `Assets` = `Resources`.

---

## Recommended immediate goals — status

| Goal | Status |
|------|--------|
| Responsive AppShell | `[~]` Flyout/TabBar not enabled right now (POS-only stable due to WinUI crash) |
| Sidebar navigation | `[x]` Shell flyout modules + POS internal `PosSidebarView` |
| Tablet layout | `[x]` POS 3-column ≥900px; flyout collapsible |
| POS screen | `[x]` Reference UI: navbar, categories, grid, cart |
| Mobile bottom navigation | `[ ]` TabBar temporarily disabled (re-enable after stability) |
| Reusable components | `[x]` PosProductCard, PosCartLine, PosCategoryTab, PosSidebar |
| Theme system | `[~]` ThemeService + Settings; full token dictionaries excluded |
| MVVM + DI | `[x]` MauiProgram registrations |
| Scalable folders | `[x]` DTOs, Repositories, Configurations added |

---

## Phase 1 — Foundation & core UI

### 1. Foundation (MVVM-first)

- [x] `BaseViewModel`, `ServiceHelper`
- [x] `INavigationService` + `ShellNavigationService` (registered in DI)
- [~] `IApiService`, `IRepository<T>` (interfaces only)
- [x] DI: POS, Settings, Theme, Dashboard mock, all module pages
- [x] Code-behind: layout breakpoints only (no business logic in POS page)

### 2. Responsive shell & navigation

- [~] `AppShell` — responsive flyout/tabbar to re-enable (POS-only currently stable)
- [x] `LayoutBreakpoints` (599 / 900)
- [~] `ApplyResponsiveChrome` (re-enable after WinUI stability)
- [x] `Routes.cs` + route registration
- [x] POS page: wide (sidebar + catalog + cart) vs narrow (stacked) at **900px**

### 3. Theme system

- [x] `Colors.xaml`, `PosStyles.xaml`, global `Styles.xaml`
- [x] `ThemeService` + Settings dark/light toggle
- [~] `Themes/*.xaml` excluded from build (re-enable when WinUI-safe)
- [ ] Semantic tokens only app-wide

### 4. Reusable components

| Component | Status |
|-----------|--------|
| PosProductCardView | `[x]` |
| PosCartLineView | `[x]` |
| PosCategoryTabView | `[x]` icons + underline |
| PosSidebarView | `[x]` collapsible |
| MetricCardView | `[~]` excluded (Dashboard) |
| Empty state | `[ ]` |

### 5. POS module (active)

- [x] Top navbar: search, compact Create Note, chat/bell, cashier avatar
- [x] Categories: icons, horizontal scroll, centered when few items
- [x] Product grid (responsive span)
- [x] Cart card: Detail Transaction + Reset inside bordered panel
- [x] Cart line: compact remove / + / −
- [x] Promo, summary, payment, Continue
- [x] Mock data + cart logic

### 6. Dashboard

- [x] `DashboardHomePage` placeholder KPI cards (in build)
- [~] Full `DashboardPage` + charts (excluded from build)

### 7. Other modules

- [~] Products, Inventory, Booking, Settings — placeholder pages in Shell

---

## Phase 2–5 (unchanged priorities)

- [ ] Customers, Reports, Users, Notifications, Logs, Branches, Payments
- [ ] API + MySQL + JWT
- [ ] SignalR realtime
- [ ] Offline sync, hardware (printer, scanner, drawer)
- [ ] Validation, logging, paging, accessibility pass

---

## Sprint order

| Sprint | Focus | Status |
|--------|--------|--------|
| **Sprint 1** | Foundation + responsive shell + theme | `[~]` **mostly done** |
| **Sprint 2** | Dashboard + POS mock | `[~]` POS done; Dashboard placeholder |
| **Sprint 3** | Module skeletons | `[~]` in Shell nav |
| **Sprint 4** | API / MySQL | `[ ]` |
| **Sprint 5** | SignalR / offline / hardware | `[ ]` |

---

## What you did right (keep doing)

- MVVM from the start  
- Reusable `Components/`  
- Responsive layouts (`LayoutBreakpoints`, POS wide/narrow)  
- Modular folders (not one giant page)  
- Mock services before API  

## Avoid

- One giant XAML page with all modules  
- Logic in code-behind  
- Hardcoded colors everywhere (migrate to tokens)  

---

## Next steps (recommended order)

1. Test on **Android emulator** (phone + tablet width) — TabBar + POS narrow layout  
2. Re-enable **DashboardPage** / **MetricCardView** safely on Windows  
3. Wire **POS internal sidebar** → Shell `GoToAsync` (remove duplicate nav)  
4. **Barcode** entry on POS search  
5. Phase 3: **DTOs** + API client implementation  

---

## How to update this doc

1. Change `[ ]` → `[x]` or `[~]`.  
2. Update **Last reviewed** date.  
3. Adjust **Next steps** when priorities change.
