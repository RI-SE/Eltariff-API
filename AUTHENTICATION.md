# Authentication

## Document Audience

This document is intended for two audiences:

- **Grid Company API Implementers**: Organizations implementing the Eltariff API to expose their tariff data
- **Service Supplier Integrators**: Third-party services that integrate with grid company APIs to access customer data

Sections are clearly marked to indicate which audience they target.

## Overview

The Eltariff API uses a split namespace authentication model where different endpoints require different authentication methods based on the sensitivity and nature of the data being accessed.

## Authentication by Endpoint Type

### Public Endpoints
The following endpoints provide general information and publicly available data without requiring authentication:

- `GET /info` - API implementation information
- `GET /tariffs` - Collection of publicly available tariffs without customer data
- `GET /prices/{componentId}` - Price information for a specific price component

### Protected Endpoints (OAuth 2.0 Required)
Any API calls that involve **meteringPointIds (MPID)** or return customer-specific data require **OAuth 2.0** authentication:

- `POST /tariffs/search` - Search tariffs by MPIDs (requires authentication to access customer-specific metering points)
- `GET /tariffs/{id}` - Specific tariff details by ID (requires authentication to access detailed tariff information)

These endpoints require authentication because they can access sensitive customer data tied to specific metering points.

## OAuth 2.0 Implementation

**Audience: Grid Company API Implementers & Service Supplier Integrators**

The API implements **OAuth 2.0 Client Credentials flow** with **Bearer Token authentication** using JWT (JSON Web Tokens) as specified in the OpenAPI security scheme.

### Security Scheme
```yaml
BearerAuth:
  type: http
  scheme: bearer
  bearerFormat: JWT
```

### Authentication Flow

The OAuth 2.0 Client Credentials flow involves the following steps:

1. **Obtain Access Token**: Exchange client credentials for a JWT access token from the identity provider
   - The identity provider URL is available via the `/info` endpoint (`identityProviderUrl` field)
   - Example token endpoint: `https://idp.gridcompany.se/realms/gridcompany/protocol/openid-connect/token`

   **Token Request:**
   ```http
   POST /realms/gridcompany/protocol/openid-connect/token
   Host: idp.gridcompany.se
   Content-Type: application/x-www-form-urlencoded

   grant_type=client_credentials&client_id={client-id}&client_secret={client-secret}
   ```

   **Token Response:**
   ```json
   {
     "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "token_type": "Bearer",
     "expires_in": 300
   }
   ```

2. **Include Bearer Token**: Add the token to the Authorization header for protected endpoints
   ```http
   Authorization: Bearer {access-token}
   ```

3. **Token Validation**: The API validates the JWT token for each request to protected endpoints

### Token Lifecycle

- **Expiration**: Access tokens are short-lived and typically expire within minutes (e.g., 300 seconds)
- **No Refresh Tokens**: The Client Credentials flow does not use refresh tokens
- **Token Renewal**: Request a new access token when:
  - The current token expires (before or after expiry based on `expires_in`)
  - The API returns a 401 Unauthorized error
  - Starting a new session
- **Best Practice**: Implement token caching and renewal logic to minimize token requests while ensuring valid tokens

### Example Request
```http
POST /tariffs/search
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "meteringPointIds": ["735999109012345678"]
}
```

**Note**: The `meteringPointIds` field uses the full term in API requests, even though this document abbreviates it as MPID.

## Error Responses

**Audience: Grid Company API Implementers & Service Supplier Integrators**

Protected endpoints return standard HTTP authentication error responses:

- **401 Unauthorized**: Missing or invalid access token (including expired tokens)
- **403 Forbidden**: Valid token but insufficient permissions for the requested MPIDs

### Partial Authorization Failure

When making requests with an array of MPIDs (such as `POST /tariffs/search`), the API enforces an **all-or-nothing authorization policy**:

- If the requester has valid permissions for **all** MPIDs in the array, the request proceeds normally
- If the requester lacks permission for **any** MPID in the array, the entire request fails with **403 Forbidden**
- No partial results are returned - the request either succeeds completely or fails completely

This security model ensures that unauthorized MPIDs cannot be used to probe for data existence or gain any information about meters the requester should not have access to.

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

**Response: 403 Forbidden** - Even though the requester has permission for 2 out of 3 MPIDs, the entire request is rejected due to the unauthorized MPID.

## Customer Consent and Client Credentials

**Audience: Service Supplier Integrators**

### Overview

Service suppliers obtain access to customer metering point data through a consent-based credential mechanism. Customers generate OAuth 2.0 client credentials through their grid company's customer portal and provide these credentials to service suppliers during service signup.

### Customer Credential Workflow

The typical flow for obtaining customer consent involves:

1. **Customer Generates Credentials**
   - The customer logs into their grid company's "My Pages" (or equivalent customer portal)
   - The customer navigates to the API access or third-party services section
   - The customer generates new OAuth 2.0 client credentials (client ID and client secret)
   - The credentials are automatically scoped to the customer's MPIDs
   - The customer may be able to name the credentials for future reference

2. **Customer Provides Credentials to Service Supplier**
   - During service signup, the service supplier should prompt the customer for their grid company client credentials
   - The customer submits the client ID and client secret through the service supplier's onboarding process
   - The service supplier stores these credentials securely to access data on the customer's behalf

3. **Service Supplier Uses Credentials**
   - The service supplier exchanges the client credentials for an access token at the grid company's identity provider
   - The resulting access token is scoped to only the customer's MPIDs
   - The service supplier includes this token in API requests to retrieve the customer's tariff information
   - The API validates that the token has permission to access the specific metering points requested

### Credential Revocation

Customers can revoke service supplier access at any time through their grid company's customer portal:

- The customer navigates to active API credentials or third-party services in "My Pages"
- The customer revokes or deletes the credentials previously provided to the service supplier
- Once revoked, any access tokens derived from those credentials will no longer be valid
- The service supplier will receive 401/403 errors when attempting to access data with revoked credentials

### Implementation Considerations for Service Suppliers

When implementing customer onboarding with this credential model:

- **Secure storage** - Store customer client credentials securely using industry-standard secret management
- **Per-customer credentials** - Each customer provides their own unique credentials from their grid company
- **Credential validation** - Validate credentials immediately during onboarding by attempting to obtain an access token
- **Error handling** - Implement proper error handling for revoked or expired credentials
- **Customer guidance** - Provide clear instructions to customers on how to obtain credentials from their grid company's portal

## Additional Resources

- [OAuth 2.0 Specification (RFC 6749)](https://tools.ietf.org/html/rfc6749)
- [JSON Web Token (JWT) Specification (RFC 7519)](https://tools.ietf.org/html/rfc7519)

For implementation details and endpoint-specific requirements, consult the main API specification documentation.