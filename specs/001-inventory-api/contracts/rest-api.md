# Contract: REST Inventory API

Base URL local Docker: `http://localhost:8088`

Auth: Bearer JWT salvo `POST /oauth/token` y `GET /health`.
Errores: ProblemDetails (RFC 7807) en inglés.
Rate limit: cabecera `Retry-After` en 429.

## POST /oauth/token

Consumes: `application/x-www-form-urlencoded`

| Campo | Requerido | Notas |
|-------|-----------|-------|
| grant_type | sí | Solo `client_credentials` |
| client_id | sí | De entorno |
| client_secret | sí | De entorno |

Response 200:

```json
{
  "access_token": "<jwt>",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

Códigos: 200, 400 (grant inválido), 401 (credenciales), 429.

## Categories — /api/categories

| Método | Ruta | Éxito | Body |
|--------|------|-------|------|
| POST | /api/categories | 201 | `{ "name", "description?" }` |
| GET | /api/categories | 200 | lista |
| GET | /api/categories/{categoryId} | 200 | ítem |
| PUT | /api/categories/{categoryId} | 200 | `{ "name", "description?" }` |
| DELETE | /api/categories/{categoryId} | 204 | — |

Response ítem:

```json
{
  "id": "uuid",
  "name": "string",
  "description": "string",
  "isActive": true,
  "createdAtUtc": "2026-10-29T00:00:00Z"
}
```

Códigos de negocio: 400 validación, 404 no encontrada, 409 tiene productos.

## Products — /api/products

| Método | Ruta | Éxito | Body / query |
|--------|------|-------|--------------|
| POST | /api/products | 201 | `{ "name", "description?", "sku", "price", "categoryId" }` |
| GET | /api/products | 200 | query `categoryId?` |
| GET | /api/products/{productId} | 200 | — |
| PUT | /api/products/{productId} | 200 | mismos campos que create |
| DELETE | /api/products/{productId} | 204 | — |

Response ítem incluye `quantityOnHand` (0 al crear).

Códigos: 400, 404, 409 (SKU duplicado o con movimientos).

## Inventory movements — /api/inventory-movements

| Método | Ruta | Éxito | Body |
|--------|------|-------|------|
| POST | /api/inventory-movements | 200 | `{ "productId", "type", "quantity", "notes?" }` |

`type`: `"In"` \| `"Out"`. Respuesta: `ProductResponse` actualizado.

Códigos: 400 (cantidad/tipo), 404 producto, 409 stock insuficiente.

## Health

`GET /health` — sin Bearer; fuera del cupo de negocio para smoke.

Detalle extendido de ejemplos: [`docs/api.md`](../../../docs/api.md).
