# React Performance & Best Practices

## Purpose

Performance optimization guide for React 19 and Next.js applications. Follow these rules to eliminate common performance pitfalls and deliver fast, responsive UIs.

## 1. Eliminate Data Fetching Waterfalls

### Rule: Fetch data in parallel, never sequentially

```tsx
// ❌ Waterfall: each fetch waits for the previous
async function Page() {
  const user = await getUser();
  const posts = await getPosts(user.id);      // waits for user
  const comments = await getComments(posts);   // waits for posts
  return <Dashboard user={user} posts={posts} comments={comments} />;
}

// ✅ Parallel fetching
async function Page() {
  const user = await getUser();
  const [posts, notifications] = await Promise.all([
    getPosts(user.id),
    getNotifications(user.id),
  ]);
  return <Dashboard user={user} posts={posts} notifications={notifications} />;
}
```

### Rule: Use React Server Components for data fetching

```tsx
// ✅ Server Component fetches data — no client bundle cost
async function ProductList() {
  const products = await db.product.findMany();
  return (
    <ul>
      {products.map(p => <ProductCard key={p.id} product={p} />)}
    </ul>
  );
}
```

### Rule: Use Suspense boundaries to stream content

```tsx
// ✅ Show shell immediately, stream data as it resolves
export default function Page() {
  return (
    <main>
      <h1>Dashboard</h1>
      <Suspense fallback={<Skeleton />}>
        <SlowDataSection />
      </Suspense>
    </main>
  );
}
```

## 2. Bundle Size Optimization

### Rule: Prefer server components — only add "use client" when needed

Client components increase bundle size. Only use `"use client"` for:
- Event handlers (`onClick`, `onChange`, etc.)
- Browser APIs (`window`, `localStorage`, etc.)
- React hooks (`useState`, `useEffect`, `useRef`, etc.)
- Third-party client libraries

### Rule: Use dynamic imports for heavy components

```tsx
import dynamic from 'next/dynamic';

const HeavyChart = dynamic(() => import('@/components/Chart'), {
  loading: () => <ChartSkeleton />,
  ssr: false,  // skip SSR for browser-only components
});
```

### Rule: Audit bundle with `@next/bundle-analyzer`

```bash
ANALYZE=true npm run build
```

## 3. Server-Side Performance

### Rule: Cache expensive computations

```tsx
import { unstable_cache } from 'next/cache';

const getCachedProducts = unstable_cache(
  async () => db.product.findMany(),
  ['products'],
  { revalidate: 3600 }  // revalidate every hour
);
```

### Rule: Use `loading.tsx` for instant navigation

Every route segment should have a `loading.tsx` to show during navigation:

```tsx
// app/products/loading.tsx
export default function Loading() {
  return <ProductListSkeleton />;
}
```

## 4. Client-Side Data Fetching

### Rule: Use React 19 `use()` hook for promises

```tsx
'use client';
import { use } from 'react';

function UserProfile({ userPromise }: { userPromise: Promise<User> }) {
  const user = use(userPromise);
  return <div>{user.name}</div>;
}
```

### Rule: Prefer server actions over client-side API calls

```tsx
// ✅ Server action — no API route needed
async function createProduct(formData: FormData) {
  'use server';
  const name = formData.get('name') as string;
  await db.product.create({ data: { name } });
  revalidatePath('/products');
}
```

## 5. Re-render Optimization

### Rule: Lift state down, not up

Keep state as close to where it's used as possible. Only lift state when multiple siblings need it.

### Rule: Use `React.memo` sparingly and correctly

```tsx
// Only memo components that receive the same props frequently
const ExpensiveList = React.memo(function ExpensiveList({ items }: Props) {
  return items.map(item => <ExpensiveItem key={item.id} item={item} />);
});
```

### Rule: Stabilize references with `useCallback` and `useMemo`

Only when passing to memoized children or expensive computations:

```tsx
const handleClick = useCallback((id: string) => {
  setSelected(id);
}, []);

const sortedItems = useMemo(
  () => items.toSorted((a, b) => a.name.localeCompare(b.name)),
  [items]
);
```

## 6. Rendering Performance

### Rule: Use CSS for animations, not React state

```tsx
// ❌ Don't animate with useState — causes re-renders
const [x, setX] = useState(0);
useEffect(() => { /* requestAnimationFrame loop */ }, []);

// ✅ Use CSS transitions or Framer Motion
<div className="transition-transform duration-300 hover:scale-105" />
```

### Rule: Virtualize long lists

```tsx
import { useVirtualizer } from '@tanstack/react-virtual';

function VirtualList({ items }: { items: Item[] }) {
  const parentRef = useRef<HTMLDivElement>(null);
  const virtualizer = useVirtualizer({
    count: items.length,
    getScrollElement: () => parentRef.current,
    estimateSize: () => 50,
  });
  // render only visible items
}
```

## 7. React 19 Patterns

### Rule: No `forwardRef` — ref is a regular prop in React 19

```tsx
// ✅ React 19: ref is just a prop
function Input({ ref, ...props }: { ref?: React.Ref<HTMLInputElement> }) {
  return <input ref={ref} {...props} />;
}
```

### Rule: Use `useActionState` for form submissions

```tsx
'use client';
import { useActionState } from 'react';

function CreateForm() {
  const [state, formAction, isPending] = useActionState(createProduct, null);
  return (
    <form action={formAction}>
      <input name="name" required />
      <button disabled={isPending}>
        {isPending ? 'Creating...' : 'Create'}
      </button>
      {state?.error && <p className="text-red-500">{state.error}</p>}
    </form>
  );
}
```

### Rule: Use `useOptimisticState` for instant UI feedback

```tsx
const [optimisticItems, addOptimistic] = useOptimistic(items);

async function handleAdd(formData: FormData) {
  const name = formData.get('name') as string;
  addOptimistic([...optimisticItems, { id: 'temp', name }]);
  await createItem(formData);
}
```

## Checklist

- [ ] No data fetching waterfalls — use `Promise.all` or parallel Suspense
- [ ] `"use client"` only where absolutely required
- [ ] Heavy components use dynamic imports
- [ ] Long lists are virtualized
- [ ] CSS handles animations, not React state
- [ ] React 19 APIs: `use()`, `useActionState`, no `forwardRef`
- [ ] Server actions preferred over client API calls
- [ ] Bundle analyzed and optimized
