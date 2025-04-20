# Dependency Injection System

## Overview

The LubeLogger Dashboard application uses a dependency injection (DI) system to manage service dependencies and promote loose coupling between components. This document explains how the DI system is set up and how to use it.

## Key Components

### 1. ServiceCollectionExtensions

The `ServiceCollectionExtensions` class provides extension methods for registering all application services with the DI container:

- `AddApplicationServices()`: Registers all application services
- `AddApiServices()`: Registers only the API-related services

### 2. ServiceLocator

The `ServiceLocator` class provides a static way to access services from anywhere in the application:

- `GetService<T>()`: Gets a service of the specified type
- `TryGetService<T>(out T service)`: Tries to get a service of the specified type

### 3. AuthenticationServiceFactory

The `AuthenticationServiceFactory` class resolves the circular dependency between `IApiClient` and `IAuthenticationService`:

- `CreateAuthenticationService()`: Creates an authentication service with the required dependencies

## Service Registration

Services are registered in the `ConfigureServices` method of the `App` class:

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // Register all application services
    services.AddApplicationServices();
}
```

## Service Lifetimes

All services are registered with a singleton lifetime, meaning that the same instance is used throughout the application. This is appropriate for most services in a desktop application.

## Resolving Services

Services can be resolved in two ways:

### 1. Constructor Injection

The preferred way to resolve services is through constructor injection:

```csharp
public class MyClass
{
    private readonly IMyService _myService;

    public MyClass(IMyService myService)
    {
        _myService = myService;
    }
}
```

### 2. Service Locator

For cases where constructor injection is not possible, the `ServiceLocator` can be used:

```csharp
var myService = ServiceLocator.GetService<IMyService>();
```

## Adding New Services

To add a new service:

1. Define the service interface in the appropriate namespace
2. Implement the service class
3. Register the service in the `ServiceCollectionExtensions.AddApplicationServices()` method

Example:

```csharp
// Register in ServiceCollectionExtensions.cs
services.AddSingleton<IMyService, MyService>();
```

## Testing

The DI system is tested in the `DependencyInjectionTests` class, which verifies that all services can be resolved correctly.

## Circular Dependencies

The application handles circular dependencies using factories. For example, the circular dependency between `IApiClient` and `IAuthenticationService` is resolved using the `AuthenticationServiceFactory`.