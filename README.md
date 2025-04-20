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

## Development Status

This project is currently in the planning and initial development phase. The API discovery phase has been completed, confirming that the LubeLogger API uses Basic Authentication and provides comprehensive endpoints for vehicle and maintenance record management.

## Project Documentation

- [Planning Document](PLANNING.md): Detailed architecture and development plan
- [Task List](TASK.md): Current development tasks and progress

## Prerequisites

- Windows 10/11
- .NET 6 Runtime
- Valid LubeLogger API credentials

## Getting Started

*Detailed setup and usage instructions will be added as development progresses.*

## License

*License information to be determined*