# LubeLogger Dashboard

A native Windows desktop application that serves as an alternative user interface for an existing LubeLogger instance.

## Project Overview

LubeLogger Dashboard is a Windows client application that connects to and utilizes the LubeLogger API to provide a native desktop experience for managing vehicle maintenance records. This application does not replicate LubeLogger's backend logic or primary data storage, but instead serves as an alternative interface to the existing system.

## Key Features (Planned)

- **Vehicle Management**: View, add, edit, and manage your vehicles
- **Maintenance Records**: Track various record types:
  - Odometer readings
  - Service records
  - Repair records
  - Upgrade records
  - Tax records
  - Gas/fuel records
  - Planned maintenance
- **Maintenance Reminders**: Set and receive notifications for upcoming maintenance
- **Reports & Analytics**: Visualize maintenance history and costs
- **Calendar View**: Timeline of all maintenance activities
- **Document Management**: Upload and manage maintenance-related documents
- **System Operations**: Create backups and perform system maintenance
- **Offline Support**: Limited functionality when disconnected from the network

## Technical Details

- **Platform**: Windows 10/11
- **Framework**: .NET 6+ with WPF
- **Architecture**: MVVM (Model-View-ViewModel)
- **API Integration**: Connects to LubeLogger API
  - Demo instance: https://demo.lubelogger.com/api
  - Authentication: Basic Auth (username/password)
- **Local Caching**: SQLite with Entity Framework Core
  - Enables offline functionality
  - Implements synchronization with the API
- **Secure Credential Storage**: Windows DPAPI for secure credential management

## Development Status

This project is currently in active development. Here's the current status:

- ✅ **API Discovery Phase**: Completed - The API has been thoroughly investigated, and all necessary endpoints have been documented.
- ✅ **Core Framework**: Completed - The MVVM architecture, API client framework, navigation infrastructure, and local caching system have been implemented.
- 🔄 **Vehicle Management**: In Progress - Currently implementing vehicle listing, details, and editing functionality.
- ⏳ **Record Management**: Planned - Implementation of various record types management.
- ⏳ **Reminders & Notifications**: Planned - Implementation of maintenance reminder system.
- ⏳ **Reports & Analytics**: Planned - Implementation of reporting and data visualization.
- ⏳ **System Operations**: Planned - Implementation of document upload, backup creation, and system cleanup.

Recent significant updates:
- Implemented local caching system using SQLite and Entity Framework Core
- Created entity classes, DbContext, database initializer, cache service, and sync service
- Added support for offline operations with synchronization capabilities

## Project Documentation

- [Planning Document](PLANNING.md): Detailed architecture and development plan
- [Task List](TASK.md): Current development tasks and progress

## Prerequisites

- Windows 10/11
- .NET 6 Runtime
- Valid LubeLogger API credentials

## Getting Started

*Detailed setup and usage instructions will be added as development progresses.*

### For Developers

1. Clone the repository
2. Open the solution in Visual Studio 2022 or later
3. Restore NuGet packages
4. Build the solution
5. Run the application

## License

*License information to be determined*