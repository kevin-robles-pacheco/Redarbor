# Documentación del API — Inventory

Referencia HTTP de la Inventory API (.NET 8). Contrato resumido también en [`specs/001-inventory-api/contracts/rest-api.md`](../specs/001-inventory-api/contracts/rest-api.md).

## General

| Ítem | Valor |
|------|-------|
| Base URL (Docker) | `http://localhost:8088` |
| Formato | JSON (`application/json`) salvo OAuth |
| Auth | `Authorization: Bearer <access_token>` |
| Errores | ProblemDetails (`application/problem+json`) en inglés |
| Docs interactivas | `GET /swagger` |

### Códigos comunes

| Código | Significado |
|--------|-------------|
| 200 | OK |
| 201 | Creado |
| 204 | Sin contenido (delete) |
| 400 | Validación / grant inválido |
| 401 | Credenciales o token inválidos |
| 404 | Recurso no encontrado |
| 409 | Conflicto de negocio (SKU, stock, dependencias) |
| 429 | Rate limit; cabecera `Retry-After` |
| 500 | Error inesperado |

### Rate limiting

| Política | Ámbito | Default | Partición |
|----------|--------|---------|-----------|
| `oauth` | `POST /oauth/token` | 10 / 60 s | IP |
| `api` | Endpoints `/api/*` | 100 / 60 s | `client_id` del JWT o IP |

Variables: `RATE_LIMIT_PERMIT_LIMIT`, `RATE_LIMIT_WINDOW_SECONDS`, `OAUTH_RATE_LIMIT_PERMIT_LIMIT`.

`GET /health` no entra en el cupo de negocio usado en el smoke.

---

## Autenticación

### POST /oauth/token

Público. `Content-Type: application/x-www-form-urlencoded`.

**Body**

| Campo | Descripción |
|-------|-------------|
| `grant_type` | Debe ser `client_credentials` |
| `client_id` | Valor de `OAUTH_CLIENT_ID` |
| `client_secret` | Valor de `OAUTH_CLIENT_SECRET` |

**Respuesta 200**

```json
{
  "access_token": "eyJhbGciOi...",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

**Errores**: 400 grant distinto; 401 secret/id inválidos; 429 cupo oauth.

**Ejemplo**

```bash
curl -s -X POST http://localhost:8088/oauth/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=inventory-api&client_secret=local-dev-only"
```

---

## Categorías

Prefijo: `/api/categories`. Requiere Bearer. Política `api`.

### POST /api/categories

Crea categoría.

**Request**

```json
{
  "name": "Beverages",
  "description": "Drinks"
}
```

`description` es opcional.

**Response 201** — cuerpo categoría + cabecera `Location`.

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Beverages",
  "description": "Drinks",
  "isActive": true,
  "createdAtUtc": "2026-10-29T13:00:00Z"
}
```

### GET /api/categories

Lista todas. **200** array de categorías.

### GET /api/categories/{categoryId}

**200** ítem. **404** si no existe.

### PUT /api/categories/{categoryId}

Mismo body que create. **200** ítem actualizado. **404** si no existe. **400** nombre inválido.

### DELETE /api/categories/{categoryId}

**204** si no tiene productos. **409** si tiene productos. **404** si no existe.

---

## Productos

Prefijo: `/api/products`. Requiere Bearer. Política `api`.

### POST /api/products

**Request**

```json
{
  "name": "Water",
  "description": "Still water",
  "sku": "WATER-1",
  "price": 1.5,
  "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response 201** — `quantityOnHand` inicia en `0`.

```json
{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "name": "Water",
  "description": "Still water",
  "sku": "WATER-1",
  "price": 1.5,
  "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "quantityOnHand": 0,
  "isActive": true,
  "createdAtUtc": "2026-10-29T13:05:00Z"
}
```

**Errores**: 400 validación; categoría inexistente → error de negocio mapeado; 409 SKU duplicado.

### GET /api/products

Query opcional: `categoryId` (Guid).

**200** array de productos.

### GET /api/products/{productId}

**200** / **404**.

### PUT /api/products/{productId}

Mismos campos que create. No altera stock por este endpoint. **409** si el SKU choca con otro producto.

### DELETE /api/products/{productId}

**204** sin movimientos. **409** si tiene movimientos. **404** si no existe.

---

## Movimientos de inventario

### POST /api/inventory-movements

Registra entrada o salida y devuelve el producto con stock actualizado.

**Request**

```json
{
  "productId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "type": "In",
  "quantity": 5,
  "notes": "Initial receipt"
}
```

| Campo | Reglas |
|-------|--------|
| `productId` | Guid existente |
| `type` | `"In"` o `"Out"` |
| `quantity` | Entero > 0 |
| `notes` | Opcional |

**Response 200** — `ProductResponse` con `quantityOnHand` nuevo.

**Errores**: 400 cantidad/tipo; 404 producto; 409 stock insuficiente en salida.

La persistencia inserta el movimiento y actualiza `quantity_on_hand` en la misma transacción Dapper.

---

## Salud

### GET /health

Sin autenticación. Comprueba que el proceso responde.

---

## Modelo de error (ProblemDetails)

Ejemplo 409:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Insufficient stock for the requested outbound movement.",
  "status": 409,
  "traceId": "00-..."
}
```

El `title` refleja el mensaje de la excepción de dominio o aplicación (inglés).

---

## Mapa excepción → HTTP

| Situación | HTTP |
|-----------|------|
| Categoría/producto no encontrado | 404 |
| Client credentials inválidas | 401 |
| SKU duplicado | 409 |
| Categoría con productos / producto con movimientos | 409 |
| Stock insuficiente | 409 |
| Nombre, SKU, precio, cantidad, tipo inválidos | 400 |
| Otros | 500 |
