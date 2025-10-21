# AppSettings Configuration

This document describes the configuration structure for the enhanced SharedLibrary.

## Basic Configuration Structure

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=localhost;Database=MyDatabase;Username=myuser;Password=mypassword",
    "RedisConnection": "localhost:6379"
  },
  "Authentication": {
    "Issuer": "MyApp",
    "Audience": "MyAppUsers",
    "Key": "ThisIsMySuperSecretKeyThatShouldBeVeryLongAndSecure",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "RateLimiting": {
    "Limit": 100,
    "Window": 60
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## Connection Strings

### PostgresConnection
The connection string for your PostgreSQL database.

Format: `Host=localhost;Database=MyDatabase;Username=myuser;Password=mypassword`

### RedisConnection
The connection string for your Redis instance (optional).

Format: `localhost:6379` or `redis.example.com:6379,password=yourpassword`

## Authentication Settings

### Issuer
The JWT token issuer identifier.

### Audience
The JWT token audience identifier.

### Key
The secret key used to sign JWT tokens. This should be a long, secure string.

### AccessTokenExpirationMinutes
The number of minutes before access tokens expire. Default: 15

### RefreshTokenExpirationDays
The number of days before refresh tokens expire. Default: 7

## Rate Limiting Settings

### Limit
The maximum number of requests allowed per time window. Default: 100

### Window
The time window in seconds. Default: 60

## Serilog Configuration

The Serilog configuration follows the standard Serilog configuration structure.

### MinimumLevel
The minimum log level to output. Options: Verbose, Debug, Information, Warning, Error, Fatal

### WriteTo
An array of sinks to write logs to.

#### Console
Outputs logs to the console.

#### File
Outputs logs to a file with rolling intervals.

## Example Complete Configuration

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=localhost;Database=MyMicroservice;Username=postgres;Password=postgres",
    "RedisConnection": "localhost:6379"
  },
  "Authentication": {
    "Issuer": "MyMicroservice",
    "Audience": "MyMicroserviceUsers",
    "Key": "ThisIsAVeryLongAndSecureSecretKeyForJWTSigning1234567890",
    "AccessTokenExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 30
  },
  "RateLimiting": {
    "Limit": 1000,
    "Window": 3600
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/microservice-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} CorrelationId: {CorrelationId} UserId: {UserId} {NewLine}{Exception}"
        }
      }
    ]
  }
}
```

This configuration provides a solid foundation for a production microservice using the enhanced SharedLibrary.