# Identity Credential Service

A .NET-based identity management service that provides user registration and JWT-based authentication capabilities. This service offers secure user credential management with password hashing and token-based authentication.

## ??? Architecture
s
The solution follows Clean Architecture principles with the following projects:

- **IdentityCredentialService.WebApi** - API layer with controllers and endpoints
- **IdentityCredentialService.BusinessLogic** - Business logic and services
- **IdentityCredentialService.Infrastructure** - Data access and external dependencies
- **IdentityCredentialService.Domain** - Domain entities and models
- **IdentityCredentialService.Tests** - Unit tests

## ??? Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, for containerized deployment)

## ?? Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/abdulhaleem7/IdentityCredentialService.git
cd IdentityCredentialService
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run the Application

#### Option A: Using .NET CLI
```bash
cd IdentityCredentialService.WebApi
dotnet run
```

#### Option B: Using Visual Studio
1. Open `IdentityCredentialService.sln` in Visual Studio
2. Set `IdentityCredentialService.WebApi` as the startup project
3. Press `F5` or click "Start Debugging"

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`

## ?? API Endpoints

### User Registration
```http
POST /api/Identity/register
Content-Type: application/json

{
    "FirstName": "John",
    "LastName": "Doe", 
    "Email": "john.doe@example.com",
    "Password": "StrongPassword123!"
}
```

**Response:**
```json
{
    "success": true,
    "data": "user-id-guid",
    "message": "User created successfully.",
    "statusCode": 200
}
```

### Issue Credential (Login)
```http
POST /api/Identity/issue-credential
Content-Type: application/json

{
    "Email": "john.doe@example.com",
    "Password": "StrongPassword123!"
}
```

**Response:**
```json
{
    "success": true,
    "data": {
        "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        "refreshToken": "refresh-token-string",
        "user": {
            "firstName": "John",
            "lastName": "Doe",
            "email": "john.doe@example.com"
        }
    },
    "message": "Credential issued successfully.",
    "statusCode": 200
}
```

## ?? Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Test Project
```bash
dotnet test IdentityCredentialService.Tests
```

## ?? Docker Deployment

### Build Docker Image
```bash
docker build -t identity-credential-service .
```

### Run Container
```bash
docker run -p 8080:8080 -p 8081:8081 identity-credential-service
```

The application will be available at `http://localhost:8080`

## ?? Configuration

### JWT Settings
The JWT configuration is stored in `appsettings.json`:

```json
{
  "Jwt": {
    "Issuer": "IssuerKey",
    "Audience": "AudienceKey",
    "PublicKey": "RSA-public-key",
    "secretKey": "RSA-private-key"
  }
}
```

### Database
The application uses Entity Framework Core with an in-memory database by default. For production, update the `Program.cs` to use a persistent database:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

## ?? Development

### Project Structure
```
IdentityCredentialService/
??? IdentityCredentialService.WebApi/          # Web API layer
?   ??? Controllers/                           # API controllers
?   ??? Program.cs                            # Application entry point
?   ??? appsettings.json                      # Configuration
??? IdentityCredentialService.BusinessLogic/   # Business logic layer
?   ??? Services/                             # Service implementations
?   ??? Dtos/                                 # Data transfer objects
??? IdentityCredentialService.Infrastructure/  # Infrastructure layer
?   ??? Context/                              # Database context
??? IdentityCredentialService.Domain/          # Domain layer
?   ??? Models/                               # Domain entities
??? IdentityCredentialService.Tests/           # Unit tests
    ??? UserServiceTests.cs                   # Service tests
```

### Adding New Features
1. Create domain models in `IdentityCredentialService.Domain`
2. Add business logic in `IdentityCredentialService.BusinessLogic`
3. Update database context in `IdentityCredentialService.Infrastructure`
4. Add API endpoints in `IdentityCredentialService.WebApi`
5. Write unit tests in `IdentityCredentialService.Tests`

## ?? Security Features

- **Password Hashing**: Uses BCrypt for secure password storage
- **JWT Authentication**: Implements RSA-based JWT tokens
- **Input Validation**: Comprehensive validation for all inputs
- **HTTPS**: Enforced HTTPS redirection
- **CORS**: Configurable cross-origin resource sharing

## ?? Testing

The solution includes comprehensive unit tests covering:
- User registration scenarios
- Credential issuance flows
- Input validation
- Error handling
- Security features

Test coverage includes:
- ? Valid user creation
- ? Input validation (required fields)
- ? Duplicate email prevention
- ? Password verification
- ? JWT token generation
- ? Error scenarios

## ?? Troubleshooting

### Common Issues

1. **Port Already in Use**
   ```bash
   # Check what's using the port
   netstat -ano | findstr :5001
   # Kill the process or change port in launchSettings.json
   ```

2. **Missing Dependencies**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Database Issues**
   - The app uses in-memory database, so data is lost on restart
   - For persistent storage, configure a proper database connection

4. **JWT Token Issues**
   - Ensure JWT configuration is properly set in `appsettings.json`
   - Verify RSA keys are correctly formatted


**Happy Coding! ??**
