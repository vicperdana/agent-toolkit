# React Composition Patterns

## Purpose

Patterns for composable, maintainable React components. Avoid boolean variant props, prefer compound components, and leverage React 19 APIs.

## Rule 1: No Boolean Variant Props

Boolean props create unclear APIs and grow combinatorially. Use compound components or explicit variants instead.

```tsx
// ❌ Bad: boolean props create 2^n combinations
<Button primary large disabled loading withIcon />

// ✅ Good: explicit variants via CVA
<Button variant="primary" size="lg" disabled>
  <Spinner /> Loading...
</Button>
```

### Use Compound Components for Complex UI

```tsx
// ✅ Compound component pattern
<Card>
  <Card.Header>
    <Card.Title>Product Details</Card.Title>
    <Card.Description>View and edit product information</Card.Description>
  </Card.Header>
  <Card.Content>
    <ProductForm product={product} />
  </Card.Content>
  <Card.Footer>
    <Button variant="outline">Cancel</Button>
    <Button>Save</Button>
  </Card.Footer>
</Card>
```

Implementation:

```tsx
function Card({ className, children, ...props }: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div className={cn("rounded-lg border bg-card p-6 shadow-sm", className)} {...props}>
      {children}
    </div>
  );
}

function CardHeader({ className, children, ...props }: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div className={cn("flex flex-col space-y-1.5", className)} {...props}>
      {children}
    </div>
  );
}

Card.Header = CardHeader;
Card.Title = CardTitle;
Card.Description = CardDescription;
Card.Content = CardContent;
Card.Footer = CardFooter;
```

## Rule 2: Decouple State Implementation

Components should not care how state is managed. Accept values and callbacks via props.

```tsx
// ❌ Bad: component owns its own fetch logic
function UserList() {
  const [users, setUsers] = useState<User[]>([]);
  useEffect(() => { fetchUsers().then(setUsers); }, []);
  return <ul>{users.map(u => <li key={u.id}>{u.name}</li>)}</ul>;
}

// ✅ Good: component receives data, parent decides how to fetch
function UserList({ users }: { users: User[] }) {
  return <ul>{users.map(u => <li key={u.id}>{u.name}</li>)}</ul>;
}

// Parent fetches (could be server component, hook, store, etc.)
async function UsersPage() {
  const users = await getUsers();
  return <UserList users={users} />;
}
```

## Rule 3: Use Context for Cross-Cutting Concerns

Define a context interface, not a concrete implementation:

```tsx
// Define the interface
interface ThemeContextValue {
  theme: 'light' | 'dark';
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextValue | null>(null);

// Custom hook with null check
function useTheme() {
  const context = useContext(ThemeContext);
  if (!context) throw new Error('useTheme must be used within ThemeProvider');
  return context;
}

// Provider owns the implementation
function ThemeProvider({ children }: { children: React.ReactNode }) {
  const [theme, setTheme] = useState<'light' | 'dark'>('light');
  const toggleTheme = useCallback(
    () => setTheme(t => t === 'light' ? 'dark' : 'light'),
    []
  );
  return (
    <ThemeContext value={{ theme, toggleTheme }}>
      {children}
    </ThemeContext>
  );
}
```

## Rule 4: Lift State Up (Only When Needed)

State should live at the lowest common ancestor of components that need it.

```tsx
// ✅ State lives in the parent that coordinates children
function ProductPage() {
  const [selectedId, setSelectedId] = useState<string | null>(null);

  return (
    <div className="grid grid-cols-2 gap-4">
      <ProductList
        onSelect={setSelectedId}
        selectedId={selectedId}
      />
      <ProductDetail productId={selectedId} />
    </div>
  );
}
```

## Rule 5: Explicit Variants via CVA

Use `class-variance-authority` for component variants instead of conditional logic:

```tsx
import { cva, type VariantProps } from "class-variance-authority";

const alertVariants = cva(
  "relative w-full rounded-lg border p-4 text-sm",
  {
    variants: {
      variant: {
        default: "bg-background text-foreground",
        success: "border-green-200 bg-green-50 text-green-900",
        warning: "border-yellow-200 bg-yellow-50 text-yellow-900",
        error: "border-red-200 bg-red-50 text-red-900",
      },
    },
    defaultVariants: { variant: "default" },
  }
);

interface AlertProps
  extends React.HTMLAttributes<HTMLDivElement>,
    VariantProps<typeof alertVariants> {}

function Alert({ variant, className, ...props }: AlertProps) {
  return <div className={cn(alertVariants({ variant }), className)} {...props} />;
}
```

## Rule 6: Children Over Render Props

Prefer `children` for composition. Use render props only when the child needs data from the parent.

```tsx
// ✅ Prefer children
<Dialog>
  <Dialog.Trigger asChild>
    <Button>Open</Button>
  </Dialog.Trigger>
  <Dialog.Content>
    <h2>Are you sure?</h2>
  </Dialog.Content>
</Dialog>

// ✅ Render props only when child needs parent data
<Combobox>
  {({ isOpen, selectedItem }) => (
    <>
      <Combobox.Input />
      {isOpen && <Combobox.Options />}
      {selectedItem && <Badge>{selectedItem.label}</Badge>}
    </>
  )}
</Combobox>
```

## Rule 7: React 19 API Patterns

### No `forwardRef` — ref is a regular prop

```tsx
// ✅ React 19
function Input({ ref, className, ...props }: InputProps & { ref?: React.Ref<HTMLInputElement> }) {
  return <input ref={ref} className={cn("border rounded px-3 py-2", className)} {...props} />;
}
```

### `use()` hook for promises and context

```tsx
'use client';
import { use } from 'react';

function Comments({ commentsPromise }: { commentsPromise: Promise<Comment[]> }) {
  const comments = use(commentsPromise);
  return comments.map(c => <Comment key={c.id} comment={c} />);
}
```

### Context with `use()` instead of `useContext`

```tsx
// ✅ React 19: use() works with context too
function ThemeToggle() {
  const { theme, toggleTheme } = use(ThemeContext);
  return <button onClick={toggleTheme}>{theme}</button>;
}
```

## Checklist

- [ ] No boolean variant props — use CVA or compound components
- [ ] Components receive data via props, don't own fetch logic
- [ ] Context has a typed interface and null-check hook
- [ ] State lives at lowest common ancestor
- [ ] Variants use CVA, not conditional class strings
- [ ] `children` preferred over render props
- [ ] React 19: no `forwardRef`, use `use()` hook
