## Project Overview
This is a sample project demonstrating multiple authentication methods with a focus on JWT (JSON Web Tokens) and modern authentication approaches in .NET 8. The project serves as a practical example for implementing secure API authentication and authorization techniques.

## Features
- **JWT Authentication**: Implementation of JWT token-based authentication with customizable claims
- **Azure AD Integration**: Support for Azure Active Directory authentication
- **Custom Authentication**: Demonstration of a custom API key authentication handler
- **Policy-based Authorization**: Dynamic routing to appropriate authentication scheme based on token
- **Swagger Integration**: Configured with JWT authentication support for API testing

## Technologies
- .NET 8
- ASP.NET Core
- JWT (JSON Web Tokens)
- Azure AD (Azure Active Directory)
- Swagger/OpenAPI

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code

### Configuration
1. Clone the repository
2. Update the `appsettings.json` with your JWT and Azure AD settings:

3. Run the application

## Authentication Flow

### JWT Authentication Flow
1. Client sends login credentials to the `/Member` endpoint
2. Server validates credentials
3. Server generates a JWT token with user claims and configured expiration (5 minutes)
4. Token is returned to the client
5. Client includes token in subsequent requests in the Authorization header
6. Server validates the token and authorizes access to protected resources

### Authentication Scheme Selection
The application implements a policy scheme that intelligently selects the appropriate authentication method:
- Examines the JWT token issuer
- Routes to "Local_JWT_Scheme" for tokens issued by the application
- Routes to "AzureAD_Scheme" for Azure AD-issued tokens
- Falls back to "CustomToken" for API key authentication

## API Endpoints

### Authentication
- **POST /Member** - Login endpoint to obtain a JWT token

### Protected Resources
- **GET /Member** - Protected endpoint requiring authentication

## Security Considerations
- JWT tokens are configured with a 5-minute expiration time
- Token validation includes issuer, audience, lifetime, and signature validation
- Clock skew is set to zero to prevent token expiration delay

## Swagger Integration
The project includes Swagger UI with JWT authentication support for easy API testing. When running the application, navigate to `/swagger` to access the interactive API documentation.
