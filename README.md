# Pizza API - Enterprise .NET Core Web Application

A production-grade CRUD API for managing pizzas, built with .NET 8, following Domain-Driven Design (DDD) principles, and containerized for OpenShift deployment.

## Architecture Overview

### Layered Architecture (Domain-Driven Design)

```
MyContainerApp.API              [Presentation Layer - HTTP Endpoints]
    ↓
MyContainerApp.Application      [Application Layer - Use Cases & DTOs]
    ↓
MyContainerApp.Infrastructure   [Infrastructure Layer - Data Access & DI]
    ↓
MyContainerApp.Domain           [Domain Layer - Business Logic (No External Dependencies)]
```

### Project Structure

```
MyContainerApp/
├── src/
│   ├── MyContainerApp.Domain/
│   │   ├── Aggregates/Pizza/        # Pizza entity (aggregate root)
│   │   ├── ValueObjects/            # PizzaId value object
│   │   └── Repositories/            # IPizzaRepository interface
│   │
│   ├── MyContainerApp.Application/
│   │   ├── DTOs/                    # Data transfer objects
│   │   ├── Mappings/                # AutoMapper profiles
│   │   └── Services/                # Application service (use case orchestration)
│   │
│   ├── MyContainerApp.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs           # EF Core DbContext
│   │   │   └── Repositories/PizzaRepository.cs   # Repository implementation
│   │   └── DependencyInjectionExtensions.cs      # Service registration
│   │
│   └── MyContainerApp.API/
│       ├── Controllers/             # REST API endpoints
│       ├── Middleware/              # Exception handling
│       ├── Program.cs               # ASP.NET Core configuration
│       └── appsettings.*.json       # Configuration files
│
└── Deployment/
    └── openshift/
        ├── 01-buildconfig.yaml      # OpenShift build configuration
        ├── 02-imagestream.yaml      # Image stream for Docker image
        ├── 03-deploymentconfig.yaml # Pod deployment configuration
        ├── 04-service.yaml          # Kubernetes service
        ├── 05-route.yaml            # OpenShift route (HTTP access)
        └── 06-configmap.yaml        # Configuration management
```

## Technology Stack

- **Framework**: .NET 8 LTS
- **Database**: Entity Framework Core with In-Memory Provider
- **API**: ASP.NET Core Web API
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **Container**: Docker (Multi-stage build)
- **Orchestration**: OpenShift/Kubernetes

## Features

### Core CRUD Operations

- **CREATE**: POST `/api/pizzas` - Create a new pizza
- **READ**: GET `/api/pizzas/{id}` - Retrieve a pizza by ID
- **READ**: GET `/api/pizzas` - Retrieve all pizzas
- **UPDATE**: PUT `/api/pizzas/{id}` - Update a pizza
- **DELETE**: DELETE `/api/pizzas/{id}` - Delete a pizza

### Pizza Model

```json
{
  "id": 1,
  "name": "Margherita",
  "price": 12.99,
  "description": "Classic pizza with tomato, mozzarella, and basil"
}
```

### Health Check

- **Endpoint**: GET `/health` - API health status
- **Response**: `{ "status": "healthy", "timestamp": "2026-03-29T10:30:00Z" }`

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker (for containerization)
- Docker Desktop or OpenShift cluster (for deployment)

### Local Development

#### 1. Build the Solution

```bash
cd MyContainerApp
dotnet build
```

#### 2. Run the API Locally

```bash
cd src/MyContainerApp.API
dotnet run
```

The API will start at `http://localhost:5000` (HTTPS) or `http://localhost:5000` (HTTP in development).

#### 3. Access Swagger Documentation

Navigate to `http://localhost:5000/swagger` in your browser to explore the API with Swagger UI.

#### 4. Test CRUD Operations

**Create a pizza:**

```bash
curl -X POST http://localhost:5000/api/pizzas \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Pepperoni",
    "price": 13.99,
    "description": "Spicy pepperoni pizza"
  }'
```

**Get all pizzas:**

```bash
curl http://localhost:5000/api/pizzas
```

**Get a specific pizza:**

```bash
curl http://localhost:5000/api/pizzas/1
```

**Update a pizza:**

```bash
curl -X PUT http://localhost:5000/api/pizzas/1 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Pepperoni Deluxe",
    "price": 14.99,
    "description": "Extra spicy pepperoni pizza"
  }'
```

**Delete a pizza:**

```bash
curl -X DELETE http://localhost:5000/api/pizzas/1
```

## Docker Deployment

### Build Docker Image

```bash
docker build -t pizza-api:latest .
```

### Run Docker Container Locally

```bash
docker run -p 8080:8080 --name pizza-api pizza-api:latest
```

Test the API at `http://localhost:8080/api/pizzas`

### Push to Container Registry

```bash
# Tag image for your registry
docker tag pizza-api:latest your-registry/pizza-api:latest

# Push to registry
docker push your-registry/pizza-api:latest
```

## OpenShift Deployment

### Prerequisites

- Access to an OpenShift cluster
- `oc` CLI installed and authenticated
- Git repository with this source code

### Deployment Steps

#### 1. Create Namespace (Optional)

```bash
oc create namespace pizza-app
oc project pizza-app
```

#### 2. Update BuildConfig with Your Repository

Edit `Deployment/openshift/01-buildconfig.yaml` and replace the Git URI:

```yaml
git:
  uri: https://github.com/your-org/your-repo.git
  ref: main
```

#### 3. Apply OpenShift Manifests

```bash
# Apply all manifests in order
oc apply -f Deployment/openshift/

# Or apply individually:
oc apply -f Deployment/openshift/01-buildconfig.yaml
oc apply -f Deployment/openshift/02-imagestream.yaml
oc apply -f Deployment/openshift/03-deploymentconfig.yaml
oc apply -f Deployment/openshift/04-service.yaml
oc apply -f Deployment/openshift/05-route.yaml
oc apply -f Deployment/openshift/06-configmap.yaml
```

#### 4. Manually Trigger Build (Manual Trigger is Enabled)

```bash
oc start-build pizza-api
```

Monitor build progress:

```bash
oc logs -f bc/pizza-api
```

#### 5. Verify Deployment

```bash
# Check deployment status
oc get dc

# Check pods
oc get pods -l app=pizza-api

# Get route URL
oc get routes
```

#### 6. Access the API

Get the route hostname:

```bash
oc get route pizza-api -o jsonpath='{.spec.host}'
```

Example: `http://pizza-api-default.apps.openshift.example.com`

Test the API:

```bash
curl http://pizza-api-default.apps.openshift.example.com/api/pizzas
```

## Configuration Management

### Application Settings

- **Local**: `src/MyContainerApp.API/appsettings.json` (Development)
- **Local**: `src/MyContainerApp.API/appsettings.Production.json` (Production)
- **OpenShift**: `Deployment/openshift/06-configmap.yaml` (ConfigMap)

### Environment Variables in OpenShift

The `DeploymentConfig` references ConfigMap for:

- `LOG_LEVEL` - Logging level

You can add more ConfigMap entries and reference them in `DeploymentConfig` as needed.

## Logging

Serilog is configured to log to:

1. **Console** - Real-time stdout
2. **File** - Rolling daily log files in `/app/logs/`

Configure log level in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Health Checks

### Endpoint

- **Path**: `/health`
- **Method**: GET
- **Response**: `200 OK` with JSON body

Used by OpenShift for:

- **Liveness Probe** - Determines if pod should be restarted
- **Readiness Probe** - Determines if pod should receive traffic

## API Error Handling

All API errors follow a consistent format:

```json
{
  "error": "Error description",
  "details": "Additional details (if applicable)"
}
```

HTTP Status Codes:

- `200 OK` - Successful GET/PUT operations
- `201 Created` - Successful POST operation (returns Location header)
- `204 No Content` - Successful DELETE operation
- `400 Bad Request` - Validation errors
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Unhandled exceptions

## Performance Considerations

### In-Memory Database

- Data is lost when the application restarts
- Suitable for development and testing
- Not suitable for production with persistent data requirements
- For persistence, replace `InMemoryDatabase` with a real database provider

### Resource Allocation

OpenShift DeploymentConfig specifies:

- **Requests**: CPU 250m, Memory 512Mi (guaranteed minimum)
- **Limits**: CPU 500m, Memory 1Gi (hard ceiling)

Adjust based on your workload patterns.

## Troubleshooting

### Build Failures

```bash
# View build logs
oc logs -f bc/pizza-api

# View last failed build
oc describe build pizza-api-1
```

### Deployment Issues

```bash
# View pod logs
oc logs pod/pizza-api-xxxx

# Get pod details
oc describe pod/pizza-api-xxxx

# Check recent events
oc get events
```

### Access Issues

```bash
# Check service
oc get svc pizza-api

# Check route
oc get routes pizza-api

# Check network policies
oc get netpol
```

## Future Enhancements

1. **Database Persistence**
   - Replace in-memory database with SQL Server, PostgreSQL, or MySQL
   - Add EF Core migrations

2. **Advanced Features**
   - Add filtering, sorting, and pagination
   - Implement CQRS pattern for complex queries
   - Add domain/integration events

3. **Security**
   - Add JWT authentication and authorization
   - Implement role-based access control (RBAC)
   - Add API key management

4. **Monitoring**
   - Integrate with Prometheus for metrics
   - Add distributed tracing with Jaeger
   - Set up alerts with AlertManager

5. **Testing**
   - Add unit tests for domain entities
   - Add integration tests for repository
   - Add API endpoint tests

## License

This project is provided as a reference implementation.

## Support

For questions or issues, please refer to the troubleshooting section or contact your development team.
