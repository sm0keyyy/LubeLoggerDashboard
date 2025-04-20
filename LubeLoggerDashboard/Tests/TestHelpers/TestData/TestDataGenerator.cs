using System;
using System.Collections.Generic;
using LubeLoggerDashboard.Models.Database.Entities;
using LubeLoggerDashboard.Models.Database.Enums;

namespace LubeLoggerDashboard.Tests.TestHelpers.TestData
{
    /// <summary>
    /// Utility class for generating test data for unit and integration tests
    /// </summary>
    public static class TestDataGenerator
    {
        private static readonly Random _random = new Random();
        
        /// <summary>
        /// Generates a random string of the specified length
        /// </summary>
        /// <param name="length">The length of the string to generate</param>
        /// <returns>A random string</returns>
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[_random.Next(chars.Length)];
            }
            
            return new string(stringChars);
        }
        
        /// <summary>
        /// Generates a random VIN (Vehicle Identification Number)
        /// </summary>
        /// <returns>A random VIN</returns>
        public static string GenerateRandomVIN()
        {
            return GenerateRandomString(17).ToUpper();
        }
        
        /// <summary>
        /// Generates a random license plate
        /// </summary>
        /// <returns>A random license plate</returns>
        public static string GenerateRandomLicensePlate()
        {
            return $"{GenerateRandomString(3).ToUpper()}-{_random.Next(100, 999)}";
        }
        
        /// <summary>
        /// Generates a test vehicle with random data
        /// </summary>
        /// <param name="id">Optional ID for the vehicle</param>
        /// <returns>A Vehicle entity with test data</returns>
        public static Vehicle GenerateTestVehicle(int? id = null)
        {
            var makes = new[] { "Toyota", "Honda", "Ford", "Chevrolet", "Nissan", "BMW", "Mercedes", "Audi", "Volkswagen", "Hyundai" };
            var models = new[] { "Camry", "Accord", "F-150", "Silverado", "Altima", "3 Series", "C-Class", "A4", "Jetta", "Sonata" };
            
            return new Vehicle
            {
                Id = id ?? _random.Next(1, 1000),
                Name = $"Test Vehicle {_random.Next(1, 1000)}",
                Make = makes[_random.Next(makes.Length)],
                Model = models[_random.Next(models.Length)],
                Year = _random.Next(2000, 2025),
                VIN = GenerateRandomVIN(),
                LicensePlate = GenerateRandomLicensePlate(),
                Notes = $"Test vehicle generated at {DateTime.UtcNow}",
                ExpirationTimestamp = DateTime.UtcNow.AddHours(24),
                SyncStatus = SyncStatus.Synced,
                LastSyncTimestamp = DateTime.UtcNow.AddHours(-1),
                IsDirty = false
            };
        }
        
        /// <summary>
        /// Generates a list of test vehicles with random data
        /// </summary>
        /// <param name="count">The number of vehicles to generate</param>
        /// <returns>A list of Vehicle entities with test data</returns>
        public static List<Vehicle> GenerateTestVehicles(int count)
        {
            var vehicles = new List<Vehicle>();
            
            for (int i = 1; i <= count; i++)
            {
                vehicles.Add(GenerateTestVehicle(i));
            }
            
            return vehicles;
        }
        
        /// <summary>
        /// Generates a test odometer record with random data
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle for the record</param>
        /// <param name="id">Optional ID for the record</param>
        /// <param name="odometer">Optional odometer value</param>
        /// <returns>An OdometerRecord entity with test data</returns>
        public static OdometerRecord GenerateTestOdometerRecord(int vehicleId, int? id = null, int? odometer = null)
        {
            return new OdometerRecord
            {
                Id = id ?? _random.Next(1, 1000),
                VehicleId = vehicleId,
                Date = DateTime.UtcNow.AddDays(-_random.Next(0, 30)),
                Odometer = odometer ?? _random.Next(1000, 100000),
                Notes = $"Test odometer record generated at {DateTime.UtcNow}",
                ExpirationTimestamp = DateTime.UtcNow.AddHours(12),
                SyncStatus = SyncStatus.Synced,
                LastSyncTimestamp = DateTime.UtcNow.AddHours(-1),
                IsDirty = false
            };
        }
        
        /// <summary>
        /// Generates a list of test odometer records with sequential dates and increasing odometer values
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle for the records</param>
        /// <param name="count">The number of records to generate</param>
        /// <param name="startOdometer">The starting odometer value</param>
        /// <param name="averageMileage">The average mileage between records</param>
        /// <returns>A list of OdometerRecord entities with test data</returns>
        public static List<OdometerRecord> GenerateSequentialOdometerRecords(int vehicleId, int count, int startOdometer = 0, int averageMileage = 1000)
        {
            var records = new List<OdometerRecord>();
            var currentOdometer = startOdometer;
            var currentDate = DateTime.UtcNow.AddDays(-count);
            
            for (int i = 1; i <= count; i++)
            {
                currentOdometer += averageMileage + _random.Next(-200, 200); // Add some randomness to the mileage
                currentDate = currentDate.AddDays(1);
                
                records.Add(new OdometerRecord
                {
                    Id = i,
                    VehicleId = vehicleId,
                    Date = currentDate,
                    Odometer = currentOdometer,
                    Notes = $"Sequential odometer record {i} of {count}",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(12),
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-1),
                    IsDirty = false
                });
            }
            
            return records;
        }
        
        /// <summary>
        /// Generates a test service record with random data
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle for the record</param>
        /// <param name="id">Optional ID for the record</param>
        /// <returns>A ServiceRecord entity with test data</returns>
        public static ServiceRecord GenerateTestServiceRecord(int vehicleId, int? id = null)
        {
            var serviceTypes = new[] { "Oil Change", "Tire Rotation", "Brake Service", "Air Filter", "Transmission Service", "Coolant Flush" };
            
            return new ServiceRecord
            {
                Id = id ?? _random.Next(1, 1000),
                VehicleId = vehicleId,
                Date = DateTime.UtcNow.AddDays(-_random.Next(0, 90)),
                Odometer = _random.Next(1000, 100000),
                Description = serviceTypes[_random.Next(serviceTypes.Length)],
                Cost = (decimal)(_random.Next(2000, 50000) / 100.0), // Random cost between $20 and $500
                ExpirationTimestamp = DateTime.UtcNow.AddHours(12),
                SyncStatus = SyncStatus.Synced,
                LastSyncTimestamp = DateTime.UtcNow.AddHours(-1),
                IsDirty = false
            };
        }
        
        /// <summary>
        /// Generates a list of test service records with random data
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle for the records</param>
        /// <param name="count">The number of records to generate</param>
        /// <returns>A list of ServiceRecord entities with test data</returns>
        public static List<ServiceRecord> GenerateTestServiceRecords(int vehicleId, int count)
        {
            var records = new List<ServiceRecord>();
            
            for (int i = 1; i <= count; i++)
            {
                records.Add(GenerateTestServiceRecord(vehicleId, i));
            }
            
            return records;
        }
    }
}