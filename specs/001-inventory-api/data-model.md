# Data Model: Inventory API

## Domain entities

### Category

| Campo | Tipo lógico | Reglas |
|-------|-------------|--------|
| Id | Guid | Generado al crear |
| Name | string | Obligatorio, no blanco |
| Description | string | Opcional en request; vacío si null |
| IsActive | bool | true al crear |
| CreatedAtUtc | DateTime | UTC al crear |

Relación lógica: una categoría tiene muchos productos. No hay FK en SQL.
No se elimina si existen productos (`CategoryHasProductsException` → 409).

### Product

| Campo | Tipo lógico | Reglas |
|-------|-------------|--------|
| Id | Guid | Generado al crear |
| Name | string | Obligatorio |
| Description | string | Opcional |
| Sku | string | Obligatorio, único |
| Price | decimal | >= 0 |
| CategoryId | Guid | Obligatorio y existente |
| QuantityOnHand | int | 0 al crear; solo cambia vía movimientos |
| IsActive | bool | true al crear |
| CreatedAtUtc | DateTime | UTC |

No se elimina si tiene movimientos (`ProductHasMovementsException` → 409).

### InventoryMovement

| Campo | Tipo lógico | Reglas |
|-------|-------------|--------|
| Id | Guid | Generado al crear |
| ProductId | Guid | Obligatorio y existente |
| Type | MovementType | `In` o `Out` |
| Quantity | int | > 0 |
| OccurredAtUtc | DateTime | UTC al registrar |
| Notes | string | Opcional |

`Product.Apply(movement)` usa `StockAdjustment`: entrada suma; salida resta y lanza `InsufficientStockException` si el resultado sería negativo.

### MovementType

Enum: `In`, `Out`. En HTTP se envía como string `"In"` / `"Out"`.

## Persistence tables

Sin foreign keys. Primary keys en `id`. Índice único `ux_products_sku`.

```text
categories(id, name, description, is_active, created_at_utc)
products(id, name, description, sku, price, category_id, quantity_on_hand, is_active, created_at_utc)
inventory_movements(id, product_id, type, quantity, occurred_at_utc, notes)
```

`type` se guarda como entero del enum.

## Auth artifacts (no persistidos en tablas de negocio)

- Client id / secret: variables de entorno.
- JWT: issuer, audience, signing key, expires_in desde entorno.
