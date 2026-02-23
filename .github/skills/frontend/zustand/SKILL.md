# Zustand State Management

## Purpose

Conventions for using Zustand for client-side state management. Zustand stores hold UI state only — no business logic, no server state.

## Core Rules

1. **Client-side only** — Zustand runs in the browser. Never import stores in Server Components.
2. **State-only stores** — Stores hold state and simple setters. Business logic lives in server actions or API calls.
3. **Decoupled actions** — Complex actions live outside the store as standalone functions.
4. **Atomic selectors** — Select the smallest piece of state needed to minimize re-renders.
5. **Stores over `useState`** — Use Zustand when state is shared across multiple components.

## Quick-Start Template

```tsx
// stores/use-cart-store.ts
import { create } from 'zustand';
import { subscribeWithSelector } from 'zustand/middleware';

interface CartItem {
  id: string;
  name: string;
  price: number;
  quantity: number;
}

interface CartState {
  items: CartItem[];
  isOpen: boolean;
}

interface CartActions {
  addItem: (item: Omit<CartItem, 'quantity'>) => void;
  removeItem: (id: string) => void;
  updateQuantity: (id: string, quantity: number) => void;
  toggleCart: () => void;
  clearCart: () => void;
}

export const useCartStore = create<CartState & CartActions>()(
  subscribeWithSelector((set) => ({
    // State
    items: [],
    isOpen: false,

    // Actions (simple state mutations only)
    addItem: (item) =>
      set((state) => {
        const existing = state.items.find((i) => i.id === item.id);
        if (existing) {
          return {
            items: state.items.map((i) =>
              i.id === item.id ? { ...i, quantity: i.quantity + 1 } : i
            ),
          };
        }
        return { items: [...state.items, { ...item, quantity: 1 }] };
      }),

    removeItem: (id) =>
      set((state) => ({
        items: state.items.filter((i) => i.id !== id),
      })),

    updateQuantity: (id, quantity) =>
      set((state) => ({
        items: state.items.map((i) =>
          i.id === id ? { ...i, quantity: Math.max(0, quantity) } : i
        ),
      })),

    toggleCart: () => set((state) => ({ isOpen: !state.isOpen })),
    clearCart: () => set({ items: [], isOpen: false }),
  }))
);
```

## Atomic Selectors

Select only what you need to prevent unnecessary re-renders:

```tsx
// ✅ Atomic selector — component only re-renders when `items` changes
function CartBadge() {
  const itemCount = useCartStore((state) => state.items.length);
  return <span className="badge">{itemCount}</span>;
}

// ✅ Atomic selector — only re-renders when `isOpen` changes
function CartDrawer() {
  const isOpen = useCartStore((state) => state.isOpen);
  const toggleCart = useCartStore((state) => state.toggleCart);
  if (!isOpen) return null;
  return <aside>...</aside>;
}

// ❌ Bad: selecting entire store causes re-render on ANY state change
function CartBadge() {
  const store = useCartStore();  // re-renders on every state change
  return <span>{store.items.length}</span>;
}
```

### Derived Values

For computed state, create a selector function:

```tsx
// ✅ Derived selector
const selectCartTotal = (state: CartState) =>
  state.items.reduce((sum, item) => sum + item.price * item.quantity, 0);

function CartTotal() {
  const total = useCartStore(selectCartTotal);
  return <span>${total.toFixed(2)}</span>;
}
```

## Decoupled Actions Pattern

Complex actions that involve API calls or business logic live OUTSIDE the store:

```tsx
// actions/cart-actions.ts
import { useCartStore } from '@/stores/use-cart-store';
import { api } from '@/lib/api/client';

export async function checkout() {
  const items = useCartStore.getState().items;

  // Business logic — NOT in the store
  if (items.length === 0) {
    throw new Error('Cart is empty');
  }

  const order = await api.post('/api/orders', { items });

  // Update store after successful API call
  useCartStore.getState().clearCart();

  return order;
}

export async function syncCartWithServer() {
  const serverCart = await api.get<CartItem[]>('/api/cart');
  useCartStore.setState({ items: serverCart });
}
```

**Rule:** `useCartStore.getState()` and `useCartStore.setState()` work outside React components. Use them in standalone action functions.

## Store vs useState Decision

| Scenario | Use |
|----------|-----|
| State used by one component only | `useState` |
| State shared across siblings | `useState` lifted to parent |
| State shared across distant components | Zustand store |
| State that persists across navigation | Zustand store |
| Server data (fetched from API) | Server Component or React Query |
| Form state | `useState` or `useActionState` |
| Global UI state (theme, sidebar, modals) | Zustand store |

## subscribeWithSelector

Use `subscribeWithSelector` middleware to react to specific state changes:

```tsx
// Subscribe to state changes outside React
const unsubscribe = useCartStore.subscribe(
  (state) => state.items.length,
  (count, prevCount) => {
    if (count > prevCount) {
      toast.success('Item added to cart');
    }
  }
);
```

## Functional Updates

Always use functional updates when new state depends on previous state:

```tsx
// ✅ Functional update — safe for concurrent updates
addItem: (item) =>
  set((state) => ({
    items: [...state.items, { ...item, quantity: 1 }],
  })),

// ❌ Bad: reading state outside `set` can cause race conditions
addItem: (item) => {
  const currentItems = useCartStore.getState().items;
  set({ items: [...currentItems, { ...item, quantity: 1 }] });
},
```

## File Conventions

```
stores/
├── use-cart-store.ts       # Cart UI state
├── use-ui-store.ts         # Global UI state (sidebar, theme, modals)
└── use-filter-store.ts     # Filter/search state
```

**Naming:** `use-{name}-store.ts` — always prefixed with `use-`, always suffixed with `-store`.

## Checklist

- [ ] Store file named `use-{name}-store.ts`
- [ ] `'use client'` not needed in store file (but required in components that use it)
- [ ] State interface and actions interface separated
- [ ] `subscribeWithSelector` middleware applied
- [ ] Atomic selectors used — never `useStore()` without a selector
- [ ] Complex actions decoupled into standalone functions
- [ ] Functional updates for state that depends on previous state
- [ ] No business logic in stores — only state + simple setters
- [ ] Server state managed separately (Server Components or React Query)
