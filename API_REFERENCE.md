# Pizza API - Quick Reference

## API Endpoints

### Base URL

```
http://localhost:5000          # Development
http://<route-hostname>         # OpenShift
```

---

## Endpoints Summary

| Method | Endpoint           | Description      | Status Code |
| ------ | ------------------ | ---------------- | ----------- |
| GET    | `/health`          | API health check | 200         |
| POST   | `/api/pizzas`      | Create new pizza | 201         |
| GET    | `/api/pizzas`      | Get all pizzas   | 200         |
| GET    | `/api/pizzas/{id}` | Get pizza by ID  | 200 / 404   |
| PUT    | `/api/pizzas/{id}` | Update pizza     | 200 / 404   |
| DELETE | `/api/pizzas/{id}` | Delete pizza     | 204 / 404   |

---

## Detailed Endpoint Documentation

### 1. Health Check

**Endpoint**: `GET /health`

**Description**: Check if API is running and healthy

**Response** (200 OK):

```json
{
  "status": "healthy",
  "timestamp": "2026-03-29T07:24:41.7304265Z"
}
```

---

### 2. Create Pizza

**Endpoint**: `POST /api/pizzas`

**Description**: Create a new pizza

**Request Headers**:

```
Content-Type: application/json
```

**Request Body**:

```json
{
  "name": "Margherita",
  "price": 12.99,
  "description": "Fresh tomato, mozzarella and basil"
}
```

**Response** (201 Created):

```json
{
  "id": 1,
  "name": "Margherita",
  "price": 12.99,
  "description": "Fresh tomato, mozzarella and basil"
}
```

**Error Response** (400 Bad Request):

```json
{
  "error": "Pizza name cannot be empty."
}
```

---

### 3. Get All Pizzas

**Endpoint**: `GET /api/pizzas`

**Description**: Retrieve all pizzas in the database

**Response** (200 OK):

```json
[
  {
    "id": 1,
    "name": "Margherita",
    "price": 12.99,
    "description": "Fresh tomato, mozzarella and basil"
  },
  {
    "id": 2,
    "name": "Pepperoni",
    "price": 13.99,
    "description": "Pepperoni pizza"
  }
]
```

**Empty Response** (200 OK):

```json
[]
```

---

### 4. Get Pizza by ID

**Endpoint**: `GET /api/pizzas/{id}`

**Description**: Retrieve a specific pizza by its ID

**Path Parameters**:

- `id` (integer, required): Pizza ID

**Response** (200 OK):

```json
{
  "id": 1,
  "name": "Margherita",
  "price": 12.99,
  "description": "Fresh tomato, mozzarella and basil"
}
```

**Not Found Response** (404 Not Found):

```json
{
  "error": "Pizza with ID 999 not found"
}
```

---

### 5. Update Pizza

**Endpoint**: `PUT /api/pizzas/{id}`

**Description**: Update an existing pizza

**Path Parameters**:

- `id` (integer, required): Pizza ID to update

**Request Headers**:

```
Content-Type: application/json
```

**Request Body**:

```json
{
  "name": "Margherita Deluxe",
  "price": 14.99,
  "description": "Premium fresh tomato, mozzarella and basil"
}
```

**Response** (200 OK):

```json
{
  "id": 1,
  "name": "Margherita Deluxe",
  "price": 14.99,
  "description": "Premium fresh tomato, mozzarella and basil"
}
```

**Not Found Response** (404 Not Found):

```json
{
  "error": "Pizza with ID 999 not found"
}
```

**Validation Error** (400 Bad Request):

```json
{
  "error": "Pizza price cannot be negative."
}
```

---

### 6. Delete Pizza

**Endpoint**: `DELETE /api/pizzas/{id}`

**Description**: Delete a pizza

**Path Parameters**:

- `id` (integer, required): Pizza ID to delete

**Response** (204 No Content):

```
(empty response body)
```

**Not Found Response** (404 Not Found):

```json
{
  "error": "Pizza with ID 999 not found"
}
```

---

## Request/Response Examples

### Using curl (Linux/macOS/Windows with Git Bash)

```bash
# Health check
curl http://localhost:5000/health

# Create pizza
curl -X POST http://localhost:5000/api/pizzas \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Margherita",
    "price": 12.99,
    "description": "Fresh tomato, mozzarella and basil"
  }'

# Get all pizzas
curl http://localhost:5000/api/pizzas

# Get specific pizza
curl http://localhost:5000/api/pizzas/1

# Update pizza
curl -X PUT http://localhost:5000/api/pizzas/1 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Margherita Deluxe",
    "price": 14.99,
    "description": "Premium fresh tomato, mozzarella and basil"
  }'

# Delete pizza
curl -X DELETE http://localhost:5000/api/pizzas/1
```

### Using PowerShell

```powershell
# Health check
Invoke-WebRequest -Uri "http://localhost:5000/health" -Method Get

# Create pizza
$body = @{
  name = "Margherita"
  price = 12.99
  description = "Fresh tomato, mozzarella and basil"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5000/api/pizzas" `
  -Method Post `
  -ContentType "application/json" `
  -Body $body

# Get all pizzas
Invoke-WebRequest -Uri "http://localhost:5000/api/pizzas" -Method Get

# Get specific pizza
Invoke-WebRequest -Uri "http://localhost:5000/api/pizzas/1" -Method Get

# Update pizza
$updateBody = @{
  name = "Margherita Deluxe"
  price = 14.99
  description = "Premium fresh tomato, mozzarella and basil"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5000/api/pizzas/1" `
  -Method Put `
  -ContentType "application/json" `
  -Body $updateBody

# Delete pizza
Invoke-WebRequest -Uri "http://localhost:5000/api/pizzas/1" -Method Delete
```

### Using Postman

1. **Create Collection**: "Pizza API"
2. **Set Base URL**: Set collection variable `base_url` to `http://localhost:5000`
3. **Create Requests**:
   - **Health Check**
     - Method: GET
     - URL: `{{base_url}}/health`
   - **Create Pizza**
     - Method: POST
     - URL: `{{base_url}}/api/pizzas`
     - Headers: `Content-Type: application/json`
     - Body (raw JSON):
       ```json
       {
         "name": "Margherita",
         "price": 12.99,
         "description": "Fresh tomato, mozzarella and basil"
       }
       ```
   - **Get All Pizzas**
     - Method: GET
     - URL: `{{base_url}}/api/pizzas`
   - **Get Pizza**
     - Method: GET
     - URL: `{{base_url}}/api/pizzas/1`
   - **Update Pizza**
     - Method: PUT
     - URL: `{{base_url}}/api/pizzas/1`
     - Headers: `Content-Type: application/json`
     - Body (raw JSON):
       ```json
       {
         "name": "Margherita Deluxe",
         "price": 14.99,
         "description": "Premium fresh tomato, mozzarella and basil"
       }
       ```
   - **Delete Pizza**
     - Method: DELETE
     - URL: `{{base_url}}/api/pizzas/1`

---

## HTTP Status Codes

| Code | Meaning               | When Used                                |
| ---- | --------------------- | ---------------------------------------- |
| 200  | OK                    | Successful GET, PUT operations           |
| 201  | Created               | Successful POST operation                |
| 204  | No Content            | Successful DELETE operation              |
| 400  | Bad Request           | Validation errors (e.g., negative price) |
| 404  | Not Found             | Pizza ID doesn't exist                   |
| 500  | Internal Server Error | Unhandled server exceptions              |

---

## Data Model

### Pizza Entity

| Field         | Type    | Required             | Notes                               |
| ------------- | ------- | -------------------- | ----------------------------------- |
| `id`          | integer | Yes (auto-generated) | Unique identifier, starts from 1    |
| `name`        | string  | Yes                  | Max 200 characters, cannot be empty |
| `price`       | decimal | Yes                  | Must be >= 0, precision 10.2        |
| `description` | string  | Yes                  | Max 500 characters, cannot be empty |

### Constraints

- **ID**: Must be positive integer, auto-generated
- **Name**: Non-empty string, max 200 chars
- **Price**: Non-negative decimal (0 to 999999999.99)
- **Description**: Non-empty string, max 500 chars

---

## Common Workflows

### Create and Read

```bash
# 1. Create a pizza
curl -X POST http://localhost:5000/api/pizzas \
  -H "Content-Type: application/json" \
  -d '{"name":"Vegetarian","price":11.99,"description":"Vegetables"}'

# Response: {"id":1,"name":"Vegetarian",...}

# 2. Read it back
curl http://localhost:5000/api/pizzas/1
```

### Create, Update, and Delete

```bash
# 1. Create
curl -X POST http://localhost:5000/api/pizzas \
  -H "Content-Type: application/json" \
  -d '{"name":"BBQ","price":15.99,"description":"BBQ chicken"}'
# ID = 1

# 2. Update price
curl -X PUT http://localhost:5000/api/pizzas/1 \
  -H "Content-Type: application/json" \
  -d '{"name":"BBQ","price":16.99,"description":"BBQ chicken"}'

# 3. Delete
curl -X DELETE http://localhost:5000/api/pizzas/1
```

### Bulk Operations

```bash
# Get all pizzas
curl http://localhost:5000/api/pizzas

# Delete all by individual ID
for id in {1..10}; do
  curl -X DELETE http://localhost:5000/api/pizzas/$id
done
```

---

## Testing Notes

- **In-Memory Database**: Data persists within a single application instance; lost on restart
- **Concurrency**: Safe for concurrent requests; EF Core handles locking
- **Validation**: All inputs are validated; server returns 400 for invalid data
- **Error Messages**: Consistent error format with descriptive messages

---

## Performance

- **Response Time**: <100ms per request (in-memory database)
- **Throughput**: Tested with concurrent requests, handles multiple simultaneous operations
- **Database**: All data in-memory; suitable for development and testing only
- **Scalability**: Horizontally scalable via OpenShift replica configuration

---

## Swagger/OpenAPI Documentation

When running locally, access Swagger UI at:

```
http://localhost:5000/swagger
```

Swagger provides interactive documentation and allows testing endpoints directly from the UI.

---

## Troubleshooting

### Connection Refused

```
Error: Connection refused
```

**Solution**: Ensure API is running

```bash
cd src/MyContainerApp.API
dotnet run
```

### 404 on Request

```json
{ "error": "Pizza with ID 999 not found" }
```

**Solution**: Verify the Pizza ID exists

```bash
curl http://localhost:5000/api/pizzas  # List all IDs
```

### 400 - Bad Request

```json
{ "error": "Pizza price cannot be negative." }
```

**Solution**: Check request data for validation errors

### 500 - Internal Server Error

```json
{ "error": "An internal server error occurred" }
```

**Solution**: Check application logs

```bash
# View logs from running application
# Look for stack trace in console output
```

---

## Additional Resources

- [README.md](README.md) - Full documentation
- [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) - OpenShift deployment instructions
- [Source Code](src/) - Complete implementation
