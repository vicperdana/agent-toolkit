# E2E Testing with Playwright

## Purpose

End-to-end testing conventions using Playwright. Tests cover both the Next.js frontend (localhost:3000) and the .NET API backend (localhost:5001).

## Project Setup

```
e2e/
├── playwright.config.ts
├── tests/
│   ├── products.spec.ts        # Product feature E2E tests
│   ├── auth.spec.ts            # Authentication flows
│   └── api/
│       └── products.api.spec.ts  # API endpoint tests
├── fixtures/
│   └── test-data.ts            # Shared test data
└── pages/                      # Page Object Models
    ├── product-list.page.ts
    └── product-detail.page.ts
```

## Configuration

```typescript
// playwright.config.ts
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',

  use: {
    baseURL: 'http://localhost:3000',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'mobile',
      use: { ...devices['iPhone 14'] },
    },
  ],

  webServer: [
    {
      command: 'npm run dev',
      url: 'http://localhost:3000',
      reuseExistingServer: !process.env.CI,
    },
    {
      command: 'dotnet run --project src/Api',
      url: 'http://localhost:5001/api/health',
      reuseExistingServer: !process.env.CI,
    },
  ],
});
```

## Browser Automation

### Navigation

```typescript
import { test, expect } from '@playwright/test';

test('navigate to products page', async ({ page }) => {
  await page.goto('/products');
  await expect(page).toHaveURL('/products');
  await expect(page).toHaveTitle(/Products/);
});

test('navigate to product detail', async ({ page }) => {
  await page.goto('/products');
  await page.getByRole('link', { name: 'Widget Pro' }).click();
  await expect(page).toHaveURL(/\/products\/\d+/);
});
```

### Locator Strategy

Prefer accessible locators in this priority order:

```typescript
// 1. Role-based (best — accessible and resilient)
page.getByRole('button', { name: 'Add to Cart' });
page.getByRole('heading', { name: 'Products' });
page.getByRole('link', { name: 'View Details' });

// 2. Label/placeholder-based (good for form fields)
page.getByLabel('Product Name');
page.getByPlaceholder('Search products...');

// 3. Text-based (good for visible content)
page.getByText('No products found');

// 4. Test ID (last resort — when no accessible locator works)
page.getByTestId('product-grid');
```

**Rule:** Never use CSS selectors (`page.locator('.btn-primary')`) or XPath. Always use accessible locators.

## Form Filling

```typescript
test('create a new product', async ({ page }) => {
  await page.goto('/products/new');

  // Fill form fields
  await page.getByLabel('Product Name').fill('New Widget');
  await page.getByLabel('Price').fill('29.99');
  await page.getByLabel('Description').fill('A fantastic new widget');
  await page.getByLabel('Category').selectOption('electronics');

  // Submit
  await page.getByRole('button', { name: 'Create Product' }).click();

  // Assert redirect and success
  await expect(page).toHaveURL('/products');
  await expect(page.getByText('Product created successfully')).toBeVisible();
});
```

### Complex Form Interactions

```typescript
test('multi-step checkout form', async ({ page }) => {
  await page.goto('/checkout');

  // Step 1: Shipping
  await page.getByLabel('Address').fill('123 Main St');
  await page.getByLabel('City').fill('Seattle');
  await page.getByRole('button', { name: 'Continue' }).click();

  // Step 2: Payment
  await expect(page.getByText('Payment Details')).toBeVisible();
  await page.getByLabel('Card Number').fill('4242424242424242');
  await page.getByRole('button', { name: 'Place Order' }).click();

  // Confirmation
  await expect(page.getByRole('heading', { name: 'Order Confirmed' })).toBeVisible();
});
```

## Assertions

```typescript
// Visibility
await expect(page.getByText('Welcome')).toBeVisible();
await expect(page.getByText('Error')).not.toBeVisible();

// Text content
await expect(page.getByRole('heading')).toHaveText('Products');
await expect(page.getByTestId('product-count')).toContainText('42 products');

// Element state
await expect(page.getByRole('button', { name: 'Submit' })).toBeEnabled();
await expect(page.getByRole('button', { name: 'Submit' })).toBeDisabled();

// Lists
const items = page.getByRole('listitem');
await expect(items).toHaveCount(5);

// URL and title
await expect(page).toHaveURL('/products');
await expect(page).toHaveTitle(/Products/);
```

## Screenshots

```typescript
// Full page screenshot
await page.screenshot({ path: 'screenshots/products.png', fullPage: true });

// Element screenshot
await page.getByTestId('product-grid').screenshot({ path: 'screenshots/grid.png' });

// Visual comparison (snapshot testing)
await expect(page).toHaveScreenshot('products-page.png');
```

## Network Mocking

Mock API responses for deterministic tests:

```typescript
test('display products from API', async ({ page }) => {
  // Mock the API response
  await page.route('**/api/products', (route) => {
    route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify([
        { id: 1, name: 'Widget', price: 9.99 },
        { id: 2, name: 'Gadget', price: 19.99 },
      ]),
    });
  });

  await page.goto('/products');
  await expect(page.getByRole('listitem')).toHaveCount(2);
});

test('handle API errors gracefully', async ({ page }) => {
  await page.route('**/api/products', (route) => {
    route.fulfill({ status: 500, body: 'Internal Server Error' });
  });

  await page.goto('/products');
  await expect(page.getByText('Something went wrong')).toBeVisible();
  await expect(page.getByRole('button', { name: 'Try again' })).toBeVisible();
});
```

### Intercept and modify requests

```typescript
test('add auth header to API calls', async ({ page }) => {
  await page.route('**/api/**', (route) => {
    route.continue({
      headers: {
        ...route.request().headers(),
        Authorization: 'Bearer test-token',
      },
    });
  });
});
```

## API Endpoint Testing

Test the .NET API directly without a browser:

```typescript
// tests/api/products.api.spec.ts
import { test, expect } from '@playwright/test';

const API_BASE = 'http://localhost:5001';

test.describe('Products API', () => {
  test('GET /api/products returns product list', async ({ request }) => {
    const response = await request.get(`${API_BASE}/api/products`);
    expect(response.ok()).toBeTruthy();
    expect(response.status()).toBe(200);

    const products = await response.json();
    expect(Array.isArray(products)).toBeTruthy();
  });

  test('POST /api/products creates a product', async ({ request }) => {
    const response = await request.post(`${API_BASE}/api/products`, {
      data: { name: 'Test Widget', price: 9.99 },
    });
    expect(response.status()).toBe(201);

    const product = await response.json();
    expect(product.name).toBe('Test Widget');
    expect(product.price).toBe(9.99);
  });

  test('GET /api/products/:id returns 404 for missing product', async ({ request }) => {
    const response = await request.get(`${API_BASE}/api/products/99999`);
    expect(response.status()).toBe(404);
  });
});
```

## Page Object Model

Encapsulate page interactions for reuse:

```typescript
// pages/product-list.page.ts
import { type Page, type Locator, expect } from '@playwright/test';

export class ProductListPage {
  readonly page: Page;
  readonly heading: Locator;
  readonly searchInput: Locator;
  readonly productCards: Locator;
  readonly createButton: Locator;

  constructor(page: Page) {
    this.page = page;
    this.heading = page.getByRole('heading', { name: 'Products' });
    this.searchInput = page.getByPlaceholder('Search products...');
    this.productCards = page.getByTestId('product-card');
    this.createButton = page.getByRole('link', { name: 'New Product' });
  }

  async goto() {
    await this.page.goto('/products');
    await expect(this.heading).toBeVisible();
  }

  async search(query: string) {
    await this.searchInput.fill(query);
    // wait for debounced search
    await this.page.waitForResponse('**/api/products?*');
  }

  async getProductCount() {
    return this.productCards.count();
  }
}
```

Usage:

```typescript
test('search filters products', async ({ page }) => {
  const productList = new ProductListPage(page);
  await productList.goto();

  await productList.search('Widget');
  expect(await productList.getProductCount()).toBeGreaterThan(0);
});
```

## Running Tests

```bash
# Run all tests
npx playwright test

# Run specific test file
npx playwright test tests/products.spec.ts

# Run with UI mode (interactive debugging)
npx playwright test --ui

# Run in headed mode (see the browser)
npx playwright test --headed

# Run only API tests
npx playwright test tests/api/

# Generate HTML report
npx playwright show-report
```

## Checklist

- [ ] Tests use accessible locators (role → label → text → testid)
- [ ] No CSS selectors or XPath
- [ ] API responses mocked for deterministic UI tests
- [ ] API endpoints tested directly via `request` fixture
- [ ] Page Object Model for reusable page interactions
- [ ] Screenshots captured on failure
- [ ] Both frontend (3000) and API (5001) configured in `webServer`
- [ ] Tests run in CI with `forbidOnly` and retries
