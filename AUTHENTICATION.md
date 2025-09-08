# Authentication

The Eltariff API uses a split namespace authentication model where different endpoints require different authentication methods based on the sensitivity and nature of the data being accessed.

## Authentication by Endpoint Type

### Public Endpoints
The following endpoints provide general information and publicly available tariff data without requiring authentication:

- `GET /info` - API implementation information
- `GET /tariffs` - Collection of publicly available tariffs without customer data

### Protected Endpoints (OAuth 2.0 Required)
Any API calls that involve **meteringPointIds (MPID)** or return customer-specific data require **OAuth 2.0** authentication:

- `POST /tariffs/search` - Search tariffs by meteringPointIds
- `GET /prices/{componentId}` - Price information for specific components
- `GET /tariffs/{id}` - Specific tariff details by ID

These endpoints require authentication because they can access sensitive customer data tied to specific metering points.

## OAuth 2.0 Implementation

The API implements **Bearer Token authentication** using JWT (JSON Web Tokens) as specified in the OpenAPI security scheme.

### Security Scheme
```yaml
BearerAuth:
  type: http
  scheme: bearer
  bearerFormat: JWT
```

### Authentication Flow

1. **Obtain Access Token**: Get a JWT access token from the identity provider server
   - The identity provider URL is available via the `/info` endpoint (`identityProviderUrl` field)
   - Example: `https://idp.gridcompany.se/oauth2/token`

2. **Include Bearer Token**: Add the token to the Authorization header for protected endpoints
   ```http
   Authorization: Bearer {jwt-token}
   ```

3. **Token Validation**: The API validates the JWT token for each request to protected endpoints

### Example Request
```http
POST /tariffs/search
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "meteringPointIds": ["735999109012345678"]
}
```

## Error Responses

Protected endpoints return standard HTTP authentication error responses:

- **401 Unauthorized**: Missing or invalid access token
- **403 Forbidden**: Valid token but insufficient permissions for the requested meteringPointIds

### Partial Authorization Failure

When making requests with an array of meteringPointIds (such as `POST /tariffs/search`), the API enforces an **all-or-nothing authorization policy**:

- If the requester has valid permissions for **all** meteringPointIds in the array, the request proceeds normally
- If the requester lacks permission for **any** meteringPointId in the array, the entire request fails with **403 Forbidden**
- No partial results are returned - the request either succeeds completely or fails completely

This security model ensures that unauthorized meteringPointIds cannot be used to probe for data existence or gain any information about meters the requester should not have access to.

#### Example Scenario
```http
POST /tariffs/search
Authorization: Bearer {valid-token}
Content-Type: application/json

{
  "meteringPointIds": [
    "735999109012345678",  // ✓ Authorized
    "735999109087654321",  // ✗ Not authorized  
    "735999109055555555"   // ✓ Authorized
  ]
}
```

**Response: 403 Forbidden** - Even though the requester has permission for 2 out of 3 meteringPointIds, the entire request is rejected due to the unauthorized meteringPointId.

## Additional Resources

- [OAuth 2.0 Specification (RFC 6749)](https://tools.ietf.org/html/rfc6749)
- [JSON Web Token (JWT) Specification (RFC 7519)](https://tools.ietf.org/html/rfc7519)

For implementation details and endpoint-specific requirements, consult the main API specification documentation.