---
description: Development workflow and local setup instructions
---

# Development Workflow

```bash
# Terminal 1: Start .NET API
dotnet run --project src/Api     # http://localhost:5001

# Terminal 2: Start Next.js frontend
cd src/web && bun dev            # http://localhost:3000
```

## API Proxy

The Next.js frontend proxies `/api/*` requests to the .NET backend at `http://localhost:5001`. This is configured in `src/web/next.config.ts`.
