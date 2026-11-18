import { defineConfig, devices } from "@playwright/test";
import { API_URL, E2E_CONNECTION_STRING, WEB_PORT, WEB_URL } from "./e2e/support/environment";

export default defineConfig({
  testDir: "./e2e",
  // All tests share one queue, so they must not run at the same time.
  fullyParallel: false,
  workers: 1,
  forbidOnly: !!process.env.CI,
  retries: 0,
  reporter: process.env.CI ? "github" : [["list"], ["html", { open: "never" }]],
  use: {
    baseURL: WEB_URL,
    trace: "retain-on-failure",
    screenshot: "only-on-failure",
  },
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],

  // Playwright starts both servers before the tests and stops them afterwards.
  // Prerequisite: the database container is running (docker compose up -d db).
  webServer: [
    {
      name: "api",
      // Release build: bin/Release is separate from bin/Debug, so this works even
      // while a development API (dotnet run) holds the Debug binaries open.
      command: `dotnet run --project ../backend/src/QueueSystem.Api -c Release --no-launch-profile --urls ${API_URL}`,
      url: `${API_URL}/health`,
      env: {
        ASPNETCORE_ENVIRONMENT: "Development",
        ConnectionStrings__QueueDb: E2E_CONNECTION_STRING,
        Database__MigrateOnStartup: "true",
      },
      reuseExistingServer: !process.env.CI,
      timeout: 180_000,
    },
    {
      name: "web",
      // Production build, so the tests run the same code that would be deployed.
      command: `npm run build && npm run start -- --port ${WEB_PORT}`,
      url: WEB_URL,
      env: { API_BASE_URL: API_URL },
      reuseExistingServer: !process.env.CI,
      timeout: 300_000,
    },
  ],
});
