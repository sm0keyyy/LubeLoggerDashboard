# LubeLogger Dashboard - Development Tasks

## Created: 4/20/2025

## Phase 0: API Discovery & Setup

- [x] Confirm LubeLogger API exists at a demo instance is available at: https://demo.lubelogger.com/api
- [x] Verify API requires authentication (401 Unauthorized response)
- [x] Determine specific authentication method required (Basic Auth)
- [x] Obtain valid API credentials for development
- [x] Document available endpoints and response formats
- [x] Set up development environment with .NET 6+ and WPF
- [x] Create project structure following MVVM architecture
- [x] Implement authentication mechanism based on API requirements
- [x] Create secure credential storage using Windows DPAPI

## Phase 1: Core Framework

- [x] Implement API client base framework with:
  - [x] API stability and versioning policies
  - [x] Rate limit handling
  - [x] Circuit breaker pattern for fault tolerance
  - [x] Feature detection capabilities
  - [x] Health check mechanism
  - [x] Payload size validation
- [x] Create API service interfaces for each resource type
- [x] Set up dependency injection system
- [x] Implement logging infrastructure using Serilog
- [x] Create navigation infrastructure and main window shell
  - [x] Implement the NavigationService class
  - [x] Implement the ViewFactory class
  - [x] Create unit tests for NavigationService and ViewFactory
  - [x] Update MainWindow to include navigation frame
  - [x] Create ShellViewModel
- [ ] Implement local caching system using SQLite and EF Core
  - [x] Design database schema for entity classes
  - [x] Document design decisions and rationale
  - [x] Create migration strategy
  - [x] Define caching strategy
  - [x] Implement entity classes and DbContext
  - [ ] Create database initializer
  - [ ] Implement cache service
  - [ ] Implement sync service
- [x] Create base ViewModel classes and common utilities

## Phase 2: Vehicle Management

- [ ] Implement vehicle listing view and ViewModel
- [ ] Create vehicle details view and ViewModel
- [ ] Implement vehicle creation/editing functionality
- [ ] Add vehicle search and filtering capabilities
- [ ] Create vehicle deletion with confirmation
- [ ] Implement vehicle data caching and offline access

## Phase 3: Record Management

- [ ] Implement odometer records management
- [ ] Implement service records management
- [ ] Implement repair records management
- [ ] Implement upgrade records management
- [ ] Implement tax records management
- [ ] Implement gas records management
- [ ] Implement plan records management
- [ ] Create common record listing interface
- [ ] Implement record filtering and sorting
- [ ] Add record creation/editing functionality
- [ ] Implement record deletion with confirmation
- [ ] Create attachment handling for records

## Phase 4: Reminders & Notifications

- [ ] Implement maintenance reminder system
- [ ] Create reminder listing view and ViewModel
- [ ] Implement reminder creation/editing functionality
- [ ] Add time-based and mileage-based reminder types
- [ ] Implement Windows notification integration
- [ ] Create reminder filtering and sorting options
- [ ] Add reminder snoozing and dismissal functionality
- [ ] Implement email notification integration

## Phase 5: Reports & Analytics

- [ ] Implement basic reporting infrastructure
- [ ] Create cost analysis reports and visualizations
- [ ] Implement service frequency analysis
- [ ] Add maintenance prediction based on history
- [ ] Create exportable reports (PDF, CSV)
- [ ] Implement dashboard widgets for key metrics
- [ ] Add customizable reporting periods
- [ ] Create calendar view for maintenance history

## Phase 6: System Operations

- [ ] Implement document upload functionality
- [ ] Create backup creation interface
- [ ] Implement system cleanup functionality
- [ ] Add user preference settings
- [ ] Implement light/dark theme support
- [ ] Create help documentation and tooltips
- [ ] Add keyboard shortcuts and accessibility features
- [ ] Enhance offline capabilities and sync

## Testing Tasks

- [ ] Create unit tests for ViewModels
- [ ] Implement integration tests for API client
- [ ] Create mock server for testing
- [ ] Implement UI automation tests for critical paths
- [ ] Test offline functionality and sync
- [ ] Perform performance testing with large datasets

## Deployment Tasks

- [ ] Create MSIX package for Microsoft Store
- [ ] Build installer for direct download
- [ ] Implement auto-update mechanism
- [ ] Create deployment documentation
- [ ] Prepare release notes template

## Discovered During Work

- [x] API exists at https://lubelogger.the-freed.family/api
- [x] API requires authentication (401 Unauthorized response)
- [x] API uses Basic Authentication (username/password)
- [x] API base URL for demo is https://demo.lubelogger.com
- [x] API endpoints documented in Postman collection:
  - User: /api/whoami
  - Vehicles: /api/vehicles, /api/vehicle/info
  - Odometer Records: /api/vehicle/odometerrecords, /api/vehicle/odometerrecords/latest, /api/vehicle/adjustedodometer
  - Plan Records: /api/vehicle/planrecords
  - Service Records: /api/vehicle/servicerecords
  - Repair Records: /api/vehicle/repairrecords
  - Upgrade Records: /api/vehicle/upgraderecords
  - Tax Records: /api/vehicle/taxrecords
  - Gas Records: /api/vehicle/gasrecords
  - Calendar: /api/calendar
  - Documents: /api/documents/upload
  - Reminders: /api/vehicle/reminders
  - System: /api/makebackup, /api/cleanup
- [x] API stability and versioning policies:
  - No explicit versioning in URL or headers observed
  - Implement version parameter in client for future-proofing
  - Use feature detection rather than version checking
  - Add abstraction layer to isolate API changes
- [x] Rate limits and other constraints:
  - Implement monitoring for rate limit headers (X-RateLimit-*)
  - Add request throttling based on rate limit information
  - Implement circuit breaker pattern for API outages
  - Add retry logic with exponential backoff for transient failures
- [x] Created .gitignore file with appropriate patterns for C# WPF project
- [x] Removed unnecessary .md files from repository (keeping only README.md, TASK.md, and PLANNING.md)
- [x] Recreated API_GUIDELINES.md with comprehensive API usage documentation
- [x] Recreated LubeLoggerDashboard/Docs/DependencyInjection.md with DI system documentation