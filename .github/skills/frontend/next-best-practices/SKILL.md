# Next.js App Router Patterns

## Purpose

Patterns and conventions for Next.js 15+ App Router. Emphasizes React Server Components, async APIs, and proper `"use client"` boundary rules.

## File Conventions

The App Router uses file-based routing with special file names:

```
app/
├── layout.tsx          # Root layout (wraps all pages)
├── page.tsx            # Home page (/)
├── loading.tsx         # Loading UI (Suspense fallback)
├── error.tsx           # Error boundary
├── not-found.tsx       # 404 page
├── products/
│   ├── page.tsx        # /products
│   ├── [id]/
│   │   ├── page.tsx    # /products/:id
│   │   └── loading.tsx # Loading state for product detail
│   └── layout.tsx      # Shared layout for /products/*
└── api/
    └── health/
        └── route.ts    # API route: GET /api/health
```

**Rules:**
- `page.tsx` — Only file that makes a route publicly accessible.
- `layout.tsx` — Wraps child pages; does NOT re-render on navigation.
- `loading.tsx` — Automatic Suspense boundary for the route segment.
- `error.tsx` — Must be a Client Component (`"use client"`).
- Route groups `(group)/` organize without affecting URL structure.

## React Server Components (Default)

All components in the App Router are **Server Components by default**. They run on the server and send HTML to the client.

### What Server Components CAN do:
- `await` async data (database queries, API calls)
- Access server-only resources (filesystem, environment variables)
- Import server-only packages
- Render with zero client-side JavaScript

### What Server Components CANNOT do:
- Use React hooks (`useState`, `useEffect`, etc.)
- Use browser APIs (`window`, `document`, `localStorage`)
- Add event handlers (`onClick`, `onChange`, etc.)
- Use context providers

```tsx
// ✅ Server Component (default) — no directive needed
async function ProductList() {
  const products = await db.product.findMany();
  return (
    <ul>
      {products.map(p => (
        <li key={p.id}>{p.name} — ${p.price}</li>
      ))}
    </ul>
  );
}
```

## "use client" Boundary Rules

### Rule: Push "use client" as far down the tree as possible

```tsx
// ❌ Bad: entire page is a client component
'use client';
export default function ProductPage() {
  const [qty, setQty] = useState(1);
  return (
    <div>
      <h1>{product.name}</h1>     {/* Static — doesn't need client */}
      <p>{product.description}</p> {/* Static — doesn't need client */}
      <QuantityPicker qty={qty} onChange={setQty} />
    </div>
  );
}

// ✅ Good: only the interactive part is a client component
// page.tsx (Server Component)
export default async function ProductPage({ params }: Props) {
  const product = await getProduct(params.id);
  return (
    <div>
      <h1>{product.name}</h1>
      <p>{product.description}</p>
      <AddToCartButton productId={product.id} />  {/* Client Component */}
    </div>
  );
}

// AddToCartButton.tsx
'use client';
export function AddToCartButton({ productId }: { productId: string }) {
  const [qty, setQty] = useState(1);
  return <button onClick={() => addToCart(productId, qty)}>Add</button>;
}
```

### Rule: Client Components can render Server Components as children

```tsx
'use client';
function InteractiveWrapper({ children }: { children: React.ReactNode }) {
  const [isOpen, setIsOpen] = useState(false);
  return (
    <div>
      <button onClick={() => setIsOpen(!isOpen)}>Toggle</button>
      {isOpen && children}  {/* children can be Server Components */}
    </div>
  );
}
```

## Async APIs (Next.js 15+)

In Next.js 15+, dynamic APIs are async and must be `await`ed:

```tsx
// ✅ Next.js 15+: params and searchParams are Promises
export default async function ProductPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const product = await getProduct(id);
  return <ProductDetail product={product} />;
}

// ✅ Cookies and headers are async
import { cookies, headers } from 'next/headers';

export async function getSession() {
  const cookieStore = await cookies();
  return cookieStore.get('session')?.value;
}
```

## Error Boundaries

Error boundaries must be Client Components:

```tsx
// app/products/error.tsx
'use client';

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <div className="p-4 text-center">
      <h2 className="text-xl font-bold">Something went wrong</h2>
      <p className="text-gray-600">{error.message}</p>
      <button
        onClick={reset}
        className="mt-4 rounded bg-blue-600 px-4 py-2 text-white"
      >
        Try again
      </button>
    </div>
  );
}
```

## Route Handlers

API routes use `route.ts` with HTTP method exports:

```tsx
// app/api/products/route.ts
import { NextRequest, NextResponse } from 'next/server';

export async function GET(request: NextRequest) {
  const searchParams = request.nextUrl.searchParams;
  const category = searchParams.get('category');
  const products = await getProducts({ category });
  return NextResponse.json(products);
}

export async function POST(request: NextRequest) {
  const body = await request.json();
  const product = await createProduct(body);
  return NextResponse.json(product, { status: 201 });
}
```

## Metadata

Use the `metadata` export or `generateMetadata` for dynamic pages:

```tsx
// Static metadata
export const metadata: Metadata = {
  title: 'Products',
  description: 'Browse our product catalog',
};

// Dynamic metadata
export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const { id } = await params;
  const product = await getProduct(id);
  return {
    title: product.name,
    description: product.description,
    openGraph: { images: [product.imageUrl] },
  };
}
```

## Image & Font Optimization

### Images

```tsx
import Image from 'next/image';

<Image
  src="/hero.jpg"
  alt="Hero image"
  width={1200}
  height={600}
  priority            // LCP image — preload
  className="rounded"
/>
```

### Fonts

```tsx
// app/layout.tsx
import { Inter } from 'next/font/google';

const inter = Inter({ subsets: ['latin'] });

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" className={inter.className}>
      <body>{children}</body>
    </html>
  );
}
```

## Checklist

- [ ] Components are Server Components by default
- [ ] `"use client"` pushed as far down the tree as possible
- [ ] `params` and `searchParams` are `await`ed (Next.js 15+)
- [ ] `cookies()` and `headers()` are `await`ed
- [ ] Every route segment has a `loading.tsx`
- [ ] Error boundaries are Client Components
- [ ] `<Image>` used for all images with `priority` on LCP
- [ ] Metadata exported on every page
- [ ] Route handlers use typed `NextRequest`/`NextResponse`
