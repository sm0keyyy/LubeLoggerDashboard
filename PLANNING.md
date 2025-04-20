# LubeLogger Dashboard - Windows Client Application

## Project Overview

The LubeLogger Dashboard is a native Windows desktop application that serves as an alternative user interface for an existing LubeLogger instance. This application integrates with LubeLogger by connecting to and utilizing its API, without replicating the backend logic or primary data storage.

## API Information

The LubeLogger API exists at: https://lubelogger.the-freed.family/api
A demo instance is available at: https://demo.lubelogger.com/api

Initial testing confirms that the API requires authentication (received 401 Unauthorized error). This validates our assumption that the API exists, but highlights that proper authentication will be a critical component of our integration.

The API uses Basic Authentication with username and password credentials.

## Core Requirements

Before proceeding with full development, we must investigate and confirm:

- ✅ API documentation and available endpoints (Documented via Postman collection)
- ✅ Authentication method (Basic Auth with username/password)
- ✅ Data formats and structures (Form data for POST/PUT, JSON responses)
- [ ] API stability and versioning policies
- [ ] Rate limits and other constraints

## Architecture

### High-Level Architecture (MVVM Pattern)

The application will follow the Model-View-ViewModel (MVVM) architecture pattern, which is well-suited for Windows desktop applications and provides a clean separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                     Windows Client App                       │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────┐  │
│  │             │    │             │    │                 │  │
│  │    Views    │◄───┤ ViewModels  │◄───┤   API Client    │◄─┼──► LubeLogger API
│  │             │    │             │    │                 │  │
│  └─────────────┘    └─────────────┘    └─────────────────┘  │
│                                                             │
│  ┌─────────────────────────────────────────────────────────┐│
│  │                  Local Data Cache                        ││
│  └─────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────┘
```

### Components

1. **Views Layer**
   - Windows UI components (XAML-based)
   - Responsible for user interaction and data display
   - Binds to ViewModels for data and commands

2. **ViewModels Layer**
   - Contains the presentation logic
   - Transforms model data for view consumption
   - Handles user interactions via commands
   - Manages view state (loading, error states, etc.)

3. **API Client Layer**
   - Handles all communication with the LubeLogger API
   - Manages authentication and credentials
   - Serializes/deserializes API data
   - Implements retry logic and error handling

4. **Local Data Cache**
   - Stores frequently accessed data for performance
   - Enables limited offline functionality
   - Implements synchronization with the API

## Technology Stack

### Primary Technologies

- **Language & Framework**: C# with .NET 6+ and WPF (Windows Presentation Foundation)
  - Provides native Windows UI capabilities
  - Strong typing and modern language features
  - Extensive ecosystem of libraries

- **UI Framework**: WPF with Material Design or Fluent UI
  - XAML-based UI definition
  - Data binding support
  - Modern styling and controls

### Key Libraries

- **HTTP Client**: `HttpClient` with `System.Net.Http`
  - Built-in .NET library for HTTP requests
  - Supports modern HTTP features

- **JSON Handling**: `System.Text.Json`
  - Modern, high-performance JSON library
  - Native to .NET

- **API Client Generation**: OpenAPI/Swagger tools (if LubeLogger provides OpenAPI specs)
  - Automates API client generation
  - Ensures type safety

- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
  - Manages service dependencies
  - Facilitates testing

- **Logging**: Serilog
  - Structured logging
  - Multiple output targets

- **Secure Storage**: Windows Data Protection API (DPAPI)
  - Securely store API credentials
  - Windows-native encryption

- **Local Cache**: SQLite with Entity Framework Core
  - Lightweight embedded database
  - ORM for type-safe data access

## API Integration Requirements

### Core Functionalities & Confirmed Endpoints

1. **Authentication**
   - Basic Authentication with username and password

2. **User Information**
   - `GET /api/whoami` - Get current user information

3. **Vehicle Management**
   - `GET /api/vehicles` - List all vehicles
   - `GET /api/vehicle/info` - Get vehicle details (with optional vehicleId parameter)

4. **Odometer Records**
   - `GET /api/vehicle/odometerrecords` - Get odometer records for a vehicle
   - `GET /api/vehicle/odometerrecords/latest` - Get latest odometer record
   - `GET /api/vehicle/adjustedodometer` - Get adjusted odometer
   - `POST /api/vehicle/odometerrecords/add` - Add odometer record
   - `PUT /api/vehicle/odometerrecords/update` - Update odometer record
   - `DELETE /api/vehicle/odometerrecords/delete` - Delete odometer record

5. **Plan Records**
   - `GET /api/vehicle/planrecords` - Get plan records for a vehicle
   - `POST /api/vehicle/planrecords/add` - Add plan record
   - `PUT /api/vehicle/planrecords/update` - Update plan record
   - `DELETE /api/vehicle/planrecords/delete` - Delete plan record

6. **Service Records**
   - `GET /api/vehicle/servicerecords` - Get service records for a vehicle
   - `POST /api/vehicle/servicerecords/add` - Add service record
   - `PUT /api/vehicle/servicerecords/update` - Update service record
   - `DELETE /api/vehicle/servicerecords/delete` - Delete service record

7. **Repair Records**
   - `GET /api/vehicle/repairrecords` - Get repair records for a vehicle
   - `POST /api/vehicle/repairrecords/add` - Add repair record
   - `PUT /api/vehicle/repairrecords/update` - Update repair record
   - `DELETE /api/vehicle/repairrecords/delete` - Delete repair record

8. **Upgrade Records**
   - `GET /api/vehicle/upgraderecords` - Get upgrade records for a vehicle
   - `POST /api/vehicle/upgraderecords/add` - Add upgrade record
   - `PUT /api/vehicle/upgraderecords/update` - Update upgrade record
   - `DELETE /api/vehicle/upgraderecords/delete` - Delete upgrade record

9. **Tax Records**
   - `GET /api/vehicle/taxrecords` - Get tax records for a vehicle
   - `POST /api/vehicle/taxrecords/add` - Add tax record
   - `PUT /api/vehicle/taxrecords/update` - Update tax record
   - `DELETE /api/vehicle/taxrecords/delete` - Delete tax record

10. **Gas Records**
    - `GET /api/vehicle/gasrecords` - Get gas records for a vehicle
    - `POST /api/vehicle/gasrecords/add` - Add gas record
    - `PUT /api/vehicle/gasrecords/update` - Update gas record
    - `DELETE /api/vehicle/gasrecords/delete` - Delete gas record

11. **Calendar**
    - `GET /api/calendar` - Get calendar data

12. **Document Management**
    - `POST /api/documents/upload` - Upload documents

13. **Reminders**
    - `GET /api/vehicle/reminders` - Get reminders for a vehicle
    - `GET /api/vehicle/reminders/send` - Send reminders email

14. **System Operations**
    - `GET /api/makebackup` - Create a backup
    - `GET /api/cleanup` - Clean up the system

### Data Formats

The API uses different data formats for requests and responses:

1. **Requests**:
   - GET requests use query parameters (e.g., `?vehicleId=1`)
   - POST and PUT requests use form data
   - DELETE requests use query parameters

2. **Responses**:
   - JSON format for data responses

Example data structures based on the Postman collection:

```json
// Vehicle
{
  "id": "number",
  "name": "string",
  "make": "string",
  "model": "string",
  "year": "number",
  "vin": "string",
  "licensePlate": "string",
  "currentMileage": "number",
  "notes": "string"
}

// Odometer Record
{
  "id": "number",
  "vehicleId": "number",
  "date": "string (MM/DD/YYYY)",
  "odometer": "number",
  "initialOdometer": "number",
  "notes": "string",
  "extrafields": [
    {
      "name": "string",
      "value": "string"
    }
  ]
}

// Plan Record
{
  "id": "number",
  "vehicleId": "number",
  "date": "string (MM/DD/YYYY)",
  "description": "string",
  "cost": "number",
  "type": "string (ServiceRecord, RepairRecord, UpgradeRecord)",
  "priority": "string (Low, Normal, Critical)",
  "progress": "string (Backlog, InProgress, Testing)"
}

// Service/Repair/Upgrade Record
{
  "id": "number",
  "vehicleId": "number",
  "date": "string (MM/DD/YYYY)",
  "odometer": "number",
  "description": "string",
  "cost": "number"
}

// Tax Record
{
  "id": "number",
  "vehicleId": "number",
  "date": "string (MM/DD/YYYY)",
  "description": "string",
  "cost": "number"
}

// Gas Record
{
  "id": "number",
  "vehicleId": "number",
  "date": "string (MM/DD/YYYY)",
  "odometer": "number",
  "fuelconsumed": "number",
  "isfilltofull": "boolean",
  "missedfuelup": "boolean",
  "cost": "number",
  "notes": "string"
}
```

### Authentication & Security

1. **Authentication Methods**:
   - Basic Authentication (username/password)
   - Credentials passed in the Authorization header

2. **Credential Storage**:
   - Use Windows Credential Manager or DPAPI
   - Never store credentials in plain text
   - Implement secure credential input and storage

3. **Security Considerations**:
   - HTTPS for all API communication
   - Validate server certificates
   - Implement proper error handling that doesn't expose sensitive information

## Data Management Strategy

### Local Caching

1. **Cache Scope**:
   - Vehicle list and details
   - Recent records (odometer, service, repair, upgrade, tax, gas)
   - Active reminders
   - User preferences

2. **Cache Strategy**:
   - Time-based expiration (configurable)
   - Manual refresh option
   - Background synchronization

3. **Offline Support**:
   - Read-only access to cached data
   - Queue modifications for sync when online
   - Clear visual indication of offline mode

### Data Synchronization

1. **Sync Triggers**:
   - Application startup
   - Manual refresh
   - Periodic background sync
   - After network reconnection

2. **Conflict Resolution**:
   - Server-wins strategy for simplicity
   - Notify user of conflicts
   - Option to merge or overwrite

## UI/UX Design

### Key Principles

1. **Native Windows Look & Feel**:
   - Follow Windows design guidelines
   - Support light/dark mode
   - Responsive to different screen sizes

2. **Dashboard-Oriented**:
   - Focus on data visualization
   - Quick access to common tasks
   - Customizable views

3. **Offline-Aware**:
   - Clear indicators of connection status
   - Graceful degradation when offline

### Primary Views

1. **Dashboard**:
   - Overview of all vehicles
   - Upcoming maintenance reminders
   - Recent service activities
   - Quick stats (total costs, services, etc.)

2. **Vehicle Management**:
   - List of vehicles with key details
   - Vehicle details view
   - Add/edit vehicle forms

3. **Record Management**:
   - Tabs for different record types (odometer, service, repair, upgrade, tax, gas)
   - Record history by vehicle
   - Record details view
   - Add/edit record forms

4. **Maintenance Reminders**:
   - List of upcoming reminders
   - Reminder details
   - Add/edit reminder forms

5. **Reports & Analytics**:
   - Cost breakdown charts
   - Service frequency analysis
   - Maintenance prediction

6. **Calendar View**:
   - Timeline of all maintenance activities
   - Filter by vehicle and record type

7. **System Operations**:
   - Document upload interface
   - Backup creation
   - System cleanup

### UI States

1. **Loading States**:
   - Progress indicators for API operations
   - Skeleton screens for initial loads

2. **Error States**:
   - User-friendly error messages
   - Retry options
   - Fallback to cached data when appropriate

3. **Empty States**:
   - Helpful guidance for new users
   - Clear calls to action

## Development Workflow & Milestones

### Phase 0: API Discovery & Setup (2 weeks)
- ✅ Investigate LubeLogger API capabilities
- ✅ Determine authentication method (Basic Auth)
- ✅ Document available endpoints
- [ ] Set up development environment
- [ ] Create project structure
- [ ] Implement authentication mechanism

### Phase 1: Core Framework (3 weeks)
- [ ] Implement API client framework
- [ ] Set up MVVM architecture
- [ ] Create navigation infrastructure
- [ ] Implement local caching system
- [ ] Set up secure credential storage

### Phase 2: Vehicle Management (2 weeks)
- [ ] Implement vehicle listing
- [ ] Create vehicle details view
- [ ] Add vehicle editing capabilities
- [ ] Implement vehicle search and filtering

### Phase 3: Record Management (4 weeks)
- [ ] Implement odometer records management
- [ ] Implement service records management
- [ ] Implement repair records management
- [ ] Implement upgrade records management
- [ ] Implement tax records management
- [ ] Implement gas records management
- [ ] Implement plan records management

### Phase 4: Reminders & Notifications (2 weeks)
- [ ] Implement reminder system
- [ ] Create reminder UI
- [ ] Add Windows notifications integration
- [ ] Implement reminder filtering and sorting

### Phase 5: Reports & Analytics (3 weeks)
- [ ] Implement basic reporting
- [ ] Create data visualization components
- [ ] Add cost analysis features
- [ ] Implement maintenance predictions

### Phase 6: System Operations (2 weeks)
- [ ] Implement document upload functionality
- [ ] Create backup creation interface
- [ ] Implement system cleanup functionality
- [ ] Add user preference settings

## Risks & Challenges

### API Limitations
- **Risk**: LubeLogger API may not provide all needed functionality
- **Mitigation**: Early API investigation, flexible architecture to accommodate limitations

### Authentication Challenges
- **Risk**: Complex or changing authentication requirements
- **Mitigation**: Modular authentication system, fallback mechanisms

### Network Reliability
- **Risk**: Users may have intermittent connectivity
- **Mitigation**: Robust offline capabilities, graceful error handling

### API Changes
- **Risk**: LubeLogger API may change unexpectedly
- **Mitigation**: Version pinning if available, abstraction layer to isolate changes

### Performance
- **Risk**: Slow API responses affecting user experience
- **Mitigation**: Aggressive caching, background loading, optimistic UI updates

## Testing Strategy

### Unit Testing
- ViewModels and business logic
- API client mocking
- Data transformation

### Integration Testing
- API client against mock server
- Cache synchronization
- Authentication flow

### UI Testing
- Automated UI tests for critical paths
- Manual testing for UX validation

### Offline Testing
- Simulate network interruptions
- Verify data integrity during sync

## Deployment Strategy

### Distribution Methods
- MSIX package for Microsoft Store
- Direct download installer
- Optional auto-update mechanism

### System Requirements
- Windows 10/11
- .NET 6 runtime
- 4GB RAM minimum
- 100MB disk space

## Conclusion

This Windows client application will provide a native, responsive interface to LubeLogger data while leveraging the existing backend API. The MVVM architecture ensures separation of concerns, making the application maintainable and testable. The local caching strategy balances performance with data freshness, while providing limited offline capabilities.

The API discovery phase has been completed, confirming that the LubeLogger API uses Basic Authentication and provides comprehensive endpoints for vehicle and maintenance record management. This information will guide the implementation of the API client layer and overall application architecture.