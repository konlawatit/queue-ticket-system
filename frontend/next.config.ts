import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // The create-next-app template turns on Cache Components; it is left off on purpose.
  // Every screen except IT 05-1 shows live queue data, so there is nothing worth
  // prerendering, and classic dynamic rendering keeps the code simpler (no <Suspense>
  // around each data read) and returns a real 404 status for an unknown ticket.
  turbopack: {
    rules: {
      "*.css": {
        loaders: ["@tailwindcss/turbopack"],
        as: "*.css",
      },
    },
  },
};

export default nextConfig;
