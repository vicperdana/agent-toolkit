# Web Project Conventions

## Purpose

Conventions for the Next.js frontend application at `src/web/`. Covers file organization, naming, imports, and patterns.

## Project Structure

Feature-based organization — code is grouped by feature, not by type:

```
src/web/
├── app/                        # Next.js App Router
│   ├── layout.tsx
│   ├── page.tsx
│   ├── globals.css
│   ├── (auth)/                 # Route group: auth pages
│   │   ├── login/page.tsx
│   │   └── register/page.tsx
│   ├── products/
│   │   ├── page.tsx
│   │   ├── [id]/page.tsx
│   │   ├── loading.tsx
│   │   └── error.tsx
│   └── api/                    # API routes (if needed)
│       └── health/route.ts
├── components/
│   ├── ui/                     # Primitives (Button, Input, Card)
│   ├── blocks/                 # Composed (DataTable, PageHeader)
│   ├── features/               # Domain-specific (ProductCard, OrderForm)
│   └── layout/                 # Shell, Navigation, Footer
├── lib/
│   ├── api/                    # API client for backend
│   │   ├── client.ts           # Base fetch wrapper
│   │   ├── products.ts         # Product API functions
│   │   └── types.ts            # API response types
│   ├── utils.ts                # cn() and shared utilities
│   └── constants.ts            # App-wide constants
├── hooks/                      # Custom React hooks
├── stores/                     # Zustand stores
├── types/                      # Shared TypeScript types
└── actions/                    # Server actions
```

## Component Placement

| What | Where | Example |
|------|-------|---------|
| Primitive UI | `components/ui/` | `button.tsx`, `input.tsx` |
| Composed blocks | `components/blocks/` | `data-table.tsx`, `page-header.tsx` |
| Feature components | `components/features/` | `product-card.tsx`, `order-form.tsx` |
| Layout components | `components/layout/` | `app-shell.tsx`, `nav.tsx` |

**Rule:** Domain-aware components (those that reference Product, Order, etc.) go in `features/`, not `ui/` or `blocks/`.

## Naming Conventions

### Files

| Item | Convention | Example |
|------|-----------|---------|
| Components | `kebab-case.tsx` | `product-card.tsx` |
| Hooks | `kebab-case.ts` | `use-products.ts` |
| Utilities | `kebab-case.ts` | `format-currency.ts` |
| Types | `kebab-case.ts` | `product-types.ts` |
| Constants | `kebab-case.ts` | `api-routes.ts` |

### Exports

| Item | Convention | Example |
|------|-----------|---------|
| Components | `PascalCase` | `export function ProductCard()` |
| Hooks | `camelCase` with `use` prefix | `export function useProducts()` |
| Functions | `camelCase` | `export function formatCurrency()` |
| Constants | `UPPER_SNAKE_CASE` | `export const API_BASE_URL = ...` |
| Types/Interfaces | `PascalCase` | `export interface Product` |

## Import Conventions

### Rule: Prefer absolute `@/` imports

```tsx
// ✅ Absolute imports
import { Button } from "@/components/ui/button";
import { useProducts } from "@/hooks/use-products";
import { cn } from "@/lib/utils";
import type { Product } from "@/types/product-types";

// ❌ Relative imports (avoid except within the same feature)
import { Button } from "../../../components/ui/button";
```

**Exception:** Use relative imports within the same directory or immediate children:

```tsx
// ✅ OK: relative import within same feature
import { ProductImage } from "./product-image";
```

### Import Order

1. React / Next.js
2. Third-party libraries
3. `@/` project imports
4. Relative imports
5. Types (with `type` keyword)

```tsx
import { Suspense } from "react";
import Image from "next/image";

import { z } from "zod";

import { Button } from "@/components/ui/button";
import { getProducts } from "@/lib/api/products";
import { cn } from "@/lib/utils";

import { ProductImage } from "./product-image";

import type { Product } from "@/types/product-types";
```

## Server Actions Pattern

Server actions live in `src/web/actions/` and are the preferred way to mutate data:

```tsx
// actions/product-actions.ts
'use server';

import { revalidatePath } from "next/cache";
import { api } from "@/lib/api/client";

export async function createProduct(formData: FormData) {
  const name = formData.get("name") as string;
  const price = parseFloat(formData.get("price") as string);

  await api.post("/api/products", { name, price });
  revalidatePath("/products");
}

export async function deleteProduct(id: string) {
  await api.delete(`/api/products/${id}`);
  revalidatePath("/products");
}
```

**Rules:**
- Always add `'use server'` directive at the top of the file.
- Call `revalidatePath()` or `revalidateTag()` after mutations.
- Validate inputs before sending to the API.
- Return structured error objects, not thrown exceptions.

## API Client at lib/api/

### Base Client

```tsx
// lib/api/client.ts
const API_BASE = process.env.API_URL ?? "http://localhost:5001";

async function fetchApi<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...options?.headers,
    },
  });

  if (!res.ok) {
    throw new Error(`API error: ${res.status} ${res.statusText}`);
  }

  return res.json();
}

export const api = {
  get: <T>(path: string) => fetchApi<T>(path),
  post: <T>(path: string, body: unknown) =>
    fetchApi<T>(path, { method: "POST", body: JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) =>
    fetchApi<T>(path, { method: "PUT", body: JSON.stringify(body) }),
  delete: <T>(path: string) =>
    fetchApi<T>(path, { method: "DELETE" }),
};
```

### Resource-specific functions

```tsx
// lib/api/products.ts
import { api } from "./client";
import type { Product, CreateProductRequest } from "./types";

export async function getProducts(): Promise<Product[]> {
  return api.get<Product[]>("/api/products");
}

export async function getProduct(id: string): Promise<Product> {
  return api.get<Product>(`/api/products/${id}`);
}

export async function createProduct(data: CreateProductRequest): Promise<Product> {
  return api.post<Product>("/api/products", data);
}
```

## Checklist

- [ ] Feature-based file organization
- [ ] Components placed in correct directory (ui/blocks/features/layout)
- [ ] File names: `kebab-case.tsx`
- [ ] Component exports: `PascalCase`
- [ ] Uses `@/` absolute imports (not deep relative paths)
- [ ] Server actions in `actions/` with `'use server'` directive
- [ ] API client functions in `lib/api/`
- [ ] Import order: React → 3rd party → @/ → relative → types
