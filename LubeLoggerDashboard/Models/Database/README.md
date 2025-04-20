# LubeLogger Dashboard - Local Caching System

This directory contains the design and implementation documentation for the LubeLogger Dashboard application's local caching system using SQLite and Entity Framework Core.

## Overview

The local caching system enables the LubeLogger Dashboard application to:
- Store frequently accessed data locally for improved performance
- Provide limited offline functionality
- Reduce server load by minimizing API calls
- Synchronize local changes with the server

## Documentation Structure

### 1. [Database Schema](./DatabaseSchema.md)

Comprehensive overview of the SQLite database schema, including:
- Entity class definitions
- Relationships between entities
- DbContext configuration
- Property requirements and constraints

This document serves as the primary reference for the database structure.

### 2. [Implementation Guide](./Implementation.md)

Detailed implementation instructions for the local caching system, including:
- Complete code examples for all entity classes
- DbContext implementation
- Repository pattern implementation
- Cache and sync service implementations
- Usage examples

This document provides the practical implementation details based on the schema design.

### 3. [Design Decisions](./DesignDecisions.md)

Explanation of key design decisions and their rationale, including:
- Base entity approach for cache metadata
- Entity relationships and cascading deletes
- JSON storage for dynamic fields
- Sync status tracking
- Offline-first approach
- SQLite with Entity Framework Core

This document helps understand why specific design choices were made.

### 4. [Migration Strategy](./MigrationStrategy.md)

Strategy for managing database migrations, including:
- Initial setup approach
- Ongoing migration management
- Implementation details for migrations
- Handling schema and data migrations
- Backup and recovery processes
- Best practices for migrations

This document guides the evolution of the database schema over time.

### 5. [Caching Strategy](./CachingStrategy.md)

Comprehensive caching strategy, including:
- Time-based expiration policies
- Cache refresh triggers
- Offline-first approach
- Synchronization strategy
- Implementation patterns
- Performance considerations
- Testing strategy

This document outlines how data is cached, refreshed, and synchronized.

## Implementation Status

- [x] Database schema design
- [x] Design decisions documentation
- [x] Migration strategy
- [x] Caching strategy
- [ ] Entity classes implementation
- [ ] DbContext implementation
- [ ] Database initializer
- [ ] Cache service implementation
- [ ] Sync service implementation
- [ ] Repository implementations
- [ ] Integration with API services

## Next Steps

1. Implement the entity classes and DbContext based on the schema design
2. Create the database initializer to set up the database
3. Implement the cache service for managing cache operations
4. Implement the sync service for synchronization with the server
5. Create repositories for each entity type
6. Integrate with existing API services
7. Add unit and integration tests

## Dependencies

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Design (for migrations)
- Microsoft.Extensions.Configuration (for configuration)

## Usage

The local caching system is used by the application's repositories to provide data access with caching capabilities. The repositories abstract the caching logic from the ViewModels, providing a clean API for data operations.

Example usage in a ViewModel:

```csharp
public class VehicleListViewModel : ViewModelBase
{
    private readonly IVehicleRepository _vehicleRepository;
    
    public VehicleListViewModel(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }
    
    public async Task LoadVehiclesAsync(bool forceRefresh = false)
    {
        IsLoading = true;
        
        try
        {
            var vehicles = await _vehicleRepository.GetAllVehiclesAsync(forceRefresh);
            Vehicles = new ObservableCollection<VehicleViewModel>(
                vehicles.Select(v => new VehicleViewModel(v)));
        }
        catch (Exception ex)
        {
            // Handle error
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

## Conclusion

The local caching system is a critical component of the LubeLogger Dashboard application, enabling improved performance and offline capabilities. The design prioritizes a balance between data freshness, performance, and user experience, with careful consideration of the specific requirements of the application.