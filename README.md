# Desk Reservation App Backend

A comprehensive desk reservation system built with **ASP.NET Core 8.0** following **Clean Architecture** principles. This application allows users to reserve desks in office spaces with email notifications and automated status management.

## Architecture

The project follows **Clean Architecture** patterns with clear separation of concerns:

- **DeskReservationApp.Domain** - Core business entities and interfaces
- **DeskReservationApp.Application** - Business logic and use cases
- **DeskReservationApp.Infrastructure** - Data access, external services, and implementations
- **DeskReservationApp.API** - Web API controllers and presentation layer

## Features

### Core Functionality
- **User Management** - Registration, authentication with JWT tokens
- **Role-Based Access Control** - Admin, TeamLead, and User roles
- **Floor Management** - Create and manage office floors
- **Desk Management** - Add, update, and track desk availability
- **Reservation System** - Book desks with time slot validation
- **Email Notifications** - Automatic confirmation emails for reservations
- **Background Services** - Automated reservation status updates

### Technical Features
- **JWT Authentication** - Secure token-based authentication
- **Email Service** - SMTP integration with development mode fallback
- **Exception Handling** - Global exception middleware
- **Dual Database Design** - Separate auth and business databases
- **Entity Framework Core** - Code-first approach with migrations
- **AutoMapper** - Object-to-object mapping
- **Swagger/OpenAPI** - Interactive API documentation
- **Background Tasks** - Automated reservation lifecycle management

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity + JWT
- **Email**: System.Net.Mail (SMTP)
- **Documentation**: Swagger/OpenAPI
- **Mapping**: AutoMapper
- **Architecture**: Clean Architecture + Repository Pattern + Unit of Work

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## Quick Start

### 1. Clone the Repository
```bash
git clone <repository-url>
cd desk-reservation-app-backend
```

### 2. Configure Database Connection
Update connection strings in `DeskReservationApp.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DeskReservationAuthConnectionString": "Server=(localdb)\\mssqllocaldb;Database=DeskReservationAuth;Trusted_Connection=True;TrustServerCertificate=True",
    "DeskReservationConnectionString": "Server=(localdb)\\mssqllocaldb;Database=DeskReservation;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Configure Email Settings (Optional)
For email notifications, update the `MailSettings` section:

```json
{
  "MailSettings": {
    "Mail": "your-email@example.com",
    "DisplayName": "Desk Reservation App",
    "Password": "your-app-password",
    "Host": "smtp.gmail.com",
    "Port": 587
  }
}
```

**Note**: If email settings are not configured, the system will log emails to the console instead.

### 4. Run the Application
```bash
# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run the API
dotnet run --project DeskReservationApp.API
```

The application will start at `https://localhost:7xxx` (port may vary).

### 5. Access Swagger Documentation
Navigate to `https://localhost:7xxx/swagger` to explore the API endpoints.

## 🗄️ Database Setup

The application automatically applies migrations and seeds initial data on startup:

- **Auth Database**: Manages users, roles, and authentication
- **Business Database**: Handles floors, desks, and reservations
- **Initial Roles**: Admin, TeamLead, User roles are automatically created

## 📚 API Documentation

### Authentication Endpoints
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login

### Reservation Endpoints
- `GET /api/reservation` - Get all reservations
- `POST /api/reservation` - Create new reservation
- `PUT /api/reservation/{id}` - Update reservation
- `DELETE /api/reservation/{id}` - Cancel reservation
- `GET /api/reservation/user` - Get user's reservations

### Desk & Floor Management
- `GET /api/desk` - Get all desks
- `POST /api/desk` - Create new desk (Admin only)
- `GET /api/floor` - Get all floors
- `POST /api/floor` - Create new floor (Admin only)

### User & Role Management
- `GET /api/user` - Get users (Admin only)
- `POST /api/role/assign` - Assign role to user (Admin only)

## 🔐 Security

### JWT Configuration
JWT tokens are used for authentication with the following default settings:
- **Issuer**: DeskReservation
- **Audience**: DeskReservationClient
- **Token Lifetime**: 60 minutes
- **Signing Key**: Configured in appsettings.json (change for production!)

### Role-Based Authorization
- **Admin**: Full system access
- **TeamLead**: Desk and user management
- **User**: Personal reservations only

## ⚙️ Configuration

### Application Settings
Key configuration sections in `appsettings.json`:

```json
{
  "Jwt": {
    "Issuer": "DeskReservation",
    "Audience": "DeskReservationClient",
    "Key": "your-secret-key-here",
    "AccessTokenMinutes": 60
  },
  "ReservationStatus": {
    "BackgroundServiceIntervalMinutes": 5,
    "EnableBackgroundService": true,
    "GracePeriodMinutes": 15,
    "AutoActivateScheduledReservations": true,
    "AllowPastReservations": false,
    "MaxAdvanceReservationDays": 30
  }
}
```

## Background Services

The application includes automated background services:

- **Reservation Status Service**: Updates reservation statuses every 5 minutes
- **Auto-activation**: Moves scheduled reservations to active when start time arrives
- **Auto-completion**: Marks reservations as completed when end time passes

## 📧 Email Integration

The email service supports both development and production modes:

### Development Mode
- Logs emails to console when real SMTP settings are not configured
- Perfect for testing without email credentials

### Production Mode
- Sends real emails via SMTP when valid settings are provided
- Supports Gmail, Outlook, and other SMTP providers

## 🧪 Testing

### Manual Testing via Swagger
1. Start the application
2. Navigate to `/swagger`
3. Register a new user
4. Login to get JWT token
5. Use "Authorize" button to set the token
6. Test various endpoints

### Sample Test Flow
1. **Register**: Create a user account
2. **Login**: Get authentication token
3. **Create Floor**: Add a floor (if admin)
4. **Create Desk**: Add desks to the floor
5. **Make Reservation**: Book a desk
6. **Check Email**: Verify email notification in console

## Deployment

### Environment Variables
For production deployment, configure these environment variables:
- `ConnectionStrings__DeskReservationAuthConnectionString`
- `ConnectionStrings__DeskReservationConnectionString`
- `Jwt__Key`
- `MailSettings__Mail`
- `MailSettings__Password`

### Database Migration
The application automatically applies migrations on startup, but you can also run them manually:

```bash
# Auth database
dotnet ef database update --project DeskReservationApp.Infrastructure --startup-project DeskReservationApp.API --context DeskReservationAuthDbContext

# Business database
dotnet ef database update --project DeskReservationApp.Infrastructure --startup-project DeskReservationApp.API --context DeskReservationDbContext
```

## Project Structure

```
desk-reservation-app-backend/
├── DeskReservationApp.API/           # Web API layer
│   ├── Controllers/                  # API controllers
│   ├── Middleware/                   # Custom middleware
│   └── Program.cs                    # Application startup
├── DeskReservationApp.Application/   # Business logic layer
│   ├── DTOs/                         # Data transfer objects
│   ├── Interfaces/                   # Service contracts
│   ├── Services/                     # Business services
│   └── Mappings/                     # AutoMapper profiles
├── DeskReservationApp.Domain/        # Core domain layer
│   ├── Entities/                     # Domain entities
│   └── Interfaces/                   # Repository contracts
└── DeskReservationApp.Infrastructure/ # Infrastructure layer
    ├── Persistance/                  # Data access
    │   ├── Repositories/             # Repository implementations
    │   └── Configurations/           # Entity configurations
    └── Services/                     # External service implementations
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔍 Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Check SQL Server is running
   - Verify connection strings
   - Ensure databases exist or migrations are applied

2. **JWT Token Issues**
   - Verify JWT configuration in appsettings.json
   - Check token expiration time
   - Ensure proper Authorization header format

3. **Email Not Working**
   - Check SMTP settings
   - Verify email credentials
   - Check firewall/antivirus settings

4. **Build Errors**
   - Run `dotnet restore`
   - Check .NET version compatibility
   - Verify all NuGet packages are installed

### Debug Mode
To enable detailed logging, set the logging level in `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

## Support

For support and questions:
- Create an issue in the repository
- Check existing documentation
- Review the API documentation via Swagger

---
