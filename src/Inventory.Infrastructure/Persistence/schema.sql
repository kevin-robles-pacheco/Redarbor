CREATE TABLE IF NOT EXISTS categories (
    id uuid PRIMARY KEY,
    name text NOT NULL,
    description text NOT NULL,
    is_active boolean NOT NULL,
    created_at_utc timestamp with time zone NOT NULL
);
CREATE TABLE IF NOT EXISTS products (
    id uuid PRIMARY KEY,
    name text NOT NULL,
    description text NOT NULL,
    sku text NOT NULL,
    price numeric(18, 2) NOT NULL,
    category_id uuid NOT NULL,
    quantity_on_hand integer NOT NULL,
    is_active boolean NOT NULL,
    created_at_utc timestamp with time zone NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS ux_products_sku ON products (sku);
CREATE TABLE IF NOT EXISTS inventory_movements (
    id uuid PRIMARY KEY,
    product_id uuid NOT NULL,
    type integer NOT NULL,
    quantity integer NOT NULL,
    occurred_at_utc timestamp with time zone NOT NULL,
    notes text NOT NULL
);
