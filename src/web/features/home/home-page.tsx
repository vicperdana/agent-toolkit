"use client";

import { Heading } from "@/components/ui/heading";

export const HomePage = () => {
  return (
    <div className="mx-auto max-w-2xl h-screen flex flex-col justify-center gap-6">
      <Heading variant="h1">
        Agent Toolkit
      </Heading>
      <p className="text-lg text-muted-foreground text-balance">
        A full-stack starter template with .NET API backend and Next.js frontend,
        powered by GitHub Copilot agent skills.
      </p>
    </div>
  );
};
