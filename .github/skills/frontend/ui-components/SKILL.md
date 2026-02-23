# UI Component Guide

## Purpose

Decision framework and conventions for building UI components. Prioritizes reuse, consistency, and accessibility.

## Component Decision Flow

When you need a UI element, follow this order:

```
1. Does an existing component in the project handle this?
   → YES: Use it. Extend only if necessary.
   → NO: Continue.

2. Does shadcn/ui have this component?
   → YES: Install and customize it.
   → NO: Continue.

3. Build a new component from Base UI primitives.
   → Follow the conventions below.
```

**Rule: Never build from scratch what shadcn/ui already provides.**

## shadcn/ui Integration

shadcn/ui components are copied into the project (not installed as a dependency). They use Radix UI primitives under the hood.

### Installing a Component

```bash
npx shadcn@latest add button
npx shadcn@latest add dialog
npx shadcn@latest add dropdown-menu
```

This copies the component source into `src/components/ui/`.

### Customizing

Modify the copied source directly. shadcn/ui components are yours to own:

```tsx
// src/components/ui/button.tsx — customize variants here
const buttonVariants = cva(
  "inline-flex items-center justify-center rounded-md text-sm font-medium transition-colors",
  {
    variants: {
      variant: {
        default: "bg-primary text-primary-foreground hover:bg-primary/90",
        destructive: "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        outline: "border border-input bg-background hover:bg-accent",
        ghost: "hover:bg-accent hover:text-accent-foreground",
        link: "text-primary underline-offset-4 hover:underline",
      },
      size: {
        default: "h-10 px-4 py-2",
        sm: "h-9 rounded-md px-3",
        lg: "h-11 rounded-md px-8",
        icon: "h-10 w-10",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "default",
    },
  }
);
```

## Component Placement

```
src/components/
├── ui/           # Primitives — Button, Input, Dialog, Card
│                 # (shadcn/ui components live here)
├── blocks/       # Composed components — DataTable, PageHeader, Sidebar
│                 # (combine multiple ui/ primitives)
├── features/     # Page-specific components — ProductCard, OrderForm
│                 # (domain-aware, used in one feature)
└── layout/       # Layout components — AppShell, Navigation, Footer
```

### Placement Rules

| Category | Location | Reusable? | Domain-aware? |
|----------|----------|-----------|---------------|
| Primitives | `ui/` | ✅ Yes | ❌ No |
| Blocks | `blocks/` | ✅ Yes | ❌ No |
| Features | `features/` | ❌ No | ✅ Yes |
| Layout | `layout/` | ✅ Yes | ❌ No |

**Rule:** If a component references a domain entity (Product, Order, User), it belongs in `features/`, not `ui/` or `blocks/`.

## Tailwind CSS + cn() Utility

Use the `cn()` utility to merge Tailwind classes conditionally:

```tsx
import { cn } from "@/lib/utils";

interface CardProps {
  className?: string;
  variant?: "default" | "highlighted";
  children: React.ReactNode;
}

export function Card({ className, variant = "default", children }: CardProps) {
  return (
    <div
      className={cn(
        "rounded-lg border p-4 shadow-sm",
        variant === "highlighted" && "border-blue-500 bg-blue-50",
        className  // allow consumer overrides
      )}
    >
      {children}
    </div>
  );
}
```

**Rules:**
- Always accept `className` prop for consumer overrides.
- Use `cn()` (not template literals) for conditional classes.
- Put base classes first, conditionals second, `className` last.

## Building New Components

### Structure

```tsx
// src/components/ui/status-badge.tsx
import { cn } from "@/lib/utils";
import { cva, type VariantProps } from "class-variance-authority";

const statusBadgeVariants = cva(
  "inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold",
  {
    variants: {
      status: {
        success: "bg-green-100 text-green-800",
        warning: "bg-yellow-100 text-yellow-800",
        error: "bg-red-100 text-red-800",
        info: "bg-blue-100 text-blue-800",
      },
    },
    defaultVariants: {
      status: "info",
    },
  }
);

interface StatusBadgeProps
  extends React.HTMLAttributes<HTMLSpanElement>,
    VariantProps<typeof statusBadgeVariants> {
  label: string;
}

export function StatusBadge({ label, status, className, ...props }: StatusBadgeProps) {
  return (
    <span className={cn(statusBadgeVariants({ status }), className)} {...props}>
      {label}
    </span>
  );
}
```

### Naming Conventions

- **File names:** `kebab-case.tsx` (e.g., `status-badge.tsx`)
- **Component names:** `PascalCase` (e.g., `StatusBadge`)
- **Props interface:** `ComponentNameProps` (e.g., `StatusBadgeProps`)
- **Variant function:** `componentNameVariants` (e.g., `statusBadgeVariants`)

## Quality Checklist

- [ ] Follows component decision flow (existing → shadcn/ui → new)
- [ ] Placed in correct directory (`ui/`, `blocks/`, `features/`, `layout/`)
- [ ] Accepts `className` prop for override
- [ ] Uses `cn()` for class merging
- [ ] Uses CVA for variants (if applicable)
- [ ] Accessible — proper ARIA attributes and keyboard support
- [ ] Responsive — works on mobile and desktop
- [ ] No domain logic in `ui/` or `blocks/` components
- [ ] Follows naming conventions (kebab-case file, PascalCase component)
